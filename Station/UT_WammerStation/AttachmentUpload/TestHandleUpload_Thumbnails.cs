using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station.AttachmentUpload;
using System.IO;
using Wammer.Model;
using Moq;
using Wammer.Station;

namespace UT_WammerStation.AttachmentUpload
{
	[TestClass]
	public class TestHandleUpload_Thumbnails
	{
		UploadData uploadData;

		[TestInitialize]
		public void SetUp()
		{
			uploadData = new UploadData
			{
				object_id = null,
				raw_data = new ArraySegment<byte>(File.ReadAllBytes(@"Penguins.jpg")),
				file_name = "filename.jpg",
				mime_type = "image/jpeg",
				title = "title",
				description = "description",
				group_id = "group1",
				type = AttachmentType.image,
				imageMeta = ImageMeta.Medium
			};
		}

		[TestMethod]
		public void ThumbnailsInfoIsWrittenToThumbailField()
		{
			Attachment savedAttachment = null;

			Mock<IAttachmentUploadStorage> storage = new Mock<IAttachmentUploadStorage>(MockBehavior.Strict);
			storage.Setup(x => x.Save(uploadData)).Returns(new AttachmentSaveResult("", "123.jpg")).Verifiable();

			Mock<IAttachmentUploadHandlerDB> db = new Mock<IAttachmentUploadHandlerDB>();
			db.Setup(x => x.InsertOrMergeToExistingDoc(It.IsAny<Attachment>())).Callback(
				(Attachment doc) => { savedAttachment = doc; }).Returns(UpsertResult.Insert).Verifiable();
			//db.Setup(x => x.GetUserByGroupId(uploadData.group_id)).Returns(new Driver { folder = "" }).Verifiable();
			db.Setup(x => x.FindSession(uploadData.session_token, uploadData.api_key)).Returns(new LoginedSession()).Verifiable();

			AttachmentUploadHandlerImp handler = new AttachmentUploadHandlerImp(db.Object, storage.Object);

			handler.Process(uploadData);

			// verify db.SaveToDB is called
			db.VerifyAll();
			storage.VerifyAll();

			Assert.AreEqual(uploadData.file_name, savedAttachment.file_name);
			Assert.AreEqual(uploadData.title, savedAttachment.title);
			Assert.AreEqual(uploadData.description, savedAttachment.description);
			Assert.AreEqual(uploadData.group_id, savedAttachment.group_id);
			Assert.AreEqual(uploadData.type, savedAttachment.type);
			Assert.IsNotNull(savedAttachment.image_meta);
			Assert.IsNotNull(savedAttachment.image_meta.medium);
			Assert.AreEqual(uploadData.raw_data.Count, savedAttachment.image_meta.medium.file_size);
			Assert.IsFalse(string.IsNullOrEmpty(savedAttachment.image_meta.medium.md5));
			Assert.AreEqual(uploadData.mime_type, savedAttachment.image_meta.medium.mime_type);
			Assert.AreEqual(1024, savedAttachment.image_meta.medium.width);
			Assert.AreEqual(768, savedAttachment.image_meta.medium.height);
			Assert.AreEqual(
				"/v2/attachments/view/?object_id=" + savedAttachment.object_id + "&image_meta=" + uploadData.imageMeta.ToString().ToLower(),
				savedAttachment.image_meta.medium.url);
			Assert.AreEqual("123.jpg", savedAttachment.image_meta.medium.saved_file_name);
		}
	}
}
