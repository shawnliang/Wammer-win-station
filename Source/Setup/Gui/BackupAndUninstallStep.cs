using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpSetup.Base;
using SharpSetup.UI.Controls;
using SharpSetup.UI.Forms.Modern;
using Gui.Properties;
using System.IO;

namespace Gui
{
	public partial class BackupAndUninstallStep : SharpSetup.UI.Forms.Modern.ModernActionStep
	{
		BackgroundWorker backupThread = new BackgroundWorker();

		public BackupAndUninstallStep()
		{
			InitializeComponent();

			backupThread.DoWork += new DoWorkEventHandler(backupThread_DoWork);
			backupThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backupThread_RunWorkerCompleted);
		}

		void backupThread_DoWork(object sender, DoWorkEventArgs e)
		{
			Migration.DoBackup();
		}

		void backupThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (this.InvokeRequired)
				this.Invoke(new RunWorkerCompletedEventHandler(this.backupThread_RunWorkerCompleted), sender, e);

			try
			{
				if (e.Error != null)
				{
					MessageBox.Show(e.Error.ToString());
					throw e.Error;
				}

				MsiConnection.Instance.Uninstall();

				if (File.Exists(Resources.MainMsiFile))
			        MsiConnection.Instance.Open(Resources.MainMsiFile, true);
			}
			catch (MsiException mex)
			{
				if (mex.ErrorCode != (uint)InstallError.UserExit)
					MessageBox.Show("Upgrade failed: " + mex.Message);
				Wizard.Finish();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Upgrade failed: " + ex.Message);
				Wizard.Finish();
			}

			ipProgress.StopListening();
			Wizard.NextStep();
		}

		private void BackupAndUninstallStep_Entered(object sender, EventArgs e)
		{
			Wizard.BackButton.Enabled = false;
			Wizard.NextButton.Enabled = false;
			Wizard.CancelButton.Enabled = false;

			ipProgress.StartListening();

			backupThread.RunWorkerAsync();
		}

		public override bool CanClose()
		{
			return false;
		}
	}
}
