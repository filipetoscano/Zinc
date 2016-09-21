using System;
using System.Globalization;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Serialization;

namespace Zinc.WebServices.ServiceModel
{
    [XmlRoot( ElementName = WcfExecutionContext.HeaderName, Namespace = Zn.Namespace )]
    public class WcfExecutionContext : MessageHeader
    {
        private const string HeaderName = "ExecutionContext";


        public Guid ActivityId { get; set; }
        public Guid ExecutionId { get; set; }
        public string Action { get; set; }
        public DateTime MomentStart { get; set; }


        /// <summary>
        /// Initializes a new instance of <see cref="WcfExecutionContext" />.
        /// </summary>
        public WcfExecutionContext()
        {
        }


        /// <summary>
        /// Looks for 'svc:ExecutionHeader' header in the message. If the header is
        /// not present, will throw exception.
        /// </summary>
        /// <param name="message">WCF message.</param>
        /// <returns>
        /// Always returns an instance of <see cref="WcfExecutionContext" />.
        /// </returns>
        public static WcfExecutionContext Read( Message message )
        {
            #region Validations

            if ( message == null )
                throw new ArgumentNullException( "message" );

            #endregion

            int ix = message.Headers.FindHeader( HeaderName, Zn.Namespace );

            // TODO: check ix != -1

            var content = message.Headers.GetHeader<XmlNode[]>( ix );

            // TODO: check content.Length

            WcfExecutionContext ctx = new WcfExecutionContext();
            ctx.ActivityId = new Guid( content[ 0 ].InnerText );
            ctx.ExecutionId = new Guid( content[ 1 ].InnerText );
            ctx.Action = content[ 2 ].InnerText;
            ctx.MomentStart = DateTime.ParseExact( content[ 3 ].InnerText, "o", CultureInfo.InvariantCulture );

            return ctx;
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
            writer.WriteElementString( "ActivityId", ActivityId.ToString() );
            writer.WriteElementString( "ExecutionId", ExecutionId.ToString() );
            writer.WriteElementString( "Action", Action );
            writer.WriteElementString( "MomentStart", MomentStart.ToString( "o", CultureInfo.InvariantCulture ) );
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
            get { return Zn.Namespace; }
        }
    }
}
