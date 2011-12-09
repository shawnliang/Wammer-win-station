using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wammer.Station;
using Wammer.Cloud;
using Wammer.Model;
using System.Net;
using System.IO;
using MongoDB.Driver;
using MongoDB.Bson;
using System.ServiceModel;
using System.ServiceModel.Web;

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
		
		Attachments doc;
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

			doc = new Attachments
			{
				object_id = objectId1,
				title = "title1",
				description = "description1"
			};

			Attachments.collection.Insert(doc);


			handler = new AttachmentGetHandler();
			server = new HttpServer(8080);
			server.AddHandler("/api/get/", handler);
			server.Start();
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
				"http://localhost:8080/api/get?object_id="+ objectId1 +"&session_token=a&apikey=b");

			Assert.AreNotEqual("", output);

			AttachmentResponse res = fastJSON.JSON.Instance.ToObject<AttachmentResponse>(output);
			Assert.AreEqual(200, res.status);
			Assert.AreEqual(0, res.api_ret_code);
			Assert.AreEqual("success", res.api_ret_msg);
			Assert.AreEqual(doc.object_id, res.attachment.object_id);
			Assert.AreEqual(doc.title, res.attachment.title);
			Assert.AreEqual(doc.description, res.attachment.description);
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
			Assert.AreEqual("success", res.api_ret_msg);
			Assert.AreEqual(doc.object_id, res.attachment.object_id);
			Assert.AreEqual(doc.title, res.attachment.title);
			Assert.AreEqual(doc.description, res.attachment.description);
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
			Assert.AreEqual("success", res.api_ret_msg);
			Assert.AreEqual(doc.object_id, res.attachment.object_id);
			Assert.AreEqual(doc.title, res.attachment.title);
			Assert.AreEqual(doc.description, res.attachment.description);

		}

		[TestMethod]
		public void TestGetAttachmentInfo_NoObjectId()
		{

			WebClient agent = new WebClient();
			try
			{
				string output = agent.DownloadString("http://localhost:8080/api/get");
			}
			catch (WebException e)
			{
				HttpWebResponse res = (HttpWebResponse)e.Response;
				Assert.AreEqual(HttpStatusCode.BadRequest, res.StatusCode);

				using (StreamReader r = new StreamReader(res.GetResponseStream()))
				{
					CloudResponse json = fastJSON.JSON.Instance.ToObject<CloudResponse>(
						r.ReadToEnd());
					Assert.AreEqual(-1, json.api_ret_code);
					Assert.AreEqual((int)HttpStatusCode.BadRequest, json.status);
					Assert.AreEqual("missing parameter: object_id" , json.api_ret_msg);
				}
				return;
			}
			Assert.Fail("expected error is not thrown");
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
					Assert.AreEqual((int)StationApiError.NotFound, json.api_ret_code);
					Assert.AreEqual("Resource not found: 123", json.api_ret_msg);
				}
				return;
			}
			Assert.Fail("expected error is not thrown");
		}

		[TestMethod]
		public void TestAttachmentMD5()
		{
			Attachments a = new Attachments();
			byte[] data = new byte[100];
			for (int i = 0; i < data.Length; i++)
				data[i] = (byte)i;

			a.RawData = data;

			ThumbnailInfo thumb = new ThumbnailInfo();
			thumb.RawData = data;


			System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] hash = md5.ComputeHash(data);
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
				builder.Append(hash[i].ToString("x2"));

			Assert.AreEqual(builder.ToString(), a.md5);
			Assert.AreEqual(builder.ToString(), thumb.md5);
		}
	}
}
