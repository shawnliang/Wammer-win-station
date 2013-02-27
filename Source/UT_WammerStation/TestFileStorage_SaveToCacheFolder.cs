using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Waveface.Stream.Model;

namespace UT_WammerStation
{
	[TestClass]
	public class TestFileStorage_SaveToCacheFolder
	{
		string wfFolder;

		[TestInitialize]
		public void setup()
		{
			wfFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Waveface");

			if (Directory.Exists(wfFolder))
				Directory.Delete(wfFolder, true);

			Assert.IsFalse(Directory.Exists(wfFolder));
		}

		[TestMethod]
		public void GetCacheFodler()
		{
			var result = FileStorage.GetCachePath("user_id1");

			Assert.AreEqual(Path.Combine(wfFolder, @"aostream\cache\user_id1"), result);
		}

		[TestMethod]
		public void SaveToCacheFolder()
		{
			byte[] data = new byte[] { 1, 2, 3, 4, 5, 6 };
			FileStorage.SaveToCacheFolder("user_id", "123.dat", new ArraySegment<byte>(data));

			var saved = File.ReadAllBytes(Path.Combine(FileStorage.GetCachePath("user_id"), "123.dat"));

			Assert.AreEqual(6, saved.Length);
		}
	}
}
