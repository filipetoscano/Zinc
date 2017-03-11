using System;
using System.Web.Http;

namespace Zinc.WebServices.Rest
{
    /// <summary />
    public static class WebApiConfig
    {
        /// <summary />
        [Obsolete( "Use 'ZincApiConfig.Register' instead, or .UseZinc on HttpConfiguration instance." )]
        public static void Register( HttpConfiguration config )
        {
            ZincApiConfig.Register( config );
        }
    }
}
