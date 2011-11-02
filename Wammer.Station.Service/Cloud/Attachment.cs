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
		public string url { get; set; }
		public int width { get; set; }
		public int height { get; set; }
		public string mime_type { get; set; }
		public DateTime modify_time { get; set; }
		public int file_size { get; set; }
	}

	[BsonIgnoreExtraElements]
	public class ImageProperty
	{
		public ThumbnailInfo small { get; set; }
		public ThumbnailInfo medium { get; set; }
		public ThumbnailInfo large { get; set; }
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
					throw new FileNotFoundException();
			}
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
			HttpWebResponse _webResponse = Waveface.MultipartFormDataPostHelper.MultipartFormDataPost(
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

		//[BsonId(IdGenerator=typeof(GuidGenerator))]
		//[BsonElement("object_id")]
		[BsonId]
		public string object_id { get; set; }
		public string file_name { get; set; }
		public string mime_type { get; set; }
		public string title { get; set; }
		public string description { get; set; }
		public AttachmentType type { get; set; }
		public string url { get; set; }
		public string image { get; set; }
		public int file_size { get; set; }
		public DateTime modify_time { get; set; }
		public ImageProperty image_meta { get; set; }

		[BsonIgnore]
		[System.Xml.Serialization.XmlIgnore]
		public byte[] RawData { get; set; }

		public Attachment()
		{
		}
	}
}
