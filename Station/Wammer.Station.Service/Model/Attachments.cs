using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Wammer.Cloud;
using Wammer.Station;
using Wammer.Utility;
using Waveface;
using fastJSON;

namespace Wammer.Model
{
	[Serializable]
	public enum ImageMeta
	{
		/// <summary>
		/// 
		/// </summary>
		[Description("none")] None = 0,

		/// <summary>
		/// 128x128 pixels
		/// </summary>
		[Description("square")] Square = 128,

		/// <summary>
		/// 120 pixels
		/// </summary>
		[Description("small")] Small = 512,

		/// <summary>
		/// 720 pixels
		/// </summary>
		[Description("medium")] Medium = 1024,

		/// <summary>
		/// 1024 pixels
		/// </summary>
		[Description("large")] Large = 2048,

		/// <summary>
		/// Original image size
		/// </summary>
		[Description("origin")] Origin = 50*1024*1024
	}

	public enum AttachmentType
	{
		image,
		doc
	}

	public interface IAttachmentInfo
	{
		string mime_type { get; }
		string saved_file_name { get; }
	}

	[BsonIgnoreExtraElements]
	public class ThumbnailInfo : IAttachmentInfo
	{
		private byte[] rawData;

		[BsonIgnoreIfNull]
		public string url { get; set; }

		[BsonIgnoreIfNull]
		public int width { get; set; }

		[BsonIgnoreIfNull]
		public int height { get; set; }

		[BsonIgnoreIfNull]
		public DateTime modify_time { get; set; }

		[BsonIgnoreIfNull]
		public long file_size { get; set; }

		[BsonIgnoreIfNull]
		public string file_name { get; set; }

		[BsonIgnoreIfNull]
		public string md5 { get; set; }

		[BsonIgnore]
		[XmlIgnore]
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
						var buff = new StringBuilder();
						for (int i = 0; i < hash.Length; i++)
							buff.Append(hash[i].ToString("x2"));

						this.md5 = buff.ToString();
					}
				}
			}
		}

		#region IAttachmentInfo Members

		[BsonIgnoreIfNull]
		public string mime_type { get; set; }

		[BsonIgnoreIfNull]
		public string saved_file_name { get; set; }

		#endregion

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
	public class Attachment : IAttachmentInfo
	{
		#region Private Method

		private static Dictionary<string, object> GetAdditionalParams(string groupId, string objectId, ImageMeta meta,
		                                                              AttachmentType type, string apiKey, string token)
		{
			var pars = new Dictionary<string, object>();
			pars["type"] = type.ToString();
			if (meta != ImageMeta.None)
				pars["image_meta"] = meta.ToString().ToLower();
			pars["session_token"] = token;
			pars["apikey"] = apiKey;
			if (objectId != null)
				pars["object_id"] = objectId;
			pars["group_id"] = groupId;
			return pars;
		}

		#endregion

		#region Upload utility functions

		public static ObjectUploadResponse Upload(Stream dataStream, string groupId,
		                                          string objectId, string fileName, string contentType,
		                                          ImageMeta meta, AttachmentType type, string apiKey,
		                                          string token, int bufferSize = 1024,
		                                          Action<object, ProgressChangedEventArgs> progressChangedCallBack = null)
		{
			return Upload(CloudServer.BaseUrl + "attachments/upload", dataStream, groupId,
			              objectId, fileName, contentType, meta, type, apiKey, token, bufferSize, progressChangedCallBack);
		}

		public static ObjectUploadResponse Upload(string url, Stream dataStream, string groupId,
		                                          string objectId, string fileName, string contentType,
		                                          ImageMeta meta, AttachmentType type, string apiKey,
		                                          string token, int bufferSize = 1024,
		                                          Action<object, ProgressChangedEventArgs> progressChangedCallBack = null)
		{
			try
			{
				if (token == null)
					throw new WammerCloudException("session token is null", WebExceptionStatus.ProtocolError, (int)GeneralApiError.SessionNotExist);

				Dictionary<string, object> pars = GetAdditionalParams(groupId, objectId, meta, type, apiKey, token);
				HttpWebResponse _webResponse = MultipartFormDataPostHelper.MultipartFormDataPost(
					url,
					"Mozilla 4.0+",
					pars,
					fileName,
					contentType,
					dataStream, bufferSize, progressChangedCallBack);

				Debug.Assert(_webResponse != null, "_webResponse != null");
				using (var reader = new StreamReader(_webResponse.GetResponseStream()))
				{
					return JSON.Instance.ToObject<ObjectUploadResponse>(reader.ReadToEnd());
				}
			}
			catch (WebException e)
			{
				throw new WammerCloudException("Wammer cloud error", e);
			}
		}

		public static ObjectUploadResponse Upload(string url, ArraySegment<byte> imageData, string groupId,
		                                          string objectId, string fileName, string contentType,
		                                          ImageMeta meta, AttachmentType type, string apiKey,
		                                          string token, int bufferSize = 1024,
		                                          Action<object, ProgressChangedEventArgs> progressChangedCallBack = null)
		{
			try
			{
				if (token == null)
					throw new WammerCloudException("session token is null", WebExceptionStatus.ProtocolError, (int)GeneralApiError.SessionNotExist);

				Dictionary<string, object> pars = GetAdditionalParams(groupId, objectId, meta, type, apiKey, token);

				HttpWebResponse _webResponse =
					MultipartFormDataPostHelper.MultipartFormDataPost(
						url,
						"Mozilla 4.0+",
						pars,
						fileName,
						contentType,
						imageData, bufferSize, progressChangedCallBack);

				Debug.Assert(_webResponse != null, "_webResponse != null");
				using (var reader = new StreamReader(_webResponse.GetResponseStream()))
				{
					return JSON.Instance.ToObject<ObjectUploadResponse>(reader.ReadToEnd());
				}
			}
			catch (WebException e)
			{
				throw new WammerCloudException("Wammer cloud error", e);
			}
		}


		public static ObjectUploadResponse UploadImage(string url, ArraySegment<byte> imageData, string groupId,
		                                               string objectId, string fileName, string contentType,
		                                               ImageMeta meta, string apiKey, string token, int bufSize = 1024,
		                                               Action<object, ProgressChangedEventArgs> callback = null)
		{
			return Upload(url, imageData, groupId, objectId, fileName, contentType, meta,
			              AttachmentType.image, apiKey, token, bufSize, callback);
		}

		public static ObjectUploadResponse UploadImage(ArraySegment<byte> imageData, string group_id,
		                                               string objectId, string fileName, string contentType, ImageMeta meta,
		                                               string apikey, string token, int buffSize = 1024,
		                                               Action<object, ProgressChangedEventArgs> callback = null)
		{
			string url = CloudServer.BaseUrl + "attachments/upload/";

			return UploadImage(url, imageData, group_id, objectId, fileName, contentType, meta,
			                   apikey, token, buffSize, callback);
		}

		public ObjectUploadResponse Upload(ImageMeta meta, string apiKey, string sessionToken, int bufferSize = 1024,
		                                   Action<object, ProgressChangedEventArgs> progressChangedCallBack = null)
		{
			string url = CloudServer.BaseUrl + "attachments/upload/";

			return Upload(url, rawData, group_id, object_id, file_name, mime_type,
			              meta, type, apiKey, sessionToken, bufferSize, progressChangedCallBack);
		}

		#endregion

		[BsonIgnore] private readonly object rawDataMutex = new object();
		private ArraySegment<byte> rawData;

		public Attachment()
		{
			rawData = new ArraySegment<byte>();
			Orientation = ExifOrientations.Unknown;
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
			Orientation = lhs.Orientation;
		}

		[BsonId]
		public string object_id { get; set; }

		[BsonIgnoreIfNull]
		public string group_id { get; set; }

		[BsonIgnoreIfNull]
		public string file_name { get; set; }

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

		[BsonDefaultValue(true)]
		public bool is_thumb_upstreamed { get; set; }

		[BsonDefaultValue(false)]
		public bool is_body_upstreamed { get; set; }

		[BsonIgnore]
		public ExifOrientations Orientation { get; set; }

		[BsonIgnore]
		public string creator_id { get; set; }

		[BsonIgnore]
		[XmlIgnore]
		public ArraySegment<byte> RawData
		{
			get
			{
				lock (rawDataMutex)
				{
					if (rawData.Array == null && saved_file_name != null)
					{
						Driver driver = DriverCollection.Instance.FindOne();
						var storage = new FileStorage(driver);
						var buffer = new byte[file_size];
						storage.Load(saved_file_name).Read(buffer, 0, buffer.Length);
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
							var buff = new StringBuilder();
							for (int i = 0; i < hash.Length; i++)
								buff.Append(hash[i].ToString("x2"));

							this.md5 = buff.ToString();
						}
					}
				}
			}
		}

		#region IAttachmentInfo Members

		[BsonIgnoreIfNull]
		public string mime_type { get; set; }

		[BsonIgnoreIfNull]
		public string saved_file_name { get; set; }

		#endregion

		public bool ShouldSerializefile_size()
		{
			return file_size > 0;
		}

		public bool HasThumbnail(ImageMeta meta)
		{
			if (image_meta == null)
				return false;

			switch (meta)
			{
				case ImageMeta.Small:
					return image_meta.small != null;
				case ImageMeta.Medium:
					return image_meta.medium != null;
				case ImageMeta.Large:
					return image_meta.large != null;
				case ImageMeta.Square:
					return image_meta.square != null;
				default:
					throw new ArgumentException("not a valid thumbmail meta: " + meta);
			}
		}

		public IAttachmentInfo GetInfoByMeta(ImageMeta meta)
		{
			if (meta == ImageMeta.None || meta == ImageMeta.Origin)
				return this;
			return image_meta.GetThumbnailInfo(meta);
		}

		public bool IsThumbnailOrBodyUpstreamed()
		{
			return is_body_upstreamed || is_thumb_upstreamed;
		}
	}

	public class AttachmentCollection : Collection<Attachment>
	{
		private static readonly AttachmentCollection instance;

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
}