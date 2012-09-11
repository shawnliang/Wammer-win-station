using System;
using System.ComponentModel;
using System.IO;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PerfMonitor;
using Wammer.Station.Retry;

namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	public class UpstreamTask : DelayedRetryTask
	{
		private static readonly IPerfCounter upstreamRateCounter = PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE);
		private static readonly IPerfCounter upstreamCount = PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT);

		public static event EventHandler<ThumbnailEventArgs> AttachmentUpstreamed;

		public UpstreamTask(string object_id, ImageMeta meta, TaskPriority pri)
			: base(pri)
		{
			this.object_id = object_id;
			this.meta = meta;
		}

		public string object_id { get; private set; }
		public ImageMeta meta { get; private set; }

		protected override void Run()
		{
			upstreamCount.Increment();

			try
			{
				Attachment attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", object_id));
				if (attachment == null)
					return;

				if (meta == ImageMeta.Medium && attachment.body_on_cloud)
					return;	// Body is already uploaded to cloud. No need to upload medium again.
							// This happens only on secondary station..

				Driver user = DriverCollection.Instance.FindDriverByGroupId(attachment.group_id);
				if (user == null)
				{
					this.LogDebugMsg("User of group id " + attachment.group_id + " is removed? Abort upstream attachment " + object_id);
					return;
				}

				IAttachmentInfo info = attachment.GetInfoByMeta(meta);
				if (info == null)
				{
					this.LogErrorMsg("Abort upstream attachment " + object_id + " due to attachment info of " + meta +
									 " is empty. Logic Error?");
					return;
				}

				var fileStorage = new FileStorage(user);

				using (FileStream f = fileStorage.Load(info.saved_file_name))
				{
					Attachment.Upload(f, attachment.group_id, object_id, attachment.file_name,
									  info.mime_type, meta, attachment.type, CloudServer.APIKey,
									  user.session_token, 65535, UpstreamProgressChanged,
									  attachment.post_id, attachment.memo, attachment.image_meta.exif);

					OnAttachmentUpstreamed(this, new ThumbnailEventArgs(this.object_id, attachment.post_id, attachment.group_id, this.meta));
				}

				if (meta == ImageMeta.Origin)
				{
					AttachmentCollection.Instance.Update(Query.EQ("_id", object_id), Update.Set("body_on_cloud", true));
				}
			}
			catch (WammerCloudException e)
			{
				if (e.WammerError == (int)AttachmentApiError.FileExisted ||
					e.WammerError == (int)AttachmentApiError.InvalidObjectId ||
					e.WammerError == (int)AttachmentApiError.InvalidPostId)
				{
					this.LogWarnMsg("attachment " + object_id + " is rejected by Cloud. Drop this task.");
				}
				else
					throw;
			}
			finally
			{
				upstreamCount.Decrement();
			}
		}

		public override void ScheduleToRun()
		{
			AttachmentUploadQueueHelper.Instance.Enqueue(this, Priority);
		}

		private void UpstreamProgressChanged(object sender, ProgressChangedEventArgs arg)
		{
			upstreamRateCounter.IncrementBy(Convert.ToInt64(arg.UserState));
		}

		private static void OnAttachmentUpstreamed(object sender, ThumbnailEventArgs evt)
		{
			EventHandler<ThumbnailEventArgs> handler = AttachmentUpstreamed;
			if (handler != null)
				handler(sender, evt);
		}
	}
}