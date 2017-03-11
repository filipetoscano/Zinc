using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml.XPath;

namespace Zinc.WebServices.ServiceModel
{
    /// <summary>
    /// Logs inbound WCF messages to SQL Server.
    /// </summary>
    public class SqlServerLoggingMessageInspector : IDispatchMessageInspector
    {
        /// <summary>
        /// Initializes a new instance of the SqlServerLoggingMessageInspector class.
        /// </summary>
        public SqlServerLoggingMessageInspector()
        {
        }


        /// <summary>
        /// Called after an inbound message has been received but before the message is dispatched
        /// to the intended operation.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="channel">The incoming channel.</param>
        /// <param name="instanceContext">The current service instance.</param>
        /// <returns>
        /// An instance of <see cref="WcfExecutionContext" />. This object is passed back to method
        /// <see cref="BeforeSendReply(ref Message, object)" />.
        /// </returns>
        public object AfterReceiveRequest( ref Message request, IClientChannel channel, InstanceContext instanceContext )
        {
            /*
             *
             */
            MessageBuffer buffer = request.CreateBufferedCopy( Int32.MaxValue );
            request = buffer.CreateMessage();


            /*
             *
             */
            WcfExecutionContext ctx = WcfExecutionContext.Read( request );


            /*
             * TODO: pull from config
             */
            var config = new MethodRawLoggingConfiguration() { Enabled = true, Request = true, Response = true };

            if ( config.Enabled == true )
            {
                string message = ToMessage( config.Response, buffer );
                Journal( ctx, 0, message );
            }

            return ctx;
        }


        /// <summary>
        ///  Called after the operation has returned but before the reply message is sent.
        /// </summary>
        /// <param name="reply">
        /// The reply message. This value is null if the operation is one way.
        /// </param>
        /// <param name="correlationState">
        /// The correlation object <see cref="WcfExecutionContext" />.
        /// </param>
        public void BeforeSendReply( ref Message reply, object correlationState )
        {
            if ( reply == null )
                return;

            var ctx = (WcfExecutionContext) correlationState;


            /*
             *
             */
            MessageBuffer buffer = reply.CreateBufferedCopy( Int32.MaxValue );
            reply = buffer.CreateMessage();


            /*
             * TODO: pull from config
             */
            var config = new MethodRawLoggingConfiguration() { Enabled = true, Request = true, Response = true };

            if ( config.Enabled == true )
            {
                string message = ToMessage( config.Response, buffer );
                Journal( ctx, 1, message );
            }
        }


        /// <summary>
        /// Converts the message buffer to the message which is actually journalled in
        /// the database. This will follow/respect the configuration settings and
        /// apply the necessary transformations.
        /// </summary>
        /// <param name="emit">Whether to obtain the message from the buffer or not.</param>
        /// <param name="buffer">Buffer containing message.</param>
        /// <returns>XML message to journal.</returns>
        private static string ToMessage( bool emit, MessageBuffer buffer )
        {
            #region Validations

            if ( buffer == null )
                throw new ArgumentNullException( nameof( buffer ) );

            #endregion

            if ( emit == false )
                return "<not-journaled />";


            /*
             * 
             */
            XPathNavigator nav = buffer.CreateNavigator();
            string message = nav.OuterXml;

            return message;
        }


        /// <summary>
        /// Writes the message to the database.
        /// </summary>
        /// <param name="context">Current execution context.</param>
        /// <param name="step">Step within execution context.</param>
        /// <param name="message">XML message.</param>
        private static void Journal( WcfExecutionContext context, int step, string message )
        {
            const string Database = "SqlServerLogging";

            if ( ConfigurationManager.ConnectionStrings[ Database ] == null )
                throw new ZincException( ER.ServiceModel_SqlServer_ConnectionMissing, Database );


            /*
             * 
             */
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings[ Database ].ConnectionString;

            try
            {
                conn.Open();
            }
            catch ( SqlException ex )
            {
                throw new ZincException( ER.ServiceModel_SqlServer_Open, ex, Database );
            }
            catch ( ConfigurationErrorsException ex )
            {
                throw new ZincException( ER.ServiceModel_SqlServer_Open, ex, Database );
            }
            catch ( InvalidOperationException ex )
            {
                throw new ZincException( ER.ServiceModel_SqlServer_Open, ex, Database );
            }


            /*
             * 
             */
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "insert into WCF_JOURNAL ( ActivityId, ExecutionId, Action, Step, XmlMessage, Moment )"
                            + "values ( @ActivityId, @ExecutionId, @Action, @Step, @XmlMessage, @Moment ) ";
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.Add( "@ActivityId", SqlDbType.UniqueIdentifier ).Value = context.ActivityId;
            cmd.Parameters.Add( "@ExecutionId", SqlDbType.UniqueIdentifier ).Value = context.ExecutionId;
            cmd.Parameters.Add( "@Action", SqlDbType.NVarChar ).Value = context.Action;
            cmd.Parameters.Add( "@Step", SqlDbType.Int ).Value = step;
            cmd.Parameters.Add( "@XmlMessage", SqlDbType.Xml ).Value = message;
            cmd.Parameters.Add( "@Moment", SqlDbType.DateTime ).Value = DateTime.UtcNow;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch ( SqlException ex )
            {
                throw new ZincException( ER.ServiceModel_SqlServer_ExecuteNonQuery, ex, Database );
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
