using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Wammer.Model;
using Wammer.Cloud;

namespace StationSystemTray
{
	public partial class LocalSettingDialog : Form
	{
		public LocalSettingDialog()
		{
			InitializeComponent();
		}

		private void LocalSettingDialog_Load(object sender, EventArgs e)
		{
			var users = DriverCollection.Instance.FindAll().Select(item => item.user).ToArray();
			cmbStations.DisplayMember = "nickname";
			cmbStations.DataSource = users;
		}
	}
}
