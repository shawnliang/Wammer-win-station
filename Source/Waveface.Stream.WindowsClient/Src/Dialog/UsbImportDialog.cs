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
		#region Var
		private string _driveToImport;
		private PortableMediaService _portableMediaService = new PortableMediaService(); 
		#endregion

		public UsbImportDialog(string driveToImport)
		{
			InitializeComponent();
			this._driveToImport = driveToImport;
			this.importControl.Service = _portableMediaService;
			this.Icon = Resources.Icon;
		}

		private void UsbImportDialog_Load(object sender, EventArgs e)
		{
			var service = new PortableMediaService();
			importControl.Service = service;
		}

		private void UsbImportDialog_Shown(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(_driveToImport))
			{
				if (_portableMediaService.GetAlwaysAutoImport(_driveToImport))
					importControl.ImportDevice(_driveToImport);
				else
					importControl.SelectDevice(_driveToImport);
			}
		}
	}
}
