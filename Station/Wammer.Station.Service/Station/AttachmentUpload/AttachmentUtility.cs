using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using Wammer.Cloud;
using Wammer.PerfMonitor;
using Wammer.Utility;

namespace Wammer.Station.AttachmentUpload
{
	class AttachmentUtility : IAttachmentUtil
	{
		private static IPerfCounter uploadTaskCounter = PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT, false);
		private static IPerfCounter upstreamRateCounter = PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE, false);

		public Model.Attachment FindAttachmentInDB(string object_id)
		{
			return Model.AttachmentCollection.Instance.FindOne(Query.EQ("_id", object_id));
		}

		public Model.Driver FindUserByGroupIdInDB(string group_id)
		{
			return Model.DriverCollection.Instance.FindDriverByGroupId(group_id);
		}

		public Model.ThumbnailInfo GenerateThumbnail(string imageFilename, Model.ImageMeta thumbnailType, string object_id, Model.Driver user, string origin_filename)
		{
			FileStorage fileStorage = new FileStorage(user);

			using (FileStream f = fileStorage.Load(imageFilename))
			using (Bitmap image = new Bitmap(f))
			{
				return ImagePostProcessing.MakeThumbnail(image, thumbnailType, ExifOrientations.Unknown, object_id, user, origin_filename);
			}
		}

		public void UpstreamImageNow(byte[] imageRaw, string group_id, string object_id, string file_name, string mime_type, Model.ImageMeta meta, string apikey, string session_token)
		{
			Attachment.UploadImage(new ArraySegment<byte>(imageRaw), group_id, object_id, file_name, mime_type, meta, apikey, session_token,
					1024, UpstreamProgressChanged);
		}

		public void UpdateThumbnailInfoToDB(string object_id, Model.ImageMeta thumbnailType, Model.ThumbnailInfo Info)
		{
			AttachmentCollection.Instance.Update(
				Query.EQ("_id", object_id),
				Update.Set(
					"image_meta." + thumbnailType.ToString().ToLower(),
					Info.ToBsonDocument()));
		}


		public void UpstreamAttachmentAsync(string object_id, ImageMeta meta, TaskPriority priority)
		{
			uploadTaskCounter.Increment();
			TaskQueue.Enqueue(new UpstreamTask(object_id, meta, priority), priority, true);
		}

		public void GenerateThumbnailAsync(string object_id, ImageMeta thumbnailType, TaskPriority priority)
		{
			uploadTaskCounter.Increment();
			TaskQueue.Enqueue(new MakeThumbnailTask(object_id, thumbnailType), priority, true);
		}


		public void UpstreamAttachmentNow(string filename, Driver user, string object_id, string file_name, string mime_type, ImageMeta meta, AttachmentType type)
		{
			FileStorage fileStorage = new FileStorage(user);

			using (FileStream f = fileStorage.Load(filename))
			{
				Attachment.Upload(f, user.groups[0].group_id, object_id, filename, mime_type, meta, type, CloudServer.APIKey, user.session_token, 1024, UpstreamProgressChanged);
			}
		}

		private void UpstreamProgressChanged(object sender, ProgressChangedEventArgs evt)
		{
			upstreamRateCounter.IncrementBy(Convert.ToInt32(evt.UserState));
		}
	}
}
