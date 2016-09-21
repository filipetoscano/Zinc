using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Zinc.WebServices.Rest
{
    /// <summary>
    /// Base class for REST message logging.
    /// </summary>
    public abstract class LoggingHandler : DelegatingHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        protected override async Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken )
        {
            /*
             * We can .First() the headers, because the ExecutionHandler (which is
             * first in the pipeline) has guaranteed that they really exist.
             */
            DateTime start = DateTime.UtcNow;

            RestExecutionContext ctx = new RestExecutionContext();
            ctx.ExecutionId = new Guid( request.Headers.First( x => x.Key == "X-ExecutionId" ).Value.First() );
            ctx.ActivityId = new Guid( request.Headers.First( x => x.Key == "X-ActivityId" ).Value.First() );
            ctx.Method = request.Method;
            ctx.RequestUri = request.RequestUri;


            /*
             * Journal request.
             */
            byte[] requestMessage = await request.Content.ReadAsByteArrayAsync();

            await HandleRequest( ctx, requestMessage );


            /*
             * Defer execution to the actual implementation.
             */
            HttpResponseMessage response = await base.SendAsync( request, cancellationToken );


            /*
             * Journal response.
             */
            byte[] responseMessage = await response.Content.ReadAsByteArrayAsync();

            await HandleResponse( ctx, response.StatusCode, responseMessage );

            return response;
        }


        protected abstract Task HandleRequest( RestExecutionContext context, byte[] message );
        protected abstract Task HandleResponse( RestExecutionContext context, HttpStatusCode statusCode, byte[] message );
    }
}
