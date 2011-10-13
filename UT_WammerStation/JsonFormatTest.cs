using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Cloud;

namespace UT_WammerStation
{
    [TestClass]
    public class JsonFormatTest
    {
        [TestMethod]
        public void TestUserLoginResponse()
        {
            UserLogInResponse res = fastJSON.JSON.Instance.ToObject<UserLogInResponse>(
                "{\"response\": { \"status\": 200, \"timestamp\": \"2011-01-01T10:20:30Z\"}," +
                  "\"session_token\": \"token1\", " +
                  "\"uid\": \"id1\" }");

            Assert.AreEqual(200, res.response.status);
            Assert.AreEqual(new DateTime(2011, 1, 1, 10, 20, 30, DateTimeKind.Utc), res.response.timestamp.ToUniversalTime());
            Assert.AreEqual("token1", res.session_token);
            Assert.AreEqual("id1", res.uid);
        }

        [TestMethod]
        public void TestStationSignUpResponse()
        {
            StationSignUpResponse res = fastJSON.JSON.Instance.ToObject<StationSignUpResponse>(
                "{\"response\": { \"status\": 200, \"timestamp\": \"2011-01-01T10:20:30Z\"}," +
                  "\"session_token\": \"token1\", " +
                  "\"station\": {\"station_id\": \"sid1\", \"creator_id\": \"cid1\", \"timestamp\": \"2011-10-20T10:20:30Z\", \"name\": \"name1\"} }");

            Assert.AreEqual(200, res.response.status);
            Assert.AreEqual(new DateTime(2011, 1, 1, 10, 20, 30, DateTimeKind.Utc), res.response.timestamp.ToUniversalTime());
            Assert.AreEqual("token1", res.session_token);
            Assert.AreEqual("sid1", res.station.station_id);
            Assert.AreEqual("cid1", res.station.creator_id);
        }

        [TestMethod]
        public void TestStationLogOnResponse()
        {
            StationLogOnResponse res = fastJSON.JSON.Instance.ToObject<StationLogOnResponse>(
                "{\"response\": { \"status\": 200, \"timestamp\": \"2011-01-01T10:20:30Z\"}," +
                  "\"session_token\": \"token1\" }");

            Assert.AreEqual(200, res.response.status);
            Assert.AreEqual(new DateTime(2011, 1, 1, 10, 20, 30, DateTimeKind.Utc), res.response.timestamp.ToUniversalTime());
            Assert.AreEqual("token1", res.session_token);
        }
    }
}
