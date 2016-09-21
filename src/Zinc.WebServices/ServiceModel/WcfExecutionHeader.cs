using System.Globalization;
using System.ServiceModel.Channels;
using System.Xml;

namespace Zinc.WebServices.ServiceModel
{
    public class WcfExecutionHeader : MessageHeader
    {
        private const string HeaderName = "ExecutionHeader";
        private const string HeaderNamespace = Zn.Namespace;
        
        public ExecutionHeader Content
        {
            get;
            set;
        }


        /// <summary>
        /// Called when the start header is serialized using the specified XML writer.
        /// </summary>
        /// <param name="writer">
        /// An <see cref="XmlDictionaryWriter"/> that is used to serialize the start header.
        /// </param>
        /// <param name="messageVersion">Unused.</param>
        /// <remarks>
        /// We override the default implementation in order to force a specific message
        /// layout, rather than relying on the default implementation which doesn't
        /// serialize in the expected manner.
        /// </remarks>
        protected override void OnWriteHeaderContents( XmlDictionaryWriter writer, MessageVersion messageVersion )
        {
            writer.WriteElementString( "ExecutionId", this.Content.ExecutionId.ToString() );
            writer.WriteElementString( "MomentStart", this.Content.MomentStart.ToString( "o", CultureInfo.InvariantCulture ) );
            writer.WriteElementString( "MomentEnd", this.Content.MomentEnd.ToString( "o", CultureInfo.InvariantCulture ) );
        }

        /// <summary>
        /// Gets the name of the current WCF header.
        /// </summary>
        /// <remarks>
        /// Required by <see cref="MessageHeader" />. 
        /// </remarks>
        public override string Name
        {
            get { return HeaderName; }
        }

        /// <summary>
        /// Gets the namespace of the current WCF header.
        /// </summary>
        /// <remarks>
        /// Required by <see cref="MessageHeader" />. 
        /// </remarks>
        public override string Namespace
        {
            get { return HeaderNamespace; }
        }
    }
}
