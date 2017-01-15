using System;
using System.Threading.Tasks;

namespace Zn.Sample.TwoService
{
    public partial class MethodTwoImplementation
    {
        /// <summary />
        public async Task<MethodTwoResponse> InnerRun( MethodTwoRequest request )
        {
            await Task.Delay( 100 );

            return new MethodTwoResponse()
            {
                RandomString = "not",
                DataString = "data",
                RandomDate = DateTime.Now,
                RandomDateTime = DateTime.Now
            };
        }
    }
}