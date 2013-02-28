using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
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
