namespace Zinc.WebService.ServiceUtil
{
    /// <summary>
    /// Describes the type of .NET type contained in the service description file.
    /// </summary>
    public enum TypeofType
    {
        /// <summary>
        /// Service interface.
        /// </summary>
        Interface = 0,

        /// <summary>
        /// Client class, which implements service interface.
        /// </summary>
        ClientClass = 1,

        /// <summary>
        /// User type, used to transport data.
        /// </summary>
        UserType = 2,
    }
}
