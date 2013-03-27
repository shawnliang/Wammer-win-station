using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Linq;
using Wammer.Station.AttachmentUpload;
using Wammer.Station.Timeline;
using Waveface.Stream.Model;

namespace Wammer.Station
{
	internal class ResourceSyncer : NonReentrantTimer
	{
		private readonly TimelineSyncer syncer = new TimelineSyncer();

		public ResourceSyncer(long timerPeriod, ITaskEnqueuable<IResourceDownloadTask> bodySyncQueue)
			: base(timerPeriod)
		{
			syncer.AttachmentModified += new EventHandler<AttachmentModifiedEventArgs>(syncer_AttachmentModified);
			syncer.PostsRetrieved += new EventHandler<TimelineSyncEventArgs>(syncer_PostsRetrieved);
			syncer.AttachmentDelete += syncer_AttachmentDelete;
		}


		void syncer_PostsRetrieved(object sender, TimelineSyncEventArgs e)
		{
			DriverCollection.Instance.Update(Query.EQ("_id", e.Driver.user_id), Update.Set("sync_range.syncing", true).Unset("sync_range.download_index_error"));

			foreach (var post in e.Posts)
			{
				Post.Save(post);
			}
		}

		void syncer_AttachmentModified(object sender, AttachmentModifiedEventArgs e)
		{
			DriverCollection.Instance.Update(Query.EQ("_id", e.user_id), Update.Set("sync_range.syncing", true).Unset("sync_range.download_index_error"));

			foreach (var item in e.attachments)
			{
				var check = new QueryIfDownstreamNeededTask(e.user_id, item.object_id) { cloudDoc = item };

				try
				{
					check.Execute();
				}
				catch (Exception err)
				{
					this.LogWarnMsg("Failed to query if download is needed. Retry later", err);
				}
			}
		}

		void syncer_AttachmentDelete(object sender, AttachmentDeleteEventArgs e)
		{
			var task = new AttachmentDeleteTask(e.attachmentIDs.ToList(), e.user_id, false);
			AttachmentUploadQueueHelper.Instance.Enqueue(task, TaskPriority.High);
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
				}
				catch (Exception e)
				{
					this.LogWarnMsg("Unable to sync timeline of user " + user.email, e);

				}
			}
		}

		public bool SyncTimeline(Driver user, Boolean firstSync = false)
		{
			try
			{
				bool changed = syncer.PullTimeline(user, firstSync);

				DriverCollection.Instance.Update(Query.EQ("_id", user.user_id),
						Update.Set("sync_range.syncing", false).Unset("sync_range.download_index_error"));


				return changed;
			}
			catch (Exception e)
			{
				var err = e.GetDisplayDescription();

				DriverCollection.Instance.Update(Query.EQ("_id", user.user_id),
						Update.Set("sync_range.syncing", false).Set("sync_range.download_index_error", err));

				throw;
			}
		}

		public static void EnablePeriodicalSync(string user_id)
		{
			DriverCollection.Instance.Update(
				Query.EQ("_id", user_id),
				Update.Set("sync_range.enable", true));
		}
	}
}