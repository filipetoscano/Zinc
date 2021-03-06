﻿using Platinum;
using System.Threading.Tasks;

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
        Task PreAsync( ExecutionContext context, object request );


        /// <summary>
        /// Logs response, after to method execution.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <param name="response">Response.</param>
        /// <param name="error">Error.</param>
        Task PostAsync( ExecutionContext context, object response, ActorException error );


        /// <summary>
        /// Logs request and response, after to method execution.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <param name="request">Request.</param>
        /// <param name="response">Response.</param>
        /// <param name="error">Error.</param>
        Task FullAsync( ExecutionContext context, object request, object response, ActorException error );
    }
}
