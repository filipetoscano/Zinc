using System;
using System.Net.Http;

namespace Zinc.WebServices.Rest
{
    /// <summary>
    /// Internal REST execution context.
    /// </summary>
    public class RestExecutionContext
    {
        /// <summary>
        /// Name of the WebAPI property which is used to store the REST execution.
        /// </summary>
        internal const string PropertyName = "zn:Execution";


        /// <summary>
        /// Externally provided activity Id.
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// Externally provided access token.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Internal Id, referring to the present execution.
        /// </summary>
        public Guid ExecutionId { get; set; }

        /// <summary>
        /// HTTP method used to invoke API.
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// Request URI.
        /// </summary>
        public Uri RequestUri { get; set; }

        /// <summary>
        /// Moment in which message was received.
        /// </summary>
        public DateTime MomentStart { get; set; }
    }
}
