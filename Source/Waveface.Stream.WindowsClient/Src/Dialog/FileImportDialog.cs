using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;

namespace Waveface.Stream.WindowsClient
{
	public partial class FileImportDialog : StepByStepWizardDialog
	{
		private static FileImportDialog _instance;

		public static FileImportDialog Instance
		{
			get
			{
				return (_instance == null || _instance.IsDisposed) ? (_instance = new FileImportDialog()) : _instance;
			}
		}

		private FileImportDialog()
		{
			InitializeComponent();

			if (!StreamClient.Instance.IsLogined)
				throw new InvalidOperationException();

			wizardControl.Parameters.Set("user_id", StreamClient.Instance.LoginedUser.UserID);
			wizardControl.Parameters.Set("session_token", StreamClient.Instance.LoginedUser.SessionToken);

			wizardControl.SetWizardPages(new StepPageControl[]
			{
				new FileImportControl()
			});
		}
	}
}
