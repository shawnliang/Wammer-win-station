using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Wammer.Station;

namespace UT_WammerStation.MoveFolder
{
	[TestClass]
	public class TestFolderUtility
	{
		[TestInitialize]
		public void setup()
		{
			if (Directory.Exists("000"))
				Directory.Delete("000", true);

			if (Directory.Exists("kkk"))
				Directory.Delete("kkk", true);

			Directory.CreateDirectory(@"kkk");

			Directory.CreateDirectory(@"000");
			Directory.CreateDirectory(@"000\111");
			Directory.CreateDirectory(@"000\111\333");
			Directory.CreateDirectory(@"000\222");
			Directory.CreateDirectory(@"000\222\444");

			using (var f = File.OpenWrite(@"000\a.dat"))
			{
				f.Write(new byte[] { 1, 2, 3 }, 0, 3);
			}

			File.Copy(@"000\a.dat", @"000\111\b1.dat");
			File.Copy(@"000\a.dat", @"000\111\b11.dat");
			File.Copy(@"000\a.dat", @"000\111\333\b3.dat");
			File.Copy(@"000\a.dat", @"000\222\b2.dat");
			File.Copy(@"000\a.dat", @"000\222\444\b4.dat");
		}

		[TestMethod]
		public void testRecursiveCopy()
		{
			var ut = new FolderUtility();
			var src = new DirectoryInfo("000");
			var dest = new DirectoryInfo("kkk");

			ut.RecursiveCopy(src.FullName, dest.FullName);

			Assert.IsTrue(File.Exists(@"kkk\111\b1.dat"));
			Assert.IsTrue(File.Exists(@"kkk\111\b11.dat"));
			Assert.IsTrue(File.Exists(@"kkk\111\333\b3.dat"));
			Assert.IsTrue(File.Exists(@"kkk\222\b2.dat"));
			Assert.IsTrue(File.Exists(@"kkk\222\444\b4.dat"));
		}

		[TestMethod]
		public void testMove()
		{
			var ut = new FolderUtility();
			var src = new DirectoryInfo("000");
			var dest = new DirectoryInfo("kkk");

			ut.MoveOnSameDrive(src.FullName, dest.FullName);

			Assert.IsTrue(File.Exists(@"kkk\111\b1.dat"));
			Assert.IsTrue(File.Exists(@"kkk\111\b11.dat"));
			Assert.IsTrue(File.Exists(@"kkk\111\333\b3.dat"));
			Assert.IsTrue(File.Exists(@"kkk\222\b2.dat"));
			Assert.IsTrue(File.Exists(@"kkk\222\444\b4.dat"));

			Assert.IsFalse(Directory.Exists(src.FullName));
		}
	}
}
