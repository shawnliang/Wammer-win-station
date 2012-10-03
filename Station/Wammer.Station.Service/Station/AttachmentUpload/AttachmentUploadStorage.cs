using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using System.ComponentModel;
using System.IO;
using Wammer.Utility;

namespace Wammer.Station.AttachmentUpload
{
	public interface IAttachmentUploadStorageDB
	{
		Driver GetUserByGroupId(string groupId);
	}

	public class AttachmentUploadStorage: IAttachmentUploadStorage
	{
		private readonly IAttachmentUploadStorageDB db;

		public AttachmentUploadStorage(IAttachmentUploadStorageDB db)
		{
			this.db = db;
		}

		public AttachmentSaveResult Save(UploadData data, string takenTime)
		{
			var user = db.GetUserByGroupId(data.group_id);
			var storage = new FileStorage(user);
			

			if (data.imageMeta != ImageMeta.Origin && data.imageMeta != ImageMeta.None)
			{
				var filename = data.object_id + "_" + data.imageMeta.GetCustomAttribute<DescriptionAttribute>().Description + ".dat";

				var savedPath = FileStorage.SaveToCacheFolder(user.user_id, filename, data.raw_data);
				return new AttachmentSaveResult("", savedPath);
			}
			else
			{
				var folderPath = getFolderByDate(data, takenTime);
				var relativePath = Path.Combine(folderPath, data.file_name);
				relativePath = storage.TrySaveFile(relativePath, data.raw_data);

				return new AttachmentSaveResult(storage.basePath, relativePath);
			}
		}

		private static string getFolderByDate(UploadData data, string takenTime)
		{
			DateTime fileTime;

			if (!string.IsNullOrEmpty(takenTime))
				fileTime = TimeHelper.ParseCloudTimeString(takenTime).ToLocalTime();
			else if (data.file_create_time.HasValue)
				fileTime = data.file_create_time.Value.ToLocalTime();
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
