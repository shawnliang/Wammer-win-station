﻿using System;
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
		[TestInitialize]
		public void classInit()
		{
			Wammer.Cloud.CloudServer.APIKey = "apiKey1";
			Wammer.Cloud.CloudServer.BaseUrl = "http://127.0.0.1/v9999/";
		}

		[TestMethod]
		public void TestUserSignOn()
		{
			Wammer.Cloud.UserLogInResponse res =
			new Wammer.Cloud.UserLogInResponse(200, DateTime.Now.ToUniversalTime(), "token1");
			res.user = new Wammer.Cloud.UserInfo { user_id = "uid" };

			using (FakeCloud fakeCloud = new FakeCloud(res))
			using (WebClient agent = new WebClient())
			{
				Wammer.Cloud.User user = Wammer.Cloud.User.LogIn(agent, "user1", "passwd1");
				Assert.AreEqual("user1", user.Name);
				Assert.AreEqual("passwd1", user.Password);
				Assert.AreEqual("uid", user.Id);
				Assert.AreEqual("/v9999/auth/login", 
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
			res.api_ret_code = 9999;

			using (FakeCloud fakeCloud = new FakeCloud(res))
			using (WebClient agent = new WebClient())
			{
				try
				{
					Wammer.Cloud.User.LogIn(agent, "user1", "passwd1");
				}
				catch (Wammer.Cloud.WammerCloudException e)
				{
					Assert.AreEqual(res.api_ret_code, e.WammerError);
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
		public void TestStationLogOn()
		{
			using (FakeCloud fakeCloud = new FakeCloud(new Wammer.Cloud.StationLogOnResponse(200, DateTime.Now.ToUniversalTime(), "newToken1")))
			using (WebClient agent = new WebClient())
			{
				Wammer.Cloud.StationApi api = new Wammer.Cloud.StationApi("sid1", "token1");
				api.LogOn(agent);
				Assert.AreEqual("/v9999/stations/logOn",
					fakeCloud.RequestedPath);
				Assert.AreEqual("session_token=token1&station_id=sid1&apikey=apiKey1",
					fakeCloud.PostData);
				Assert.AreEqual("application/x-www-form-urlencoded",
					fakeCloud.RequestedContentType);
				Assert.AreEqual("newToken1", api.Token);
			}

		}

		[TestMethod]
		public void TestStationLogOnParams()
		{
			using (FakeCloud fakeCloud = new FakeCloud(new Wammer.Cloud.StationLogOnResponse(200, DateTime.Now.ToUniversalTime(), "newToken1")))
			using (WebClient agent = new WebClient())
			{
				Wammer.Cloud.StationApi api = new Wammer.Cloud.StationApi("sid1", "token1");

				Dictionary<object, object> param = new Dictionary<object, object>();
				param.Add("host_name", "hostname1");
				param.Add("ip_address", "ip1");
				param.Add("port", "9999");
				api.LogOn(agent, param);

				Assert.AreEqual("/v9999/stations/logOn",
					fakeCloud.RequestedPath);
				Assert.AreEqual(
					"host_name=hostname1&ip_address=ip1&port=9999&" +
					"session_token=token1&station_id=sid1&apikey=apiKey1",
					fakeCloud.PostData);
				Assert.AreEqual("application/x-www-form-urlencoded",
					fakeCloud.RequestedContentType);
				Assert.AreEqual("newToken1", api.Token);

				Assert.AreEqual("newToken1", api.Token);
			}
		}

		[TestMethod]
		public void TestStationLogOnParams_Encoding()
		{
			using (FakeCloud fakeCloud = new FakeCloud(new Wammer.Cloud.StationLogOnResponse(200, DateTime.Now.ToUniversalTime(), "newToken1")))
			using (WebClient agent = new WebClient())
			{
				Wammer.Cloud.StationApi api = new Wammer.Cloud.StationApi("sid1", "token1");

				Dictionary<object, object> param = new Dictionary<object, object>();
				param.Add("key", @"<>+@/\|");
				api.LogOn(agent, param);

				Assert.AreEqual("/v9999/stations/logOn",
					fakeCloud.RequestedPath);
				Assert.AreEqual(
					"key=" + HttpUtility.UrlEncode(@"<>+@/\|") + 
					"&session_token=token1&station_id=sid1&apikey=apiKey1",
					fakeCloud.PostData);
				Assert.AreEqual("application/x-www-form-urlencoded",
					fakeCloud.RequestedContentType);
				Assert.AreEqual("newToken1", api.Token);

				Assert.AreEqual("newToken1", api.Token);
			}
		}
	}
}
