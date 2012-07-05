using System;
using MongoDB.Driver.Builders;
using Wammer.Model;
using Wammer.Station.Retry;

namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	internal class MakeThumbnailTask : DelayedRetryTask
	{
		private readonly string object_id;
		private readonly ImageMeta thumbnail_type;
		private int retry_count;

		public static event EventHandler<ThumbnailEventArgs> ThumbnailGenerated;

		public MakeThumbnailTask(string object_id, ImageMeta thumbnail_type, TaskPriority pri)
			: base(pri)
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

			MakeThumbnail();
		}

		public void MakeThumbnail()
		{
			Attachment attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", object_id));
			if (attachment == null)
				return;

			Driver user = DriverCollection.Instance.FindDriverByGroupId(attachment.group_id);
			if (user == null)
				return;

			var imgProc = new AttachmentUtility();
			ThumbnailInfo thumbnail = imgProc.GenerateThumbnail(attachment.saved_file_name, thumbnail_type,
			                                                    object_id, user, attachment.file_name);

			imgProc.UpdateThumbnailInfoToDB(object_id, thumbnail_type, thumbnail);

			OnThumbnailGenerated(this, new ThumbnailEventArgs(object_id, attachment.post_id, attachment.group_id, thumbnail_type));
		}

		public override void ScheduleToRun()
		{
			TaskQueue.Enqueue(this, priority);
		}

		protected static void OnThumbnailGenerated(object sender, ThumbnailEventArgs evt)
		{
			EventHandler<ThumbnailEventArgs> handler = ThumbnailGenerated;
			if (handler != null)
				handler(sender, evt);
		}
	}
}