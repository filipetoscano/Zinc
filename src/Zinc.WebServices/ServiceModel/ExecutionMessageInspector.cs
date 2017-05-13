using Platinum;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace Zinc.WebServices.ServiceModel
{
    /// <summary />
    public class ExecutionMessageInspector : IDispatchMessageInspector
    {
        /// <summary />
        public object AfterReceiveRequest( ref Message request, IClientChannel channel, InstanceContext instanceContext )
        {
            /*
             *
             */
            WcfExecutionContext ctx = new WcfExecutionContext();
            ctx.ActivityId = Guid.Empty;
            ctx.AccessToken = null;
            ctx.ExecutionId = Guid.NewGuid();
            ctx.Action = request.Headers.Action;
            ctx.MomentStart = PreciseDateTime.UtcNow;


            /*
             * Don't read the header using a deserializer, because we don't want to trust
             * the request.
             */
            int ix = request.Headers.FindHeader( "EndpointHeader", Zn.Namespace );

            if ( ix > -1 )
            {
                var xr = request.Headers.GetReaderAtHeader( ix );

                XmlNamespaceManager manager = new XmlNamespaceManager( new NameTable() );
                manager.AddNamespace( "zn", Zn.Namespace );

                XmlDocument doc = new XmlDocument();
                doc.Load( xr );


                var activityElem = doc.DocumentElement.SelectSingleNode( " zn:ActivityId ", manager );

                if ( activityElem != null )
                {
                    try
                    {
                        ctx.ActivityId = new Guid( activityElem.InnerText );
                    }
                    catch
                    {
                        // Snuff!
                    }
                }


                var accessTokenElem = doc.DocumentElement.SelectSingleNode( " zn:AccessToken ", manager );

                if ( accessTokenElem != null )
                {
                    ctx.AccessToken = accessTokenElem.InnerText;
                }
            }


            /*
             *
             */
            request.Headers.Add( ctx );

            return ctx;
        }


        /// <summary />
        public void BeforeSendReply( ref Message reply, object correlationState )
        {
            if ( reply == null )
                return;

            var ctx = (WcfExecutionContext) correlationState;


            /*
             * Always append the ExecutionHeader to the response.
             */
            ExecutionHeader content = new ExecutionHeader();
            content.ExecutionId = ctx.ExecutionId;
            content.MomentStart = ctx.MomentStart;
            content.MomentEnd = DateTime.UtcNow;

            WcfExecutionHeader header = new WcfExecutionHeader();
            header.Content = content;

            reply.Headers.Add( header );
        }
    }
}
