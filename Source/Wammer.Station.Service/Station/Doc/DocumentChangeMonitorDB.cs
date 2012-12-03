using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.Doc
{
	class DocumentChangeMonitorDB : IDocumentChangeMonitorDB
	{
		public IEnumerable<Model.MonitorItem> FindAllMonitorItems()
		{
			return Model.MonitorItemCollection.Instance.FindAll();
		}

		public void UpdateMonitorItem(Model.MonitorItem item)
		{
			Model.MonitorItemCollection.Instance.Save(item);
		}
	}
}
