using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace Wammer.Station.Doc
{
	public class DocumentChangeMonitor
	{
		private IDocumentChangeMonitorDB db;
		private IDocumentChangeMonitorUtil util;

		public DocumentChangeMonitor(IDocumentChangeMonitorDB db, IDocumentChangeMonitorUtil util)
		{
			this.db = db;
			this.util = util;
		}

		public void ProcessChangedDocs()
		{
			var monitorTargets = db.FindAllMonitorItems();

			foreach (var target in monitorTargets)
			{
				try
				{
					var fileWriteTime = util.GetFileWriteTime(target.path);
					if (fileWriteTime > target.last_modify_time)
					{

						util.ProcessChangedDoc(target, fileWriteTime);
						target.last_modify_time = fileWriteTime;
						db.UpdateMonitorItem(target);

					}
				}
				catch (Exception e)
				{
					this.LogWarnMsg("Unable to process changed doc: " + target.path, e);
				}
			}
		}

		public void RemoveUnchangedMonitorItems(DateTime since)
		{
			Model.MonitorItemCollection.Instance.Remove(
				Query.And(
					Query.LT("last_modify_time", since),
					Query.GT("last_modify_time", DateTime.MinValue)
				));
		}
	}

	public interface IDocumentChangeMonitorDB
	{
		IEnumerable<MonitorItem> FindAllMonitorItems();
		void UpdateMonitorItem(MonitorItem item);
	}

	public interface IDocumentChangeMonitorUtil
	{
		DateTime GetFileWriteTime(string path);
		void ProcessChangedDoc(MonitorItem target, DateTime fileModifyTime);
	}
}
