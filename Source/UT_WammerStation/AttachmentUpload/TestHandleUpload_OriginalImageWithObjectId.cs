using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using Wammer.Station.AttachmentUpload;
using Waveface.Stream.Model;

namespace UT_WammerStation.AttachmentUpload
{
	[TestClass]
	public class TestHandleUpload_OriginalImageWithObjectId
	{
		UploadData uploadData;

		[TestInitialize]
		public void SetUp()
		{
			uploadData = new UploadData
			{
				object_id = "object1",
				raw_data = new ArraySegment<byte>(File.ReadAllBytes(@"Penguins.jpg")),
				file_name = "filename.jpg",
				mime_type = "image/jpeg",
				title = "title",
				description = "description",
				group_id = "group1",
				type = AttachmentType.image,
				imageMeta = ImageMeta.Origin
			};
		}

		[TestMethod]
		public void TestUploadAnAttachmentWithObjectId()
		{
			Attachment savedAttachment = null;

			Mock<IAttachmentUploadStorage> storage = new Mock<IAttachmentUploadStorage>(MockBehavior.Strict);
			storage.Setup(x => x.Save(uploadData, It.IsAny<string>())).Returns(new AttachmentSaveResult("", @"2001\10\20\filename.jpg")).Verifiable();

			Mock<IAttachmentUploadHandlerDB> db = new Mock<IAttachmentUploadHandlerDB>(MockBehavior.Strict);
			//db.Setup(x => x.GetUserByGroupId(uploadData.group_id)).Returns(new Driver { folder = "" }).Verifiable();
			db.Setup(x => x.InsertOrMergeToExistingDoc(It.IsAny<Attachment>())).Callback((Attachment a) => { savedAttachment = a; }).Returns(UpsertResult.Update).Verifiable();
			db.Setup(x => x.FindSession(uploadData.session_token, uploadData.api_key)).Returns(new LoginedSession()).Verifiable();
			AttachmentUploadHandlerImp handler = new AttachmentUploadHandlerImp(db.Object, storage.Object);

			handler.Process(uploadData);

			db.VerifyAll();
			storage.VerifyAll();
			//Assert.AreEqual(savedAttachment.object_id, res.object_id);

			Assert.AreEqual(uploadData.file_name, savedAttachment.file_name);
			Assert.AreEqual(uploadData.mime_type, savedAttachment.mime_type);
			Assert.AreEqual(uploadData.title, savedAttachment.title);
			Assert.AreEqual(uploadData.description, savedAttachment.description);
			Assert.AreEqual(uploadData.group_id, savedAttachment.group_id);
			Assert.AreEqual(uploadData.type, savedAttachment.type);

			Assert.AreEqual(@"2001\10\20\filename.jpg", savedAttachment.saved_file_name);
		}
	}
}
