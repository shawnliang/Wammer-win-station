using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Builders;

namespace Wammer.Station.Doc
{
	class MonitorAddHandlerDB : IMonitorAddHandlerDB
	{
		public Model.Attachment FindAttachmentByFilePath(string path, string user_id)
		{
			var group_id = Model.DriverCollection.Instance.GetGroupIdByUser(user_id);
			if (string.IsNullOrEmpty(group_id))
				throw new WammerStationException("user does not exist: " + user_id, -1);

			return Model.AttachmentCollection.Instance.FindOne(
				Query.And(
					Query.EQ("file_path", path),
					Query.EQ("group_id", group_id)
				));
		}

		public void SaveAttachmentDB(Model.Attachment attDoc)
		{
			Model.AttachmentCollection.Instance.Save(attDoc);
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
