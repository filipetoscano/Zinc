using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Zinc.Json
{
    /// <summary>
    /// Converts a nullable <see cref="DateTime" /> into a JSON value, with only the
    /// time part, stripping away all of the date/timezone information.
    /// </summary>
    public class NullableTimeConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>true if this instance can convert the specified object type; otherwise, false.</returns>
        public override bool CanConvert( Type objectType )
        {
            return objectType == typeof( DateTime );
        }


        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
        {
            string v = (string) reader.Value;

            if ( v == null )
                return null;
            else
                return DateTime.ParseExact( (string) reader.Value, "hh:mm:ss", CultureInfo.InvariantCulture );
        }


        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
        {
            DateTime? d = (DateTime?) value;

            if ( d.HasValue == true )
                writer.WriteValue( d.Value.ToString( "hh:mm:ss", CultureInfo.InvariantCulture ) );
            else
                writer.WriteNull();
        }
    }
}
