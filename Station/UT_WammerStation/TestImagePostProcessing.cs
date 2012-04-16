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
using Wammer.Model;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace UT_WammerStation
{
	class NullTask : ITask
	{
		public void Execute()
		{
		}
	}

	class NullUpstreamThumbnailTaskFactory: IUpstreamThumbnailTaskFactory
	{
		public ITask CreateTask(AttachmentEventArgs args, ImageMeta meta)
		{
			return new NullTask();
		}
	}

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

			CloudServer.BaseUrl = "http://localhost:8080/v2/";
			Wammer.Cloud.CloudServer.SessionToken = "thisIsASessionToken";
			DummyImageUploadHandler.evt.Reset();

			MongoDatabase db = mongodb.GetDatabase("wammer");
			db.Drop();

			if (mongodb.GetDatabase("wammer").CollectionExists("attachments"))
				mongodb.GetDatabase("wammer").DropCollection("attachments");

			object_id1 = Guid.NewGuid().ToString();
			AttachmentCollection.Instance.Save(new Attachment
			{
				object_id = object_id1,
				title = "orig_title",
				description = "orig_desc",
				type = AttachmentType.image,
				file_name = "file_name",
				saved_file_name = object_id1 + ".jpg",
				modify_time = DateTime.UtcNow,

				image_meta = new ImageProperty {
					small = new ThumbnailInfo {
								mime_type = "image/jpeg",
								url = "http://url/",
								file_size = 123,
								width = 1000,
								height = 2000
							},
					medium = new ThumbnailInfo {
								modify_time = DateTime.UtcNow.AddDays(-1.0)
							}
					
				}
			});

			List<UserGroup> groups = new List<UserGroup>();
			groups.Add(new UserGroup { group_id = "group1", description = "group1 descript", creator_id = "driver1_id", name = "group1" });
			DriverCollection.Instance.Save(
				new Driver
				{
					email = "driver1@waveface.com",
					user_id = "driver1_id",
					folder = @"resource\group1",
					groups = groups
				}
			);

			if (!Directory.Exists(@"resource\group1"))
				Directory.CreateDirectory(@"resource\group1");

			File.Copy("Penguins.jpg", Path.Combine(@"resource\group1", object_id1 + ".jpg"));
		}

		[TestCleanup]
		public void tearDown()
		{
			if (mongodb.GetDatabase("wammer").CollectionExists("attachments"))
				mongodb.GetDatabase("wammer").DropCollection("attachments");

            //if (Directory.Exists("resource"))
            //    Directory.Delete("resource", true);
		}

		[TestMethod]
		public void TestObjectUploadHandler_ResponseCompleted()
		{
			using (HttpServer cloud = new HttpServer(8080))
			using (HttpServer server = new HttpServer(80))
			{
				List<UserGroup> groups = new List<UserGroup>();
				groups.Add(new UserGroup { creator_id = "id1", group_id = "gid1", name = "group1", description = "none" });
				FileStorage fileStore = new FileStorage(new Driver { email = "driver1@waveface.com", folder = @"resource\group1", groups = groups, session_token = "session_token1", user_id = "id1" });
				AttachmentUploadHandler handler = new AttachmentUploadHandler();

				DummyRequestCompletedHandler evtHandler = new DummyRequestCompletedHandler();
				handler.ImageAttachmentCompleted += evtHandler.Handle;

				server.AddHandler("/test/", handler);
				server.Start();

				cloud.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/upload/",
				                                                    new DummyImageUploadHandler());
				cloud.Start();

				ObjectUploadResponse res = Wammer.Model.Attachment.UploadImage(
					"http://localhost:80/test/", new ArraySegment<byte>(imageRawData), "group1", null,
					"orig_name.jpeg", "image/jpeg", ImageMeta.Origin, "apiKey1", "token1");

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

				ObjectUploadResponse res = Wammer.Model.Attachment.UploadImage(
					"http://localhost:80/test/", new ArraySegment<byte>(imageRawData), "group1", null,
					"orig_name.jpeg", "image/jpeg", ImageMeta.Origin, "apikey1", "token1");

				Assert.AreEqual("token1", 
					DummyImageUploadHandler.recvParameters["session_token"]);
			}
		}

		[TestMethod]
		public void TestStationRecv_OldOriginalImage()
		{
			using (HttpServer cloud = new HttpServer(8080))
			using (HttpServer server = new HttpServer(80))
			{
				List<UserGroup> groups = new List<UserGroup>();
				groups.Add(new UserGroup { creator_id = "id1", group_id = "gid1", name = "group1", description = "none" });
				FileStorage fileStore = new FileStorage(new Driver { email = "driver1@waveface.com", folder = @"resource\group1", groups = groups, session_token = "session_token1", user_id = "id1" });
				AttachmentUploadHandler handler = new AttachmentUploadHandler();
				server.AddHandler("/test/", handler);
				server.Start();
				cloud.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/upload/",
		                                                            new DummyImageUploadHandler());
				cloud.Start();

				ObjectUploadResponse res = Wammer.Model.Attachment.UploadImage(
					"http://localhost:80/test/", new ArraySegment<byte>(imageRawData), "group1", object_id1, 
					"orig_name2.png", "image/png", ImageMeta.Origin, "apikey1", "token1");

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
			using (HttpServer cloud = new HttpServer(8080))
			using (HttpServer server = new HttpServer(80))
			{
				List<UserGroup> groups = new List<UserGroup>();
				groups.Add(new UserGroup { creator_id = "id1", group_id = "gid1", name = "group1", description = "none" });
				FileStorage fileStore = new FileStorage(new Driver { email = "driver1@waveface.com", folder = @"resource\group1", groups = groups, session_token = "session_token1", user_id = "id1" });
				AttachmentUploadHandler handler = new AttachmentUploadHandler();
				server.AddHandler("/test/", handler);
				server.Start();
				cloud.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/upload/",
																	new DummyImageUploadHandler());
				cloud.Start();

				string oid = Guid.NewGuid().ToString();
				ObjectUploadResponse res = Wammer.Model.Attachment.UploadImage(
					"http://localhost:80/test/", new ArraySegment<byte>(imageRawData), "group1", oid, 
					"orig_name2.jpeg", "image/jpeg", ImageMeta.Large, "apikey1", "token1");

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
					string.Format("/v2/attachments/view/?object_id={0}&image_meta=large",
																					res.object_id),
					meta["large"].AsBsonDocument["url"].AsString);

				Assert.AreEqual(res.object_id + "_large.jpeg",
					meta["large"].AsBsonDocument["file_name"]);
				Assert.AreEqual(imageRawData.Length, meta["large"].AsBsonDocument["file_size"].ToInt32());
				Assert.AreEqual("image/jpeg", meta["large"].AsBsonDocument["mime_type"].AsString);
			}
		}

		[TestMethod]
		public void TestUpdateMediumThumbnailIfItsModifyTimeIsOlder()
		{
			ImageAttachmentEventArgs args = new ImageAttachmentEventArgs(
				object_id1, "driver1_id", "key1", "token1", ImageMeta.Origin);
			
			ImagePostProcessing post = new ImagePostProcessing(new NullUpstreamThumbnailTaskFactory());
			post.HandleImageAttachmentSaved(this, args);

			Attachment doc = AttachmentCollection.Instance.FindOne(Query.EQ("_id", object_id1));
				
			Assert.AreEqual(1024, doc.image_meta.width);
			Assert.AreEqual(768, doc.image_meta.height);
			Assert.AreEqual("orig_title", doc.title);

			Assert.AreEqual("/v2/attachments/view/?object_id=" + object_id1 + "&image_meta=medium",
				doc.image_meta.medium.url);
			Assert.AreEqual(512, doc.image_meta.medium.width);
			Assert.AreEqual(384, doc.image_meta.medium.height);
			Assert.AreEqual("image/jpeg", doc.image_meta.medium.mime_type);
			Assert.AreEqual(object_id1 + "_medium.dat", doc.image_meta.medium.saved_file_name);
			Assert.AreEqual(doc.file_name, doc.image_meta.medium.file_name);
			Assert.IsTrue(doc.image_meta.medium.file_size > 0);
			Assert.IsTrue(doc.image_meta.medium.modify_time > doc.modify_time);
		}

		[TestMethod]
		public void TestGenerateSmallLargeAndSquareThumbnails()
		{
			ImageAttachmentEventArgs args = new ImageAttachmentEventArgs(
				object_id1, "driver1_id", "key1", "token1", ImageMeta.Origin);

			MakeAllThumbnailsAndUpstreamTask task = new MakeAllThumbnailsAndUpstreamTask(
				args, new NullUpstreamThumbnailTaskFactory());
			task.Execute();

			Attachment doc = AttachmentCollection.Instance.FindOne(Query.EQ("_id", object_id1));

			Assert.IsNotNull(doc.image_meta.small);
			Assert.IsNotNull(doc.image_meta.large);
			Assert.IsNotNull(doc.image_meta.square);

			Assert.AreEqual("/v2/attachments/view/?object_id=" + object_id1 +
				"&image_meta=small",
				doc.image_meta.small.url);
			Assert.AreEqual(120, doc.image_meta.small.width);
			Assert.AreEqual(90, doc.image_meta.small.height);
			Assert.AreEqual("image/jpeg", doc.image_meta.small.mime_type);
			Assert.AreEqual(object_id1 + "_small.dat", doc.image_meta.small.saved_file_name);
			Assert.IsTrue(doc.image_meta.small.file_size > 0);
			Assert.IsTrue(doc.image_meta.small.modify_time - DateTime.UtcNow < TimeSpan.FromSeconds(10));

			Assert.AreEqual("/v2/attachments/view/?object_id=" + object_id1 +
				"&image_meta=large",
				doc.image_meta.large.url);
			Assert.AreEqual(1024, doc.image_meta.large.width);
			Assert.AreEqual(768, doc.image_meta.large.height);
			Assert.AreEqual("image/jpeg", doc.image_meta.large.mime_type);
			Assert.AreEqual(object_id1 + "_large.dat", doc.image_meta.large.saved_file_name);
			Assert.IsTrue(doc.image_meta.large.file_size > 0);
			Assert.IsTrue(doc.image_meta.large.modify_time - DateTime.UtcNow < TimeSpan.FromSeconds(10));

			Assert.AreEqual("/v2/attachments/view/?object_id=" + object_id1 +
				"&image_meta=square",
				doc.image_meta.square.url);
			Assert.AreEqual(128, doc.image_meta.square.width);
			Assert.AreEqual(128, doc.image_meta.square.height);
			Assert.AreEqual("image/jpeg", doc.image_meta.square.mime_type);
			Assert.AreEqual(object_id1 + "_square.dat", doc.image_meta.square.saved_file_name);
			Assert.IsTrue(doc.image_meta.square.file_size > 0);
			Assert.IsTrue(doc.image_meta.square.modify_time - DateTime.UtcNow < TimeSpan.FromSeconds(10));
		}
	}
}
