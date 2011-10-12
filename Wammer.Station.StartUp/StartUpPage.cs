using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Microsoft.Win32;

namespace Wammer.Station.StartUp
{
    public partial class StartUpPage : Form
    {
        private WebClient agent = new WebClient();

        public StartUpPage()
        {
            InitializeComponent();
        }

        public static bool Inited()
        {
            object stationId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation", "stationId", null);
            return stationId != null && !stationId.Equals("");
        }

        private void button2_Click(object sender, EventArgs e)
        {
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
                MessageBox.Show("Incorrect account or password.");
                return;
            }

            try
            {
                Guid stationId = Guid.NewGuid();
                Wammer.Cloud.Station station = Wammer.Cloud.Station.SignUp(
                    this.agent,
                    stationId.ToString(),
                    user.Token);

                Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation", "stationId", stationId.ToString());
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation", "stationToken", station.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


            MessageBox.Show(
                "Your windows station is connected with Wammer successfully.\r\n" +
                "Enjoy using Wammer.");

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
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
