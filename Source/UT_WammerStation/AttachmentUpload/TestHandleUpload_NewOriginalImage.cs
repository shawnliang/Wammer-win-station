using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using Wammer.Station.AttachmentUpload;
using Waveface.Stream.Model;

namespace UT_WammerStation.AttachmentUpload
{
	[TestClass]
	public class TestHandleUpload_NewOriginalImage
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
				imageMeta = ImageMeta.Origin,
				api_key = "api_key",
				session_token = "session"
			};
		}

		[TestMethod]
		public void UploadNewOriginImg_AttachmentInfoIsSavedToDB()
		{
			Attachment savedAttachment = null;

			Mock<IAttachmentUploadStorage> storage = new Mock<IAttachmentUploadStorage>(MockBehavior.Strict);
			storage.Setup(x => x.Save(uploadData, It.IsAny<string>())).Returns(new AttachmentSaveResult("", @"2001\10\20\filename.jpg")).Verifiable();

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

			// verify saved data
			Assert.AreEqual(uploadData.file_name, savedAttachment.file_name);
			Assert.AreEqual(uploadData.mime_type, savedAttachment.mime_type);
			Assert.AreEqual(uploadData.title, savedAttachment.title);
			Assert.AreEqual(uploadData.description, savedAttachment.description);
			Assert.AreEqual(uploadData.group_id, savedAttachment.group_id);
			Assert.AreEqual(uploadData.type, savedAttachment.type);
			Assert.AreEqual(uploadData.raw_data.Count, savedAttachment.file_size);
			Assert.IsFalse(String.IsNullOrEmpty(savedAttachment.MD5));
			Assert.AreEqual(DateTime.UtcNow.Year, savedAttachment.modify_time.Year);
			Assert.AreEqual(DateTime.UtcNow.Month, savedAttachment.modify_time.Month);
			Assert.AreEqual(DateTime.UtcNow.Day, savedAttachment.modify_time.Day);
			Assert.AreEqual("/v3/attachments/view/?object_id=" + savedAttachment.object_id, savedAttachment.url);

			Assert.AreEqual(1024, savedAttachment.image_meta.width);
			Assert.AreEqual(768, savedAttachment.image_meta.height);
		}



		//[TestMethod]
		//public void UploadNewOriginImg_ResponseIsReturned()
		//{
		//    Attachment savedAttachment = null;

		//    Mock<IAttachmentUploadHandlerDB> db = new Mock<IAttachmentUploadHandlerDB>();
		//    db.Setup(x => x.InsertOrMergeToExistingDoc(It.IsAny<Attachment>())).Callback(
		//        (Attachment doc) => { savedAttachment = doc; }).Returns(UpsertResult.Insert).Verifiable();
		//    db.Setup(x => x.GetUserByGroupId(uploadData.group_id)).Returns(new Driver { folder = "" }).Verifiable();
		//    db.Setup(x => x.FindSession(uploadData.session_token, uploadData.api_key)).Returns(new LoginedSession()).Verifiable();

		//    AttachmentUploadHandlerImp handler = new AttachmentUploadHandlerImp(db.Object);

		//    ObjectUploadResponse res = handler.Process(uploadData);
		//    Assert.AreEqual(0, res.api_ret_code);
		//    Assert.AreEqual("Success", res.api_ret_message);
		//    Assert.AreEqual(savedAttachment.object_id, res.object_id);
		//    Assert.AreEqual(200, res.status);
		//    Assert.IsNotNull(res.timestamp);
		//}

		[TestMethod]
		public void UploadNewOriginImg_MimeTypeDefaultsToOctectStream()
		{
			Attachment savedAttachment = null;

			Mock<IAttachmentUploadStorage> storage = new Mock<IAttachmentUploadStorage>(MockBehavior.Strict);
			storage.Setup(x => x.Save(uploadData, It.IsAny<string>())).Returns(new AttachmentSaveResult("", @"2001\10\20\filename.jpg")).Verifiable();

			Mock<IAttachmentUploadHandlerDB> db = new Mock<IAttachmentUploadHandlerDB>();
			db.Setup(x => x.InsertOrMergeToExistingDoc(It.IsAny<Attachment>())).Callback(
				(Attachment doc) => { savedAttachment = doc; }).Returns(UpsertResult.Insert).Verifiable();
			//db.Setup(x => x.GetUserByGroupId(uploadData.group_id)).Returns(new Driver { folder = "" }).Verifiable();
			db.Setup(x => x.FindSession(uploadData.session_token, uploadData.api_key)).Returns(new LoginedSession()).Verifiable();

			AttachmentUploadHandlerImp handler = new AttachmentUploadHandlerImp(db.Object, storage.Object);


			uploadData.mime_type = null; // no mime_type 
			handler.Process(uploadData);

			// verify db.SaveToDB is called
			db.VerifyAll();

			// verify saved data
			Assert.AreEqual("application/octet-stream", savedAttachment.mime_type);
		}


		[TestMethod]
		public void UploadNewOriginImg_EventIsEmitted()
		{
			Attachment savedAttachment = null;

			Mock<IAttachmentUploadStorage> storage = new Mock<IAttachmentUploadStorage>(MockBehavior.Strict);
			storage.Setup(x => x.Save(uploadData, It.IsAny<string>())).Returns(new AttachmentSaveResult("", @"2001\10\20\filename.jpg")).Verifiable();

			Mock<IAttachmentUploadHandlerDB> db = new Mock<IAttachmentUploadHandlerDB>();
			db.Setup(x => x.InsertOrMergeToExistingDoc(It.IsAny<Attachment>())).Callback(
				(Attachment doc) => { savedAttachment = doc; }).Returns(UpsertResult.Insert).Verifiable();
			//db.Setup(x => x.GetUserByGroupId(uploadData.group_id)).Returns(new Driver { folder = "" }).Verifiable();
			db.Setup(x => x.FindSession(uploadData.session_token, uploadData.api_key)).Returns(new LoginedSession()).Verifiable();

			AttachmentUploadHandlerImp handler = new AttachmentUploadHandlerImp(db.Object, storage.Object);

			AttachmentEventArgs arg = null;
			handler.AttachmentProcessed += ((sender, evtArg) => { arg = evtArg; });
			handler.Process(uploadData);

			// verify all
			db.VerifyAll();

			Assert.AreEqual(savedAttachment.object_id, arg.AttachmentId);
			Assert.AreEqual(ImageMeta.Origin, arg.ImgMeta);
			Assert.IsTrue(arg.IsFromThisWindows);
			Assert.AreEqual(UpsertResult.Insert, arg.UpsertResult);
		}

		void handler_AttachmentProcessed(object sender, AttachmentEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
