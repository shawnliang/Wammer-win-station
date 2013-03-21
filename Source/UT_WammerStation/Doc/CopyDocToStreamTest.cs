using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Waveface.Stream.Model;

namespace UT_WammerStation.Doc
{
	[TestClass]
	public class CopyDocToStreamTest
	{
		private string saved_path = "";

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

			if (!string.IsNullOrEmpty(saved_path))
			{
				var file = Path.Combine(Path.Combine("", "user1"), saved_path);
				File.SetAttributes(file, FileAttributes.Normal);

				Directory.Delete(Path.Combine("", "user1"), true);
			}
		}
	}


}
