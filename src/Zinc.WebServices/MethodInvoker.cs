using Platinum;
using Platinum.Reflection;
using Platinum.Validation;
using System;
using System.Reflection;
using System.Xml.Serialization;
using Zinc.WebServices.Journaling;

namespace Zinc.WebServices
{
    public class MethodInvoker<T, Rq, Rp> : IMethod<Rq, Rp>
        where T : IMethod<Rq, Rp>
    {
        public Rp Run( ExecutionContext context, Rq request )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( request == null )
                throw new ArgumentNullException( nameof( request ) );

            #endregion


            /*
             * TODO: from config
             */
            IExecutionJournal journal = new SqlServerJournal();
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
                journal.Pre( context, request );
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
                var vex = new WsException( ER.MethodInvoker_RequestValidate, ex, request.GetType().FullName );

                // TODO: journal
                if ( config.Type == MethodLoggingType.PrePost )
                    journal.Post( context, vex );
                else
                    journal.Full( context, request, vex );

                throw vex;
            }

            if ( vr.IsValid == false )
            {
                ActorAggregateException agex = new ActorAggregateException( vr.Errors );

                var vex = new WsException( ER.MethodInvoker_RequestInvalid, agex, request.GetType().FullName );

                // TODO: journal
                if ( config.Type == MethodLoggingType.PrePost )
                    journal.Post( context, vex );
                else
                    journal.Full( context, request, vex );

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
                response = method.Run( context, request );
            }
            catch ( ActorException ex )
            {
                // TODO: journal
                if ( config.Type == MethodLoggingType.PrePost )
                    journal.Post( context, ex );
                else
                    journal.Full( context, request, ex );

                throw;
            }
            catch ( Exception ex )
            {
                // TODO: journal
                if ( config.Type == MethodLoggingType.PrePost )
                    journal.Post( context, ex );
                else
                    journal.Full( context, request, ex );

                throw;
            }


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
                var vex = new WsException( ER.MethodInvoker_ResponseValidate, ex, request.GetType().FullName );

                // TODO: journal
                // TODO: perhaps we could log response AND vex
                if ( config.Type == MethodLoggingType.PrePost )
                    journal.Post( context, vex );
                else
                    journal.Full( context, request, vex );

                throw vex;
            }

            if ( vr.IsValid == false )
            {
                ActorAggregateException agex = new ActorAggregateException( vr.Errors );

                var vex = new WsException( ER.MethodInvoker_ResponseInvalid, agex, request.GetType().FullName );

                // TODO: journal
                // TODO: perhaps we could log response AND vex
                if ( config.Type == MethodLoggingType.PrePost )
                    journal.Post( context, vex );
                else
                    journal.Full( context, request, vex );

                throw vex;
            }


            /*
             * Post-log
             */
            if ( config.Type == MethodLoggingType.PrePost )
                journal.Post( context, response );
            else
                journal.Full( context, request, response );

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
