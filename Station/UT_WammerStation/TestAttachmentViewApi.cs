using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Model;
using Wammer.Station;
using Wammer.Cloud;
using System.IO;

namespace UT_WammerStation
{

	class FackAttViewHandler: HttpHandler
	{
		protected override void HandleRequest()
		{
			using (BinaryWriter w = new BinaryWriter(Response.OutputStream))
			{
				w.Write(Encoding.UTF8.GetBytes("123456"));
			}
			Response.Close();
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	[TestClass]
	public class TestAttachmentViewApi
	{
		HttpServer server8080 = new HttpServer(8080);
		HttpServer server80 = new HttpServer(80);

		[TestInitialize]
		public void Setup()
		{
			CloudServer.BaseUrl = "http://127.0.0.1:8080/v2/";

			FakeCloudRemoteHandler.SavedParams = new System.Collections.Specialized.NameValueCollection();
			server8080.AddHandler("/v2/attachments/view/", new FakeCloudRemoteHandler());
			server8080.Start();

			server80.AddHandler("/v2/objects/view/DownloadAttachment", new FackAttViewHandler());
			server80.Start();
		}

		[TestCleanup]
		public void TearDown()
		{
			server8080.Stop();
			server80.Stop();
		}

		[TestMethod]
		public void TestGetAttachmentAndMetadata()
		{
			DownloadResult result = AttachmentApi.DownloadImageWithMetadata("objectid", "session_token", "api_key", ImageMeta.Origin, "stationId");

			// verify that these paramenters are sent to cloud
			Assert.AreEqual("objectid", FakeCloudRemoteHandler.SavedParams["object_id"]);
			Assert.AreEqual("session_token", FakeCloudRemoteHandler.SavedParams["session_token"]);
			Assert.AreEqual(null, FakeCloudRemoteHandler.SavedParams["image_meta"]);
			Assert.AreEqual("stationId", FakeCloudRemoteHandler.SavedParams["station_id"]);
			Assert.AreEqual("api_key", FakeCloudRemoteHandler.SavedParams["apikey"]);
			Assert.AreEqual("true", FakeCloudRemoteHandler.SavedParams["return_meta"]);

			// verify that metadata is retrieved
			Wammer.Station.JSONClass.AttachmentView actualMetadata = 
				fastJSON.JSON.Instance.ToObject<Wammer.Station.JSONClass.AttachmentView>(FakeCloudRemoteHandler.jsonString);
			Assert.AreEqual(result.Metadata.creator_id, actualMetadata.creator_id);
			Assert.AreEqual(result.Metadata.file_name, actualMetadata.file_name);
			Assert.AreEqual(result.Metadata.md5, actualMetadata.md5);
			Assert.AreEqual(result.Metadata.group_id, actualMetadata.group_id);
			Assert.AreEqual(result.Metadata.redirect_to, actualMetadata.redirect_to);
			

			// verify that image is received
			Assert.AreEqual("123456", Encoding.UTF8.GetString(result.Image));
		}
	}
}
