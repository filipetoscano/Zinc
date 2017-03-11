using Platinum;
using System;
using System.ServiceModel;

namespace Zinc.WebServices
{
    /// <summary />
    public static class Soap
    {
        /// <summary>
        /// Converts an actor exception into an WCF exception.
        /// </summary>
        /// <param name="exception">
        /// Exception.
        /// </param>
        /// <returns>
        /// WCF exception.
        /// </returns>
        public static FaultException<ActorFault> ToException( ActorException exception )
        {
            ActorFault detail = ActorFault.From( exception );

            string faultCode = "Server";

            if ( detail.Actor.EndsWith( ".Client", StringComparison.Ordinal ) == true )
                faultCode = "Client";

            return new FaultException<ActorFault>( detail, detail.Message, new FaultCode( faultCode ) );
        }


        /// <summary>
        /// Converts an exception into an WCF exception.
        /// </summary>
        /// <param name="exception">
        /// Exception.
        /// </param>
        /// <returns>
        /// WCF exception.
        /// </returns>
        public static FaultException<ActorFault> ToUnhandledException( Exception exception )
        {
            ActorFault detail = ActorFault.FromUnhandled( exception );

            string faultCode = "Server";

            if ( detail.Actor.EndsWith( ".Client", StringComparison.Ordinal ) == true )
                faultCode = "Client";

            return new FaultException<ActorFault>( detail, detail.Message, new FaultCode( faultCode ) );
        }
    }
}
