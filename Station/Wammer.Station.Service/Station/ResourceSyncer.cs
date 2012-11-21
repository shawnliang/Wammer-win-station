using System;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station.Timeline;

namespace Wammer.Station
{
	internal class ResourceSyncer : NonReentrantTimer
	{
		private readonly ResourceDownloader downloader;
		private readonly TimelineSyncer syncer;
		private bool isFirstRun = true;

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

				if (e.meta == ImageMeta.Origin && user.isPrimaryStation &&
					localHasNoOrigin(localAtt))
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
			}
			catch (Exception ex)
			{
				this.LogWarnMsg("Unable to enqueue body download task", ex);
			}
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
			if (isFirstRun)
			{
				System.Threading.ThreadPool.QueueUserWorkItem(
					(s) => downloader.ResumeUnfinishedDownstreamTasks());

				isFirstRun = false;
			}

			PullTimeline();
		}

		public void OnIsPrimaryChanged(object sender, IsPrimaryChangedEvtArgs args)
		{
			try
			{
				if (args.driver.isPrimaryStation)
				{
					// just upgraded to primary station
					foreach (Attachment attachment in AttachmentCollection.Instance.Find(Query.EQ("group_id", args.driver.groups[0].group_id)))
					{
						var syncedOriginalAttachments = new List<string>();

						if (string.IsNullOrEmpty(attachment.saved_file_name))
							// download missing original attachments
							downloader.EnqueueDownstreamTask(new AttachmentInfo(attachment), args.driver, ImageMeta.Origin);
						else
							syncedOriginalAttachments.Add(attachment.object_id);

						if (syncedOriginalAttachments.Count > 0)
						{
							TaskQueue.Enqueue(
								   new NotifyCloudOfMultiBodySyncedTask(syncedOriginalAttachments, args.driver.user_id),
								   TaskPriority.Low, true);
						}
					}
				}
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Error when adding downstream task", e);
			}
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