using System;
using System.Windows.Forms;

using Waveface.API.V2;

namespace Waveface.SettingUI
{
    public partial class PreferenceForm : Form
    {
		WService m_service;
		string sessionToken;
		string email;

        public PreferenceForm(string sessionToken, string email)
        {
            InitializeComponent();
			this.m_service = new WService();
			this.sessionToken = sessionToken;
			this.email = email;
        }

        private void PreferenceForm_Load(object sender, EventArgs e)
        {
			this.lblUserName.Text = email;
			MR_station_status stationStatus = m_service.GetStationStatus(sessionToken);
			long usedSize = 0;
			foreach (DiskUsage du in stationStatus.station_status.diskusage)
			{
				usedSize += du.used;
			}
			this.lblLocalStorageUsage.Text = string.Format("{0:0.0} MB", usedSize / (1024 * 1024));
			this.lblDeviceName.Text = stationStatus.station_status.computer_name;
        }

		private void btnOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
    }
}
