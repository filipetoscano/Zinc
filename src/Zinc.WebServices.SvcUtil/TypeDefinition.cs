namespace Zinc.WebService.ServiceUtil
{
    /// <summary>
    /// Describes a type .NET as defined in the description file.
    /// </summary>
    public class TypeDefinition
    {
        /// <summary>
        /// Gets or sets the name of the type defined in the class.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of the type. :)
        /// </summary>
        public TypeofType TypeType { get; set; }

        /// <summary>
        /// C# definition of the type, including leading [Attributes].
        /// </summary>
        public string Definition { get; set; }
    }
}
