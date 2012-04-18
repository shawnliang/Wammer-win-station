using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

using Wammer.Station;
using Wammer.Model;

namespace Wammer.PostUpload
{
	abstract class PostUploadTask : ITask
	{
		public string postId { get; set; }
		public DateTime timestamp { get; set; }
		public Driver driver { get; set; }
		public NameValueCollection parameters { get; set; }
		public PostUploadActionType type { get; set; }

		public abstract void Execute();
	}

	class NullPostUploadTask : PostUploadTask
	{
		public override void Execute()
		{
		}
	}
}
