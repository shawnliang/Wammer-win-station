using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station.Retry;

namespace UT_WammerStation.FailureAndRetry
{
	[Serializable]
	class Task : IRetryTask
	{
		public Task(string tag)
		{
			Tag = tag;
		}

		public string Tag { get; set; }

		public DateTime NextRetryTime
		{
			get { throw new NotImplementedException(); }
		}

		public Wammer.Station.TaskPriority Priority
		{
			get { throw new NotImplementedException(); }
		}

		public void Execute()
		{
			throw new NotImplementedException();
		}


		public void ScheduleToRun()
		{
			throw new NotImplementedException();
		}
	}


	class BadTask:IRetryTask
	{
		public DateTime NextRetryTime
		{
			get { throw new NotImplementedException(); }
		}

		public Wammer.Station.TaskPriority Priority
		{
			get { throw new NotImplementedException(); }
		}

		public void Execute()
		{
			throw new NotImplementedException();
		}


		public void ScheduleToRun()
		{
			throw new NotImplementedException();
		}
	}

	[TestClass]
	public class TestRetryQueuePersistentStore
	{
		[TestInitialize]
		public void Setup()
		{
			Wammer.Model.RetryQueueCollection.Instance.RemoveAll();
		}

		[TestMethod]
		public void SavedTasksCanBeLoaded()
		{
			RetryQueuePersistentStorage store = new RetryQueuePersistentStorage();

			DateTime k1 = new DateTime(2011,1,1, 0,0,0, DateTimeKind.Local);
			Task t1  = new Task("t1");
			DateTime k2 = new DateTime(2011,3,1, 0,0,0, DateTimeKind.Utc);
			Task t2  = new Task("t2");
			DateTime k3 = new DateTime(2010,12,1, 0,0,0, DateTimeKind.Local);
			Task t3  = new Task("t3");

			store.Add(k1, t1);
			store.Add(k2, t2);
			store.Add(k3, t3);

			ICollection<RetryQueueItem> items = store.LoadSavedTasks();
			int result = 0;
			foreach (var item in items)
			{
				if (item.NextRunTime == k1.ToUniversalTime() && (item.Task as Task).Tag == t1.Tag)
					result |= 0x001;
				else if (item.NextRunTime == k2.ToUniversalTime() && (item.Task as Task).Tag == t2.Tag)
					result |= 0x010;
				else if (item.NextRunTime == k3.ToUniversalTime() && (item.Task as Task).Tag == t3.Tag)
					result |= 0x100;
			}

			Assert.AreEqual((int)0x111, result);
		}

		[TestMethod]
		public void TestRemoveItems()
		{
			RetryQueuePersistentStorage store = new RetryQueuePersistentStorage();

			DateTime k1 = new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Local);
			Task t1 = new Task("t1");
			DateTime k2 = new DateTime(2011, 3, 1, 0, 0, 0, DateTimeKind.Utc);
			Task t2 = new Task("t2");
			DateTime k3 = new DateTime(2010, 12, 1, 0, 0, 0, DateTimeKind.Local);
			Task t3 = new Task("t3");

			store.Add(k1, t1);
			store.Add(k2, t2);
			store.Add(k3, t3);


			store.Remove(k3);
			store.Remove(k1);

			ICollection<RetryQueueItem> items = store.LoadSavedTasks();
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(k2, items.First().NextRunTime.ToUniversalTime());
			Assert.AreEqual(t2.Tag, (items.First().Task as Task).Tag);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TaskMustBeSerializable()
		{
			new RetryQueuePersistentStorage().Add(
				DateTime.Now,
				new BadTask());
		}
	}
}
