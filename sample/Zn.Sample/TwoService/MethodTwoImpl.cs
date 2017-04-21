using System;
using System.Threading.Tasks;

namespace Zn.Sample.TwoService
{
    public partial class MethodTwoImplementation
    {
        /// <summary />
        public async Task<MethodTwoResponse> InnerRun( MethodTwoRequest request )
        {
            /*
             * 
             */
            Zn.Sample.OneClient svc = new OneClient();

            var resp1 = await svc.MethodOneAsync( new OneSvc.MethodOneRequest()
            {
                Value = request.InValue,
            } );

            var resp2 = await svc.MethodOneAsync( new OneSvc.MethodOneRequest()
            {
                Value = request.InValue + 5,
            } );


            /*
             * 
             */
            return new MethodTwoResponse()
            {
                OutValue = resp1.Value + resp2.Value,
                RandomString = "not",
                DataString = "data",
                RandomDate = DateTime.Now,
                RandomDateTime = DateTime.Now,
                ActivityId = this.Context.ActivityId,
                AccessToken = this.Context.AccessToken,
                ExecutionId = this.Context.ExecutionId,
                OutEnum = request.InEnum,
            };
        }
    }
}