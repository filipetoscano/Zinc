using Newtonsoft.Json;
using Swashbuckle.Swagger;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Zinc.Json;

namespace Zinc.WebServices
{
    /// <summary />
    public class ZincSchemaFilter : ISchemaFilter
    {
        /// <summary />
        public void Apply( Schema schema, SchemaRegistry schemaRegistry, Type type )
        {
            #region Validations

            if ( schema == null )
                throw new ArgumentNullException( nameof( schema ) );

            if ( type == null )
                throw new ArgumentNullException( nameof( type ) );

            #endregion


            /*
             * Date/Time
             */
            var properties = type.GetProperties()
                .Where( prop => prop.PropertyType == typeof( DateTime )
                     && prop.GetCustomAttribute<JsonConverterAttribute>() != null );

            foreach ( var prop in properties )
            {
                var conv = prop.GetCustomAttribute<JsonConverterAttribute>();
                var propSchema = schema.properties[ prop.Name ];

                if ( conv.ConverterType == typeof( DateConverter ) || conv.ConverterType == typeof( NullableDateConverter ) )
                {
                    propSchema.format = "date";
                    propSchema.example = DateTime.UtcNow.ToString( "yyyy-MM-dd", CultureInfo.InvariantCulture );
                }

                if ( conv.ConverterType == typeof( TimeConverter ) || conv.ConverterType == typeof( NullableTimeConverter ) )
                {
                    propSchema.format = "time";
                    propSchema.example = DateTime.UtcNow.ToString( "HH:mm:ss", CultureInfo.InvariantCulture );
                }
            }
        }
    }
}
