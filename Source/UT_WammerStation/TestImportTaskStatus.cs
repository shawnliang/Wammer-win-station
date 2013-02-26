using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waveface.Stream.Model;

namespace UT_WammerStation
{
	[TestClass]
	public class TestImportTaskStatus
	{
		[TestMethod]
		public void testEnumerate()
		{
			var task = new ImportTaskStaus() { IsStarted = true };

			Assert.IsTrue(task.IsEnumerating());
			Assert.IsFalse(task.IsEnumerated());
		}

		[TestMethod]
		public void testIndexing()
		{
			var task = new ImportTaskStaus() { IsStarted = true, Total = 100 };
			Assert.IsTrue(task.IsIndexing());
			Assert.IsFalse(task.IsIndexed());
		}

		[TestMethod]
		public void testIndexing2()
		{
			var task = new ImportTaskStaus() { IsStarted = true, Total = 100, Indexed = 50, Skipped = 30 };
			Assert.IsTrue(task.IsIndexing());
			Assert.IsFalse(task.IsIndexed());
		}

		[TestMethod]
		public void testIndexed()
		{
			var task = new ImportTaskStaus() { IsStarted = true, Total = 100, Indexed = 50, Skipped = 50 };
			Assert.IsFalse(task.IsIndexing());
			Assert.IsTrue(task.IsIndexed());
		}

		[TestMethod]
		public void testIndexed2()
		{
			var task = new ImportTaskStaus() { IsStarted = true, Total = 100, Indexed = 0, Skipped = 100 };
			Assert.IsFalse(task.IsIndexing());
			Assert.IsTrue(task.IsIndexed());
		}

		[TestMethod]
		public void testIndexed3()
		{
			var task = new ImportTaskStaus() { IsStarted = true, Total = 100, Indexed = 100, Skipped = 0 };
			Assert.IsFalse(task.IsIndexing());
			Assert.IsTrue(task.IsIndexed());
		}

		[TestMethod]
		public void testCopying()
		{
			var task = new ImportTaskStaus() { IsStarted = true, Total = 100, Indexed = 100, Copied = 50 };
			Assert.IsTrue(task.IsCopying());
			Assert.IsFalse(task.IsCopied());
		}

		[TestMethod]
		public void testCopied()
		{
			var task = new ImportTaskStaus() { IsStarted = true, Total = 100, Indexed = 100, Copied = 100 };
			Assert.IsFalse(task.IsCopying());
			Assert.IsTrue(task.IsCopied());
		}

		[TestMethod]
		public void testCopied2()
		{
			var task = new ImportTaskStaus() { IsStarted = true, Total = 100, Indexed = 80, Skipped = 20, Copied = 70 };
			for (int i = 0; i < 10; i++)
				task.CopyFailed.Add(new ObjectIdAndPath());

			Assert.IsFalse(task.IsCopying());
			Assert.IsTrue(task.IsCopied());
		}

		[TestMethod]
		public void testThumbnailing()
		{
			var task = new ImportTaskStaus() { IsStarted = true, Total = 100, Indexed = 80, Skipped = 20, Copied = 70, Thumbnailed = 20 };
			for (int i = 0; i < 10; i++)
				task.CopyFailed.Add(new ObjectIdAndPath());

			Assert.IsTrue(task.IsThumbnailing());
			Assert.IsFalse(task.IsThumbnailed());
		}

		[TestMethod]
		public void testThumbnailed()
		{
			var task = new ImportTaskStaus() { IsStarted = true, Total = 100, Indexed = 80, Skipped = 20, Copied = 70, Thumbnailed = 70 };
			for (int i = 0; i < 10; i++)
				task.CopyFailed.Add(new ObjectIdAndPath());

			Assert.IsFalse(task.IsThumbnailing());
			Assert.IsTrue(task.IsThumbnailed());
		}

		[TestMethod]
		public void testSyncing()
		{
			var task = new ImportTaskStaus() { IsStarted = true, Total = 100, Indexed = 80, Skipped = 20, Copied = 70, Thumbnailed = 70, UploadCount = 30, UploadedCount = 25 };
			for (int i = 0; i < 10; i++)
				task.CopyFailed.Add(new ObjectIdAndPath());

			Assert.IsTrue(task.IsUploading());
			Assert.IsFalse(task.IsUploaded());
		}

		[TestMethod]
		public void testSynced()
		{
			var task = new ImportTaskStaus() { IsStarted = true, Total = 100, Indexed = 80, Skipped = 20, Copied = 70, Thumbnailed = 70, UploadCount = 30, UploadedCount = 30 };
			for (int i = 0; i < 10; i++)
				task.CopyFailed.Add(new ObjectIdAndPath());

			Assert.IsFalse(task.IsUploading());
			Assert.IsTrue(task.IsUploaded());
		}
	}
}
