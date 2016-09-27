using System.Web;

namespace Zinc.Web
{
    /// <summary />
    public class WadlGeneratorHandler : IHttpHandler
    {
        /// <summary />
        public void ProcessRequest( HttpContext context )
        {
            context.Response.ContentType = "application/xml";
            context.Response.Write( "<root />" );
        }


        /// <summary />
        public bool IsReusable
        {
            get { return true; }
        }
    }
}
