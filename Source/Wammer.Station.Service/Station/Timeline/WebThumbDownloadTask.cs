using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Wammer.Station.Retry;
using Waveface.Stream.Model;
using Wammer.Cloud;

namespace Wammer.Station.Timeline
{
	class WebThumbDownloadTask : DelayedRetryTask, IResourceDownloadTask
	{
		string user_id;
		string object_id;
		long webthumb_id;

		public WebThumbDownloadTask(string user_id, string object_id, long webthumb_id)
			: base(TaskPriority.Medium)
		{
			this.user_id = user_id;
			this.object_id = object_id;
			this.webthumb_id = webthumb_id;
		}

		protected override void Run()
		{
			var user = DriverCollection.Instance.FindOneById(user_id);
			if (user == null)
				return;

			if (string.IsNullOrEmpty(user.session_token))
				throw new Exception("unable to download webthumb because user session expired.");

			var saved_path = Path.Combine(FileStorage.GetCachePath(user_id), string.Format("{0}_webthumb_{1}.dat", object_id, webthumb_id));

			AttachmentApi.DownloadWebThumb(user.session_token, CloudServer.APIKey, object_id, webthumb_id, saved_path);

			AttachmentCollection.Instance.UpdateWebThumbSavedFile(object_id, webthumb_id, saved_path);
		}

		public override void ScheduleToRun()
		{
			BodySyncQueue.Instance.Enqueue(this, this.Priority);
		}

		public string Name
		{
			get { return string.Format("{0}_webthumb_{1}", object_id, webthumb_id); }
		}

		public string UserId
		{
			get { return user_id; }
		}
	}
}
