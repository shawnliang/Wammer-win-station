using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wammer.Station;
using Wammer.Cloud;
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
		
		Attachment doc;
		string objectId1;
		WebServiceHost host;
		AttachmentService svc;

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

			MongoCollection<Attachment> att = wammerDb.GetCollection<Attachment>("attachments");
			doc = new Attachment
			{
				object_id = objectId1,
				title = "title1",
				description = "description1"
			};

			att.Insert(doc);

			svc = new AttachmentService(mongodb);
			host = new WebServiceHost(svc, new Uri("http://localhost:8080/api/"));
			host.Open();
		}

		[TestCleanup]
		public void tearDown()
		{
			if (wammerDb.CollectionExists("attachments"))
				wammerDb.DropCollection("attachments");
			host.Close();
		}

		[TestMethod]
		public void TestGetAttachmentInfo()
		{
			WebClient agent = new WebClient();
			string output = agent.DownloadString(
				"http://localhost:8080/api/get?object_id="+ objectId1 +"&session_token=a&apikey=b");

			Assert.AreNotEqual("", output);
			BsonDocument result =
				MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(output);

			Assert.AreEqual(objectId1, result["object_id"].AsString);
			Assert.AreEqual("title1", result["title"].AsString);
			Assert.AreEqual("description1", result["description"].AsString);
			Assert.IsFalse(result.Contains("_id"));
		}

		[TestMethod]
		public void TestGetAttachmentInfo_paramsCanBeInAnyOrder()
		{
			WebClient agent = new WebClient();
			string output = agent.DownloadString(
				"http://localhost:8080/api/get?apikey=b&object_id=" + objectId1 + "&session_token=a");

			Assert.AreNotEqual("", output);
			BsonDocument result =
				MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(output);

			Assert.AreEqual(objectId1, result["object_id"].AsString);
			Assert.AreEqual("title1", result["title"].AsString);
			Assert.AreEqual("description1", result["description"].AsString);
			Assert.IsFalse(result.Contains("_id"));
		}

		[TestMethod]
		public void TestGetAttachmentInfo_paramCanBeOptional()
		{
			WebClient agent = new WebClient();
			string output = agent.DownloadString(
				"http://localhost:8080/api/get?apikey=b&object_id=" + objectId1);

			Assert.AreNotEqual("", output);
			BsonDocument result =
				MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(output);

			Assert.AreEqual(objectId1, result["object_id"].AsString);
			Assert.AreEqual("title1", result["title"].AsString);
			Assert.AreEqual("description1", result["description"].AsString);
			Assert.IsFalse(result.Contains("_id"));

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
				Assert.AreEqual(HttpStatusCode.NotFound, res.StatusCode);

				using (StreamReader r = new StreamReader(res.GetResponseStream()))
				{
					CloudResponse json = fastJSON.JSON.Instance.ToObject<CloudResponse>(
						r.ReadToEnd());
					Assert.AreEqual(-1, json.api_ret_code);
					Assert.AreEqual((int)HttpStatusCode.NotFound, json.status);
					Assert.AreEqual("object not found: 123", json.api_ret_msg);
				}
				return;
			}
			Assert.Fail("expected error is not thrown");
		}
	}
}
