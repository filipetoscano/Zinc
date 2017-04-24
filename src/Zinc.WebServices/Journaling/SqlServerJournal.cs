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
                Application = App.Name,
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
                Application = App.Name,
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


        /// <summary>
        /// Serializes an instance of an object to XML.
        /// </summary>
        /// <param name="obj">
        /// Object.
        /// </param>
        /// <returns>
        /// XML representation of object.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2202:Do not dispose objects multiple times" )]
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
                }

                return sw.ToString();
            }
        }


        /// <summary>
        /// Serializes an instance of <see cref="ActorException" /> to XML.
        /// </summary>
        /// <param name="error">
        /// Exception.
        /// </param>
        /// <returns>
        /// XML representation of error.
        /// </returns>
        private static string ToXml( ActorException error )
        {
            if ( error == null )
                return null;

            var fault = ActorFault.From( error, true );
            return ToXml( fault );
        }
    }
}
