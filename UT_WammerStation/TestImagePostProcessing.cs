using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Net;

using Wammer.Station;
using Wammer.Cloud;
using MongoDB.Driver;
using MongoDB.Bson;

namespace UT_WammerStation
{
	class DummyImageUploadHandler : HttpHandler
	{
		public static List<UploadedFile> recvFiles;
		public static NameValueCollection recvParameters;
		public static CookieCollection recvCookies;

		public static AutoResetEvent evt = new AutoResetEvent(false);

		protected override void HandleRequest()
		{
			recvFiles = this.Files;
			recvParameters= this.Parameters;
			recvCookies = this.Request.Cookies;

			HttpHelper.RespondSuccess(Response,
				ObjectUploadResponse.CreateSuccess(recvParameters["object_id"]));
			evt.Set();
		}

		public static bool Wait()
		{
			return evt.WaitOne(TimeSpan.FromSeconds(10));
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}

	class DummyRequestCompletedHandler
	{
		private ManualResetEvent signal = new ManualResetEvent(false);

		public bool EventReceived()
		{
			return signal.WaitOne(TimeSpan.FromSeconds(10));
		}

		public void Handle(object sender, ImageAttachmentEventArgs evt)
		{
			signal.Set();
		}
	}

	[TestClass]
	public class TestImagePostProcessing
	{
		byte[] imageRawData;
		string object_id1;
		static MongoDB.Driver.MongoServer mongodb;

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			mongodb = MongoDB.Driver.MongoServer.Create("mongodb://localhost:10319/?safe=true");
		}

		[TestInitialize]
		public void setUp()
		{
			using (FileStream f = File.OpenRead("Penguins.jpg"))
			using (BinaryReader r = new BinaryReader(f))
			{
				imageRawData = r.ReadBytes((int)f.Length);
			}

			Wammer.Cloud.CloudServer.HostName = "localhost";
			Wammer.Cloud.CloudServer.Port = 8080;
			Wammer.Cloud.CloudServer.SessionToken = "thisIsASessionToken";
			DummyImageUploadHandler.evt.Reset();

			if (mongodb.GetDatabase("wammer").CollectionExists("attachments"))
				mongodb.GetDatabase("wammer").DropCollection("attachments");

			mongodb.GetDatabase("wammer").CreateCollection("attachments");
			MongoCollection<Attachment> atts = mongodb.GetDatabase("wammer").GetCollection<
				Attachment>("attachments");

			object_id1 = Guid.NewGuid().ToString();
			atts.Insert(new Attachment
			{
				object_id = object_id1,
				title = "orig_title",
				description = "orig_desc",
				type = AttachmentType.image,
				image_meta = new ImageProperty {
					small = new ThumbnailInfo {
								mime_type = "image/jpeg",
								url = "http://url/",
								file_size = 123,
								width = 1000,
								height = 2000
							}
					
				}
			});
		}

		[TestMethod]
		public void TestObjectUploadHandler_ResponseCompleted()
		{
			using (HttpServer cloud = new HttpServer(8080))
			using (HttpServer server = new HttpServer(80))
			{
				FileStorage fileStore = new FileStorage("resource");
				ObjectUploadHandler handler = new ObjectUploadHandler(fileStore, mongodb);

				DummyRequestCompletedHandler evtHandler = new DummyRequestCompletedHandler();
				handler.ImageAttachmentCompleted += evtHandler.Handle;

				server.AddHandler("/test/", handler);
				server.Start();

				cloud.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/upload/",
																	new DummyImageUploadHandler());
				cloud.Start();

				ObjectUploadResponse res = Wammer.Cloud.Attachment.UploadImage(
														"http://localhost:80/test/", imageRawData,
												"orig_name.jpeg", "image/jpeg", ImageMeta.Origin);

				Assert.IsTrue(evtHandler.EventReceived());
			}
		}

		[TestMethod]
		public void TestObjectUploadHandler_SessionTokenIsInParams()
		{
			using (HttpServer server = new HttpServer(80))
			{
				DummyImageUploadHandler handler = new DummyImageUploadHandler();
				server.AddHandler("/test/", handler);
				server.Start();

				ObjectUploadResponse res = Wammer.Cloud.Attachment.UploadImage(
														"http://localhost:80/test/", imageRawData,
												"orig_name.jpeg", "image/jpeg", ImageMeta.Origin);

				Assert.AreEqual("thisIsASessionToken",
					DummyImageUploadHandler.recvParameters["session_token"]);
			}
		}

		[TestMethod]
		public void TestStationRecv_NewOriginalImage()
		{
			using (HttpServer cloud = new HttpServer(8080))
			using (HttpServer server = new HttpServer(80))
			{
				FileStorage fileStore = new FileStorage("resource");
				ImagePostProcessing postProc = new ImagePostProcessing(fileStore);
				ObjectUploadHandler handler = new ObjectUploadHandler(fileStore, mongodb);

				handler.ImageAttachmentSaved += postProc.HandleImageAttachmentSaved;

				server.AddHandler("/test/", handler);
				server.Start();

				cloud.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/upload/",
																	new DummyImageUploadHandler());
				cloud.Start();

				ObjectUploadResponse res = Wammer.Cloud.Attachment.UploadImage(
														"http://localhost:80/test/", imageRawData,
												"orig_name2.jpeg", "image/jpeg",ImageMeta.Origin);

				// verify
				Assert.IsTrue(DummyImageUploadHandler.Wait());
				Assert.AreEqual(1, DummyImageUploadHandler.recvFiles.Count);
				string thumbnail_id = Path.GetFileNameWithoutExtension(
														DummyImageUploadHandler.recvFiles[0].Name);
				Guid thumbnail_guid = new Guid(thumbnail_id);
				Assert.AreNotEqual(thumbnail_id, res.object_id); // a new id is created for thumnail

				Assert.AreEqual("image/jpeg", DummyImageUploadHandler.recvFiles[0].ContentType);
				Assert.AreEqual(res.object_id, DummyImageUploadHandler.recvParameters["object_id"]);
				Assert.AreEqual("small", DummyImageUploadHandler.recvParameters["image_meta"]);
				Assert.AreEqual("image", DummyImageUploadHandler.recvParameters["type"]);

				using (FileStream f = fileStore.Load(thumbnail_id + ".jpeg"))
				{
					Bitmap saveImg = new Bitmap(f);
					Assert.AreEqual((int)Wammer.Cloud.ImageMeta.Small, saveImg.Width);

					Assert.AreEqual(f.Length, DummyImageUploadHandler.recvFiles[0].Data.Length);
					f.Position = 0;
					byte[] imageData = new byte[f.Length];
					Assert.AreEqual(imageData.Length, f.Read(imageData, 0, imageData.Length));

					for (int i = 0; i < f.Length; i++)
					{
						Assert.AreEqual(imageData[i], DummyImageUploadHandler.recvFiles[0].Data[i]);
					}
				}

			}
		}

		[TestMethod]
		public void TestStationRecv_OldOriginalImage()
		{
			using (HttpServer server = new HttpServer(80))
			{
				FileStorage fileStore = new FileStorage("resource");
				ObjectUploadHandler handler = new ObjectUploadHandler(fileStore, mongodb);
				server.AddHandler("/test/", handler);
				server.Start();

				ObjectUploadResponse res = Wammer.Cloud.Attachment.UploadImage(
														"http://localhost:80/test/", imageRawData,
									object_id1 ,"orig_name2.png", "image/png", ImageMeta.Origin);

				// verify saved file
				using (FileStream f = fileStore.Load(object_id1+".png"))
				{
					byte[] imageData = new byte[f.Length];
					Assert.AreEqual(imageData.Length, f.Read(imageData, 0, imageData.Length));

					for (int i = 0; i < f.Length; i++)
					{
						Assert.AreEqual(imageData[i], imageRawData[i]);
					}
				}

				// verify db
				MongoCursor<Attachment> cursor = 
				mongodb.GetDatabase("wammer").GetCollection<Attachment>("attachments")
					.Find(new QueryDocument("_id", object_id1));


				Assert.AreEqual<long>(1,cursor.Count());
				foreach (Attachment doc in cursor)
				{
					Assert.AreEqual(object_id1, doc.object_id);
					Assert.AreEqual("orig_desc", doc.description);
					Assert.AreEqual("orig_title", doc.title);
					Assert.AreEqual(AttachmentType.image, doc.type);
					Assert.AreEqual(imageRawData.Length, doc.file_size);
					Assert.AreEqual("image/png", doc.mime_type);
				}
			}
		}

		[TestMethod]
		public void TestStationRecv_NewThumbnailImage()
		{
			using (HttpServer server = new HttpServer(80))
			{
				FileStorage fileStore = new FileStorage("resource");
				ObjectUploadHandler handler = new ObjectUploadHandler(fileStore, mongodb);
				server.AddHandler("/test/", handler);
				server.Start();

				string oid = Guid.NewGuid().ToString();
				ObjectUploadResponse res = Wammer.Cloud.Attachment.UploadImage(
														"http://localhost:80/test/", imageRawData,
								oid,"orig_name2.jpeg", "image/jpeg", ImageMeta.Large);

				// verify
				Assert.AreEqual(oid, res.object_id);
				using (FileStream f = fileStore.Load(oid+"_large.jpeg"))
				{
					Assert.AreEqual((long)imageRawData.Length, f.Length);
					for (int i = 0; i < imageRawData.Length; i++)
					{
						Assert.AreEqual((int)imageRawData[i], f.ReadByte());
					}
				}

				BsonDocument saveData = mongodb.GetDatabase("wammer").
					GetCollection("attachments").FindOne(
					new QueryDocument("_id", oid));

				Assert.IsNotNull(saveData);
				Assert.IsFalse(saveData.Contains("title"));
				Assert.IsFalse(saveData.Contains("description"));
				Assert.IsFalse(saveData.Contains("mime_type"));
				Assert.IsFalse(saveData.Contains("url"));
				Assert.IsFalse(saveData.Contains("file_size"));
				Assert.AreEqual((int)AttachmentType.image, saveData["type"].AsInt32);
				BsonDocument meta = saveData["image_meta"].AsBsonDocument;
				Assert.AreEqual(
					string.Format("http://{0}:9981/v2/attachments/view/?object_id={1}&image_meta=large",
						StationInfo.IPv4Address, res.object_id),
					meta["large"].AsBsonDocument["url"].AsString);

				Assert.AreEqual(res.object_id + "_large.jpeg",
					meta["large"].AsBsonDocument["file_name"]);
				Assert.AreEqual(imageRawData.Length, meta["large"].AsBsonDocument["file_size"].AsInt32);
				Assert.AreEqual("image/jpeg", meta["large"].AsBsonDocument["mime_type"].AsString);
			}
		}

		[TestMethod]
		public void TestHandleImageAttachmentSaved()
		{
			ImageAttachmentEventArgs args = new ImageAttachmentEventArgs(
				new Attachment
				{
					title = "title1",
					mime_type = "image/jpeg",
					type = AttachmentType.image,
					RawData = imageRawData,
					object_id = object_id1
				},
				ImageMeta.Origin,
				mongodb.GetDatabase("wammer").GetCollection<BsonDocument>("attachments")
			);

			ImagePostProcessing post = new ImagePostProcessing(new FileStorage("resource"));
			post.HandleImageAttachmentSaved(this, args);

			//save
			Attachment doc = mongodb.GetDatabase("wammer").
				GetCollection<Attachment>("attachments").FindOne(
				new QueryDocument("_id", args.Attachment.object_id));

			Assert.AreEqual(1024, doc.image_meta.width);
			Assert.AreEqual(768, doc.image_meta.height);
			Assert.AreEqual("orig_title", doc.title);
		}
	}
}
