using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Wammer.Station;
using Wammer.Model;
using Wammer.PerfMonitor;

using MongoDB.Driver.Builders;

namespace Wammer.PostUpload
{
	public class PostUploadTaskQueue
	{
		private Queue<string> postIdQueue;
		private Dictionary<string, LinkedList<PostUploadTask>> postQueue;
		private object cs = new object();
		private PostUploadMonitor monitor = new PostUploadMonitor();

		private static PostUploadTaskQueue _instance;

		public static PostUploadTaskQueue Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PostUploadTaskQueue();
					_instance.InitFromDB();
				}
				return _instance;
			}
		}

		public void InitFromDB()
		{
			postIdQueue = new Queue<string>();
			postQueue = new Dictionary<string, LinkedList<PostUploadTask>>();
			foreach (PostUploadTasks ptasks in PostUploadTasksCollection.Instance.FindAll())
			{
				foreach (PostUploadTask task in ptasks.tasks)
				{
					task.Status = PostUploadTaskStatus.Wait;
					monitor.PostUploadTaskEnqueued();
				}
				postQueue.Add(ptasks.post_id, ptasks.tasks);
				postIdQueue.Enqueue(ptasks.post_id);
			}
		}

		public void Enqueue(PostUploadTask task)
		{
			lock (cs)
			{
				LinkedList<PostUploadTask> queue;
				if (postQueue.TryGetValue(task.PostId, out queue))
				{
					queue.AddLast(task);
				}
				else
				{
					queue = new LinkedList<PostUploadTask>();
					queue.AddLast(task);
					postQueue.Add(task.PostId, queue);
					postIdQueue.Enqueue(task.PostId);
				}
				PostUploadTasksCollection.Instance.Save(
					new PostUploadTasks { post_id = task.PostId, tasks = queue });
				
				monitor.PostUploadTaskEnqueued();
			}
		}

		public PostUploadTask Dequeue()
		{
			lock (cs)
			{
				try
				{
					if (postIdQueue.Count == 0 || postQueue.Count == 0)
					{
						return new NullPostUploadTask();
					}

					string targetPostId = postIdQueue.Dequeue();
					PostUploadTask task = postQueue[targetPostId].First();

					if (task.Status == PostUploadTaskStatus.InProgress)
					{
						return new NullPostUploadTask();
					}
					else if (task.Status == PostUploadTaskStatus.Wait)
					{
						task.Status = PostUploadTaskStatus.InProgress;
					}

					postIdQueue.Enqueue(targetPostId);

					return task;
				}
				catch (Exception e)
				{
					this.LogDebugMsg("Error while dequeueing post upload task.", e);
					return new NullPostUploadTask();
				}
			}
		}

		public void Done(PostUploadTask task)
		{
			lock (cs)
			{
				if (task is NullPostUploadTask)
				{
					return;
				}

				PostUploadTask headTask = postQueue[task.PostId].First();
				Debug.Assert(task.Timestamp == headTask.Timestamp);
				postQueue[task.PostId].RemoveFirst();

				if (postQueue[task.PostId].Count > 0)
				{
					PostUploadTasksCollection.Instance.Save(
						new PostUploadTasks {
							post_id = task.PostId, tasks = postQueue[task.PostId] });
				}
				else
				{
					postQueue.Remove(task.PostId);
					PostUploadTasksCollection.Instance.Remove(
						Query.EQ("_id", task.PostId));
				}

				monitor.PostUploadTaskDone();
			}
		}

		public void Undo(PostUploadTask task)
		{
			lock (cs)
			{
				PostUploadTask headTask = postQueue[task.PostId].First();
				Debug.Assert(task.Timestamp == headTask.Timestamp);
				headTask.Status = PostUploadTaskStatus.Wait;
			}
		}
	}
}
