using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wammer.Model;
using Wammer.PostUpload;
using MongoDB.Driver;

namespace UT_WammerStation
{
	[TestClass]
	public class TestPostUploadTaskQueue
	{
		private string POST_ID1 = Guid.NewGuid().ToString();
		private string POST_ID2 = Guid.NewGuid().ToString();
		private DateTime TIMESTAMP = DateTime.UtcNow;
		private Driver DRIVER = new Driver { email = "big", user_id = Guid.NewGuid().ToString() };
		private NameValueCollection PARAM = new NameValueCollection { { "apikey", Guid.NewGuid().ToString() } };

		[TestInitialize]
		public void setUp()
		{
			PostUploadTasksCollection.Instance.RemoveAll();
		}

		[TestCleanup]
		public void tearDown()
		{
			//PostUploadTasksCollection.Instance.RemoveAll();
		}

		[TestMethod]
		public void TestInitFromDB()
		{
			LinkedList<PostUploadTask> tasks = new LinkedList<PostUploadTask>();
			tasks.AddLast(new NewPostTask{postId=POST_ID1, timestamp=TIMESTAMP, driver=DRIVER, parameters=PARAM});
			tasks.AddLast(new UpdatePostTask{postId=POST_ID1, timestamp=TIMESTAMP, driver=DRIVER, parameters=PARAM});
			tasks.AddLast(new NewPostTask{postId=POST_ID1, timestamp=TIMESTAMP, driver=DRIVER, parameters=PARAM});
			tasks.AddLast(new UpdatePostTask{postId=POST_ID1, timestamp=TIMESTAMP, driver=DRIVER, parameters=PARAM});
			PostUploadTasks doc = new PostUploadTasks { post_id = POST_ID1, tasks = tasks };
			PostUploadTasksCollection.Instance.Save(doc);

			PostUploadTaskQueue queue = new PostUploadTaskQueue();
			queue.InitFromDB();

			// only 4 tasks and the sequence is [NEW, UPDATE, NEW, UPDATE]
			//Assert.IsTrue(queue.Dequeue() is NewPostTask);
			//Assert.IsTrue(queue.Dequeue() is UpdatePostTask);
			//Assert.IsTrue(queue.Dequeue() is NewPostTask);
			//Assert.IsTrue(queue.Dequeue() is UpdatePostTask);
			//Assert.IsTrue(queue.Dequeue() is NullPostUploadTask);
		}

		[TestMethod]
		public void TestInitFromEmptyDB()
		{
			PostUploadTaskQueue queue = new PostUploadTaskQueue();
			queue.InitFromDB();

			Assert.IsTrue(queue.Dequeue() is NullPostUploadTask);
		}

		[TestMethod]
		public void TestEnqueueTwiceAndDequeueTwice()
		{
			PostUploadTaskQueue queue = new PostUploadTaskQueue();
			queue.InitFromDB();
			
			queue.Enqueue(new NewPostTask { postId = POST_ID1, timestamp = TIMESTAMP, driver = DRIVER, parameters = PARAM });
			queue.Enqueue(new UpdatePostTask { postId = POST_ID1, timestamp = TIMESTAMP, driver = DRIVER, parameters = PARAM });

			PostUploadTasks doc = PostUploadTasksCollection.Instance.FindOne();
			Assert.IsTrue(doc.tasks.ElementAt(0) is NewPostTask);
			Assert.IsTrue(doc.tasks.ElementAt(1) is UpdatePostTask);

			Assert.IsTrue(queue.Dequeue() is NewPostTask);
			
			doc = PostUploadTasksCollection.Instance.FindOne();
			Assert.IsTrue(doc.tasks.ElementAt(0) is UpdatePostTask);

			Assert.IsTrue(queue.Dequeue() is UpdatePostTask);

			doc = PostUploadTasksCollection.Instance.FindOne();
			Assert.AreEqual(doc, null);

			Assert.IsTrue(queue.Dequeue() is NullPostUploadTask);
		}

		[TestMethod]
		public void TestUndo()
		{
			PostUploadTaskQueue queue = new PostUploadTaskQueue();
			queue.InitFromDB();

			queue.Enqueue(new NewPostTask { postId = POST_ID1, timestamp = TIMESTAMP, driver = DRIVER, parameters = PARAM });
			queue.Enqueue(new UpdatePostTask { postId = POST_ID1, timestamp = TIMESTAMP, driver = DRIVER, parameters = PARAM });

			PostUploadTask task = queue.Dequeue();
			Assert.IsTrue(task is NewPostTask);

			queue.Undo(task);
			Assert.IsTrue(queue.Dequeue() is NewPostTask);
		}

		[TestMethod]
		public void TestRoundRobin()
		{
			PostUploadTaskQueue queue = new PostUploadTaskQueue();
			queue.InitFromDB();

			queue.Enqueue(new NewPostTask { postId = POST_ID1, timestamp = TIMESTAMP, driver = DRIVER, parameters = PARAM });
			queue.Enqueue(new UpdatePostTask { postId = POST_ID1, timestamp = TIMESTAMP, driver = DRIVER, parameters = PARAM });
			queue.Enqueue(new NewPostTask { postId = POST_ID2, timestamp = TIMESTAMP, driver = DRIVER, parameters = PARAM });
			queue.Enqueue(new UpdatePostTask { postId = POST_ID2, timestamp = TIMESTAMP, driver = DRIVER, parameters = PARAM });

			Assert.AreEqual(queue.Dequeue().postId, POST_ID1);
			Assert.AreEqual(queue.Dequeue().postId, POST_ID2);
			Assert.AreEqual(queue.Dequeue().postId, POST_ID1);
			Assert.AreEqual(queue.Dequeue().postId, POST_ID2);
		}

		[TestMethod]
		public void TestUndoRoundRobin()
		{
			PostUploadTaskQueue queue = new PostUploadTaskQueue();
			queue.InitFromDB();

			queue.Enqueue(new NewPostTask { postId = POST_ID1, timestamp = TIMESTAMP, driver = DRIVER, parameters = PARAM });
			queue.Enqueue(new UpdatePostTask { postId = POST_ID1, timestamp = TIMESTAMP, driver = DRIVER, parameters = PARAM });
			queue.Enqueue(new NewPostTask { postId = POST_ID2, timestamp = TIMESTAMP, driver = DRIVER, parameters = PARAM });
			queue.Enqueue(new UpdatePostTask { postId = POST_ID2, timestamp = TIMESTAMP, driver = DRIVER, parameters = PARAM });

			PostUploadTask task = queue.Dequeue();
			Assert.AreEqual(task.postId, POST_ID1);

			queue.Undo(task);

			Assert.AreEqual(queue.Dequeue().postId, POST_ID2);
			Assert.AreEqual(queue.Dequeue().postId, POST_ID1);
			Assert.AreEqual(queue.Dequeue().postId, POST_ID2);
		}
	}
}
