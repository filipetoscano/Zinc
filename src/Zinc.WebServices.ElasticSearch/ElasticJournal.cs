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
            LogLevel level = LogLevel.Debug;

            if ( error != null )
            {
                if ( error.Actor.EndsWith( ".Client" ) == true )
                    level = LogLevel.Info;
                else
                    level = LogLevel.Error;
            }


            /*
             * 
             */
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
            LogLevel level = LogLevel.Debug;

            if ( error != null )
                level = LogLevel.Error;


            /*
             * 
             */
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


            /*
             * 
             */
            logger.Log( level, "Zn.PreAsync", context, request );

            return Task.CompletedTask;
        }
    }
}
