using Platinum;
using System;
using System.Net.Http;
using System.ServiceModel;
using Zinc.WebServices.Rest;
using WcfExecutionContext = Zinc.WebServices.ServiceModel.WcfExecutionContext;

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
        /// Access token, used to access the current application.
        /// </summary>
        public string AccessToken { get; set; }

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


        /// <summary>
        /// Builds a Zinc execution context, based on the WCF execution context.
        /// </summary>
        /// <param name="wcfContext">WCF execution context.</param>
        public ExecutionContext( OperationContext operationContext )
        {
            #region Validations

            if ( operationContext == null )
                throw new ArgumentNullException( nameof( operationContext ) );

            #endregion

            var wcfContext = WcfExecutionContext.Get( operationContext );

            this.Application = App.Name;
            this.ActivityId = wcfContext.ActivityId;
            this.AccessToken = wcfContext.AccessToken;
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

            var ctx = (RestExecutionContext) request.Properties[ RestExecutionContext.PropertyName ];

            if ( ctx == null )
                throw new ZincConfigurationException( ER.Rest_NoExecutionContext );

            this.Application = App.Name;
            this.ActivityId = ctx.ActivityId;
            this.AccessToken = ctx.AccessToken;
            this.ExecutionId = ctx.ExecutionId;
            this.MomentStart = ctx.MomentStart;
        }
    }
}
