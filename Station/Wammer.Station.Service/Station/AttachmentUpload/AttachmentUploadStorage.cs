using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using System.ComponentModel;
using System.IO;

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

		public AttachmentSaveResult Save(UploadData data)
		{
			var user = db.GetUserByGroupId(data.group_id);
			var storage = new FileStorage(user);
			var filename = GetSavedFileName(data);
			storage.SaveFile(filename, data.raw_data);

			return new AttachmentSaveResult(storage.basePath, filename);
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
	}

	class AttachmentUploadStorageDB : IAttachmentUploadStorageDB
	{
		public Driver GetUserByGroupId(string groupId)
		{
			return DriverCollection.Instance.FindDriverByGroupId(groupId);
		}
	}
}
