using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

using Wammer.MultiPart;
using Wammer.Cloud;
using Wammer.Utility;

using MongoDB.Driver;
using MongoDB.Bson;

namespace Wammer.Station
{
	public class ObjectUploadHandler : HttpHandler
	{
		private FileStorage storage;
		private MongoServer mongodb;
		private MongoCollection<BsonDocument> attachmentCollection;

		/// <summary>
		/// Fired on the uploaded attachment is saved
		/// </summary>
		public event EventHandler<ImageAttachmentEventArgs> ImageAttachmentSaved;
		/// <summary>
		/// Fired on the whole request processing is completed
		/// </summary>
		public event EventHandler<ImageAttachmentEventArgs> ImageAttachmentCompleted;

		public ObjectUploadHandler(FileStorage fileStore, MongoServer mongodb)
			: base()
		{
			this.storage = fileStore;
			this.mongodb = mongodb;

			MongoDatabase db = mongodb.GetDatabase("wammer");
			if (!db.CollectionExists("attachments"))
				db.CreateCollection("attachments");

			this.attachmentCollection = db.GetCollection<BsonDocument>("attachments");
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}

		protected override void HandleRequest()
		{
			Attachment file = GetFileFromMultiPartData();
			ImageMeta meta = GetImageMeta();

			if (file.ObjectId == null)
			{
				file.ObjectId = Guid.NewGuid().ToString();
				file.IdCreatedByStation = true;
			}

			string savedName = GetSavedFilename(file, meta);
			storage.Save(savedName, file.RawData);
			
			ImageAttachmentEventArgs evtArgs = new ImageAttachmentEventArgs(file, meta);

			BsonDocument dbDoc = new BsonDocument()
				.Add("object_id", file.ObjectId)
				.Add("title", file.Title)
				.Add("description", file.Description)
				.Add("type", file.Kind.ToString().ToLower());

			if (meta == ImageMeta.Origin)
			{
				dbDoc.Add("url", string.Format("http://{0}:9981/v2/attachments/view/?object_id={1}",
															StationInfo.IPv4Address, file.ObjectId))
					.Add("meme_type", file.ContentType)
					.Add("file_size", file.RawData.Length);
			}
			else 
			{
				BsonDocument metaDoc = new BsonDocument()
					.Add("url", string.Format("http://{0}:9981/v2/attachments/view/?object_id={1}" +
														"&image_meta={2}", StationInfo.IPv4Address, 
																					file.ObjectId,
																		meta.ToString().ToLower()))
					.Add("file_name", savedName)
					.Add("width", 0)
					.Add("height", 0)
					.Add("modify_time", TimeHelper.GetMillisecondsSince1970())
					.Add("file_size", file.RawData.Length)
					.Add("mime_type", file.ContentType);

				dbDoc.Add("image_meta", new BsonDocument().Add(meta.ToString().ToLower(), metaDoc));
			}

			this.attachmentCollection.Insert(dbDoc);


			if (file.Kind == AttachmentType.image)
				OnImageAttachmentSaved(evtArgs);

			ObjectUploadResponse json = ObjectUploadResponse.CreateSuccess(file.ObjectId);
			HttpHelper.RespondSuccess(Response, json);

			if (file.Kind == AttachmentType.image)
				OnImageAttachmentCompleted(evtArgs);
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

		private static string GetSavedFilename(Attachment file, ImageMeta meta)
		{
			string name = file.ObjectId;

			if (meta != ImageMeta.Origin && meta != ImageMeta.None)
			{
				name += "_" + meta.ToString().ToLower();
			}

			string originalSuffix = Path.GetExtension(file.Filename);
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

			file.ObjectId = Parameters["object_id"];
			file.RawData = Files[0].Data;
			file.Filename = Files[0].Name;
			file.ContentType = Files[0].ContentType;
			file.Title = Parameters["title"];
			file.Description = Parameters["description"];

			if (Parameters["type"]==null)
				throw new FormatException("type is missing in file upload multipart data");

			try
			{
				file.Kind = (AttachmentType)Enum.Parse(typeof(AttachmentType),
																			Parameters["type"], true);
			}
			catch (ArgumentException e)
			{
				throw new FormatException("Unknown attachment type: " + Parameters["type"], e);
			}

			if (file.Filename == null)
				throw new FormatException("filename is missing in file upload multipart data");

			if (file.RawData == null)
				throw new FormatException("file is missing in file upload multipart data");

			return file;
		}
	}

	public class ImageAttachmentEventArgs : AttachmentEventArgs
	{
		public ImageMeta Meta { get; private set; }

		public ImageAttachmentEventArgs(Attachment attachment, ImageMeta meta)
			:base(attachment)
		{
			this.Meta = meta;
		}
	}

	public class AttachmentEventArgs : EventArgs
	{
		public Attachment Attachment { get; private set; }

		public AttachmentEventArgs(Attachment attachment)
			: base()
		{
			this.Attachment = attachment;
		}
	}
}
