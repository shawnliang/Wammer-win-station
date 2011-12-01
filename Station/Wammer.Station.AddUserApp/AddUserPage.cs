using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.ServiceProcess;

using Microsoft.Win32;
using Wammer.Station;
using Wammer.Station.Management;

namespace Wammer.Station.StartUp
{
	public partial class AddUserPage : Form
	{
		private WebClient agent = new WebClient();
		private log4net.ILog logger;

		public AddUserPage()
		{
			InitializeComponent();

			log4net.Config.XmlConfigurator.Configure();
			logger = log4net.LogManager.GetLogger("StartUpPage");
			this.linkLabel1.Links.Clear();
			this.linkLabel1.Links.Add(28, 12, "http://www.waveface.com/");
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (textEmail.Text.Length == 0 || textPassword.Text.Length == 0)
			{
				MessageBox.Show("All fields must be filled.");
				return;
			}

			try
			{
				StationController.AddUser(textEmail.Text, textPassword.Text);

				MessageBox.Show("User has been successfully added to this station. blah blah...");
				this.Close();
			}
			catch (AuthenticationException ex)
			{
				MessageBox.Show("Invalid email or password");
				logger.Error("Unable to add user", ex);
			}
			catch (StationAlreadyHasDriverException ex)
			{
				MessageBox.Show("Unable to add user because the station already has an driver");
				logger.Error("Unable to add user", ex);
			}
			catch (UserAlreadyHasStationException ex)
			{
				logger.Warn("User already has a station");
				HandleAlreadyHasStaion(ex);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unknown error: " + ex.Message);
				logger.Error("Unable to add user", ex);
			}
		}

		private void HandleAlreadyHasStaion(UserAlreadyHasStationException ex)
		{

			string text = string.Format(
				"You already have an registered station:\r\n" +
				"station id: " + ex.Id + "\r\n" +
				"location: " + ex.Location + "\r\n" +
				"last sync time: " + ex.LastSyncTime + "\r\n" +
				"\r\n" +
				"You need to unregister the old station before adding " + textEmail.Text +
				"to this station.\r\n" +
				"Continue to unregister the old station?");

			DialogResult result = MessageBox.Show(text,
				"Unregister the old station?", MessageBoxButtons.OKCancel);

			if (result == DialogResult.Cancel)
				return;

			StationController.SignoffStation(ex.Id, textEmail.Text, textPassword.Text);
			MessageBox.Show("Old station is unregistered successfully. Please add user again.");
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			e.Link.Visited = true;
			System.Diagnostics.Process.Start(e.Link.LinkData as string);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			long quota = Convert.ToInt64(quotaText.Text);
			try
			{
				StationController.ConnectDropbox(quota);
				MessageBox.Show("Connect Dropbox successfullly.");
			}
			catch (DropboxNotInstalledException ex)
			{
				MessageBox.Show(ex.Message);
			}
			catch (DropboxNoSyncFolderException ex)
			{
				MessageBox.Show(ex.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			List<StorageStatus> storages = StationController.ListCloudStorage();
			cloudStorageListTextBox.Text = fastJSON.JSON.Instance.ToJSON(storages, false, false, false, false);
		}

		private void button5_Click(object sender, EventArgs e)
		{
			try
			{
				string url = StationController.GetDropboxOAuthUrl();
				dropboxOAuthUrlTextBox.Text = url;
			}
			catch (GetDropboxOAuthException ex)
			{
				MessageBox.Show(ex.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
