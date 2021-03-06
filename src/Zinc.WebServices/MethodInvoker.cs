﻿using Platinum;
using Platinum.Reflection;
using Platinum.Validation;
using System;
using System.Linq;
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
             * 
             */
            JournalConfiguration config = ZincConfiguration.Current.Journaling;

            if ( config.To == null )
                throw new ZincException( ER.Journaling_NotConfigured );

            ZincJournal journalConfig = ZincConfiguration.Current.Journals.FirstOrDefault( j => j.Name == config.To );

            if ( journalConfig == null )
                throw new ZincException( ER.Journaling_JournalNotFound, config.To );

            IExecutionJournal journal;

            try
            {
                journal = Platinum.Activator.Create<IExecutionJournal>( journalConfig.Type );
            }
            catch ( ActorException ex )
            {
                throw new ZincException( ER.Journaling_InvalidMoniker, ex, config.To );
            }


            /*
             * Normalize WCF dates, so that they are ALL DateTimeKind = Utc
             * Once normalized, secrefy the message.
             */
            WalkFix( request );
            var jrequest = Secrets.Strip<Rq>( request );


            /*
             * Pre-log
             */
            if ( config.Type == MethodLoggingType.PrePost )
            {
                await journal.PreAsync( context, jrequest );
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
                context.MomentEnd = PreciseDateTime.UtcNow;

                if ( config.Type == MethodLoggingType.PrePost )
                    await journal.PostAsync( context, null, vex );
                else
                    await journal.FullAsync( context, jrequest, null, vex );

                throw vex;
            }

            if ( vr.IsValid == false )
            {
                ActorAggregateException agex = new ActorAggregateException( vr.Errors );

                var vex = new ZincException( ER.MethodInvoker_RequestInvalid, agex, request.GetType().FullName );
                context.MomentEnd = PreciseDateTime.UtcNow;

                if ( config.Type == MethodLoggingType.PrePost )
                    await journal.PostAsync( context, null, vex );
                else
                    await journal.FullAsync( context, jrequest, null, vex );

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
                context.MomentEnd = PreciseDateTime.UtcNow;

                if ( config.Type == MethodLoggingType.PrePost )
                    await journal.PostAsync( context, null, ex );
                else
                    await journal.FullAsync( context, jrequest, null, ex );

                throw;
            }
            catch ( AggregateException ex )
            {
                if ( ex.InnerExceptions.Count == 1 && ex.InnerExceptions[ 0 ] is ActorException )
                {
                    var aex = (ActorException) ex.InnerExceptions[ 0 ];

                    context.MomentEnd = PreciseDateTime.UtcNow;

                    if ( config.Type == MethodLoggingType.PrePost )
                        await journal.PostAsync( context, null, aex );
                    else
                        await journal.FullAsync( context, jrequest, null, aex );

                    // Yes, this will re-write exception stack :-(
                    throw aex;
                }
                else if ( ex.InnerExceptions.All( x => x is ActorException ) == true )
                {
                    ActorAggregateException agg = new ActorAggregateException( ex.InnerExceptions.Select( x => x as ActorException ) );
                    var uex = new ZincAggregateException( ex.InnerExceptions[ 0 ] as ActorException, agg );

                    context.MomentEnd = PreciseDateTime.UtcNow;

                    if ( config.Type == MethodLoggingType.PrePost )
                        await journal.PostAsync( context, null, uex );
                    else
                        await journal.FullAsync( context, jrequest, null, uex );

                    throw uex;
                }
                else
                {
                    var uex = new ZincException( ER.MethodInvoker_UnhandledException, ex, typeof( T ).FullName, ex.Message );

                    context.MomentEnd = PreciseDateTime.UtcNow;

                    if ( config.Type == MethodLoggingType.PrePost )
                        await journal.PostAsync( context, null, uex );
                    else
                        await journal.FullAsync( context, jrequest, null, uex );

                    throw uex;
                }
            }
            catch ( Exception ex )
            {
                var uex = new ZincException( ER.MethodInvoker_UnhandledException, ex, typeof( T ).FullName, ex.Message );

                context.MomentEnd = PreciseDateTime.UtcNow;

                if ( config.Type == MethodLoggingType.PrePost )
                    await journal.PostAsync( context, null, uex );
                else
                    await journal.FullAsync( context, jrequest, null, uex );

                throw uex;
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
            var jresponse = Secrets.Strip<Rp>( response );


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
                context.MomentEnd = PreciseDateTime.UtcNow;

                if ( config.Type == MethodLoggingType.PrePost )
                    await journal.PostAsync( context, jresponse, vex );
                else
                    await journal.FullAsync( context, jrequest, jresponse, vex );

                throw vex;
            }

            if ( vr.IsValid == false )
            {
                ActorAggregateException agex = new ActorAggregateException( vr.Errors );
                var vex = new ZincException( ER.MethodInvoker_ResponseInvalid, agex, request.GetType().FullName );

                context.MomentEnd = PreciseDateTime.UtcNow;

                if ( config.Type == MethodLoggingType.PrePost )
                    await journal.PostAsync( context, jresponse, vex );
                else
                    await journal.FullAsync( context, jrequest, jresponse, vex );

                throw vex;
            }


            /*
             * Post-log
             */
            context.MomentEnd = PreciseDateTime.UtcNow;

            if ( config.Type == MethodLoggingType.PrePost )
                await journal.PostAsync( context, jresponse, null );
            else
                await journal.FullAsync( context, jrequest, jresponse, null );

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
