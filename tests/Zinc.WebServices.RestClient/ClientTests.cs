using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zn.Sample;

namespace Zinc.WebServices.RestClient.Test
{
    [TestClass]
    public class ClientTests
    {
        /// <summary>
        /// MethodOne with 10, works ok.
        /// </summary>
        [TestMethod]
        public void MethodOneAsyncOk()
        {
            OneClient svc = new OneClient();
            svc.ActivityId = Guid.NewGuid();
            svc.AccessToken = "my.jwt.token";

            var response = svc.MethodOneAsync( new Zn.Sample.OneSvc.MethodOneRequest()
            {
                Value = 10
            } ).GetAwaiter().GetResult();

            Assert.AreEqual( 100, response.Value );
        }


        /// <summary>
        /// MethodOne with 15, throws unhandled exception.
        /// </summary>
        [TestMethod]
        public void MethodOneAsyncNotOk()
        {
            OneClient svc = new OneClient();
            svc.ActivityId = Guid.NewGuid();
            svc.AccessToken = "my.jwt.token";

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


        /// <summary>
        /// MethodOne with 10, works ok.
        /// </summary>
        [TestMethod]
        public void MethodOneOk()
        {
            OneClient svc = new OneClient();
            svc.ActivityId = Guid.NewGuid();
            svc.AccessToken = "my.jwt.token";

            var response = svc.MethodOne( new Zn.Sample.OneSvc.MethodOneRequest()
            {
                Value = 10
            } );

            Assert.AreEqual( 100, response.Value );
        }


        /// <summary>
        /// MethodOne with 15, throws unhandled exception.
        /// </summary>
        [TestMethod]
        public void MethodOneNotOk()
        {
            OneClient svc = new OneClient();
            svc.ActivityId = Guid.NewGuid();
            svc.AccessToken = "my.jwt.token";

            try
            {
                svc.MethodOne( new Zn.Sample.OneSvc.MethodOneRequest()
                {
                    Value = 15
                } );

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
