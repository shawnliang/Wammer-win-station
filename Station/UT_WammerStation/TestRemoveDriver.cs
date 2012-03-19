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
	public class TestRemoveDriver
	{
		static MongoServer mongodb;
		HttpServer server;
		RemoveOwnerHandler handler;

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			mongodb = MongoServer.Create("mongodb://localhost:10319/safe=true");
		}

		[TestInitialize]
		public void setUp()
		{
			server = new HttpServer(8080);
			handler = new RemoveOwnerHandler("stationId");
			server.AddHandler("/v2/station/drivers/remove/", handler);
			server.Start();

			if (!Directory.Exists(@"C:\TempUT"))
				Directory.CreateDirectory(@"c:\TempUT");
			if (!Directory.Exists(@"C:\TempUT\user1"))
				Directory.CreateDirectory(@"c:\TempUT\user1");

			CloudServer.BaseUrl = "http://localhost/v2/";

			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").RemoveAll();
			mongodb.GetDatabase("wammer").GetCollection("station").RemoveAll();
		}

		[TestCleanup]
		public void tearDown()
		{
			server.Close();

			if (Directory.Exists(@"C:\TempUT"))
				Directory.Delete(@"C:\TempUT", true);

			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").RemoveAll();
			mongodb.GetDatabase("wammer").GetCollection("station").RemoveAll();
		}

		[TestMethod]
		public void TestRemoveDriver_RemoveWhenExistMoreThanOneItem()
		{
			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").RemoveAll();
			mongodb.GetDatabase("wammer").GetCollection("station").RemoveAll();

			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").Insert(
				new Driver
				{
					user_id = "exist_uid1",
					email = "exist1@gmail.com",
					folder = "resource\\user_exist_uid1",
					groups = new List<UserGroup> { new UserGroup { group_id = "123" } }
				});
			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").Insert(
				new Driver
				{
					user_id = "exist_uid2",
					email = "exist2@gmail.com",
					folder = "resource\\user_exist_uid2",
					groups = new List<UserGroup> { new UserGroup { group_id = "123" } }
				});

			mongodb.GetDatabase("wammer").GetCollection("station").Insert(
				new StationInfo() 
				{
					Id = "1234"
				});

			using (FakeCloud cloud = new FakeCloud(new CloudResponse()))
			{
				CloudServer.request<CloudResponse>(new WebClient(), "http://localhost:8080/v2/station/drivers/remove",
					new Dictionary<object, object>{
						{ "session_token", "token123"},
						{ "user_ID", "exist_uid1"}
					});

				Assert.IsNotNull(Wammer.Model.StationCollection.Instance.FindOne());
			}
		}


		[TestMethod]
		public void TestRemoveDriver_RemoveWhenSingleItem()
		{
			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").RemoveAll();
			mongodb.GetDatabase("wammer").GetCollection("station").RemoveAll();

			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").Insert(
				new Driver
				{
					user_id = "exist_uid1",
					email = "exist1@gmail.com",
					folder = "resource\\user_exist_uid1",
					groups = new List<UserGroup> { new UserGroup { group_id = "123" } }
				});

			mongodb.GetDatabase("wammer").GetCollection("station").Insert(
				new StationInfo()
				{
					Id = "1234"
				});

			using (FakeCloud cloud = new FakeCloud(new CloudResponse()))
			{
				CloudServer.request<CloudResponse>(new WebClient(), "http://localhost:8080/v2/station/drivers/remove",
					new Dictionary<object, object>{ 
						{ "session_token", "token123"}, 
						{ "user_ID", "exist_uid1"} 
					});

				Assert.IsNull(Wammer.Model.StationCollection.Instance.FindOne());
			}
		}

	}
}
