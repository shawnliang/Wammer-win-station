using System;
using Wammer.Model;
using Waveface.Stream.Model;

namespace Wammer.Station.Doc
{
	public class MonitorAddHandlerImp
	{
		private IMonitorAddHandlerDB db;
		private IMonitorAddHandlerUtility util;

		public MonitorAddHandlerImp(IMonitorAddHandlerDB db, IMonitorAddHandlerUtility util)
		{
			if (db == null)
				throw new ArgumentNullException("db");
			if (util == null)
				throw new ArgumentNullException("util");

			this.db = db;
			this.util = util;
		}

		public void Process(string apikey, string session_token, string user_id, string file)
		{
			if (IsFileInStreamFolder(file))
				return;

			var attDoc = db.FindLatestVersion(file, user_id);
			if (attDoc == null)
			{
				db.SaveMonitorItemDB(new MonitorItem(file, user_id));
			}
			else
			{
				util.UpdateDocOpenTimeAsync(user_id, attDoc.object_id, DateTime.Now);

				var item = new MonitorItem(file, user_id) { last_modify_time = attDoc.doc_meta.modify_time };
				if (db.FindMonitorItem(item.id) == null)
					db.SaveMonitorItemDB(item);
			}
		}

		private static bool IsFileInStreamFolder(string file)
		{
			return file.ToLower().Contains(FileStorage.ResourceFolder.ToLower());
		}
	}

	public interface IMonitorAddHandlerDB
	{
		Attachment FindLatestVersion(string path, string user_id);
		MonitorItem FindMonitorItem(string id);
		void SaveMonitorItemDB(Model.MonitorItem item);
	}

	public interface IMonitorAddHandlerUtility
	{
		void UpdateDocOpenTimeAsync(string user_id, string object_id, DateTime openTime);
	}
}
