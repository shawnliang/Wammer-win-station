using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using Wammer.Model;
using Wammer.Cloud;
using MongoDB.Driver.Builders;

namespace Wammer.Station.Timeline
{
	class TimelineSyncerDB : ITimelineSyncerDB
	{
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
	}
}
