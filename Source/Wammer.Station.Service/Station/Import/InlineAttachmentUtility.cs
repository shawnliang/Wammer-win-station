using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station.AttachmentUpload;
using Waveface.Stream.Model;

namespace Wammer.Station.Import
{
	class InlineAttachmentUtility : IAttachmentUtil
	{
		private Guid ImportTaskId { get; set; }
		private AttachmentUtility util;

		public InlineAttachmentUtility(Guid importTaskId)
		{
			ImportTaskId = importTaskId;
			util = new AttachmentUtility(importTaskId);
		}

		public Waveface.Stream.Model.Attachment FindAttachmentInDB(string object_id)
		{
			return util.FindAttachmentInDB(object_id);
		}

		public Waveface.Stream.Model.Driver FindUserByGroupIdInDB(string group_id)
		{
			return util.FindUserByGroupIdInDB(group_id);
		}

		public void GenerateThumbnailAsync(string object_id, Waveface.Stream.Model.ImageMeta thumbnailType, TaskPriority priority)
		{
			var att = AttachmentCollection.Instance.FindOneById(object_id);
			var thumbInfo = att.GetInfoByMeta(thumbnailType);

			if (thumbInfo != null && !string.IsNullOrEmpty(thumbInfo.saved_file_name))
				return;

			var task = new MakeThumbnailTask(object_id, thumbnailType, priority, ImportTaskId);
			task.Execute();
		}

		public void GenerateThumbnailAsyncAndUpstream(string object_id, Waveface.Stream.Model.ImageMeta thumbnailType, TaskPriority priority)
		{
			GenerateThumbnailAsync(object_id, thumbnailType, priority);

			UpstreamAttachmentAsync(object_id, thumbnailType, priority);
		}

		public void UpdateThumbnailInfoToDB(string object_id, Waveface.Stream.Model.ImageMeta thumbnailType, Waveface.Stream.Model.ThumbnailInfo Info)
		{
			util.UpdateThumbnailInfoToDB(object_id, thumbnailType, Info);
		}

		public void UpstreamAttachmentAsync(string object_id, Waveface.Stream.Model.ImageMeta meta, TaskPriority priority)
		{
			var upstream = new UpstreamTask(object_id, meta, priority, ImportTaskId);
			AttachmentUpload.AttachmentUploadQueueHelper.Instance.Enqueue(upstream, priority);
		}
	}
}
