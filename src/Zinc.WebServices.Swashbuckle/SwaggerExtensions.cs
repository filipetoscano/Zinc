using Platinum;
using Swashbuckle.Application;

namespace Zinc.WebServices
{
    /// <summary />
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Configures the Swashbuckle document generation, as per Zinc needs.
        /// </summary>
        /// <param name="config">
        /// Swagger configuration options.
        /// </param>
        public static void ZincConfigure( this SwaggerDocsConfig config )
        {
            config.UseFullTypeNameInSchemaIds();
            config.DescribeAllEnumsAsStrings();
            config.SchemaFilter<ZincSchemaFilter>();


            /*
             * Custom types
             */
            config.MapType<Duration>( () => new Swashbuckle.Swagger.Schema()
            {
                type = "string",
                format = "duration",
                example = "PT1H",
            } );

            config.MapType<Duration?>( () => new Swashbuckle.Swagger.Schema()
            {
                type = "string",
                format = "duration",
                example = "PT1H",
            } );
        }
    }
}
