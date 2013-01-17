using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station;
using Waveface.Stream.Model;


namespace UT_WammerStation
{
	[TestClass]
	public class TestFolderCollection
	{
		[TestMethod]
		public void TestFolderCollection1()
		{
			var files = new List<ObjectIdAndPath> { 
				new ObjectIdAndPath { file_path = @"c:\a\b\c\1.jpg", object_id = "1"},
				new ObjectIdAndPath { file_path = @"c:\a\b\c\2.jpg", object_id = "2"},
				new ObjectIdAndPath { file_path = @"c:\m\n\3.jpg", object_id = "3"},
				new ObjectIdAndPath { file_path = @"c:\4.jpg", object_id = "4"}
			};

			Dictionary<string, FolderCollection> res = FolderCollection.Build(files);

			var dirs = res.Keys;
			Assert.AreEqual(3, dirs.Count);
			Assert.IsTrue(dirs.Contains(@"c:\a\b\c"));
			Assert.IsTrue(dirs.Contains(@"c:\m\n"));
			Assert.IsTrue(dirs.Contains(@"c:\"));


			var col1 = res[@"c:\a\b\c"];
			Assert.AreEqual("c", col1.FolderName);
			Assert.AreEqual(@"c:\a\b\c", col1.FolderPath);
			Assert.IsTrue(col1.Objects.Contains("1"));
			Assert.IsTrue(col1.Objects.Contains("2"));

			var col2 = res[@"c:\m\n"];
			Assert.AreEqual("n", col2.FolderName);
			Assert.AreEqual(@"c:\m\n", col2.FolderPath);
			Assert.IsTrue(col2.Objects.Contains("3"));

			var col3 = res[@"c:\"];
			Assert.AreEqual(@"c:\", col3.FolderName);
			Assert.AreEqual(@"c:\", col3.FolderPath);
			Assert.IsTrue(col3.Objects.Contains("4"));

		}


		[TestMethod]
		public void TestCast()
		{
			var list = new List<FileMetadata> {
				new FileMetadata(),
				new FileMetadata(),
			};

			var res = list.Cast<ObjectIdAndPath>();

			Assert.AreEqual(2, res.Count());
			Assert.AreEqual(list.First(), res.First());
			Assert.AreEqual(list.Last(), res.Last());
		}
	}
}

