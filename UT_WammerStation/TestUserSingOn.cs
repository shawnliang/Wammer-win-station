using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using System.Web;

namespace UT_WammerStation
{
	[TestClass]
	public class TestUserSingOn
	{
		[ClassInitialize]
		public static void classInit(TestContext testContext)
		{
			Wammer.Cloud.CloudServer.HostName = "127.0.0.1";
			Wammer.Cloud.CloudServer.Port = 80;
			Wammer.Cloud.CloudServer.APIKey = "apiKey1";
		}

		[TestMethod]
		public void TestUserSignOn()
		{
			using (FakeCloud fakeCloud = new FakeCloud(
				new Wammer.Cloud.UserLogInResponse(200, DateTime.Now.ToUniversalTime(), "token1")))
			using (WebClient agent = new WebClient())
			{
				Wammer.Cloud.User user = Wammer.Cloud.User.LogIn(agent, "user1", "passwd1");
				Assert.AreEqual("user1", user.Name);
				Assert.AreEqual("passwd1", user.Password);

				Assert.AreEqual("/" + Wammer.Cloud.CloudServer.DEF_BASE_PATH + "/auth/login", 
					fakeCloud.RequestedPath);
				Assert.AreEqual("email=user1&password=passwd1&apikey=apiKey1",
					fakeCloud.PostData);
				Assert.AreEqual("application/x-www-form-urlencoded",
					fakeCloud.RequestedContentType);

				Assert.AreEqual("token1", user.Token);
			}
		}

		[TestMethod]
		public void TestUserSignOnError()
		{
			Wammer.Cloud.UserLogInResponse res =
				new Wammer.Cloud.UserLogInResponse(403,
					DateTime.Now.ToUniversalTime(), "token1");

			using (FakeCloud fakeCloud = new FakeCloud(res))
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
			Wammer.Cloud.StationSignUpResponse stationRes = 
				new Wammer.Cloud.StationSignUpResponse(200, DateTime.Now.ToUniversalTime(), "stationToken1");

			using (FakeCloud fakeCloud = new FakeCloud(stationRes))
			using (WebClient agent = new WebClient())
			{
				Wammer.Cloud.Station station = 
					Wammer.Cloud.Station.SignUp(agent, "stationId1", "userToken1");

				Assert.AreEqual("/" + Wammer.Cloud.CloudServer.DEF_BASE_PATH + "/stations/signup",
					fakeCloud.RequestedPath);
				Assert.AreEqual("session_token=userToken1&station_id=stationId1&apikey=apiKey1",
					fakeCloud.PostData);
				Assert.AreEqual("application/x-www-form-urlencoded",
					fakeCloud.RequestedContentType);

				Assert.AreEqual("stationId1", station.Id);
				Assert.AreEqual("stationToken1", station.Token);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(Wammer.Cloud.WammerCloudException))]
		public void TestStationSignUpError()
		{
			Wammer.Cloud.StationSignUpResponse stationRes = 
				new Wammer.Cloud.StationSignUpResponse(501, DateTime.Now.ToUniversalTime(), "");

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
			using (FakeCloud fakeCloud = new FakeCloud(new Wammer.Cloud.StationLogOnResponse(200, DateTime.Now.ToUniversalTime(), "newToken1")))
			using (WebClient agent = new WebClient())
			{
				Wammer.Cloud.Station station = new Wammer.Cloud.Station("sid1", "token1");
				station.LogOn(agent);
				Assert.AreEqual("/" + Wammer.Cloud.CloudServer.DEF_BASE_PATH + "/stations/logOn",
					fakeCloud.RequestedPath);
				Assert.AreEqual("session_token=token1&station_id=sid1&apikey=apiKey1",
					fakeCloud.PostData);
				Assert.AreEqual("application/x-www-form-urlencoded",
					fakeCloud.RequestedContentType);
				Assert.AreEqual("newToken1", station.Token);
			}

		}

		[TestMethod]
		public void TestStationLogOnParams()
		{
			using (FakeCloud fakeCloud = new FakeCloud(new Wammer.Cloud.StationLogOnResponse(200, DateTime.Now.ToUniversalTime(), "newToken1")))
			using (WebClient agent = new WebClient())
			{
				Wammer.Cloud.Station station = new Wammer.Cloud.Station("sid1", "token1");

				Dictionary<object, object> param = new Dictionary<object, object>();
				param.Add("host_name", "hostname1");
				param.Add("ip_address", "ip1");
				param.Add("port", "9999");
				station.LogOn(agent, param);

				Assert.AreEqual("/" + Wammer.Cloud.CloudServer.DEF_BASE_PATH + "/stations/logOn",
					fakeCloud.RequestedPath);
				Assert.AreEqual(
					"host_name=hostname1&ip_address=ip1&port=9999&" +
					"session_token=token1&station_id=sid1&apikey=apiKey1",
					fakeCloud.PostData);
				Assert.AreEqual("application/x-www-form-urlencoded",
					fakeCloud.RequestedContentType);
				Assert.AreEqual("newToken1", station.Token);

				Assert.AreEqual("newToken1", station.Token);
			}
		}

		[TestMethod]
		public void TestStationLogOnParams_Encoding()
		{
			using (FakeCloud fakeCloud = new FakeCloud(new Wammer.Cloud.StationLogOnResponse(200, DateTime.Now.ToUniversalTime(), "newToken1")))
			using (WebClient agent = new WebClient())
			{
				Wammer.Cloud.Station station = new Wammer.Cloud.Station("sid1", "token1");

				Dictionary<object, object> param = new Dictionary<object, object>();
				param.Add("key", @"<>+@/\|");
				station.LogOn(agent, param);

				Assert.AreEqual("/" + Wammer.Cloud.CloudServer.DEF_BASE_PATH + "/stations/logOn",
					fakeCloud.RequestedPath);
				Assert.AreEqual(
					"key=" + HttpUtility.UrlEncode(@"<>+@/\|") + 
					"&session_token=token1&station_id=sid1&apikey=apiKey1",
					fakeCloud.PostData);
				Assert.AreEqual("application/x-www-form-urlencoded",
					fakeCloud.RequestedContentType);
				Assert.AreEqual("newToken1", station.Token);

				Assert.AreEqual("newToken1", station.Token);
			}
		}
	}
}
