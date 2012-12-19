using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public partial class UsbImportDialog : Form
	{
		private string driveToImport;
		private string user_id;
		private string session_token;
		private PortableMediaService portableMediaService = new PortableMediaService();

		public UsbImportDialog(string driveToImport, string user_id, string session_token)
		{
			InitializeComponent();
			this.driveToImport = driveToImport;
			this.user_id = user_id;
			this.session_token = session_token;
			this.importControl.Service = portableMediaService;
			this.Icon = Resources.Icon;
		}

		private void UsbImportDialog_Load(object sender, EventArgs e)
		{
			var service = new PortableMediaService();
			importControl.Service = service;
		}

		private void UsbImportDialog_Shown(object sender, EventArgs e)
		{
			var parameters = new WizardParameters();
			parameters.Set("user_id", user_id);
			parameters.Set("session_token", session_token);
			importControl.OnEnteringStep(parameters);

			if (!string.IsNullOrEmpty(driveToImport))
			{
				if (portableMediaService.GetAlwaysAutoImport(driveToImport))
					importControl.ImportDevice(driveToImport);
				else
					importControl.SelectDevice(driveToImport);
			}
		}
	}
}
