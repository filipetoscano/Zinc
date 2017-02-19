using Newtonsoft.Json;
using Platinum;
using Platinum.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zinc.WebServices.RestClient
{
    /// <summary />
    public abstract class ServiceClient
    {
        /// <summary />
        public ServiceClient( string application )
        {
            #region Validations

            if ( application == null )
                throw new ArgumentNullException( nameof( application ) );

            #endregion

            this.Application = application;
            this.Service = this.GetType().Name.Substring( 0, this.GetType().Name.Length - "Client".Length );

            string key = "Service:" + application;
            string moduleBase = AppConfiguration.Get<string>( key );
            this.BaseUrl = moduleBase + "api/" + this.Service;
        }


        /// <summary />
        public Guid ActivityId
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the name of the Service application.
        /// </summary>
        private string Application
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the name of the Service service.
        /// </summary>
        private string Service
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the URL for the current service.
        /// </summary>
        private string BaseUrl
        {
            get;
            set;
        }


        /// <summary />
        [ಠ_ಠ( "Needs error-handling." )]
        protected Tp Invoke<Tq, Tp>( string method, Tq request )
        {
            string json = JsonConvert.SerializeObject( request );

            string url = this.BaseUrl + "/" + method;

            HttpWebRequest webRequest = (HttpWebRequest) HttpWebRequest.Create( url );
            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";
            webRequest.ServicePoint.Expect100Continue = false;


            /*
             * 
             */
            Stream reqs = webRequest.GetRequestStream();

            StreamWriter sw = new StreamWriter( reqs );
            sw.WriteLine( json );
            sw.Close();

            reqs.Close();


            /*
             * 
             */
            HttpWebResponse webResponse = (HttpWebResponse) webRequest.GetResponse();

            Stream resp = webResponse.GetResponseStream();

            StreamReader sr = new StreamReader( resp );
            string jsonResp = sr.ReadToEnd();

            sr.Close();
            resp.Close();
            webResponse.Close();

            return JsonConvert.DeserializeObject<Tp>( jsonResp );
        }


        /// <summary />
        protected async Task<Tp> InvokeAsync<Tq, Tp>( string method, Tq request, CancellationToken cancellationToken )
        {
            #region Validations

            if ( method == null )
                throw new ArgumentNullException( nameof( method ) );

            if ( request == null )
                throw new ArgumentNullException( nameof( request ) );

            #endregion

            string url = this.BaseUrl + "/" + method;

            using ( var http = new HttpClient() )
            {
                var content = new StringContent( JsonConvert.SerializeObject( request ) );
                content.Headers.ContentType.MediaType = "application/json";
                content.Headers.Add( "X-ActivityId", this.ActivityId.ToString() );

                var reqm = new HttpRequestMessage();
                reqm.Content = content;
                reqm.Method = HttpMethod.Post;
                reqm.RequestUri = new Uri( url, UriKind.Absolute );


                /*
                 * 
                 */
                HttpResponseMessage resp;

                try
                {
                    resp = await http.SendAsync( reqm, HttpCompletionOption.ResponseContentRead, cancellationToken ).ConfigureAwait( false );
                }
                catch ( HttpRequestException ex )
                {
                    if ( ex.InnerException is WebException )
                    {
                        WebException wex = (WebException) ex.InnerException;

                        throw new ServiceException( ER.InvokeAsync_Send_WebException, url, wex.Status );
                    }
                    else
                    {
                        throw new ServiceException( ER.InvokeAsync_Send_HttpRequestException, ex, url );
                    }
                }
                catch ( Exception ex )
                {
                    throw new ServiceException( ER.InvokeAsync_Send_Exception, ex, url );
                }


                /*
                 * 
                 */
                var data = await resp.Content.ReadAsByteArrayAsync().ConfigureAwait( false );
                var status = resp.StatusCode;

                if ( status == HttpStatusCode.InternalServerError )
                {
                    var err = Encoding.UTF8.GetString( data );
                    ServiceFault fault;

                    try
                    {
                        fault = JsonConvert.DeserializeObject<ServiceFault>( err );
                    }
                    catch ( JsonSerializationException ex )
                    {
                        throw new ServiceException( ER.InvokeAsync_Fault_Deserialize, ex, url );
                    }

                    throw new ServiceFaultException( url, fault );
                }

                if ( status != HttpStatusCode.OK )
                {
                    ServiceFault fault = new ServiceFault();
                    fault.Actor = this.Application + "Svc";
                    fault.Code = 1000;
                    fault.Message = $"Status '{ status }' invoking service.";

                    throw new ServiceFaultException( url, fault );
                }


                /*
                 * 
                 */
                var str = Encoding.UTF8.GetString( data );
                Tp response;

                try
                {
                    response = JsonConvert.DeserializeObject<Tp>( str );
                }
                catch ( JsonSerializationException ex )
                {
                    throw new ServiceException( ER.InvokeAsync_Response_Deserialize, ex, url );
                }

                return response;
            }
        }
    }
}
