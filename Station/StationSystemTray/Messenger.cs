using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StationSystemTray
{
	public class Messenger
	{
		private object cs;
		private const string TITLE = "Stream";
		private Form _form;

		private delegate DialogResult ShowMessageDelegate(Form form, string msg, string title);

		public Messenger(Form form)
		{
			this._form = form;
			this.cs = new object();
		}

		public void ShowMessage(string msg)
		{
			lock (cs)
			{
				if (_form.InvokeRequired)
				{
					if (!_form.IsDisposed)
						_form.Invoke(new ShowMessageDelegate(MessageBox.Show), new object[] {_form, msg, TITLE});
				}
				else
					MessageBox.Show(_form, msg, TITLE);
			}
		}
	}
}
