using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Zinc.WebServices.ServiceModel
{
    /// <summary>
    /// Logs inbound WCF messages to ElasticSearch Server.
    /// </summary>
    public class ElasticLoggingMessageInspector : IDispatchMessageInspector
    {
        /// <summary>
        /// Called after an inbound message has been received but before the message is dispatched
        /// to the intended operation.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="channel">The incoming channel.</param>
        /// <param name="instanceContext">The current service instance.</param>
        /// <returns>
        /// An instance of <see cref="WcfExecutionContext" />. This object is passed back to method
        /// <see cref="BeforeSendReply(ref Message, object)" />.
        /// </returns>
        public object AfterReceiveRequest( ref Message request, IClientChannel channel, InstanceContext instanceContext )
        {
            /*
             *
             */
            MessageBuffer buffer = request.CreateBufferedCopy( Int32.MaxValue );
            request = buffer.CreateMessage();


            /*
             *
             */
            WcfExecutionContext ctx = WcfExecutionContext.Read( request );


            return ctx;
        }


        /// <summary>
        ///  Called after the operation has returned but before the reply message is sent.
        /// </summary>
        /// <param name="reply">
        /// The reply message. This value is null if the operation is one way.
        /// </param>
        /// <param name="correlationState">
        /// The correlation object <see cref="WcfExecutionContext" />.
        /// </param>
        public void BeforeSendReply( ref Message reply, object correlationState )
        {
            if ( reply == null )
                return;

            var ctx = (WcfExecutionContext) correlationState;
        }
    }
}
