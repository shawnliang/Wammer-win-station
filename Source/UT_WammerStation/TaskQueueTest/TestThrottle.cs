using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Queue;
using Moq;
using Wammer.Station.AttachmentUpload;
using Wammer.Model;

namespace UT_WammerStation.TaskQueueTest
{
	[TestClass]
	public class TestThrottle
	{
		class Throttle1 : Throttle
		{
			public Throttle1(int concurrentCount)
				: base(concurrentCount)
			{
			}

			public override bool ShouldThrottle(Wammer.Station.ITask task)
			{
				return true;
			}
		}

		[TestInitialize]
		public void setup()
		{
			QueuedTaskCollection.Instance.RemoveAll();
		}

		[TestMethod]
		public void throttle_allow_at_most_n_current_tasks()
		{
			var threadPool = new Mock<IThreadPool>();
			var queue = new TaskQueueImp(threadPool.Object);

			var concurrentCount = 3;
			var taskType = typeof(NullTask);
			var thottle1 = new Throttle1(concurrentCount);

			queue.AddThrottle(thottle1);

			var count = 0;
			threadPool.Setup(x => x.QueueWorkItem(queue.RunPriorityQueue)).Callback(() => count++);


			queue.Enqueue(new NullTask());
			queue.Enqueue(new NullTask());
			queue.Enqueue(new NullTask());
			queue.Enqueue(new NullTask());
			queue.Enqueue(new NullTask());

			Assert.AreEqual(3, count);

			queue.RunPriorityQueue(null);
			Assert.AreEqual(4, count);

			queue.RunPriorityQueue(null);
			Assert.AreEqual(5, count);

			queue.RunPriorityQueue(null);
			queue.RunPriorityQueue(null);
			queue.RunPriorityQueue(null);

			Assert.AreEqual(5, count);
		}

		[TestMethod]
		public void throttle_also_supports_persistency()
		{
			var threadPool = new Mock<IThreadPool>();
			var queue = new TaskQueueImp(threadPool.Object);

			var concurrentCount = 1;
			var taskType = typeof(NullTask);
			var thottle1 = new Throttle1(concurrentCount);

			queue.AddThrottle(thottle1);

			var count = 0;
			threadPool.Setup(x => x.QueueWorkItem(queue.RunPriorityQueue)).Callback(() => count++);


			queue.Enqueue(new NullTask(), Wammer.Station.TaskPriority.Medium, true);
			queue.Enqueue(new NullTask(), Wammer.Station.TaskPriority.Medium, true);
			queue.Enqueue(new NullTask(), Wammer.Station.TaskPriority.Medium, true);
			queue.Enqueue(new NullTask(), Wammer.Station.TaskPriority.Medium, true);
			queue.Enqueue(new NullTask(), Wammer.Station.TaskPriority.Medium, true);


			var loadFromStorage = 0;
			var queue2 = new TaskQueueImp(threadPool.Object);
			threadPool.Setup(x => x.QueueWorkItem(queue2.RunPriorityQueue)).Callback(() => loadFromStorage++);
			queue2.Init();

			Assert.AreEqual(5, loadFromStorage);
		}

		[TestMethod]
		public void db_is_cleared_if_a_persistent_throttle_task_is_executed()
		{
			var threadPool = new Mock<IThreadPool>();
			var queue = new TaskQueueImp(threadPool.Object);

			var concurrentCount = 1;
			var taskType = typeof(NullTask);
			var thottle1 = new Throttle1(concurrentCount);

			queue.AddThrottle(thottle1);

			var count = 0;
			threadPool.Setup(x => x.QueueWorkItem(queue.RunPriorityQueue)).Callback(() => count++);
			queue.Enqueue(new NullTask(), Wammer.Station.TaskPriority.Medium, true);
			queue.Enqueue(new NullTask(), Wammer.Station.TaskPriority.Medium, true);

			queue.RunPriorityQueue(null);
			queue.RunPriorityQueue(null);

			Assert.AreEqual(0L, QueuedTaskCollection.Instance.FindAll().Count());
		}
	}
}
