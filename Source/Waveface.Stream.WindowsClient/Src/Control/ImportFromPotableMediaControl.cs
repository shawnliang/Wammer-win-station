using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Model;
using System.Collections.Generic;

namespace Waveface.Stream.WindowsClient
{
	public partial class ImportFromPotableMediaControl : StepPageControl
	{
		private IPortableMediaService service;
		private volatile bool canceled;
		private string user_id;
		private string session_token;
		private int import_count;

		public ImportFromPotableMediaControl()
		{
			InitializeComponent();
			progressBar.Value = 0;
			this.Service = new NullPortableMediaService();
			this.PageTitle = "Import from media";
		}

		public ImportFromPotableMediaControl(IPortableMediaService service)
			:this()
		{
			this.Service = service;
		}

		public IPortableMediaService Service
		{
			get { return service; }
			set 
			{
				this.service = value;
				this.service.ImportDone += service_ImportDone;
				this.service.FileImported += service_FileImported;
			}
		}

		public void ImportDevice(string driveName)
		{
			foreach (PortableDevice dev in deviceCombobox.Items)
			{
				if (dev.DrivePath.Equals(driveName, StringComparison.InvariantCultureIgnoreCase))
				{
					deviceCombobox.SelectedItem = dev;
					importDevice(dev);
					return;
				}
			}
		}

		public void SelectDevice(string driveName)
		{
			foreach (PortableDevice dev in deviceCombobox.Items)
			{
				if (dev.DrivePath.Equals(driveName, StringComparison.InvariantCultureIgnoreCase))
				{
					deviceCombobox.SelectedItem = dev;
					return;
				}
			}
		}

		void service_FileImported(object sender, FileImportedEventArgs e)
		{
			import_count++;

			progressBar.Invoke(new MethodInvoker(() =>
			{
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
				dataGridView1.Rows.Add(deviceCombobox.SelectedItem, import_count);
				importButton.Enabled = true;
			}));
		}

		private void ImportFromPotableMediaControl_Load(object sender, EventArgs e)
		{
			refreshDrives();
			dataGridView1.Rows.Clear();
		}

		private void refreshDrives()
		{
			deviceCombobox.Items.Clear();
			deviceCombobox.Items.AddRange(service.GetPortableDevices().ToArray());
		}

		private void importButton_Click(object sender, EventArgs e)
		{
			var device = deviceCombobox.SelectedItem as PortableDevice;

			importDevice(device);
		}

		private void importDevice(PortableDevice device)
		{
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

		private void deviceCombobox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (deviceCombobox.SelectedItem != null)
			{
				var device = (PortableDevice)deviceCombobox.SelectedItem;
				checkBox1.Checked = service.GetAlwaysAutoImport(device.DrivePath);
			}
		}

		private void checkBox1_Click(object sender, EventArgs e)
		{
			if (deviceCombobox.SelectedItem != null)
			{
				var device = (PortableDevice)deviceCombobox.SelectedItem;

				try
				{
					service.SetAlwaysAutoImport(device.DrivePath, checkBox1.Checked);
				}
				catch (System.IO.IOException ioe)
				{
					MessageBox.Show(ioe.Message, "Unable to save");
				}
			}
		}
	}

	internal class NullPortableMediaService : IPortableMediaService
	{

		public event EventHandler<FileImportedEventArgs> FileImported;

		public event EventHandler<ImportDoneEventArgs> ImportDone;

		public System.Collections.Generic.IEnumerable<PortableDevice> GetPortableDevices()
		{
			return new List<PortableDevice>();
		}

		public System.Collections.Generic.IEnumerable<string> GetFileList(string path)
		{
			return new List<string>();
		}

		public void ImportAsync(System.Collections.Generic.IEnumerable<string> files, string user_id, string session_token, string apikey)
		{
		}

		public bool GetAlwaysAutoImport(string driveName)
		{
			return false;
		}

		public void SetAlwaysAutoImport(string driveName, bool autoImport)
		{
		}
	}
}