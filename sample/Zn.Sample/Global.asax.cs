using Platinum.Configuration;
using System;
using System.Net;
using System.Web.Http;
using Zinc.WebServices;

namespace Zn.Sample
{
    public class Global : System.Web.HttpApplication
    {
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
            GlobalConfiguration.Configure( WebApiConfig.Register );
        }


        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Set( "Server", "Zinc" );
        }
    }
}