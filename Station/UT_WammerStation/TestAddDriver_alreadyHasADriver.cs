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

			CloudServer.HostName = "localhost";
			CloudServer.Port = 80;

			mongodb.GetDatabase("wammer").GetCollection<Drivers>("drivers").RemoveAll();
			mongodb.GetDatabase("wammer").GetCollection<Drivers>("drivers").Insert(
				new Drivers
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
			host.Close();

			if (Directory.Exists(@"C:\TempUT"))
				Directory.Delete(@"C:\TempUT", true);

			mongodb.GetDatabase("wammer").GetCollection<Drivers>("drivers").RemoveAll();
		}

		[TestMethod]
		public void TestAddRegisteredDriver()
		{
			try
			{
				Drivers.RequestToAdd("http://localhost:8080/v2/station/drivers/add", 
					"exist@gmail.com", "12345");
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
				Drivers.RequestToAdd("http://localhost:8080/v2/station/drivers/add", 
					"new_user@gmail.com", "12345");
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
