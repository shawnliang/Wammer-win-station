using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station.Timeline
{
	public class TimelineSyncerDB : ITimelineSyncerDB
	{
		#region ITimelineSyncerDB Members

		public void SavePost(PostInfo post)
		{
			PostCollection.Instance.Save(post);
		}

		public void UpdateDriverSyncRange(string userId, SyncRange syncRange)
		{
			DriverCollection.Instance.Update(
				Query.EQ("_id", userId),
				Update.Set("sync_range", syncRange.ToBsonDocument()));
		}

		public void UpdateDriverChangeHistorySynced(string userId, bool synced)
		{
			DriverCollection.Instance.Update(
				Query.EQ("_id", userId),
				Update.Set("is_change_history_synced", synced));
		}

		public void SaveUserTracks(UserTracks userTracks)
		{
			UserTrackCollection.Instance.Save(userTracks);
		}

		#endregion
	}

	// update driver in memory instead of in database
	public class TimelineSyncerDBWithDriverCached : ITimelineSyncerDB
	{
		private readonly Driver driver;

		public TimelineSyncerDBWithDriverCached(Driver driver)
		{
			this.driver = driver;
		}

		#region ITimelineSyncerDB Members

		public void SavePost(PostInfo post)
		{
			PostCollection.Instance.Save(post);
		}

		public void UpdateDriverSyncRange(string userId, SyncRange syncRange)
		{
			driver.sync_range = syncRange;
		}

		public void UpdateDriverChangeHistorySynced(string userId, bool synced)
		{
			driver.is_change_history_synced = synced;
		}

		public void SaveUserTracks(UserTracks userTracks)
		{
			UserTrackCollection.Instance.Save(userTracks);
		}

		#endregion
	}
}