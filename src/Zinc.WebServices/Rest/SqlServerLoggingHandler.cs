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
        protected override async Task HandleRequest( RestExecutionContext context, byte[] message )
        {
            string asString = Encoding.UTF8.GetString( message );
            await Journal( context, 0, null, asString );
        }


        protected override async Task HandleResponse( RestExecutionContext context, HttpStatusCode statusCode, byte[] message )
        {
            // TODO: What about binary files? :/
            string asString = Encoding.UTF8.GetString( message );
            await Journal( context, 1, statusCode, asString );
        }


        private async Task Journal( RestExecutionContext context, int step, HttpStatusCode? statusCode, string message )
        {
            const string Database = "SqlServerLogging";

            if ( ConfigurationManager.ConnectionStrings[ Database ] == null )
                throw new WsException( ER.Rest_SqlServer_ConnectionMissing, Database );


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
                throw new WsException( ER.Rest_SqlServer_Open, ex, Database );
            }
            catch ( ConfigurationErrorsException ex )
            {
                throw new WsException( ER.Rest_SqlServer_Open, ex, Database );
            }
            catch ( InvalidOperationException ex )
            {
                throw new WsException( ER.Rest_SqlServer_Open, ex, Database );
            }


            /*
             * 
             */
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "insert into REST_JOURNAL ( ActivityId, ExecutionId, Method, URI, Step, StatusCode, JsonMessage, Moment ) "
                            + "values ( @ActivityId, @ExecutionId, @Method, @URI, @Step, @StatusCode, @JsonMessage, @Moment ) ";
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.Add( "@ActivityId", SqlDbType.UniqueIdentifier ).Value = context.ActivityId;
            cmd.Parameters.Add( "@ExecutionId", SqlDbType.UniqueIdentifier ).Value = context.ExecutionId;
            cmd.Parameters.Add( "@Method", SqlDbType.NVarChar ).Value = context.Method.ToString();
            cmd.Parameters.Add( "@URI", SqlDbType.NVarChar ).Value = context.RequestUri.ToString();
            cmd.Parameters.Add( "@Step", SqlDbType.Int ).Value = step;

            if ( statusCode.HasValue == true )
                cmd.Parameters.Add( "@StatusCode", SqlDbType.NVarChar ).Value = statusCode.ToString();
            else
                cmd.Parameters.Add( "@StatusCode", SqlDbType.NVarChar ).Value = DBNull.Value;

            cmd.Parameters.Add( "@JsonMessage", SqlDbType.NVarChar ).Value = message;
            cmd.Parameters.Add( "@Moment", SqlDbType.DateTime ).Value = DateTime.UtcNow;

            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch ( SqlException ex )
            {
                throw new WsException( ER.Rest_SqlServer_ExecuteNonQuery, ex, Database );
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
