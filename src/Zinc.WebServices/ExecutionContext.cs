using Platinum;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;

namespace Zinc.WebServices
{
    /// <summary>
    /// Zinc execution context.
    /// </summary>
    public class ExecutionContext
    {
        /// <summary>
        /// Name of application, whose method is being invoked.
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// Unique name of method being invoked.
        /// </summary>
        public string Method { get; set; }

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

        /// <summary>
        /// Start of (inner) execution.
        /// </summary>
        public DateTime MomentStart { get; set; }

        /// <summary>
        /// End of (inner) execution.
        /// </summary>
        public DateTime MomentEnd { get; set; }


        public ExecutionContext()
        {
        }


        /// <summary>
        /// Builds a Zinc execution context, based on the WCF execution context.
        /// </summary>
        /// <param name="wcfContext">WCF execution context.</param>
        public ExecutionContext( ServiceModel.WcfExecutionContext wcfContext )
        {
            #region Validations

            if ( wcfContext == null )
                throw new ArgumentNullException( nameof( wcfContext ) );

            #endregion

            this.Application = App.Name;
            this.ActivityId = wcfContext.ActivityId;
            this.ExecutionId = wcfContext.ExecutionId;
            this.MomentStart = wcfContext.MomentStart;
        }


        /// <summary>
        /// Builds a Zinc execution context, based on the WebApi execution
        /// context.
        /// </summary>
        /// <param name="request">WebApi execution context.</param>
        public ExecutionContext( HttpRequestMessage request )
        {
            #region Validations

            if ( request == null )
                throw new ArgumentNullException( nameof( request ) );

            #endregion

            // TODO: What if headers aren't present?

            var he = request.Headers.FirstOrDefault( x => x.Key == "X-ExecutionId" );
            this.ExecutionId = new Guid( he.Value.First() );

            var ha = request.Headers.FirstOrDefault( x => x.Key == "X-ActivityId" );
            this.ActivityId = new Guid( ha.Value.First() );

            var hs = request.Headers.FirstOrDefault( x => x.Key == "X-MomentStart" );
            this.MomentStart = DateTime.ParseExact( hs.Value.First(), "O", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal );
        }
    }
}
