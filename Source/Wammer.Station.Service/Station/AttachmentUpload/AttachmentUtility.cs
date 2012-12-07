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

		public void UpstreamImageNow(byte[] imageRaw, string group_id, string object_id, string file_name, string mime_type,
									 ImageMeta meta, string apikey, string session_token)
		{
			using (var s = new MemoryStream(imageRaw))
			{
				AttachmentApi.Upload(s, group_id, object_id, file_name, mime_type, meta, AttachmentType.image, apikey, session_token, 1024, UpstreamProgressChanged);
			}
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
			TaskQueue.Enqueue(new MakeThumbnailAndUpstreamTask(object_id, thumbnailType, priority, this), priority, true);
		}


		public void UpstreamAttachmentNow(string filename, Driver user, string object_id, string origFileName,
										  string mime_type, ImageMeta meta, AttachmentType type, string session, string apikey)
		{
			var fileStorage = new FileStorage(user);

			using (FileStream f = fileStorage.Load(filename, meta))
			{
				AttachmentApi.Upload(f, user.groups[0].group_id, object_id, origFileName, mime_type, meta, type, apikey,
								  session, 1024, UpstreamProgressChanged);
			}
		}
		#endregion

		private void UpstreamProgressChanged(object sender, ProgressChangedEventArgs evt)
		{
			upstreamRateCounter.IncrementBy(Convert.ToInt32(evt.UserState));
		}
	}
}