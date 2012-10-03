using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station.Retry;
using Wammer.Utility;
using System.Diagnostics;
using Wammer.PerfMonitor;

namespace Wammer.Station.Timeline
{
	public interface IResourceDownloadTask : ITask
	{
		string Name { get; }
		string UserId { get; }
	}



	[Serializable]
	public class ResourceDownloadTask : DelayedRetryTask, IResourceDownloadTask
	{
		private static readonly IPerfCounter downloadCount = PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT);
		private static readonly ILog logger = LogManager.GetLogger(typeof(ResourceDownloadTask));
		private static readonly AttachmentUpload.AttachmentUploadStorage resStorage = new AttachmentUpload.AttachmentUploadStorage(new AttachmentUpload.AttachmentUploadStorageDB());
		private ResourceDownloadEventArgs evtargs;

		public ResourceDownloadTask(ResourceDownloadEventArgs arg, TaskPriority pri)
			: base(pri)
		{
			Debug.Assert(!String.IsNullOrEmpty(arg.attachment.object_id));
			evtargs = arg;
		}

		private static bool AttachmentExists(ResourceDownloadEventArgs evtargs)
		{
			bool alreadyExist;

			if (evtargs.imagemeta == ImageMeta.Origin || evtargs.imagemeta == ImageMeta.None)
				// origin image and non-image attachment here
				alreadyExist = AttachmentCollection.Instance.FindOne(
					Query.And(
						Query.EQ("_id", evtargs.attachment.object_id),
						Query.Exists("saved_file_name", true))) != null;
			else
				// thumbnails here
				alreadyExist = AttachmentCollection.Instance.FindOne(
					Query.And(
						Query.EQ("_id", evtargs.attachment.object_id),
						Query.Exists("image_meta." + evtargs.imagemeta.ToString().ToLower() + ".saved_file_name", true))) != null;

			return alreadyExist;
		}

		private static void DownloadComplete(ResourceDownloadEventArgs args, Driver driver)
		{
			try
			{
				AttachmentInfo attachment = args.attachment;
				ImageMeta imagemeta = args.imagemeta;
				string filepath = args.filepath;

				ThumbnailInfo thumbnail;
				string savedFileName;
				var rawdata = new ArraySegment<byte>(File.ReadAllBytes(filepath));

				var fs = new FileStorage(driver);

				switch (imagemeta)
				{
					case ImageMeta.Origin:

						var data = new Wammer.Station.AttachmentUpload.UploadData
						{ 
							raw_data = rawdata,
							file_name = args.attachment.file_name,
							imageMeta = ImageMeta.Origin,
							object_id = args.attachment.object_id,
							group_id = args.attachment.group_id,
							type = AttachmentType.image,
							file_create_time =  TimeHelper.ParseCloudTimeString(attachment.file_create_time)
						};

						var takenTimeStr = extractExifTakenTime(rawdata);

						savedFileName = resStorage.Save(data, takenTimeStr).RelativePath;
						File.Delete(filepath);
						var size = ImageHelper.GetImageSize(rawdata);
						

						MD5 md5 = MD5.Create();
						byte[] hash = md5.ComputeHash(rawdata.Array);
						var md5buff = new StringBuilder();
						for (var i = 0; i < hash.Length; i++)
							md5buff.Append(hash[i].ToString("x2"));

						AttachmentCollection.Instance.Update(
							Query.EQ("_id", attachment.object_id),
							Update.Set("group_id", attachment.group_id
								).Set("file_name", attachment.file_name
								).Set("title", attachment.title
								).Set("description", attachment.description
								).Set("type", (int)(AttachmentType)Enum.Parse(typeof(AttachmentType), attachment.type)
								).Set("url", "/v2/attachments/view?object_id=" + attachment.object_id
								).Set("mime_type", "application/octet-stream"
								).Set("saved_file_name", savedFileName
								).Set("md5", md5buff.ToString()
								).Set("image_meta.width", size.Width
								).Set("image_meta.height", size.Height
								).Set("file_size", rawdata.Count
								).Set("modify_time", TimeHelper.ConvertToDateTime(attachment.modify_time)),
							UpdateFlags.Upsert
							);

						TaskQueue.Enqueue(new NotifyCloudOfBodySyncedTask(attachment.object_id, driver.session_token), TaskPriority.Low,
								  true);

						break;

					case ImageMeta.Small:
						savedFileName = attachment.object_id + "_small.dat";
						savedFileName = FileStorage.SaveToCacheFolder(driver.user_id, savedFileName, rawdata);
						File.Delete(filepath);

						thumbnail = new ThumbnailInfo
						{
							url = "/v2/attachments/view?object_id=" + attachment.object_id + "&image_meta=small",
							width = attachment.image_meta.small.width,
							height = attachment.image_meta.small.height,
							mime_type = attachment.image_meta.small.mime_type,
							modify_time = TimeHelper.ConvertToDateTime(attachment.image_meta.small.modify_time),
							file_size = attachment.image_meta.small.file_size,
							file_name = attachment.image_meta.small.file_name,
							md5 = attachment.image_meta.small.md5,
							saved_file_name = savedFileName
						};
						AttachmentCollection.Instance.Update(
							Query.EQ("_id", attachment.object_id), Update
																	.Set("group_id", attachment.group_id)
																	.Set("file_name", attachment.file_name)
																	.Set("type",
																		 (int)
																		 (AttachmentType)
																		 Enum.Parse(typeof(AttachmentType), attachment.type))
																	.Set("image_meta.small", thumbnail.ToBsonDocument()),
							UpdateFlags.Upsert
							);
						break;

					case ImageMeta.Medium:
						savedFileName = attachment.object_id + "_medium.dat";
						savedFileName = FileStorage.SaveToCacheFolder(driver.user_id, savedFileName, rawdata);
						File.Delete(filepath);

						thumbnail = new ThumbnailInfo
						{
							url = "/v2/attachments/view?object_id=" + attachment.object_id + "&image_meta=medium",
							width = attachment.image_meta.medium.width,
							height = attachment.image_meta.medium.height,
							mime_type = attachment.image_meta.medium.mime_type,
							modify_time = TimeHelper.ConvertToDateTime(attachment.image_meta.medium.modify_time),
							file_size = attachment.image_meta.medium.file_size,
							file_name = attachment.image_meta.medium.file_name,
							md5 = attachment.image_meta.medium.md5,
							saved_file_name = savedFileName
						};
						AttachmentCollection.Instance.Update(
							Query.EQ("_id", attachment.object_id), Update
																	.Set("group_id", attachment.group_id)
																	.Set("file_name", attachment.file_name)
																	.Set("type",
																		 (int)
																		 (AttachmentType)
																		 Enum.Parse(typeof(AttachmentType), attachment.type))
																	.Set("image_meta.medium", thumbnail.ToBsonDocument()),
							UpdateFlags.Upsert
							);
						break;

					case ImageMeta.Large:
						savedFileName = attachment.object_id + "_large.dat";
						savedFileName = FileStorage.SaveToCacheFolder(driver.user_id, savedFileName, rawdata);
						File.Delete(filepath);

						thumbnail = new ThumbnailInfo
						{
							url = "/v2/attachments/view?object_id=" + attachment.object_id + "&image_meta=large",
							width = attachment.image_meta.large.width,
							height = attachment.image_meta.large.height,
							mime_type = attachment.image_meta.large.mime_type,
							modify_time = TimeHelper.ConvertToDateTime(attachment.image_meta.large.modify_time),
							file_size = attachment.image_meta.large.file_size,
							file_name = attachment.image_meta.large.file_name,
							md5 = attachment.image_meta.large.md5,
							saved_file_name = savedFileName
						};
						AttachmentCollection.Instance.Update(
							Query.EQ("_id", attachment.object_id), Update
																	.Set("group_id", attachment.group_id)
																	.Set("file_name", attachment.file_name)
																	.Set("type",
																		 (int)
																		 (AttachmentType)
																		 Enum.Parse(typeof(AttachmentType), attachment.type))
																	.Set("image_meta.large", thumbnail.ToBsonDocument()),
							UpdateFlags.Upsert
							);
						break;

					case ImageMeta.Square:
						savedFileName = attachment.object_id + "_square.dat";
						savedFileName = FileStorage.SaveToCacheFolder(driver.user_id, savedFileName, rawdata);
						File.Delete(filepath);

						thumbnail = new ThumbnailInfo
						{
							url = "/v2/attachments/view?object_id=" + attachment.object_id + "&image_meta=square",
							width = attachment.image_meta.square.width,
							height = attachment.image_meta.square.height,
							mime_type = attachment.image_meta.square.mime_type,
							modify_time = TimeHelper.ConvertToDateTime(attachment.image_meta.square.modify_time),
							file_size = attachment.image_meta.square.file_size,
							file_name = attachment.image_meta.square.file_name,
							md5 = attachment.image_meta.square.md5,
							saved_file_name = savedFileName
						};
						AttachmentCollection.Instance.Update(
							Query.EQ("_id", attachment.object_id), Update
																	.Set("group_id", attachment.group_id)
																	.Set("file_name", attachment.file_name)
																	.Set("type",
																		 (int)
																		 (AttachmentType)
																		 Enum.Parse(typeof(AttachmentType), attachment.type))
																	.Set("image_meta.square", thumbnail.ToBsonDocument()),
							UpdateFlags.Upsert
							);
						break;

					default:
						logger.WarnFormat("Unknown image meta type {0}", imagemeta);
						break;
				}

			}
			catch (Exception ex)
			{
				logger.Warn("Unable to save attachment, ignore it.", ex);
			}
		}

		private static string extractExifTakenTime(ArraySegment<byte> rawdata)
		{
			try
			{
				var exif = ExifLibrary.ExifFile.Read(rawdata.Array);

				DateTime? takenTime = null;

				if (exif.Properties.ContainsKey(ExifLibrary.ExifTag.DateTimeOriginal))
					takenTime = (DateTime)exif.Properties[ExifLibrary.ExifTag.DateTimeOriginal].Value;

				var takenTimeStr = takenTime.HasValue ? takenTime.Value.ToCloudTimeString() : null;
				return takenTimeStr;
			}
			catch
			{
				return null;
			}
		}

		protected override void Run()
		{
			string meta = evtargs.imagemeta.ToString();
			string oldFile = evtargs.filepath;

			downloadCount.Increment();

			try
			{
				bool alreadyExist = AttachmentExists(evtargs);

				if (alreadyExist)
				{
					logger.DebugFormat("Attachment {0} meta {1} already exists. Skip downstreaming it.", evtargs.attachment.object_id,
									   meta);
					return;
				}

				Driver user = DriverCollection.Instance.FindOne(Query.EQ("_id", evtargs.user_id));
				if (user == null)
				{
					logger.Info("drop download task because user does not exist anymore: " + evtargs.user_id);
					return;
				}

				var api = new AttachmentApi(user);
				api.AttachmentView(evtargs, StationRegistry.StationId);
				DownloadComplete(evtargs, user);
			}
			catch (Exception e)
			{
				string msg = string.Format("Unabel to download attachment {0} meta {1}. ", evtargs.attachment.object_id, meta);

				if (e is WammerCloudException)
					throw new Exception(msg + (e as WammerCloudException).response, e);
				else
					throw new Exception(msg, e);
			}
			finally
			{
				downloadCount.Decrement();

				if (File.Exists(oldFile))
					File.Delete(oldFile);
			}
		}

		public string Name
		{
			get
			{
				return evtargs.attachment.object_id + evtargs.imagemeta;
			}
		}

		public string UserId
		{
			get
			{
				return evtargs.user_id;
			}
		}

		public override void ScheduleToRun()
		{
			var meta = evtargs.imagemeta.ToString();
			if (++evtargs.failureCount >= 10)
			{
				logger.WarnFormat("Unable to download attachment object_id={0}, image_meta={1}", evtargs.attachment.object_id, meta);
				return;
			}

			logger.WarnFormat("Unable to download attachment. Enqueue download task again: attachment object_id={0}, image_meta={1}",
									   evtargs.attachment.object_id, meta);
			BodySyncQueue.Instance.Enqueue(this, priority);
		}
	}
}
