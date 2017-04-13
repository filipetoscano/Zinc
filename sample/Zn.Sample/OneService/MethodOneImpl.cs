using System;
using System.Threading.Tasks;

namespace Zn.Sample.OneService
{
    public partial class MethodOneImplementation
    {
        /// <summary />
        public Task<MethodOneResponse> InnerRun( MethodOneRequest request )
        {
            /*
             * Handled exception
             */
            if ( request.Value > 100 )
            {
                SampleException iex = new SampleException( ER.One_Error1, "V1" );
                throw new SampleException( ER.One_Error2, iex, "V2", "V3" );
            }


            /*
             * Unhandled exception
             */
            if ( request.Value > 10 )
                throw new Exception( "oops" );


            /*
             * Normal behavior
             */
            return Task.FromResult( new MethodOneResponse()
            {
                Value = request.Value * 10
            } );
        }
    }
}