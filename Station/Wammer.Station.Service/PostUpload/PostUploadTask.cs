using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Wammer.Station;
using Wammer.Model;
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
		public NameValueCollection Parameters { get; set; }
		public PostUploadTaskStatus Status { get; set; }

		public abstract void Execute();
	}

	public class NullPostUploadTask : PostUploadTask
	{
		public override void Execute()
		{
		}
	}
}
