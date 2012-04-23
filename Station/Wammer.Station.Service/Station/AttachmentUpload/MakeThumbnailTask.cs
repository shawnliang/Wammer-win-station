using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace Wammer.Station.AttachmentUpload
{
	class MakeThumbnailTask : ITask
	{
		private string object_id;
		private ImageMeta thumbnail_type;

		public MakeThumbnailTask(string object_id, ImageMeta thumbnail_type)
		{
			if (thumbnail_type == ImageMeta.Origin || thumbnail_type == ImageMeta.None)
				throw new ArgumentException("thumbnail_type");

			this.object_id = object_id;
			this.thumbnail_type = thumbnail_type;
		}

		public void Execute()
		{
			Attachment attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", this.object_id));
			if (attachment == null)
				return;

			Driver user = DriverCollection.Instance.FindDriverByGroupId(attachment.group_id);
			if (user == null)
				return;

			IAttachmentInfo info = attachment.GetInfoByMeta(this.thumbnail_type);
			if (info == null)
				return;

			AttachmentUtility imgProc = new AttachmentUtility();
			ThumbnailInfo thumbnail = imgProc.GenerateThumbnail(info.saved_file_name, thumbnail_type,
				this.object_id, user, attachment.file_name);

			imgProc.UpdateThumbnailInfoToDB(this.object_id, this.thumbnail_type, thumbnail);
		}
	}
}
