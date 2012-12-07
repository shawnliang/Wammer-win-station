using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using Wammer.Model;
using Wammer.Station;
using Wammer.Station.AttachmentView;

namespace UT_WammerStation.AttachmentViewTest
{
	/// <summary>
	/// Summary description for TestViewStorage
	/// </summary>
	[TestClass]
	public class TestViewStorage
	{
		private Driver user = new Driver { folder = "user1", user_id = "uuuu" };
		private string mediumPath;

		[TestInitialize]
		public void setUp()
		{
			var bytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes("1234567890"));
			mediumPath = FileStorage.SaveToCacheFolder(user.user_id, "obj1.dat", bytes);

			var storage = new FileStorage(user);
			storage.TrySaveFile("file1.jpg", bytes);
		}

		[TestMethod]
		public void GetOriginalStream()
		{
			var storage = new AttachmentViewStorage();

			using (var s = storage.GetAttachmentStream(ImageMeta.Origin, user, "file1.jpg"))
			using (var r = new StreamReader(s))
			{
				Assert.AreEqual("1234567890", r.ReadToEnd());
			}
		}

		[TestMethod]
		public void GetThumbnail()
		{
			var storage = new AttachmentViewStorage();

			using (var s = storage.GetAttachmentStream(ImageMeta.Medium, user, mediumPath))
			using (var r = new StreamReader(s))
			{
				Assert.AreEqual("1234567890", r.ReadToEnd());
			}
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void GetNoFile_medium()
		{
			var storage = new AttachmentViewStorage();

			storage.GetAttachmentStream(ImageMeta.Medium, user, "123.dat");
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void GetNoFile_Origin()
		{
			var storage = new AttachmentViewStorage();

			storage.GetAttachmentStream(ImageMeta.Origin, user, "123.dat");
		}
	}
}
