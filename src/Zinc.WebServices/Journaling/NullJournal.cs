using Platinum;
using System;
using System.Threading.Tasks;

namespace Zinc.WebServices.Journaling
{
    /// <summary />
    public class NullJournal : IExecutionJournal
    {
        /// <summary />
        public Task PreAsync( ExecutionContext context, object request )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( request == null )
                throw new ArgumentNullException( nameof( request ) );

            #endregion

            return Task.CompletedTask;
        }


        /// <summary />
        public Task PostAsync( ExecutionContext context, object response, ActorException error )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            #endregion

            return Task.CompletedTask;
        }


        /// <summary />
        public Task FullAsync( ExecutionContext context, object request, object response, ActorException error )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( request == null )
                throw new ArgumentNullException( nameof( request ) );

            #endregion

            return Task.CompletedTask;
        }
    }
}
