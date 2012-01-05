using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

using Wammer.MultiPart;
using Wammer.Cloud;
using Wammer.Utility;
using Wammer.Model;
using System.Drawing;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace Wammer.Station
{
	public class AttachmentUploadHandler : HttpHandler
	{
		/// <summary>
		/// Fired on the uploaded image attachment is saved
		/// </summary>
		public event EventHandler<ImageAttachmentEventArgs> ImageAttachmentSaved;
		/// <summary>
		/// Fired on the whole request processing is completed
		/// </summary>
		public event EventHandler<ImageAttachmentEventArgs> ImageAttachmentCompleted;
		/// <summary>
		/// Fired on the uploaded attachment is saved
		/// </summary>
		public event EventHandler<AttachmentEventArgs> AttachmentSaved;

		public event EventHandler<ThumbnailUpstreamedEventArgs> ThumbnailUpstreamed;

		public AttachmentUploadHandler()
			: base()
		{
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}

		protected override void HandleRequest()
		{
			Attachment file = GetFileFromMultiPartData();
			ImageMeta meta = GetImageMeta();
			bool isNewOrigImage = file.object_id == null && meta == ImageMeta.Origin;

			if (file.object_id == null)
			{
				file.object_id = Guid.NewGuid().ToString();
			}
			 
			if (Parameters["apikey"] == null || Parameters["session_token"] == null)
				throw new FormatException("apikey or session_token is missing");

			Driver driver = DriverCollection.Instance.FindOne(Query.ElemMatch("groups", Query.EQ("group_id", file.group_id)));
			if (driver==null)
				throw new FormatException("group_id is not assocaited with a registered user");

			string savedName = GetSavedFilename(file, meta);
			FileStorage storage = new FileStorage(driver);

			IAttachmentUploadStrategy handleStrategy = GetHandleStrategy(file, isNewOrigImage, meta);
			handleStrategy.Execute(file, meta, Parameters, driver, savedName, this);
		}

		private static IAttachmentUploadStrategy GetHandleStrategy(Attachment file, bool isNewOrigImage, ImageMeta meta)
		{
			if (isNewOrigImage)
				return new NewOriginalImageUploadStrategy();
			else if (file.type == AttachmentType.doc)
				return new DocumentUploadStrategy();
			else if (file.type == AttachmentType.image && meta != ImageMeta.Origin)
				return new ImageThumbnailUploadStrategy();
			else
				return new OldOriginImageUploadStrategy();
		}

		private static BsonDocument CreateDbDocument(Attachment file, ImageMeta meta,
																				string savedName)
		{
			if (meta == ImageMeta.None)
				return file.ToBsonDocument();

			// image 
			if (meta == ImageMeta.Origin)
			{
				return file.ToBsonDocument();
			}
			else
			{
				using (Bitmap img = new Bitmap(new MemoryStream(file.RawData)))
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

		private ImageMeta GetImageMeta()
		{
			string imageMeta = Parameters["image_meta"];
			ImageMeta meta;
			if (imageMeta == null)
				meta = ImageMeta.None;
			else
				meta = (ImageMeta)Enum.Parse(typeof(ImageMeta), imageMeta, true);
			return meta;
		}

		public void OnAttachmentSaved(AttachmentEventArgs evt)
		{
			EventHandler<AttachmentEventArgs> handler = AttachmentSaved;
			if (handler != null)
			{
				handler(this, evt);
			}
		}

		public void OnImageAttachmentSaved(ImageAttachmentEventArgs evt)
		{
			EventHandler<ImageAttachmentEventArgs> handler = ImageAttachmentSaved;
			if (handler != null)
			{
				handler(this, evt);
			}
		}

		public void OnImageAttachmentCompleted(ImageAttachmentEventArgs evt)
		{
			EventHandler<ImageAttachmentEventArgs> handler = ImageAttachmentCompleted;
			if (handler != null)
			{
				handler(this, evt);
			}
		}

		public void OnThumbnailUpstreamed(ThumbnailUpstreamedEventArgs evt)
		{
			EventHandler<ThumbnailUpstreamedEventArgs> handler = ThumbnailUpstreamed;
			if (handler != null)
			{
				handler(this, evt);
			}
		}

		private static string GetSavedFilename(Attachment file, ImageMeta meta)
		{
			string name = file.object_id;

			if (meta != ImageMeta.Origin && meta != ImageMeta.None)
			{
				name += "_" + meta.ToString().ToLower();
			}

			string originalSuffix = Path.GetExtension(file.file_name);
			if (originalSuffix != null)
			{
				return name + originalSuffix;
			}
			else
			{
				return name;
			}
		}

		private Attachment GetFileFromMultiPartData()
		{
			Attachment file = new Attachment();

			if (Files.Count == 0)
				throw new FormatException("No file is uploaded");

			file.object_id = Parameters["object_id"];
			file.RawData = Files[0].Data;
			file.file_name = Files[0].Name;
			file.mime_type = Files[0].ContentType;
			file.title = Parameters["title"];
			file.description = Parameters["description"];
			file.group_id = Parameters["group_id"];

			if (Parameters["type"]==null)
				throw new FormatException("type is missing in file upload multipart data");

			try
			{
				file.type = (AttachmentType)Enum.Parse(typeof(AttachmentType),
																			Parameters["type"], true);
			}
			catch (ArgumentException e)
			{
				throw new FormatException("Unknown attachment type: " + Parameters["type"], e);
			}

			if (file.file_name == null)
				throw new FormatException("filename is missing in file upload multipart data");

			if (file.RawData == null)
				throw new FormatException("file is missing in file upload multipart data");

			if (file.group_id == null)
				throw new FormatException("group_id is missing in file upload multipart data");

			return file;
		}
	}

	public class ImageAttachmentEventArgs : AttachmentEventArgs
	{
		public ImageMeta Meta { get; set; }
		public string UserApiKey { get; set; }
		public string UserSessionToken { get; set; }

		public ImageAttachmentEventArgs()
		{
		}
	}

	public class AttachmentEventArgs : EventArgs
	{
		public Attachment Attachment { get; set; }
		public Driver Driver { get; set; }

		public AttachmentEventArgs()
		{
		}
	}
}
