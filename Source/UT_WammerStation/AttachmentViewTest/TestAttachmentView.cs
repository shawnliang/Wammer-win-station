using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Specialized;
using System.IO;
using Wammer.Station;
using Wammer.Station.AttachmentView;
using Waveface.Stream.Model;

namespace UT_WammerStation.AttachmentViewTest
{
	/// <summary>
	/// Summary description for TestAttachmentView
	/// </summary>
	[TestClass]
	public class TestAttachmentView
	{
		Mock<IAttachmentViewHandlerDB> db;
		Mock<IAttachmentViewStorage> storage;
		AttachmentViewHandlerImp handler;

		Driver user = new Driver { folder = "folder1" };
		System.IO.MemoryStream m = new System.IO.MemoryStream();

		[TestInitialize]
		public void setUp()
		{
			db = new Mock<IAttachmentViewHandlerDB>(MockBehavior.Strict);
			storage = new Mock<IAttachmentViewStorage>(MockBehavior.Strict);
			handler = new AttachmentViewHandlerImp();
			handler.Storage = storage.Object;
			handler.DB = db.Object;
		}


		[TestMethod]
		public void GetOriginal_NoImageMetaSpecified()
		{
			db.Setup(x => x.GetAttachment("obj1")).
				Returns(new Attachment { object_id = "obj1", saved_file_name = @"2010\10\20\file1.jpg", group_id = "group1", mime_type = "mime" }).Verifiable();
			db.Setup(x => x.GetUserByGroupId("group1")).Returns(user).Verifiable();
			db.Setup(x => x.UpdateLastAccessTime("obj1")).Verifiable();

			storage.Setup(x => x.GetAttachmentStream(ImageMeta.Origin, user, @"2010\10\20\file1.jpg")).Returns(m).Verifiable();

			var result = handler.GetAttachmentStream(new NameValueCollection { { "object_id", "obj1" } });
			Assert.AreEqual(m, result.Stream);
			Assert.AreEqual("mime", result.MimeType);

			db.VerifyAll();
			storage.VerifyAll();
		}

		[TestMethod]
		public void GetOriginal_MetaIsOrigin()
		{
			db.Setup(x => x.GetAttachment("obj1")).
				Returns(new Attachment { object_id = "obj1", saved_file_name = @"2010\10\20\file1.jpg", group_id = "group1", mime_type = "mime" }).Verifiable();
			db.Setup(x => x.GetUserByGroupId("group1")).Returns(user).Verifiable();
			db.Setup(x => x.UpdateLastAccessTime("obj1")).Verifiable();

			storage.Setup(x => x.GetAttachmentStream(ImageMeta.Origin, user, @"2010\10\20\file1.jpg")).Returns(m).Verifiable();

			var result = handler.GetAttachmentStream(new NameValueCollection { { "object_id", "obj1" }, { "image_meta", "origin" } });
			Assert.AreEqual(m, result.Stream);
			Assert.AreEqual("mime", result.MimeType);

			db.VerifyAll();
			storage.VerifyAll();
		}

		[TestMethod]
		public void GetMedium()
		{
			db.Setup(x => x.GetAttachment("obj1")).
				Returns(new Attachment
					{
						object_id = "obj1",
						group_id = "group1",
						image_meta = new ImageProperty
						{
							medium = new ThumbnailInfo { saved_file_name = "obj1_medium.dat", mime_type = "mm" }
						}
					}).Verifiable();

			db.Setup(x => x.GetUserByGroupId("group1")).Returns(user).Verifiable();

			storage.Setup(x => x.GetAttachmentStream(ImageMeta.Medium, user, "obj1_medium.dat")).Returns(m).Verifiable();

			var result = handler.GetAttachmentStream(new NameValueCollection { { "object_id", "obj1" }, { "image_meta", "medium" } });
			Assert.AreEqual(m, result.Stream);
			Assert.AreEqual("mm", result.MimeType);
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void NoAttachmentDbRecord()
		{
			db.Setup(x => x.GetAttachment(It.IsAny<string>())).Returns((Attachment)null).Verifiable();

			handler.GetAttachmentStream(new NameValueCollection { { "object_id", "obj1" } });
		}

		[TestMethod]
		[ExpectedException(typeof(WammerStationException))]
		public void NoUser()
		{
			db.Setup(x => x.GetAttachment(It.IsAny<string>())).Returns(new Attachment { group_id = "123" });
			db.Setup(x => x.GetUserByGroupId("123")).Returns((Driver)null);

			handler.GetAttachmentStream(new NameValueCollection { { "object_id", "obj1" } });
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void DbHasNotOriginSaveFileName()
		{
			db.Setup(x => x.GetAttachment(It.IsAny<string>())).Returns(new Attachment { group_id = "123" });
			db.Setup(x => x.GetUserByGroupId("123")).Returns(user);

			handler.GetAttachmentStream(new NameValueCollection { { "object_id", "obj1" } });
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void DbHasNotMediumSaveFileName()
		{
			db.Setup(x => x.GetAttachment(It.IsAny<string>())).Returns(new Attachment { group_id = "123" });
			db.Setup(x => x.GetUserByGroupId("123")).Returns(user);

			handler.GetAttachmentStream(new NameValueCollection { { "object_id", "obj1" }, { "image_meta", "medium" } });
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void NoObjectId()
		{
			db.Setup(x => x.GetAttachment("obj1")).
				Returns(new Attachment { object_id = "obj1", saved_file_name = @"2010\10\20\file1.jpg", group_id = "group1" }).Verifiable();
			db.Setup(x => x.GetUserByGroupId("group1")).Returns(user).Verifiable();

			storage.Setup(x => x.GetAttachmentStream(ImageMeta.Origin, user, @"2010\10\20\file1.jpg")).Returns(m).Verifiable();

			handler.GetAttachmentStream(new NameValueCollection { });
		}
	}
}
