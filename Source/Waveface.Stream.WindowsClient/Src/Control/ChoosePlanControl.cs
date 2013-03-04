using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{

	public partial class ChoosePlanControl : UserControl
	{
		public ChoosePlanControl()
		{
			InitializeComponent();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			GoToWeb.OpenInBrowser("/");
		}

		private void planBox4_Click(object sender, System.EventArgs e)
		{

		}
	}
}
