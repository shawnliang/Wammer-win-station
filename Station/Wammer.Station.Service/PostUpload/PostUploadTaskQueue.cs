using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Wammer.Station;
using Wammer.Model;

using MongoDB.Driver.Builders;

namespace Wammer.PostUpload
{
	public interface IUndoablePostUploadTaskQueue
	{
		void Enqueue(PostUploadTask task);
		PostUploadTask Dequeue();
		void Undo(PostUploadTask task);
	}

	public class PostUploadTaskQueue : IUndoablePostUploadTaskQueue
	{
		private Queue<string> postIdQueue;
		private Dictionary<string, LinkedList<PostUploadTask>> postQueue;
		private object cs = new object();

		public void InitFromDB()
		{
			postIdQueue = new Queue<string>();
			postQueue = new Dictionary<string, LinkedList<PostUploadTask>>();
			foreach (PostUploadTasks ptasks in PostUploadTasksCollection.Instance.FindAll())
			{
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
			}
		}

		public PostUploadTask Dequeue()
		{
			lock (cs)
			{
				try
				{
					string targetPostId = postIdQueue.Dequeue();
					PostUploadTask task = postQueue[targetPostId].First();
					postQueue[targetPostId].RemoveFirst();
					if (postQueue[targetPostId].Count > 0)
					{
						postIdQueue.Enqueue(targetPostId);
						PostUploadTasksCollection.Instance.Save(
							new PostUploadTasks { post_id = task.PostId, tasks = postQueue[targetPostId] });
					}
					else
					{
						postQueue.Remove(targetPostId);
						PostUploadTasksCollection.Instance.Remove(
							Query.EQ("_id", targetPostId));
					}
					return task;
				}
				catch (InvalidOperationException e)
				{
					this.LogDebugMsg("Queue is empty", e);
					return new NullPostUploadTask();
				}
			}
		}

		public void Undo(PostUploadTask task)
		{
			lock (cs)
			{
				LinkedList<PostUploadTask> queue;
				if (postQueue.TryGetValue(task.PostId, out queue))
				{
					postQueue[task.PostId].AddFirst(task);
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
			}
		}
	}
}
