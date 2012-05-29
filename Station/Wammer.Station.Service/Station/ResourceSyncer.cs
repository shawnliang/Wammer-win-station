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
			syncer = new TimelineSyncer(new PostProvider(), new TimelineSyncerDB(), new UserTracksApi());
			syncer.PostsRetrieved += downloader.PostRetrieved;
			syncer.BodyAvailable += syncer_BodyAvailable;
		}

		private void syncer_BodyAvailable(object sender, BodyAvailableEventArgs e)
		{
			try
			{
				Driver user = DriverCollection.Instance.FindOne(Query.EQ("_id", e.user_id));
				if (user != null && user.isPrimaryStation)
				{
					var info = AttachmentApi.GetInfo(e.object_id, user.session_token);
					downloader.EnqueueDownstreamTask(info, user, ImageMeta.Origin);
				}
			}
			catch (Exception ex)
			{
				this.LogWarnMsg("Unable to enqueue body download task", ex);
			}
		}

		protected override void ExecuteOnTimedUp(object state)
		{
			if (isFirstRun)
			{
				downloader.ResumeUnfinishedDownstreamTasks();
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
				try
				{
					syncer.PullTimeline(user);
				}
				catch (Exception e)
				{
					this.LogDebugMsg("Unable to sync timeline of user " + user.email, e);
				}
			}
		}
	}
}