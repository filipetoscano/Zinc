using System;
using System.Collections.Generic;

namespace Zinc.WebServices.Description
{
    /// <summary>
    /// Describes a method request/response message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets the .NET runtime type.
        /// </summary>
        public Type MessageType { get; set; }

        /// <summary>
        /// Gets the list of properties on a method message.
        /// </summary>
        public List<Property> Properties { get; set; }
    }
}
