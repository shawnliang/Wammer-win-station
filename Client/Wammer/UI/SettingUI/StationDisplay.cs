using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Waveface.API.V2;

namespace Waveface.SettingUI
{
	public partial class StationDisplay : UserControl
	{
		public StationDisplay(Station station, Button btnUnlink)
		{
			InitializeComponent();
			lblComputerName.Text = station.computer_name;
			if (btnUnlink != null)
			{
				lblComputerName.Text = lblComputerName.Text + " " + I18n.L.T("ThisPC");
				tableLayoutPanel1.Controls.Remove(lblLastSeen);
				btnUnlink.Anchor = AnchorStyles.Right;
				tableLayoutPanel1.Controls.Add(btnUnlink);
			}
			else
			{
				DateTime JAN_1_1970 = new DateTime(1970, 1, 1);
				DateTime lastSeen = JAN_1_1970.AddSeconds(long.Parse(station.last_seen));
				lblLastSeen.Text = lblLastSeen.Text + " " + lastSeen.ToString();
			}
		}
	}
}
