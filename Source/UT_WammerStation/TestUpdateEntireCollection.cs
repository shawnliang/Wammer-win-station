using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver.Builders;
using Waveface.Stream.Model;

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
				Assert.IsTrue(item.IsCopyComplete);
				Assert.IsTrue(item.Error.Length > 0);
			}
		}
	}
}
