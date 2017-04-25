using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zn.Sample;

namespace Zinc.WebServices.RestClient.Test
{
    /// <summary />
    [TestClass]
    public class DateTimeTests
    {
        /// <summary />
        internal class Call
        {
            internal Zn.Sample.TwoSvc.MethodTwoRequest Request { get; set; }
            internal Zn.Sample.TwoSvc.MethodTwoResponse Response { get; set; }
        }


        /// <summary />
        private Call OkCall()
        {
            TwoClient svc = new TwoClient();
            svc.ActivityId = Guid.NewGuid();
            svc.AccessToken = "my.jwt.token";

            var request = new Zn.Sample.TwoSvc.MethodTwoRequest()
            {
                InDate = DateTime.UtcNow.Date,
                InDateTime = DateTime.UtcNow,
                InTime = DateTime.UtcNow,
                InValue = -1,
                InEnum = TestEnum.First,
            };

            var response = svc.MethodTwoAsync( request ).Result;

            return new Call()
            {
                Request = request,
                Response = response,
            };
        }


        /// <summary />
        [TestMethod]
        public void DateOk()
        {
            var call = OkCall();

            Assert.AreEqual( DateTimeKind.Utc, call.Response.OutDateTime.Kind );
            Assert.AreEqual( call.Request.InDate, call.Response.OutDate );
        }


        /// <summary />
        [TestMethod]
        public void TimeOk()
        {
            var call = OkCall();

            Assert.AreEqual( DateTimeKind.Utc, call.Response.OutDateTime.Kind );
            Assert.AreEqual( call.Request.InTime.Hour, call.Response.OutTime.Hour );
            Assert.AreEqual( call.Request.InTime.Minute, call.Response.OutTime.Minute );
            Assert.AreEqual( call.Request.InTime.Second, call.Response.OutTime.Second );
        }


        /// <summary />
        [TestMethod]
        public void DateTimeOk()
        {
            var call = OkCall();

            Assert.AreEqual( DateTimeKind.Utc, call.Response.OutDateTime.Kind );
            Assert.AreEqual( call.Request.InDateTime, call.Response.OutDateTime );
        }
    }
}
