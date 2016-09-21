using System.ServiceModel;
using System.Xml.Serialization;
using Zinc.WebServices;

namespace Zn.Sample.OneService
{
    public partial class MethodOneImplementation
    {
        /// <summary />
        public MethodOneResponse InnerRun( MethodOneRequest request )
        {
            if ( request.Value > 10 )
                throw new System.Exception( "oops" );

            return new MethodOneResponse()
            {
                Value = request.Value * 10
            };
        }
    }
}