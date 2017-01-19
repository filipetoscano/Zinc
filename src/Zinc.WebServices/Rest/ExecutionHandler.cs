using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Zinc.WebServices.Rest
{
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
            DateTime start = DateTime.UtcNow;

            RestExecutionContext ctx = new RestExecutionContext();
            ctx.ExecutionId = Guid.NewGuid();
            ctx.ActivityId = Guid.Empty;
            ctx.Method = request.Method;
            ctx.RequestUri = request.RequestUri;

            if ( request.Headers.Contains( "X-ActivityId" ) == true )
            {
                var h = request.Headers.First( x => x.Key == "X-ActivityId" );
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

            request.Headers.Remove( "X-ActivityId" );

            request.Headers.Add( "X-ActivityId", ctx.ActivityId.ToString( "D" ) );
            request.Headers.Add( "X-ExecutionId", ctx.ExecutionId.ToString( "D" ) );
            request.Headers.Add( "X-MomentStart", start.ToString( "O", CultureInfo.InvariantCulture ) );


            /*
             * Defer execution to the actual implementation.
             */
            var response = await base.SendAsync( request, cancellationToken );


            /*
             *
             */
            response.Headers.Add( "X-ExecutionId", ctx.ExecutionId.ToString( "D" ) );
            response.Headers.Add( "X-MomentStart", start.ToString( "O", CultureInfo.InvariantCulture ) );
            response.Headers.Add( "X-MomentEnd", DateTime.UtcNow.ToString( "O", CultureInfo.InvariantCulture ) );

            return response;
        }
    }
}
