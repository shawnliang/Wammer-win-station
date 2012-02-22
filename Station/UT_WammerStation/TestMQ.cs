using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Queue;


namespace UT_WammerStation
{
	[TestClass]
	public class TestMQ
	{
		[TestMethod]
		public void TestEmptyQueue()
		{
			WMSBroker qMgr = new WMSBroker();
			WMSQueue q1 = qMgr.GetQueue("name");
			Assert.IsNotNull(q1);

			WMSSession s = qMgr.CreateSession();
			Assert.IsNull(s.Pop(q1));
		}

		[TestMethod]
		public void TestGetSameQueue()
		{
			WMSBroker qMgr = new WMSBroker();
			WMSQueue q1 = qMgr.GetQueue("name");
			WMSQueue q2 = qMgr.GetQueue("name");
			Assert.IsNotNull(q1);
			Assert.IsNotNull(q2);
			Assert.AreEqual(q1, q2);
		}

		[TestMethod]
		public void TestAcknowledgeWillConsumeAnItem()
		{
			WMSBroker qMgr = new WMSBroker();
			WMSQueue q = qMgr.GetQueue("name");
			Assert.IsNotNull(q);

			WMSSession s = qMgr.CreateSession();
			s.Push(q, "abcd");

			WMSMessage qItem = s.Pop(q);
			Assert.AreEqual("abcd", (string)qItem.Data);
			qItem.Acknowledge();

			s.Close();

			s = qMgr.CreateSession();
			Assert.IsNull(s.Pop(q));
		}

		[TestMethod]
		public void TestUnackedItemsWillBeRestockedAfterSessionClosed()
		{
			WMSBroker qMgr = new WMSBroker();
			WMSQueue q = qMgr.GetQueue("name");
			WMSSession s = qMgr.CreateSession();
			s.Push(q, "abcd");

			Assert.AreEqual("abcd", (string)s.Pop(q).Data);
			Assert.IsNull(s.Pop(q));
			s.Close();


			s = qMgr.CreateSession();
			Assert.IsNotNull(s.Pop(q));
		}

		[TestMethod]
		public void TestOnlySessionUnackedMsgIsRestocked()
		{
			WMSBroker qMgr = new WMSBroker();
			WMSQueue q = qMgr.GetQueue("name");
			WMSSession s1 = qMgr.CreateSession();
			s1.Push(q, "abcd");
			s1.Push(q, "1234");
			s1.Push(q, "____");


			Assert.AreEqual("abcd", (string)s1.Pop(q).Data);

			WMSSession s2 = qMgr.CreateSession();
			Assert.AreEqual("1234", (string)s2.Pop(q).Data);


			// s1 is closed -> unacked msg "abcd" is pushed again
			s1.Close();
			Assert.AreEqual("____", (string)s2.Pop(q).Data);
			Assert.AreEqual("abcd", (string)s2.Pop(q).Data);
			Assert.IsNull(s2.Pop(q));
		}

		[TestMethod]
		public void TestAckedMessagesWillBeRemovedFromQueuePermenantly()
		{
			WMSBroker qMgr = new WMSBroker();
			WMSQueue q = qMgr.GetQueue("name");
			WMSSession s1 = qMgr.CreateSession();
			s1.Push(q, "abcd");
			s1.Push(q, "1234");
			s1.Push(q, "____");

			WMSSession s2 = qMgr.CreateSession();
			{
				WMSMessage m1 = s2.Pop(q);
				Assert.IsNotNull(m1);
				Assert.AreEqual("abcd", m1.Data);
				m1.Acknowledge();
			}
			{
				WMSMessage m1 = s2.Pop(q);
				Assert.IsNotNull(m1);
				Assert.AreEqual("1234", m1.Data);
				m1.Acknowledge();
			}

			s2.Close();

			WMSSession s3 = qMgr.CreateSession();
			Assert.AreEqual("____", (string)s3.Pop(q).Data);
			Assert.IsNull(s3.Pop(q));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestQueuePushDoesNotAcceptNull()
		{
			WMSQueue q = new WMSQueue();
			q.Push(null);
		}
	}
}
