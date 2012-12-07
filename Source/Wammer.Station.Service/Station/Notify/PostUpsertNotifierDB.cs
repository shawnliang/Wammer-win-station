
namespace Wammer.Station.Notify
{
	class PostUpsertNotifierDB : IPostUpsertNotifierDB
	{
		public string GetUserIdByGroupId(string group_id)
		{
			var driver = Wammer.Model.DriverCollection.Instance.FindDriverByGroupId(group_id);
			if (driver != null)
				return driver.user_id;
			else
				throw new WammerStationException("No such user group: " + group_id, -1);
		}
	}
}
