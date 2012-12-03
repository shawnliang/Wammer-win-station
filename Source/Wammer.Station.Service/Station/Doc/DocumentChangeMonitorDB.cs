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
			throw new NotImplementedException();
		}

		public void UpdateMonitorItem(Model.MonitorItem item)
		{
			throw new NotImplementedException();
		}
	}
}
