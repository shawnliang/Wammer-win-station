using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station;
using Wammer.Station.Management;

namespace StationSystemTray
{
	public partial class SettingDialog : Form
	{
		private string m_CurrentUserSession { get; set; }

		public event EventHandler<AccountEventArgs> AccountRemoving;
		public event EventHandler<AccountEventArgs> AccountRemoved;

		protected void OnAccountRemoving(AccountEventArgs e)
		{
			if (AccountRemoving == null)
				return;

			AccountRemoving(this, e);
		}

		protected void OnAccountRemoved(AccountEventArgs e)
		{
			if (AccountRemoved == null)
				return;

			AccountRemoved(this, e);
		}

		public SettingDialog(string currentUserSession)
		{
			InitializeComponent();

			m_CurrentUserSession = currentUserSession;

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
			var user = cmbStations.SelectedValue as UserInfo;

			if (user == null)
			{
				lblStorageUsageValue.Text = "0 MB";
				return;
			}

			var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", user.user_id));

			if (driver == null)
			{
				lblStorageUsageValue.Text = "0 MB";
				return;
			}

			var fs = new FileStorage(driver);
			lblStorageUsageValue.Text = (fs.GetUsedSize() / 1024 / 1024).ToString() + " MB";
		}


		private void LocalSettingDialog_Load(object sender, EventArgs e)
		{
			RefreshAccountList();
		}

		private void RefreshAccountList()
		{
			var loginedUser = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", m_CurrentUserSession));
			var users = (from item in DriverCollection.Instance.FindAll()
			             where item != null
			             select new
			                    	{
			                    		User = item.user,
			                    		EMail = item.user.email,
			                    		NickName = item.user.nickname,
			                    		DisplayName =
			             	(loginedUser != null && item.user.email == loginedUser.user.email)
			             		? item.user.nickname + " " + Properties.Resources.CURRENT_ACCOUT
			             		: item.user.nickname
			                    	}).ToList();

			cmbStations.DisplayMember = "DisplayName";
			cmbStations.ValueMember = "User";
			cmbStations.DataSource = users;
			AdjustRemoveButton();
		}

		private void btnUnlink_Click(object sender, EventArgs e)
		{
			var user = cmbStations.SelectedValue as UserInfo;

			if (user == null)
				return;

			using (var dialog = new CleanResourceForm(user.email))
			{
				dialog.TopMost = this.TopMost;
				dialog.BackColor = this.BackColor;
				dialog.ShowInTaskbar = false;
				if (dialog.ShowDialog() == DialogResult.Yes)
					RemoveCurrentAccount();
			}
		}

		private void RemoveCurrentAccount()
		{
			var user = cmbStations.SelectedValue as UserInfo;

			if (user == null)
				return;

			OnAccountRemoving(new AccountEventArgs(user.email));

			StationController.RemoveOwner(user.user_id, false);
			RefreshAccountList();

			OnAccountRemoved(new AccountEventArgs(user.email));
		}

		private void cmbStations_SelectedIndexChanged(object sender, EventArgs e)
		{
			var user = cmbStations.SelectedValue as UserInfo;

			if (user == null)
			{
				lblUserEmail.Text = string.Empty;
				return;
			}

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
			SetStorageUsage();
		}
	}
}
