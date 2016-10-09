using System;

namespace Zinc.WebServices.Journaling
{
    /// <summary />
    public class SqlServerJournal : IExecutionJournal
    {
        /// <summary />
        public void Pre( ExecutionContext context, object request )
        {
            // TODO:
        }


        /// <summary />
        public void Post( ExecutionContext context, object response )
        {
            // TODO:
        }


        /// <summary />
        public void Full( ExecutionContext context, object request, object response )
        {
            // TODO:
        }
    }
}
