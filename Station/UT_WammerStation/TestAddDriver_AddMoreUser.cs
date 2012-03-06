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

			if (!Directory.Exists(@"C:\TempUT"))
				Directory.CreateDirectory(@"c:\TempUT");
			if (!Directory.Exists(@"C:\TempUT\user1"))
				Directory.CreateDirectory(@"c:\TempUT\user1");

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

			if (Directory.Exists(@"C:\TempUT"))
				Directory.Delete(@"C:\TempUT",true);

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

			using (FakeCloud cloud = new FakeCloud(res))
			{
				CloudServer.request<CloudResponse>(new WebClient(), REST_COMMAND_ADD,
					new Dictionary<object, object>{ 
					{ "email", "exist@gmail.com"}, 
					{ "password", "12345"} });
			}

			Driver driver = Wammer.Model.DriverCollection.Instance.FindOne(Query.EQ("email", "exist@gmail.com"));
			Assert.IsNotNull(driver);
		}

		[TestMethod]
		public void TestAddAnotherDriver()
		{
			StationSignUpResponse res1 = new StationSignUpResponse
			{
				api_ret_code = 0,
				api_ret_message = "success",
				session_token = "token1",
				status = 200,
				timestamp = DateTime.UtcNow
			};

			UserLogInResponse res2 = new UserLogInResponse
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

			StationLogOnResponse res3 = new StationLogOnResponse(200, DateTime.UtcNow, "token3");
			res3.api_ret_code = 0;

			using (FakeCloud cloud = new FakeCloud(res1))
			{
				cloud.addJsonResponse(res3);
				cloud.addJsonResponse(res2);
				CloudServer.request<CloudResponse>(new WebClient(), REST_COMMAND_ADD,
					new Dictionary<object, object>{ 
					{ "email", "user1@gmail.com"}, 
					{ "password", "12345"} });


				// verify db
				Driver driver = mongodb.GetDatabase("wammer").
					GetCollection<Driver>("drivers").FindOne(
					Query.EQ("email", "user1@gmail.com"));
				
				Assert.IsNotNull(driver);
				Assert.AreEqual("user1@gmail.com", driver.email);
				Assert.AreEqual(@"resource\user_uid1", driver.folder);
				Assert.AreEqual(res2.user.user_id, driver.user_id);
				Assert.AreEqual(1, driver.groups.Count);
				Assert.AreEqual(res2.session_token, driver.session_token);
				Assert.AreEqual(res2.groups[0].group_id, driver.groups[0].group_id);
				Assert.AreEqual(res2.groups[0].name, driver.groups[0].name);
				Assert.AreEqual(res2.groups[0].description, driver.groups[0].description);

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
