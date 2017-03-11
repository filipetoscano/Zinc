using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Zinc.WebServices.Rest;

namespace Zinc.WebServices
{
    /// <summary />
    public static class WebApiConfig
    {
        /// <summary />
        public static void Register( HttpConfiguration config )
        {
            #region Validations

            if ( config == null )
                throw new ArgumentNullException( nameof( config ) );

            #endregion


            /*
             * 
             */
            var cs = ZincConfiguration.Current.Rest;

            if ( cs == null )
                return;

            if ( cs.Enabled == false )
                return;


            /*
             * 
             */
            Dictionary<string, string> ext = new Dictionary<string, string>();

            foreach ( var h in cs.Extensions )
                ext.Add( h.Name, h.Type );


            /*
             * MessageHandlers
             */
            if ( cs.Handlers != null )
            {
                foreach ( var e in cs.Handlers.Elements() )
                {
                    if ( ext.ContainsKey( e.Name.LocalName ) == false )
                        throw new ZincConfigurationException( ER.Rest_Configuration_MissingHandler, e.Name.LocalName );

                    string type = ext[ e.Name.LocalName ];

                    DelegatingHandler handler = Platinum.Activator.Create<DelegatingHandler>( type );
                    config.MessageHandlers.Add( handler );
                }
            }


            /*
             * Exception handling:
             *   - Filter: Exceptions returned from the WebApi are 'reworked'.
             *   - Services: Global exception handling.
             */
            config.Filters.Add( new HandleExceptionFilter() );
            config.Services.Replace( typeof( System.Web.Http.ExceptionHandling.IExceptionHandler ), new ExceptionHandler() );


            /*
             * Clear all of the formatters, and *only* add JSON.
             */
            config.Formatters.Clear();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Include;
            settings.Converters.Add( new StringEnumConverter() { AllowIntegerValues = true } );

            var formatter = new JsonNetFormatter( settings );
            formatter.SupportedMediaTypes.Add( new MediaTypeHeaderValue( "application/json" ) );
            formatter.SupportedMediaTypes.Add( new MediaTypeHeaderValue( "application/javascript" ) );

            config.Formatters.Add( formatter );


            /*
             * Web API routes
             */
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "ActionApi",
            //    routeTemplate: "api/{controller}/{action}",
            //    defaults: new { }
            //);
        }
    }
}
