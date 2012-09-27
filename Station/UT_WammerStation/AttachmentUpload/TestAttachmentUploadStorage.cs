using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wammer.Station.AttachmentUpload;

namespace UT_WammerStation.AttachmentUpload
{
	[TestClass]
	public class TestAttachmentUploadStorage
	{
		private ArraySegment<byte> raw_data;
		private Wammer.Model.Driver user;

		private const string TEXT = "this is my data";

		[TestInitialize]
		public void Setup()
		{
			using (var m = new MemoryStream())
			using (var w = new StreamWriter(m))
			{
				w.Write(TEXT);
				w.Flush();

				raw_data = new ArraySegment<byte>(m.ToArray());
			}

			if (!System.IO.Directory.Exists("user1"))
				System.IO.Directory.CreateDirectory("user1");

			user = new Wammer.Model.Driver { folder = "user1" };
		}

		[TestMethod]
		public void SaveOriginalAttachmentToResourceFolder()
		{
			Mock<IAttachmentUploadStorageDB> db = new Mock<IAttachmentUploadStorageDB>(MockBehavior.Strict);
			db.Setup(x => x.GetUserByGroupId("group1")).Returns(user);

			AttachmentUploadStorage storage = new AttachmentUploadStorage(db.Object);
			var result = storage.Save(
				new UploadData
				{
					group_id = "group1",
					imageMeta = Wammer.Model.ImageMeta.Origin,
					raw_data = raw_data,
					file_name = "file1.jpg",
					object_id = "obj1",
				});

			Assert.AreEqual("obj1.jpg", result.RelativePath);
			Assert.IsTrue(result.StorageBasePath.EndsWith(@"user1"));
			Assert.IsTrue(result.FullPath.EndsWith(@"user1\obj1.jpg"));

			using (var r = new StreamReader(result.FullPath))
			{
				Assert.AreEqual(TEXT, r.ReadToEnd());
			}
		}

		[TestMethod]
		public void SaveOriginalAttachmentHasNoSuffix()
		{
			Mock<IAttachmentUploadStorageDB> db = new Mock<IAttachmentUploadStorageDB>(MockBehavior.Strict);
			db.Setup(x => x.GetUserByGroupId("group1")).Returns(user);

			AttachmentUploadStorage storage = new AttachmentUploadStorage(db.Object);
			var result = storage.Save(
				new UploadData
				{
					group_id = "group1",
					imageMeta = Wammer.Model.ImageMeta.Origin,
					raw_data = raw_data,
					file_name = "file1",
					object_id = "obj1",
				});

			Assert.AreEqual("obj1", result.RelativePath);
			Assert.IsTrue(result.StorageBasePath.EndsWith(@"user1"));
			Assert.IsTrue(result.FullPath.EndsWith(@"user1\obj1"));

			using (var r = new StreamReader(result.FullPath))
			{
				Assert.AreEqual(TEXT, r.ReadToEnd());
			}
		}

		[TestMethod]
		public void SaveMediumAttachmentToResourceFolder()
		{
			Mock<IAttachmentUploadStorageDB> db = new Mock<IAttachmentUploadStorageDB>(MockBehavior.Strict);
			db.Setup(x => x.GetUserByGroupId("group1")).Returns(user);

			AttachmentUploadStorage storage = new AttachmentUploadStorage(db.Object);
			var result = storage.Save(
				new UploadData
				{
					group_id = "group1",
					imageMeta = Wammer.Model.ImageMeta.Medium,
					raw_data = raw_data,
					file_name = "file1.jpg",
					object_id = "obj1",
				});

			Assert.AreEqual("obj1_medium.dat", result.RelativePath);
			Assert.IsTrue(result.StorageBasePath.EndsWith(@"user1"));
			Assert.IsTrue(result.FullPath.EndsWith(@"user1\obj1_medium.dat"));

			using (var r = new StreamReader(result.FullPath))
			{
				Assert.AreEqual(TEXT, r.ReadToEnd());
			}
		}
	}
}
