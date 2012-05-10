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
using Wammer.Station.Management;

namespace StationSystemTray
{
	public partial class LocalSettingDialog : Form
	{
		private List<UserInfo> users = DriverCollection.Instance.FindAll().Select(item => item.user).ToList();

		public LocalSettingDialog()
		{
			InitializeComponent();
		}

		private void LocalSettingDialog_Load(object sender, EventArgs e)
		{
			cmbStations.DisplayMember = "nickname";
			cmbStations.DataSource = users;
		}

		private void btnUnlink_Click(object sender, EventArgs e)
		{
			var user = cmbStations.SelectedItem as UserInfo;

			if (user == null)
				return;

			StationController.RemoveOwner(user.user_id, false);
			users.Remove(user);
		}

		private void cmbStations_SelectedIndexChanged(object sender, EventArgs e)
		{
			var user = cmbStations.SelectedItem as UserInfo;

			if (user == null)
				return;

			lblUserEmail.Text = user.email;
		}
	}
}
