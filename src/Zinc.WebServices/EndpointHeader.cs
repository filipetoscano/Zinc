using System;
using System.Xml.Serialization;

namespace Zinc.WebServices
{
    /// <summary>
    /// Optional request header when invoking Zinc services.
    /// </summary>
    [XmlType( Namespace = Zn.Namespace )]
    [XmlRoot( Namespace = Zn.Namespace )]
    public class EndpointHeader
    {
        /// <summary>
        /// Unique identifier of the SOA activity, of which this message is a part
        /// of. This value can then be used to cross-reference all of the messages
        /// issued/dispatched as part of the activity.
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// Access token, which may be validated to see if caller may access the
        /// current API.
        /// </summary>
        public string AccessToken { get; set; }
    }
}
