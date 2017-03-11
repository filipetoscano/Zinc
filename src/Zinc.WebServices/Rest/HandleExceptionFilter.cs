using Platinum;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Zinc.WebServices.Rest
{
    /// <summary>
    /// Converts the exception raised by the implementation class into an
    /// instance of <see cref="ActorFault" />.
    /// </summary>
    public class HandleExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// Based on the exception raised by the implementation class, assigns
        /// as response an instance of <see cref="ActorFault" />.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public override void OnException( HttpActionExecutedContext context )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            #endregion

            var request = context.ActionContext.Request;

            ActorFault fault;
            
            if ( context.Exception is ActorException )
            {
                ActorException aex = (ActorException) context.Exception;
                fault = ActorFault.From( aex );
            }
            else
            {
                fault = ActorFault.FromUnhandled( context.Exception );
            }

            context.Response = request.CreateResponse<ActorFault>( HttpStatusCode.InternalServerError, fault );
        }
    }
}
