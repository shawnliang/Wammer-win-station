using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization.Formatters.Binary;
using Wammer.Station.AttachmentUpload;
using System.IO;
using Wammer.Station.Retry;

namespace UT_WammerStation
{
	[TestClass]
	public class TestSerializable
	{
		[TestMethod]
		public void UploadMetadataTaskIsSerializable()
		{
			BinaryFormatter f = new BinaryFormatter();

			var t = new UploadMetadataTask("a", "b", 1);

			MemoryStream m = new MemoryStream();
			f.Serialize(m, t);

			m.Position = 0;
			UploadMetadataTask deserializedTask = f.Deserialize(m) as UploadMetadataTask;

			Assert.AreEqual(t.group_id, deserializedTask.group_id);
			Assert.AreEqual(t.metadata, deserializedTask.metadata);
			Assert.AreEqual(t.metaCount, deserializedTask.metaCount);
		}

		[TestMethod]
		public void UpstreamTaskIsSerialiable()
		{
			BinaryFormatter f = new BinaryFormatter();

			UpstreamTask t = new UpstreamTask("123", Wammer.Model.ImageMeta.None, Wammer.Station.TaskPriority.High);

			MemoryStream m = new MemoryStream();
			f.Serialize(m, t);

			m.Position = 0;
			UpstreamTask deserializedTask = f.Deserialize(m) as UpstreamTask;

			Assert.AreEqual(t.meta, deserializedTask.meta);
			Assert.AreEqual(t.object_id, deserializedTask.object_id);
			Assert.AreEqual(t.Priority, deserializedTask.Priority);
		}

		[TestMethod]
		public void CreatePhotoCollectionTaskIsSerializable()
		{
			BinaryFormatter f = new BinaryFormatter();

			var p = new Dictionary<string, Wammer.Station.FolderCollection>{ {"123", new Wammer.Station.FolderCollection("1","2")}};
			var t = new Wammer.Station.CreatePhotoFolderCollectionTask(p, "aaa", "bbb");

			MemoryStream m = new MemoryStream();
			f.Serialize(m, t);

			m.Position = 0;
			UpstreamTask deserializedTask = f.Deserialize(m) as UpstreamTask;
		}

		[TestMethod]
		public void PostponedTaskIsSerialiable()
		{
			BinaryFormatter f = new BinaryFormatter();

			PostponedTask t = new PostponedTask(
				DateTime.Now, 
				Wammer.Station.TaskPriority.High, 
				new UpstreamTask("123", Wammer.Model.ImageMeta.None, Wammer.Station.TaskPriority.High));

			MemoryStream m = new MemoryStream();
			f.Serialize(m, t);

			m.Position = 0;
			PostponedTask deserializedTask = f.Deserialize(m) as PostponedTask;

			Assert.AreEqual(t.Priority, deserializedTask.Priority);
		}

		[Serializable]
		class FailedTask: DelayedRetryTask
		{
			public FailedTask()
				:base(Wammer.Station.TaskPriority.High)
			{

			}

			protected override void Run()
			{
				throw new NotFiniteNumberException();
			}

			public override void ScheduleToRun()
			{
				throw new NotImplementedException();
			}
		}

		[TestMethod]
		public void FailedDelayedRetryTaskCanBeInsertedToRetryQueue()
		{
			BinaryFormatter f = new BinaryFormatter();

			FailedTask t = new FailedTask();

			MemoryStream m = new MemoryStream();
			f.Serialize(m, t);

			m.Position = 0;
			IRetryTask deserializedTask = f.Deserialize(m) as IRetryTask;


			//clean up retry queue
			RetryQueueHelper.Instance.Dequeue(DateTime.MaxValue);
			// run deserialized task => will fail internally
			deserializedTask.Execute();

			//verify the task is added to retryQueue
			ICollection<IRetryTask> tasks = RetryQueueHelper.Instance.Dequeue(DateTime.MaxValue);
			Assert.AreEqual(1, tasks.Count);
			Assert.AreEqual(deserializedTask, tasks.First());
		}
	}
}
