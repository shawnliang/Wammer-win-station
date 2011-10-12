using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;

namespace UT_WammerStation
{

    [TestClass]
    public class TestUserSingOn
    {
        [ClassInitialize]
        public static void classInit(TestContext testContext)
        {
            Wammer.Cloud.CloudServer.Address = "127.0.0.1";
            Wammer.Cloud.CloudServer.Port = 80;
            Wammer.Cloud.CloudServer.APIKey = "apiKey1";
        }

        [TestMethod]
        public void TestUserSignOn()
        {
            Wammer.Cloud.StatusResponse res = new Wammer.Cloud.StatusResponse(200, DateTime.Now.ToUniversalTime());
            using (FakeCloud fakeCloud = new FakeCloud(new Wammer.Cloud.UserLogInResponse(res, "id1", "token1")))
            using (WebClient agent = new WebClient())
            {
                Wammer.Cloud.User user = Wammer.Cloud.User.LogIn(agent, "user1", "passwd1");
                Assert.AreEqual("user1", user.Name);
                Assert.AreEqual("passwd1", user.Password);
                Assert.AreEqual("/api/v2/auth/login/user_account/user1/password/passwd1/api_key/apiKey1",
                   fakeCloud.RequestedPath);
                Assert.AreEqual("token1", user.Token);
                Assert.AreEqual("id1", user.Id);
            }
        }

        [TestMethod]
        public void TestUserSignOnError()
        {
            Wammer.Cloud.StatusResponse res = new Wammer.Cloud.StatusResponse(403, DateTime.Now.ToUniversalTime());
            using (FakeCloud fakeCloud = new FakeCloud(new Wammer.Cloud.UserLogInResponse(res, "", "")))
            using (WebClient agent = new WebClient())
            {
                try
                {
                    Wammer.Cloud.User.LogIn(agent, "user1", "passwd1");
                }
                catch (Wammer.Cloud.WammerCloudException e)
                {
                    Assert.AreEqual(403, e.WammerError);
                    return;
                }
                Assert.Fail("Expected exception is not thrown.");
            }
        }

        [TestMethod]
        public void TestUserSignOnError2()
        {

            using (FakeErrorCloud fakeCloud = new FakeErrorCloud(403))
            using (WebClient agent = new WebClient())
            {
                try
                {
                    Wammer.Cloud.User.LogIn(agent, "user1", "passwd1");
                }
                catch (Wammer.Cloud.WammerCloudException e)
                {
                    Assert.AreEqual(WebExceptionStatus.ProtocolError, e.HttpError);
                    return;
                }
                Assert.Fail("Expected exception is not thrown.");
            }
        }

        [TestMethod]
        public void TestStationSignUp()
        {
            Wammer.Cloud.StatusResponse res = new Wammer.Cloud.StatusResponse(200, DateTime.Now.ToUniversalTime());
            Wammer.Cloud.StationSignUpResponse stationRes = new Wammer.Cloud.StationSignUpResponse(res, "stationToken1");
            stationRes.station = new Wammer.Cloud.StationResponse();
            stationRes.station.station_id = "stationId1";
            stationRes.station.creator_id = "cid1";

            using (FakeCloud fakeCloud = new FakeCloud(stationRes))
            using (WebClient agent = new WebClient())
            {
                Wammer.Cloud.Station station = Wammer.Cloud.Station.SignUp(agent, "stationId1", "userToken1");
                Assert.AreEqual("/api/v2/station/signup/user_token/userToken1/station_id/stationId1/api_key/apiKey1",
                    fakeCloud.RequestedPath);

                Assert.AreEqual("stationId1", station.Id);
                Assert.AreEqual("stationToken1", station.Token);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Wammer.Cloud.WammerCloudException))]
        public void TestStationSignUpError()
        {
            Wammer.Cloud.StatusResponse res = new Wammer.Cloud.StatusResponse(501, DateTime.Now.ToUniversalTime());
            Wammer.Cloud.StationSignUpResponse stationRes = new Wammer.Cloud.StationSignUpResponse(res, "");
            stationRes.station = new Wammer.Cloud.StationResponse();
            stationRes.station.station_id = "";
            stationRes.station.creator_id = "";

            using (FakeCloud fakeCloud = new FakeCloud(stationRes))
            using (WebClient agent = new WebClient())
            {
                Wammer.Cloud.Station.SignUp(agent, "stationId1", "userToken1");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Wammer.Cloud.WammerCloudException))]
        public void TestStationSignUpError2()
        {
            using (FakeErrorCloud fakeCloud = new FakeErrorCloud(403))
            using (WebClient agent = new WebClient())
            {
                Wammer.Cloud.Station.SignUp(agent, "stationId1", "userToken1");
            }
        }

        [TestMethod]
        public void TestStationLogOn()
        {
            Wammer.Cloud.StatusResponse res = new Wammer.Cloud.StatusResponse(200, DateTime.Now.ToUniversalTime());
            using (FakeCloud fakeCloud = new FakeCloud(new Wammer.Cloud.StationLogOnResponse(res, "newToken1")))
            using (WebClient agent = new WebClient())
            {
                Wammer.Cloud.Station station = new Wammer.Cloud.Station("sid1", "token1");
                station.LogOn(agent);
                Assert.AreEqual("/api/v2/station/logOn/station_token/token1/station_id/sid1/api_key/apiKey1",
                    fakeCloud.RequestedPath);
                Assert.AreEqual("newToken1", station.Token);
            }

        }

        [TestMethod]
        public void TestStationLogOnParams()
        {
            Wammer.Cloud.StatusResponse res = new Wammer.Cloud.StatusResponse(200, DateTime.Now.ToUniversalTime());
            using (FakeCloud fakeCloud = new FakeCloud(new Wammer.Cloud.StationLogOnResponse(res, "newToken1")))
            using (WebClient agent = new WebClient())
            {
                Wammer.Cloud.Station station = new Wammer.Cloud.Station("sid1", "token1");

                Dictionary<object, object> param = new Dictionary<object, object>();
                param.Add("host_name", "hostname1");
                param.Add("ip_address", "ip1");
                param.Add("port", "9999");
                station.LogOn(agent, param);
                Assert.AreEqual("/api/v2/station/logOn/station_token/token1/station_id/sid1/api_key/apiKey1" +
                    "/host_name/hostname1/ip_address/ip1/port/9999",
                    fakeCloud.RequestedPath);
                Assert.AreEqual("newToken1", station.Token);
            }
        }

        [TestMethod]
        public void TestRegistry()
        {

        }
    }
}
