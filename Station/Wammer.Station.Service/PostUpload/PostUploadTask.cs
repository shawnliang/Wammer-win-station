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
	[BsonKnownTypes(typeof(NewPostTask), typeof(UpdatePostTask))]
	public abstract class PostUploadTask : ITask
	{
		public string postId { get; set; }
		public DateTime timestamp { get; set; }
		public string userId { get; set; }
		public NameValueCollection parameters { get; set; }

		public abstract void Execute();
	}

	public class NullPostUploadTask : PostUploadTask
	{
		public override void Execute()
		{
		}
	}
}
