using System;
using System.Reflection;
using System.Windows.Forms;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient.Src.Control
{
	public partial class AboutControl : UserControl
	{
		public AboutControl()
		{
			InitializeComponent();
		}

		private void CheckUpdateButton_Click(object sender, EventArgs e)
		{
			var update = new AutoUpdate(false);
			if (update.IsUpdateRequired())
				update.ShowUpdateNeededUI();
			else
				MessageBox.Show(Resources.ALREAD_UPDATED);
		}

		private void AboutControl_Load(object sender, EventArgs e)
		{
			lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}
	}
}
