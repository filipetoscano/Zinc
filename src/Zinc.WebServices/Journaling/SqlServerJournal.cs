using System;
using System.Threading.Tasks;

namespace Zinc.WebServices.Journaling
{
    /// <summary />
    public class SqlServerJournal : IExecutionJournal
    {
        /// <summary />
        public Task PreAsync( ExecutionContext context, object request )
        {
            // TODO:
            return Task.CompletedTask;
        }


        /// <summary />
        public Task PostAsync( ExecutionContext context, object response )
        {
            // TODO:
            return Task.CompletedTask;
        }


        /// <summary />
        public Task FullAsync( ExecutionContext context, object request, object response )
        {
            // TODO:
            return Task.CompletedTask;
        }
    }
}
