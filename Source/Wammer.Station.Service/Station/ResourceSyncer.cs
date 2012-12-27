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
				TaskQueue.Enqueue(new QueryIfDownstreamNeededTask(e.user_id, e.object_id), TaskPriority.Medium);
			}
			catch (Exception ex)
			{
				this.LogWarnMsg("Unable to enqueue body download task", ex);
			}
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