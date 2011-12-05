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
	public class TestAddDriver
	{
		static MongoServer mongodb;

		WebServiceHost host;
		StationManagementService svc;


		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			mongodb = MongoServer.Create("mongodb://localhost:10319/safe=true");
		}

		[TestInitialize]
		public void setUp()
		{
			svc = new StationManagementService("resource", "stationId");
			host = new WebServiceHost(svc, new Uri("http://localhost:8080/v2/station/"));
			host.Open();

			if (!Directory.Exists(@"C:\TempUT"))
				Directory.CreateDirectory(@"c:\TempUT");
			if (!Directory.Exists(@"C:\TempUT\user1"))
				Directory.CreateDirectory(@"c:\TempUT\user1");

			CloudServer.BaseUrl = "http://localhost/v2/";

			mongodb.GetDatabase("wammer").GetCollection<Drivers>("drivers").RemoveAll();
			mongodb.GetDatabase("wammer").GetCollection("station").RemoveAll();
		}

		[TestCleanup]
		public void tearDown()
		{
			host.Close();

			if (Directory.Exists(@"C:\TempUT"))
				Directory.Delete(@"C:\TempUT", true);

			mongodb.GetDatabase("wammer").GetCollection<Drivers>("drivers").RemoveAll();
			mongodb.GetDatabase("wammer").GetCollection("station").RemoveAll();
		}

		[TestMethod]
		public void TestAddADriver()
		{
			StationSignUpResponse res1 = new StationSignUpResponse
			{
				api_ret_code = 0,
				api_ret_msg = "success",
				session_token = "token1",
				status = 200,
				timestamp = DateTime.UtcNow
			};

			UserLogInResponse res2 = new UserLogInResponse
			{
				api_ret_msg = "success",
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
				CloudServer.request<CloudResponse>(new WebClient(), "http://localhost:8080/v2/station/drivers/add",
					new Dictionary<object, object>{ 
					{ "email", "user1@gmail.com"}, 
					{ "password", "12345"} });


		        // verify db
		        Drivers driver = mongodb.GetDatabase("wammer").
		            GetCollection<Drivers>("drivers").FindOne(
		            Query.EQ("email", "user1@gmail.com"));

		        Assert.AreEqual("user1@gmail.com", driver.email);
		        Assert.AreEqual(@"resource\group1", driver.folder);
		        Assert.AreEqual(res2.user.user_id, driver.user_id);
		        Assert.AreEqual(1, driver.groups.Count);
				Assert.AreEqual(res2.session_token, driver.session_token);
		        Assert.AreEqual(res2.groups[0].group_id, driver.groups[0].group_id);
				Assert.AreEqual(res2.groups[0].name, driver.groups[0].name);
				Assert.AreEqual(res2.groups[0].description, driver.groups[0].description);

				//verify station
				Wammer.Model.StationInfo s = Wammer.Model.StationInfo.collection.FindOne();
				Assert.IsNotNull(s);
				Assert.AreEqual("token3", s.SessionToken);
		    }
		}

		[TestMethod]
		public void TestAddADriver_incorrectUserNamePwd()
		{
			UserLogInResponse res1 = new UserLogInResponse
			{
				api_ret_msg = "station res msg",
				api_ret_code = 4097, // cloud retuns 4097 for invalid user name or password
				session_token = "token1",
				status = (int)HttpStatusCode.Forbidden,
				timestamp = DateTime.UtcNow,
				groups = new List<UserGroup>(),
				user = new UserInfo { user_id = "uid1" }
			};

			using (FakeCloud cloud = new FakeCloud(res1, res1.status))
			{
				try
				{
					CloudServer.request<CloudResponse>(
						new WebClient(), 
						"http://localhost:8080/v2/station/drivers/add",
						new Dictionary<object, object>{ 
							{ "email", "user1@gmail.com"}, 
							{ "password", "12345"} 
						});
				}
				catch (WammerCloudException e)
				{
					Assert.AreEqual((int)StationApiError.AuthFailed, e.WammerError);
					return;
				}

				Assert.Fail("Expected exception is not thrown");
			}
		}

		[TestMethod]
		public void TestAddADriver_AlreadyHasAStation()
		{
			StationSignUpResponse res1 = new StationSignUpResponse
			{
				api_ret_msg = "fail",
				api_ret_code = 16387, // already has station
				timestamp = DateTime.UtcNow,
				status = 400,
				station = new UserStation
				{
					creator_id = Guid.NewGuid().ToString(),
					location = "http://location.com/",
					station_id = Guid.NewGuid().ToString(),
					status = "connected",
					LastSeen = DateTime.UtcNow
				}
			};

			using (FakeCloud cloud = new FakeCloud(res1, res1.status))
			{
				try
				{
					CloudServer.request<CloudResponse>(
						new WebClient(),
						"http://localhost:8080/v2/station/drivers/add",
						new Dictionary<object, object>{ 
							{ "email", "user1@gmail.com"}, 
							{ "password", "12345"} 
						});
				}
				catch (WammerCloudException e)
				{
					Assert.AreEqual((int)StationApiError.AlreadyHasStaion, e.WammerError);
					AddUserResponse res = fastJSON.JSON.Instance.ToObject<AddUserResponse>(e.response);
					Assert.AreEqual(res1.station.station_id, res.station.station_id);
					Assert.AreEqual(res1.station.location, res.station.location);
					Assert.AreEqual(res1.station.last_seen, res.station.last_seen);
					return;
				}

				Assert.Fail("Expected exception is not thrown");
			}
		}
	}
}
