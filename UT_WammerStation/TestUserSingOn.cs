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
        public void TestSignOn()
        {
            FakeCloud fakeCloud = new FakeCloud(new Wammer.Cloud.UserSigninResponse(200, "id1", "token1"));

            Wammer.Cloud.User user = Wammer.Cloud.User.SignIn("user1", "passwd1", "apiKey1");
            Assert.AreEqual("user1", user.Name);
            Assert.AreEqual("passwd1", user.Password);
            Assert.AreEqual("/api/v2/auth/login/email/user1/password/passwd1/apiKey/apiKey1",
               fakeCloud.RequestedPath);
            Assert.AreEqual("token1", user.Token);
            Assert.AreEqual("id1", user.Id);
        }
    }
}
