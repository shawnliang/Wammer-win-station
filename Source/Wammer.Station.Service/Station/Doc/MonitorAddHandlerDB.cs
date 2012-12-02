using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.Doc
{
	class MonitorAddHandlerDB : IMonitorAddHandlerDB
	{
		public Model.Attachment FindAttachmentByFilePath(string path, string user_id)
		{
			throw new NotImplementedException();
		}

		public void SaveAttachmentDB(Model.Attachment attDoc)
		{
			throw new NotImplementedException();
		}

		public Model.MonitorItem FindMonitorItem(string id)
		{
			throw new NotImplementedException();
		}

		public void SaveMonitorItemDB(Model.MonitorItem item)
		{
			throw new NotImplementedException();
		}
	}
}
