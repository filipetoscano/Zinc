using Swashbuckle.Application;
using System.Web.Http;
using WebActivatorEx;
using Zinc.WebServices;
using Zn.Sample;

[assembly: PreApplicationStartMethod( typeof( SwaggerConfig ), "Register" )]

namespace Zn.Sample
{
    /// <summary />
    public class SwaggerConfig
    {
        /// <summary />
        public static void Register()
        {
            var thisAssembly = typeof( SwaggerConfig ).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger( c =>
                     {
                         c.SingleApiVersion( "v1", "Zn.Sample" );
                         c.ZincConfigure();
                     } )
                .EnableSwaggerUi( c =>
                     {
                     } );
        }
    }
}
