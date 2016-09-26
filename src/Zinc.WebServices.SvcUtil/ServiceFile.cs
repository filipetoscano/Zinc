using System.Collections.Generic;

namespace Zinc.WebService.ServiceUtil
{
    /// <summary>
    /// Describes the content of a service description file.
    /// </summary>
    public class ServiceFile
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string Filename;

        /// <summary>
        /// Gets or sets the namespace in which types were defined.
        /// </summary>
        public string Namespace;

        /// <summary>
        /// Gets or sets the list of types defined in the file.
        /// </summary>
        public List<TypeDefinition> Types;
    }
}
