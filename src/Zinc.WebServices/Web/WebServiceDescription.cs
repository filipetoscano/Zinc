using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Reflection;
using System.Linq;

namespace Zinc.WebServices.Web
{
    /// <summary />
    public class WebServiceDescription
    {
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the description of the service.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the namespace of the service.
        /// </summary>
        public string Namespace { get; set; }


        /// <summary>
        /// Loads all of the web services available in the assembly where
        /// the given type is specified,
        /// </summary>
        /// <param name="type">
        /// Type.
        /// </param>
        /// <returns>
        /// List of web services.
        /// </returns>
        public static List<WebServiceDescription> Load( Type type )
        {
            return Load( type.Assembly );
        }


        /// <summary>
        /// Loads all of the web services available in the given assembly.
        /// </summary>
        /// <param name="assembly">
        /// Assembly.
        /// </param>
        /// <returns>
        /// List of web services.
        /// </returns>
        public static List<WebServiceDescription> Load( Assembly assembly )
        {
            #region Validations

            if ( assembly == null )
                throw new ArgumentNullException( nameof( assembly ) );

            #endregion

            var services = new List<WebServiceDescription>();

            foreach ( Type type in (from t in assembly.GetTypes()
                                  where t.IsInterface == false
                                  where t.GetCustomAttribute<ServiceBehaviorAttribute>() != null
                                  select t) )
            {
                var sd = new WebServiceDescription();
                sd.Name = type.Name.Substring( 0, type.Name.Length - "Services".Length );
                sd.Namespace = type.GetCustomAttribute<ServiceBehaviorAttribute>().Namespace;

                services.Add( sd );
            }

            return services;
        }
    }
}
