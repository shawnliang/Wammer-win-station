using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PerfMonitor;
using Wammer.Station.Retry;
using Wammer.Utility;
using Waveface.Stream.Core;
using Waveface.Stream.Model;

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
		private static AttachmentUpload.AttachmentUploadStorage storage = new AttachmentUpload.AttachmentUploadStorage(new AttachmentUpload.AttachmentUploadStorageDB());

		public ResourceDownloadEventArgs evtargs { get; set; }

		public ResourceDownloadTask()
			: base(TaskPriority.Medium)
		{
		}

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
						Query.Exists("saved_file_name"))) != null;
			else
				// thumbnails here
				alreadyExist = AttachmentCollection.Instance.FindOne(
					Query.And(
						Query.EQ("_id", evtargs.attachment.object_id),
						Query.Exists("image_meta." + evtargs.imagemeta.ToString().ToLower() + ".saved_file_name"))) != null;

			return alreadyExist;
		}

		private static void DownloadComplete(ResourceDownloadEventArgs args, Driver driver)
		{
			try
			{
				var rawData = File.ReadAllBytes(args.filepath);
				var saveResult = SaveAttachmentToDisk(args.imagemeta, args.attachment, rawData);

				SaveToAttachmentDB(args.imagemeta, saveResult.RelativePath, args.attachment, rawData.Length);

				SystemEventSubscriber.Instance.TriggerAttachmentArrivedEvent(args.attachment.object_id);

				if (args.attachment.type.Equals("doc", StringComparison.InvariantCultureIgnoreCase))
					TaskQueue.Enqueue(new MakeDocPreviewsTask(args.attachment.object_id), TaskPriority.Medium);
			}
			catch (Exception ex)
			{
				logger.Warn("Unable to save attachment, ignore it.", ex);
			}
		}

		public static AttachmentUpload.AttachmentSaveResult SaveAttachmentToDisk(ImageMeta meta, AttachmentInfo metaData, byte[] image)
		{
			var param = new AttachmentUpload.UploadData
			{
				object_id = metaData.object_id,
				group_id = metaData.group_id,
				file_name = metaData.file_name,
				imageMeta = meta,
				raw_data = new ArraySegment<byte>(image),
				file_create_time = string.IsNullOrEmpty(metaData.file_create_time) ? (DateTime?)null : TimeHelper.ParseCloudTimeString(metaData.file_create_time)
			};

			if (meta == ImageMeta.Origin)
			{
				//string takenTime = extractTakenTimeFromImageExif(image);
				return storage.Save(param, metaData.event_time.ToUTCISO8601ShortString());
			}
			else
			{
				return storage.Save(param, null);
			}
		}

		private static string extractTakenTimeFromImageExif(byte[] image)
		{
			try
			{
				var exif = ExifLibrary.ExifFile.Read(image);
				string takenTime = null;
				if (exif.Properties.ContainsKey(ExifLibrary.ExifTag.DateTimeOriginal))
					takenTime = ((DateTime)(exif.Properties[ExifLibrary.ExifTag.DateTimeOriginal].Value)).ToUTCISO8601ShortString();
				return takenTime;
			}
			catch
			{
				return null;
			}
		}

		private static DateTime? ConvertISO8601ToDateTime(string dt)
		{
			try
			{
				return TimeHelper.ParseCloudTimeString(dt);
			}
			catch
			{
				return null;
			}
		}

		public static void SaveToAttachmentDB(ImageMeta meta, string saveFileName, AttachmentInfo attachmentAttributes, long length)
		{
			// gps info is moved out of exif structure in cloud response but station still keeps gps info inside exif in station db.
			if (attachmentAttributes.image_meta != null && attachmentAttributes.image_meta.exif != null)
				attachmentAttributes.image_meta.exif.gps = attachmentAttributes.image_meta.gps;

			if (meta == ImageMeta.Origin || attachmentAttributes.type.Equals("doc", StringComparison.InvariantCultureIgnoreCase))
			{
				var update = Update.Set("url", "/v3/attachments/view/?object_id=" + attachmentAttributes.object_id)
								.Set("mime_type", attachmentAttributes.mime_type)
								.Set("file_size", length)
								.Set("modify_time", DateTime.UtcNow)
								.Set("type", (int)(AttachmentType)Enum.Parse(typeof(AttachmentType), attachmentAttributes.type, true))
								.Set("group_id", attachmentAttributes.group_id)
								.Set("saved_file_name", saveFileName)
								.Set("body_on_cloud", true);

				if (attachmentAttributes.image_meta != null)
				{
					update.Set("image_meta.width", attachmentAttributes.image_meta.width)
						.Set("image_meta.height", attachmentAttributes.image_meta.height);
				}

				if (attachmentAttributes.doc_meta != null)
				{
					update.Set("doc_meta", attachmentAttributes.doc_meta.ToBsonDocument());
				}

				if (attachmentAttributes.web_meta != null)
				{
					update.Set("web_meta", attachmentAttributes.web_meta.ToBsonDocument());
				}

				setOptionalAttributes(attachmentAttributes, update);

				AttachmentCollection.Instance.Update(Query.EQ("_id", attachmentAttributes.object_id),
					update, UpdateFlags.Upsert);
			}
			else
			{
				string metaStr = meta.GetCustomAttribute<DescriptionAttribute>().Description;

				var thumbnail = new ThumbnailInfo
				{
					mime_type = attachmentAttributes.GetThumbnail(meta).mime_type,
					modify_time = DateTime.UtcNow,
					url = "/v3/attachments/view/?object_id=" + attachmentAttributes.object_id + "&image_meta=" + metaStr,
					file_size = length,
					width = attachmentAttributes.GetThumbnail(meta).width,
					height = attachmentAttributes.GetThumbnail(meta).height,
					saved_file_name = saveFileName
				};

				var update = Update.Set("group_id", attachmentAttributes.group_id)
								.Set("type", (int)(AttachmentType)Enum.Parse(typeof(AttachmentType), attachmentAttributes.type, true))
								.Set("image_meta." + metaStr, thumbnail.ToBsonDocument())
								.Set("body_on_cloud", !string.IsNullOrEmpty(attachmentAttributes.url));

				setOptionalAttributes(attachmentAttributes, update);

				AttachmentCollection.Instance.Update(Query.EQ("_id", attachmentAttributes.object_id),
						update, UpdateFlags.Upsert);
			}
		}

		private static void setOptionalAttributes(AttachmentInfo attachmentAttributes, UpdateBuilder update)
		{
			if (!string.IsNullOrEmpty(attachmentAttributes.file_name))
				update.Set("file_name", attachmentAttributes.file_name);

			if (!string.IsNullOrEmpty(attachmentAttributes.creator_id))
				update.Set("creator_id", attachmentAttributes.creator_id);

			if (attachmentAttributes.event_time > DateTime.MinValue)
				update.Set("event_time", attachmentAttributes.event_time);

			if (!string.IsNullOrEmpty(attachmentAttributes.md5))
				update.Set("md5", attachmentAttributes.md5);

			if (!string.IsNullOrEmpty(attachmentAttributes.file_create_time))
				update.Set("file_create_time", ConvertISO8601ToDateTime(attachmentAttributes.file_create_time));

			if (!string.IsNullOrEmpty(attachmentAttributes.import_time))
				update.Set("import_time", ConvertISO8601ToDateTime(attachmentAttributes.import_time));

			if (!string.IsNullOrEmpty(attachmentAttributes.file_path))
				update.Set("file_path", attachmentAttributes.file_path);

			if (attachmentAttributes.image_meta != null && attachmentAttributes.image_meta.exif != null)
				update.Set("image_meta.exif", attachmentAttributes.image_meta.exif.ToBsonDocument());

			if (!string.IsNullOrEmpty(attachmentAttributes.device_id))
				update.Set("device_id", attachmentAttributes.device_id);
		}

		protected override void Run()
		{
			string meta = evtargs.imagemeta.ToString();
			string oldFile = evtargs.filepath;
			string user_id = null;
			downloadCount.Increment();

			try
			{
				bool alreadyExist = AttachmentExists(evtargs);

				if (alreadyExist)
				{
					return;
				}

				Driver user = DriverCollection.Instance.FindOne(Query.EQ("_id", evtargs.user_id));
				if (user == null)
				{
					logger.Info("drop download task because user does not exist anymore: " + evtargs.user_id);
					return;
				}

				user_id = user.user_id;

				if ((evtargs.imagemeta == ImageMeta.Origin || evtargs.imagemeta == ImageMeta.None) && user.ReachOriginSizeLimit())
				{
					logger.DebugFormat("origin size limit {} is reached. Skip", user.origin_limit);
					return;
				}

				var api = new AttachmentApi(user);
				api.AttachmentView(evtargs, StationRegistry.StationId);
				DownloadComplete(evtargs, user);

				DriverCollection.Instance.Update(Query.EQ("_id", user_id), Update.Unset("sync_range.download_error"));
			}
			catch (Exception e)
			{
				var err = e.Message;

				if (e is WammerCloudException)
				{
					var cloudErr = (WammerCloudException)e;

					if (cloudErr.HttpError == System.Net.WebExceptionStatus.ProtocolError)
						err = cloudErr.GetCloudRetMsg();
					else if (cloudErr.InnerException != null)
						err = cloudErr.InnerException.Message;
					else
						err = cloudErr.Message;
				}

				DriverCollection.Instance.Update(Query.EQ("_id", user_id), Update.Set("sync_range.download_error", err));


				string msg = string.Format("Unabel to download attachment {0} meta {1}: {2}", evtargs.attachment.object_id, meta, e.ToString());

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
			BodySyncQueue.Instance.EnqueueAlways(this, priority);
		}
	}
}
