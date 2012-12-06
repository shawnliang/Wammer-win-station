using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.Doc
{
	class CheckDocumentChangeTimer : NonReentrantTimer
	{
		private DocumentChangeMonitor monitor =
			new DocumentChangeMonitor(new DocumentChangeMonitorDB(), new DocumentChangeMonitorUtil());


		public CheckDocumentChangeTimer()
			:base(60*1000)
		{
		}

		protected override void ExecuteOnTimedUp(object state)
		{
			try
			{
				monitor.ProcessChangedDocs();
				monitor.RemoveUnchangedMonitorItems(DateTime.Now.AddDays(-7.0));
			}
			catch (Exception e)
			{
				this.LogWarnMsg(e.ToString());
			}
		}
	}
}
