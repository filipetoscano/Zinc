using System;
using System.Text;
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
            var ssvc = new SampleSvcClient();
            var beer = await ssvc.PingAsync();


            /*
             * 
             */
            var outValue = -1;

            if ( request.InValue >= 0 )
            {

                var svc = new OneClient();
                svc.ActivityId = this.Context.ActivityId;

                var t1 = svc.MethodOneAsync( new OneSvc.MethodOneRequest()
                {
                    Value = request.InValue,
                } );

                var t2 = svc.MethodOneAsync( new OneSvc.MethodOneRequest()
                {
                    Value = request.InValue + 5,
                } );


                /*
                 * 
                 */
                await Task.WhenAll( t1, t2 );

                outValue = t1.Result.Value + t2.Result.Value;
            }


            /*
             * 
             */
            return new MethodTwoResponse()
            {
                OutValue = outValue,
                OutDate = request.InDate,
                OutTime = request.InTime,
                OutDateTime = request.InDateTime,
                BinaryValue = Encoding.UTF8.GetBytes( "hello world" ),
                ActivityId = this.Context.ActivityId,
                AccessToken = this.Context.AccessToken,
                ExecutionId = this.Context.ExecutionId,
                OutEnum = request.InEnum,
            };
        }
    }
}