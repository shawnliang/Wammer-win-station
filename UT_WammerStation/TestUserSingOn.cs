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
        }

        [TestMethod]
        public void TestUserSignOn()
        {
            using (FakeCloud fakeCloud = new FakeCloud(new Wammer.Cloud.UserSigninResponse(200, "id1", "token1")))
            {
                Wammer.Cloud.User user = Wammer.Cloud.User.SignIn("user1", "passwd1", "apiKey1");
                Assert.AreEqual("user1", user.Name);
                Assert.AreEqual("passwd1", user.Password);
                Assert.AreEqual("/api/v2/auth/login/email/user1/password/passwd1/api_key/apiKey1",
                   fakeCloud.RequestedPath);
                Assert.AreEqual("token1", user.Token);
                Assert.AreEqual("id1", user.Id);
            }
        }

        [TestMethod]
        public void TestStationSignUp()
        {
            using (FakeCloud fakeCloud = new FakeCloud(new Wammer.Cloud.StationSignUpResponse(200, "stationToken1")))
            {
                Wammer.Cloud.Station station = Wammer.Cloud.Station.SignUp("stationId1", "userToken1", "apiKey1");
                Assert.AreEqual("/api/v2/station/sign_up/user_token/userToken1/station_id/stationId1/api_key/apiKey1",
                    fakeCloud.RequestedPath);

                Assert.AreEqual("stationId1", station.Id);
                Assert.AreEqual("stationToken1", station.Token);
            }
        }
        

    }
}
