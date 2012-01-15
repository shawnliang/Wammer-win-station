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
		private SignInForm siform;

		public Messenger(Form form)
		{
			this.form = form;
			this.cs = new object();
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
				siform = new SignInForm();
				DialogResult res = siform.ShowDialog();
				siform = null;
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

		public bool ShowLoginDialog()
		{
			lock (cs)
			{
				siform = new SignInForm();
				DialogResult res = siform.ShowDialog();
				siform = null;
				if (res == DialogResult.Yes)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public void ActivateLoginDialog()
		{
			if (siform != null)
			{
				siform.Activate();
			}
		}
	}
}
