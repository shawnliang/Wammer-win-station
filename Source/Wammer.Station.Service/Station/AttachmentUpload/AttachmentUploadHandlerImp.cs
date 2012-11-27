using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using ExifLibrary;
using System.Collections.Generic;
using Newtonsoft.Json;
using MongoDB.Driver.Builders;
using System.Globalization;

namespace Wammer.Station.AttachmentUpload
{
	public enum UpsertResult
	{
		Insert,
		Update
	}

	public interface IAttachmentUploadHandlerDB
	{
		UpsertResult InsertOrMergeToExistingDoc(Attachment doc);
		LoginedSession FindSession(string sessionToken, string apiKey);
	}

	public class AttachmentSaveResult
	{
		public AttachmentSaveResult(string storagePath, string relativePath)
		{
			this.StorageBasePath = storagePath;
			this.RelativePath = relativePath;
		}

		/// <summary>
		/// Storage base path
		/// </summary>
		public string StorageBasePath { get; private set; }
		
		/// <summary>
		/// Attachment saved path which is a relative path to StorageBasePath
		/// </summary>
		public string RelativePath { get; private set; }
		
		public string FullPath
		{
			get { return Path.Combine(StorageBasePath, RelativePath); }
		}
	}

	public interface IAttachmentUploadStorage
	{
		/// <summary>
		/// Saves attachment to proper location
		/// </summary>
		/// <param name="data">attachment data</param>
		/// <returns>relative save path to user resource filder (if attachment is original) or to user cache folder (if attachment is a thumbnail)</returns>
		AttachmentSaveResult Save(UploadData data, string takenTime);
	}

	public class UploadData
	{
		private string _mime_type;

		public string object_id { get; set; }
		public string group_id { get; set; }

		public string mime_type
		{
			get { return string.IsNullOrEmpty(_mime_type) ? "application/octet-stream" : _mime_type; }
			set { _mime_type = value; }
		}

		public string file_name { get; set; }
		public string title { get; set; }
		public string description { get; set; }
		public AttachmentType type { get; set; }
		public ArraySegment<byte> raw_data { get; set; }
		public ImageMeta imageMeta { get; set; }

		public string api_key { get; set; }
		public string session_token { get; set; }
		public string post_id { get; set; }
		public string file_path { get; set; }
		public DateTime? import_time { get; set; }
		public DateTime? file_create_time { get; set; }
		public string exif { get; set; }
		public int? timezone { get; set; }
		public bool fromLocal { get; set; }
	}

	public class AttachmentEventArgs : EventArgs
	{
		public AttachmentEventArgs(string attachmentId, bool isFromThisWindows, UpsertResult upsertResult, ImageMeta meta,
			string userSession, string apikey, string postId = null, string group_id = null)
		{
			AttachmentId = attachmentId;
			IsFromThisWindows = isFromThisWindows;
			UpsertResult = upsertResult;
			ImgMeta = meta;
			UserSession = userSession;
			APIKey = apikey;
			PostId = postId;
			GroupId = group_id;
		}

		public string PostId { get; private set; }
		public string AttachmentId { get; private set; }
		public bool IsFromThisWindows { get; private set; }
		public UpsertResult UpsertResult { get; private set; }
		public ImageMeta ImgMeta { get; private set; }
		public string UserSession { get; private set; }
		public string APIKey { get; private set; }
		public string GroupId { get; private set; }
	}

	public interface IExifExtractor
	{
		/// <summary>
		/// Extracts exif information from image file
		/// </summary>
		/// <param name="imageData"></param>
		/// <returns>exif data; null is returned if no exif is embeded or error</returns>
		exif extract(ArraySegment<byte> imageData);
	}


	public class AttachmentUploadHandlerImp
	{
		private readonly IAttachmentUploadHandlerDB db;
		private readonly IAttachmentUploadStorage storage;
		public IExifExtractor exifExtractor { get; set; }

		public AttachmentUploadHandlerImp(IAttachmentUploadHandlerDB db, IAttachmentUploadStorage storage)
		{
			DebugInfo.ShowMethod();

			this.db = db;
			this.storage = storage;
			this.exifExtractor = new ExifExtractor();
		}

		public event EventHandler<AttachmentEventArgs> AttachmentProcessed;

		public void Process(UploadData uploadData)
		{
			Size imageSize = ImageHelper.GetImageSize(uploadData.raw_data);
			Process(uploadData, imageSize);
		}

		public void Process(UploadData uploadData, Size imageSize)
		{
			DebugInfo.ShowMethod();
			if (uploadData == null)
				throw new ArgumentNullException("uploadData");

			if (uploadData.object_id == null)
				uploadData.object_id = Guid.NewGuid().ToString();

			//Attachment already exists => return
			var metaStr = uploadData.imageMeta.GetCustomAttribute<DescriptionAttribute>().Description;

			var doc =
				AttachmentCollection.Instance.FindOne(uploadData.imageMeta == ImageMeta.Origin
														? Query.And(Query.EQ("_id", uploadData.object_id), Query.Exists("saved_file_name"))
														: Query.And(Query.EQ("_id", uploadData.object_id),
																	Query.Exists("image_meta." + metaStr)));
			if (doc != null)
				return;


			exif exif = null;
			if (!string.IsNullOrEmpty(uploadData.exif))
				exif = parseExifParameter(uploadData);
			else
				exif = extractExifFromOriginImage(uploadData);

			var saveReult = storage.Save(uploadData, (exif != null) ? exif.DateTimeOriginal : null);

			DateTime eventTime = guessEventTime(uploadData, exif);

			var dbDoc = new Attachment
							{
								object_id = uploadData.object_id,
								group_id = uploadData.group_id,
								file_name = uploadData.file_name,
								title = uploadData.title,
								description = uploadData.description,
								modify_time = DateTime.UtcNow,
								post_id = uploadData.post_id,
								file_path = uploadData.file_path,
								import_time = uploadData.import_time,
								image_meta = new ImageProperty { exif = exif },
								event_time = eventTime,
								timezone = uploadData.timezone,
								fromLocal = uploadData.fromLocal
							};

			if (uploadData.imageMeta == ImageMeta.Origin || uploadData.imageMeta == ImageMeta.None)
			{
				dbDoc.mime_type = uploadData.mime_type;
				dbDoc.saved_file_name = saveReult.RelativePath;
				dbDoc.file_size = uploadData.raw_data.Count;
				dbDoc.url = GetViewApiUrl(uploadData);
				dbDoc.md5 = ComputeMD5(uploadData.raw_data);
				dbDoc.image_meta.width = imageSize.Width;
				dbDoc.image_meta.height = imageSize.Height;
				dbDoc.file_create_time = uploadData.file_create_time;
			}
			else
			{
				var thumb = new ThumbnailInfo
								{
									file_size = uploadData.raw_data.Count,
									md5 = ComputeMD5(uploadData.raw_data),
									mime_type = uploadData.mime_type,
									saved_file_name = saveReult.RelativePath,
									url = GetViewApiUrl(uploadData),
									width = imageSize.Width,
									height = imageSize.Height
								};

				dbDoc.image_meta.SetThumbnailInfo(uploadData.imageMeta, thumb);
			}

			UpsertResult dbResult = db.InsertOrMergeToExistingDoc(dbDoc);

			OnAttachmentProcessed(
				new AttachmentEventArgs(
					uploadData.object_id,
					db.FindSession(uploadData.session_token, uploadData.api_key) != null,
					dbResult,
					uploadData.imageMeta,
					uploadData.session_token,
					uploadData.api_key,
					uploadData.post_id,
					uploadData.group_id
				)
			);
		}

		private static DateTime guessEventTime(UploadData uploadData, exif exif)
		{
			DateTime eventTime;
			if (exif != null && exif.gps != null && !string.IsNullOrEmpty(exif.gps.GPSDateStamp) && exif.gps.GPSTimeStamp != null)
			{
				eventTime = DateTime.ParseExact(exif.gps.GPSDateStamp, "yyyy:MM:dd", CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);

				var hour = getRationalValue(exif.gps.GPSTimeStamp[0]);
				var min = getRationalValue(exif.gps.GPSTimeStamp[1]);
				var sec = getRationalValue(exif.gps.GPSTimeStamp[2]);

				eventTime = eventTime.AddHours((double)hour).AddMinutes((double)min).AddSeconds((double)sec);
			}
			else if (uploadData.timezone.HasValue && exif != null && !string.IsNullOrEmpty(exif.DateTime))
			{
				var exifTime = DateTime.ParseExact(exif.DateTime, "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);
				eventTime = exifTime.AddMinutes(-uploadData.timezone.Value);
			}
			else if (uploadData.file_create_time.HasValue)
			{
				eventTime = uploadData.file_create_time.Value;
			}
			else
			{
				eventTime = DateTime.Now;
			}
			return eventTime;
		}

		private static uint getRationalValue(object[] rational)
		{
			var value = Convert.ToUInt32(rational[0]) / Convert.ToUInt32(rational[1]);
			return value;
		}

		private exif parseExifParameter(UploadData uploadData)
		{
			try
			{
				return JsonConvert.DeserializeObject<exif>(uploadData.exif);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.Assert(false, "exif format is not correct: " + e.Message);
				throw new FormatException("parameter 'exif' format error", e);
			}
		}

		private exif extractExifFromOriginImage(UploadData uploadData)
		{
			if (uploadData.imageMeta == ImageMeta.Origin)
				return exifExtractor.extract(uploadData.raw_data);
			else
				return null;
		}

		private void OnAttachmentProcessed(AttachmentEventArgs evt)
		{
			DebugInfo.ShowMethod();

			EventHandler<AttachmentEventArgs> handler = AttachmentProcessed;

			if (handler != null)
			{
				handler(this, evt);
			}
		}

		private static string GetViewApiUrl(UploadData data)
		{
			DebugInfo.ShowMethod();

			var buf = new StringBuilder();
			buf.Append("/v2/attachments/view/?object_id=").
				Append(data.object_id);

			var imageMeta = data.imageMeta;
			if (ImageMeta.Square <= imageMeta && imageMeta <= ImageMeta.Large)
			{
				buf.Append("&image_meta=").
					Append(imageMeta.GetCustomAttribute<DescriptionAttribute>().Description);
			}

			return buf.ToString();
		}

		private static string GetSavedFileName(UploadData data)
		{
			DebugInfo.ShowMethod();

			var buf = new StringBuilder();
			buf.Append(data.object_id);

			var imageMeta = data.imageMeta;
			if (ImageMeta.Square <= imageMeta && imageMeta <= ImageMeta.Large)
			{
				buf.Append("_").
					Append(imageMeta.GetCustomAttribute<DescriptionAttribute>().Description).
					Append(".dat");
			}
			else
			{
				buf.Append(Path.GetExtension(data.file_name));
			}

			return buf.ToString();
		}

		private static string ComputeMD5(ArraySegment<byte> rawData)
		{
			DebugInfo.ShowMethod();

			using (MD5 md5 = MD5.Create())
			{
				byte[] hash = md5.ComputeHash(rawData.Array, rawData.Offset, rawData.Count);
				var buff = new StringBuilder();
				for (int i = 0; i < hash.Length; i++)
					buff.Append(hash[i].ToString("x2"));

				return buff.ToString();
			}
		}
	}
}
