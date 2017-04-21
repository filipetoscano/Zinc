using System;
using System.Web.Http;
using System.Web.Http.Description;

namespace Zinc.WebServices
{
    /// <summary />
    [ApiExplorerSettings( IgnoreApi = true )]
    public class PingController : ApiController
    {
        /// <summary />
        [HttpGet]
        [Route( "ping" )]
        public PingController.PingResponse Ping()
        {
            return new PingController.PingResponse()
            {
                Pong = DateTime.UtcNow
            };
        }


        /// <summary />
        [HttpPost]
        [Route( "ping" )]
        public PingController.PingResponse Ping( PingController.PingRequest request )
        {
            return new PingController.PingResponse()
            {
                Pong = DateTime.UtcNow
            };
        }


        /// <summary />
        public class PingRequest
        {
        }


        /// <summary />
        public class PingResponse
        {
            /// <summary />
            public DateTime Pong { get; set; }
        }
    }
}
