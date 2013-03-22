using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient.Src.Control
{
	public partial class DeviceNameControl : UserControl
	{
		public DeviceNameControl()
		{
			InitializeComponent();
		}

		public string DeviceName
		{
			get
			{
				return deviceNameLabel.Text;
			}
			set
			{
				deviceNameLabel.Text = value;
			}
		}
	}
}
