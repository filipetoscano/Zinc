using System;
using System.Net.Http;

namespace Zinc.WebServices.Rest
{
    public class RestExecutionContext
    {
        public Guid ExecutionId;
        public Guid ActivityId;
        public HttpMethod Method;
        public Uri RequestUri;
    }
}
