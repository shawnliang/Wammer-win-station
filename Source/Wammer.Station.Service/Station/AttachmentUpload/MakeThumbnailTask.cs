using MongoDB.Driver.Builders;
using System;
using Wammer.Model;
using Wammer.PerfMonitor;
using Wammer.Station.Retry;
using Waveface.Stream.Model;
using System.IO;
using System.Drawing;

namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	internal class MakeThumbnailTask : DelayedRetryTask
	{
		public string object_id { get; set; }
		public ImageMeta thumbnail_type { get; set; }
		public int retry_count { get; set; }
		public Guid? importTaskId { get; set; }

		private static IPerfCounter smallThumbnailCounter = PerfCounter.GetCounter(PerfCounter.SMALL_THUMBNAIL_GENERATE_COUNT);
		private static IPerfCounter mediumThumbnailCounter = PerfCounter.GetCounter(PerfCounter.MEDIUM_THUMBNAIL_GENERATE_COUNT);
		public static event EventHandler<ThumbnailEventArgs> ThumbnailGenerated;

		public MakeThumbnailTask()
			:base(TaskPriority.Medium)
		{
		}

		public MakeThumbnailTask(string object_id, ImageMeta thumbnail_type, TaskPriority pri, Guid? importTaskId = null)
			: base(pri)
		{
			if (thumbnail_type == ImageMeta.Origin || thumbnail_type == ImageMeta.None)
				throw new ArgumentException("thumbnail_type");

			this.object_id = object_id;
			this.thumbnail_type = thumbnail_type;
			this.importTaskId = importTaskId;
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

			ThumbnailInfo thumbnail = null;

			if (thumbnail_type == ImageMeta.Small)
			{
				if (attachment.HasMediumPreview())
				{
					thumbnail = imgProc.GenerateThumbnail(attachment.image_meta.medium.saved_file_name,
									thumbnail_type, object_id, user, attachment.file_name, ImageMeta.Medium);
				}
				else if (attachment.HasOriginFile())
				{
					thumbnail = imgProc.GenerateThumbnail(attachment.saved_file_name,
									thumbnail_type, object_id, user, attachment.file_name);
				}
			}
			else if (thumbnail_type == ImageMeta.Medium && attachment.HasOriginFile())
			{
				thumbnail = imgProc.GenerateThumbnail(attachment.saved_file_name, thumbnail_type,
								object_id, user, attachment.file_name);
			}

			if (thumbnail == null)
			{
				this.LogWarnMsg("File is not available to make thumbnail: " + attachment.object_id);
				return;
			}

			imgProc.UpdateThumbnailInfoToDB(object_id, thumbnail_type, thumbnail);

			OnThumbnailGenerated(this, new ThumbnailEventArgs(object_id, attachment.post_id, attachment.group_id, thumbnail_type));

			if (ImageMeta.Medium == thumbnail_type)
				mediumThumbnailCounter.Increment();
			else if (ImageMeta.Small == thumbnail_type)
				smallThumbnailCounter.Increment();

			updateImportTaskIfNeeded();
		}

		private void updateImportTaskIfNeeded()
		{
			if (importTaskId.HasValue && thumbnail_type == ImageMeta.Medium)
				TaskStatusCollection.Instance.Update(
					Query.EQ("_id", importTaskId.Value),
					Update.Inc("Thumbnailed", 1));
		}

		public override void ScheduleToRun()
		{
			TaskQueue.Enqueue(this, priority, true);
		}

		protected static void OnThumbnailGenerated(object sender, ThumbnailEventArgs evt)
		{
			EventHandler<ThumbnailEventArgs> handler = ThumbnailGenerated;
			if (handler != null)
				handler(sender, evt);
		}
	}
}