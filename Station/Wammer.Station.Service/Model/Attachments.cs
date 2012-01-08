using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;

using Wammer.Cloud;
using Wammer.MultiPart;
using Wammer.Station;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace Wammer.Model
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
		Medium = 512,
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
		public long file_size { get; set; }
		[BsonIgnoreIfNull]
		public string file_name { get; set; }
		[BsonIgnoreIfNull]
		public string md5 { get; set; }
		[BsonIgnoreIfNull]
		public string saved_file_name { get; set; }

		[BsonIgnore]
		[System.Xml.Serialization.XmlIgnore]
		public byte[] RawData
		{
			get { return rawData; }
			set
			{
				rawData = value;
				if (rawData != null)
				{
					using (MD5 md5 = MD5.Create())
					{
						byte[] hash = md5.ComputeHash(rawData);
						StringBuilder buff = new StringBuilder();
						for (int i = 0; i < hash.Length; i++)
							buff.Append(hash[i].ToString("x2"));

						this.md5 = buff.ToString();
					}
				}
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

		public bool ShouldSerializefile_size()
		{
			return file_size > 0;
		}

		private byte[] rawData;
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
		public static ObjectUploadResponse Upload(string url, ArraySegment<byte> imageData, string groupId,
											string objectId, string fileName, string contentType,
											ImageMeta meta, AttachmentType type, string apiKey, 
											string token)
		{
			try
			{
				Dictionary<string, object> pars = new Dictionary<string, object>();
				pars["type"] = type.ToString();
				if (meta != ImageMeta.None)
					pars["image_meta"] = meta.ToString().ToLower();
				pars["session_token"] = token;
				pars["apikey"] = apiKey;
				if (objectId != null)
					pars["object_id"] = objectId;
				pars["group_id"] = groupId;
				HttpWebResponse _webResponse =
					Waveface.MultipartFormDataPostHelper.MultipartFormDataPost(
					url,
					"Mozilla 4.0+",
					pars,
					fileName,
					contentType,
					imageData);

				using (StreamReader reader = new StreamReader(_webResponse.GetResponseStream()))
				{
					return fastJSON.JSON.Instance.ToObject<ObjectUploadResponse>(reader.ReadToEnd());
				}
			}
			catch (WebException e)
			{
				throw new WammerCloudException("Wammer cloud error", e);
			}
		}

		public static ObjectUploadResponse UploadImage(string url, ArraySegment<byte> imageData, string groupId,
											string objectId, string fileName, string contentType,
											ImageMeta meta, string apiKey, string token)
		{
			return Upload(url, imageData, groupId, objectId, fileName, contentType, meta,
				AttachmentType.image, apiKey, token);
		}

		public static ObjectUploadResponse UploadImage(ArraySegment<byte> imageData, string group_id,
			string objectId, string fileName, string contentType, ImageMeta meta,
			string apikey, string token)
		{
			string url = CloudServer.BaseUrl + "attachments/upload/";

			return UploadImage(url, imageData, group_id, objectId, fileName, contentType, meta,
				apikey, token);
		}

		public ObjectUploadResponse Upload(ImageMeta meta, string apiKey, string sessionToken)
		{
			string url = CloudServer.BaseUrl + "attachments/upload/";

			return Upload(url, rawData, group_id, object_id, file_name, mime_type, 
																meta, type, apiKey, sessionToken);
		}
		#endregion

		[BsonId]
		public string object_id { get; set; }
		[BsonIgnoreIfNull]
		public string group_id { get; set; }
		[BsonIgnoreIfNull]
		public string file_name { get; set; }
		[BsonIgnoreIfNull]
		public string mime_type { get; set; }
		[BsonIgnoreIfNull]
		public string title { get; set; }
		[BsonIgnoreIfNull]
		public string description { get; set; }
		[BsonIgnoreIfNull]
		public string md5 { get; set; }
		public AttachmentType type { get; set; }
		[BsonIgnoreIfNull]
		public string url { get; set; }
		[BsonIgnoreIfNull]
		public string image { get; set; }
		[BsonIgnoreIfNull]
		public long file_size { get; set; }
		[BsonIgnoreIfNull]
		public DateTime modify_time { get; set; }
		[BsonIgnoreIfNull]
		public ImageProperty image_meta { get; set; }
		[BsonIgnoreIfNull]
		public string saved_file_name { get; set; }

		[BsonIgnore]
		private object rawDataMutex = new object();

		[BsonIgnore]
		[System.Xml.Serialization.XmlIgnore]
		public ArraySegment<byte> RawData
		{
			get {
				lock (rawDataMutex)
				{
					if (rawData.Array == null && this.saved_file_name != null)
					{
						Driver driver = DriverCollection.Instance.FindOne();
						FileStorage storage = new FileStorage(driver);
						byte[] buffer = new byte[this.file_size];
						storage.Load(this.saved_file_name).Read(buffer, 0, buffer.Length);
						rawData = new ArraySegment<byte>(buffer);
					}
					return rawData;
				}
			}
			set
			{
				lock (rawDataMutex)
				{
					rawData = value;
					if (rawData.Array != null)
					{
						using (MD5 md5 = MD5.Create())
						{
							byte[] hash = md5.ComputeHash(rawData.Array, rawData.Offset, rawData.Count);
							StringBuilder buff = new StringBuilder();
							for (int i = 0; i < hash.Length; i++)
								buff.Append(hash[i].ToString("x2"));

							this.md5 = buff.ToString();
						}
					}
				}
			}
		}

		public Attachment()
		{
			rawData = new ArraySegment<byte>();
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
			group_id = lhs.group_id;
			saved_file_name = lhs.saved_file_name;
		}

		public bool ShouldSerializefile_size()
		{
			return file_size > 0;
		}

		private ArraySegment<byte> rawData;
	}

	public class AttachmentCollection : Collection<Attachment>
	{
		private static AttachmentCollection instance;

		static AttachmentCollection()
		{
			instance = new AttachmentCollection();
		}

		private AttachmentCollection()
			: base("attachments")
		{
		}

		public static AttachmentCollection Instance
		{
			get { return instance; }
		}
	}

	public class AttachmentResponse : CloudResponse
	{
		public Attachment attachment { get; set; }

		public AttachmentResponse()
			: base(200, 0, "success")
		{
		}

		public AttachmentResponse(Attachment att)
			: base(200, 0, "success")
		{
			attachment = att;
		}
	}
}
