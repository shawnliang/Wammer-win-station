using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using MongoDB.Driver.Builders;
using System.IO;
using Wammer.PerfMonitor;
using System.ComponentModel;
using Wammer.Cloud;


namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	public class UpstreamTask: Retry.DelayedRetryTask
	{
		private static readonly IPerfCounter upstreamRateCounter = PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE, false);
		private static readonly IPerfCounter upstreamCount = PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT, false);

		public string object_id { get; private set; }
		public ImageMeta meta { get; private set; }

		public UpstreamTask(string object_id, ImageMeta meta, TaskPriority pri)
			:base(Retry.RetryQueue.Instance, pri)
		{
			this.object_id = object_id;
			this.meta = meta;
		}

		protected override void Run()
		{
			Attachment attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", object_id));
			if (attachment == null)
			{
				upstreamCount.Decrement();
				return;
			}

			Driver user = DriverCollection.Instance.FindDriverByGroupId(attachment.group_id);
			if (user == null)
			{
				this.LogDebugMsg("User of group id " + attachment.group_id + " is removed? Abort upstream attachment " + object_id);
				upstreamCount.Decrement();
				return;
			}

			if (this.meta == ImageMeta.Origin || this.meta == ImageMeta.None)
			{
				GetUserResponse userInfo = User.GetInfo(user.user_id, Cloud.CloudServer.APIKey, user.session_token);
				if (userInfo.storages.waveface.over_quota)
				{
					failQueue.Enqueue(new Retry.PostponedTask(
						userInfo.storages.waveface.interval.GetIntervalEndTime(),
						this.priority, this));

					this.LogWarnMsg("Postpone upstream attachment " + object_id + " to " + userInfo.storages.waveface.interval.GetIntervalEndTime() + "due to over quota");
					return;
				}
			}

			IAttachmentInfo info = attachment.GetInfoByMeta(meta);
			if (info == null)
			{
				this.LogErrorMsg("Abort upstream attachment " + object_id + " due to attachment info of " + meta + " is empty. Logic Error?");
				upstreamCount.Decrement();
				return;
			}

			var fileStorage = new FileStorage(user);

			using (FileStream f = fileStorage.Load(info.saved_file_name))
			{
				Attachment.Upload(f, attachment.group_id, this.object_id, attachment.file_name,
					info.mime_type, this.meta, attachment.type, Cloud.CloudServer.APIKey,
					user.session_token, 65535, UpstreamProgressChanged);
			}

			AttachmentCollection.Instance.Update(
				Query.EQ("_id", this.object_id),
				(this.meta == ImageMeta.Origin || this.meta == ImageMeta.Origin) ?
					Update.Set("is_body_upstreamed", true) : Update.Set("is_thumb_upstreamed", true));

			upstreamCount.Decrement();
		}

		public override void ScheduleToRun()
		{
			AttachmentUploadQueue.Instance.Enqueue(this, this.Priority);
		}

		private void UpstreamProgressChanged(object sender, ProgressChangedEventArgs arg)
		{
			upstreamRateCounter.IncrementBy(Convert.ToInt64(arg.UserState));
		}
	}
}
