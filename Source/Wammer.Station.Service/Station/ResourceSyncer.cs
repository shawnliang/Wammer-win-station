using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station.Timeline;
using Waveface.Stream.Model;

namespace Wammer.Station
{
	internal class ResourceSyncer : NonReentrantTimer
	{
		private readonly ResourceDownloader downloader;
		private readonly TimelineSyncer syncer;

		public ResourceSyncer(long timerPeriod, ITaskEnqueuable<IResourceDownloadTask> bodySyncQueue)
			: base(timerPeriod)
		{
			downloader = new ResourceDownloader(bodySyncQueue);
			syncer = new TimelineSyncer(new PostProvider(), new TimelineSyncerDB(), new ChangeLogsApi());
			syncer.PostsRetrieved += downloader.PostRetrieved;
			syncer.AttachmentAvailable += syncer_AttachmentAvailable;
		}

		private void syncer_AttachmentAvailable(object sender, AttachmentAvailableEventArgs e)
		{
			try
			{
				Driver user = DriverCollection.Instance.FindOne(Query.EQ("_id", e.user_id));

				if (user == null)
					return;

				var localAtt = AttachmentCollection.Instance.FindOneById(e.object_id);

				if ((e.meta == ImageMeta.Origin || e.meta == ImageMeta.None) &&
					user.isPrimaryStation && localHasNoOrigin(localAtt))
				{
					var info = AttachmentApi.GetInfo(e.object_id, user.session_token);
					downloader.EnqueueDownstreamTask(info, user, ImageMeta.Origin);
				}
				else if (e.meta == ImageMeta.Medium &&
					localHasNoMedium(localAtt))
				{
					var info = AttachmentApi.GetInfo(e.object_id, user.session_token);
					downloader.EnqueueDownstreamTask(info, user, ImageMeta.Medium);
					downloader.EnqueueDownstreamTask(info, user, ImageMeta.Small);
				}
				else if (e.meta == ImageMeta.None && localHasNotPreviews(localAtt))
				{
					var task = new DownloadDocPreviewsTask(e.object_id);
					TaskQueue.Enqueue(task, task.Priority);
				}
			}
			catch (Exception ex)
			{
				this.LogWarnMsg("Unable to enqueue body download task", ex);
			}
		}

		private static bool localHasNotPreviews(Attachment localAtt)
		{
			return localAtt == null || localAtt.doc_meta == null ||
				localAtt.doc_meta.preview_files == null ||
				localAtt.doc_meta.preview_files.Count == 0;
		}

		private static bool localHasNoMedium(Attachment localAtt)
		{
			return localAtt == null || localAtt.image_meta == null || localAtt.image_meta.medium == null ||
					string.IsNullOrEmpty(localAtt.image_meta.medium.saved_file_name);
		}

		private static bool localHasNoOrigin(Attachment localAtt)
		{
			return localAtt == null || string.IsNullOrEmpty(localAtt.saved_file_name);
		}

		protected override void ExecuteOnTimedUp(object state)
		{
			PullTimeline();
		}

		private void PullTimeline()
		{
			MongoCursor<Driver> users = DriverCollection.Instance.FindAll();

			foreach (Driver user in users)
			{
				if (string.IsNullOrEmpty(user.session_token))
					continue;
				try
				{
					if (syncer.PullTimeline(user))
						Station.Instance.PostUpsertNotifier.NotifyUser(user.user_id);
				}
				catch (Exception e)
				{
					this.LogDebugMsg("Unable to sync timeline of user " + user.email, e);
				}
			}
		}
	}
}