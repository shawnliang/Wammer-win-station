using System.Collections.Generic;

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
