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
            await Task.Run( () =>
            {
                string asString = Encoding.UTF8.GetString( message );
                Journal( context, 0, null, asString );
            } );
        }


        protected override async Task HandleResponse( RestExecutionContext context, HttpStatusCode statusCode, byte[] message )
        {
            await Task.Run( () =>
            {
                // TODO: What about binary files? :/
                string asString = Encoding.UTF8.GetString( message );
                Journal( context, 1, statusCode, asString );
            } );
        }


        private void Journal( RestExecutionContext context, int step, HttpStatusCode? statusCode, string message )
        {
            //context.ActivityId;
            //context.ExecutionId;
            //context.Method;
            //context.RequestUri;
            //step;
        }
    }
}
