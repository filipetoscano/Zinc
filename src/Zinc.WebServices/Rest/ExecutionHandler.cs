using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Zinc.WebServices.Rest
{
    /// <summary />
    public class ExecutionHandler : DelegatingHandler
    {
        /// <summary>
        /// Mandatory <see cref="DelegatingHandler" /> for Zinc services, since it will
        /// ensure that the expected headers are in place.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <remarks>
        /// When configuring the WebAPI pipeline, <see cref="ExecutionHandler" /> must
        /// always be placed first.
        /// </remarks>
        protected override async Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken )
        {
            #region Validations

            if ( request == null )
                throw new ArgumentNullException( "request" );

            #endregion


            /*
             * Ensures that the request contains valid headers for ActivityId and
             * ExecutionId.
             */
            RestExecutionContext ctx = new RestExecutionContext();
            ctx.ExecutionId = Guid.NewGuid();
            ctx.ActivityId = Guid.Empty;
            ctx.Method = request.Method;
            ctx.RequestUri = request.RequestUri;
            ctx.MomentStart = DateTime.UtcNow;


            /*
             * 
             */
            if ( request.Headers.Contains( ZnHeaders.ActivityId ) == true )
            {
                var h = request.Headers.First( x => x.Key == ZnHeaders.ActivityId );
                var v = h.Value.First();
                Guid activityId;

                try
                {
                    activityId = new Guid( v );
                }
                catch ( FormatException )
                {
                    activityId = Guid.Empty;
                }
                catch ( ArgumentNullException )
                {
                    activityId = Guid.Empty;
                }

                ctx.ActivityId = activityId;
            }


            /*
             * 
             */
            if ( request.Headers.Authorization != null
                && request.Headers.Authorization.Scheme == "Bearer" )
            {
                ctx.AccessToken = request.Headers.Authorization.Parameter;
            }


            /*
             * 
             */
            request.Properties[ RestExecutionContext.PropertyName ] = ctx;


            /*
             * Defer execution to the actual implementation.
             */
            var response = await base.SendAsync( request, cancellationToken );


            /*
             *
             */
            response.Headers.Add( ZnHeaders.ExecutionId, ctx.ExecutionId.ToString( "D" ) );
            response.Headers.Add( ZnHeaders.MomentStart, ctx.MomentStart.ToString( "O", CultureInfo.InvariantCulture ) );
            response.Headers.Add( ZnHeaders.MomentEnd, DateTime.UtcNow.ToString( "O", CultureInfo.InvariantCulture ) );

            return response;
        }
    }
}
