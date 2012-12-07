using System;
using System.Drawing;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	public partial class ServiceItemControl : UserControl
	{
		public event EventHandler<ServiceConnectivityChangeEventArgs> OnChange;

		public ServiceItemControl()
		{
			InitializeComponent();
		}

		public string ServiceName
		{
			get { return serviceName.Text; }
			set { serviceName.Text = value; }
		}

		public Image ServiceIcon
		{
			get { return serviceIcon.Image; }
			set { serviceIcon.Image = value; }
		}

		public bool ServiceEnabled
		{
			get { return connectCheckbox.Checked; }
			set { connectCheckbox.Checked = value; }
		}

		private void raiseOnChangeEvent(bool turnOn)
		{
			EventHandler<ServiceConnectivityChangeEventArgs> handler = OnChange;

			if (handler != null)
			{
				var arg = new ServiceConnectivityChangeEventArgs { TurnedOn = turnOn };
				handler(this, arg);

				connectCheckbox.Checked = (arg.Cancel) ? !turnOn : turnOn;
			}
		}

		private void connectCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (ServiceEnabled)
			{
				connectCheckbox.Text = "Connected";
				raiseOnChangeEvent(true); // turn on
			}
			else
			{
				connectCheckbox.Text = "Connect";
				raiseOnChangeEvent(false); // turn off
			}
		}
	}

	public class ServiceConnectivityChangeEventArgs : EventArgs
	{
		public bool Cancel { get; set; }
		public bool TurnedOn { get; set; }
	}

}
