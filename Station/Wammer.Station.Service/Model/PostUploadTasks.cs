﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Wammer.PostUpload;
using MongoDB.Bson.Serialization.Attributes;

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
		private static PostUploadTasksCollection instance;

		static PostUploadTasksCollection()
		{
			instance = new PostUploadTasksCollection();
		}

		private PostUploadTasksCollection()
			: base("PostUploadTasks")
		{
		}

		public static PostUploadTasksCollection Instance
		{
			get { return instance; }
		}
	}
}