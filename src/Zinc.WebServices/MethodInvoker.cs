using Platinum;
using Platinum.Reflection;
using Platinum.Validation;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Zinc.WebServices.Journaling;

namespace Zinc.WebServices
{
    /// <summary>
    /// Wrapper around concrete method implementation: this pre/post pipeline
    /// needs to be constructed, so that a webservice may be available as a
    /// WCF service *and* a WebAPI service.
    /// </summary>
    /// <typeparam name="T">Implementation type.</typeparam>
    /// <typeparam name="Rq">Request message type.</typeparam>
    /// <typeparam name="Rp">Response message type.</typeparam>
    public class MethodInvoker<T, Rq, Rp> : IMethod<Rq, Rp>
        where T : IMethod<Rq, Rp>
    {
        /// <summary>
        /// Performs the 'common' workload before and after executing the underlying
        /// method.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <param name="request">Request message.</param>
        /// <returns>Response message.</returns>
        public async Task<Rp> RunAsync( ExecutionContext context, Rq request )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            #endregion


            /*
             * Where's my request? This can happen, when the user doesn't
             * provide a "{}" during the JSON post.
             */
            if ( request == null )
                throw new ZincException( ER.MethodInvoker_RequestIsNull );


            /*
             * TODO: from config
             */
            IExecutionJournal journal = new NullJournal();
            MethodLoggingConfiguration config = new MethodLoggingConfiguration() { Type = MethodLoggingType.PrePost, Request = true, Response = true };


            /*
             * Normalize WCF dates, so that they are ALL DateTimeKind = Utc
             */
            WalkFix( request );


            /*
             * Pre-log
             */
            if ( config.Type == MethodLoggingType.PrePost )
            {
                await journal.PreAsync( context, request );
            }


            /*
             * Validate
             */
            ValidationResult vr;

            try
            {
                vr = Validator.Validate<Rq>( request );
            }
            catch ( Exception ex )
            {
                var vex = new ZincException( ER.MethodInvoker_RequestValidate, ex, request.GetType().FullName );
                context.MomentEnd = DateTime.UtcNow;

                // TODO: journal
                if ( config.Type == MethodLoggingType.PrePost )
                    await journal.PostAsync( context, vex );
                else
                    await journal.FullAsync( context, request, vex );

                throw vex;
            }

            if ( vr.IsValid == false )
            {
                ActorAggregateException agex = new ActorAggregateException( vr.Errors );

                var vex = new ZincException( ER.MethodInvoker_RequestInvalid, agex, request.GetType().FullName );
                context.MomentEnd = DateTime.UtcNow;

                // TODO: journal
                if ( config.Type == MethodLoggingType.PrePost )
                    await journal.PostAsync( context, vex );
                else
                    await journal.FullAsync( context, request, vex );

                throw vex;
            }


            /*
             * 
             */
            T method = System.Activator.CreateInstance<T>();


            /*
             * Run
             */
            Rp response;

            try
            {
                response = await method.RunAsync( context, request );
            }
            catch ( ActorException ex )
            {
                context.MomentEnd = DateTime.UtcNow;
                
                // TODO: journal
                if ( config.Type == MethodLoggingType.PrePost )
                    await journal.PostAsync( context, ex );
                else
                    await journal.FullAsync( context, request, ex );

                throw;
            }
            catch ( Exception ex )
            {
                context.MomentEnd = DateTime.UtcNow;
                
                // TODO: journal
                if ( config.Type == MethodLoggingType.PrePost )
                    await journal.PostAsync( context, ex );
                else
                    await journal.FullAsync( context, request, ex );

                throw;
            }


            /*
             * We always expect a response: we should never accept a null response!
             */
            if ( response == null )
                throw new ZincException( ER.MethodInvoker_ResponseIsNull, typeof( T ).FullName );


            /*
             * 
             */
            WalkFix( response );


            /*
             * Validate
             */
            try
            {
                vr = Validator.Validate<Rp>( response );
            }
            catch ( Exception ex )
            {
                var vex = new ZincException( ER.MethodInvoker_ResponseValidate, ex, request.GetType().FullName );
                context.MomentEnd = DateTime.UtcNow;

                // TODO: journal
                // TODO: perhaps we could log response AND vex
                if ( config.Type == MethodLoggingType.PrePost )
                    await journal.PostAsync( context, vex );
                else
                    await journal.FullAsync( context, request, vex );

                throw vex;
            }

            if ( vr.IsValid == false )
            {
                ActorAggregateException agex = new ActorAggregateException( vr.Errors );

                var vex = new ZincException( ER.MethodInvoker_ResponseInvalid, agex, request.GetType().FullName );
                context.MomentEnd = DateTime.UtcNow;

                // TODO: journal
                // TODO: perhaps we could log response AND vex
                if ( config.Type == MethodLoggingType.PrePost )
                    await journal.PostAsync( context, vex );
                else
                    await journal.FullAsync( context, request, vex );

                throw vex;
            }


            /*
             * Post-log
             */
            context.MomentEnd = DateTime.UtcNow;

            if ( config.Type == MethodLoggingType.PrePost )
                await journal.PostAsync( context, response );
            else
                await journal.FullAsync( context, request, response );

            return response;
        }



        private static void WalkFix( object obj )
        {
            #region Validations

            if ( obj == null )
                throw new ArgumentNullException( nameof( obj ) );

            #endregion

            Type t = obj.GetType();

            foreach ( var prop in t.GetProperties() )
            {
                /*
                 * Array
                 */
                if ( prop.PropertyType.IsArray == true )
                {
                    Array arr = (Array) prop.GetValue( obj );

                    if ( arr == null )
                        continue;

                    if ( arr.Length == 0 )
                        continue;

                    var et = prop.PropertyType.GetElementType();

                    if ( et == typeof( DateTime ) )
                    {
                        XmlArrayItemAttribute attr = prop.GetCustomAttribute<XmlArrayItemAttribute>();

                        for ( int i = 0; i < arr.Length; i++ )
                        {
                            DateTime vx = (DateTime) arr.GetValue( i );
                            DateTime fx;

                            if ( DateTimeFix( attr?.DataType ?? "dateTime", vx, out fx ) == true )
                                arr.SetValue( fx, i );
                        }

                        continue;
                    }

                    if ( et.IsCustomClass() == true )
                    {
                        foreach ( var o in arr )
                            WalkFix( o );

                        continue;
                    }

                    continue;
                }


                /*
                 * DateTime?
                 */
                if ( prop.PropertyType == typeof( DateTime? ) )
                {
                    DateTime? v = (DateTime?) prop.GetValue( obj );
                    XmlElementAttribute attr = prop.GetCustomAttribute<XmlElementAttribute>();

                    DateTime f;

                    if ( v.HasValue == false )
                        continue;

                    if ( DateTimeFix( attr?.DataType ?? "dateTime", v.Value, out f ) == true )
                        prop.SetValue( obj, f );

                    continue;
                }


                /*
                 * DateTime
                 */
                if ( prop.PropertyType == typeof( DateTime ) )
                {
                    DateTime v = (DateTime) prop.GetValue( obj );
                    XmlElementAttribute attr = prop.GetCustomAttribute<XmlElementAttribute>();

                    DateTime f;

                    if ( DateTimeFix( attr?.DataType ?? "dateTime", v, out f ) == true )
                        prop.SetValue( obj, f );

                    continue;
                }


                /*
                 * Nested type
                 */
                if ( prop.PropertyType.IsCustomClass() == true )
                {
                    var deep = prop.GetValue( obj );

                    if ( deep == null )
                        continue;

                    WalkFix( deep );
                    continue;
                }
            }
        }


        /// <summary>
        /// Corrects the value of the <see cref="DateTime" /> property, so that we don't lose
        /// information over the wire.
        /// </summary>
        /// <param name="dataType">One of three XSD values: date, time, dateTime.</param>
        /// <param name="value">Current value.</param>
        /// <param name="fix">Fixed value.</param>
        /// <returns>Whether the value was modified, and should be replaced.</returns>
        private static bool DateTimeFix( string dataType, DateTime value, out DateTime fix )
        {
            #region Validations

            if ( dataType == null )
                throw new ArgumentNullException( nameof( dataType ) );

            #endregion

            bool fixIt = false;
            fix = value;


            /*
             * Magic values!
             */
            if ( value == DateTime.MinValue )
                return false;

            if ( value == DateTime.MaxValue )
                return false;


            /*
             * Kind checks
             */
            if ( fix.Kind == DateTimeKind.Local )
            {
                fix = fix.ToUniversalTime();
                fixIt = true;
            }
            else if ( fix.Kind == DateTimeKind.Unspecified )
            {
                fix = DateTime.SpecifyKind( fix, DateTimeKind.Utc );
                fixIt = true;
            }


            /*
             * Strip time from date.
             */
            if ( dataType == "date" && (fix.Hour > 0 || fix.Minute > 0 || fix.Second > 0 || fix.Millisecond > 0) )
            {
                fix = fix.Date;
                fixIt = true;
            }


            /*
             * 'Strip' date from time.
             */
            if ( dataType == "time" && (fix.Year != 1 || fix.Month != 1 || fix.Day != 1) )
            {
                fix = new DateTime( 1, 1, 1, fix.Hour, fix.Minute, fix.Second, fix.Millisecond, DateTimeKind.Utc );
                fixIt = true;
            }

            return fixIt;
        }
    }
}
