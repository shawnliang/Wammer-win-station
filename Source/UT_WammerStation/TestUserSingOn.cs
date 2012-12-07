using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Wammer.Cloud;

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
			{
				Wammer.Cloud.User user = Wammer.Cloud.User.LogIn("user1", "passwd1", "deviceId", "deviceName");
				Assert.AreEqual("user1", user.Name);
				Assert.AreEqual("passwd1", user.Password);
				Assert.AreEqual("uid", user.Id);
				Assert.AreEqual("/v9999/auth/login",
					fakeCloud.RequestedPath);
				Assert.AreEqual("email=user1&password=passwd1&apikey=apiKey1&device_id=deviceId&device_name=deviceName",
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
			{
				try
				{
					Wammer.Cloud.User.LogIn("user1", "passwd1", "deviceId", "deviceName");
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
			{
				try
				{
					Wammer.Cloud.User.LogIn("user1", "passwd1", "deviceId", "deviceName");
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
			using (FakeCloud fakeCloud = new FakeCloud(new StationLogOnResponse
			{
				api_ret_message = "success",
				api_ret_code = 0,
				status = 200,
				timestamp = DateTime.UtcNow.ToUniversalTime(),
				session_token = "newToken1",
				groups = new List<UserGroup>{
					new UserGroup {
						creator_id = "creator1",
						description = "gdesc1",
						group_id = "group_id1",
						name = "group1"				
					}
				},
				user = new UserInfo { user_id = "uid1" },
				stations = new List<UserStation>()
				{
					new UserStation() { station_id = "aabbcc" },
				}
			}))
			{
				Wammer.Cloud.StationApi api = new Wammer.Cloud.StationApi("sid1", "token1");
				api.LogOn();
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
			using (FakeCloud fakeCloud = new FakeCloud(new StationLogOnResponse
			{
				api_ret_message = "success",
				api_ret_code = 0,
				status = 200,
				timestamp = DateTime.UtcNow.ToUniversalTime(),
				session_token = "newToken1",
				groups = new List<UserGroup>{
					new UserGroup {
						creator_id = "creator1",
						description = "gdesc1",
						group_id = "group_id1",
						name = "group1"				
					}
				},
				user = new UserInfo { user_id = "uid1" },
				stations = new List<UserStation>()
				{
					new UserStation() { station_id = "aabbcc" },
				}
			}))
			{
				Wammer.Cloud.StationApi api = new Wammer.Cloud.StationApi("sid1", "token1");

				Dictionary<object, object> param = new Dictionary<object, object>();
				param.Add("host_name", "hostname1");
				param.Add("ip_address", "ip1");
				param.Add("port", "9999");
				api.LogOn(param);

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
			using (FakeCloud fakeCloud = new FakeCloud(new StationLogOnResponse
			{
				api_ret_message = "success",
				api_ret_code = 0,
				status = 200,
				timestamp = DateTime.UtcNow.ToUniversalTime(),
				session_token = "newToken1",
				groups = new List<UserGroup>{
					new UserGroup {
						creator_id = "creator1",
						description = "gdesc1",
						group_id = "group_id1",
						name = "group1"				
					}
				},
				user = new UserInfo { user_id = "uid1" },
				stations = new List<UserStation>()
				{
					new UserStation() { station_id = "aabbcc" },
				}
			}))
			{
				Wammer.Cloud.StationApi api = new Wammer.Cloud.StationApi("sid1", "token1");

				Dictionary<object, object> param = new Dictionary<object, object>();
				param.Add("key", @"<>+@/\|");
				api.LogOn(param);

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
