using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zn.Sample;

namespace Zinc.WebServices.RestClient.Test
{
    [TestClass]
    public class BadConfigTests
    {
        /// <summary>
        /// MethodOne: Bad config
        /// (Hmm, how to always test this?)
        /// </summary>
        [TestMethod]
        public void Sync()
        {
            OneClient svc = new OneClient();
            svc.ActivityId = Guid.NewGuid();
            svc.AccessToken = "my.jwt.token";

            try
            {
                var response = svc.MethodOne( new Zn.Sample.OneSvc.MethodOneRequest()
                {
                    Value = 10
                } );

                Assert.Fail( "Expected exception." );
            }
            catch ( ServiceException ex )
            {
                Assert.AreEqual( "Invoke_NotJson", ex.Message );
                Assert.AreEqual( "ZnClient.Sample", ex.Actor );
            }
            catch ( Exception ex )
            {
                Assert.Fail( "Expected named exception, instead got: " + ex.GetType().FullName );
            }
        }


        /// <summary>
        /// MethodOne: Bad config
        /// (Hmm, how to always test this?)
        /// </summary>
        [TestMethod]
        public void Async()
        {
            OneClient svc = new OneClient();
            svc.ActivityId = Guid.NewGuid();
            svc.AccessToken = "my.jwt.token";

            try
            {
                var response = svc.MethodOneAsync( new Zn.Sample.OneSvc.MethodOneRequest()
                {
                    Value = 10
                } ).GetAwaiter().GetResult();

                Assert.Fail( "Expected exception." );
            }
            catch ( ServiceException ex )
            {
                Assert.AreEqual( "Invoke_NotJson", ex.Message );
                Assert.AreEqual( "ZnClient.Sample", ex.Actor );
            }
            catch ( Exception ex )
            {
                Assert.Fail( "Expected named exception, instead got: " + ex.GetType().FullName );
            }
        }
    }
}
