using Newtonsoft.Json;
using Platinum.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zinc.WebServices.RestClient
{
    /// <summary />
    public abstract class ServiceClient
    {
        private static HttpClient _httpClient = new HttpClient( new HttpClientHandler()
        {
            //UseCookies = false,
            //UseDefaultCredentials = false,
            Proxy = new WebProxy( "http://localhost:8888", false ),
            UseProxy = true,
        } );


        /// <summary />
        public ServiceClient( string application, short version = 1 )
        {
            #region Validations

            if ( application == null )
                throw new ArgumentNullException( nameof( application ) );

            if ( version < 1 )
                throw new ArgumentOutOfRangeException( nameof( version ), "Expected non-negative number." );

            #endregion

            this.Application = application;
            this.Service = this.GetType().Name.StripSuffix( "Client" );

            string key = "Service:" + application;

            this.ServiceUrl = AppConfiguration.Get<string>( key ).EnsureEndsWith( "/" );
            this.BaseUrl = ServiceUrl + "api/v" + version + "/" + this.Service;
        }


        /// <summary />
        public Guid ActivityId
        {
            get;
            set;
        }


        /// <summary />
        public string AccessToken
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
        /// Gets the name of the service.
        /// </summary>
        private string Service
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the URL for the current service.
        /// </summary>
        public string ServiceUrl
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the base URL for methods the current service.
        /// </summary>
        private string BaseUrl
        {
            get;
            set;
        }


        /// <summary />
        protected Tp Invoke<Tq, Tp>( string method, Tq request )
        {
            #region Validations

            if ( method == null )
                throw new ArgumentNullException( nameof( method ) );

            if ( request == null )
                throw new ArgumentNullException( nameof( request ) );

            #endregion

            string json = JsonConvert.SerializeObject( request );
            string url = this.BaseUrl + "/" + method;
            string service = this.Service + "/" + method;


            /*
             * 
             */
            HttpWebRequest webRequest;

            try
            {
                webRequest = (HttpWebRequest) HttpWebRequest.Create( url );
            }
            catch ( Exception ex )
            {
                throw new ServiceException( ER.InvokeSync_Request_Create, ex, this.Application, service, url );
            }

            webRequest.Proxy = null;
            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";
            webRequest.ServicePoint.Expect100Continue = false;

            webRequest.Headers.Add( "Zn-ActivityId", this.ActivityId.ToString() );

            if ( string.IsNullOrEmpty( this.AccessToken ) == false )
                webRequest.Headers.Add( "Authorization", "Bearer " + this.AccessToken );


            /*
             * 
             */
            Stream reqs;

            try
            {
                reqs = webRequest.GetRequestStream();
            }
            catch ( Exception ex )
            {
                throw new ServiceException( ER.InvokeSync_Request_GetStream, ex, this.Application, service );
            }

            StreamWriter sw = new StreamWriter( reqs );
            sw.WriteLine( json );
            sw.Close();

            reqs.Close();


            /*
             * 
             */
            HttpWebResponse webResponse;
            bool isError = false;

            try
            {
                webResponse = (HttpWebResponse) webRequest.GetResponse();
            }
            catch ( WebException ex )
            {
                webResponse = (HttpWebResponse) ex.Response;

                if ( webResponse.StatusCode == HttpStatusCode.Forbidden )
                {
                    webResponse.Dispose();
                    throw new ServiceException( ER.Invoke_Forbidden, this.Application, service );
                }

                if ( webResponse.StatusCode == HttpStatusCode.NotFound )
                {
                    webResponse.Dispose();
                    throw new ServiceException( ER.Invoke_MethodNotFound, this.Application, service );
                }

                if ( webResponse.StatusCode != HttpStatusCode.InternalServerError )
                {
                    webResponse.Dispose();
                    throw new ServiceException( ER.Invoke_NeitherOkNorError, ex, this.Application, service, webResponse.StatusCode );
                }

                isError = true;
            }
            catch ( Exception ex )
            {
                throw new ServiceException( ER.InvokeSync_Response_UnhandledGet, ex, this.Application, service );
            }


            /*
             * 
             */
            if ( webResponse.ContentType.StartsWith( "application/json" ) == false )
            {
                webResponse.Dispose();
                throw new ServiceException( ER.Invoke_NotJson, this.Application, service, webResponse.ContentType );
            }


            /*
             * 
             */
            Stream resp;

            try
            {
                resp = webResponse.GetResponseStream();
            }
            catch ( Exception ex )
            {
                webResponse.Dispose();
                throw new ServiceException( ER.InvokeSync_Response_GetStream, ex, this.Application, service );
            }


            /*
             * 
             */
            StreamReader sr = new StreamReader( resp );
            string jsonResp = sr.ReadToEnd();

            sr.Close();
            resp.Close();
            webResponse.Dispose();


            /*
             * 
             */
            if ( isError == true )
            {
                ServiceFault fault;

                try
                {
                    fault = JsonConvert.DeserializeObject<ServiceFault>( jsonResp );
                }
                catch ( JsonSerializationException ex )
                {
                    throw new ServiceException( ER.Invoke_Fault_Deserialize, ex, this.Application, service );
                }

                throw fault.AsException();
            }


            /*
             * 
             */
            Tp response;

            try
            {
                response = JsonConvert.DeserializeObject<Tp>( jsonResp );
            }
            catch ( JsonSerializationException ex )
            {
                throw new ServiceException( ER.Invoke_Response_Deserialize, ex, this.Application, service );
            }

            return response;
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

            string url;
            string service;

            if ( method == "##ping" )
            {
                url = this.ServiceUrl + "ping";
                service = this.Service + "/ping";
            }
            else
            {
                url = this.BaseUrl + "/" + method;
                service = this.Service + "/" + method;
            }


            /*
             * 
             */
            var content = new StringContent( JsonConvert.SerializeObject( request ) );
            content.Headers.ContentType.MediaType = "application/json";
            content.Headers.Add( "Zn-ActivityId", this.ActivityId.ToString() );


            /*
             * 
             */
            var reqm = new HttpRequestMessage();
            reqm.Content = content;
            reqm.Method = HttpMethod.Post;
            reqm.RequestUri = new Uri( url, UriKind.Absolute );

            if ( string.IsNullOrEmpty( this.AccessToken ) == false )
                reqm.Headers.Authorization = new AuthenticationHeaderValue( "Bearer", this.AccessToken );


            /*
             * 
             */
            HttpResponseMessage resp;

            try
            {
                resp = await _httpClient.SendAsync( reqm, HttpCompletionOption.ResponseHeadersRead, cancellationToken ).ConfigureAwait( false );
            }
            catch ( HttpRequestException ex )
            {
                if ( ex.InnerException is WebException )
                {
                    WebException wex = (WebException) ex.InnerException;

                    throw new ServiceException( ER.InvokeAsync_Send_WebException, this.Application, service, wex.Status );
                }
                else
                {
                    throw new ServiceException( ER.InvokeAsync_Send_HttpRequestException, ex, this.Application, service );
                }
            }
            catch ( Exception ex )
            {
                throw new ServiceException( ER.InvokeAsync_Send_Exception, ex, this.Application, service );
            }


            /*
             * 
             */
            byte[] data;

            using ( resp )
            {
                var status = resp.StatusCode;

                if ( status == HttpStatusCode.Forbidden )
                    throw new ServiceException( ER.Invoke_Forbidden, this.Application, service );

                if ( status == HttpStatusCode.NotFound )
                    throw new ServiceException( ER.Invoke_MethodNotFound, this.Application, service );

                if ( status == HttpStatusCode.InternalServerError )
                {
                    if ( resp.Content.Headers.ContentType.MediaType != "application/json" )
                        throw new ServiceException( ER.Invoke_NotJson, this.Application, service, resp.Content.Headers.ContentType.MediaType );

                    byte[] errData = resp.Content.ReadAsByteArrayAsync().Result;
                    var err = Encoding.UTF8.GetString( errData );
                    ServiceFault fault;

                    try
                    {
                        fault = JsonConvert.DeserializeObject<ServiceFault>( err );
                    }
                    catch ( JsonSerializationException ex )
                    {
                        throw new ServiceException( ER.Invoke_Fault_Deserialize, ex, service );
                    }

                    throw fault.AsException();
                }

                if ( status != HttpStatusCode.OK )
                    throw new ServiceException( ER.Invoke_NeitherOkNorError, this.Application, service, status );

                data = resp.Content.ReadAsByteArrayAsync().Result;
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
                throw new ServiceException( ER.Invoke_Response_Deserialize, ex, service );
            }

            return response;
        }
    }
}
