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
			Attachments file = GetFileFromMultiPartData();
			ImageMeta meta = GetImageMeta();
			bool isNewOrigImage = file.object_id == null && meta == ImageMeta.Origin;

			if (file.object_id == null)
			{
				file.object_id = Guid.NewGuid().ToString();
			}
			 
			if (Parameters["apikey"] == null || Parameters["session_token"] == null)
				throw new FormatException("apikey or session_token is missing");
			
			Drivers driver = Drivers.collection.FindOne(Query.ElemMatch("groups", Query.EQ("group_id", file.group_id)));
			if (driver==null)
				throw new FormatException("group_id is not assocaited with a registered user");

			string savedName = GetSavedFilename(file, meta);

			// Upload to cloud and then save to local to ensure cloud post API
			// can process post with attachments correctly.
			// In the future when station is able to handle post and sync data with cloud
			// this step is not necessary
			if (isNewOrigImage)
			{
				file.Bitmap = new Bitmap(new MemoryStream(file.RawData));
				ThumbnailInfo medium = ImagePostProcessing.MakeThumbnail(
									file.Bitmap, ImageMeta.Medium, file.object_id, driver.folder);
				Attachments thumb = new Attachments(file);
				thumb.RawData = medium.RawData;
				thumb.file_size = medium.file_size;
				thumb.mime_type = medium.mime_type;
				thumb.Upload(ImageMeta.Medium, Parameters["apikey"], Parameters["session_token"]);

				file.image_meta = new ImageProperty
				{
					medium = medium,
					width = file.Bitmap.Width,
					height = file.Bitmap.Height
				};
				file.saved_file_name = savedName;
			}
			else if (file.type == AttachmentType.doc)
			{
				file.saved_file_name = savedName;
				file.Upload(meta, Parameters["apikey"], Parameters["session_token"]);
			}
			else if (file.type == AttachmentType.image && meta != ImageMeta.Origin)
			{
				file.Upload(meta, Parameters["apikey"], Parameters["session_token"]);
			}


			FileStorage storage = new FileStorage(driver.folder);
			storage.Save(savedName, file.RawData);
			file.file_size = file.RawData.Length;
			file.modify_time = DateTime.UtcNow;
			file.url = "/v2/attachments/view/?object_id=" + file.object_id;

			AttachmentEventArgs aEvtArgs = new AttachmentEventArgs
			{
				Attachment = file,
				FolderPath = driver.folder,
				DriverId = driver.user_id
			};

			ImageAttachmentEventArgs evtArgs = new ImageAttachmentEventArgs
			{
				Attachment = file,
				DbDocs = Attachments.collection,
				Meta = meta,
				UserApiKey = Parameters["apikey"],
				UserSessionToken = Parameters["session_token"],
				FolderPath = driver.folder
			};

			BsonDocument dbDoc = CreateDbDocument(file, meta, savedName);
			BsonDocument existDoc = Attachments.collection.FindOneAs<BsonDocument>(
													new QueryDocument("_id", file.object_id));
			if (existDoc != null)
			{
				existDoc.DeepMerge(dbDoc);
				Attachments.collection.Save(existDoc);
			}
			else
				Attachments.collection.Insert(dbDoc);

			OnAttachmentSaved(aEvtArgs);

			if (file.type == AttachmentType.image)
				OnImageAttachmentSaved(evtArgs);

			ObjectUploadResponse json = ObjectUploadResponse.CreateSuccess(file.object_id);
			HttpHelper.RespondSuccess(Response, json);

			if (file.type == AttachmentType.image)
				OnImageAttachmentCompleted(evtArgs);
		}

		private static BsonDocument CreateDbDocument(Attachments file, ImageMeta meta,
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
					Attachments thumbnailAttachment = new Attachments(file);
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
							height = img.Height
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

		protected void OnAttachmentSaved(AttachmentEventArgs evt)
		{
			EventHandler<AttachmentEventArgs> handler = AttachmentSaved;
			if (handler != null)
			{
				handler(this, evt);
			}
		}

		protected void OnImageAttachmentSaved(ImageAttachmentEventArgs evt)
		{
			EventHandler<ImageAttachmentEventArgs> handler = ImageAttachmentSaved;
			if (handler != null)
			{
				handler(this, evt);
			}
		}

		protected void OnImageAttachmentCompleted(ImageAttachmentEventArgs evt)
		{
			EventHandler<ImageAttachmentEventArgs> handler = ImageAttachmentCompleted;
			if (handler != null)
			{
				handler(this, evt);
			}
		}

		private static string GetSavedFilename(Attachments file, ImageMeta meta)
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

		private Attachments GetFileFromMultiPartData()
		{
			Attachments file = new Attachments();

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
		public MongoCollection DbDocs { get; set; }
		public string UserApiKey { get; set; }
		public string UserSessionToken { get; set; }
		public string FolderPath { get; set; }

		public ImageAttachmentEventArgs()
		{
		}
	}

	public class AttachmentEventArgs : EventArgs
	{
		public Attachments Attachment { get; set; }
		public string FolderPath { get; set; }
		public string DriverId { get; set; }

		public AttachmentEventArgs()
		{
		}
	}
}
