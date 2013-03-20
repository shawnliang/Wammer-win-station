using MongoDB.Driver.Builders;
using Waveface.Stream.Model;

namespace Wammer.Station.AttachmentUpload
{
	public interface IAttachmentUtil
	{
		Attachment FindAttachmentInDB(string object_id);
		Driver FindUserByGroupIdInDB(string group_id);

		void GenerateThumbnailAsync(string object_id, ImageMeta thumbnailType, TaskPriority priority);
		void GenerateThumbnailAsyncAndUpstream(string object_id, ImageMeta thumbnailType, TaskPriority priority);

		void UpdateThumbnailInfoToDB(string object_id, ImageMeta thumbnailType, ThumbnailInfo Info);

		void UpstreamAttachmentAsync(string object_id, ImageMeta meta, TaskPriority priority);
	}

	public class AttachmentProcessedHandler
	{
		private readonly IAttachmentUtil util;

		public AttachmentProcessedHandler(IAttachmentUtil util)
		{
			this.util = util;
		}

		public void OnProcessed(object sender, AttachmentEventArgs args)
		{
			Attachment attachment = util.FindAttachmentInDB(args.AttachmentId);
			Driver user = util.FindUserByGroupIdInDB(attachment.group_id);

			if (user == null)
				return; // user has been unlinked from this station?

			ProcessForFastAndSmoothClients(args, attachment, user);
		}

		private void ProcessForFastAndSmoothClients(AttachmentEventArgs args, Attachment attachment, Driver user)
		{
			bool isCoverImage = false;

			var post = PostDBDataCollection.Instance.FindOne(Query.EQ("cover_attachment_id", attachment.object_id));
			isCoverImage = post != null;

			if (!isCoverImage)
			{
				var collection = CollectionCollection.Instance.FindOne(Query.EQ("cover", attachment.object_id));
				isCoverImage = collection != null;
			}

			if (args.ImgMeta == ImageMeta.Medium)
			{
				util.GenerateThumbnailAsync(args.AttachmentId, ImageMeta.Small, isCoverImage ? TaskPriority.High : TaskPriority.Medium);
				util.UpstreamAttachmentAsync(args.AttachmentId, ImageMeta.Medium, TaskPriority.VeryLow);
			}
			else if (args.ImgMeta == ImageMeta.Origin)
			{
				if (user.isPaidUser)
					util.UpstreamAttachmentAsync(args.AttachmentId, ImageMeta.Origin, TaskPriority.VeryLow);

				if (args.UpsertResult == UpsertResult.Insert)
				{
					if (user.isPaidUser)
						util.GenerateThumbnailAsync(args.AttachmentId, ImageMeta.Medium, TaskPriority.Low);
					else
						util.GenerateThumbnailAsyncAndUpstream(args.AttachmentId, ImageMeta.Medium, TaskPriority.Low);

					util.GenerateThumbnailAsync(args.AttachmentId, ImageMeta.Small, isCoverImage ? TaskPriority.High : TaskPriority.Medium);
				}
			}
		}
	}
}
