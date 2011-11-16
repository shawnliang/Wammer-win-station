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
				Wammer.Model.Drivers.RequestToAdd("http://localhost:9981/v2/station/drivers/add",
					textEmail.Text,
					textPassword.Text);

				MessageBox.Show("User has been successfully added to this station. blah blah...");
				this.Close();
			}
			catch (Wammer.Cloud.WammerCloudException ex)
			{
				switch ((int)ex.WammerError)
				{
					case (int)StationApiError.BadPath:
						MessageBox.Show("Folder path must be an abslute path.");
						break;
					case (int)StationApiError.DriverExist:
						MessageBox.Show("User has already been added to this station.");
						break;
					case (int)StationApiError.AuthFailed:
						MessageBox.Show("Invalid email or password");
						break;
					case (int)StationApiError.AlreadyHasStaion:
						HandleAlreadyHasStaion(ex.response);
						break;
					default:
						MessageBox.Show("Unknown error :" + ex.ToString());
						break;
				}

				logger.Error("Unable to add user", ex);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unknown error: " + ex.Message);
				logger.Error("Unable to add user", ex);
			}
		}

		private void HandleAlreadyHasStaion(string responseText)
		{
			AddUserResponse json = fastJSON.JSON.Instance.ToObject<AddUserResponse>(responseText);

			string text = string.Format(
				"You already have an registered station:\r\n" +
				"station id: " + json.station.station_id + "\r\n" +
				"location: " + json.station.location + "\r\n" +
				"last sync time: " + json.station.LastSeen + "\r\n" +
				"\r\n" +
				"You need to unregister the old station before adding " + textEmail.Text +
				"to this station.\r\n" +
				"Continue to unregister the old station?");

			DialogResult result = MessageBox.Show(text,
				"Unregister the old station?", MessageBoxButtons.OKCancel);

			if (result == DialogResult.Cancel)
				return;

			Wammer.Cloud.Station.SignOff(new WebClient(), json.station.station_id,
				json.session_token);

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
	}
}
