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
		public BackupAndUninstallStep()
		{
			InitializeComponent();
		}

		private void BackupAndUninstallStep_Entered(object sender, EventArgs e)
		{
			Wizard.BackButton.Enabled = false;
			Wizard.NextButton.Enabled = false;

			ipProgress.StartListening();
			try
			{
				Migration.DoBackup();
				MsiConnection.Instance.Uninstall();
				/*
				try
				{
					MsiConnection.Instance.Open(new Guid("{da97b6b8-4989-4dd0-964f-ac25a7d36a36}"));
					MsiConnection.Instance.Uninstall();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Uninstall");
				}
				*/
				if (File.Exists(Resources.MainMsiFile))
					MsiConnection.Instance.Open(Resources.MainMsiFile, true);
				
			}
			catch (MsiException mex)
			{
				if (mex.ErrorCode != (uint)InstallError.UserExit)
					MessageBox.Show("Installation failed: " + mex.Message);
				Wizard.Finish();
			}

			ipProgress.StopListening();
			Wizard.NextStep();
		}

		public override bool CanClose()
		{
			return false;
		}
	}
}
