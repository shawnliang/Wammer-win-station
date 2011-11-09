using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wammer.Station;
using Wammer.Cloud;
using MongoDB.Bson;
using MongoDB.Driver;

namespace UT_WammerStation
{
	[TestClass]
	public class TestViewFile
	{
		static MongoServer mongodb;
		static FileStorage storage;

		Guid object_id1;
		Guid object_id2;

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			storage = new FileStorage("resource");
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
				mime_type = "image/png",
				file_size = 10,
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
						file_size = 10
					}
				}
			});
		}

		[TestCleanup]
		public void tearDown()
		{
			Directory.Delete("resource", true);
		}

		[TestMethod]
		public void TestView_ByPOST()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/v1/objects/view", new ViewObjectHandler(
					storage, mongodb));
				server.Start();

				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
											"http://localhost/v1/objects/view");
				req.ContentType = "application/x-www-form-urlencoded";
				req.Method = "POST";
				using (StreamWriter fs = new StreamWriter(req.GetRequestStream()))
				{
					fs.Write("object_id="+object_id1.ToString());
				}

				HttpWebResponse response = (HttpWebResponse)req.GetResponse();
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Assert.AreEqual("image/png", response.ContentType);

				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					string responseText = reader.ReadToEnd();
					Assert.AreEqual("1234567890", responseText);
				}
			}
		}

		[TestMethod]
		public void TestView_ByGET()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/v1/objects/view", new ViewObjectHandler(storage, mongodb));
				server.Start();

				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
										"http://localhost/v1/objects/view" +
										"?object_id=" + object_id1.ToString() + "&image_meta=origin");
				req.Method = "GET";

				HttpWebResponse response = (HttpWebResponse)req.GetResponse();
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Assert.AreEqual("image/png", response.ContentType);

				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					string responseText = reader.ReadToEnd();
					Assert.AreEqual("1234567890", responseText);
				}
			}
		}

		[TestMethod]
		public void TestView_ViewNotExistOrigImage()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/v1/objects/view", new ViewObjectHandler(storage, mongodb));
				server.Start();

				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
										"http://localhost/v1/objects/view" +
										"?object_id=object_id_na&image_meta=origin");
				req.Method = "GET";

				try
				{
					HttpWebResponse response = (HttpWebResponse)req.GetResponse();
				}
				catch (WebException e)
				{
					Assert.AreEqual(HttpStatusCode.NotFound, ((HttpWebResponse)e.Response).StatusCode);
					return;
				}

				Assert.Fail("expected exception is not thrown");
			}
		}

		[TestMethod]
		public void TestView_ViewThumbnail()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/v1/objects/view", new ViewObjectHandler(storage, mongodb));
				server.Start();

				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
										"http://localhost/v1/objects/view" +
										"?object_id=" + object_id1.ToString() + "&image_meta=medium");
				req.Method = "GET";

				HttpWebResponse response = (HttpWebResponse)req.GetResponse();
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Assert.AreEqual("image/jpeg", response.ContentType);

				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					string responseText = reader.ReadToEnd();
					Assert.AreEqual("abcde", responseText);
				}
			}
		}

		[TestMethod]
		public void TestView_NoObjectID()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/v1/objects/view", new ViewObjectHandler(storage, mongodb));
				server.Start();

				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
										"http://localhost/v1/objects/view");
				req.Method = "GET";

				try
				{
					req.GetResponse();
				}
				catch (WebException e)
				{
					Assert.AreEqual(WebExceptionStatus.ProtocolError, e.Status);
					HttpWebResponse response = (HttpWebResponse) e.Response;
					Assert.AreEqual("application/json", response.ContentType);

					using (StreamReader reader = new StreamReader(response.GetResponseStream()))
					{
						string responseText = reader.ReadToEnd();

						CloudResponse res = fastJSON.JSON.Instance.ToObject<CloudResponse>(responseText);
						Assert.AreEqual(400, res.status);
						Assert.AreEqual(-1, res.api_ret_code);
						Assert.AreEqual("missing required param: object_id", res.api_ret_msg);
					}

					return;
				}

				Assert.Fail("Expected failure is not thrown");
			}
		}

		[TestMethod]
		public void TestView_NoImageMetaMeansOriginal()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/v1/objects/view", new ViewObjectHandler(storage, mongodb));
				server.Start();

				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
										"http://localhost/v1/objects/view" +
										"?object_id=" + object_id1.ToString());
				req.Method = "GET";

				HttpWebResponse response = (HttpWebResponse)req.GetResponse();
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Assert.AreEqual("image/png", response.ContentType);

				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					string responseText = reader.ReadToEnd();
					Assert.AreEqual("1234567890", responseText);
				}
			}
		}

		[TestMethod]
		public void TestView_FileNotFound()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/v1/objects/view", new ViewObjectHandler(storage, mongodb));
				server.Start();

				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
										"http://localhost/v1/objects/view" +
										"?object_id=abc");
				req.Method = "GET";

				try
				{
					req.GetResponse();
				}
				catch (WebException e)
				{
					Assert.AreEqual(WebExceptionStatus.ProtocolError, e.Status);
					HttpWebResponse response = (HttpWebResponse)e.Response;
					Assert.AreEqual("application/json", response.ContentType);

					using (StreamReader reader = new StreamReader(response.GetResponseStream()))
					{
						string responseText = reader.ReadToEnd();

						CloudResponse res = fastJSON.JSON.Instance.ToObject<CloudResponse>(responseText);
						Assert.AreEqual(404, res.status);
						Assert.AreEqual(-1, res.api_ret_code);
						Assert.AreEqual("object abc is not found", res.api_ret_msg);
					}

					return;
				}

				Assert.Fail("Expected failure is not thrown");
			}
		}

	}
}
