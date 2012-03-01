using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Queue;

namespace UT_WammerStation.TestData
{

	class FakePersistentStore : IPersistentStore
	{
		public WMSQueue TryLoadQueue(string queueName)
		{
			WMSQueue q = new WMSQueue();
			q.Push(new WMSMessage("1"));
			q.Push(new WMSMessage("2"));
			q.Push(new WMSMessage("3"));

			return q;
		}
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
	}
}
