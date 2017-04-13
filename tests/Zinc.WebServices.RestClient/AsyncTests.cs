﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Platinum;
using System;
using Zn.Sample;

namespace Zinc.WebServices.RestClient.Test
{
    [TestClass]
    public class AsyncTests
    {
        /// <summary>
        /// MethodOne with 10, works ok.
        /// </summary>
        [TestMethod]
        public void MethodOne_Ok()
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
        /// MethodFake, which doesn't actually exist.
        /// </summary>
        [TestMethod]
        public void MethodFake_Ok()
        {
            OneClient svc = new OneClient();
            svc.ActivityId = Guid.NewGuid();
            svc.AccessToken = "my.jwt.token";

            try
            {
                svc.MethodFakeAsync( new Zn.Sample.OneSvc.MethodOneRequest()
                {
                    Value = 10
                } ).GetAwaiter().GetResult();

                Assert.Fail( "Expected exception." );
            }
            catch ( ServiceException )
            {
                Assert.Fail( "TODO" );
            }
            catch ( Exception ex )
            {
                Assert.Fail( "Expected named exception, instead got: " + ex.GetType().FullName );
            }
        }


        /// <summary>
        /// MethodOne with 15, throws unhandled exception.
        /// </summary>
        [TestMethod]
        public void MethodOn_Unhandled()
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
                // Zinc.WebServices.ZincException
                Assert.AreEqual( "ZnSample.Zinc", ex.Actor );
                Assert.AreEqual( 4006, ex.Code );

                // System.Exception
                Assert.IsTrue( ex.InnerException != null, "Expected inner exception." );
                Assert.IsTrue( ex.InnerException is ActorException, "Inner exception must be ActorException." );
                ActorException iex = (ActorException) ex.InnerException;
                Assert.AreEqual( "ZnSample", iex.Actor );
                Assert.AreEqual( 991, iex.Code );
            }
            catch ( Exception ex )
            {
                Assert.Fail( "Expected named exception, instead got: " + ex.GetType().FullName );
            }
        }


        /// <summary>
        /// MethodOne with 110, throws remote exception with stack.
        /// </summary>
        [TestMethod]
        public void MethodOn_RemoteException()
        {
            OneClient svc = new OneClient();
            svc.ActivityId = Guid.NewGuid();
            svc.AccessToken = "my.jwt.token";

            try
            {
                svc.MethodOneAsync( new Zn.Sample.OneSvc.MethodOneRequest()
                {
                    Value = 110
                } ).GetAwaiter().GetResult();

                Assert.Fail( "Expected exception." );
            }
            catch ( ServiceFaultException ex )
            {
                // Zn.Sample.SampleException
                Assert.AreEqual( "ZnSample", ex.Actor );
                Assert.AreEqual( 10001, ex.Code );

                // Zn.Sample.SampleException
                Assert.IsTrue( ex.InnerException != null, "Expected inner exception." );
                Assert.IsTrue( ex.InnerException is ActorException, "Inner exception must be ActorException." );
                ActorException iex = (ActorException) ex.InnerException;
                Assert.AreEqual( "ZnSample", iex.Actor );
                Assert.AreEqual( 10000, iex.Code );
            }
            catch ( Exception ex )
            {
                Assert.Fail( "Expected named exception, instead got: " + ex.GetType().FullName );
            }
        }
    }
}
