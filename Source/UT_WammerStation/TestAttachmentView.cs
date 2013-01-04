using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Wammer;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PerfMonitor;
using Wammer.Station;
using Waveface.Stream.Model;

namespace UT_WammerStation
{
	[TestClass]
	public class TestAttachmentView
	{
		static MongoServer mongodb;

		Guid object_id1;
		Guid object_id2;

		#region Private Method
		private static void TestTunnelToCloud(string argument, string requestMethod)
		{
			CloudServer.BaseUrl = "http://localhost/";

			using (HttpServer cloud = new HttpServer(80))
			using (HttpServer server = new HttpServer(8080))
			{
				cloud.AddHandler("/attachments/view", new FakeCloudRemoteHandler());
				cloud.AddHandler("/objects/view/DownloadAttachment", new FakeCloudRemoteAttachmentHandler());
				cloud.Start();
				cloud.TaskEnqueue += new EventHandler<TaskQueueEventArgs>(HttpRequestMonitor.Instance.OnTaskEnqueue);

				server.AddHandler("/objects/view", new AttachmentViewHandler("sid"));
				server.TaskEnqueue += new EventHandler<TaskQueueEventArgs>(HttpRequestMonitor.Instance.OnTaskEnqueue);
				server.Start();

				Boolean isGetRequest = requestMethod.Equals("GET", StringComparison.CurrentCultureIgnoreCase);

				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
					"http://localhost:8080/objects/view" + ((isGetRequest) ? argument : string.Empty));
				req.ContentType = "application/x-www-form-urlencoded";
				req.Method = requestMethod.ToLower();

				if (!isGetRequest)
					using (StreamWriter fs = new StreamWriter(req.GetRequestStream()))
					{
						fs.Write(argument);
					}

				HttpWebResponse response = (HttpWebResponse)req.GetResponse();
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Assert.AreEqual("image/jpeg", response.ContentType);

				using (BinaryReader reader = new BinaryReader(response.GetResponseStream()))
				{
					byte[] readData = reader.ReadBytes(1000);
					Assert.AreEqual(10, readData.Length);

					for (int i = 0; i < readData.Length; i++)
						Assert.AreEqual((byte)i + 1, readData[i]);
				}

				Assert.AreEqual("sid", FakeCloudRemoteHandler.SavedParams["station_id"]);
			}
		}
		#endregion

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			mongodb = MongoDB.Driver.MongoServer.Create("mongodb://localhost:10319/?safe=true");
		}

		[TestInitialize]
		public void setUp()
		{
			if (!Directory.Exists("resource"))
				Directory.CreateDirectory("resource");

			object_id1 = Guid.NewGuid();
			object_id2 = Guid.NewGuid();

			FileStream fs = File.Open("resource/" + object_id1.ToString() + ".png", FileMode.Create);
			using (StreamWriter w = new StreamWriter(fs))
			{
				w.Write("1234567890");
			}
			fs.Close();

			fs = File.Open("resource/" + object_id1.ToString() + "_medium.jpeg", FileMode.Create);
			using (StreamWriter w = new StreamWriter(fs))
			{
				w.Write("abcde");
			}
			fs.Close();

			MongoDatabase db = mongodb.GetDatabase("wammer");
			if (db.CollectionExists("attachments"))
				db.Drop();

			db.CreateCollection("attachments");

			db.GetCollection<Attachment>("attachments").Insert(new Attachment
			{
				object_id = object_id1.ToString(),
				group_id = "group1",
				mime_type = "image/png",
				file_size = 10,
				saved_file_name = object_id1 + ".png",
				image_meta = new ImageProperty
				{
					medium = new ThumbnailInfo
					{
						mime_type = "image/jpeg",
						file_size = 5
					}
				}
			});

			db.GetCollection<Attachment>("attachments").Insert(new Attachment
			{
				object_id = object_id2.ToString(),
				type = AttachmentType.image,
				image_meta = new ImageProperty
				{
					medium = new ThumbnailInfo
					{
						mime_type = "image/jpeg",
						file_size = 5
					}
				}
			});

			DriverCollection.Instance.Save(
				new Driver
				{
					email = "driver1@waveface.com",
					user_id = "driver1_id",
					folder = "",
					groups = new List<UserGroup>
					{
						new UserGroup {
							group_id = "e8e228bd-3e6a-417a-88fe-16a6a8fdf94e",
							description = "group1 descript",
							creator_id = "driver1_id",
							name = "group1"
						}
					}
				}
			);
		}

		[TestCleanup]
		public void tearDown()
		{
			System.Threading.Thread.Sleep(200);
			Directory.Delete("resource", true);

			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").RemoveAll();
		}

		[TestMethod]
		public void TestView_FileNotFound_ForwardToCloud()
		{
			TestTunnelToCloud("?object_id=abc&apikey=123&session_token=token123&image_meta=medium", "GET");
		}

		[TestMethod]
		public void AccessOriginPhotoFromSecondaryStationIsNotAllowed()
		{
			string requestMethod = "GET";
			string argument = "?object_id=abc&apikey=123&session_token=token123";

			CloudServer.BaseUrl = "http://localhost/";

			using (HttpServer cloud = new HttpServer(80))
			using (HttpServer server = new HttpServer(8080))
			{
				cloud.AddHandler("/attachments/view", new FakeCloudRemoteHandler());
				cloud.AddHandler("/objects/view/DownloadAttachment", new FakeCloudRemoteAttachmentHandler());
				cloud.TaskEnqueue += new EventHandler<TaskQueueEventArgs>(HttpRequestMonitor.Instance.OnTaskEnqueue);
				cloud.Start();

				server.AddHandler("/objects/view", new AttachmentViewHandler("sid"));
				server.TaskEnqueue += new EventHandler<TaskQueueEventArgs>(HttpRequestMonitor.Instance.OnTaskEnqueue);
				server.Start();

				Boolean isGetRequest = requestMethod.Equals("GET", StringComparison.CurrentCultureIgnoreCase);

				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
					"http://localhost:8080/objects/view" + ((isGetRequest) ? argument : string.Empty));
				req.ContentType = "application/x-www-form-urlencoded";
				req.Method = requestMethod.ToLower();

				try
				{
					HttpWebResponse response = (HttpWebResponse)req.GetResponse();
				}
				catch (WebException e)
				{
					WammerCloudException ce = new WammerCloudException("", e);
					Assert.AreEqual((int)StationLocalApiError.AccessDenied, ce.WammerError);
					return;
				}

				Assert.Fail("shoud not come here");
			}
		}
	}

	class FakeCloudRemoteHandler : HttpHandler
	{
		public static System.Collections.Specialized.NameValueCollection SavedParams;

		public const string jsonString = @"{
	""status"": 302,
	""session_token"": ""IPiyl7s8H85V0VBaI5PJTuAQ.KER6e9YNVYs8ypjsZ6+C8UFcVey375SytfvOH9X9SLA"",
	""session_expires"": ""2012-05-19T02:44:36Z"",
	""redirect_to"": ""http://localhost:80/objects/view/DownloadAttachment"",
	""api_ret_code"": 0,
	""api_ret_message"": ""success"",
	""debug"": {
		""connection_id"": ""133221147833549391""
	},
	""loc"": 0,
	""group_id"": ""e8e228bd-3e6a-417a-88fe-16a6a8fdf94e"",
	""meta_time"": 0,
	""description"": """",
	""title"": """",
	""file_name"": ""IMG_20120319_104743.jpg"",
	""meta_status"": ""OK"",
	""object_id"": ""98d5cac5-4387-48cc-b974-414a921a6144"",
	""creator_id"": ""driver1_id"",
	""image_meta"": {
		""small"": {
			""url"": ""/attachments/view?object_id=98d5cac5-4387-48cc-b974-414a921a6144&image_meta=small"",
			""height"": 120,
			""width"": 72,
			""modify_time"": 1332125268,
			""file_size"": 3002,
			""mime_type"": ""image/jpeg"",
			""md5"": ""332daefeef4a3b646dff4ded0285a16b""
		},
		""medium"": {
			""url"": ""/attachments/view?object_id=98d5cac5-4387-48cc-b974-414a921a6144&image_meta=medium"",
			""file_name"": ""IMG_20120319_104743.jpg"",
			""height"": 512,
			""width"": 307,
			""modify_time"": 1332125267,
			""file_size"": 96632,
			""mime_type"": ""image/jpeg"",
			""md5"": ""c96562d5a457d0234d5f2f4615c6514d""
		}
	},
	""default_post"": false,
	""modify_time"": 1332125267,
	""code_name"": ""Android"",
	""hidden"": ""false"",
	""type"": ""image"",
	""device_id"": ""7c80239a-b9a9-460a-9da8-d8001896f0f0""
}";

		public override void HandleRequest()
		{
			SavedParams = this.Parameters;

			Response.RedirectLocation = "http://localhost:80/objects/view/DownloadAttachment";

			Response.StatusCode = 200;
			Response.ContentType = "application/json";

			using (StreamWriter w = new StreamWriter(Response.OutputStream))
			{
				w.Write(jsonString);
			}
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}


	class FakeCloudRemoteAttachmentHandler : HttpHandler
	{
		public static System.Collections.Specialized.NameValueCollection SavedParams;

		public override void HandleRequest()
		{
			SavedParams = this.Parameters;

			RespondSuccess("image/jpeg", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}

}
