using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Wammer.Queue;

namespace UT_WammerStation.MongoStorage
{
	
	class Dummy : Wammer.Station.ITask
	{
		public string name { get; set; }

		public Dummy()
		{
		}

		public Dummy(string name)
		{
			this.name = name;
		}

		public void Execute()
		{
		}
	}

	[TestClass]
	public class TestMongoPersistentStorage
	{
		[TestInitialize]
		public void SetUp()
		{
			Wammer.Model.QueuedTaskCollection.Instance.RemoveAll();
		}

		[TestMethod]
		public void SaveAndLoad()
		{
			MongoPersistentStorage s = new MongoPersistentStorage();

			var id1 = Guid.NewGuid().ToString();
			s.Save(new WMSMessage(id1, new Dummy("12345")) { Queue = new WMSQueue("q1", s) });
			var id2 = Guid.NewGuid().ToString();
			s.Save(new WMSMessage(id2, new Dummy("abc")) { Queue = new WMSQueue("q2", s) });


			WMSQueue q1 = s.TryLoadQueue("q1");
			Assert.AreEqual(1, q1.Count);
			WMSMessage m1 = q1.Pop(new WMSSession());
			Assert.IsNotNull(m1);
			Assert.AreEqual(id1, m1.Id);
			Assert.AreEqual("12345", (m1.Data as Dummy).name);

			WMSQueue q2 = s.TryLoadQueue("q2");
			Assert.AreEqual(1, q2.Count);
			WMSMessage m2 = q2.Pop(new WMSSession());
			Assert.IsNotNull(m2);
			Assert.AreEqual(id2, m2.Id);
			Assert.AreEqual("abc", (m2.Data as Dummy).name);
		}

		[TestMethod]
		public void SaveAndLoad2()
		{
			MongoPersistentStorage s = new MongoPersistentStorage();

			var id1 = Guid.NewGuid().ToString();
			s.Save(new WMSMessage(id1, new Dummy("12345")) { Queue = new WMSQueue("q1", s) });
			var id2 = Guid.NewGuid().ToString();
			s.Save(new WMSMessage(id2, new Dummy("abc")) { Queue = new WMSQueue("q1", s) });


			WMSQueue q1 = s.TryLoadQueue("q1");
			Assert.AreEqual(2, q1.Count);
			WMSMessage m1 = q1.Pop(new WMSSession());
			Assert.IsNotNull(m1);
			Assert.AreEqual(id1, m1.Id);
			Assert.AreEqual("12345", (m1.Data as Dummy).name);

			WMSMessage m2 = q1.Pop(new WMSSession());
			Assert.IsNotNull(m2);
			Assert.AreEqual(id2, m2.Id);
			Assert.AreEqual("abc", (m2.Data as Dummy).name);
		}

		[TestMethod]
		public void RemoveItem()
		{
			MongoPersistentStorage s = new MongoPersistentStorage();

			var id1 = Guid.NewGuid().ToString();
			s.Save(new WMSMessage(id1, new Dummy("12345")) { Queue = new WMSQueue("q1", s) });
			var id2 = Guid.NewGuid().ToString();
			WMSMessage msg2 = new WMSMessage(id2, new Dummy("abc")) { Queue = new WMSQueue("q1", s) };
			s.Save(msg2);


			s.Remove(msg2);

			WMSQueue q1 = s.TryLoadQueue("q1");
			Assert.AreEqual(1, q1.Count);
			WMSMessage m1 = q1.Pop(new WMSSession());
			Assert.IsNotNull(m1);
			Assert.AreEqual(id1, m1.Id);
			Assert.AreEqual("12345", (m1.Data as Dummy).name);

		}
	}
}
