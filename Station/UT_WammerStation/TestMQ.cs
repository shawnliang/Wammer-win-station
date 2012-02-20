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
			QueueManager qMgr = new QueueManager();
			Queue q1 = qMgr.CreateQueue("queue1");
			Assert.IsNotNull(q1);
			Assert.IsNull(q1.Pop());
		}
	}
}
