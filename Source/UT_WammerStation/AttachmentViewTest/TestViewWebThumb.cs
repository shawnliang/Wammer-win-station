using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wammer.Station.AttachmentView;
using Waveface.Stream.Model;
using System.Collections.Specialized;
using System.IO;

namespace UT_WammerStation.AttachmentViewTest
{
	[TestClass]
	public class TestViewWebThumb
	{
		Mock<IAttachmentViewHandlerDB> db;
		Mock<IAttachmentViewStorage> storage;
		AttachmentViewHandlerImp handler;

		Driver user = new Driver { folder = "folder1" };
		System.IO.MemoryStream m = new System.IO.MemoryStream();

		[TestInitialize]
		public void setUp()
		{
			AttachmentCollection.Instance.RemoveAll();

			db = new Mock<IAttachmentViewHandlerDB>(MockBehavior.Strict);
			storage = new Mock<IAttachmentViewStorage>(MockBehavior.Strict);
			handler = new AttachmentViewHandlerImp();
			handler.Storage = storage.Object;
			handler.DB = db.Object;
		}


		[TestMethod]
		public void GetViewWebThumb()
		{
			var dbDoc = new Attachment
			{
				type = AttachmentType.webthumb,
				web_meta = new WebProperty
				{
					thumbs = new List<WebThumb> {
						new WebThumb{ id = 0, saved_file_name = "file0.dat"},
						new WebThumb{ id = 1, saved_file_name = "file1.dat"},
						new WebThumb{ id = 2, saved_file_name = "file2.dat"},
					}
				},
				 group_id = "group1"
			};

			db.Setup(x => x.GetAttachment("obj1")).
				Returns(dbDoc).Verifiable();
			db.Setup(x => x.GetUserByGroupId("group1")).Returns(user).Verifiable();

			storage.Setup(x => x.GetAttachmentStream(ImageMeta.None, user, dbDoc.web_meta.thumbs[1].saved_file_name)).Returns(m).Verifiable();

			var result = handler.GetAttachmentStream(new NameValueCollection { { "object_id", "obj1" }, { "target", "preview" }, { "id", "1"} });

			Assert.AreEqual(m, result.Stream);
			Assert.AreEqual("image/jpeg", result.MimeType);

			db.VerifyAll();
			storage.VerifyAll();
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void GetPreview_DBRecordNotExist()
		{
			db.Setup(x => x.GetAttachment("obj1")).Returns(null as Attachment);
			db.Setup(x => x.GetUserByGroupId("group1")).Returns(user);

			var result = handler.GetAttachmentStream(new NameValueCollection { { "object_id", "obj1" }, { "target", "preview" }, { "id", "1" } });
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void GetPreview_UnableToGetStream()
		{
			var dbDoc = new Attachment
			{
				type = AttachmentType.webthumb,
				web_meta = new WebProperty
				{
					thumbs = new List<WebThumb> {
						new WebThumb{ id = 0, saved_file_name = "file0.dat"},
						new WebThumb{ id = 1, saved_file_name = "file1.dat"},
						new WebThumb{ id = 2, saved_file_name = "file2.dat"},
					}
				},
				group_id = "group1"
			};

			db.Setup(x => x.GetAttachment("obj1")).Returns(dbDoc);
			db.Setup(x => x.GetUserByGroupId("group1")).Returns(user);

			storage.Setup(x => x.GetAttachmentStream(ImageMeta.None, user, dbDoc.web_meta.thumbs[1].saved_file_name)).Throws(new FileNotFoundException());

			var result = handler.GetAttachmentStream(new NameValueCollection { { "object_id", "obj1" }, { "target", "preview" }, { "id", "1" } });
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void GetPreview_NoSavedFileName()
		{
			var dbDoc = new Attachment
			{
				type = AttachmentType.webthumb,
				web_meta = new WebProperty
				{
					thumbs = new List<WebThumb> {
						new WebThumb{ id = 0, saved_file_name = "file0.dat"},
						new WebThumb{ id = 1, saved_file_name = null },
						new WebThumb{ id = 2, saved_file_name = "file2.dat"},
					}
				},
				group_id = "group1"
			};

			db.Setup(x => x.GetAttachment("obj1")).Returns(dbDoc);
			db.Setup(x => x.GetUserByGroupId("group1")).Returns(user);

			var result = handler.GetAttachmentStream(new NameValueCollection { { "object_id", "obj1" }, { "target", "preview" }, { "id", "1" } });
		}

		[TestMethod]
		[ExpectedException(typeof(Wammer.Station.WammerStationException))]
		public void noPreviewBecauseCloudCannotGenerateIt()
		{
			var dbDoc = new Attachment
			{
				type = AttachmentType.webthumb,
				web_meta = new WebProperty
				{
					thumbs = new List<WebThumb> {
						new WebThumb{ id = 0, saved_file_name = "file0.dat"},
						new WebThumb{ id = 1, saved_file_name = null, broken_thumb = true },
						new WebThumb{ id = 2, saved_file_name = "file2.dat"},
					}
				},
				group_id = "group1"
			};

			db.Setup(x => x.GetAttachment("obj1")).Returns(dbDoc);
			db.Setup(x => x.GetUserByGroupId("group1")).Returns(user);

			var result = handler.GetAttachmentStream(new NameValueCollection { { "object_id", "obj1" }, { "target", "preview" }, { "id", "1" } });
		}
	}
}
