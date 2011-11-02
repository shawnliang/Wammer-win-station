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

			if (file.object_id == null)
			{
				file.object_id = Guid.NewGuid().ToString();
			}

			string savedName = GetSavedFilename(file, meta);
			storage.Save(savedName, file.RawData);
			
			ImageAttachmentEventArgs evtArgs = new ImageAttachmentEventArgs(file, meta,
																		this.attachmentCollection);

			BsonDocument dbDoc = GetBsonDocument(file, meta, savedName);

			BsonDocument existDoc = this.attachmentCollection.FindOne(
													new QueryDocument("object_id", file.object_id));
			if (existDoc != null)
			{
				existDoc.DeepMerge(dbDoc);
				this.attachmentCollection.Save(existDoc);
			}
			else
				this.attachmentCollection.Insert(dbDoc);


			if (file.type == AttachmentType.image)
				OnImageAttachmentSaved(evtArgs);

			ObjectUploadResponse json = ObjectUploadResponse.CreateSuccess(file.object_id);
			HttpHelper.RespondSuccess(Response, json);

			if (file.type == AttachmentType.image)
				OnImageAttachmentCompleted(evtArgs);
		}

		private static BsonDocument GetBsonDocument(Attachment file, ImageMeta meta, string savedName)
		{
			BsonDocument dbDoc = new BsonDocument()
						 .Add("object_id", file.object_id)
						 .Add("type", file.type.ToString().ToLower());

			dbDoc.Add("title", file.title);
			dbDoc.Add("description", file.description);


			if (meta != ImageMeta.None)
			{
				if (meta == ImageMeta.Origin)
				{
					dbDoc.Add("url", StationInfo.BaseURL + "attachments/view/?object_id=" +
																					file.object_id)
						.Add("mime_type", file.mime_type)
						.Add("file_size", file.RawData.Length);
				}
				else
				{
					BsonDocument metaDoc = new BsonDocument()
						.Add("url", string.Format("{0}attachments/view/?object_id={1}&image_meta={2}",
							StationInfo.BaseURL, file.object_id, meta.ToString().ToLower()))
						.Add("file_name", savedName)
						.Add("modify_time", TimeHelper.GetMillisecondsSince1970())
						.Add("file_size", file.RawData.Length)
						.Add("mime_type", file.mime_type);

					dbDoc.Add("image_meta", new BsonDocument().Add(meta.ToString().ToLower(), metaDoc));
				}
			}
			return dbDoc;
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

			file.object_id = Parameters["object_id"];
			file.RawData = Files[0].Data;
			file.file_name = Files[0].Name;
			file.mime_type = Files[0].ContentType;
			file.title = Parameters["title"];
			file.description = Parameters["description"];

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

			return file;
		}
	}

	public class ImageAttachmentEventArgs : AttachmentEventArgs
	{
		public ImageMeta Meta { get; private set; }
		public MongoCollection<BsonDocument> DbDocs { get; set; }

		public ImageAttachmentEventArgs(Attachment attachment, ImageMeta meta, MongoCollection<BsonDocument> dbDocs)
			:base(attachment)
		{
			this.Meta = meta;
			this.DbDocs = dbDocs;
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
