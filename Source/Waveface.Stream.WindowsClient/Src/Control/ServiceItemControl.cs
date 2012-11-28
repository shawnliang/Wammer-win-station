using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
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

				if (arg.Cancel)
				{
					connectCheckbox.Checked = turnOn;
				}
			}
		}

		private void connectCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (ServiceEnabled)
			{
				raiseOnChangeEvent(true); // turn on
				connectCheckbox.Text = "Connected";
			}
			else
			{
				raiseOnChangeEvent(false); // turn off
				connectCheckbox.Text = "Connect";
			}
		}
	}

	public class ServiceConnectivityChangeEventArgs : EventArgs
	{
		public bool Cancel { get; set; }
		public bool TurnedOn { get; set; }
	}

}
