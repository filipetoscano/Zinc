using Platinum;
using Platinum.Validation;
using System;

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
             * Pre-log
             */
            // TODO


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

                throw vex;
            }

            if ( vr.IsValid == false )
            {
                ActorAggregateException agex = new ActorAggregateException( vr.Errors );

                var vex = new WsException( ER.MethodInvoker_RequestInvalid, agex, request.GetType().FullName );

                // TODO: journal

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
            catch ( ActorException )
            {
                // TODO: journal

                throw;
            }
            catch ( Exception )
            {
                // TODO: journal

                throw;
            }


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

                if ( true )
                {
                    // TODO: journal

                    throw vex;
                }
            }

            if ( vr.IsValid == false )
            {
                ActorAggregateException agex = new ActorAggregateException( vr.Errors );

                var vex = new WsException( ER.MethodInvoker_ResponseInvalid, agex, request.GetType().FullName );

                if ( true )
                {
                    // TODO: journal

                    throw vex;
                }
            }


            /*
             * Post-log
             */
            // TODO: journal

            return response;
        }
    }
}
