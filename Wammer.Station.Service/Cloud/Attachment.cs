using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

using Wammer.MultiPart;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Wammer.Cloud
{
	public enum ImageMeta
	{
		None = 0,
		/// <summary>
		/// 128x128 pixels
		/// </summary>
		Square = 128,
		/// <summary>
		/// 120 pixels
		/// </summary>
		Small = 120,
		/// <summary>
		/// 720 pixels
		/// </summary>
		Medium = 720,
		/// <summary>
		/// 1024 pixels
		/// </summary>
		Large = 1024,
		/// <summary>
		/// Original image size
		/// </summary>
		Origin = 50 * 1024 * 1024
	}

	public enum AttachmentType
	{
		image,
		doc
	}

	[BsonIgnoreExtraElements]
	public class ThumbnailInfo
	{
		[BsonIgnoreIfNull]
		public string url { get; set; }
		[BsonIgnoreIfNull]
		public int width { get; set; }
		[BsonIgnoreIfNull]
		public int height { get; set; }
		[BsonIgnoreIfNull]
		public string mime_type { get; set; }
		[BsonIgnoreIfNull]
		public DateTime modify_time { get; set; }
		[BsonIgnoreIfNull]
		public int file_size { get; set; }
		[BsonIgnoreIfNull]
		public string file_name { get; set; }

		[BsonIgnore]
		[System.Xml.Serialization.XmlIgnore]
		public byte[] RawData { get; set; }

		public bool ShouldSerializewidth()
		{
			return width > 0;
		}

		public bool ShouldSerializeheight()
		{
			return height > 0;
		}

		public bool ShouldSerializefile_size()
		{
			return file_size > 0;
		}
	}

	[BsonIgnoreExtraElements]
	public class ImageProperty
	{
		[BsonIgnoreIfNull]
		public int width { get; set; }
		[BsonIgnoreIfNull]
		public int height { get; set; }
		[BsonIgnoreIfNull]
		public ThumbnailInfo small { get; set; }
		[BsonIgnoreIfNull]
		public ThumbnailInfo medium { get; set; }
		[BsonIgnoreIfNull]
		public ThumbnailInfo large { get; set; }
		[BsonIgnoreIfNull]
		public ThumbnailInfo square { get; set; }

		public ThumbnailInfo GetThumbnailInfo(ImageMeta meta)
		{
			switch (meta)
			{
				case ImageMeta.Small:
					return small;
				case ImageMeta.Medium:
					return medium;
				case ImageMeta.Large:
					return large;
				case ImageMeta.Square:
					return square;
				default:
					throw new ArgumentException("meta is not a thumbnail: " + meta);
			}
		}


		public void SetThumbnailInfo(ImageMeta meta, ThumbnailInfo thumbnail)
		{
			switch (meta)
			{
				case ImageMeta.Small:
					small = thumbnail;
					break;
				case ImageMeta.Medium:
					medium = thumbnail;
					break;
				case ImageMeta.Large:
					large = thumbnail;
					break;
				case ImageMeta.Square:
					square = thumbnail;
					break;
				default:
					throw new ArgumentException("meta is not a thumbnail: " + meta);
			}
		}

		public bool ShouldSerializewidth()
		{
			return width > 0;
		}

		public bool ShouldSerializeheight()
		{
			return height > 0;
		}
	}

	[BsonIgnoreExtraElements]
	public class Attachment
	{
		#region Upload utility functions
		public static ObjectUploadResponse UploadImage(string url, byte[] imageData, string objectId,
												string fileName, string contentType, ImageMeta meta)
		{

			Dictionary<string, object> pars = new Dictionary<string, object>();
			pars["type"] = "image";
			pars["image_meta"] = meta.ToString().ToLower();
			pars["session_token"] = CloudServer.SessionToken;
			if (objectId != null)
				pars["object_id"] = objectId;
			pars["file"] = imageData;
			HttpWebResponse _webResponse = 
				Waveface.MultipartFormDataPostHelper.MultipartFormDataPost(
				url,
				"Mozilla 4.0+",
				pars,
				fileName,
				contentType);

			using (StreamReader reader = new StreamReader(_webResponse.GetResponseStream()))
			{
				return fastJSON.JSON.Instance.ToObject<ObjectUploadResponse>(reader.ReadToEnd());
			}
		}

		public static ObjectUploadResponse UploadImage(string url, byte[] imageData,
												string fileName, string contentType, ImageMeta meta)
		{
			return UploadImage(url, imageData, null, fileName, contentType, meta);
		}

		public static ObjectUploadResponse UploadImage(byte[] imageData, string objectId,
												string fileName, string contentType, ImageMeta meta)
		{
			string url = string.Format("http://{0}:{1}/{2}/attachments/upload/",
				Cloud.CloudServer.HostName,
				Cloud.CloudServer.Port,
				CloudServer.DEF_BASE_PATH);

			return UploadImage(url, imageData, objectId, fileName, contentType, meta);
		}
		#endregion

		[BsonId]
		public string object_id { get; set; }
		[BsonIgnoreIfNull]
		public string file_name { get; set; }
		[BsonIgnoreIfNull]
		public string mime_type { get; set; }
		[BsonIgnoreIfNull]
		public string title { get; set; }
		[BsonIgnoreIfNull]
		public string description { get; set; }
		[BsonIgnoreIfNull]
		public AttachmentType type { get; set; }
		[BsonIgnoreIfNull]
		public string url { get; set; }
		[BsonIgnoreIfNull]
		public string image { get; set; }
		[BsonIgnoreIfNull]
		public int file_size { get; set; }
		[BsonIgnoreIfNull]
		public DateTime modify_time { get; set; }
		[BsonIgnoreIfNull]
		public ImageProperty image_meta { get; set; }

		[BsonIgnore]
		[System.Xml.Serialization.XmlIgnore]
		public byte[] RawData { get; set; }

		public Attachment()
		{
		}

		public Attachment(Attachment lhs)
		{
			object_id = lhs.object_id;
			file_name = lhs.file_name;
			mime_type = lhs.mime_type;
			title = lhs.title;
			description = lhs.description;
			type = lhs.type;
			url = lhs.url;
			image = lhs.image;
			file_size = lhs.file_size;
			modify_time = lhs.modify_time;
			image_meta = lhs.image_meta;
			RawData = lhs.RawData;
		}

		public bool ShouldSerializefile_size()
		{
			return file_size > 0;
		}
	}

	public class AttachmentResponse: CloudResponse
	{
		public Attachment attachment { get; set; }

		public AttachmentResponse()
			:base(200, 0, "success")
		{
		}

		public AttachmentResponse(Attachment att)
			: base(200, 0, "success")
		{
			attachment = att;
		}
	}
}
