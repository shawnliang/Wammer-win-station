using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StationSystemTray
{
	public partial class ImportFromPotableMediaControl : StepPageControl
	{
		private IPortableMediaService service;
		private volatile bool canceled;
		private string user_id;
		private string session_token;
		private int import_count;

		public ImportFromPotableMediaControl(IPortableMediaService service)
		{
			InitializeComponent();
			this.service = service;
			this.service.ImportDone += new EventHandler<ImportDoneEventArgs>(service_ImportDone);
			this.service.FileImported += new EventHandler<FileImportEventArgs>(service_FileImported);
			progressBar.Value = 0;
		}

		void service_FileImported(object sender, FileImportEventArgs e)
		{
			import_count++;

			progressBar.Invoke(new MethodInvoker(() => {
				progressText.Text = string.Format("{0}/{1} {2} processed", import_count, progressBar.Maximum, e.FilePath);
				progressBar.PerformStep();
			}));
		}

		void service_ImportDone(object sender, ImportDoneEventArgs e)
		{
			string doneMsg;

			if (e.Error != null)
				doneMsg = string.Format("{0} files imported. An error occurred: {1}", import_count, e.Error);
			else
				doneMsg = string.Format("{0} files are imported successfully", import_count);

			progressBar.Invoke(new MethodInvoker(() =>
			{
				progressText.Text = doneMsg;
				progressBar.Value = progressBar.Maximum;
			}));

			importButton.Enabled = true;
		}

		private void ImportFromPotableMediaControl_Load(object sender, EventArgs e)
		{
			refreshDrives();
		}

		private void refreshDrives()
		{
			deviceCombobox.Items.Clear();
			deviceCombobox.Items.AddRange(service.GetPortableDevices().ToArray());
		}

		private void importButton_Click(object sender, EventArgs e)
		{
			var device = deviceCombobox.SelectedItem as PortableDevice;

			if (device == null)
				return;

			Cursor.Current = Cursors.WaitCursor;

			progressBar.Visible = progressText.Visible = true;
			progressBar.Value = 0;
			progressText.Text = "Scanning photos...";
			canceled = false;
			import_count = 0;
			importButton.Enabled = false;

			backgroundWorker.RunWorkerAsync(device);
		}

		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var device = e.Argument as PortableDevice;
			var files = service.GetFileList(device.DrivePath);

			progressBar.Invoke(new MethodInvoker(() => 
			{
				progressBar.Maximum = files.Count();
			}));


			service.ImportAsync(files, user_id, session_token, StationAPI.API_KEY);

		}

		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MessageBox.Show(e.Error.Message);
				importButton.Enabled = true;
			}
		}

		private void deviceCombobox_DropDown(object sender, EventArgs e)
		{
			refreshDrives();
		}

		public override void OnLeavingStep(WizardParameters parameters)
		{
			if (backgroundWorker.IsBusy)
			{
				canceled = true;

				while (backgroundWorker.IsBusy)
					Application.DoEvents();

				progressBar.Visible = progressText.Visible = false;
			}
		}

		public override void OnEnteringStep(WizardParameters parameters)
		{
			session_token = (string)parameters.Get("session_token");
			user_id = (string)parameters.Get("user_id");
		}
	}
}