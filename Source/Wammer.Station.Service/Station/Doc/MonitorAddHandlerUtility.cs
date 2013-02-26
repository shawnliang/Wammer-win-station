using MongoDB.Driver.Builders;
using System;
using Waveface.Stream.Model;

namespace Wammer.Station.Doc
{
	class MonitorAddHandlerUtility : IMonitorAddHandlerUtility
	{
		public void UpdateDocOpenTimeAsync(string user_id, string object_id, DateTime openTime)
		{
			AttachmentCollection.Instance.Update(
				Query.EQ("_id", object_id), Update.Push("doc_meta.access_time", openTime));

			TaskQueue.Enqueue(new UpdateDocAccessTimeTask(user_id, object_id, openTime), TaskPriority.Medium);
		}


		public bool IsFileInStreamFolder(string user_id, string file_path)
		{
			var user = DriverCollection.Instance.FindOneById(user_id);

			return file_path.ToLower().Contains(user.folder.ToLower());
		}
	}


	[Serializable]
	class UpdateDocAccessTimeTask : Retry.DelayedRetryTask
	{
		public string user_id { get; set; }
		public string doc_id { get; set; }
		public DateTime openTime { get; set; }
		public int retryCount { get; set; }

		public UpdateDocAccessTimeTask()
			: base(TaskPriority.Medium)
		{
			retryCount = 50;
		}

		public UpdateDocAccessTimeTask(string user_id, string doc_id, DateTime openTime)
			: this()
		{
			this.user_id = user_id;
			this.doc_id = doc_id;
			this.openTime = openTime;
		}

		protected override void Run()
		{
			var user = DriverCollection.Instance.FindOneById(user_id);
			if (user == null)
				return;

			Cloud.AttachmentApi.updateDocMetadata(user.session_token, CloudServer.APIKey, doc_id, openTime);
		}

		public override void ScheduleToRun()
		{
			if (--retryCount > 0)
			{
				TaskQueue.Enqueue(this, this.priority);
			}
		}
	}
}
