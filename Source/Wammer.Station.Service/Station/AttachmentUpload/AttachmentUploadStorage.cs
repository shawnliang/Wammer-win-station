using MongoDB.Driver.Builders;
using System;
using System.ComponentModel;
using System.IO;
using Waveface.Stream.Model;

namespace Wammer.Station.AttachmentUpload
{
	public interface IAttachmentUploadStorageDB
	{
		Driver GetUserByGroupId(string groupId);
	}

	public class AttachmentUploadStorage : IAttachmentUploadStorage
	{
		private readonly IAttachmentUploadStorageDB db;

		public AttachmentUploadStorage(IAttachmentUploadStorageDB db)
		{
			this.db = db;
		}

		public AttachmentSaveResult Save(UploadData data, string takenTime)
		{
			var user = db.GetUserByGroupId(data.group_id);
			if (user == null)
				throw new WammerStationException("driver does not exist", -1);

			var storage = new FileStorage(user);


			if (data.imageMeta != ImageMeta.Origin && data.imageMeta != ImageMeta.None)
			{
				var filename = data.object_id + "_" + data.imageMeta.GetCustomAttribute<DescriptionAttribute>().Description + ".dat";

				var savedPath = FileStorage.SaveToCacheFolder(user.user_id, filename, data.raw_data);
				return new AttachmentSaveResult("", savedPath);
			}
			else
			{
				var relativeFile = GetAttachmentRelativeFile(data.file_name, takenTime, data.file_create_time);
				relativeFile = storage.TrySaveFile(relativeFile, data.raw_data, data.object_id);

				DriverCollection.Instance.Update(
					Query.EQ("_id", user.user_id),
					Update.Inc("cur_origin_size", data.raw_data.Count));

				if (data.fromLocal)
				{
					updateFileModifyTime(data, Path.Combine(storage.basePath, relativeFile));
				}

				return new AttachmentSaveResult(storage.basePath, relativeFile);
			}
		}

		private static void updateFileModifyTime(UploadData data, string savedFile)
		{
			try
			{
				var lastWriteTime = File.GetLastWriteTime(data.file_path);
				File.SetLastWriteTime(savedFile, lastWriteTime);
			}
			catch
			{
			}
		}

		public static string GetAttachmentRelativeFile(string fileName, string eventTime, DateTime? fileCreateTime = null, string userID = null, string attachmentID = null, ImageMeta meta = ImageMeta.None)
		{
			var folderPath = GetAttachmentRelativeFolder(eventTime, fileCreateTime, userID, meta);

			var relativePathFile = default(string);

			if (meta != ImageMeta.Origin && meta != ImageMeta.None)
			{
				relativePathFile = Path.Combine(folderPath, attachmentID + "_" + meta.GetCustomAttribute<DescriptionAttribute>().Description + ".dat");
			}
			else
			{
				relativePathFile = Path.Combine(folderPath, fileName);
			}

			return relativePathFile;
		}

		public static string GetAttachmentRelativeFolder(string eventTime, DateTime? fileCreateTime = null, string userID = null, ImageMeta meta = ImageMeta.None)
		{
			var folderPath = default(string);
			if (meta != ImageMeta.Origin && meta != ImageMeta.None)
			{
				folderPath = FileStorage.GetCachePath(userID);
			}
			else
			{
				folderPath = getFolderByDate(eventTime, fileCreateTime);
			}
			return folderPath;
		}

		private static string getFolderByDate(string eventTime, DateTime? fileCreateTime = null)
		{
			DateTime fileTime;

			if (!string.IsNullOrEmpty(eventTime))
				fileTime = TimeHelper.ParseGeneralDateTime(eventTime).ToLocalTime();
			else if (fileCreateTime.HasValue)
				fileTime = fileCreateTime.Value.ToLocalTime();
			else
				fileTime = DateTime.Now;

			var yyyyDir = fileTime.Year.ToString("d4");
			string yyyyMMDir = Path.Combine(yyyyDir, fileTime.Month.ToString("d2"));

			return Path.Combine(yyyyMMDir, fileTime.Day.ToString("d2"));
		}
	}

	class AttachmentUploadStorageDB : IAttachmentUploadStorageDB
	{
		public Driver GetUserByGroupId(string groupId)
		{
			return DriverCollection.Instance.FindDriverByGroupId(groupId);
		}
	}
}
