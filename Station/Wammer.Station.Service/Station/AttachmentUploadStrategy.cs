using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	interface IAttachmentUploadStrategy
	{
		void Execute(Attachment file, ImageMeta meta, NameValueCollection Parameters, Driver driver, string savedName, AttachmentUploadHandler handler, FileStorage storage);
	}


	abstract class AbstractImageUploadStrategy : IAttachmentUploadStrategy
	{
		protected AttachmentUploadHandler handler;
		protected Driver driver;

		public void Execute(Attachment file, ImageMeta meta, NameValueCollection Parameters, Driver driver, string savedName, AttachmentUploadHandler handler, FileStorage storage)
		{
			this.handler = handler;
			this.driver = driver;


			file.file_size = file.RawData.Count;
			file.modify_time = DateTime.UtcNow;
			file.url = "/v2/attachments/view/?object_id=" + file.object_id;
			file.saved_file_name = savedName;

			BeforeSaveAttachment(file, Parameters, meta);
			storage.SaveFile(savedName, file.RawData);
			SaveAttachmentInfoToDB(file, meta, savedName);

			AttachmentEventArgs attachmentEvtArg = new AttachmentEventArgs(
				file.object_id, driver.user_id, Parameters["apikey"], Parameters["session_token"]);

			ImageAttachmentEventArgs imgAttachmentEvtArg = new ImageAttachmentEventArgs(
				file.object_id, driver.user_id, Parameters["apikey"], Parameters["session_token"], meta);
			
			if (meta == ImageMeta.Origin)
				handler.OnBodyAttachmentSaved(attachmentEvtArg);

			handler.OnImageAttachmentSaved(imgAttachmentEvtArg);

			HttpHelper.RespondSuccess(handler.Response, ObjectUploadResponse.CreateSuccess(file.object_id));

			handler.OnImageAttachmentCompleted(imgAttachmentEvtArg);
		}

		private void SaveAttachmentInfoToDB(Attachment file, ImageMeta meta, string savedName)
		{

			BsonDocument dbDoc = CreateDbDocument(file, meta, savedName);
			BsonDocument existDoc = AttachmentCollection.Instance.FindOneAs<BsonDocument>(
													new QueryDocument("_id", file.object_id));
			if (existDoc != null)
			{
				existDoc.DeepMerge(dbDoc);
				AttachmentCollection.Instance.Save(existDoc);
			}
			else
				AttachmentCollection.Instance.Save(dbDoc);
		}

		abstract protected BsonDocument CreateDbDocument(Attachment file, ImageMeta meta, string savedName);
		virtual protected void BeforeSaveAttachment(Attachment file, NameValueCollection Parameters, ImageMeta meta)
		{
		}
	}

	class ImageThumbnailUploadStrategy : AbstractImageUploadStrategy
	{
		protected override void  BeforeSaveAttachment(Attachment file, NameValueCollection Parameters, ImageMeta meta)
		{
			file.saved_file_name = null; // this is thumbnail, so don't modify saved_file_name of original image
			file.Upload(meta, Parameters["apikey"], Parameters["session_token"], 1024, (sender, e) => 
			{
				handler.OnThumbnailUpstreamed(new ThumbnailUpstreamedEventArgs(long.Parse(e.UserState.ToString())));
			});			
		}

		protected override BsonDocument  CreateDbDocument(Attachment file, ImageMeta meta, string savedName)
		{
			using (Bitmap img = new Bitmap(new MemoryStream(file.RawData.Array, file.RawData.Offset, file.RawData.Count)))
			{
				Attachment thumbnailAttachment = new Attachment(file);
				thumbnailAttachment.image_meta = new ImageProperty();
				thumbnailAttachment.image_meta.SetThumbnailInfo(meta,
					new ThumbnailInfo
					{
						mime_type = file.mime_type,
						modify_time = DateTime.UtcNow,
						url = file.url + "&image_meta=" + meta.ToString().ToLower(),
						file_size = file.file_size,
						file_name = savedName,
						width = img.Width,
						height = img.Height,
						saved_file_name = savedName
					});

				thumbnailAttachment.mime_type = null;
				thumbnailAttachment.modify_time = DateTime.UtcNow;
				thumbnailAttachment.url = null;
				thumbnailAttachment.file_size = 0;
				return thumbnailAttachment.ToBsonDocument();
			}
		}
	}

	class OldOriginImageUploadStrategy : AbstractImageUploadStrategy
	{
		protected override BsonDocument CreateDbDocument(Attachment file, ImageMeta meta, string savedName)
		{
			return file.ToBsonDocument();
		}
	}

	class NewOriginalImageUploadStrategy : AbstractImageUploadStrategy
	{
		protected override void BeforeSaveAttachment(Attachment file, NameValueCollection Parameters, ImageMeta meta)
		{
			// Upload to cloud and then save to local to ensure cloud post API
			// can process post with attachments correctly.
			// In the future when station is able to handle post and sync data with cloud
			// this step is not necessary
			int imgWidth, imgHeight;
			ThumbnailInfo medium;

			using (Bitmap imageBitmap = new Bitmap(new MemoryStream(file.RawData.Array, file.RawData.Offset, file.RawData.Count)))
			{
				imgWidth = imageBitmap.Width;
				imgHeight = imageBitmap.Height;

				file.Orientation = ImageHelper.ImageOrientation(imageBitmap);

				medium = ImagePostProcessing.MakeThumbnail(
				   imageBitmap, ImageMeta.Medium, file.Orientation, file.object_id, driver, file.file_name);


				// medium is made from original attachment file. Set their modify_time identical to 
				// prevent ImagePostProcessing task re-generates a medium thumbnail.
				medium.modify_time = file.modify_time;
			}

			Attachment thumb = new Attachment(file);
			thumb.RawData = new ArraySegment<byte>(medium.RawData);
			thumb.file_size = medium.file_size;
			thumb.mime_type = medium.mime_type;
			thumb.Upload(ImageMeta.Medium, Parameters["apikey"], Parameters["session_token"] ,1024, (sender, e) => 
			{
				handler.OnThumbnailUpstreamed(new ThumbnailUpstreamedEventArgs(long.Parse(e.UserState.ToString())));
			});			

			file.image_meta = new ImageProperty
			{
				medium = medium,
				width = imgWidth,
				height = imgHeight
			};
		}

		protected override BsonDocument CreateDbDocument(Attachment file, ImageMeta meta, string savedName)
		{
			return file.ToBsonDocument();
		}
	}

	class DocumentUploadStrategy : IAttachmentUploadStrategy
	{

		public void Execute(Attachment file, ImageMeta meta, NameValueCollection Parameters, Driver driver, string savedName, AttachmentUploadHandler handler, FileStorage storage)
		{
			file.file_size = file.RawData.Count;
			file.modify_time = DateTime.UtcNow;
			file.url = "/v2/attachments/view/?object_id=" + file.object_id;
			file.saved_file_name = savedName;

			file.Upload(meta, Parameters["apikey"], Parameters["session_token"]);
			storage.SaveAttachment(file);

			BsonDocument dbDoc = CreateDbDocument(file, meta, savedName);
			BsonDocument existDoc = AttachmentCollection.Instance.FindOneAs<BsonDocument>(
													new QueryDocument("_id", file.object_id));
			if (existDoc != null)
			{
				existDoc.DeepMerge(dbDoc);
				AttachmentCollection.Instance.Save(existDoc);
			}
			else
				AttachmentCollection.Instance.Save(dbDoc);

			AttachmentEventArgs aEvtArgs = new AttachmentEventArgs(
				file.object_id, driver.user_id, Parameters["apikey"], Parameters["session_token"]);

			handler.OnBodyAttachmentSaved(aEvtArgs);
			HttpHelper.RespondSuccess(handler.Response, ObjectUploadResponse.CreateSuccess(file.object_id));
		}

		private static BsonDocument CreateDbDocument(Attachment file, ImageMeta meta, string savedName)
		{
			return file.ToBsonDocument();
		}
	}
}

