using MongoDB.Driver.Builders;
using System.Linq;
using Waveface.Stream.Model;

namespace Wammer.Station.Doc
{
	class MonitorAddHandlerDB : IMonitorAddHandlerDB
	{
		public Attachment FindLatestVersion(string path, string user_id)
		{
			var group_id = DriverCollection.Instance.GetGroupIdByUser(user_id);
			if (string.IsNullOrEmpty(group_id))
				throw new WammerStationException("user does not exist: " + user_id, -1);

			return AttachmentCollection.Instance.Find(
				Query.And(
					Query.EQ("file_path", path),
					Query.EQ("group_id", group_id),
					Query.EQ("device_id", StationRegistry.StationId)
				)).SetSortOrder(SortBy.Descending("file_modify_time")).SetLimit(1).FirstOrDefault();
		}

		public Model.MonitorItem FindMonitorItem(string id)
		{
			return Model.MonitorItemCollection.Instance.FindOneById(id);
		}

		public void SaveMonitorItemDB(Model.MonitorItem item)
		{
			Model.MonitorItemCollection.Instance.Save(item);
		}
	}
}
