using Dapper;
using Platinum.Data;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Zinc.WebServices.Journaling
{
    /// <summary />
    public class SqlServerJournal : IExecutionJournal
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


            /*
             * 
             */
            DbConnection conn = Db.Connection( "SqlServerLogging" );




            // TODO:
            return Task.CompletedTask;
        }


        /// <summary />
        public Task PostAsync( ExecutionContext context, object response )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( response == null )
                throw new ArgumentNullException( nameof( response ) );

            #endregion


            // TODO:
            return Task.CompletedTask;
        }


        /// <summary />
        public Task FullAsync( ExecutionContext context, object request, object response )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( request == null )
                throw new ArgumentNullException( nameof( request ) );

            if ( response == null )
                throw new ArgumentNullException( nameof( response ) );

            #endregion


            // TODO:
            return Task.CompletedTask;
        }
    }
}
