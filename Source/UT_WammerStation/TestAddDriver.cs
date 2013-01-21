using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Net;
using Wammer;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PerfMonitor;
using Wammer.Station;
using Waveface.Stream.Model;

namespace UT_WammerStation
{
	class NullPlanChecker : IBillingPlanChecker
	{
		public bool IsPaidUser(string user_id, string session_token)
		{
			return false;
		}
	}

	[TestClass]
	public class TestAddDriver
	{
		static MongoServer mongodb;
		HttpServer server;
		AddDriverHandler handler;

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			mongodb = MongoServer.Create("mongodb://localhost:10319/safe=true");
		}

		[TestInitialize]
		public void setUp()
		{
			server = new HttpServer(8080);
			handler = new AddDriverHandler();
			handler.m_DriverAgent.PlanChecker = new NullPlanChecker();
			server.AddHandler("/station/drivers/add/", handler);
			server.Start();
			server.TaskEnqueue += new EventHandler<TaskQueueEventArgs>(HttpRequestMonitor.Instance.OnTaskEnqueue);

			CloudServer.BaseUrl = "http://localhost/";

			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").RemoveAll();
			mongodb.GetDatabase("wammer").GetCollection("station").RemoveAll();
		}

		[TestCleanup]
		public void tearDown()
		{
			server.Close();

			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").RemoveAll();
			mongodb.GetDatabase("wammer").GetCollection("station").RemoveAll();
		}

		[TestMethod]
		public void TestAddADriver()
		{
			StationLogOnResponse res1 = new StationLogOnResponse
			{
				api_ret_message = "success",
				api_ret_code = 0,
				status = 200,
				timestamp = DateTime.UtcNow,
				session_token = "token3",
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
					new UserStation { 
						station_id = Wammer.Station.Station.Instance.StationID,
						type = "primary"
					},
				}
			};

			using (FakeCloud cloud = new FakeCloud(res1))
			{
				CloudServer.request<CloudResponse>("http://localhost:8080/station/drivers/add",
					new Dictionary<object, object>{ 
					{ "email", "user1@gmail.com"}, 
					{ "password", "12345"},
					{ "device_id", "deviceId"},
					{ "device_name", "deviceName"}, 
					{ "user_folder", "user_folder"} });


				// verify db
				Driver driver = mongodb.GetDatabase("wammer").
					GetCollection<Driver>("drivers").FindOne(
					Query.EQ("email", "user1@gmail.com"));

				Assert.AreEqual("user1@gmail.com", driver.email);
				Assert.AreEqual("user_folder", driver.folder);
				Assert.AreEqual(res1.user.user_id, driver.user_id);
				Assert.IsTrue(driver.isPrimaryStation);
				Assert.AreEqual(1, driver.groups.Count);
				Assert.AreEqual(res1.session_token, driver.session_token);
				Assert.AreEqual(res1.groups[0].group_id, driver.groups[0].group_id);
				Assert.AreEqual(res1.groups[0].name, driver.groups[0].name);
				Assert.AreEqual(res1.groups[0].description, driver.groups[0].description);

				//verify station
				Wammer.Model.StationInfo s = Wammer.Model.StationCollection.Instance.FindOne();
				Assert.IsNotNull(s);
				Assert.AreEqual(res1.session_token, s.SessionToken);
			}
		}



		[TestMethod]
		public void TestAddADriver_SecondaryStation()
		{
			StationLogOnResponse res1 = new StationLogOnResponse
			{
				api_ret_message = "success",
				api_ret_code = 0,
				status = 200,
				timestamp = DateTime.UtcNow,
				session_token = "token3",
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
			};
			using (FakeCloud cloud = new FakeCloud(res1))
			{
				CloudServer.request<CloudResponse>("http://localhost:8080/station/drivers/add",
					new Dictionary<object, object>{ 
					{ "email", "user1@gmail.com"}, 
					{ "password", "12345"},
					{ "device_id", "deviceId"},
					{ "device_name", "deviceName"},
					{ "user_folder", "folder"} } );


				// verify db
				Driver driver = mongodb.GetDatabase("wammer").
					GetCollection<Driver>("drivers").FindOne(
					Query.EQ("email", "user1@gmail.com"));

				Assert.AreEqual("user1@gmail.com", driver.email);
				Assert.AreEqual(@"folder", driver.folder);
				Assert.AreEqual(res1.user.user_id, driver.user_id);
				Assert.IsFalse(driver.isPrimaryStation);
				Assert.AreEqual(1, driver.groups.Count);
				Assert.AreEqual(res1.session_token, driver.session_token);
				Assert.AreEqual(res1.groups[0].group_id, driver.groups[0].group_id);
				Assert.AreEqual(res1.groups[0].name, driver.groups[0].name);
				Assert.AreEqual(res1.groups[0].description, driver.groups[0].description);

				//verify station
				Wammer.Model.StationInfo s = Wammer.Model.StationCollection.Instance.FindOne();
				Assert.IsNotNull(s);
				Assert.AreEqual("token3", s.SessionToken);
			}
		}

		[TestMethod]
		public void TestAddADriver_incorrectUserNamePwd()
		{
			UserLogInResponse res1 = new UserLogInResponse
			{
				api_ret_message = "station res msg",
				api_ret_code = 4097, // cloud retuns 4097 for invalid user name or password
				session_token = "token1",
				status = (int)HttpStatusCode.BadRequest,
				timestamp = DateTime.UtcNow,
				groups = new List<UserGroup>(),
				user = new UserInfo { user_id = "uid1" }
			};

			using (FakeCloud cloud = new FakeCloud(res1, res1.status))
			{
				try
				{
					CloudServer.request<CloudResponse>(
						"http://localhost:8080/station/drivers/add",
						new Dictionary<object, object>{ 
							{ "email", "user1@gmail.com"}, 
							{ "password", "12345"},
 							{ "device_id", "deviceId"},
							{ "device_name", "deviceName"}
						});
				}
				catch (WammerCloudException e)
				{
					Assert.AreEqual((int)AuthApiError.InvalidEmailPassword, e.WammerError);
					return;
				}

				Assert.Fail("Expected exception is not thrown");
			}
		}
	}
}
