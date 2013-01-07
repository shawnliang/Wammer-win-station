using MongoDB.Driver.Builders;
using System;
using System.ComponentModel;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PerfMonitor;
using Wammer.Station.Retry;
using Waveface.Stream.Model;
using System.Net;

namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	public class UpstreamTask : DelayedRetryTask
	{
		private static readonly IPerfCounter upstreamRateCounter = PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE);
		private static readonly IPerfCounter upstreamCount = PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT);

		public static event EventHandler<ThumbnailEventArgs> AttachmentUpstreamed;

		public string object_id { get; set; }
		public ImageMeta meta { get; set; }

		public UpstreamTask()
			: base(TaskPriority.Medium)
		{
		}

		public UpstreamTask(string object_id, ImageMeta meta, TaskPriority pri)
			: base(pri)
		{
			this.object_id = object_id;
			this.meta = meta;
		}

		protected override void Run()
		{
			upstreamCount.Increment();

			string user_id = null;

			try
			{
				Attachment attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", object_id));
				if (attachment == null)
					return;

				Driver user = DriverCollection.Instance.FindDriverByGroupId(attachment.group_id);
				if (user == null)
				{
					this.LogDebugMsg("User of group id " + attachment.group_id + " is removed? Abort upstream attachment " + object_id);
					return;
				}

				user_id = user.user_id;

				if (meta == ImageMeta.Origin && !user.isPaidUser)
					return;

				IAttachmentInfo info = attachment.GetInfoByMeta(meta);
				if (info == null)
				{
					this.LogErrorMsg("Abort upstream attachment " + object_id + " due to attachment info of " + meta +
									 " is empty. Logic Error?");
					return;
				}

				var fileStorage = new FileStorage(user);

				using (var f = fileStorage.Load(info.saved_file_name, meta))
				{
					AttachmentApi.Upload(f, attachment.group_id, object_id, attachment.file_name,
									  info.mime_type, meta, attachment.type, CloudServer.APIKey,
									  user.session_token,
									  65535,
									  UpstreamProgressChanged,
									  attachment.post_id,
									  attachment.file_path,
									  (attachment.image_meta == null) ? null : attachment.image_meta.exif,
									  attachment.import_time,
									  attachment.timezone,
									  attachment.file_create_time,
									  attachment.doc_meta);

					OnAttachmentUpstreamed(this, new ThumbnailEventArgs(this.object_id, attachment.post_id, attachment.group_id, this.meta));
				}

				if (meta == ImageMeta.Origin || meta == ImageMeta.None)
				{
					AttachmentCollection.Instance.Update(Query.EQ("_id", object_id), Update.Set("body_on_cloud", true));
				}

				DriverCollection.Instance.Update(Query.EQ("_id", user_id), Update.Unset("sync_range.upload_error"));
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
				{
					if (!string.IsNullOrEmpty(user_id))
					{
						var err = e.Message;

						if (e.HttpError == WebExceptionStatus.ProtocolError)
							err = e.GetCloudRetMsg();
						else if (e.InnerException is WebException)
							err = e.InnerException.Message;

						DriverCollection.Instance.Update(Query.EQ("_id", user_id),
							Update.Set("sync_range.upload_error", err));
					}

					throw;
				}
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