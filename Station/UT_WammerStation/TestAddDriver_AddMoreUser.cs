using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Station;
using Wammer.Model;
using Wammer.PerfMonitor;
using Wammer;

namespace UT_WammerStation
{
	[TestClass]
	public class TestAddMoreUser
	{
		#region Const
		const int SERVER_PORT = 8080;
		const string REST_COMMAND_ADD = "http://localhost:8080/v2/station/drivers/add"; 
		#endregion

		#region Var
		private HttpServer _server;
		#endregion

		#region Property
		public HttpServer Server
		{
			get 
			{
				if (_server == null)
					_server = new HttpServer(8080);
				return _server;
			}
		} 
		#endregion

		static MongoServer mongodb;
		AddDriverHandler handler;

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			mongodb = MongoServer.Create("mongodb://localhost:10319/safe=true");
		}

		[TestInitialize]
		public void setUp()
		{
			handler = new AddDriverHandler("stationId", "resource");
			Server.AddHandler("/v2/station/drivers/add/", handler);
			Server.Start();
			Server.TaskEnqueue += new EventHandler<TaskQueueEventArgs>(HttpRequestMonitor.Instance.OnTaskEnqueue);

			CloudServer.BaseUrl = "http://localhost/v2/";

			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").RemoveAll();
			mongodb.GetDatabase("wammer").GetCollection("station").RemoveAll();

			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").Insert(
				new Driver
				{
					user_id = "exist_uid",
					email = "exist@gmail.com",
					folder = "resource\\user_exist_uid",
					groups = new List<UserGroup> { new UserGroup { group_id = "123" } }
				});
		}

		[TestCleanup]
		public void tearDown()
		{
			Server.Close();

			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").RemoveAll();
			mongodb.GetDatabase("wammer").GetCollection("station").RemoveAll();
		}

		[TestMethod]
		public void TestAddTheSameDriver()
		{
			UserLogInResponse res = new UserLogInResponse
			{
				api_ret_message = "success",
				api_ret_code = 0,
				session_token = "token2",
				status = 200,
				timestamp = DateTime.UtcNow,
				groups = new List<UserGroup>{
					new UserGroup {
						creator_id = "creator1",
						description = "gdesc1",
						group_id = "group_id1",
						name = "group1"				
					}
				},
				user = new UserInfo { user_id = "uid1" }
			};

            Driver driver = Wammer.Model.DriverCollection.Instance.FindOne(Query.EQ("email", "exist@gmail.com"));
            var referenceCount = driver.ref_count;
            var expectedValue =  referenceCount + 1;

			using (FakeCloud cloud = new FakeCloud(res))
			{
				CloudServer.request<CloudResponse>(new WebClient(), REST_COMMAND_ADD,
					new Dictionary<object, object>{ 
					{ "email", "exist@gmail.com"}, 
					{ "password", "12345"},
					{ "device_id", "deviceId"},
					{ "device_name", "deviceName"}
					});
			}

			driver = Wammer.Model.DriverCollection.Instance.FindOne(Query.EQ("email", "exist@gmail.com"));           

			Assert.IsNotNull(driver);

            var actualValue = driver.ref_count;
            Assert.AreEqual(expectedValue, actualValue);
		}

		[TestMethod]
		public void TestAddAnotherDriver()
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
				CloudServer.request<CloudResponse>(new WebClient(), REST_COMMAND_ADD,
					new Dictionary<object, object>{ 
					{ "email", "user1@gmail.com"}, 
					{ "password", "12345"},
					{ "device_id", "deviceId"},
					{ "device_name", "deviceName"}});


				// verify db
				Driver driver = mongodb.GetDatabase("wammer").
					GetCollection<Driver>("drivers").FindOne(
					Query.EQ("email", "user1@gmail.com"));
				
				Assert.IsNotNull(driver);
				Assert.AreEqual("user1@gmail.com", driver.email);
				Assert.AreEqual(@"resource\user_uid1", driver.folder);
				Assert.AreEqual(res1.user.user_id, driver.user_id);
				Assert.AreEqual(1, driver.groups.Count);
				Assert.AreEqual(res1.session_token, driver.session_token);
				Assert.AreEqual(res1.groups[0].group_id, driver.groups[0].group_id);
				Assert.AreEqual(res1.groups[0].name, driver.groups[0].name);
				Assert.AreEqual(res1.groups[0].description, driver.groups[0].description);

				Driver existUser = mongodb.GetDatabase("wammer").
					GetCollection<Driver>("drivers").FindOne(
					Query.EQ("email", "exist@gmail.com"));
				Assert.IsNotNull(existUser);

				//verify station
				Wammer.Model.StationInfo s = Wammer.Model.StationCollection.Instance.FindOne();
				Assert.IsNotNull(s);
				Assert.AreEqual("token3", s.SessionToken);
			}
		}

	}
}
