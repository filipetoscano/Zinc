using Swashbuckle.Application;

namespace Zinc.WebServices
{
    public static class SwaggerExtensions
    {
        public static void ZincConfigure( this SwaggerDocsConfig config )
        {
            config.UseFullTypeNameInSchemaIds();
            config.DescribeAllEnumsAsStrings();
            config.SchemaFilter<ZincSchemaFilter>();
        }
    }
}
