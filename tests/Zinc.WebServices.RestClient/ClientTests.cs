using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zn.Sample;

namespace Zinc.WebServices.RestClient.Test
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public void MethodOneAsync1()
        {
            OneClient svc = new OneClient();

            var response = svc.MethodOneAsync( new Zn.Sample.OneSvc.MethodOneRequest()
            {
                Value = 10
            } ).GetAwaiter().GetResult();

            Assert.AreEqual( 100, response.Value );
        }


        [TestMethod]
        public void MethodOneAsync2()
        {
            OneClient svc = new OneClient();

            try
            {
                svc.MethodOneAsync( new Zn.Sample.OneSvc.MethodOneRequest()
                {
                    Value = 15
                } ).GetAwaiter().GetResult();

                Assert.Fail( "Expected exception." );
            }
            catch ( ServiceFaultException ex )
            {
                // Expecting: Unhandled exception
                Assert.AreEqual( "ZnSample.Zinc", ex.Actor );
                Assert.AreEqual( 4006, ex.Code );
            }
            catch ( Exception ex )
            {
                Assert.Fail( "Expected named exception: " + ex.GetType().FullName );
            }
        }
    }
}
