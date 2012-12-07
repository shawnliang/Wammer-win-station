using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Wammer;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PostUpload;

namespace UT_WammerStation
{
	[TestClass]
	public class TestPostUploadTaskQueue
	{
		private static string POST_ID1 = Guid.NewGuid().ToString();
		private static string POST_ID2 = Guid.NewGuid().ToString();
		private static DateTime TIMESTAMP = DateTime.UtcNow;
		private static string USER_ID1 = Guid.NewGuid().ToString();
		private static string GROUP_ID1 = Guid.NewGuid().ToString();
		private static string APIKEY_1 = Guid.NewGuid().ToString();
		private Dictionary<string, string> PARAM = new Dictionary<string, string> { { "apikey", APIKEY_1 }, { "group_id", GROUP_ID1 } };
		private NameValueCollection NPARAM = new NameValueCollection { { "apikey", APIKEY_1 }, { "group_id", GROUP_ID1 } };

		[TestInitialize]
		public void setUp()
		{
			PostUploadTasksCollection.Instance.RemoveAll();
			DriverCollection.Instance.RemoveAll();
		}

		[TestCleanup]
		public void tearDown()
		{
			PostUploadTasksCollection.Instance.RemoveAll();
			DriverCollection.Instance.RemoveAll();
		}

		[TestMethod]
		public void TestInitFromDB()
		{
			LinkedList<PostUploadTask> tasks = new LinkedList<PostUploadTask>();
			tasks.AddLast(new NewPostTask { PostId = POST_ID1, Timestamp = TIMESTAMP, UserId = USER_ID1, Parameters = PARAM });
			tasks.AddLast(new UpdatePostTask { PostId = POST_ID1, Timestamp = TIMESTAMP, UserId = USER_ID1, Parameters = PARAM });
			tasks.AddLast(new NewPostTask { PostId = POST_ID1, Timestamp = TIMESTAMP, UserId = USER_ID1, Parameters = PARAM });
			tasks.AddLast(new UpdatePostTask { PostId = POST_ID1, Timestamp = TIMESTAMP, UserId = USER_ID1, Parameters = PARAM });
			PostUploadTasks doc = new PostUploadTasks { post_id = POST_ID1, tasks = tasks };
			PostUploadTasksCollection.Instance.Save(doc);

			PostUploadTaskQueue queue = new PostUploadTaskQueue();
			queue.InitFromDB();

			// only 4 tasks and the sequence is [NEW, UPDATE, NEW, UPDATE]
			PostUploadTask task = queue.Dequeue();
			Assert.IsTrue(task is NewPostTask);
			queue.Done(task);
			task = queue.Dequeue();
			Assert.IsTrue(task is UpdatePostTask);
			queue.Done(task);
			task = queue.Dequeue();
			Assert.IsTrue(task is NewPostTask);
			queue.Done(task);
			task = queue.Dequeue();
			Assert.IsTrue(task is UpdatePostTask);
			queue.Done(task);
		}

		[TestMethod]
		public void TestInitFromEmptyDB()
		{
			PostUploadTaskQueue queue = new PostUploadTaskQueue();
			queue.InitFromDB();
		}

		[TestMethod]
		public void TestEnqueueTwiceAndDequeueTwice()
		{
			PostUploadTaskQueue queue = new PostUploadTaskQueue();
			queue.InitFromDB();
			queue.Enqueue(new NewPostTask { PostId = POST_ID1, Timestamp = TIMESTAMP, UserId = USER_ID1, Parameters = PARAM });
			queue.Enqueue(new UpdatePostTask { PostId = POST_ID1, Timestamp = TIMESTAMP, UserId = USER_ID1, Parameters = PARAM });

			PostUploadTasks doc = PostUploadTasksCollection.Instance.FindOne();
			Assert.IsTrue(doc.tasks.ElementAt(0) is NewPostTask);
			Assert.IsTrue(doc.tasks.ElementAt(1) is UpdatePostTask);

			PostUploadTask task = queue.Dequeue();
			Assert.IsTrue(task is NewPostTask);
			queue.Done(task);

			doc = PostUploadTasksCollection.Instance.FindOne();
			Assert.IsTrue(doc.tasks.ElementAt(0) is UpdatePostTask);

			task = queue.Dequeue();
			Assert.IsTrue(task is UpdatePostTask);
			queue.Done(task);

			doc = PostUploadTasksCollection.Instance.FindOne();
			Assert.AreEqual(doc, null);
		}

		[TestMethod]
		public void TestUndo()
		{
			PostUploadTaskQueue queue = new PostUploadTaskQueue();
			queue.InitFromDB();
			queue.Enqueue(new NewPostTask { PostId = POST_ID1, Timestamp = TIMESTAMP, UserId = USER_ID1, Parameters = PARAM });
			queue.Enqueue(new UpdatePostTask { PostId = POST_ID1, Timestamp = TIMESTAMP, UserId = USER_ID1, Parameters = PARAM });

			PostUploadTask task = queue.Dequeue();
			Assert.IsTrue(task is NewPostTask);

			queue.Undo(task);
			Assert.IsTrue(queue.Dequeue() is NewPostTask);
			queue.Done(task);
		}

		[TestMethod]
		public void TestPostUploadTaskController()
		{
			List<UserGroup> groups = new List<UserGroup>();
			groups.Add(new UserGroup { name = "big group", creator_id = USER_ID1, group_id = GROUP_ID1, description = "none" });
			DriverCollection.Instance.Save(new Driver { user_id = USER_ID1, groups = groups });

			PostUploadTaskController.Instance.AddPostUploadAction(POST_ID1, PostUploadActionType.NewPost, NPARAM, DateTime.Now, DateTime.Now);
			PostUploadTaskController.Instance.AddPostUploadAction(POST_ID1, PostUploadActionType.UpdatePost, NPARAM, DateTime.Now, DateTime.Now);

			PostUploadTask task = PostUploadTaskQueue.Instance.Dequeue();
			Assert.IsTrue(task is NewPostTask);
			PostUploadTaskQueue.Instance.Done(task);
			task = PostUploadTaskQueue.Instance.Dequeue();
			Assert.IsTrue(task is UpdatePostTask);
			PostUploadTaskQueue.Instance.Done(task);
		}

		[TestMethod]
		public void TestPostUploadTaskRunner()
		{
			PostUploadTaskQueue queue = new PostUploadTaskQueue();
			queue.InitFromDB();
			PostUploadTaskRunner runner = new PostUploadTaskRunner(queue);
			runner.Start();
			queue.Enqueue(new NewPostTask { PostId = POST_ID1, Timestamp = TIMESTAMP, UserId = USER_ID1, Parameters = PARAM });
			queue.Enqueue(new UpdatePostTask { PostId = POST_ID1, Timestamp = TIMESTAMP, UserId = USER_ID1, Parameters = PARAM });
		}
	}
}
