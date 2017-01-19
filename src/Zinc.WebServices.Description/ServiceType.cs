using System;
using System.Collections.Generic;

namespace Zinc.WebServices.Description
{
    /// <summary>
    /// Describes a service type, aka domain model.
    /// </summary>
    public class ServiceType
    {
        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the .NET runtime type.
        /// </summary>
        public Type ModelType { get; set; }

        /// <summary>
        /// Gets the list of properties.
        /// </summary>
        public List<Property> Properties { get; set; }
    }
}
