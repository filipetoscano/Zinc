using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Zinc.WebServices.Rest
{
    /// <summary>
    /// Logs the request / response pairs to the Visual Studio debug pane.
    /// </summary>
    public class DebugLoggingHandler : LoggingHandler
    {
        /// <summary />
        protected override async Task HandleRequest( RestExecutionContext context, byte[] message )
        {
            await Task.Run( () =>
            {
                Debug.WriteLine( string.Format(
                    CultureInfo.InvariantCulture,
                    "R:{0} - {1} {2}\r\n{3}",
                    context.ActivityId,
                    context.Method,
                    context.RequestUri.AbsolutePath,
                    Encoding.UTF8.GetString( message ) ) );
            } );
        }


        /// <summary />
        protected override async Task HandleResponse( RestExecutionContext context, HttpStatusCode statusCode, byte[] message )
        {
            await Task.Run( () =>
            {
                Debug.WriteLine( string.Format(
                    CultureInfo.InvariantCulture,
                    "r:{0}\r\n{1}",
                    context.ActivityId,
                    Encoding.UTF8.GetString( message ) ) );
            } );
        }
    }
}
