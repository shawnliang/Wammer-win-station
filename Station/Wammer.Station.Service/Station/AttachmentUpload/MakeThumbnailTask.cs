using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	class MakeThumbnailTask : Retry.DelayedRetryTask
	{
		private string object_id;
		private ImageMeta thumbnail_type;
		private int retry_count;
		private const int MAX_RETRY = 30;

		public MakeThumbnailTask(string object_id, ImageMeta thumbnail_type, TaskPriority pri)
			:base(Retry.RetryQueue.Instance, pri)
		{
			if (thumbnail_type == ImageMeta.Origin || thumbnail_type == ImageMeta.None)
				throw new ArgumentException("thumbnail_type");

			this.object_id = object_id;
			this.thumbnail_type = thumbnail_type;
		}

		protected override void Run()
		{
			if (++retry_count > 30)
			{
				this.LogWarnMsg("Retry making " + thumbnail_type + " of attachment " + object_id + " too many times, abort");
				return;
			}

			Attachment attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", this.object_id));
			if (attachment == null)
				return;

			Driver user = DriverCollection.Instance.FindDriverByGroupId(attachment.group_id);
			if (user == null)
				return;

			AttachmentUtility imgProc = new AttachmentUtility();
			ThumbnailInfo thumbnail = imgProc.GenerateThumbnail(attachment.saved_file_name, thumbnail_type,
				this.object_id, user, attachment.file_name);

			imgProc.UpdateThumbnailInfoToDB(this.object_id, this.thumbnail_type, thumbnail);
		}

		public override void ScheduleToRun()
		{
			TaskQueue.Enqueue(this, priority);
		}
	}
}
