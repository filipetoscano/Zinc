using Elasticsearch.Net;
using Newtonsoft.Json;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using Platinum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zinc.WebServices.ElasticSearch
{
    /// <summary>
    /// Target which publishes Zinc messages to ElasticSearch in a structured
    /// format.
    /// </summary>
    [Target( "ElasticJournal" )]
    public class ElasticJournalTarget : TargetWithLayout
    {
        private IElasticLowLevelClient _client;

        /// <summary>
        /// Gets or sets the Uri used to connect to ElasticSearch.
        /// </summary>
        [RequiredParameter]
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the name of the elasticsearch index to write to.
        /// </summary>
        [RequiredParameter]
        public Layout Index { get; set; }

        /// <summary>
        /// Gets or sets the document type for the elasticsearch index.
        /// </summary>
        [RequiredParameter]
        public Layout DocumentType { get; set; }

        /// <summary>
        /// Gets or sets if exceptions will be rethrown.
        /// </summary>
        public bool ThrowExceptions { get; set; }


        /// <summary>
        /// Creates a new instance of the <see cref="ElasticJournalTarget" /> class.
        /// </summary>
        public ElasticJournalTarget()
        {
            Name = "ElasticJournal";
            Uri = "http://localhost:9200";
            DocumentType = "message";
            Index = "message-${date:format=yyyy.MM.dd}";
        }


        /// <summary>
        /// Initializes the current instance.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            var uri = Uri;
            var nodes = uri.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries ).Select( url => new Uri( url ) );
            var connectionPool = new StaticConnectionPool( nodes );

            var config = new ConnectionConfiguration( connectionPool );
            config.DisableAutomaticProxyDetection();

            _client = new ElasticLowLevelClient( config );
        }


        /// <summary>
        /// Writes a single event to ElasticSearch.
        /// </summary>
        /// <param name="logEvent">NLog Event.</param>
        protected override void Write( AsyncLogEventInfo logEvent )
        {
            Write( new[] { logEvent } );
        }


        /// <summary>
        /// Writes a batch of events to ElasticSearch.
        /// </summary>
        /// <param name="logEvents">Array of NLog Event.</param>
        protected override void Write( AsyncLogEventInfo[] logEvents )
        {
            SendBatch( logEvents );
        }


        private void SendBatch( IEnumerable<AsyncLogEventInfo> events )
        {
            #region Validations

            if ( events == null )
                throw new ArgumentNullException( nameof( events ) );

            #endregion

            if ( events.Count() == 0 )
                return;

            try
            {
                var logEvents = events.Select( e => e.LogEvent );
                var payload = ToPostData( logEvents );

                if ( payload == null )
                    return;

                var result = _client.Bulk<byte[]>( payload );

                if ( result.Success == true )
                    return;


                /*
                 * 
                 */
                InternalLogger.Error( "Failed to send Zinc messages to elasticsearch: status={0}, message=\"{1}\"",
                    result.HttpStatusCode,
                    result.OriginalException?.Message ?? "No error message. Enable Trace logging for more information." );

                InternalLogger.Trace( "Failed to send Zinc messages to elasticsearch: result={0}", result );

                if ( result.OriginalException != null )
                    throw result.OriginalException;
            }
            catch ( Exception ex )
            {
                InternalLogger.Error( "Error while sending Zinc messages to elasticsearch: message=\"{0}\"", ex.Message );

                if ( ThrowExceptions == true )
                    throw;
            }
        }


        private object ToPostData( IEnumerable<LogEventInfo> logEvents )
        {
            #region Validations

            if ( logEvents == null )
                throw new ArgumentNullException( nameof( logEvents ) );

            #endregion


            /*
             * 
             */
            string appName;
            string appEnvironment;

            try
            {
                appName = App.Name;
            }
            catch
            {
                appName = "##NotDefined##";
            }

            try
            {
                appEnvironment = App.Environment;
            }
            catch
            {
                appEnvironment = "##NotDefined##";
            }


            /*
             * 
             */
            var payload = new List<object>();


            foreach ( var logEvent in logEvents )
            {
                ExecutionContext context;
                object request = null;
                object response = null;
                ActorException error = null;
                TimeSpan? timeSpan = null;

                switch ( logEvent.Message )
                {
                    case "Zn.FullAsync":
                        context = (ExecutionContext) logEvent.Parameters[ 0 ];
                        request = logEvent.Parameters[ 1 ];
                        response = logEvent.Parameters[ 2 ];
                        error = (ActorException) logEvent.Parameters[ 3 ];

                        timeSpan = context.MomentEnd - context.MomentStart;
                        break;


                    case "Zn.PostAsync":
                        context = (ExecutionContext) logEvent.Parameters[ 0 ];
                        response = logEvent.Parameters[ 1 ];
                        error = (ActorException) logEvent.Parameters[ 2 ];
                        break;


                    case "Zn.PreAsync":
                        context = (ExecutionContext) logEvent.Parameters[ 0 ];
                        request = logEvent.Parameters[ 1 ];
                        break;


                    default:
                        // Not a valid log event! Skip!
                        continue;
                }


                /*
                 * 
                 */
                var document = new Dictionary<string, object>
                {
                    { "@timestamp", logEvent.TimeStamp },
                    { "level", logEvent.Level.Name },
                    { "application", App.Name },
                    { "environment", App.Environment },
                    { "host", Environment.MachineName },
                    { "message", "OK" },
                    { "accessToken", context.AccessToken },
                    { "activityId", context.ActivityId },
                    { "executionId", context.ExecutionId },
                    { "method", context.Method },
                };

                if ( timeSpan.HasValue == true )
                    document.Add( "duration", timeSpan.Value.TotalMilliseconds );

                if ( request != null )
                    document.Add( "request", ToJson( request ) );

                if ( response != null )
                    document.Add( "response", ToJson( response ) );

                if ( error != null )
                {
                    document[ "message" ] = error.Description;
                    document.Add( "actor", error.Actor );
                    document.Add( "code", error.Code );
                    document.Add( "exception", error.ToString() );
                }
                else
                {
                    document.Add( "actor", null );
                    document.Add( "code", null );
                    document.Add( "exception", null );
                }

                var index = Index.Render( logEvent ).ToLowerInvariant();
                var type = DocumentType.Render( logEvent );

                payload.Add( new { index = new { _index = index, _type = type } } );
                payload.Add( document );
            }


            /*
             * 
             */
            if ( payload.Count == 0 )
                return null;

            return payload;
        }



        /// <summary>
        /// Serialize an object into pretty-printed JSON.
        /// </summary>
        /// <param name="obj">
        /// Object to serialize.
        /// </param>
        /// <returns>
        /// String representation of object, in JSON.
        /// </returns>
        private static string ToJson( object obj )
        {
            if ( obj == null )
                throw new ArgumentNullException( nameof( obj ) );

            return JsonConvert.SerializeObject( obj, Formatting.Indented, _converters );
        }


        private static JsonConverter[] _converters = new JsonConverter[]
        {
            new Newtonsoft.Json.Converters.StringEnumConverter()
        };
    }
}
