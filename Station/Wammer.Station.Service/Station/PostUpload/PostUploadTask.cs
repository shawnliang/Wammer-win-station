﻿using System;
using System.Collections.Generic;
using System.Net;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station;

namespace Wammer.PostUpload
{
	public enum PostUploadTaskStatus
	{
		Wait,
		InProgress
	}

	[BsonKnownTypes(typeof (NewPostTask), typeof (UpdatePostTask), typeof (NullPostUploadTask))]
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
				    && Enum.IsDefined(typeof (PostApiError), e.WammerError)
				    && e.WammerError == (int) PostApiError.PostNotExist)
				{
					return false;
				}
				throw;
			}
		}

		protected bool IsAttachmentExist(string objectId)
		{
			Attachment att = AttachmentCollection.Instance.FindOne(Query.EQ("_id", objectId));
			return att != null && att.IsThumbnailOrBodyUpstreamed();
		}
	}

	public class NullPostUploadTask : PostUploadTask
	{
		public NullPostUploadTask()
		{
			CodeName = PostId = UserId = "";
			Timestamp = DateTime.UtcNow;
			Status = PostUploadTaskStatus.Wait;
			Parameters = new Dictionary<string, string>();
		}

		public override void Execute()
		{
		}
	}
}