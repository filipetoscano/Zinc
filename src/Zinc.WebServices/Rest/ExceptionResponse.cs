using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Zinc.WebServices.Rest
{
    /// <summary />
    public class ExceptionResponse : HttpResponseMessage, IHttpActionResult
    {
        /// <summary />
        public ExceptionResponse( ActorFault fault )
            : base()
        {
            if ( fault == null )
                throw new ArgumentNullException( nameof( fault ) );

            this.StatusCode = HttpStatusCode.InternalServerError;
            this.Content = new StringContent( JsonConvert.SerializeObject( fault ) );
            this.Content.Headers.ContentType = new MediaTypeHeaderValue( "application/json" );
        }


        /// <summary />
        public Task<HttpResponseMessage> ExecuteAsync( CancellationToken cancellationToken )
        {
            HttpResponseMessage rm = this;

            return Task.FromResult( rm );
        }
    }
}
