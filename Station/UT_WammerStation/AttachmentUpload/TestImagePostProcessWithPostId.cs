using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station.AttachmentUpload;
using Moq;
using Wammer.Model;

namespace UT_WammerStation.AttachmentUpload
{

	/// <summary>
	/// Summary description for TestImagePostProcessWithPostId
	/// </summary>
	[TestClass]
	public class TestImagePostProcessWithPostId
	{
		Attachment oldAtt;
		Driver user;
		ThumbnailInfo thumb;

		[TestInitialize]
		public void SetUp()
		{
			oldAtt = new Attachment
			{
				object_id = "object1",
				file_name = "filename",
				description = "desc",
				group_id = "group1",
				saved_file_name = "save_file_name",
				title = "title",

				image_meta = new ImageProperty
				{
					medium = new ThumbnailInfo
					{
						saved_file_name = "saved_medium",
						mime_type = "medium_mime"
					}
				}
			};

			user = new Driver
			{
				user_id = "user1",
				groups = new List<Wammer.Cloud.UserGroup> { new Wammer.Cloud.UserGroup { group_id = "group1" } },
				folder = "",
				isPrimaryStation = true
			};

			thumb = new ThumbnailInfo
			{
				RawData = new byte[] { 1, 2, 3, 4, 5 },
				mime_type = "mime",
				saved_file_name = "saved_thumb_name",
			};
		}

		[TestMethod]
		public void RecvNewMedium_AsyncCreateSmallAndUpstream()
		{
			Moq.Mock<IAttachmentUtil> mock = new Moq.Mock<IAttachmentUtil>(MockBehavior.Strict);
			mock.Setup(x => x.FindAttachmentInDB(oldAtt.object_id)).Returns(oldAtt).Verifiable();
			mock.Setup(x => x.FindUserByGroupIdInDB(oldAtt.group_id)).Returns(user).Verifiable();
			mock.Setup(x => x.UpstreamAttachmentAsync(oldAtt.object_id, ImageMeta.Medium, Wammer.Station.TaskPriority.Low)).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsync(oldAtt.object_id, ImageMeta.Small, Wammer.Station.TaskPriority.Medium));
			
			AttachmentProcessedHandler procHandler = new AttachmentProcessedHandler(mock.Object);

			procHandler.OnProcessed(this, new AttachmentEventArgs(
				oldAtt.object_id,
				true,
				UpsertResult.Insert,
				Wammer.Model.ImageMeta.Medium,
				"session", "apikey",
				"post1"));

			mock.VerifyAll();
		}


		[TestMethod]
		public void RecvNewOrigin_AsyncCreateThumbnailsAndUpstreamMedium()
		{
			Moq.Mock<IAttachmentUtil> mock = new Moq.Mock<IAttachmentUtil>(MockBehavior.Strict);
			mock.Setup(x => x.FindAttachmentInDB(oldAtt.object_id)).Returns(oldAtt).Verifiable();
			mock.Setup(x => x.FindUserByGroupIdInDB(oldAtt.group_id)).Returns(user).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsyncAndUpstream(oldAtt.object_id, ImageMeta.Medium, It.IsAny<Wammer.Station.TaskPriority>())).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsync(oldAtt.object_id, ImageMeta.Small, It.IsAny<Wammer.Station.TaskPriority>())).Verifiable();

			AttachmentProcessedHandler procHandler = new AttachmentProcessedHandler(mock.Object);

			procHandler.OnProcessed(this, new AttachmentEventArgs(
				oldAtt.object_id,
				true,
				UpsertResult.Insert,
				Wammer.Model.ImageMeta.Origin,
				"session", "apikey",
				"post1"));

			mock.VerifyAll();
		}


		[TestMethod]
		public void RecvKnownOrigin_AsyncCreateThumbnails()
		{
			Moq.Mock<IAttachmentUtil> mock = new Moq.Mock<IAttachmentUtil>(MockBehavior.Strict);
			mock.Setup(x => x.FindAttachmentInDB(oldAtt.object_id)).Returns(oldAtt).Verifiable();
			mock.Setup(x => x.FindUserByGroupIdInDB(oldAtt.group_id)).Returns(user).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsync(oldAtt.object_id, ImageMeta.Small, It.IsAny<Wammer.Station.TaskPriority>())).Verifiable();

			AttachmentProcessedHandler procHandler = new AttachmentProcessedHandler(mock.Object);

			procHandler.OnProcessed(this, new AttachmentEventArgs(
				oldAtt.object_id,
				false,
				UpsertResult.Update,
				Wammer.Model.ImageMeta.Origin,
				"session", "apikey",
				"post1"));

			mock.VerifyAll();
		}
	}


}
