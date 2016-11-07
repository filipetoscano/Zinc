using System.Web.Http;
using WebActivatorEx;
using Zn.Sample;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod( typeof( SwaggerConfig ), "Register" )]

namespace Zn.Sample
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof( SwaggerConfig ).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger( c =>
                     {
                         c.SingleApiVersion( "v1", "Zn.Sample" );
                         c.UseFullTypeNameInSchemaIds();
                         c.DescribeAllEnumsAsStrings();
                     } )
                .EnableSwaggerUi( c =>
                     {
                     } );
        }
    }
}
