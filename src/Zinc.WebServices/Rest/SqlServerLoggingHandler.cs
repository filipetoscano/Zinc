using Platinum;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Zinc.WebServices.Rest
{
    /// <summary>
    /// Logs request/response REST messages to SQL server.
    /// </summary>
    public class SqlServerLoggingHandler : LoggingHandler
    {
        /// <summary />
        protected override async Task HandleRequest( RestExecutionContext context, byte[] message )
        {
            string asString = Encoding.UTF8.GetString( message );
            await Journal( context, false, null, asString );
        }


        /// <summary />
        protected override async Task HandleResponse( RestExecutionContext context, HttpStatusCode statusCode, byte[] message )
        {
            // TODO: What about binary files? :/
            string asString = Encoding.UTF8.GetString( message );
            await Journal( context, true, statusCode, asString );
        }


        private async Task Journal( RestExecutionContext context, bool direction, HttpStatusCode? statusCode, string message )
        {
            const string Database = "SqlServerLogging";

            if ( ConfigurationManager.ConnectionStrings[ Database ] == null )
                throw new ZincException( ER.Rest_SqlServer_ConnectionMissing, Database );


            /*
             * 
             */
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings[ Database ].ConnectionString;

            try
            {
                await conn.OpenAsync();
            }
            catch ( SqlException ex )
            {
                throw new ZincException( ER.Rest_SqlServer_Open, ex, Database );
            }
            catch ( ConfigurationErrorsException ex )
            {
                throw new ZincException( ER.Rest_SqlServer_Open, ex, Database );
            }
            catch ( InvalidOperationException ex )
            {
                throw new ZincException( ER.Rest_SqlServer_Open, ex, Database );
            }


            /*
             * 
             */
            object accessToken;
            object statusCodeStr;

            if ( context.AccessToken != null )
                accessToken = context.AccessToken;
            else
                accessToken = DBNull.Value;

            if ( statusCode.HasValue == true )
                statusCodeStr = statusCode.ToString();
            else
                statusCodeStr = DBNull.Value;


            /*
             * 
             */
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "insert into ZN_REST_JOURNAL ( Application, ActivityId, AccessToken, ExecutionId, Method, URI, Direction, StatusCode, JsonMessage, Moment ) "
                            + "values ( @Application, @ActivityId, @AccessToken, @ExecutionId, @Method, @URI, @Direction, @StatusCode, @JsonMessage, @Moment ) ";
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.Add( "@Application", SqlDbType.VarChar ).Value = App.Name;
            cmd.Parameters.Add( "@ActivityId", SqlDbType.UniqueIdentifier ).Value = context.ActivityId;
            cmd.Parameters.Add( "@AccessToken", SqlDbType.VarChar ).Value = accessToken;
            cmd.Parameters.Add( "@ExecutionId", SqlDbType.UniqueIdentifier ).Value = context.ExecutionId;
            cmd.Parameters.Add( "@Method", SqlDbType.NVarChar ).Value = context.Method.ToString();
            cmd.Parameters.Add( "@URI", SqlDbType.NVarChar ).Value = context.RequestUri.ToString();
            cmd.Parameters.Add( "@Direction", SqlDbType.Bit ).Value = direction;
            cmd.Parameters.Add( "@StatusCode", SqlDbType.NVarChar ).Value = statusCodeStr;
            cmd.Parameters.Add( "@JsonMessage", SqlDbType.NVarChar ).Value = message;
            cmd.Parameters.Add( "@Moment", SqlDbType.DateTime ).Value = DateTime.UtcNow;

            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch ( SqlException ex )
            {
                throw new ZincException( ER.Rest_SqlServer_ExecuteNonQuery, ex, Database );
            }


            /*
             * 
             */
            try
            {
                conn.Close();
            }
            catch ( SqlException )
            {
            }
        }
    }
}
