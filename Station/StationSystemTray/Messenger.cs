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
		private const string TITLE = "Waveface";
		private Form form;

		public Messenger(Form form)
		{
			this.form = form;
		}

		public DialogResult ShowMessage(string msg)
		{
			lock (cs)
			{
				return MessageBox.Show(form, msg, TITLE);
			}
		}

		public void ShowLoginDialog()
		{
			lock (cs)
			{
				// do login
			}
		}
	}
}
