using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using Wammer.PostUpload;

namespace Wammer.Model
{
	public class PostUploadTasks
	{
		[BsonId]
		public string post_id { get; set; }

		public LinkedList<PostUploadTask> tasks { get; set; }
	}

	public class PostUploadTasksCollection : Collection<PostUploadTasks>
	{
		#region Var
		private static PostUploadTasksCollection _instance;
		#endregion

		#region Property
		public static PostUploadTasksCollection Instance
		{
			get { return _instance ?? (_instance = new PostUploadTasksCollection()); }
		}
		#endregion

		private PostUploadTasksCollection()
			: base("PostUploadTasks")
		{
		}
	}
}