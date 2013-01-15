using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;
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
		private string curTaskId;

		private Timer timer;

		public ImportFromPotableMediaControl()
		{
			InitializeComponent();
			progressBar.Value = 0;
			this.Service = new NullPortableMediaService();
			this.PageTitle = "Import from media";

			timer = new Timer();
			timer.Interval = 1000;
			timer.Tick += new EventHandler(timer_Tick);

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
			importButton.Enabled = false;

			curTaskId = service.ImportAsync(device.DrivePath, user_id, session_token, StationAPI.API_KEY);
			timer.Start();
		}

		private void refreshTaskStatus(string taskId)
		{
			if (canceled)
			{
				timer.Stop();
				return;
			}

			var taskStatus = service.QueryTaskStatus(taskId);

			if (taskStatus.IsCopyComplete)
			{
				if (string.IsNullOrEmpty(taskStatus.Error))
				{
					dataGridView1.Rows.Add(deviceCombobox.SelectedItem, taskStatus.Copied);
				}
				else
				{
					dataGridView1.Rows.Add(deviceCombobox.SelectedItem, taskStatus.Error);
				}

				progressBar.Maximum = taskStatus.Total;
				progressBar.Value = taskStatus.Total;
				progressText.Text = string.Format("{0} imported. {1} failed. {2} already imported.", taskStatus.Copied, taskStatus.CopyFailed.Count, taskStatus.Skipped);
				timer.Stop();
				importButton.Enabled = true;
			}
			else
			{
				progressBar.Maximum = taskStatus.Total;
				progressBar.Value = taskStatus.Skipped + taskStatus.Copied;
				progressText.Text = string.Format("{0} files processed", taskStatus.Copied + taskStatus.CopyFailed.Count + taskStatus.Skipped);
			}
		}

		private void deviceCombobox_DropDown(object sender, EventArgs e)
		{
			refreshDrives();
		}

		public override void OnLeavingStep(WizardParameters parameters)
		{
			canceled = true;
			progressBar.Visible = progressText.Visible = false;
		}

		public override void OnEnteringStep(WizardParameters parameters)
		{
			session_token = (string)parameters.Get("session_token");
			user_id = (string)parameters.Get("user_id");
		}

		void timer_Tick(object sender, EventArgs e)
		{
			refreshTaskStatus(curTaskId);
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


		public IEnumerable<PortableDevice> GetPortableDevices()
		{
			return new List<PortableDevice>();
		}

		public string ImportAsync(string drive_path, string user_id, string session_token, string apikey)
		{
			return "";
		}

		public ImportTaskStaus QueryTaskStatus(string taskId)
		{
			return new ImportTaskStaus();
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