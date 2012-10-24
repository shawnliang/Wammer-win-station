using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StationSystemTray.Src.Control
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
			get { return onOffSwitch.Value == 1; }
			set { onOffSwitch.Value = value ? 1 : 0; }
		}

		private void onOffSwitch_Scroll(object sender, EventArgs e)
		{
			if (ServiceEnabled)
			{
				raiseOnChangeEvent(true); // turn on
			}
			else
			{
				raiseOnChangeEvent(false); // turn off
			}
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
					onOffSwitch.Value = turnOn ? 0 : 1;
				}
			}
		}
	}

	public class ServiceConnectivityChangeEventArgs : EventArgs
	{
		public bool Cancel { get; set; }
		public bool TurnedOn { get; set; }
	}

}
