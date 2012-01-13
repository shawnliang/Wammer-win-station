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

		public bool ShowLoginDialog(SimpleUIController uictrl, object parameter)
		{
			lock (cs)
			{
				SignInForm siform = new SignInForm();
				DialogResult res = siform.ShowDialog();
				if (res == DialogResult.Yes)
				{
					uictrl.PerformAction(parameter);
					return true;
				}
				else
				{
					return false;
				}
			}
		}
	}
}
