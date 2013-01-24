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

		public string Description
		{
			get { return description.Text; }
			set { description.Text = value; }
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

				connectCheckbox.Text = connectCheckbox.Checked ? "Connected" : "Connect";
			}
		}

		private void connectCheckbox_Click(object sender, EventArgs e)
		{
			if (ServiceEnabled)
			{
				connectCheckbox.Text = "Connecting";
				raiseOnChangeEvent(true);
			}
			else
			{
				connectCheckbox.Text = "Disconnecting";
				raiseOnChangeEvent(false);
			}
		}

		private void connectCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			connectCheckbox.Text = connectCheckbox.Checked ? "Connected" : "Connect";
		}

	}

	public class ServiceConnectivityChangeEventArgs : EventArgs
	{
		public bool Cancel { get; set; }
		public bool TurnedOn { get; set; }
	}

}
