using Zinc.WebServices.RestClient;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zn.Sample
{
    /// <summary />
    public partial class OneClient
    {
        /// <summary>
        /// Fake method.
        /// </summary>
        public OneSvc.MethodOneResponse MethodFake( OneSvc.MethodOneRequest request )
        {
            return Invoke<OneSvc.MethodOneRequest, OneSvc.MethodOneResponse>( "MethodFake", request );
        }

        /// <summary>
        /// Fake method.
        /// </summary>
        public Task<OneSvc.MethodOneResponse> MethodFakeAsync( OneSvc.MethodOneRequest request )
        {
            return MethodFakeAsync( request, CancellationToken.None );
        }

        /// <summary>
        /// Fake method.
        /// </summary>
        public Task<OneSvc.MethodOneResponse> MethodFakeAsync( OneSvc.MethodOneRequest request, CancellationToken cancellationToken )
        {
            return InvokeAsync<OneSvc.MethodOneRequest, OneSvc.MethodOneResponse>( "MethodFake", request, cancellationToken );
        }
    }
}