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
using Wammer.Station;
using MongoDB.Driver.Builders;

namespace StationSystemTray
{
	public partial class SettingDialog : Form
	{
		private readonly List<UserInfo> users = DriverCollection.Instance.FindAll().Select(item => item.user).ToList();

		public SettingDialog()
		{
			InitializeComponent();
			AdjustUserEMail();
		}

		private void AdjustRemoveButton()
		{
			btnUnlink.Enabled = !string.IsNullOrEmpty(cmbStations.Text);
		}

		private void AdjustUserEMail()
		{
			lblUserEmail.Visible = !string.IsNullOrEmpty(cmbStations.Text);
		}

		private void SetStorageUsage()
		{
			var user = cmbStations.SelectedItem as UserInfo;

			if (user == null)
				return;

			var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", user.user_id));

			if (driver == null)
				return;

			var fs = new FileStorage(driver);
			lblStorageUsageValue.Text = (fs.GetUsedSize() / 1024 / 1024).ToString() + "MB";
		}


		private void LocalSettingDialog_Load(object sender, EventArgs e)
		{
			cmbStations.DisplayMember = "nickname";
			cmbStations.DataSource = users;
			AdjustRemoveButton();
			SetStorageUsage();
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

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void cmbStations_TextChanged(object sender, EventArgs e)
		{
			AdjustRemoveButton();
			AdjustUserEMail();
		}
	}
}
