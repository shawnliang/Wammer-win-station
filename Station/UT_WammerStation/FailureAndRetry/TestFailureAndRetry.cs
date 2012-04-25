using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wammer.Station.Retry;

namespace UT_WammerStation.FailureAndRetry
{
	[TestClass]
	public class TestFailureAndRetry
	{
		class AbstrackRetryTask1 : AbstrackRetryTask
		{
			public AbstrackRetryTask1(IRetryQueue q)
				:base(q, Wammer.Station.TaskPriority.VeryLow)
			{}

			protected override void Do()
			{
				throw new NotImplementedException();
			}

			public override DateTime NextRetryTime
			{
				get { return DateTime.Now; }
			}

			public override void ScheduleToRun()
			{
				throw new NotImplementedException();
			}
		}

		[TestMethod]
		public void FailedTaskGoesToRetryQueue()
		{
			Mock<IRetryQueue> retryQueue = new Mock<IRetryQueue>();
			AbstrackRetryTask1 task = new AbstrackRetryTask1(retryQueue.Object);

			retryQueue.Setup(q => q.Enqueue(task)).Verifiable();
			task.Execute();
			retryQueue.VerifyAll();
		}

		[TestMethod]
		public void FailedTaskIsDequeuedAfterNextRunTime()
		{
			RetryQueue queue = new RetryQueue(new NullRetryQueuePersistentStorage());

			Mock<IRetryTask> task1 = new Mock<IRetryTask>();
			Mock<IRetryTask> task2 = new Mock<IRetryTask>();
			Mock<IRetryTask> task3 = new Mock<IRetryTask>();

			task1.Setup(t => t.NextRetryTime).Returns(new DateTime(2012, 4, 29));
			task2.Setup(t => t.NextRetryTime).Returns(new DateTime(2012, 4, 30));
			task3.Setup(t => t.NextRetryTime).Returns(new DateTime(2012, 5, 2));

			queue.Enqueue(task1.Object);
			queue.Enqueue(task2.Object);
			queue.Enqueue(task3.Object);

			ICollection<IRetryTask> tasks = queue.Dequeue(new DateTime(2012, 5, 1));
			Assert.AreEqual(2, tasks.Count);
			Assert.IsTrue(tasks.Contains(task1.Object));
			Assert.IsTrue(tasks.Contains(task2.Object));

			tasks = queue.Dequeue(new DateTime(2012, 5, 2));
			Assert.AreEqual(1, tasks.Count);
			Assert.IsTrue(tasks.Contains(task3.Object));

			tasks = queue.Dequeue(new DateTime(2099, 1, 1));
			Assert.AreEqual(0, tasks.Count);
		}


		class DelayedRetryTask1 :DelayedRetryTask
		{
			public DelayedRetryTask1(IRetryQueue q)
				:base(q, Wammer.Station.TaskPriority.Low)
			{
			}

			protected override void Run()
			{
				throw new NotImplementedException();
			}

			public override void ScheduleToRun()
			{
				throw new NotImplementedException();
			}
		}

		[TestMethod]
		public void TestDelayedTasks()
		{
			Mock<IRetryQueue> retryQueue = new Mock<IRetryQueue>();
			retryQueue.Setup(x => x.Enqueue(It.IsAny<IRetryTask>())).Verifiable();

			DelayedRetryTask1 task = new DelayedRetryTask1(retryQueue.Object);

			task.Execute();
			DateTime retry1 = task.NextRetryTime;
			task.Execute();
			DateTime retry2 = task.NextRetryTime;
			task.Execute();
			DateTime retry3 = task.NextRetryTime;
			task.Execute();
			DateTime retry4 = task.NextRetryTime;
			task.Execute();
			DateTime retry5 = task.NextRetryTime;


			Assert.IsTrue(retry1 < retry2);
			Assert.IsTrue(retry2 < retry3);
			Assert.IsTrue(retry3 < retry4);
			Assert.IsTrue(retry4 < retry5);
		}
	}
}
