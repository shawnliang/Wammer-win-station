using System;
using System.Collections.Generic;
using System.Net;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station;
using Wammer.Utility;

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

		#region ITask Members

		public abstract void Execute();

		#endregion

		protected bool IsPostExist(PostApi api)
		{
			try
			{
				api.PostGetSingle(Parameters["group_id"], PostId);
				return true;
			}
			catch (WammerCloudException e)
			{
				if (!CloudServer.IsNetworkError(e)
				    && !CloudServer.IsSessionError(e)
				    && Enum.IsDefined(typeof (PostApiError), e.WammerError)
				    && e.WammerError == (int) PostApiError.PostNotExist)
				{
					return false;
				}
				throw;
			}
		}

		protected bool IsAttachmentUploaded(string objectId, string session_token)
		{
			try
			{
				AttachmentApi.GetInfo(objectId, session_token);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}

	public class NullPostUploadTask : PostUploadTask
	{
		public NullPostUploadTask()
		{
			CodeName = PostId = UserId = string.Empty;
			Timestamp = DateTime.UtcNow;
			Status = PostUploadTaskStatus.Wait;
			Parameters = new Dictionary<string, string>();
		}

		public override void Execute()
		{
		}
	}
}