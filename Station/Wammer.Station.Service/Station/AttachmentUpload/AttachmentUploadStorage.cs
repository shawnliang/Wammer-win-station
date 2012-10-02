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
				var cacheFolder = "cache";

				if (!Directory.Exists(cacheFolder))
					Directory.CreateDirectory(cacheFolder);

				var tmpFile = Guid.NewGuid().ToString();
				var tmpPath = Path.Combine(cacheFolder, tmpFile);

				using (var tmp = File.OpenWrite(tmpPath))
				{
					tmp.Write(data.raw_data.Array, data.raw_data.Offset, data.raw_data.Count);
				}

				var destPath = Path.Combine(cacheFolder, filename);
				File.Move(tmpPath, destPath);

				return new AttachmentSaveResult("", destPath);
			}
			else
			{
				DateTime fileTime;

				if (!string.IsNullOrEmpty(takenTime))
				{
					fileTime = TimeHelper.ParseCloudTimeString(takenTime).ToLocalTime();
				}
				else if (data.file_create_time.HasValue)
				{
					fileTime = data.file_create_time.Value.ToLocalTime();
				}
				else
				{
					fileTime = DateTime.Now;
				}


				//string L1 = Path.Combine(storage.basePath, fileTime.Year.ToString("d4"));
				string L1 = fileTime.Year.ToString("d4");
				string L2 = Path.Combine(L1, fileTime.Month.ToString("d2"));
				string L3 = Path.Combine(L2, fileTime.Day.ToString("d2"));

				//if (!Directory.Exists(L1))
				//    Directory.CreateDirectory(L1);
				//if (!Directory.Exists(L2))
				//    Directory.CreateDirectory(L2);
				//if (!Directory.Exists(L3))
				//    Directory.CreateDirectory(L3);

				//string relativePath = Path.Combine(L3.Remove(0, storage.basePath.Length), data.file_name);
				//if (relativePath.StartsWith(@"\"))
				//    relativePath = relativePath.Substring(1);

				var relativePath = Path.Combine(L3, data.file_name);
				relativePath = storage.TrySaveFile(relativePath, data.raw_data);

				return new AttachmentSaveResult(storage.basePath, relativePath);
			}

			
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
