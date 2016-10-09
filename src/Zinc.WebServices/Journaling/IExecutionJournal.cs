namespace Zinc.WebServices.Journaling
{
    /// <summary>
    /// Describes an execution logger.
    /// </summary>
    public interface IExecutionJournal
    {
        /// <summary>
        /// Logs request, prior to method execution.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <param name="request">Request.</param>
        void Pre( ExecutionContext context, object request );


        /// <summary>
        /// Logs response, after to method execution.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <param name="response">Response.</param>
        void Post( ExecutionContext context, object response );


        /// <summary>
        /// Logs request and response, after to method execution.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <param name="request">Request.</param>
        /// <param name="response">Response.</param>
        void Full( ExecutionContext context, object request, object response );
    }
}
