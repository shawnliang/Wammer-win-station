
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Wammer.Station.Management;

namespace Wammer.Station
{
	public partial class SignInForm : Form
	{
		public SignInForm()
		{
			InitializeComponent();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			// 檢查是否都有填值
			if ((textBoxMail.Text == string.Empty) || (textBoxPassword.Text == string.Empty))
			{
				MessageBox.Show("Please fill all the fields!", "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			// 檢查email格式
			if (!TestEmailFormat(textBoxMail.Text))
			{
				MessageBox.Show("Invalid email format", "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
				MessageBox.Show("Sign in success!", "Waveface", MessageBoxButtons.OK);

				StationSetup.WammerZHelper.SetRegistered();
				StationSetup.WammerZHelper.StartWammerZ();
				Close();
			}
			catch (AuthenticationException _e)
			{
				Cursor.Current = Cursors.Default;

				MessageBox.Show(_e.Message, "Waveface", MessageBoxButtons.OK,
								MessageBoxIcon.Warning);

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

						//重新呼叫 StationController.AddUser
						timerDelay.Enabled = true;
					}
					catch
					{
						Cursor.Current = Cursors.Default;

						ShowErrorDialogAndExit("Sign off Station Error!");
					}
				}
				else
				{
					ShowErrorDialogAndExit("您必須移除舊的Station後, 才能安裝新的");
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
			MessageBox.Show(message, "Waveface", MessageBoxButtons.OK,
								MessageBoxIcon.Error);

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

		private void timerDelay_Tick(object sender, EventArgs e)
		{
			timerDelay.Enabled = false;

			AddUser();
		}
	}
}
