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

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			mongodb = MongoServer.Create("mongodb://localhost:10319/safe=true");
		}

		[TestInitialize]
		public void setUp()
		{
			svc = new StationManagementService(mongodb, "station_id1");
			host = new WebServiceHost(svc, new Uri("http://localhost:8080/v2/"));
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
				app_ret_msg = "success",
				app_ret_code = 0,
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

		        HttpWebRequest request = (HttpWebRequest)
		            WebRequest.Create("http://localhost:8080/v2/station/drivers/add");
		        request.Method = "POST";
		        request.ContentType = "application/x-www-form-urlencoded";

		        using (StreamWriter w = new StreamWriter(request.GetRequestStream()))
		        {
		            w.Write("session_token=token");
		            w.Write("&");
		            w.Write("email=" + HttpUtility.UrlEncode("user1@gmail.com"));
		            w.Write("&");
		            w.Write("password=12345");
		            w.Write("&");
		            w.Write("folder=" + HttpUtility.UrlEncode(@"c:\TempUT\user1"));
		        }

		        // verify response
		        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
		        using (StreamReader r = new StreamReader(response.GetResponseStream()))
		        {
		            CloudResponse json = fastJSON.JSON.Instance.ToObject<CloudResponse>(r.ReadToEnd());
		            Assert.AreEqual(0, json.app_ret_code);
		            Assert.AreEqual(200, json.status);
		            Assert.AreEqual("success", json.app_ret_msg);
		            Assert.IsTrue(json.timestamp - DateTime.UtcNow < TimeSpan.FromSeconds(10));
		        }

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
		    }
		}

		[TestMethod]
		public void TestAddRegisteredDriver()
		{
			HttpWebRequest request = (HttpWebRequest)
				WebRequest.Create("http://localhost:8080/v2/station/drivers/add");
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			using (StreamWriter w = new StreamWriter(request.GetRequestStream()))
			{
				w.Write("session_token=token");
				w.Write("&");
				w.Write("email=" + HttpUtility.UrlEncode("exist@gmail.com"));
				w.Write("&");
				w.Write("password=12345");
				w.Write("&");
				w.Write("folder=" + HttpUtility.UrlEncode(@"c:\TempUT\user1"));
			}

			// verify response
			try
			{
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			}
			catch (WebException e)
			{
				Assert.AreEqual(HttpStatusCode.Conflict, ((HttpWebResponse)e.Response).StatusCode);
				using (StreamReader r = new StreamReader(e.Response.GetResponseStream()))
				{
					CloudResponse json = fastJSON.JSON.Instance.ToObject<CloudResponse>(r.ReadToEnd());
					Assert.AreEqual((int)StationApiError.DriverExist, json.app_ret_code);
					Assert.AreEqual((int)HttpStatusCode.Conflict, json.status);
					Assert.AreEqual("already registered", json.app_ret_msg);
				}

				return;
			}

			Assert.Fail("expected exception is not thrown");
		}

		[TestMethod]
		public void TestFolderShouldBeAbsPath()
		{
			HttpWebRequest request = (HttpWebRequest)
				WebRequest.Create("http://localhost:8080/v2/station/drivers/add");
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			using (StreamWriter w = new StreamWriter(request.GetRequestStream()))
			{
				w.Write("session_token=token");
				w.Write("&");
				w.Write("email=" + HttpUtility.UrlEncode("user1@gmail.com"));
				w.Write("&");
				w.Write("password=12345");
				w.Write("&");
				w.Write("folder=" + HttpUtility.UrlEncode(@"TempUT\user1"));
			}

			// verify response
			try
			{
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			}
			catch (WebException e)
			{
				Assert.AreEqual(HttpStatusCode.BadRequest, ((HttpWebResponse)e.Response).StatusCode);
				using (StreamReader r = new StreamReader(e.Response.GetResponseStream()))
				{
					CloudResponse json = fastJSON.JSON.Instance.ToObject<CloudResponse>(r.ReadToEnd());
					Assert.AreEqual((int)StationApiError.BadPath, json.app_ret_code);
					Assert.AreEqual((int)HttpStatusCode.BadRequest, json.status);
					Assert.AreEqual("folder is not an absolute path", json.app_ret_msg);
				}

				return;
			}

			Assert.Fail("expected exception is not thrown");
		}
	}
}
