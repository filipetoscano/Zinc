using Dapper;
using Platinum;
using Platinum.Data;
using System;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Zinc.WebServices.Journaling
{
    /// <summary />
    public class SqlServerJournal : IExecutionJournal
    {
        /// <summary />
        public async Task PreAsync( ExecutionContext context, object request )
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
            string requestXml = ToXml( request );


            /*
             * 
             */
            DbConnection conn = Db.Connection( "SqlServerLogging" );

            await conn.ExecuteAsync( Q.SqlPre, new
            {
                ExecutionId = context.ExecutionId,
                Method = context.Method,
                ActivityId = context.ActivityId,
                AccessToken = context.AccessToken,
                RequestXml = requestXml,
                MomentStart = context.MomentStart
            } );


            return;
        }


        /// <summary />
        public async Task PostAsync( ExecutionContext context, object response, ActorException error )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            #endregion

            if ( response == null && error == null )
                throw new ArgumentOutOfRangeException( nameof( response ), "Field may not be null, if error is also null." );


            /*
             * 
             */
            string responseXml = ToXml( response );
            string errorXml = ToXml( error );


            /*
             * 
             */
            DbConnection conn = Db.Connection( "SqlServerLogging" );

            await conn.ExecuteAsync( Q.SqlPost, new
            {
                ExecutionId = context.ExecutionId,
                ResponseXml = responseXml,
                ErrorXml = errorXml,
                MomentEnd = context.MomentEnd
            } );
        }


        /// <summary />
        public async Task FullAsync( ExecutionContext context, object request, object response, ActorException error )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( request == null )
                throw new ArgumentNullException( nameof( request ) );

            if ( response == null )
                throw new ArgumentNullException( nameof( response ) );

            #endregion


            /*
             * 
             */
            string requestXml = ToXml( request );
            string responseXml = ToXml( response );
            string errorXml = ToXml( error );


            /*
             * 
             */
            DbConnection conn = Db.Connection( "SqlServerLogging" );

            await conn.ExecuteAsync( Q.SqlFull, new
            {
                ExecutionId = context.ExecutionId,
                Method = context.Method,
                ActivityId = context.ActivityId,
                AccessToken = context.AccessToken,
                RequestXml = requestXml,
                ResponseXml = responseXml,
                ErrorXml = errorXml,
                MomentStart = context.MomentStart,
                MomentEnd = context.MomentEnd
            } );
        }


        private static string ToXml( object obj )
        {
            if ( obj == null )
                return null;

            XmlSerializer ser = new XmlSerializer( obj.GetType() );

            using ( var sw = new StringWriter() )
            {
                using ( var xw = XmlWriter.Create( sw ) )
                {
                    ser.Serialize( xw, obj );
                    return sw.ToString();
                }
            }
        }
    }
}
