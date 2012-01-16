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

		private delegate DialogResult ShowMessageDelegate(Form form, string msg, string title);

		public Messenger(Form form)
		{
			this.form = form;
			this.cs = new object();
		}

		public void ShowMessage(string msg)
		{
			lock (cs)
			{
				if (form.InvokeRequired)
				{
					form.Invoke(new ShowMessageDelegate(MessageBox.Show), new object[] {form, msg, TITLE});
				}
				else
					MessageBox.Show(form, msg, TITLE);
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

		public void ShowLoginDialog()
		{
			lock (cs)
			{
				siform = new SignInForm();
				siform.FormClosed += new FormClosedEventHandler(siform_FormClosed);
				siform.Show(form);
			}
		}

		void siform_FormClosed(object sender, FormClosedEventArgs e)
		{
			// user has to re-open the parent form after re-login,
			// but closing parent form will fire close event to this form,
			// so we add a check here to avoid infinite loop.
			if (e.CloseReason != CloseReason.FormOwnerClosing)
				form.Close();
		}
	}
}
