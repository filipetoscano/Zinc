using Platinum;
using System.Collections.Generic;
using System.Linq;

namespace Zinc.WebServices.RestClient
{
    /// <summary />
    public class ServiceFault
    {
        /// <summary />
        public string Actor { get; set; }

        /// <summary />
        public int Code { get; set; }

        /// <summary />
        public string Message { get; set; }

        /// <summary />
        public string ExceptionType { get; set; }

        /// <summary />
        public List<ServiceFault> InnerFaults { get; set; }

        /// <summary />
        public string StackTrace { get; set; }


        /// <summary />
        public ServiceFaultException AsException()
        {
            ActorException innerException = null;

            if ( this.InnerFaults != null && this.InnerFaults.Count > 0 )
            {
                if ( this.InnerFaults.Count == 1 )
                {
                    innerException = this.InnerFaults[ 0 ].AsException();
                }
                else
                {
                    var exceptions = this.InnerFaults.Select( x => x.AsException() );
                    innerException = new ActorAggregateException( exceptions );
                }
            }

            return new ServiceFaultException( this.Actor, this.Code, this.Message, innerException );
        }
    }
}
