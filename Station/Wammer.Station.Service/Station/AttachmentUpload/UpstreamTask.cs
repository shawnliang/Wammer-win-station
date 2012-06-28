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

		public UpstreamTask(string object_id, ImageMeta meta, TaskPriority pri)
			: base(RetryQueue.Instance, pri)
		{
			this.object_id = object_id;
			this.meta = meta;
		}

		public string object_id { get; private set; }
		public ImageMeta meta { get; private set; }

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

			IAttachmentInfo info = attachment.GetInfoByMeta(meta);
			if (info == null)
			{
				this.LogErrorMsg("Abort upstream attachment " + object_id + " due to attachment info of " + meta +
				                 " is empty. Logic Error?");
				upstreamCount.Decrement();
				return;
			}

			var fileStorage = new FileStorage(user);

			using (FileStream f = fileStorage.Load(info.saved_file_name))
			{
				Attachment.Upload(f, attachment.group_id, object_id, attachment.file_name,
				                  info.mime_type, meta, attachment.type, CloudServer.APIKey,
				                  user.session_token, 65535, UpstreamProgressChanged);
			}

			upstreamCount.Decrement();
		}

		public override void ScheduleToRun()
		{
			AttachmentUploadQueue.Instance.Enqueue(this, Priority);
		}

		private void UpstreamProgressChanged(object sender, ProgressChangedEventArgs arg)
		{
			upstreamRateCounter.IncrementBy(Convert.ToInt64(arg.UserState));
		}
	}
}