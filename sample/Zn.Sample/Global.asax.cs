using Platinum.Configuration;
using Platinum.Data;
using System.Net;
using System.Web.Http;
using Zinc.WebServices;

namespace Zn.Sample
{
    /// <summary />
    public class Global : System.Web.HttpApplication
    {
        /// <summary />
        protected void Application_Start()
        {
            /*
             * 
             */
            if ( AppConfiguration.Get<bool>( "SslValidation.Enabled", true ) == false )
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                    ( sender, cert, chain, sslPolicyErrors ) => true;
            }


            /*
             *
             */
            GlobalConfiguration.Configure( ZincApiConfig.Register );
            DataConfig.Register();
        }


        /// <summary />
        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Set( "Server", "Zinc" );
        }
    }
}