using System.Xml;

namespace Zinc.WebServices.ProxyGenerator
{
    /// <summary />
    internal static class XmlExtensions
    {
        /// <summary />
        internal static XmlElement AddElement( this XmlDocument parent, string elementName )
        {
            XmlElement child = parent.CreateElement( elementName );

            parent.AppendChild( child );

            return child;
        }


        /// <summary />
        internal static XmlElement AddElement( this XmlElement parent, string elementName )
        {
            XmlElement child = parent.OwnerDocument.CreateElement( elementName );

            parent.AppendChild( child );

            return child;
        }


        /// <summary />
        internal static XmlElement AddAttribute( this XmlElement element, string attributeName, string attributeValue )
        {
            XmlAttribute attr = element.OwnerDocument.CreateAttribute( attributeName );
            attr.Value = attributeValue;

            element.Attributes.Append( attr );

            return element;
        }
    }
}
