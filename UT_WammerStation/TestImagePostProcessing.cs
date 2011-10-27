using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Drawing;
using System.Net;

using Wammer.Station;
using Wammer.Cloud;

namespace UT_WammerStation
{
	class DummyImageUploadHandler : HttpHandler
	{
		public static List<UploadedFile> recvFiles;
		public static NameValueCollection recvParameters;

		protected override void HandleRequest()
		{
			recvFiles = this.Files;
			recvParameters= this.Parameters;

			HttpHelper.RespondSuccess(Response, 
				ObjectUploadResponse.CreateSuccess(recvParameters["object_id"]));

		}

		public override object Clone()
		{
			return new DummyImageUploadHandler();
		}
	}

	[TestClass]
	public class TestImagePostProcessing
	{
		byte[] imageRawData;

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
		}

		[TestMethod]
		public void TestStationRecvOriginalImage()
		{
			using (HttpServer cloud = new HttpServer(8080))
			using (HttpServer server = new HttpServer(80))
			{
				FileStorage fileStore = new FileStorage("resource");
				ImagePostProcessing postProc = new ImagePostProcessing(fileStore);
				ObjectUploadHandler handler = new ObjectUploadHandler(fileStore);

				handler.AttachmentSaved += postProc.HandleAttachmentSaved;

				server.AddHandler("/test/", handler);
				server.Start();

				cloud.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/upload/", 
																	new DummyImageUploadHandler());
				cloud.Start();

				ObjectUploadResponse res = Wammer.Cloud.Attachment.UploadImage(
														"http://localhost:80/test/", imageRawData,
												"orig_name.jpeg", "image/jpeg",ImageMeta.Original);

				// verify
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
	}
}
