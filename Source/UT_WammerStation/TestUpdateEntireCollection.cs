using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waveface.Stream.Model;
using MongoDB.Driver.Builders;

namespace UT_WammerStation
{
	[TestClass]
	public class TestUpdateEntireCollection
	{
		[TestInitialize]
		public void setup()
		{
			TaskStatusCollection.Instance.RemoveAll();
			TaskStatusCollection.Instance.Save(new ImportTaskStaus { });
			TaskStatusCollection.Instance.Save(new ImportTaskStaus { });
			TaskStatusCollection.Instance.Save(new ImportTaskStaus { });
		}

		[TestCleanup]
		public void teardown()
		{
			TaskStatusCollection.Instance.RemoveAll();
		}

		[TestMethod]
		public void testHideAll()
		{
			TaskStatusCollection.Instance.HideAll();
			var hiddenCount = TaskStatusCollection.Instance.Find(Query.EQ("Hidden", true)).Count();

			Assert.AreEqual(3L, hiddenCount);
		}

		[TestMethod]
		public void testMarkAllPendingTasksAsAborted()
		{
			TaskStatusCollection.Instance.AbortAllIncompleteTasks();
			var all = TaskStatusCollection.Instance.FindAll();

			foreach (var item in all)
			{
				Assert.IsTrue(item.IsComplete);
				Assert.IsTrue(item.Error.Length > 0);
			}
		}
	}
}
