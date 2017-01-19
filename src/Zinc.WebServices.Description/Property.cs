using System;

namespace Zinc.WebServices.Description
{
    /// <summary>
    /// Describes a message/type property.
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// Gets summary / description.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets remarks.
        /// </summary>
        public string Remarks { get; set; }
    }
}
