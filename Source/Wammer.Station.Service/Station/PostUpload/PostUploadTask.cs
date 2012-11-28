using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Wammer.Cloud;
using Wammer.Station;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace Wammer.PostUpload
{
	public enum PostUploadTaskStatus
	{
		Wait,
		InProgress
	}

	[BsonKnownTypes(typeof (NewPostTask), typeof (UpdatePostTask), typeof (NullPostUploadTask), typeof(CommentTask), typeof(HidePostTask), typeof(UnhidePostTask))]
	public abstract class PostUploadTask : ITask
	{
		public string PostId { get; set; }
		public DateTime Timestamp { get; set; }
		public string UserId { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
		public PostUploadTaskStatus Status { get; set; }
		public string CodeName { get; set; }
		public DateTime LastUpdateTime { get; set; }

		#region ITask Members

		public void Execute()
		{
			var user = DriverCollection.Instance.FindOne(Query.EQ("_id", UserId));

			if (user == null)
				return;

			if (string.IsNullOrEmpty(user.session_token))
				throw new Exception("User session is expired. Postponed this task: " + user.email);

			try
			{
				Do(user);
			}
			catch (WammerCloudException e)
			{
				this.LogDebugMsg("Post upload task failed: " + this.GetType().Name, e);

				if (CloudServer.IsNetworkError(e) || CloudServer.IsSessionError(e))
				{
					throw;
				}
			}
		}

		#endregion

		protected abstract void Do(Driver user);
	}

	public class NullPostUploadTask : PostUploadTask
	{
		public NullPostUploadTask()
		{
			CodeName = PostId = UserId = "-1";
			Timestamp = DateTime.UtcNow;
			Status = PostUploadTaskStatus.Wait;
			Parameters = new Dictionary<string, string>();
		}

		protected override void Do(Driver user)
		{
		}
	}
}