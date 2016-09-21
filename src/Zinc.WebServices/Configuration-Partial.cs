using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Zinc.WebServices
{
    public partial class ZincRestConfiguration
    {
        /// <summary>
        /// Gets the list of message handlers which should be injected into
        /// REST pipeline.
        /// </summary>
        /// <remarks>
        /// In order to use a handler, must be defined as an extension.
        /// </remarks>
        public XElement Handlers
        {
            get;
            private set;
        }


        protected override bool OnDeserializeUnrecognizedElement( string elementName, XmlReader reader )
        {
            if ( elementName == "handlers" )
            {
                this.Handlers = (XElement) XElement.ReadFrom( reader );
                return true;
            }
            else
            {
                return base.OnDeserializeUnrecognizedElement( elementName, reader );
            }
        }
    }


    public class JournallingMessageConfig
    {
        public bool Journal;
        public List<DocumentExpression> Secrets;
    }


    public class DocumentExpression
    {
        public string Expression;
    }
}
