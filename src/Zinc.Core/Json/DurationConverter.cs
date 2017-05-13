using Newtonsoft.Json;
using Platinum;
using System;

namespace Zinc.Json
{
    /// <summary>
    /// Converts a <see cref="DateTime" /> into a JSON value, with only the
    /// time part, stripping away all of the date/timezone information.
    /// </summary>
    public class DurationConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>true if this instance can convert the specified object type; otherwise, false.</returns>
        public override bool CanConvert( Type objectType )
        {
            return objectType == typeof( Duration );
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
            string v;

            try
            {
                v = (string) reader.Value;
            }
            catch ( InvalidCastException )
            {
                throw new JsonSerializationException( $"Expected string when parsing xs:duration, got '{ reader.ValueType.FullName }'. Path '{ reader.Path }'." );
            }

            try
            {
                return Duration.Parse( v );
            }
            catch ( Exception )
            {
                throw new JsonSerializationException( $"Value '{ v }' is not a valid xs:duration. Path '{ reader.Path }'." );
            }
        }


        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
        {
            Duration d = (Duration) value;
            writer.WriteValue( d.ToString() );
        }
    }
}
