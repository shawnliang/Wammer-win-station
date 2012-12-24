using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PerfMonitor;
using Wammer.Utility;
using Waveface.Stream.Model;

namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	internal class AttachmentUtility : IAttachmentUtil
	{
		private static readonly IPerfCounter upstreamRateCounter = PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE);

		#region IAttachmentUtil Members

		public Attachment FindAttachmentInDB(string object_id)
		{
			return AttachmentCollection.Instance.FindOne(Query.EQ("_id", object_id));
		}

		public Driver FindUserByGroupIdInDB(string group_id)
		{
			return DriverCollection.Instance.FindDriverByGroupId(group_id);
		}

		public ThumbnailInfo GenerateThumbnail(string imageFilename, ImageMeta thumbnailType, string object_id, Driver user,
											   string origin_filename, ImageMeta from)
		{
			var fileStorage = new FileStorage(user);

			using (FileStream f = fileStorage.Load(imageFilename, from))
			using (var image = new Bitmap(f))
			{
				return ImagePostProcessing.MakeThumbnail(image, thumbnailType, ExifOrientations.Unknown, object_id, user,
														 origin_filename);
			}
		}

		public ThumbnailInfo GenerateThumbnail(string imageFilename, ImageMeta thumbnailType, string object_id, Driver user,
											   string origin_filename)
		{
			return GenerateThumbnail(imageFilename, thumbnailType, object_id, user, origin_filename, ImageMeta.Origin);
		}

		public void UpdateThumbnailInfoToDB(string object_id, ImageMeta thumbnailType, ThumbnailInfo Info)
		{
			AttachmentCollection.Instance.Update(
				Query.EQ("_id", object_id),
				Update.Set(
					"image_meta." + thumbnailType.ToString().ToLower(),
					Info.ToBsonDocument()));
		}


		public void UpstreamAttachmentAsync(string object_id, ImageMeta meta, TaskPriority priority)
		{
			AttachmentUploadQueueHelper.Instance.Enqueue(new UpstreamTask(object_id, meta, priority), priority);
		}

		public void GenerateThumbnailAsync(string object_id, ImageMeta thumbnailType, TaskPriority priority)
		{
			TaskQueue.Enqueue(new MakeThumbnailTask(object_id, thumbnailType, priority), priority, true);
		}

		public void GenerateThumbnailAsyncAndUpstream(string object_id, ImageMeta thumbnailType, TaskPriority priority)
		{
			TaskQueue.Enqueue(new MakeThumbnailAndUpstreamTask(object_id, thumbnailType, priority), priority, true);
		}
		#endregion

		private void UpstreamProgressChanged(object sender, ProgressChangedEventArgs evt)
		{
			upstreamRateCounter.IncrementBy(Convert.ToInt32(evt.UserState));
		}
	}
}