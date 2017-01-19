using Dapper;
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
                Method = context.Method,
                ActivityId = context.ActivityId,
                ExecutionId = context.ExecutionId,
                Request = requestXml,
                MomentStart = context.MomentStart
            } );


            return;
        }


        /// <summary />
        public async Task PostAsync( ExecutionContext context, object response )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( response == null )
                throw new ArgumentNullException( nameof( response ) );

            #endregion


            /*
             * 
             */
            string responseXml = ToXml( response );


            /*
             * 
             */
            DbConnection conn = Db.Connection( "SqlServerLogging" );

            await conn.ExecuteAsync( Q.SqlPost, new
            {
                Method = context.Method,
                ActivityId = context.ActivityId,
                ExecutionId = context.ExecutionId,
                Response = responseXml,
                MomentEnd = context.MomentEnd
            } );
        }


        /// <summary />
        public async Task FullAsync( ExecutionContext context, object request, object response )
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


            /*
             * 
             */
            DbConnection conn = Db.Connection( "SqlServerLogging" );

            await conn.ExecuteAsync( Q.SqlFull, new
            {
                Method = context.Method,
                ActivityId = context.ActivityId,
                ExecutionId = context.ExecutionId,
                Request = requestXml,
                Response = responseXml,
                MomentStart = context.MomentStart,
                MomentEnd = context.MomentEnd
            } );
        }


        private static string ToXml( object obj )
        {
            #region Validations

            if ( obj == null )
                throw new ArgumentNullException( nameof( obj ) );

            #endregion

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
