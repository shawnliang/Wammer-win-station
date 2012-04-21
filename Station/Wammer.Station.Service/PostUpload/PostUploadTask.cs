using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using Wammer.Station;
using Wammer.Model;
using Wammer.Cloud;
using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.PostUpload
{
	public enum PostUploadTaskStatus
	{
		Wait,
		InProgress
	}

	[BsonKnownTypes(typeof(NewPostTask), typeof(UpdatePostTask))]
	public abstract class PostUploadTask : ITask
	{
		public string PostId { get; set; }
		public DateTime Timestamp { get; set; }
		public string UserId { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
		public PostUploadTaskStatus Status { get; set; }

		public abstract void Execute();

		protected bool IsPostExist(PostApi api, WebClient agent)
		{
			try
			{
				api.PostGetSingle(agent, Parameters["group_id"], PostId);
				return true;
			}
			catch (WammerCloudException e)
			{
				if (!CloudServer.IsNetworkError(e)
					&& !CloudServer.IsSessionError(e)
					&& Enum.IsDefined(typeof(PostApiError), e.WammerError)
					&& e.WammerError == (int)PostApiError.PostNotExist)
				{
					return false;
				}
				throw e;
			}
		}

		protected bool IsAttachmentExist(AttachmentApi api, WebClient agent, string objectId)
		{
			try
			{
				api.AttachmentGet(agent, objectId);
				return true;
			}
			catch (WammerCloudException e)
			{
				if (!CloudServer.IsNetworkError(e)
					&& !CloudServer.IsSessionError(e)
					&& Enum.IsDefined(typeof(AttachmentApiError), e.WammerError)
					&& e.WammerError == (int)AttachmentApiError.AttachmentNotExist)
				{
					return false;
				}
				throw e;
			}
		}
	}

	public class NullPostUploadTask : PostUploadTask
	{
		public override void Execute()
		{
		}
	}
}
