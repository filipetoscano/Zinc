using System;
using System.Xml.Serialization;

namespace Zinc.WebServices
{
    /// <summary>
    /// Response header.
    /// </summary>
    [XmlType( Namespace = Zn.Namespace )]
    [XmlRoot( Namespace = Zn.Namespace )]
    public class ExecutionHeader
    {
        /// <summary>
        /// Gets a server-side generated ExecutionId, specified to this
        /// message. This ultimately connects this execution as a participant
        /// to the caller Activity.
        /// </summary>
        public Guid ExecutionId { get; set; }

        /// <summary>
        /// Gets the UTC moment in which the execution of the service began.
        /// </summary>
        public DateTime MomentStart { get; set; }

        /// <summary>
        /// Gets the UTC moment in which the execution of the service ended.
        /// </summary>
        public DateTime MomentEnd { get; set; }
    }
}
