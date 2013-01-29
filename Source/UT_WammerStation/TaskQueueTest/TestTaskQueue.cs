using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station;
using Wammer.Station.AttachmentUpload;
using Moq;
using Wammer.Queue;
using Wammer.Model;

namespace UT_WammerStation.TaskQueueTest
{
	[TestClass]
	public class TestTaskQueue
	{
		ITask t1;
		ITask t2;
		ITask t3;


		[TestInitialize]
		public void setup()
		{
			t1 = new NullTask();
			t2 = new NullTask();
			t3 = new NullTask();

			QueuedTaskCollection.Instance.RemoveAll();
		}

		[TestMethod]
		public void addItemsToTaskQueue_addItemsToThreadPool()
		{
			var threadPool = new Mock<IThreadPool>();
			var queue = new TaskQueueImp(threadPool.Object);


			int queuedCount = 0;
			threadPool.Setup(x => x.QueueWorkItem(queue.RunPriorityQueue)).Callback(() => queuedCount++);

			queue.Enqueue(t1);
			queue.Enqueue(t2);
			queue.Enqueue(t3);

			Assert.AreEqual(3, queuedCount);
		}

		[TestMethod]
		public void addItemsToTaskQueue_maxCurrency_2()
		{
			var threadPool = new Mock<IThreadPool>();
			var queue = new TaskQueueImp(threadPool.Object, 2);


			int queuedCount = 0;
			threadPool.Setup(x => x.QueueWorkItem(queue.RunPriorityQueue)).Callback(() => queuedCount++);

			queue.Enqueue(t1, TaskPriority.High);
			queue.Enqueue(t2, TaskPriority.High);
			queue.Enqueue(t3, TaskPriority.High);

			Assert.AreEqual(2, queuedCount);

			queue.RunPriorityQueue(null);
			
			Assert.AreEqual(3, queuedCount);
		}

		[TestMethod]
		public void medium_and_low_priority_task_takes_at_most_half_of_concurrency()
		{
			const int concurrency = 10;

			var threadPool = new Mock<IThreadPool>();
			var queue = new TaskQueueImp(threadPool.Object, concurrency);


			int queuedCount = 0;
			threadPool.Setup(x => x.QueueWorkItem(queue.RunPriorityQueue)).Callback(() => queuedCount++);

			queue.Enqueue(t1);
			queue.Enqueue(t2);
			queue.Enqueue(t3);
			queue.Enqueue(new NullTask());
			queue.Enqueue(new NullTask());
			queue.Enqueue(new NullTask());
			queue.Enqueue(new NullTask());

			Assert.AreEqual(concurrency / 2, queuedCount);
		}

		[TestMethod]
		public void high_priority_tasks_eat_up_all_concurrency()
		{
			const int concurrency = 5;

			var threadPool = new Mock<IThreadPool>();
			var queue = new TaskQueueImp(threadPool.Object, concurrency);


			int queuedCount = 0;
			threadPool.Setup(x => x.QueueWorkItem(queue.RunPriorityQueue)).Callback(() => queuedCount++);

			queue.Enqueue(new NullTask(), TaskPriority.High);
			queue.Enqueue(new NullTask(), TaskPriority.High);
			queue.Enqueue(new NullTask(), TaskPriority.High);
			queue.Enqueue(new NullTask(), TaskPriority.High);
			queue.Enqueue(new NullTask(), TaskPriority.High);
			queue.Enqueue(new NullTask(), TaskPriority.High);
			queue.Enqueue(new NullTask(), TaskPriority.High);

			Assert.AreEqual(concurrency, queuedCount);
		}

		[TestMethod]
		public void TestRunPriorityQueue()
		{
			const int concurrency = 10;

			var threadPool = new Mock<IThreadPool>();
			var queue = new TaskQueueImp(threadPool.Object, concurrency);


			int queuedCount = 0;
			threadPool.Setup(x => x.QueueWorkItem(queue.RunPriorityQueue)).Callback(() => queuedCount++);

			queue.Enqueue(new NullTask());
			queue.Enqueue(new NullTask());
			queue.Enqueue(new NullTask());


			Assert.AreEqual(3, queuedCount);

			queue.RunPriorityQueue(null);
			queue.RunPriorityQueue(null);
			queue.RunPriorityQueue(null);

			Assert.AreEqual(3, queuedCount);
		}

		[TestMethod]
		public void TestRunPriorityQueue2()
		{
			const int concurrency = 4;

			var threadPool = new Mock<IThreadPool>();
			var queue = new TaskQueueImp(threadPool.Object, concurrency);


			int queuedCount = 0;
			threadPool.Setup(x => x.QueueWorkItem(queue.RunPriorityQueue)).Callback(() => queuedCount++);

			queue.Enqueue(new NullTask());
			queue.Enqueue(new NullTask());

			queue.Enqueue(new NullTask());
			queue.Enqueue(new NullTask());
			queue.Enqueue(new NullTask());
			queue.Enqueue(new NullTask());

			Assert.AreEqual(concurrency / 2, queuedCount);
			

			queue.RunPriorityQueue(null);
			queue.RunPriorityQueue(null);
			Assert.AreEqual(concurrency / 2 + 2, queuedCount);

			queue.RunPriorityQueue(null);
			queue.RunPriorityQueue(null);
			Assert.AreEqual(concurrency / 2 + 2 + 2, queuedCount);
			Assert.AreEqual(6, queuedCount);

			queue.RunPriorityQueue(null);
			Assert.AreEqual(6, queuedCount);
			queue.RunPriorityQueue(null);
			Assert.AreEqual(6, queuedCount);
		}


		[TestMethod]
		public void TestFirstInFirstOut()
		{
			var tt1 = new SmokeTest();
			var tt2 = new SmokeTest();
			var tt3 = new SmokeTest();
			var tt4 = new SmokeTest();
			var tt5 = new SmokeTest();


			var threadPool = new Mock<IThreadPool>();
			var queue = new TaskQueueImp(threadPool.Object);
			threadPool.Setup(x => x.QueueWorkItem(queue.RunPriorityQueue));

			queue.Enqueue(tt1, TaskPriority.Low);
			queue.Enqueue(tt2, TaskPriority.Medium);
			queue.Enqueue(tt3, TaskPriority.High);
			queue.Enqueue(tt4, TaskPriority.VeryLow);
			queue.Enqueue(tt5, TaskPriority.Medium);

			queue.RunPriorityQueue(null);
			Assert.IsTrue(tt3.executed);
			queue.RunPriorityQueue(null);
			Assert.IsTrue(tt2.executed);
			queue.RunPriorityQueue(null);
			Assert.IsTrue(tt5.executed);
			queue.RunPriorityQueue(null);
			Assert.IsTrue(tt1.executed);
			queue.RunPriorityQueue(null);
			Assert.IsTrue(tt4.executed);
		}

		[TestMethod]
		public void testPersistency()
		{
			var queue = new TaskQueueImp(new Mock<IThreadPool>().Object);

			queue.Enqueue(new NullTask(), TaskPriority.High, true);
			queue.Enqueue(new NullTask(), TaskPriority.High, false);
			queue.Enqueue(new NullTask(), TaskPriority.Medium, true);
			queue.Enqueue(new NullTask(), TaskPriority.Medium, false);
			queue.Enqueue(new NullTask(), TaskPriority.Low, true);
			queue.Enqueue(new NullTask(), TaskPriority.Low, false);
			queue.Enqueue(new NullTask(), TaskPriority.VeryLow, true);
			queue.Enqueue(new NullTask(), TaskPriority.VeryLow, false);

			var threadPool = new Mock<IThreadPool>();
			var queue2 = new TaskQueueImp(threadPool.Object);

			int count = 0;
			threadPool.Setup(x => x.QueueWorkItem(queue2.RunPriorityQueue)).Callback(() => count++);
			queue2.Init();

			Assert.AreEqual(4, count);
		}
	}


	internal class SmokeTest : ITask
	{
		public bool executed = false;
		public void Execute()
		{
			executed = true;
		}
	}
}

