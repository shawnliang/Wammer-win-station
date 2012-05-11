using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station.AttachmentUpload
{
	public interface IAttachmentUtil
	{
		Attachment FindAttachmentInDB(string object_id);
		Driver FindUserByGroupIdInDB(string group_id);

		ThumbnailInfo GenerateThumbnail(string imageFilename, ImageMeta thumbnailType, string object_id, Driver user,
		                                string origin_filename);

		void GenerateThumbnailAsync(string object_id, ImageMeta thumbnailType, TaskPriority priority);
		void GenerateThumbnailAsyncAndUpstream(string object_id, ImageMeta thumbnailType, TaskPriority priority);

		void UpdateThumbnailInfoToDB(string object_id, ImageMeta thumbnailType, ThumbnailInfo Info);

		void UpstreamImageNow(byte[] imageRaw, string group_id, string object_id, string file_name, string mime_type,
		                      ImageMeta meta, string apikey, string session_token);

		void UpstreamAttachmentNow(string filename, Driver user, string object_id, string file_name, string mime_type,
		                           ImageMeta meta, AttachmentType type);

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

			if (args.ImgMeta == ImageMeta.Origin)
			{
				if (args.UpsertResult == UpsertResult.Insert)
				{
					ThumbnailInfo medium = util.GenerateThumbnail(attachment.saved_file_name,
					                                              ImageMeta.Medium, attachment.object_id, user, attachment.file_name);

					util.UpdateThumbnailInfoToDB(attachment.object_id, ImageMeta.Medium, medium);

					if (args.IsFromThisWindows)
					{
						util.UpstreamAttachmentAsync(attachment.object_id, ImageMeta.Medium, TaskPriority.Medium);
					}
					else
					{
						util.UpstreamImageNow(medium.RawData, attachment.group_id, attachment.object_id, attachment.file_name,
						                      medium.mime_type, ImageMeta.Medium, CloudServer.APIKey, user.session_token);
					}
				}
				else
				{
					util.GenerateThumbnailAsync(attachment.object_id, ImageMeta.Medium, TaskPriority.Medium);
				}

				util.GenerateThumbnailAsync(attachment.object_id, ImageMeta.Small, TaskPriority.Medium);
				util.GenerateThumbnailAsyncAndUpstream(attachment.object_id, ImageMeta.Large, TaskPriority.Low);
				util.GenerateThumbnailAsync(attachment.object_id, ImageMeta.Square, TaskPriority.Low);

				if (!user.isPrimaryStation)
					util.UpstreamAttachmentAsync(attachment.object_id, ImageMeta.Origin, TaskPriority.VeryLow);
			}
			else if (args.ImgMeta == ImageMeta.None)
			{
				if (args.IsFromThisWindows)
				{
					util.UpstreamAttachmentAsync(attachment.object_id, args.ImgMeta, TaskPriority.Low);
				}
				else
				{
					util.UpstreamAttachmentNow(attachment.saved_file_name, user, args.AttachmentId, attachment.file_name,
					                           attachment.mime_type, ImageMeta.None, attachment.type);
				}
			}
			else
			{
				if (args.IsFromThisWindows)
				{
					util.UpstreamAttachmentAsync(args.AttachmentId, args.ImgMeta, TaskPriority.Low);
				}
				else
				{
					IAttachmentInfo info = attachment.GetInfoByMeta(args.ImgMeta);
					util.UpstreamAttachmentNow(info.saved_file_name, user, args.AttachmentId, attachment.file_name, info.mime_type,
					                           args.ImgMeta, AttachmentType.image);
				}
			}
		}
	}
}