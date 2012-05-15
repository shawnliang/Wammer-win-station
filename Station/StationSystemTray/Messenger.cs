using System.Windows.Forms;

namespace StationSystemTray
{
	public class Messenger
	{
		private const string TITLE = "Stream";
		private readonly Form _form;
		private readonly object cs;

		public Messenger(Form form)
		{
			_form = form;
			cs = new object();
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

		#region Nested type: ShowMessageDelegate

		private delegate DialogResult ShowMessageDelegate(Form form, string msg, string title);

		#endregion
	}
}