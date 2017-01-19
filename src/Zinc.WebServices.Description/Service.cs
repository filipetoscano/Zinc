using System.Collections.Generic;

namespace Zinc.WebServices.Description
{
    /// <summary>
    /// Describes a service.
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Gets name of service.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets summary / description.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets remarks.
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// Gets list of methods defined in service.
        /// </summary>
        public List<Method> Methods { get; set; }
    }
}
