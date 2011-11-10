using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

using Wammer.MultiPart;
using Wammer.Station;
using Wammer.Cloud;

using MongoDB.Bson;
using MongoDB.Driver;

namespace UT_WammerStation
{
	[TestClass]
	public class TestReceiveObjects
	{
		byte[] file;
		byte[] filename;
		byte[] filetype;
		byte[] oid;

		Part filePart;
		Part filenamePart;
		Part filetypePart;
		Part oidPart;

		List<Part> parts = new List<Part>();


		MongoServer mongo;
		FileStorage storage;
		AtomicDictionary<string, FileStorage> groupStoreMap;

		public TestReceiveObjects()
		{
			mongo = MongoServer.Create("mongodb://localhost:10319/?safe=true");
			storage = new FileStorage("resource");
		}

		private Part CreatePart(byte[] data)
		{
			return new Part(data, 0, data.Length, new NameValueCollection());
		}

		[TestInitialize]
		public void SetUp()
		{
			file = new byte[20];
			for (int i = 0; i < file.Length; i++)
				file[i] = (byte)i;
			filename = Encoding.UTF8.GetBytes("file1.jpg");
			filetype = Encoding.UTF8.GetBytes("100");
			oid = Encoding.UTF8.GetBytes("object_id1");

			filePart = CreatePart(file);
			filenamePart = CreatePart(filename);
			filetypePart = CreatePart(filetype);
			oidPart = CreatePart(oid);

			parts.Clear();
			parts.Add(filePart);
			parts.Add(filenamePart);
			parts.Add(filetypePart);
			parts.Add(oidPart);

			MongoDatabase db = mongo.GetDatabase("wammer");
			db.Drop();

			groupStoreMap = new AtomicDictionary<string, FileStorage>();
			groupStoreMap.Add("group1", storage);
		}

		[TestCleanup]
		public void TearDown()
		{
			if (Directory.Exists("resource"))
				Directory.Delete("resource", true);
		}

		[TestMethod]
		public void TestFileStorage()
		{
			FileStorage storage = new FileStorage("resource");
			storage.Save("id1.jpeg", file);

			using (FileStream f = File.OpenRead(@"resource\id1.jpeg"))
			{
				Assert.AreEqual(file.Length, f.Length);
				byte[] savedFile = new byte[file.Length];
				int size = f.Read(savedFile, 0, savedFile.Length);
				Assert.AreEqual(size, f.Length);
				for (int i = 0; i < size; i++)
					Assert.AreEqual(file[i], savedFile[i]);
			}
		}

		[TestMethod]
		public void TestAsyncSave()
		{
			FileStorage storage = new FileStorage("resource");
			IAsyncResult async = storage.BeginSave("id1.jpeg", file, null, null);

			storage.EndSave(async);

			using (FileStream f = File.OpenRead(@"resource\id1.jpeg"))
			{
				Assert.AreEqual(file.Length, f.Length);
				byte[] savedFile = new byte[file.Length];
				int size = f.Read(savedFile, 0, savedFile.Length);
				Assert.AreEqual(size, f.Length);
				for (int i = 0; i < size; i++)
					Assert.AreEqual(file[i], savedFile[i]);
			}
		}

		[TestMethod]
		[Ignore]
		public void TestObjectReceiveHandler_withObjectId()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/test/", new ObjectUploadHandler(mongo, groupStoreMap));
				server.Start();

				FakeClient client = new FakeClient("http://localhost/test/",
															"multipart/form-data; boundary=AaB03x");
				FakeClientResult result = client.PostFile("ObjectUpload1.txt");


				ObjectUploadResponse res = fastJSON.JSON.Instance.ToObject
										<ObjectUploadResponse>(result.ResponseAsText);

				Assert.AreEqual(200, res.status);
				Assert.IsNotNull(res.timestamp);
				Assert.AreEqual(0, res.api_ret_code);
				Assert.AreEqual("Success", res.api_ret_msg);
				Assert.AreEqual("object_id1", res.object_id);


				using (FileStream fs = File.OpenRead(@"resource\object_id1.jpeg"))
				using (StreamReader ss = new StreamReader(fs))
				{
					string fileContent = ss.ReadToEnd();
					Assert.AreEqual("1234567890abcdefghij", fileContent);
				}
			}
		}

		[TestMethod]
		public void TestObjectReceiveHandler_UploadNewOrignalImage()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/test/", new ObjectUploadHandler(mongo, groupStoreMap));
				server.Start();

				FakeClient client = new FakeClient("http://localhost/test/",
															"multipart/form-data; boundary=AaB03x");
				FakeClientResult result = client.PostFile("ObjectUpload1_noObjId.txt");


				ObjectUploadResponse res = fastJSON.JSON.Instance.ToObject
										<ObjectUploadResponse>(result.ResponseAsText);

				Assert.AreEqual(200, res.status);
				Assert.AreEqual(0, res.api_ret_code);

				MongoDatabase db = mongo.GetDatabase("wammer");
				Assert.IsNotNull(db);
				MongoCollection<BsonDocument> attachments = db.GetCollection("attachments");
				Attachment saveData = 
					attachments.FindOneAs<Attachment>(new QueryDocument("_id", res.object_id));

				Assert.IsNotNull(saveData);
				Assert.AreEqual("title1", saveData.title);
				Assert.AreEqual("desc", saveData.description);
				Assert.AreEqual("image/jpeg", saveData.mime_type);
				Assert.AreEqual(StationInfo.BaseURL + "attachments/view/?object_id=" + res.object_id,
								saveData.url);
				Assert.AreEqual(20, saveData.file_size);
				Assert.AreEqual(AttachmentType.image, saveData.type);
				Assert.IsNull(saveData.image_meta);
			}
		}

		[TestMethod]
		public void TestObjectReceiveHandler_withoutObjectId()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/test/", new ObjectUploadHandler(mongo, groupStoreMap));
				server.Start();

				FakeClient client = new FakeClient("http://localhost/test/",
															"multipart/form-data; boundary=AaB03x");
				FakeClientResult result = client.PostFile("ObjectUpload1_noObjId.txt");

				ObjectUploadResponse res = fastJSON.JSON.Instance.ToObject
										<ObjectUploadResponse>(result.ResponseAsText);

				Assert.AreEqual(200, res.status);
				Assert.IsNotNull(res.timestamp);
				Assert.AreEqual(0, res.api_ret_code);
				Assert.AreEqual("Success", res.api_ret_msg);
				Assert.IsNotNull(res.object_id);

				using (FileStream fs = File.OpenRead(@"resource\" + res.object_id + ".jpeg"))
				using (StreamReader ss = new StreamReader(fs))
				{
					string fileContent = ss.ReadToEnd();
					Assert.AreEqual("1234567890abcdefghij", fileContent);
				}
			}
		}

		[TestMethod]
		public void TestObjectReceiveHandler_ClientError()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/test/", new ObjectUploadHandler(mongo, groupStoreMap));
				server.Start();

				FakeClient client = new FakeClient("http://localhost/test/",
														"multipart/form-data; boundary=AaB03x");
				FakeClientResult result = client.PostFile("SingeMultiPart.txt");


				Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
				ObjectUploadResponse res = fastJSON.JSON.Instance.ToObject
										<ObjectUploadResponse>(result.ResponseAsText);

				Assert.AreEqual(400, res.status);
				Assert.IsNotNull(res.timestamp);
				Assert.AreNotEqual(0, res.api_ret_code);
				Assert.AreNotEqual("Success", res.api_ret_msg);
				Assert.AreEqual(null, res.object_id);
			}
		}
	}
}
