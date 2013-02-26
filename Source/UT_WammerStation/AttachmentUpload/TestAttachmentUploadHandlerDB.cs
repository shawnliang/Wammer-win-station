using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver.Builders;
using System;
using Wammer.Station.AttachmentUpload;
using Waveface.Stream.Model;

namespace UT_WammerStation.AttachmentUpload
{
	[TestClass]
	public class TestAttachmentUploadHandlerDB
	{
		[TestInitialize]
		public void SetUp()
		{
			AttachmentCollection.Instance.RemoveAll();
		}

		[TestMethod]
		public void TestAllFieldsAreSaved()
		{
			Attachment att = new Attachment
			{
				object_id = "id",
				mime_type = "mime",
				group_id = "group",
				saved_file_name = "saved_name",
				file_size = 123,
				file_name = "file_name",
				description = "desc",
				MD5 = "123",
				modify_time = DateTime.UtcNow,
				url = "url",
				title = "title",
				image = null,
				type = AttachmentType.image,
			};

			AttachmentUploadHandlerDB db = new AttachmentUploadHandlerDB();
			db.InsertOrMergeToExistingDoc(att);

			Attachment savedDoc = AttachmentCollection.Instance.FindOne(Query.EQ("_id", att.object_id));
			Assert.AreEqual(att.object_id, savedDoc.object_id);
			Assert.AreEqual(att.mime_type, savedDoc.mime_type);
			Assert.AreEqual(att.group_id, savedDoc.group_id);
			Assert.AreEqual(att.saved_file_name, savedDoc.saved_file_name);
			Assert.AreEqual(att.file_size, savedDoc.file_size);
			Assert.AreEqual(att.file_name, savedDoc.file_name);
			Assert.AreEqual(att.description, savedDoc.description);
			Assert.AreEqual(att.MD5, savedDoc.MD5);
			Assert.IsTrue(att.modify_time - savedDoc.modify_time < TimeSpan.FromSeconds(1));
			Assert.AreEqual(att.url, savedDoc.url);
			Assert.AreEqual(att.title, savedDoc.title);
			Assert.AreEqual(att.image, savedDoc.image);
			Assert.AreEqual(att.type, savedDoc.type);
		}

		[TestMethod]
		public void TestNullFieldsAreNotSaved()
		{
			Attachment att = new Attachment
			{
				object_id = "id",
			};

			AttachmentUploadHandlerDB db = new AttachmentUploadHandlerDB();
			db.InsertOrMergeToExistingDoc(att);

			Attachment savedDoc = AttachmentCollection.Instance.FindOne(Query.EQ("_id", att.object_id));
			Assert.AreEqual(att.object_id, savedDoc.object_id);
			Assert.IsNull(savedDoc.mime_type);
			Assert.IsNull(savedDoc.group_id);
			Assert.IsNull(savedDoc.saved_file_name);
			Assert.AreEqual(0, savedDoc.file_size);
			Assert.IsNull(savedDoc.file_name);
			Assert.IsNull(savedDoc.description);
			Assert.IsNull(savedDoc.MD5);
			Assert.AreEqual(DateTime.MinValue, savedDoc.modify_time);
			Assert.IsNull(savedDoc.url);
			Assert.IsNull(savedDoc.title);
			Assert.IsNull(savedDoc.image);
			Assert.AreEqual(AttachmentType.image, savedDoc.type);
		}

		[TestMethod]
		public void TestMergeAttachmentToExistingOne()
		{
			AttachmentCollection.Instance.Save(new Attachment
				{
					object_id = "id",
					image_meta = new ImageProperty
					{
						height = 100,
						small = new ThumbnailInfo
						{
							saved_file_name = "file"
						}
					}
				});

			Attachment att = new Attachment
			{
				object_id = "id",
				file_name = "orig",
				image_meta = new ImageProperty
				{
					width = 200,
					small = new ThumbnailInfo
					{
						file_size = 300,
					},
					medium = new ThumbnailInfo
					{
						file_name = "medium"
					}
				}
			};

			AttachmentUploadHandlerDB db = new AttachmentUploadHandlerDB();
			db.InsertOrMergeToExistingDoc(att);

			Attachment savedDoc = AttachmentCollection.Instance.FindOne(Query.EQ("_id", att.object_id));
			Assert.AreEqual(att.file_name, savedDoc.file_name);
			Assert.AreEqual(100, savedDoc.image_meta.height);
			Assert.AreEqual(200, savedDoc.image_meta.width);
			Assert.AreEqual("file", savedDoc.image_meta.small.saved_file_name);
			Assert.AreEqual(300, savedDoc.image_meta.small.file_size);
			Assert.AreEqual("medium", savedDoc.image_meta.medium.file_name);
		}
	}
}
