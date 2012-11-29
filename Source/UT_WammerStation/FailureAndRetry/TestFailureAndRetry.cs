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
		[Serializable]
		class AbstrackRetryTask1 : AbstrackRetryTask
		{
			public AbstrackRetryTask1()
				: base(Wammer.Station.TaskPriority.VeryLow)
			{ }

			protected override void Do()
			{
				throw new NotImplementedException();
			}

			public override DateTime NextRetryTime
			{
				get { return DateTime.Now; }
				set { }
			}

			public override void ScheduleToRun()
			{
				throw new NotImplementedException();
			}
		}

		[TestInitialize]
		public void SetUp()
		{
			RetryQueueHelper.Instance.Clear();
		}

		[TestMethod]
		public void FailedTaskGoesToRetryQueue()
		{
			AbstrackRetryTask1 task = new AbstrackRetryTask1();
			task.Execute();

			var retrtTasks = RetryQueueHelper.Instance.Dequeue(DateTime.Now);
			Assert.AreEqual(1, retrtTasks.Count);
			Assert.AreEqual(task, retrtTasks.First());
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

		[TestMethod]
		public void TasksWithSameRetryTimeWillBeForcedSeperated()
		{
			RetryQueue queue = new RetryQueue(new NullRetryQueuePersistentStorage());

			Mock<IRetryTask> task1 = new Mock<IRetryTask>();
			Mock<IRetryTask> task2 = new Mock<IRetryTask>();

			task1.SetupProperty(x => x.NextRetryTime, new DateTime(2012, 1, 1));
			task2.SetupProperty(x => x.NextRetryTime, new DateTime(2012, 1, 1));

			queue.Enqueue(task1.Object);
			queue.Enqueue(task2.Object);

			ICollection<IRetryTask> tasks = queue.Dequeue(new DateTime(2012, 5, 1));
			Assert.AreEqual(2, tasks.Count);
			Assert.IsTrue(tasks.Contains(task1.Object));
			Assert.IsTrue(tasks.Contains(task2.Object));

			Assert.AreEqual(new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Local).ToUniversalTime(), task1.Object.NextRetryTime.ToUniversalTime());
			Assert.AreEqual(new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Local).ToUniversalTime().AddMilliseconds(1.0).Ticks, task2.Object.NextRetryTime.ToUniversalTime().Ticks);
		}

		[Serializable]
		class DelayedRetryTask1 : DelayedRetryTask
		{
			public DelayedRetryTask1(IRetryQueue q)
				: base(Wammer.Station.TaskPriority.Low)
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

		[TestMethod]
		public void TestSavedRetryTaskNextRunTimeIsKey()
		{
			Wammer.Model.RetryQueueCollection.Instance.RemoveAll();
			RetryQueuePersistentStorage storage = new RetryQueuePersistentStorage();


			DateTime key1 = DateTime.Now;

			Moq.Mock<IRetryQueue> queue = new Mock<IRetryQueue>(MockBehavior.Strict);

			storage.Add(key1, new DelayedRetryTask1(queue.Object));

			var savedTasks = storage.LoadSavedTasks();
			Assert.AreEqual(1, savedTasks.Count);
			var savedItem = savedTasks.First();
			Assert.AreEqual(key1.ToUniversalTime(), savedItem.NextRunTime.ToUniversalTime());
		}

		[Serializable]
		class FakeTask : IRetryTask
		{
			DateTime key;

			public FakeTask(DateTime key)
			{
				this.key = key;
			}

			public DateTime NextRetryTime
			{
				get { return key; }
				set { key = value; }
			}

			public Wammer.Station.TaskPriority Priority
			{
				get { return Wammer.Station.TaskPriority.Medium; }
			}

			public void ScheduleToRun()
			{
				throw new NotImplementedException();
			}

			public void Execute()
			{
				throw new NotImplementedException();
			}
		}

		[TestMethod]
		public void AfterAckingDequeue_retryTaskIsRemovedFromDB()
		{
			RetryQueuePersistentStorage storage = new RetryQueuePersistentStorage();
			DateTime key1 = DateTime.Now;

			storage.Add(key1, new FakeTask(key1));

			RetryQueue queue = new RetryQueue(storage);
			queue.LoadSavedTasks(null);
			var popped_tasks = queue.Dequeue(DateTime.Now);
			Assert.AreEqual(1, popped_tasks.Count);

			foreach (var task in popped_tasks)
			{
				queue.AckDequeue(task.NextRetryTime);
			}

			Assert.AreEqual(0, storage.LoadSavedTasks().Count);
		}
	}
}
