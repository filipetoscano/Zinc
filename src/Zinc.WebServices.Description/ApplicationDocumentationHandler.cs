using System.Web;

namespace Zinc.WebServices.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationDocumentationHandler : IHttpHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest( HttpContext context )
        {
            // TODO

            context.Response.Write( "<html>" );
            context.Response.Write( "<body>" );
            context.Response.Write( "Documentation" );
            context.Response.Write( "</body>" );
            context.Response.Write( "</html>" );
        }


        /// <summary>
        /// Returns that this handler is re-usable.
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }
    }
}
