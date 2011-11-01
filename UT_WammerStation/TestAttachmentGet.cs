using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wammer.Station;
using System.Net;
using System.IO;
using MongoDB.Driver;
using MongoDB.Bson;
using Wammer.Cloud;
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
		static BsonDocument doc;


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
			MongoCollection<BsonDocument> att = wammerDb.GetCollection<BsonDocument>("attachments");
			doc = new BsonDocument
			{
				{"object_id", "object_id_abc"},
				{"title", "title1"},
				{"description", "description1"},
			};

			att.Insert(doc);
		}

		[TestCleanup]
		public void tearDown()
		{
			if (wammerDb.CollectionExists("attachments"))
				wammerDb.DropCollection("attachments");
		}

		[TestMethod]
		public void TestGetAttachmentInfo()
		{
			AttachmentService svc = new AttachmentService(mongodb);
			WebServiceHost host = new WebServiceHost(svc, new Uri("http://localhost:8080/api/"));
			host.Open();

			WebClient agent = new WebClient();
			string output = agent.DownloadString(
				"http://localhost:8080/api/get?object_id=object_id_abc&session_token=a&apikey=b");

			Assert.AreNotEqual("", output);
			BsonDocument result = 
				MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(output);

			Assert.AreEqual("object_id_abc", result["object_id"].AsString);
			Assert.AreEqual("title1", result["title"].AsString);
			Assert.AreEqual("description1", result["description"].AsString);
			Assert.IsFalse(result.Contains("_id"));

			host.Close();
		}

		[TestMethod]
		public void TestGetAttachmentInfo_paramsCanBeInAnyOrder()
		{
			AttachmentService svc = new AttachmentService(mongodb);
			WebServiceHost host = new WebServiceHost(svc, new Uri("http://localhost:8080/api/"));
			host.Open();

			WebClient agent = new WebClient();
			string output = agent.DownloadString(
				"http://localhost:8080/api/get?apikey=b&object_id=object_id_abc&session_token=a");

			Assert.AreNotEqual("", output);
			BsonDocument result =
				MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(output);

			Assert.AreEqual("object_id_abc", result["object_id"].AsString);
			Assert.AreEqual("title1", result["title"].AsString);
			Assert.AreEqual("description1", result["description"].AsString);
			Assert.IsFalse(result.Contains("_id"));

			host.Close();
		}

		[TestMethod]
		public void TestGetAttachmentInfo_paramCanBeOptional()
		{
			AttachmentService svc = new AttachmentService(mongodb);
			WebServiceHost host = new WebServiceHost(svc, new Uri("http://localhost:8080/api/"));
			host.Open();

			WebClient agent = new WebClient();
			string output = agent.DownloadString(
				"http://localhost:8080/api/get?apikey=b&object_id=object_id_abc");

			Assert.AreNotEqual("", output);
			BsonDocument result =
				MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(output);

			Assert.AreEqual("object_id_abc", result["object_id"].AsString);
			Assert.AreEqual("title1", result["title"].AsString);
			Assert.AreEqual("description1", result["description"].AsString);
			Assert.IsFalse(result.Contains("_id"));

			host.Close();
		}
	}
}
