using System;

namespace Zn.Sample.TwoService
{
    public partial class MethodTwoImplementation
    {
        /// <summary />
        public MethodTwoResponse InnerRun( MethodTwoRequest request )
        {
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