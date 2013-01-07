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
		private readonly TimelineSyncer syncer = new TimelineSyncer();

		public ResourceSyncer(long timerPeriod, ITaskEnqueuable<IResourceDownloadTask> bodySyncQueue)
			: base(timerPeriod)
		{
			downloader = new ResourceDownloader(bodySyncQueue);
			syncer.AttachmentModified += new EventHandler<AttachmentModifiedEventArgs>(syncer_AttachmentModified);
			syncer.PostsRetrieved += new EventHandler<TimelineSyncEventArgs>(syncer_PostsRetrieved);
		}

		void syncer_PostsRetrieved(object sender, TimelineSyncEventArgs e)
		{
			DriverCollection.Instance.Update(Query.EQ("_id", e.Driver.user_id), Update.Set("sync_range.syncing", true).Unset("sync_range.error"));

			foreach (var post in e.Posts)
			{
				Post.Save(post);
			}
		}

		void syncer_AttachmentModified(object sender, AttachmentModifiedEventArgs e)
		{
			DriverCollection.Instance.Update(Query.EQ("_id", e.user_id), Update.Set("sync_range.syncing", true).Unset("sync_range.error"));

			foreach (var item in e.attachments)
			{
				var check = new QueryIfDownstreamNeededTask(e.user_id, item.object_id) { cloudDoc = item };
				check.Execute();
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

				if (!user.sync_range.enable)
					continue;

				try
				{
					if (SyncTimeline(user))
						Station.Instance.PostUpsertNotifier.NotifyUser(user.user_id);

					DriverCollection.Instance.Update(Query.EQ("_id", user.user_id),
						Update.Set("sync_range.syncing", false).Unset("sync_range.error"));
				}
				catch (Exception e)
				{
					this.LogDebugMsg("Unable to sync timeline of user " + user.email, e);
					DriverCollection.Instance.Update(Query.EQ("_id", user.user_id),
						Update.Set("sync_range.syncing", false).Set("sync_range.error", e.Message));
				}
			}
		}

		public bool SyncTimeline(Driver user)
		{
			return syncer.PullTimeline(user);
		}

		public static void EnablePeriodicalSync(string user_id)
		{
			DriverCollection.Instance.Update(
				Query.EQ("_id", user_id),
				Update.Set("sync_range.enable", true));
		}
	}
}