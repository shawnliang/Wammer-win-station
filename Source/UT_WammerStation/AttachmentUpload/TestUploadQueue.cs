using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station.AttachmentUpload;
using Wammer.Utility;
using Waveface.Stream.Model;
using Wammer.Model;
using Moq;
using Wammer.Station;

namespace UT_WammerStation.AttachmentUpload
{
	[TestClass]
	public class TestUploadQueue
	{
		[TestInitialize]
		public void setup()
		{
			QueuedTaskCollection.Instance.RemoveAll();
		}

		[TestCleanup]
		public void teardown()
		{
			QueuedTaskCollection.Instance.RemoveAll();
		}

		[TestMethod]
		public void BugVerify1()
		{
			AttachmentUploadQueue q = new AttachmentUploadQueue();
			q.Init();

			Mock<INamedTask> origTask = new Mock<INamedTask>();
			origTask.Setup(t => t.Name).Returns("name1");

			// origTask is enqueued
			q.Enqueue(origTask.Object, Wammer.Station.TaskPriority.High);

			// pop origTask
			var poppedTask = q.Dequeue();
			Assert.IsNotNull(poppedTask);
			Assert.AreEqual(origTask.Object, poppedTask.Task);


			// Assume origTask runs unsuccessfully, insert it back again
			q.Enqueue(poppedTask.Task, TaskPriority.High);


			// run the failed origTask again and this time it succeeded, ack the dequeue
			var pop2 = q.Dequeue();
			Assert.IsNotNull(pop2);
			Assert.AreEqual(origTask.Object, pop2.Task);
			q.AckDequeue(pop2);



			// verify there is nothing in attachment upload queue
			Assert.AreEqual(0, q.GetCount());
			Assert.AreEqual(0L, QueuedTaskCollection.Instance.FindAll().Count());
		}
	}
}
