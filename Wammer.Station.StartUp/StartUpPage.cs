using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace Wammer.Station.StartUp
{
    public partial class StartUpPage : Form
    {
        private bool registerSuccess = false;
        private WebClient agent = new WebClient();

        public StartUpPage()
        {
            InitializeComponent();
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
                Wammer.Cloud.Station station = Wammer.Cloud.Station.SignUp(
                    this.agent,
                    "stationId", // TODO: create a new station id
                    user.Token); // TODO: api key

                station.LogOn(this.agent); // TODO: machine info, ip, hostname, ...
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


            MessageBox.Show(
                "Your windows station is connected with Wammer successfully.\r\n" +
                "Enjoy using Wammer.");

            registerSuccess = true;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void StartUpPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (registerSuccess)
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
