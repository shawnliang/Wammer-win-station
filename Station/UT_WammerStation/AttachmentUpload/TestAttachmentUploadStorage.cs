﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wammer.Station.AttachmentUpload;
using Wammer.Utility;

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

			user = new Wammer.Model.Driver { folder = "user1", user_id = "uuuu" };

			var userRes = Path.Combine(Wammer.Station.FileStorage.ResourceFolder, "user1");
			if (Directory.Exists(userRes))
				Directory.Delete(userRes, true);
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
					file_create_time = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc)
				}, "");

			Assert.AreEqual(@"2000\01\02\file1.jpg", result.RelativePath);
			Assert.IsTrue(result.StorageBasePath.EndsWith(@"user1"));
			Assert.IsTrue(result.FullPath.EndsWith(@"user1\2000\01\02\file1.jpg"));

			using (var r = new StreamReader(result.FullPath))
			{
				Assert.AreEqual(TEXT, r.ReadToEnd());
			}
		}


		[TestMethod]
		public void SaveOriginalAttachmentToResourceFolder_useTakenTime_IS8601()
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
					file_create_time = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc)
				},
				new DateTime(1999, 9, 7, 23, 30, 40, DateTimeKind.Utc).ToCloudTimeString());

			Assert.AreEqual(@"1999\09\08\file1.jpg", result.RelativePath);
			Assert.IsTrue(result.StorageBasePath.EndsWith(@"user1"));
			Assert.IsTrue(result.FullPath.EndsWith(@"user1\1999\09\08\file1.jpg"));

			using (var r = new StreamReader(result.FullPath))
			{
				Assert.AreEqual(TEXT, r.ReadToEnd());
			}
		}

		[TestMethod]
		public void SaveOriginalAttachmentToResourceFolder_useTakenTime_GeneralFormat()
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
					file_create_time = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc)
				}, "1999:09:08 10:10:10");

			Assert.AreEqual(@"1999\09\08\file1.jpg", result.RelativePath);
			Assert.IsTrue(result.StorageBasePath.EndsWith(@"user1"));
			Assert.IsTrue(result.FullPath.EndsWith(@"user1\1999\09\08\file1.jpg"));

			using (var r = new StreamReader(result.FullPath))
			{
				Assert.AreEqual(TEXT, r.ReadToEnd());
			}
		}

		[TestMethod]
		public void SaveOriginalAttachmentToResourceFolder_sameFileName()
		{
			// prepare env
			var store = new Wammer.Station.FileStorage(user);
			store.TrySaveFile(@"1999\09\08\file1.jpg", raw_data);
			store.TrySaveFile(@"1999\09\08\file1.jpg", raw_data);
			store.TrySaveFile(@"1999\09\08\file1.jpg", raw_data);



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
					file_create_time = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc)
				},
				new DateTime(1999, 9, 7, 23, 30, 40, DateTimeKind.Utc).ToCloudTimeString());

			Assert.AreEqual(@"1999\09\08\file1 (3).jpg", result.RelativePath);
			Assert.IsTrue(result.StorageBasePath.EndsWith(@"user1"));
			Assert.IsTrue(result.FullPath.EndsWith(@"user1\1999\09\08\file1 (3).jpg"));

			using (var r = new StreamReader(result.FullPath))
			{
				Assert.AreEqual(TEXT, r.ReadToEnd());
			}
		}

		[TestMethod]
		public void SaveOriginalAttachmentToResourceFolder_importTime()
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
					file_create_time = null
				},
				null);

			var date = DateTime.Now.ToString(@"yyyy\\MM\\dd");

			Assert.AreEqual(date + @"\file1.jpg", result.RelativePath);
			Assert.IsTrue(result.StorageBasePath.EndsWith(@"user1"));
			Assert.IsTrue(result.FullPath.EndsWith(@"user1\" + date + @"\file1.jpg"));

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
					file_create_time = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc)
				}, "");

			Assert.AreEqual(@"2000\01\02\file1", result.RelativePath);
			Assert.IsTrue(result.StorageBasePath.EndsWith(@"user1"));
			Assert.IsTrue(result.FullPath.EndsWith(@"user1\2000\01\02\file1"));

			using (var r = new StreamReader(result.FullPath))
			{
				Assert.AreEqual(TEXT, r.ReadToEnd());
			}
		}

		[TestMethod]
		public void SaveMediumAttachmentToCacheFolder()
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
					file_create_time = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc)
				}, "");

			Assert.AreEqual(@"cache\uuuu\obj1_medium.dat", result.RelativePath);
			Assert.AreEqual("", result.StorageBasePath);
			Assert.AreEqual(@"cache\uuuu\obj1_medium.dat", result.FullPath);

			using (var r = new StreamReader(result.FullPath))
			{
				Assert.AreEqual(TEXT, r.ReadToEnd());
			}
		}
	}
}