using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MongoDB.Driver.Builders;
using Wammer.Model;
using Wammer.PerfMonitor;

namespace Wammer.PostUpload
{
	public class PostUploadTaskQueue
	{
		private static PostUploadTaskQueue _instance;
		private readonly object cs = new object();
		private readonly Semaphore headTasks = new Semaphore(0, int.MaxValue);
		private readonly PostUploadMonitor monitor = new PostUploadMonitor();

		private List<string> postIDList = new List<string>();
		private Dictionary<string, LinkedList<PostUploadTask>> postQueue;

		//TODO: 沒Session或已沒Driver的Task不該立即載入

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
			//var lowPriorityTasks = new List<PostUploadTask>();
			postQueue = new Dictionary<string, LinkedList<PostUploadTask>>();
			foreach (PostUploadTasks ptasks in PostUploadTasksCollection.Instance.FindAll())
			{
				foreach (PostUploadTask task in ptasks.tasks)
				{
					//var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", task.UserId));

					//if (driver == null)
					//    continue;

					//if(string.IsNullOrEmpty(driver.session_token))
					//{
					//    lowPriorityTasks.Add(task);
					//    continue;
					//}

					task.Status = PostUploadTaskStatus.Wait;
					Enqueue(task);
				}
			}

			//foreach (var task in lowPriorityTasks)
			//{
			//    task.Status = PostUploadTaskStatus.Wait;
			//    Enqueue(task);
			//}
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
					postIDList.Add(task.PostId);

					queue = new LinkedList<PostUploadTask>();
					queue.AddLast(task);
					postQueue.Add(task.PostId, queue);

					AddAvailableHeadTask();
				}
				PostUploadTasksCollection.Instance.Save(
					new PostUploadTasks {post_id = task.PostId, tasks = queue});

				monitor.PostUploadTaskEnqueued();
			}
		}

		public PostUploadTask Dequeue()
		{
			IsAvailableHeadTaskExist();
			lock (cs)
			{
				//var lowPriorityTasks = new List<PostUploadTask>();
				foreach (var postID in postIDList)
				{
					var task = postQueue[postID].First();

					//var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", task.UserId));

					//if (driver == null)
					//    continue;

					//if (string.IsNullOrEmpty(driver.session_token))
					//{
					//    lowPriorityTasks.Add(task);
					//    continue;
					//}

					if (task.Status == PostUploadTaskStatus.Wait)
					{
						task.Status = PostUploadTaskStatus.InProgress;
						return task;
					}
				}

				//foreach (var task in lowPriorityTasks)
				//{
				//    if (task.Status == PostUploadTaskStatus.Wait)
				//    {
				//        task.Status = PostUploadTaskStatus.InProgress;
				//        return task;
				//    }
				//}
				throw new InvalidOperationException("No available head tasks in queue");
			}
		}

		public void Done(PostUploadTask task)
		{
			lock (cs)
			{
				PostUploadTask headTask = postQueue[task.PostId].First();
				Debug.Assert(task.Timestamp == headTask.Timestamp);
				postQueue[task.PostId].RemoveFirst();

				if (postQueue[task.PostId].Count > 0)
				{
					PostUploadTasksCollection.Instance.Save(
						new PostUploadTasks
							{
								post_id = task.PostId,
								tasks = postQueue[task.PostId]
							});
					AddAvailableHeadTask();
				}
				else
				{
					postIDList.Remove(task.PostId);
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
				postIDList.Remove(task.PostId);
				postIDList.Add(task.PostId);

				PostUploadTask headTask = postQueue[task.PostId].First();
				Debug.Assert(task.Timestamp == headTask.Timestamp);
				headTask.Status = PostUploadTaskStatus.Wait;
				AddAvailableHeadTask();
			}
		}

		private void IsAvailableHeadTaskExist()
		{
			headTasks.WaitOne();
		}

		private void AddAvailableHeadTask()
		{
			headTasks.Release();
		}
	}
}