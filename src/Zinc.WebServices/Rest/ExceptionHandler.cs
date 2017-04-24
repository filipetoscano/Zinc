using Platinum;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace Zinc.WebServices.Rest
{
    /// <summary>
    /// Handles *any* unhandled exception which occurs as part of the
    /// WebAPI pipeline.
    /// </summary>
    /// <remarks>
    /// Since exceptions thrown by the method implementation are covered
    /// by <see cref="HandleExceptionFilter" />, this will end up handling
    /// only those pertinent to the pre/post pipeline.
    /// </remarks>
    public class ExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="context">
        /// Exception handling context.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token.
        /// </param>
        /// <returns>
        /// Task.
        /// </returns>
        public Task HandleAsync( ExceptionHandlerContext context, CancellationToken cancellationToken )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            #endregion

            ActorFault fault;
            bool detailed = ZincConfiguration.Current.Rest.Errors.Detailed;

            if ( context.Exception is ActorException )
            {
                ActorException aex = (ActorException) context.Exception;
                fault = ActorFault.From( aex, detailed );
            }
            else
            {
                fault = ActorFault.FromUnhandled( context.Exception, detailed );
            }

            context.Result = new ExceptionResponse( fault );
            return Task.CompletedTask;
        }
    }
}
