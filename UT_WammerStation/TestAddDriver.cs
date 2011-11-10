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

namespace UT_WammerStation
{
	[TestClass]
	public class TestAddDriver
	{
		static MongoServer mongodb;

		WebServiceHost host;
		StationManagementService svc;
		StationDriver addedDriver;

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			mongodb = MongoServer.Create("mongodb://localhost:10319/safe=true");
		}

		[TestInitialize]
		public void setUp()
		{
			svc = new StationManagementService(mongodb, "station_id1");
			host = new WebServiceHost(svc, new Uri("http://localhost:8080/v2/station/"));
			host.Open();

			if (!Directory.Exists(@"C:\TempUT"))
				Directory.CreateDirectory(@"c:\TempUT");
			if (!Directory.Exists(@"C:\TempUT\user1"))
				Directory.CreateDirectory(@"c:\TempUT\user1");

			CloudServer.HostName = "localhost";
			CloudServer.Port = 80;

			mongodb.GetDatabase("wammer").GetCollection<StationDriver>("drivers").RemoveAll();
			mongodb.GetDatabase("wammer").GetCollection<StationDriver>("drivers").Insert(
				new StationDriver { 
					user_id = "exist_uid",
					email = "exist@gmail.com",
					folder = "fo"});

			addedDriver = null;
		}

		[TestCleanup]
		public void tearDown()
		{
			host.Close();

			if (Directory.Exists(@"C:\TempUT"))
				Directory.Delete(@"C:\TempUT", true);
		}

		[TestMethod]
		public void TestAddADriver()
		{
			UserLogInResponse res1 = new UserLogInResponse
			{
				api_ret_msg = "success",
				api_ret_code = 0,
				session_token = "token1",
				status = 200,
				timestamp = DateTime.UtcNow,
				groups = new List<UserGroup>(),
				user = new UserInfo { user_id = "uid1" }
			};

			res1.groups.Add(new UserGroup{ 
				creator_id = "creator1",
				description = "gdesc1",
				group_id = "group_id1",
				name = "group1"});

		    using (FakeCloud cloud = new FakeCloud(res1))
		    {
				cloud.addResponse(new StationSignUpResponse(200, DateTime.Now, "token2"));
				cloud.addResponse(new StationLogOnResponse(200, DateTime.Now, "token3"));
				svc.DriverAdded += new EventHandler<DriverEventArgs>(svc_DriverAdded);
				StationDriver.RequestToAdd("http://localhost:8080/v2/station/drivers/add", "user1@gmail.com", "12345", @"c:\TempUT\user1");

		        // verify db
		        StationDriver driver = mongodb.GetDatabase("wammer").
		            GetCollection<StationDriver>("drivers").FindOne(
		            Query.EQ("email", "user1@gmail.com"));

		        Assert.AreEqual("user1@gmail.com", driver.email);
		        Assert.AreEqual(@"c:\TempUT\user1", driver.folder);
		        Assert.AreEqual(res1.user.user_id, driver.user_id);
		        Assert.AreEqual(1, driver.groups.Count);
		        Assert.AreEqual(res1.groups[0].group_id, driver.groups[0].group_id);
				Assert.AreEqual(res1.groups[0].name, driver.groups[0].name);
				Assert.AreEqual(res1.groups[0].description, driver.groups[0].description);


				// verify event is fired
				Assert.AreEqual(driver.email, addedDriver.email);
				Assert.AreEqual(driver.folder, addedDriver.folder);
				Assert.AreEqual(driver.groups.Count, addedDriver.groups.Count);
				Assert.AreEqual(driver.groups[0].description, addedDriver.groups[0].description);
				Assert.AreEqual(driver.groups[0].group_id, addedDriver.groups[0].group_id);
				Assert.AreEqual(driver.groups[0].name, addedDriver.groups[0].name);
				Assert.AreEqual(driver.user_id, addedDriver.user_id);
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

			using (FakeCloud cloud = new FakeCloud(res1))
			{
				try
				{
					StationDriver.RequestToAdd("http://localhost:8080/v2/station/drivers/add", "user1@gmail.com", "12345", @"c:\TempUT\user1");
				}
				catch (WammerCloudException e)
				{
					Assert.AreEqual((int)StationApiError.AuthFailed, e.WammerError);
					return;
				}

				Assert.Fail("Expected exception is not thrown");
			}
		}

		void svc_DriverAdded(object sender, DriverEventArgs e)
		{
			Assert.IsNotNull(sender);
			addedDriver = e.Driver;
		}

		[TestMethod]
		public void TestAddRegisteredDriver()
		{
			try
			{
				StationDriver.RequestToAdd("http://localhost:8080/v2/station/drivers/add", "exist@gmail.com", "12345", @"c:\TempUT\user1");
			}
			catch (WammerCloudException e)
			{
				Assert.AreEqual((int)StationApiError.DriverExist, e.WammerError);
				return;
			}

			Assert.Fail("expected exception is not thrown");
		}

		[TestMethod]
		public void TestFolderShouldBeAbsPath()
		{
			try
			{
				StationDriver.RequestToAdd("http://localhost:8080/v2/station/drivers/add", "exist@gmail.com", "12345", @"TempUT\user1");
			}
			catch (WammerCloudException e)
			{
				Assert.AreEqual((int)StationApiError.BadPath, e.WammerError);
				return;
			}

			Assert.Fail("expected exception is not thrown");
		}
	}
}
