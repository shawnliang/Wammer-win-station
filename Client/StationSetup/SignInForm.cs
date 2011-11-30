
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Wammer.Station.Management;
using Waveface.Localization;

namespace Wammer.Station
{
	public partial class SignInForm : Form
	{
		private const string SignUpURL = @"http://develop.waveface.com:4343/signup";
		private Localizer L;

		public SignInForm()
		{
			L = new Localizer();
			L.WItemsFullPath = Application.StartupPath + "\\StationML.xml";
			L.CurrentCulture = CultureManager.ApplicationUICulture;
			
			InitializeComponent();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			// 檢查是否都有填值
			if ((textBoxMail.Text == string.Empty) || (textBoxPassword.Text == string.Empty))
			{
				MessageBox.Show(L.T("FillAllFields"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			// 檢查email格式
			if (!TestEmailFormat(textBoxMail.Text))
			{
				MessageBox.Show(L.T("InvalidEmail"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			AddUser();
		}

		private void AddUser()
		{
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				StationController.AddUser(textBoxMail.Text, textBoxPassword.Text);

				MessageBox.Show(L.T("SignInSuccess"), "Waveface", MessageBoxButtons.OK);

				StationSetup.WavefaceWindowsClientHelper.StartWavefaceWindowsClient(textBoxMail.Text, textBoxPassword.Text);
				Close();
			}
			catch (AuthenticationException _e)
			{
				Cursor.Current = Cursors.Default;

				MessageBox.Show(_e.Message, "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				textBoxPassword.Text = string.Empty;

				return;
			}
			catch (UserAlreadyHasStationException _e)
			{
				Cursor.Current = Cursors.Default;

				RemoveStationForm _form = new RemoveStationForm(_e.Id, _e.Location);
				DialogResult _dr = _form.ShowDialog();

				if (_dr == DialogResult.Yes)
				{
					try
					{
						Cursor.Current = Cursors.WaitCursor;

						StationController.SignoffStation(_e.Id, textBoxMail.Text, textBoxPassword.Text);
						StationController.AddUser(textBoxMail.Text, textBoxPassword.Text);
						StationSetup.WavefaceWindowsClientHelper.StartWavefaceWindowsClient(textBoxMail.Text, textBoxPassword.Text);
						Close();
					}
					catch
					{
						Cursor.Current = Cursors.Default;

						ShowErrorDialogAndExit(L.T("SignOffStationError"));
					}
				}
				else
				{
					ShowErrorDialogAndExit(L.T("MustRemoveOld"));
				}
			}
			catch (StationAlreadyHasDriverException _e)
			{
				Cursor.Current = Cursors.Default;

				ShowErrorDialogAndExit(_e.Message);
			}
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
	}
}
