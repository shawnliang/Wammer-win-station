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
	public class TestAddDriver_alreadyHasADriver
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
			handler = new AddDriverHandler("stationId", "resource");
			server.AddHandler("/v2/station/drivers/add/", handler);
			server.Start();

			if (!Directory.Exists(@"C:\TempUT"))
				Directory.CreateDirectory(@"c:\TempUT");
			if (!Directory.Exists(@"C:\TempUT\user1"))
				Directory.CreateDirectory(@"c:\TempUT\user1");

			CloudServer.BaseUrl = "http://localhost/v2/";

			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").RemoveAll();
			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").Insert(
				new Driver
				{
					user_id = "exist_uid",
					email = "exist@gmail.com",
					folder = "fo",
					groups = new List<UserGroup> { new UserGroup { group_id = "123" } }
				});
		}

		[TestCleanup]
		public void tearDown()
		{
			server.Close();

			if (Directory.Exists(@"C:\TempUT"))
				Directory.Delete(@"C:\TempUT", true);

			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").RemoveAll();
		}

		[TestMethod]
		public void TestAddRegisteredDriver()
		{
			try
			{
				CloudServer.request<CloudResponse>(
						new WebClient(),
						"http://localhost:8080/v2/station/drivers/add",
						new Dictionary<object, object>{ 
							{ "email", "exist@gmail.com"}, 
							{ "password", "12345"} 
						});
			}
			catch (WammerCloudException e)
			{
				Assert.AreEqual((int)StationApiError.DriverExist, e.WammerError);
				return;
			}

			Assert.Fail("expected exception is not thrown");
		}

		[TestMethod]
		public void TestAlreadyHasAUser()
		{
			try
			{
				CloudServer.request<CloudResponse>(
						new WebClient(),
						"http://localhost:8080/v2/station/drivers/add",
						new Dictionary<object, object>{ 
							{ "email", "new_user@gmail.com"}, 
							{ "password", "12345"} 
						});
			}
			catch (WammerCloudException e)
			{
				Assert.AreEqual((int)StationApiError.DriverExist, e.WammerError);
				return;
			}

			Assert.Fail("expected exception is not thrown");
		}
	}
}
