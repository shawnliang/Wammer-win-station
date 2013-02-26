using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System;
using System.IO;
using System.Net;
using Wammer;
using Wammer.Cloud;
using Wammer.PerfMonitor;
using Wammer.Station;
using Waveface.Stream.Model;

namespace UT_WammerStation
{
	/// <summary>
	/// Summary description for TestAttachmentGet
	/// </summary>
	[TestClass]
	public class TestAttachmentGet
	{
		static MongoServer mongodb;
		static MongoDatabase wammerDb;

		Attachment doc;
		string objectId1;

		HttpServer server;
		AttachmentGetHandler handler;

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			mongodb = MongoServer.Create("mongodb://localhost:10319/safe=true");
		}

		[TestInitialize]
		public void setUp()
		{
			wammerDb = mongodb.GetDatabase("wammer");
			if (wammerDb.CollectionExists("attachments"))
				wammerDb.DropCollection("attachments");

			wammerDb.CreateCollection("attachments");

			objectId1 = Guid.NewGuid().ToString();

			doc = new Attachment
			{
				object_id = objectId1,
				title = "title1",
				description = "description1"
			};

			AttachmentCollection.Instance.Save(doc);


			handler = new AttachmentGetHandler();
			server = new HttpServer(8080);
			server.AddHandler("/api/get/", handler);
			server.Start();
			server.TaskEnqueue += new EventHandler<TaskQueueEventArgs>(HttpRequestMonitor.Instance.OnTaskEnqueue);
			//host = new WebServiceHost(svc, new Uri("http://localhost:8080/api/"));
			//host.Open();
		}

		[TestCleanup]
		public void tearDown()
		{
			if (wammerDb.CollectionExists("attachments"))
				wammerDb.DropCollection("attachments");

			server.Close();
		}

		[TestMethod]
		public void TestGetAttachmentInfo()
		{
			WebClient agent = new WebClient();
			string output = agent.DownloadString(
				"http://localhost:8080/api/get?object_id=" + objectId1 + "&session_token=a&apikey=b");

			Assert.AreNotEqual("", output);

			AttachmentResponse res = fastJSON.JSON.Instance.ToObject<AttachmentResponse>(output);
			Assert.AreEqual(200, res.status);
			Assert.AreEqual(0, res.api_ret_code);
			Assert.AreEqual("success", res.api_ret_message);
			Assert.AreEqual(doc.object_id, res.object_id);
			Assert.AreEqual(doc.title, res.title);
			Assert.AreEqual(doc.description, res.description);
		}

		[TestMethod]
		public void TestGetAttachmentInfo_paramsCanBeInAnyOrder()
		{
			WebClient agent = new WebClient();
			string output = agent.DownloadString(
				"http://localhost:8080/api/get?apikey=b&object_id=" + objectId1 + "&session_token=a");

			Assert.AreNotEqual("", output);
			AttachmentResponse res = fastJSON.JSON.Instance.ToObject<AttachmentResponse>(output);
			Assert.AreEqual(200, res.status);
			Assert.AreEqual(0, res.api_ret_code);
			Assert.AreEqual("success", res.api_ret_message);
			Assert.AreEqual(doc.object_id, res.object_id);
			Assert.AreEqual(doc.title, res.title);
			Assert.AreEqual(doc.description, res.description);
		}

		[TestMethod]
		public void TestGetAttachmentInfo_paramCanBeOptional()
		{
			WebClient agent = new WebClient();
			string output = agent.DownloadString(
				"http://localhost:8080/api/get?apikey=b&object_id=" + objectId1);

			Assert.AreNotEqual("", output);
			AttachmentResponse res = fastJSON.JSON.Instance.ToObject<AttachmentResponse>(output);
			Assert.AreEqual(200, res.status);
			Assert.AreEqual(0, res.api_ret_code);
			Assert.AreEqual("success", res.api_ret_message);
			Assert.AreEqual(doc.object_id, res.object_id);
			Assert.AreEqual(doc.title, res.title);
			Assert.AreEqual(doc.description, res.description);

		}

		[TestMethod]
		[ExpectedException(typeof(WebException))]
		public void TestGetAttachmentInfo_NoObjectId()
		{
			using (var agent = new WebClient())
			{
				agent.DownloadString("http://localhost:8080/api/get");
			}
		}

		[TestMethod]
		public void TestGetAttachmentInfo_attachmentNotFound()
		{

			WebClient agent = new WebClient();
			try
			{
				string output = agent.DownloadString("http://localhost:8080/api/get?object_id=123");
			}
			catch (WebException e)
			{
				HttpWebResponse res = (HttpWebResponse)e.Response;
				Assert.AreEqual(HttpStatusCode.BadRequest, res.StatusCode);

				using (StreamReader r = new StreamReader(res.GetResponseStream()))
				{
					CloudResponse json = fastJSON.JSON.Instance.ToObject<CloudResponse>(
						r.ReadToEnd());
					Assert.AreEqual((int)StationLocalApiError.NotFound, json.api_ret_code);
					Assert.AreEqual("Resource not found: 123", json.api_ret_message);
				}
				return;
			}
			Assert.Fail("expected error is not thrown");
		}
	}
}
