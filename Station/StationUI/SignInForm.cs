
﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Web;
using StationSetup;
using Wammer.Station.Management;
using Waveface.Localization;

namespace Wammer.Station
{
    public partial class SignInForm : Form
    {
        private const string SignUpURL = @"https://waveface.com/signup?device=station";

        public SignInForm()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if ((textBoxMail.Text == string.Empty) || (textBoxPassword.Text == string.Empty))
            {
                MessageBox.Show(I18n.L.T("FillAllFields"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!TestEmailFormat(textBoxMail.Text))
            {
                MessageBox.Show(I18n.L.T("InvalidEmail"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AddUser(textBoxMail.Text, textBoxPassword.Text);
        }

        private void AddUser(string email, string password)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                AddUserResult result = StationController.AddUser(email, password);

                DropboxInstallAndLink(email, password, result);

                Close();
            }
            catch (AuthenticationException)
            {
                Cursor.Current = Cursors.Default;

                MessageBox.Show(I18n.L.T("AuthError"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                textBoxPassword.Text = string.Empty;

                return;
            }
            catch (UserAlreadyHasStationException _e)
            {
                Cursor.Current = Cursors.Default;

                RemoveStationForm _form = new RemoveStationForm(_e.ComputerName);
                DialogResult _dr = _form.ShowDialog();

                if (_dr == DialogResult.Yes)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        StationController.SignoffStation(_e.Id, textBoxMail.Text, textBoxPassword.Text);
                        AddUserResult result = StationController.AddUser(textBoxMail.Text, textBoxPassword.Text);
                        result.has_old_station = true;
                        DropboxInstallAndLink(textBoxMail.Text, textBoxPassword.Text, result);

                        Close();
                    }
                    catch (Exception e)
                    {
                        Cursor.Current = Cursors.Default;

                        ShowErrorDialogAndExit(I18n.L.T("SignOffStationError") + " : " + e.ToString());
                    }
                }
                else
                {
                    ShowErrorDialogAndExit(I18n.L.T("MustRemoveOld"));
                }
            }
            catch (StationAlreadyHasDriverException)
            {
                Cursor.Current = Cursors.Default;

                ShowErrorDialogAndExit(I18n.L.T("StationHasDriverError"));
            }
            catch (StationServiceDownException)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(I18n.L.T("StationDown"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(I18n.L.T("UnknownSigninError"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DropboxInstallAndLink(string email, string password, AddUserResult userInfo)
        {
            Hide();

            DropboxForm _dropboxForm = new DropboxForm(email, password, userInfo);
            _dropboxForm.ShowDialog();
        }

        private void ShowErrorDialogAndExit(string message)
        {
            MessageBox.Show(message, "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Close();
        }

        public bool TestEmailFormat(string emailAddress)
        {
            const string _patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                                         + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                                         + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                         + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                                         + @"[a-zA-Z]{2,}))$";

            Regex _reStrict = new Regex(_patternStrict);
            return _reStrict.IsMatch(emailAddress);
        }

        private void linkLabelNew_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(SignUpURL, null);
        }

        private void SignInForm_DoubleClick(object sender, EventArgs e)
        {
            if (CultureManager.ApplicationUICulture.Name == "en-US")
            {
                CultureManager.ApplicationUICulture = new CultureInfo("zh-TW");
                return;
            }

            if (CultureManager.ApplicationUICulture.Name == "zh-TW")
            {
                CultureManager.ApplicationUICulture = new CultureInfo("en-US");
                return;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter && !this.buttonOK.Focused)
            {
                buttonOK_Click(null, null);
            }
            return base.ProcessDialogKey(keyData);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void SignInForm_Load(object sender, EventArgs e)
        {
            this.textBoxMail.Focus();
        }
    }
}