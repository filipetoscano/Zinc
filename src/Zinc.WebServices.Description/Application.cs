using System.Collections.Generic;

namespace Zinc.WebServices.Description
{
    /// <summary>
    /// Describes an application.
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Gets name of application.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets list of services contained in application.
        /// </summary>
        public List<Service> Services { get; set; }

        /// <summary>
        /// Gets list of model types used by application.
        /// </summary>
        public List<ServiceType> Models { get; set; }
    }
}
