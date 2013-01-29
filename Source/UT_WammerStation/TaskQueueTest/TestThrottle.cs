using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Queue;
using Moq;
using Wammer.Station.AttachmentUpload;

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

		[TestMethod]
		public void TestMethod1()
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
	}
}
