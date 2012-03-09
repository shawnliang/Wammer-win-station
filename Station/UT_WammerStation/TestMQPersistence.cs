using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Queue;

namespace UT_WammerStation.TestData
{
	class Item
	{
		public string QName {get;set;}
		public object Data {get;set;}

		public Item(string qname, object data)
		{
			this.QName = qname;
			this.Data = data;
		}
	}

	class FakePersistentStore : IPersistentStore
	{
		public FakePersistentStore()
		{
			SavedItems = new List<Item>();
			DeletedItems = new List<Item>();
		}

		public WMSQueue TryLoadQueue(string queueName)
		{
			
			WMSQueue q = new WMSQueue(queueName, this);
			if (queueName == "perQueue")
			{
				q.Push(new WMSMessage("1"), false);
				q.Push(new WMSMessage("2"), false);
				q.Push(new WMSMessage("3"), false);
			}

			return q;
		}

		public void Save(WMSMessage msg)
		{
			SavedItems.Add(new Item(msg.Queue.Name, msg.Data));
		}

		public void Remove(WMSMessage msg)
		{
			DeletedItems.Add(new Item(msg.Queue.Name, msg.Data));
		}

		public List<Item> DeletedItems { get; private set; }
		public List<Item> SavedItems { get; private set; }
	}

	[TestClass]
	public class TestMQPersistence
	{
		[TestMethod]
		public void TestLoadFromPersistentStore()
		{
			FakePersistentStore pStore = new FakePersistentStore();

			WMSBroker broker = new WMSBroker(pStore);
			WMSQueue q = broker.GetQueue("perQueue");
			WMSSession s = broker.CreateSession();

			Assert.AreEqual("1", (string)s.Pop(q).Data);
			Assert.AreEqual("2", (string)s.Pop(q).Data);
			Assert.AreEqual("3", (string)s.Pop(q).Data);
			Assert.IsNull(s.Pop(q));
		}

		[TestMethod]
		public void TestWriteToPersistentStore()
		{
			FakePersistentStore pStore = new FakePersistentStore();

			WMSBroker broker = new WMSBroker(pStore);
			WMSQueue q1 = broker.GetQueue("q1");
			WMSQueue q2 = broker.GetQueue("q2");
			WMSSession s = broker.CreateSession();

			s.Push(q1, "1234567890", true);
			s.Push(q1, "abcdefghij", true);
			s.Push(q1, "----------");

			s.Push(q2, "q2q2q2", true);
			s.Push(q2, "nnnnnn");

			Assert.AreEqual(3, pStore.SavedItems.Count);
			Assert.AreEqual("1234567890", (string)pStore.SavedItems[0].Data);
			Assert.AreEqual("q1", (string)pStore.SavedItems[0].QName);
			Assert.AreEqual("abcdefghij", (string)pStore.SavedItems[1].Data);
			Assert.AreEqual("q1", (string)pStore.SavedItems[1].QName);
			Assert.AreEqual("q2q2q2", (string)pStore.SavedItems[2].Data);
			Assert.AreEqual("q2", (string)pStore.SavedItems[2].QName);
		}

		[TestMethod]
		public void TestRemoveFromPersistentStore()
		{
			FakePersistentStore pStore = new FakePersistentStore();

			WMSBroker broker = new WMSBroker(pStore);
			WMSQueue q1 = broker.GetQueue("q1");
			WMSSession s = broker.CreateSession();

			s.Push(q1, "1", true);
			s.Push(q1, "2", true);
			s.Push(q1, "3", true);
			WMSMessage m1 = s.Pop(q1);
			WMSMessage m2 = s.Pop(q1);
			WMSMessage m3 = s.Pop(q1);
			m1.Acknowledge();
			m2.Acknowledge();

			s.Close();

			Assert.AreEqual(2, pStore.DeletedItems.Count);
			Assert.AreEqual("1", (string)pStore.DeletedItems[0].Data);
			Assert.AreEqual("2", (string)pStore.DeletedItems[1].Data);

			Assert.AreEqual(3, pStore.SavedItems.Count);
		}

		[TestMethod]
		public void TestNonPersistentMsgWontAffectStorage()
		{
			FakePersistentStore pStore = new FakePersistentStore();

			WMSBroker broker = new WMSBroker(pStore);
			WMSQueue q1 = broker.GetQueue("q1");
			WMSSession s = broker.CreateSession();

			s.Push(q1, "1", false);  // non-persistent data
			s.Push(q1, "2", false);  // non-persistent data
			s.Push(q1, "3", false);  // non-persistent data
			WMSMessage m1 = s.Pop(q1);
			WMSMessage m2 = s.Pop(q1);
			WMSMessage m3 = s.Pop(q1);
			m1.Acknowledge();
			m2.Acknowledge();

			s.Close();

			Assert.AreEqual(0, pStore.DeletedItems.Count);
			Assert.AreEqual(0, pStore.SavedItems.Count);
		}

	}
}
