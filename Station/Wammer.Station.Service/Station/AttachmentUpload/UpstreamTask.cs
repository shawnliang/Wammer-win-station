using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using MongoDB.Driver.Builders;
using System.IO;
using Wammer.PerfMonitor;
using System.ComponentModel;

namespace Wammer.Station.AttachmentUpload
{
	class UpstreamTask: Retry.DelayedRetryTask
	{
		private static IPerfCounter upstreamRateCounter = PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE, false);

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
				return;

			Driver user = DriverCollection.Instance.FindDriverByGroupId(attachment.group_id);
			if (user == null)
				return;

			IAttachmentInfo info = attachment.GetInfoByMeta(meta);
			if (info == null)
				return;

			FileStorage fileStorage = new FileStorage(user);

			using (FileStream f = fileStorage.Load(info.saved_file_name))
			{
				Attachment.Upload(f, attachment.group_id, this.object_id, attachment.file_name,
					info.mime_type, this.meta, attachment.type, Cloud.CloudServer.APIKey,
					user.session_token, 1024, UpstreamProgressChanged);
			}
		}


		private void UpstreamProgressChanged(object sender, ProgressChangedEventArgs arg)
		{
			upstreamRateCounter.IncrementBy(Convert.ToInt64(arg.UserState));
		}
	}
}
