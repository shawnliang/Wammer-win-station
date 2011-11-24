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
using Wammer.Model;

using MongoDB.Bson;
using MongoDB.Driver;

using System.Drawing;

namespace UT_WammerStation
{
	[TestClass]
	public class TestAttachmentUpload
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
		ObjectUploadResponse cloudResponse;

		public TestAttachmentUpload()
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

			List<UserGroup> groups = new List<UserGroup>();
			groups.Add(new UserGroup{group_id="group1", description="group1 descript", creator_id="driver1_id", name="group1"});
			Drivers.collection.Save(
				new Drivers{
					email = "driver1@waveface.com",
					user_id = "driver1_id",
					folder = "resource",
					groups = groups
				}
			);

			CloudServer.BaseUrl = "http://localhost/v2/";

			cloudResponse = new ObjectUploadResponse
			{
				api_ret_code = 0,
				api_ret_msg = "success",
				status = 200,
				timestamp = DateTime.UtcNow
			};
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
		public void TestObjectReceiveHandler_UploadNewOrignalImage()
		{
			using (FakeCloud cloud = new FakeCloud(cloudResponse))
			using (HttpServer server = new HttpServer(8080))
			{
				server.AddHandler("/test/", new AttachmentUploadHandler());
				server.Start();

				ObjectUploadResponse res = null;
				using (FileStream input = File.OpenRead("Penguins.jpg"))
				using (MemoryStream output = new MemoryStream())
				{
					input.CopyTo(output);
					res = Attachments.UploadImage("http://localhost:8080/test/", output.ToArray(),
						"group1", null, "filename1.jpg", "image/jpeg",
						ImageMeta.Origin, "key1", "token");


					Assert.IsNotNull(res);
					Assert.AreEqual(200, res.status);
					Assert.AreEqual(0, res.api_ret_code);

					MongoDatabase db = mongo.GetDatabase("wammer");
					Assert.IsNotNull(db);
					MongoCollection<BsonDocument> attachments = db.GetCollection("attachments");
					Attachments saveData =
						attachments.FindOneAs<Attachments>(new QueryDocument("_id", res.object_id));

					Assert.IsNotNull(saveData);
					Assert.AreEqual("group1", saveData.group_id);
					Assert.AreEqual(1024, saveData.image_meta.width);
					Assert.AreEqual(768, saveData.image_meta.height);
					Assert.AreEqual("filename1.jpg", saveData.file_name);
					Assert.AreEqual("image/jpeg", saveData.mime_type);
					Assert.AreEqual("/v2/attachments/view/?object_id=" + res.object_id,
									saveData.url);
					Assert.AreEqual(input.Length, saveData.file_size);
					Assert.AreEqual(AttachmentType.image, saveData.type);
					Assert.IsNotNull(saveData.image_meta.medium);
					Assert.AreEqual((int)ImageMeta.Medium, saveData.image_meta.medium.width);
					Assert.AreEqual("image/jpeg", saveData.image_meta.medium.mime_type);
					Assert.AreEqual("/v2/attachments/view/?object_id=" + res.object_id
									+ "&image_meta=medium",
									saveData.image_meta.medium.url);

					using (Bitmap origImg = new Bitmap(Path.Combine("resource", res.object_id + ".jpg")))
					{
						Assert.AreEqual(1024, origImg.Width);
						Assert.AreEqual(768, origImg.Height);
					}

					using (Bitmap mediumImg = new Bitmap(Path.Combine("resource", res.object_id + "_medium.jpeg")))
					{
						Assert.AreEqual(512, mediumImg.Width);
						Assert.AreEqual(384, mediumImg.Height);
					}

				}
			}
		}

		[TestMethod]
		public void TestObjectReceiveHandler_UploadNewSmallImage()
		{
			using (FakeCloud cloud = new FakeCloud(cloudResponse))
			using (HttpServer server = new HttpServer(8080))
			{
				server.AddHandler("/test/", new AttachmentUploadHandler());
				server.Start();

				ObjectUploadResponse res = null;
				using (FileStream input = File.OpenRead("Penguins.jpg"))
				using (MemoryStream output = new MemoryStream())
				{
					input.CopyTo(output);
					res = Attachments.UploadImage("http://localhost:8080/test/", output.ToArray(),
						"group1", null, "filename1.jpg", "image/jpeg",
						ImageMeta.Large, "key1", "token");


					Assert.IsNotNull(res);
					Assert.AreEqual(200, res.status);
					Assert.AreEqual(0, res.api_ret_code);

					MongoCollection<BsonDocument> attachments = mongo.GetDatabase("wammer").
																GetCollection("attachments");
					Attachments saveData =
						attachments.FindOneAs<Attachments>(new QueryDocument("_id", res.object_id));

					Assert.IsNotNull(saveData);
					Assert.AreEqual("group1", saveData.group_id);
					Assert.AreEqual(0, saveData.image_meta.width);
					Assert.AreEqual(0, saveData.image_meta.height);
					Assert.AreEqual("filename1.jpg", saveData.file_name);
					Assert.AreEqual(0, saveData.file_size);
					Assert.AreEqual(null, saveData.mime_type);
					Assert.AreEqual(null, saveData.url);
					
					Assert.AreEqual(AttachmentType.image, saveData.type);
					Assert.IsNotNull(saveData.image_meta.large);
					Assert.AreEqual(1024, saveData.image_meta.large.width);
					Assert.AreEqual(768, saveData.image_meta.large.height);
					Assert.AreEqual("image/jpeg", saveData.image_meta.large.mime_type);
					Assert.AreEqual("/v2/attachments/view/?object_id=" + res.object_id
									+ "&image_meta=large",
									saveData.image_meta.large.url);

					Assert.IsFalse(File.Exists("resource\\" + res.object_id + ".jpg"));

					using (Bitmap largeImg = new Bitmap("resource\\" + res.object_id + "_large.jpg"))
					{
						Assert.AreEqual(1024, largeImg.Width);
						Assert.AreEqual(768, largeImg.Height);
					}

				}
			}
		}

		[TestMethod]
		public void TestObjectReceiveHandler_ClientError()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/test/", new AttachmentUploadHandler());
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
