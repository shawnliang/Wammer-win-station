using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;

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
			var attDoc = db.FindLatestVersion(file, user_id);
			if (attDoc == null)
			{
				db.SaveMonitorItemDB(new MonitorItem(file, user_id));
			}
			else
			{
				var item = new MonitorItem(file, user_id) { last_modify_time = attDoc.file_modify_time };
				if (db.FindMonitorItem(item.id) != null)
				{
					util.UpdateDocOpenTimeAsync(attDoc.object_id, DateTime.Now);
				}
				else
				{
					db.SaveMonitorItemDB(item);
				}
			}
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
		void UpdateDocOpenTimeAsync(string object_id, DateTime openTime);
	}
}
