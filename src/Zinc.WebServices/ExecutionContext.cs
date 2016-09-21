using System;
using System.Linq;
using System.Net.Http;

namespace Zinc.WebServices
{
    public class ExecutionContext
    {
        /// <summary>
        /// Unique identifier of the SOA activity, of which this message is a part
        /// of. This value can then be used to cross-reference all of the messages
        /// issued/dispatched as part of the activity.
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// Unique identifier of the present method execution, which is a part
        /// of the overall SOA activity stream.
        /// </summary>
        public Guid ExecutionId { get; set; }



        public ExecutionContext()
        {
        }


        public ExecutionContext( HttpRequestMessage request )
        {
            #region Validations

            if ( request == null )
                throw new ArgumentNullException( nameof( request ) );

            #endregion

            // TODO: What if headers aren't present?

            var ha = request.Headers.FirstOrDefault( x => x.Key == "X-ActivityId" );
            this.ActivityId = new Guid( ha.Value.First() );

            var he = request.Headers.FirstOrDefault( x => x.Key == "X-ExecutionId" );
            this.ExecutionId = new Guid( ha.Value.First() );
        }
    }
}
