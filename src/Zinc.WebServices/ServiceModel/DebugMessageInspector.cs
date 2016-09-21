using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Zinc.WebServices.ServiceModel
{
    public class DebugMessageInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest( ref Message request, IClientChannel channel, InstanceContext instanceContext )
        {
            MessageBuffer buffer = request.CreateBufferedCopy( Int32.MaxValue );
            request = buffer.CreateMessage();

            Debug.WriteLine( buffer.CreateMessage().ToString() );

            return null;
        }


        public void BeforeSendReply( ref Message reply, object correlationState )
        {
            MessageBuffer buffer = reply.CreateBufferedCopy( Int32.MaxValue );
            reply = buffer.CreateMessage();

            Debug.WriteLine( "Sending:\n{0}", buffer.CreateMessage().ToString() );
        }
    }
}
