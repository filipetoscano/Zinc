using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Zinc.WebServices.Rest
{
    /// <summary>
    /// Serializes and deserializes WebAPI request/responses using Newtonsoft's
    /// latest <see cref="JsonSerializer" />.
    /// </summary>
    public class JsonNetFormatter : MediaTypeFormatter
    {
        private JsonSerializerSettings _jsonSerializerSettings;
        private Encoding _encoding;


        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetFormatter" /> class.
        /// </summary>
        /// <param name="settings">
        /// Optional settings that are passed to <see cref="JsonSerializer" /> class.
        /// </param>
        public JsonNetFormatter( JsonSerializerSettings settings )
        {
            _encoding = new UTF8Encoding( false, true );
            _jsonSerializerSettings = settings ?? new JsonSerializerSettings();

            SupportedMediaTypes.Add( new MediaTypeHeaderValue( "application/json" ) { CharSet = "utf-8" } );
            SupportedMediaTypes.Add( new MediaTypeHeaderValue( "text/json" ) { CharSet = "utf-8" } );
        }


        /// <summary>
        /// Queries whether this <see cref="MediaTypeFormatter"/> can deserialize an
        /// object of the specified type.
        /// </summary>
        /// <param name="type">The type to deserialize.</param>
        /// <returns>Always true.</returns>
        public override bool CanReadType( Type type )
        {
            return true;
        }


        /// <summary>
        /// Queries whether this <see cref="MediaTypeFormatter"/> can serialize an
        /// object of the specified type.
        /// </summary>
        /// <param name="type">The type to serialize.</param>
        /// <returns>Always true.</returns>
        public override bool CanWriteType( Type type )
        {
            return true;
        }


        /// <summary>
        /// Asynchronously deserializes an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="readStream">The <see cref="Stream"/> to read.</param>
        /// <param name="content">The <see cref="HttpContent" />, if available. It may be null.</param>
        /// <param name="formatterLogger">The <see cref="IFormatterLogger"/> to log events to.</param>
        /// <returns>A <see cref="Task" /> whose result will be an object of the given type.</returns>
        [SuppressMessage( "Microsoft.Usage", "CA2202:Do not dispose objects multiple times" )]
        public override Task<object> ReadFromStreamAsync( Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger )
        {
            // Create a serializer
            JsonSerializer serializer = JsonSerializer.Create( _jsonSerializerSettings );

            // Create task reading the content
            return Task.Factory.StartNew( () =>
             {
                 using ( StreamReader streamReader = new StreamReader( readStream, _encoding ) )
                 {
                     using ( JsonTextReader jsonTextReader = new JsonTextReader( streamReader ) )
                     {
                         return serializer.Deserialize( jsonTextReader, type );
                     }
                 }
             } );
        }


        /// <summary>
        /// Asynchronously writes an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the object to write.</param>
        /// <param name="value">The object value to write. It may be null.</param>
        /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
        /// <param name="content">The <see cref="HttpContent"/> if available. It may be null.</param>
        /// <param name="transportContext">The <see cref="TransportContext"/> if available. It may be null.</param>
        /// <returns>A <see cref="Task"/> that will perform the write.</returns>
        public override Task WriteToStreamAsync( Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext )
        {
            // Create a serializer
            JsonSerializer serializer = JsonSerializer.Create( _jsonSerializerSettings );

            // Create task writing the serialized content
            return Task.Factory.StartNew( () =>
             {
                 using ( JsonTextWriter jsonTextWriter = new JsonTextWriter( new StreamWriter( writeStream, _encoding ) ) { CloseOutput = false } )
                 {
                     serializer.Serialize( jsonTextWriter, value );
                     jsonTextWriter.Flush();
                 }
             } );
        }
    }
}
