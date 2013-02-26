using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;
using System.ComponentModel;
using System.Drawing;
using Waveface.Stream.Model;

namespace Wammer.Station.AttachmentUpload
{
	[Serializable]
	internal class AttachmentUtility : IAttachmentUtil
	{
		private static readonly IPerfCounter upstreamRateCounter = PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE);

		private Guid? importTaskId;

		public AttachmentUtility(Guid? importTaskId = null)
		{
			this.importTaskId = importTaskId;
		}

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
			var fs = new FileStorage(user);

			using (var image = new Bitmap(fs.Load(imageFilename, from)))
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
			incUploadSizeIfNeeded(object_id, meta);

			AttachmentUploadQueueHelper.Instance.Enqueue(new UpstreamTask(object_id, meta, priority, importTaskId), priority);
		}

		private void incUploadSizeIfNeeded(string object_id, ImageMeta meta)
		{
			try
			{
				if (importTaskId.HasValue)
				{
					var att = AttachmentCollection.Instance.FindOneById(object_id);
					if (att != null)
					{

						var info = att.GetInfoByMeta(meta);
						var uploadSize = info.file_size;

						TaskStatusCollection.Instance.Update(
							Query.EQ("_id", importTaskId),
							Update.Inc("UploadSize", uploadSize).Inc("UploadCount", 1));
					}
				}
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Unable to update import task's upload size", e);
			}
		}

		public void GenerateThumbnailAsync(string object_id, ImageMeta thumbnailType, TaskPriority priority)
		{
			TaskQueue.Enqueue(new MakeThumbnailTask(object_id, thumbnailType, priority, importTaskId), priority, true);
		}

		public void GenerateThumbnailAsyncAndUpstream(string object_id, ImageMeta thumbnailType, TaskPriority priority)
		{
			TaskQueue.Enqueue(new MakeThumbnailAndUpstreamTask(object_id, thumbnailType, priority, importTaskId), priority, true);
		}
		#endregion

		private void UpstreamProgressChanged(object sender, ProgressChangedEventArgs evt)
		{
			upstreamRateCounter.IncrementBy(Convert.ToInt32(evt.UserState));
		}
	}
}