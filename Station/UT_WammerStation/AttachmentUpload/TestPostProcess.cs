using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station.AttachmentUpload;
using Moq;
using Wammer.Model;
using Wammer.Cloud;
using Wammer.Station;

namespace UT_WammerStation.AttachmentUpload
{
	[TestClass]
	public class TestPostProcess
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
				groups = new List<Wammer.Cloud.UserGroup>{ new Wammer.Cloud.UserGroup{ group_id = "group1"} },
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
		public void InlineGenerateAndUpstreamMediumThumbnailForNewOriginImageFromNonWindows()
		{
			Moq.Mock<IAttachmentUtil> mock = new Moq.Mock<IAttachmentUtil>(MockBehavior.Strict);
			mock.Setup(x => x.FindAttachmentInDB("object1")).Returns(oldAtt).Verifiable();
			mock.Setup(x => x.FindUserByGroupIdInDB(oldAtt.group_id)).Returns(user).Verifiable();
			mock.Setup(x => x.GenerateThumbnail(oldAtt.saved_file_name, ImageMeta.Medium, oldAtt.object_id, user, oldAtt.file_name)).
				Returns(thumb).Verifiable();
			mock.Setup(x => x.UpdateThumbnailInfoToDB(oldAtt.object_id, ImageMeta.Medium, thumb)).Verifiable();
			mock.Setup(x => x.UpstreamImageNow(thumb.RawData, oldAtt.group_id, oldAtt.object_id, oldAtt.file_name, thumb.mime_type, ImageMeta.Medium, CloudServer.APIKey, user.session_token)).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsync(oldAtt.object_id, ImageMeta.Small, It.IsAny<TaskPriority>())).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsyncAndUpstream(oldAtt.object_id, ImageMeta.Large, It.IsAny<TaskPriority>())).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsync(oldAtt.object_id, ImageMeta.Square, It.IsAny<TaskPriority>())).Verifiable();

			AttachmentProcessedHandler procHandler = new AttachmentProcessedHandler(mock.Object);

			procHandler.OnProcessed(this,
				new Wammer.Station.AttachmentUpload.AttachmentEventArgs(
					"object1",							// object id
					false,								// not from this windows
					UpsertResult.Insert,				// is new
					Wammer.Model.ImageMeta.Origin));	// original image

			mock.VerifyAll();
		}

		[TestMethod]
		public void InlineGenerateAndUpstreamMediumThumbnailForNewOriginImageFromNonWindows_NonPrimaryStation()
		{
			user.isPrimaryStation = false;

			Moq.Mock<IAttachmentUtil> mock = new Moq.Mock<IAttachmentUtil>(MockBehavior.Strict);
			mock.Setup(x => x.FindAttachmentInDB("object1")).Returns(oldAtt).Verifiable();
			mock.Setup(x => x.FindUserByGroupIdInDB(oldAtt.group_id)).Returns(user).Verifiable();
			mock.Setup(x => x.GenerateThumbnail(oldAtt.saved_file_name, ImageMeta.Medium, oldAtt.object_id, user, oldAtt.file_name)).
				Returns(thumb).Verifiable();
			mock.Setup(x => x.UpdateThumbnailInfoToDB(oldAtt.object_id, ImageMeta.Medium, thumb)).Verifiable();
			mock.Setup(x => x.UpstreamImageNow(thumb.RawData, oldAtt.group_id, oldAtt.object_id, oldAtt.file_name, thumb.mime_type, ImageMeta.Medium, CloudServer.APIKey, user.session_token)).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsync(oldAtt.object_id, ImageMeta.Small, It.IsAny<TaskPriority>())).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsyncAndUpstream(oldAtt.object_id, ImageMeta.Large, It.IsAny<TaskPriority>())).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsync(oldAtt.object_id, ImageMeta.Square, It.IsAny<TaskPriority>())).Verifiable();
			mock.Setup(x => x.UpstreamAttachmentAsync(oldAtt.object_id, ImageMeta.Origin, TaskPriority.VeryLow)).Verifiable();

			AttachmentProcessedHandler procHandler = new AttachmentProcessedHandler(mock.Object);

			procHandler.OnProcessed(this,
				new Wammer.Station.AttachmentUpload.AttachmentEventArgs(
					"object1",							// object id
					false,								// not from this windows
					UpsertResult.Insert,				// is new
					Wammer.Model.ImageMeta.Origin));	// original image

			mock.VerifyAll();
		}

		[TestMethod]
		public void InlineGenerateMediumThumbnailAndOfflineUpstreamForNewOriginImageFromWindows()
		{
			Moq.Mock<IAttachmentUtil> mock = new Moq.Mock<IAttachmentUtil>(MockBehavior.Strict);
			mock.Setup(x => x.FindAttachmentInDB("object1")).Returns(oldAtt).Verifiable();
			mock.Setup(x => x.FindUserByGroupIdInDB(oldAtt.group_id)).Returns(user).Verifiable();
			mock.Setup(x => x.GenerateThumbnail(oldAtt.saved_file_name, ImageMeta.Medium, oldAtt.object_id, user, oldAtt.file_name)).
				Returns(thumb).Verifiable();
			mock.Setup(x => x.UpdateThumbnailInfoToDB(oldAtt.object_id, ImageMeta.Medium, thumb)).Verifiable();
			mock.Setup(x => x.UpstreamAttachmentAsync(oldAtt.object_id, ImageMeta.Medium, TaskPriority.Medium)).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsync(oldAtt.object_id, ImageMeta.Small, It.IsAny<TaskPriority>())).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsyncAndUpstream(oldAtt.object_id, ImageMeta.Large, It.IsAny<TaskPriority>())).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsync(oldAtt.object_id, ImageMeta.Square, It.IsAny<TaskPriority>())).Verifiable();
			//mock.Setup(x => x.UpstreamAttachmentAsync(oldAtt.object_id, ImageMeta.Origin, TaskPriority.VeryLow)).Verifiable();

			AttachmentProcessedHandler procHandler = new AttachmentProcessedHandler(mock.Object);

			procHandler.OnProcessed(this,
				new Wammer.Station.AttachmentUpload.AttachmentEventArgs(
					"object1",							// object id
					true,								// from this windows
					UpsertResult.Insert,				// is new
					Wammer.Model.ImageMeta.Origin));	// original image

			mock.VerifyAll();
		}

		[TestMethod]
		public void InlineGenerateMediumThumbnailAndOfflineUpstreamForNewOriginImageFromWindows_NonPrimaryStation()
		{
			user.isPrimaryStation = false;
			Moq.Mock<IAttachmentUtil> mock = new Moq.Mock<IAttachmentUtil>(MockBehavior.Strict);
			mock.Setup(x => x.FindAttachmentInDB("object1")).Returns(oldAtt).Verifiable();
			mock.Setup(x => x.FindUserByGroupIdInDB(oldAtt.group_id)).Returns(user).Verifiable();
			mock.Setup(x => x.GenerateThumbnail(oldAtt.saved_file_name, ImageMeta.Medium, oldAtt.object_id, user, oldAtt.file_name)).
				Returns(thumb).Verifiable();
			mock.Setup(x => x.UpdateThumbnailInfoToDB(oldAtt.object_id, ImageMeta.Medium, thumb)).Verifiable();
			mock.Setup(x => x.UpstreamAttachmentAsync(oldAtt.object_id, ImageMeta.Medium, TaskPriority.Medium)).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsync(oldAtt.object_id, ImageMeta.Small, It.IsAny<TaskPriority>())).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsyncAndUpstream(oldAtt.object_id, ImageMeta.Large, It.IsAny<TaskPriority>())).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsync(oldAtt.object_id, ImageMeta.Square, It.IsAny<TaskPriority>())).Verifiable();
			mock.Setup(x => x.UpstreamAttachmentAsync(oldAtt.object_id, ImageMeta.Origin, TaskPriority.VeryLow)).Verifiable();

			AttachmentProcessedHandler procHandler = new AttachmentProcessedHandler(mock.Object);

			procHandler.OnProcessed(this,
				new Wammer.Station.AttachmentUpload.AttachmentEventArgs(
					"object1",							// object id
					true,								// from this windows
					UpsertResult.Insert,				// is new
					Wammer.Model.ImageMeta.Origin));	// original image

			mock.VerifyAll();
		}

		[TestMethod]
		public void OfflineGenerateThumbnailsAndUpstreamForOldOriginImageFromNonWindows()
		{
			Moq.Mock<IAttachmentUtil> mock = new Moq.Mock<IAttachmentUtil>(MockBehavior.Strict);
			mock.Setup(x => x.FindAttachmentInDB("object1")).Returns(oldAtt).Verifiable();
			mock.Setup(x => x.FindUserByGroupIdInDB(oldAtt.group_id)).Returns(user).Verifiable();

			mock.Setup(x => x.GenerateThumbnailAsync(oldAtt.object_id, ImageMeta.Medium, It.IsAny<TaskPriority>())).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsync(oldAtt.object_id, ImageMeta.Small, It.IsAny<TaskPriority>())).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsyncAndUpstream(oldAtt.object_id, ImageMeta.Large, It.IsAny<TaskPriority>())).Verifiable();
			mock.Setup(x => x.GenerateThumbnailAsync(oldAtt.object_id, ImageMeta.Square, It.IsAny<TaskPriority>())).Verifiable();
			//mock.Setup(x => x.UpstreamAttachmentAsync(oldAtt.object_id, ImageMeta.Origin, TaskPriority.VeryLow)).Verifiable();

			AttachmentProcessedHandler procHandler = new AttachmentProcessedHandler(mock.Object);

			procHandler.OnProcessed(this,
				new Wammer.Station.AttachmentUpload.AttachmentEventArgs(
					"object1",							// object id
					false,								// not from this windows
					UpsertResult.Update,				// is new
					Wammer.Model.ImageMeta.Origin));	// original image

			mock.VerifyAll();
		}

		[TestMethod]
		public void OnlineUpstreamForNewThumbnialFromNonWindows()
		{
			Moq.Mock<IAttachmentUtil> mock = new Moq.Mock<IAttachmentUtil>(MockBehavior.Strict);
			mock.Setup(x => x.FindAttachmentInDB("object1")).Returns(oldAtt).Verifiable();
			mock.Setup(x => x.FindUserByGroupIdInDB(oldAtt.group_id)).Returns(user).Verifiable();
			mock.Setup(x => x.UpstreamAttachmentNow(oldAtt.image_meta.medium.saved_file_name, user, oldAtt.object_id, oldAtt.file_name, oldAtt.image_meta.medium.mime_type, ImageMeta.Medium, AttachmentType.image)).Verifiable();

			AttachmentProcessedHandler procHandler = new AttachmentProcessedHandler(mock.Object);

			procHandler.OnProcessed(this,
				new Wammer.Station.AttachmentUpload.AttachmentEventArgs(
					"object1",							// object id
					false,								// not from this windows
					UpsertResult.Update,				// is new
					Wammer.Model.ImageMeta.Medium));	// medium image

			mock.VerifyAll();
		}

		[TestMethod]
		public void OfflineUpstreamOldThumbnialFromWindows()
		{
			Moq.Mock<IAttachmentUtil> mock = new Moq.Mock<IAttachmentUtil>(MockBehavior.Strict);
			mock.Setup(x => x.FindAttachmentInDB("object1")).Returns(oldAtt).Verifiable();
			mock.Setup(x => x.FindUserByGroupIdInDB(oldAtt.group_id)).Returns(user).Verifiable();
			mock.Setup(x => x.UpstreamAttachmentAsync(oldAtt.object_id, ImageMeta.Medium, TaskPriority.Low)).Verifiable();

			AttachmentProcessedHandler procHandler = new AttachmentProcessedHandler(mock.Object);

			procHandler.OnProcessed(this,
				new Wammer.Station.AttachmentUpload.AttachmentEventArgs(
					"object1",							// object id
					true,								// from this windows
					UpsertResult.Update,				// is not new
					Wammer.Model.ImageMeta.Medium));	// medium image

			mock.VerifyAll();
		}

		[TestMethod]
		public void OnlineUpstreamDocForNonWindows()
		{
			oldAtt.type = AttachmentType.doc;

			Moq.Mock<IAttachmentUtil> mock = new Moq.Mock<IAttachmentUtil>();
			mock.Setup(x => x.FindAttachmentInDB("object1")).Returns(oldAtt).Verifiable();
			mock.Setup(x => x.FindUserByGroupIdInDB(oldAtt.group_id)).Returns(user).Verifiable();
			mock.Setup(x => x.UpstreamAttachmentNow(oldAtt.saved_file_name, user, oldAtt.object_id, oldAtt.file_name, oldAtt.mime_type, ImageMeta.None, AttachmentType.doc)).Verifiable();

			AttachmentProcessedHandler procHandler = new AttachmentProcessedHandler(mock.Object);

			procHandler.OnProcessed(this,
				new Wammer.Station.AttachmentUpload.AttachmentEventArgs(
					"object1",							// object id
					false,								// from this windows
					UpsertResult.Update,				// is new
					Wammer.Model.ImageMeta.None));	// medium image

			mock.VerifyAll();
		}

		[TestMethod]
		public void OfflineUpstreamDocForWindows()
		{
			oldAtt.type = AttachmentType.doc;

			Moq.Mock<IAttachmentUtil> mock = new Moq.Mock<IAttachmentUtil>();
			mock.Setup(x => x.FindAttachmentInDB("object1")).Returns(oldAtt).Verifiable();
			mock.Setup(x => x.FindUserByGroupIdInDB(oldAtt.group_id)).Returns(user).Verifiable();
			mock.Setup(x => x.UpstreamAttachmentAsync(oldAtt.object_id, ImageMeta.None, TaskPriority.Low)).Verifiable();

			AttachmentProcessedHandler procHandler = new AttachmentProcessedHandler(mock.Object);

			procHandler.OnProcessed(this,
				new Wammer.Station.AttachmentUpload.AttachmentEventArgs(
					"object1",							// object id
					true,								// from this windows
					UpsertResult.Update,				// is new
					Wammer.Model.ImageMeta.None));	// medium image

			mock.VerifyAll();
		}
	}
}