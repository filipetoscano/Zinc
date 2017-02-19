using System.Collections.Generic;

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
    }
}
