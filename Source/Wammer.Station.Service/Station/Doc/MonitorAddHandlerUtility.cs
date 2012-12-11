using System;
using MongoDB.Driver.Builders;
using Wammer.Model;

namespace Wammer.Station.Doc
{
	class MonitorAddHandlerUtility : IMonitorAddHandlerUtility
	{
		public void UpdateDocOpenTimeAsync(string user_id, string object_id, DateTime openTime)
		{
			Model.AttachmentCollection.Instance.Update(
				Query.EQ("_id", object_id), Update.Push("doc_meta.access_time", openTime));

			TaskQueue.Enqueue(new UpdateDocAccessTimeTask(user_id, object_id, openTime), TaskPriority.Medium);
		}
	}


	[Serializable]
	class UpdateDocAccessTimeTask : Retry.DelayedRetryTask
	{
		private string user_id;
		private string doc_id;
		private DateTime openTime;
		private int retryCount = 50;

		public UpdateDocAccessTimeTask(string user_id, string doc_id, DateTime openTime)
			:base(TaskPriority.Medium)
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

			Cloud.AttachmentApi.updateDocMetadata(user.session_token, Cloud.CloudServer.APIKey, doc_id, openTime);
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
