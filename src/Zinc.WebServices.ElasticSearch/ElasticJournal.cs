using NLog;
using Platinum;
using System;
using System.Threading.Tasks;
using Zinc.WebServices.Journaling;

namespace Zinc.WebServices.ElasticSearch
{
    /// <summary />
    public class ElasticJournal : IExecutionJournal
    {
        /// <summary />
        private static Logger logger = LogManager.GetCurrentClassLogger();


        /// <summary />
        public Task FullAsync( ExecutionContext context, object request, object response, ActorException error )
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
            LogLevel level = LogLevelFor( error );
            logger.Log( level, "Zn.FullAsync", context, request, response, error );

            return Task.CompletedTask;
        }


        /// <summary />
        public Task PostAsync( ExecutionContext context, object response, ActorException error )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            #endregion


            /*
             * 
             */
            LogLevel level = LogLevelFor( error );
            logger.Log( level, "Zn.PostAsync", context, response, error );

            return Task.CompletedTask;
        }


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
            LogLevel level = LogLevel.Debug;
            logger.Log( level, "Zn.PreAsync", context, request );

            return Task.CompletedTask;
        }


        /// <summary>
        /// Determines the log level for the current message, based on the value of
        /// the error parameter.
        /// </summary>
        /// <param name="error">
        /// Error which occurred during processing, or null if none.
        /// </param>
        /// <returns>
        /// Log level that the message should be journaled in.
        /// </returns>
        private static LogLevel LogLevelFor( ActorException error )
        {
            if ( error == null )
                return LogLevel.Debug;

            LogLevel level;

            if ( error.Actor.EndsWith( ".Client" ) == true )
                level = LogLevel.Warn;
            else
                level = LogLevel.Error;

            if ( error.Data.Contains( "Pt.Level" ) == true )
            {
                try
                {
                    string value = (string) error.Data[ "Pt.Level" ];
                    level = LogLevel.FromString( value );
                }
                catch ( Exception )
                {
                    // Snuff it!
                }
            }

            return level;
        }
    }
}
