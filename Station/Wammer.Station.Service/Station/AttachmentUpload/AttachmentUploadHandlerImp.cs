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
		Driver GetUserByGroupId(string groupId);
		LoginedSession FindSession(string sessionToken, string apiKey);
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
		public string memo { get; set; }
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

	public class AttachmentUploadHandlerImp
	{
		private readonly IAttachmentUploadHandlerDB db;

		public AttachmentUploadHandlerImp(IAttachmentUploadHandlerDB db)
		{
			DebugInfo.ShowMethod();

			this.db = db;
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
														? Query.And(Query.EQ("_id", uploadData.object_id), Query.Exists("saved_file_name", true))
														: Query.And(Query.EQ("_id", uploadData.object_id),
																	Query.Exists("image_meta." + metaStr, true)));
			if (doc != null)
				return;

			var storage = GetUserStorage(uploadData);
			var filename = GetSavedFileName(uploadData);
			storage.SaveFile(filename, uploadData.raw_data);

			var dbDoc = new Attachment
							{
								object_id = uploadData.object_id,
								group_id = uploadData.group_id,
								file_name = uploadData.file_name,
								title = uploadData.title,
								description = uploadData.description,
								modify_time = DateTime.UtcNow,
								post_id = uploadData.post_id,
								memo = uploadData.memo,
								image_meta = new ImageProperty()
							};


			if (uploadData.imageMeta == ImageMeta.Origin || uploadData.imageMeta == ImageMeta.None)
			{
				dbDoc.mime_type = uploadData.mime_type;
				dbDoc.saved_file_name = filename;
				dbDoc.file_size = uploadData.raw_data.Count;
				dbDoc.url = GetViewApiUrl(uploadData);
				dbDoc.md5 = ComputeMD5(uploadData.raw_data);
				dbDoc.image_meta.width = imageSize.Width;
				dbDoc.image_meta.height = imageSize.Height;

				var photoFile = Path.Combine(storage.basePath, dbDoc.saved_file_name);


				extractExif(dbDoc, photoFile);
			}
			else
			{
				var thumb = new ThumbnailInfo
								{
									file_size = uploadData.raw_data.Count,
									md5 = ComputeMD5(uploadData.raw_data),
									mime_type = uploadData.mime_type,
									saved_file_name = filename,
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

		private static void extractExif(Attachment dbDoc, string photoFile)
		{
			DebugInfo.ShowMethod();

			try
			{
				ExifFile exifFile = ExifFile.Read(photoFile);

				var exif = new exif();

				// Read metadata
				foreach (ExifProperty item in exifFile.Properties.Values)
				{
					switch (item.Tag)
					{
						case ExifTag.YResolution:
							exif.YResolution = new List<int>() { (int)((ExifURational)item).Value.Numerator, (int)((ExifURational)item).Value.Denominator };
							break;
						case ExifTag.ResolutionUnit:
							exif.ResolutionUnit = (int)((ResolutionUnit)item.Value);
							break;
						case ExifTag.Make:
							exif.Make = item.Value.ToString();
							break;
						case ExifTag.Flash:
							exif.Flash = (int)((Flash)item.Value);
							break;
						case ExifTag.DateTime:
							exif.DateTime = ((DateTime)item.Value).ToString("yyyy:MM:dd HH:mm:ss");
							break;
						case ExifTag.MeteringMode:
							exif.MeteringMode = (int)((MeteringMode)item.Value);
							break;
						case ExifTag.XResolution:
							exif.XResolution = new List<int>() { (int)((ExifURational)item).Value.Numerator, (int)((ExifURational)item).Value.Denominator };
							break;
						case ExifTag.ExposureProgram:
							exif.ExposureProgram = (int)((ExposureMode)item.Value);
							break;
						case ExifTag.ColorSpace:
							exif.ColorSpace = (int)((ColorSpace)item.Value);
							break;
						case ExifTag.DateTimeDigitized:
							exif.DateTimeDigitized = ((DateTime)item.Value).ToString("yyyy:MM:dd HH:mm:ss");
							break;
						case ExifTag.DateTimeOriginal:
							exif.DateTimeOriginal = ((DateTime)item.Value).ToString("yyyy:MM:dd HH:mm:ss");
							break;
						case ExifTag.SensingMethod:
							exif.SensingMethod = (int)item.Value;
							break;
						case ExifTag.FNumber:
							exif.FNumber = new List<int>() { (int)((ExifURational)item).Value.Numerator, (int)((ExifURational)item).Value.Denominator };
							break;
						case ExifTag.FocalLength:
							exif.FocalLength = new List<int>() { (int)((ExifURational)item).Value.Numerator, (int)((ExifURational)item).Value.Denominator };
							break;
						case ExifTag.ISOSpeedRatings:
							exif.ISOSpeedRatings = (int)((ExifLibrary.ExifUShort)(item)).Value;
							break;
						case ExifTag.Model:
							exif.Model = item.Value.ToString();
							break;
						case ExifTag.Software:
							exif.Software = item.Value.ToString();
							break;
						case ExifTag.FlashpixVersion:
							exif.FlashPixVersion = item.Value.ToString();
							break;
						case ExifTag.YCbCrPositioning:
							exif.YCbCrPositioning = (int)((YCbCrPositioning)item.Value);
							break;
						case ExifTag.ExifVersion:
							exif.ExifVersion = exifFile.Properties[ExifTag.ExifVersion].Value.ToString();
							break;
					}
				}

				if (exifFile.Properties.ContainsKey(ExifTag.GPSLatitudeRef) &&
					exifFile.Properties.ContainsKey(ExifTag.GPSLatitude) &&
					exifFile.Properties.ContainsKey(ExifTag.GPSLongitudeRef) &&
					exifFile.Properties.ContainsKey(ExifTag.GPSLongitude))
				{
					var gpsLatitudeRef = exifFile.Properties[ExifTag.GPSLatitudeRef].Value.ToString();
					var gpsLatitude = exifFile.Properties[ExifTag.GPSLatitude] as GPSLatitudeLongitude;
					var gpsLongitudeRef = exifFile.Properties[ExifTag.GPSLongitudeRef].Value.ToString();
					var gpsLongitude = exifFile.Properties[ExifTag.GPSLongitude] as GPSLatitudeLongitude;

					var gpsInfo = new GPSInfo()
					{
						GPSLatitudeRef = gpsLatitudeRef[0].ToString(),
						GPSLongitudeRef = gpsLongitudeRef[0].ToString()
					};

					gpsInfo.GPSLatitude = new List<List<int>>();
					gpsInfo.GPSLatitude.Add(new List<int>() { (int)gpsLatitude.Degrees.Numerator, (int)gpsLatitude.Degrees.Denominator });
					gpsInfo.GPSLatitude.Add(new List<int>() { (int)gpsLatitude.Minutes.Numerator, (int)gpsLatitude.Minutes.Denominator });
					gpsInfo.GPSLatitude.Add(new List<int>() { (int)gpsLatitude.Seconds.Numerator, (int)gpsLatitude.Seconds.Denominator });

					gpsInfo.GPSLongitude = new List<List<int>>();
					gpsInfo.GPSLongitude.Add(new List<int>() { (int)gpsLongitude.Degrees.Numerator, (int)gpsLongitude.Degrees.Denominator });
					gpsInfo.GPSLongitude.Add(new List<int>() { (int)gpsLongitude.Minutes.Numerator, (int)gpsLongitude.Minutes.Denominator });
					gpsInfo.GPSLongitude.Add(new List<int>() { (int)gpsLongitude.Seconds.Numerator, (int)gpsLongitude.Seconds.Denominator });

					exif.GPSInfo = gpsInfo;
				}

				dbDoc.image_meta.exif = exif;
			}
			catch
			{
			}
		}

		private FileStorage GetUserStorage(UploadData uploadData)
		{
			DebugInfo.ShowMethod();

			Driver user = db.GetUserByGroupId(uploadData.group_id);
			if (user == null)
				throw new WammerStationException("User is not associated with this station", (int)StationLocalApiError.InvalidDriver);

			var storage = new FileStorage(user);
			return storage;
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