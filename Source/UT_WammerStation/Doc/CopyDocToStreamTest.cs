using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Wammer.Model;
using Wammer.Station;

namespace UT_WammerStation.Doc
{
	[TestClass]
	public class CopyDocToStreamTest
	{
		[TestInitialize]
		public void setUp()
		{
			Directory.CreateDirectory("kkkkk");
			Directory.CreateDirectory(@"kkkkk\aaaaa");
			using (var fileStream = File.OpenWrite(@"kkkkk\aaaaa\123.doc"))
			{
				fileStream.Write(new byte[1024], 0, 1024);
			}

			if (Directory.Exists("user1"))
				Directory.Delete("user1", true);
		}

		[TestCleanup]
		public void tearDown()
		{
			Directory.Delete(@"kkkkk", true);
		}

		[TestMethod]
		public void CopyToStreamTest()
		{
			var storage = new FileStorage(new Driver { folder = "user1" });
			var saved_path = storage.CopyToStorage(@"kkkkk\aaaaa\123.doc");

			var fileTime = File.GetLastWriteTime(@"kkkkk\aaaaa\123.doc");
			var expectPath = string.Format(@"{0}\{1}\{2}\123.doc", fileTime.Year.ToString("d4"), fileTime.Month.ToString("d2"), fileTime.Day.ToString("d2"));

			Assert.AreEqual(expectPath, saved_path);
			Assert.AreEqual(fileTime, File.GetLastWriteTime(Path.Combine(storage.basePath, saved_path)));
		}
	}


}
