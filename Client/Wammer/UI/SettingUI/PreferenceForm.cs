using System;
using System.Windows.Forms;
using System.ComponentModel;
using NLog;

using Waveface.API.V2;

namespace Waveface.SettingUI
{
	public partial class PreferenceForm : Form
	{
		private static Logger s_logger = LogManager.GetCurrentClassLogger();

		public PreferenceForm()
		{
			InitializeComponent();
		}

		private void PreferenceForm_Load(object sender, EventArgs e)
		{
			this.lblUserName.Text = Main.Current.RT.Login.user.email;
			MR_station_status stationStatus = Main.Current.RT.REST.GetStationStatus();
			long usedSize = 0;
			foreach (DiskUsage du in stationStatus.station_status.diskusage)
			{
				usedSize += du.used;
			}
			this.lblLocalStorageUsage.Text = string.Format("{0:0.0} MB", usedSize / (1024 * 1024));
			this.lblDeviceName.Text = stationStatus.station_status.computer_name;

			bgworkerGetAllData.RunWorkerAsync(Main.Current.RT.Login.session_token);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void RefreshCloudStorage(StorageUsage storage)
		{
			if (InvokeRequired)
			{
				Invoke(new MethodInvoker(
						   delegate
						   {
							   RefreshCloudStorage(storage);
						   }
						   ));
			}
			else
			{
				this.lblCloudStorageLimit.Text = storage.quota.ToString();
				this.lblStartTime.Text = storage.startTime.ToString();
				this.barCloudUsage.Value = (int)(storage.usage * 100 / storage.quota);
			}
		}

		private void bgworkerGetAllData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			RefreshCloudStorage((StorageUsage)e.Result);
		}

		private void bgworkerGetAllData_DoWork(object sender, DoWorkEventArgs e)
		{
			string session_token = (string)e.Argument;
			WService service = new WService();
			MR_storages_usage storageUsage = service.storages_usage(session_token);
			long quota = storageUsage.storages.waveface.quota.month_total_objects;
			long usage = storageUsage.storages.waveface.usage.month_total_objects;
			DateTime startTime = new DateTime(1970,1,1,0,0,0,0);
			startTime = startTime.AddSeconds(storageUsage.storages.waveface.quota_starting_time);
			e.Result = new StorageUsage{quota = quota, usage = usage, startTime = startTime};
		}
	}

	public class StorageUsage
	{
		public long quota { get; set; }
		public long usage { get; set; }
		public DateTime startTime { get; set; }
	}
}