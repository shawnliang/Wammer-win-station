using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Builders;
using MongoDB.Driver;

namespace Wammer.Station.TimelineChange
{
	public class UserInfoUpdator: IUserInfoUpdator
	{
		public void UpdateChangeLogSyncTime(string user_id, string time)
		{
			Model.DriverCollection.Instance.Update(
				Query.EQ("_id", user_id),
				Update.Set("change_log_sync_time", time));
		}
	}
}
