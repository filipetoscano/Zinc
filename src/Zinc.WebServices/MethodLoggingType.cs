namespace Zinc.WebServices
{
    /// <summary>
    /// When the request/response messages are logged.
    /// </summary>
    public enum MethodLoggingType
    {
        /// <summary>
        /// Request is logged prior to method execution, and response is
        /// logged after method execution.
        /// </summary>
        PrePost,

        /// <summary>
        /// Request and response are logged after method execution.
        /// </summary>
        Post,
    }
}
