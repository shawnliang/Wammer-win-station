using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Microsoft.Win32;
using Wammer.Station;

namespace Wammer.Station.StartUp
{
	public partial class StartUpPage : Form
	{
		private WebClient agent = new WebClient();

		public StartUpPage()
		{
			InitializeComponent();

			log4net.Config.XmlConfigurator.Configure();
		}

		public static bool Inited()
		{
			object stationId = StationRegistry.GetValue("stationId", null);
			return stationId != null && !stationId.Equals("");
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			log4net.ILog logger = log4net.LogManager.GetLogger("StartUpPage");
			Wammer.Cloud.User user = null;
			try
			{
				user = Wammer.Cloud.User.LogIn(
					this.agent,
					this.accountTextField.Text.Trim(),
					this.passwordTextField.Text.Trim());
			}
			catch (Exception ex)
			{
				logger.Error("Cannot login user", ex);
				MessageBox.Show("Incorrect account or password.");
				return;
			}
			logger.Info("user login successfully");
			logger.Info("token = " + user.Token);

			try
			{
				Guid stationId = Guid.NewGuid();
				Wammer.Cloud.Station station = Wammer.Cloud.Station.SignUp(
					this.agent,
					stationId.ToString(),
					user.Token);

				StationRegistry.SetValue("stationId", stationId.ToString());
				StationRegistry.SetValue("stationToken", station.Token);
			}
			catch (Exception ex)
			{
				logger.Error("Cannot signup station", ex);
				MessageBox.Show("Unexpected error when registering with Wammer Cloud");
				return;
			}


			MessageBox.Show(
				"Your windows station is connected with Wammer successfully.\r\n" +
				"Enjoy using Wammer.");


			logger.Info("Wammer starts up successfully");
			this.Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void StartUpPage_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (Inited())
				return;

			DialogResult res =
				MessageBox.Show(
					"You Wammer Windows Station is not yet connected with Wammer.\r\n" +
					"Do you want to quit?",
					"Quit Wammer Windows Station Setup?",
					MessageBoxButtons.YesNo);

			if (res == System.Windows.Forms.DialogResult.No)
				e.Cancel = true;
		}
	}
}
