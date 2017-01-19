namespace Zinc.WebServices.Description
{
    /// <summary>
    /// Describes a method.
    /// </summary>
    public class Method
    {
        /// <summary>
        /// Gets name of method.
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
        /// Gets the definition of the request message.
        /// </summary>
        public Message Request { get; set; }

        /// <summary>
        /// Gets the definition of the response message.
        /// </summary>
        public Message Response { get; set; }
    }
}
