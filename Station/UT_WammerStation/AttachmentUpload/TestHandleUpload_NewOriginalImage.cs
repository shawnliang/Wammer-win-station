using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wammer.Station.AttachmentUpload;
using Wammer.Model;
using System.IO;
using Wammer.Cloud;

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

			Mock<IAttachmentUploadHandlerDB> db = new Mock<IAttachmentUploadHandlerDB>();
			db.Setup(x => x.InsertOrMergeToExistingDoc(It.IsAny<Attachment>())).Callback(
				(Attachment doc) => { savedAttachment = doc; }).Returns(UpsertResult.Insert).Verifiable();
			db.Setup(x => x.GetUserByGroupId(uploadData.group_id)).Returns(new Driver { folder = "" }).Verifiable();
			db.Setup(x => x.FindSession(uploadData.session_token, uploadData.api_key)).Returns(new LoginedSession()).Verifiable();

			AttachmentUploadHandlerImp handler = new AttachmentUploadHandlerImp(db.Object);

			handler.Process(uploadData);

			// verify db.SaveToDB is called
			db.VerifyAll();

			// verify saved data
			Assert.AreEqual(uploadData.file_name, savedAttachment.file_name);
			Assert.AreEqual(uploadData.mime_type, savedAttachment.mime_type);
			Assert.AreEqual(uploadData.title, savedAttachment.title);
			Assert.AreEqual(uploadData.description, savedAttachment.description);
			Assert.AreEqual(uploadData.group_id, savedAttachment.group_id);
			Assert.AreEqual(uploadData.type, savedAttachment.type);
			Assert.AreEqual(uploadData.raw_data.Count, savedAttachment.file_size);
			Assert.IsFalse(String.IsNullOrEmpty(savedAttachment.md5));
			Assert.AreEqual(DateTime.UtcNow.Year, savedAttachment.modify_time.Year);
			Assert.AreEqual(DateTime.UtcNow.Month, savedAttachment.modify_time.Month);
			Assert.AreEqual(DateTime.UtcNow.Day, savedAttachment.modify_time.Day);
			Assert.AreEqual("/v2/attachments/view/?object_id=" + savedAttachment.object_id, savedAttachment.url);

			Assert.AreEqual(1024, savedAttachment.image_meta.width);
			Assert.AreEqual(768, savedAttachment.image_meta.height);
		}

		[TestMethod]
		public void UploadNewOriginImg_AttachmentIsSavedToFile()
		{
			Attachment savedAttachment = null;

			Mock<IAttachmentUploadHandlerDB> db = new Mock<IAttachmentUploadHandlerDB>();
			db.Setup(x => x.InsertOrMergeToExistingDoc(It.IsAny<Attachment>())).Callback(
				(Attachment doc) => { savedAttachment = doc; }).Returns(UpsertResult.Insert).Verifiable();
			db.Setup(x => x.GetUserByGroupId(uploadData.group_id)).Returns(new Driver { folder = ""}).Verifiable();
			db.Setup(x => x.FindSession(uploadData.session_token, uploadData.api_key)).Returns(new LoginedSession()).Verifiable();

			AttachmentUploadHandlerImp handler = new AttachmentUploadHandlerImp(db.Object);

			handler.Process(uploadData);

			// verify all
			db.VerifyAll();

			// verify saved data
			Assert.AreEqual(savedAttachment.object_id + ".jpg", savedAttachment.saved_file_name);
			Wammer.Station.FileStorage storage = new Wammer.Station.FileStorage(new Driver { folder = "" });
			byte[] savedFile = File.ReadAllBytes(Path.Combine(storage.basePath, savedAttachment.saved_file_name));
			Assert.AreEqual(uploadData.raw_data.Count, savedFile.Length);
			for (int i=0;i<savedFile.Length; i++)
				Assert.AreEqual(uploadData.raw_data.Array[i], savedFile[i]);
		}

		[TestMethod]
		public void UploadNewOriginImg_ResponseIsReturned()
		{
			Attachment savedAttachment = null;

			Mock<IAttachmentUploadHandlerDB> db = new Mock<IAttachmentUploadHandlerDB>();
			db.Setup(x => x.InsertOrMergeToExistingDoc(It.IsAny<Attachment>())).Callback(
				(Attachment doc) => { savedAttachment = doc; }).Returns(UpsertResult.Insert).Verifiable();
			db.Setup(x => x.GetUserByGroupId(uploadData.group_id)).Returns(new Driver { folder = "" }).Verifiable();
			db.Setup(x => x.FindSession(uploadData.session_token, uploadData.api_key)).Returns(new LoginedSession()).Verifiable();

			AttachmentUploadHandlerImp handler = new AttachmentUploadHandlerImp(db.Object);

			ObjectUploadResponse res = handler.Process(uploadData);
			Assert.AreEqual(0, res.api_ret_code);
			Assert.AreEqual("Success", res.api_ret_message);
			Assert.AreEqual(savedAttachment.object_id, res.object_id);
			Assert.AreEqual(200, res.status);
			Assert.IsNotNull(res.timestamp);
		}

		[TestMethod]
		public void UploadNewOriginImg_MimeTypeDefaultsToOctectStream()
		{
			Attachment savedAttachment = null;

			Mock<IAttachmentUploadHandlerDB> db = new Mock<IAttachmentUploadHandlerDB>();
			db.Setup(x => x.InsertOrMergeToExistingDoc(It.IsAny<Attachment>())).Callback(
				(Attachment doc) => { savedAttachment = doc; }).Returns(UpsertResult.Insert).Verifiable();
			db.Setup(x => x.GetUserByGroupId(uploadData.group_id)).Returns(new Driver { folder = "" }).Verifiable();
			db.Setup(x => x.FindSession(uploadData.session_token, uploadData.api_key)).Returns(new LoginedSession()).Verifiable();

			AttachmentUploadHandlerImp handler = new AttachmentUploadHandlerImp(db.Object);


			uploadData.mime_type = null; // no mime_type 
			handler.Process(uploadData);

			// verify db.SaveToDB is called
			db.VerifyAll();

			// verify saved data
			Assert.AreEqual("application/octet-stream", savedAttachment.mime_type);
		}

		[TestMethod]
		public void UploadNewOriginImg_SavedFileNameHasNoSuffixIfInputFileNameHasNoSuffix()
		{
			Attachment savedAttachment = null;

			Mock<IAttachmentUploadHandlerDB> db = new Mock<IAttachmentUploadHandlerDB>();
			db.Setup(x => x.InsertOrMergeToExistingDoc(It.IsAny<Attachment>())).Callback(
				(Attachment doc) => { savedAttachment = doc; }).Returns(UpsertResult.Insert).Verifiable();
			db.Setup(x => x.GetUserByGroupId(uploadData.group_id)).Returns(new Driver { folder = "" }).Verifiable();
			db.Setup(x => x.FindSession(uploadData.session_token, uploadData.api_key)).Returns(new LoginedSession()).Verifiable();

			AttachmentUploadHandlerImp handler = new AttachmentUploadHandlerImp(db.Object);

			uploadData.file_name = "no_suffix";
			handler.Process(uploadData);

			// verify all
			db.VerifyAll();

			// verify saved data
			Assert.AreEqual(savedAttachment.object_id, savedAttachment.saved_file_name);
			Assert.AreEqual(uploadData.file_name, savedAttachment.file_name);
		}

		[TestMethod]
		public void UploadNewOriginImg_EventIsEmitted()
		{
			Attachment savedAttachment = null;

			Mock<IAttachmentUploadHandlerDB> db = new Mock<IAttachmentUploadHandlerDB>();
			db.Setup(x => x.InsertOrMergeToExistingDoc(It.IsAny<Attachment>())).Callback(
				(Attachment doc) => { savedAttachment = doc; }).Returns(UpsertResult.Insert).Verifiable();
			db.Setup(x => x.GetUserByGroupId(uploadData.group_id)).Returns(new Driver { folder = "" }).Verifiable();
			db.Setup(x => x.FindSession(uploadData.session_token, uploadData.api_key)).Returns(new LoginedSession()).Verifiable();

			AttachmentUploadHandlerImp handler = new AttachmentUploadHandlerImp(db.Object);
			
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
