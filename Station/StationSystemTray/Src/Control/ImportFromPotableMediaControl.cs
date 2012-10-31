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

		public ImportFromPotableMediaControl(IPortableMediaService service)
		{
			InitializeComponent();
			this.service = service;

			progressBar.Value = 0;
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
			importButton.Enabled = false;

			backgroundWorker.RunWorkerAsync(device);
		}

		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var device = e.Argument as PortableDevice;
			var files = service.GetFileList(device.DrivePath);

			var total = files.Count();
			progressBar.Invoke(new MethodInvoker(() => 
			{
				progressBar.Maximum = total;
			}));


			int processedCount = 0;

			foreach (var file in files)
			{
				if (canceled)
					break;

				processedCount++;

				progressText.Invoke(new MethodInvoker(() =>
				{
					progressText.Text = string.Format("{0}/{1} Importing {2}...", processedCount, total, file);
				}));

				service.Import(file);
				

				progressBar.Invoke(new MethodInvoker(() => {
					progressBar.PerformStep();
				}));
			}

			progressBar.Invoke(new MethodInvoker(() =>
			{
				progressText.Text = total + " files imported";
				progressBar.Value = progressBar.Maximum;
			}));

		}

		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MessageBox.Show(e.Error.Message);
			}

			importButton.Enabled = true;
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
	}
}