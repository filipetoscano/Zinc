using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Zinc.WebServices.Rest
{
    /// <summary>
    /// Logs request/response REST messages to ElasticSearch server.
    /// </summary>
    public class ElasticLoggingHandler : LoggingHandler
    {
        protected override async Task HandleRequest( RestExecutionContext context, byte[] message )
        {
            string asString = Encoding.UTF8.GetString( message );
            await Journal( context, 0, null, asString );
        }


        protected override async Task HandleResponse( RestExecutionContext context, HttpStatusCode statusCode, byte[] message )
        {
            // TODO: What about binary files? :/
            string asString = Encoding.UTF8.GetString( message );
            await Journal( context, 1, statusCode, asString );
        }


        private Task Journal( RestExecutionContext context, int step, HttpStatusCode? statusCode, string message )
        {
            return Task.CompletedTask;
        }
    }
}
