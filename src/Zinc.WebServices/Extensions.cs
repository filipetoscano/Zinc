using System.Web.Http;

namespace Zinc.WebServices
{
    /// <summary />
    public static class Extensions
    {
        /// <summary />
        public static void UseZinc( this HttpConfiguration config )
        {
            ZincApiConfig.Register( config );
        }
    }
}
