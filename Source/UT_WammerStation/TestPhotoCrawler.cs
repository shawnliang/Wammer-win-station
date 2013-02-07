using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Waveface.Stream.Core;

namespace UT_WammerStation
{
	[TestClass]
	public class TestPhotoCrawler
	{
		[TestInitialize]
		public void setup()
		{
			Directory.CreateDirectory(@"dir1\.hidden");
			Directory.CreateDirectory(@"dir1\dir2\dir21");
			Directory.CreateDirectory(@"dir1\dir3\dir31");
			Directory.CreateDirectory(@"dir1\dir4");

			createJpg(@"dir1\dir2\dir21\1.jpg");
			createJpg(@"dir1\dir3\dir31\2.jpg");
		}

		private void createJpg(string file)
		{
			using (var f = File.OpenWrite(file))
			{
				f.Write("123", Encoding.ASCII);
			}
		}

		[TestCleanup]
		public void teardown()
		{
			Directory.Delete("dir1", true);
		}

		[TestMethod]
		public void findPhotoDir()
		{
			var c1 = new PhotoCrawler();

			var foundDir = new List<string>();

			c1.FindPhotoDirs("dir1", (path, count) => {
				Assert.AreEqual(1, count);
				foundDir.Add(path);
				return true; 
			});

			Assert.AreEqual(2, foundDir.Count);
		}


		[TestMethod]
		public void findPhotoDirWithPrune()
		{
			var c1 = new PhotoCrawler();

			var foundDir = new List<string>();

			int found = 0;
			c1.FindPhotoDirs("dir1", (path, count) =>
			{
				Assert.AreEqual(1, count);
				foundDir.Add(path);

				found++;

				if (found > 0)
					return false;
				else
					return true;
			});

			Assert.AreEqual(1, found);
			Assert.AreEqual(1, foundDir.Count);
		}

		[TestMethod]
		public void findPhotos()
		{
			var c1 = new PhotoCrawler();

			var foundDir = new List<string>();

			var result = c1.FindPhotos(new List<string> { @"dir1" });

			Assert.AreEqual(2, result.Count());
		}
	}
}
