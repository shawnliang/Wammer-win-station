#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using Waveface.API.V2;
using AppLimit.NetSparkle;
#endregion

namespace Waveface.SettingUI
{
    public partial class SettingForm : Form
    {
        #region AllData

        public class AllData
        {
            public StorageUsage storageusage { get; set; }
            public List<Station> mystations { get; set; }
        }

        public class StorageUsage
        {
            public long quota { get; set; }
            public long usage { get; set; }
            public int daysLeft { get; set; }
        }

        #endregion

        // private static Logger s_logger = LogManager.GetCurrentClassLogger();
        Sparkle m_autoUpdator;
        public bool isUnlink = false;
        List<StationInfo> stationList = new List<StationInfo>();

        public SettingForm(Sparkle autoUpdator)
        {
            m_autoUpdator = autoUpdator;
            InitializeComponent();
        }

        private void PreferenceForm_Load(object sender, EventArgs e)
        {
            lblUserName.Text = Main.Current.RT.Login.user.email;

            string _execPath = Assembly.GetExecutingAssembly().Location;
            FileVersionInfo version = FileVersionInfo.GetVersionInfo(_execPath);
            lblVersion.Text = version.FileVersion;

            btnOK.Select();

            Cursor = Cursors.WaitCursor;
            bgworkerGetAllData.RunWorkerAsync();
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
                           delegate { RefreshCloudStorage(storage); }
                           ));
            }
            else
            {
                if (storage.quota < 0)
                    label_MonthlyLimitValue.Text = I18n.L.T("MonthlyUsage_Unlimited");
                else
                    label_MonthlyLimitValue.Text = storage.quota.ToString();

                label_DaysLeftValue.Text = storage.daysLeft.ToString();
                label_UsedCountValue.Text = storage.usage.ToString();

                barCloudUsage.Value = (int) (storage.usage*100/storage.quota);

                lblLoadingUsage.Visible = false;
                label_MonthlyLimit.Visible = true;
                label_MonthlyLimitValue.Visible = true;
                label_UsedCount.Visible = true;
                label_UsedCountValue.Visible = true;
                label_DaysLeft.Visible = true;
                label_DaysLeftValue.Visible = true;
                barCloudUsage.Visible = true;
            }
        }

        private class StationInfo
        {
            public Station info;
            public bool isThisPC;

            public StationInfo(Station info, bool isThisPC)
            {
                this.info = info;
                this.isThisPC = isThisPC;
            }

            public string Display
            {
                get
                {
                    if (isThisPC)
                        return info.computer_name + " " + I18n.L.T("ThisPC");
                    else
                        return info.computer_name;
                }
            }

            public string Id
            {
                get
                {
                    return info.station_id;
                }
            }
        }

        private void RefreshLinkedComputers(List<Station> mystations)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                            delegate { RefreshLinkedComputers(mystations); }
                            ));
            }
            else
            {
                lblLoadingStations.Visible = false;
                StationInfo thisPC = null;
                foreach (Station station in mystations)
                {
                    StationInfo stationInfo;

                    if (station.station_id == (string)StationRegHelper.GetValue("stationId", ""))
                    {
                        stationInfo = new StationInfo(station, true);
                        thisPC = stationInfo;
                    }
                    else
                    {
                        stationInfo = new StationInfo(station, false);
                    }

                    stationList.Add(stationInfo);

                    if (station.type == "primary")
                    {
                        lblPrimaryStation.Text = stationInfo.Display;
                    }
                }
                cmbStations.DataSource = stationList;
                cmbStations.DisplayMember = "Display";
                cmbStations.ValueMember = "Id";
                if (thisPC != null)
                    cmbStations.SelectedValue = thisPC.Id;
                else
                    cmbStations.SelectedValue = stationList[0].Id;

                cmbStations.Visible = true;
                lblLastSync.Visible = true;
                lblLastSyncValue.Visible = true;
                lblStorageUsage.Visible = true;
                lblStorageUsageValue.Visible = true;
                lblOriginDesc.Visible = true;
                lblPrimaryStation.Visible = true;
                btnChangeLoc.Visible = true;
                btnUnlink.Visible = true;
            }
        }

        private void bgworkerGetAllData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                AllData alldata = (AllData)e.Result;
                RefreshCloudStorage(alldata.storageusage);
                RefreshLinkedComputers(alldata.mystations);
            }
            else
            {
                lblLoadingUsage.Text = I18n.L.T("LoadDataFailed");
                lblLoadingStations.Text = I18n.L.T("LoadDataFailed");
            }
            Cursor = Cursors.Default;
        }

        private void bgworkerGetAllData_DoWork(object sender, DoWorkEventArgs e)
        {
            WService _service = new WService();
            MR_storages_usage _storageUsage = _service.storages_usage(Main.Current.RT.Login.session_token);
            long _quota = _storageUsage.storages.waveface.quota.month_total_objects;
            long _usage = _storageUsage.storages.waveface.usage.month_total_objects;
            int _daysLeft = _storageUsage.storages.waveface.interval.quota_interval_left_days;

            MR_users_findMyStation _myStations = _service.users_findMyStation(Main.Current.RT.Login.session_token);

            e.Result = new AllData {
                storageusage = new StorageUsage {quota = _quota, usage = _usage, daysLeft = _daysLeft},
                mystations = _myStations.stations
            };
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            string _userProfileUrl = WService.WebURL + "/user/profile";
            Process.Start(WService.WebURL + "/login?cont=" + HttpUtility.UrlEncode(_userProfileUrl), null);
        }

        private void linkLegalNotice_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(WService.WebURL + "/page/privacy", null);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            bgworkerUpdate.RunWorkerAsync();
            btnUpdate.Text = I18n.L.T("CheckingUpdates");
            btnUpdate.Enabled = false;
        }

        private void bgworkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            NetSparkleAppCastItem lastVersion;

            if (m_autoUpdator.IsUpdateRequired(m_autoUpdator.GetApplicationConfig(), out lastVersion))
                e.Result = lastVersion;
            else
                e.Result = null;
        }

        private void bgworkerUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            NetSparkleAppCastItem lastVersion = e.Result as NetSparkleAppCastItem;

            if (lastVersion != null)
            {
                m_autoUpdator.ShowUpdateNeededUI(lastVersion);
            }
            else
                MessageBox.Show(I18n.L.T("AlreadyUpdated"));

            btnUpdate.Enabled = true;
            btnUpdate.Text = I18n.L.T("CheckForUpdates");
        }

        private void btnUnlink_Click(object sender, EventArgs e)
        {
            UnlinkForm unlinkform = new UnlinkForm();
            DialogResult res = unlinkform.ShowDialog();
            if (res == DialogResult.OK)
            {
                isUnlink = true;
                Close();
            }
        }

        private void cmbStations_SelectedValueChanged(object sender, EventArgs e)
        {
            string id = cmbStations.SelectedValue.ToString();
            foreach (StationInfo station in stationList)
            {
                if (id == station.Id)
                {
                    DateTime lastSync = DateTimeHelp.ConvertUnixTimestampToDateTime(long.Parse(station.info.last_seen));
                    string lastSyncString = DateTimeHelp.PrettyDate(lastSync.ToString());
                    if (lastSyncString == lastSync.ToString())
                        lblLastSyncValue.Text = lastSync.ToString("MM/dd tt hh:mm:ss");
                    else
                        lblLastSyncValue.Text = lastSyncString;
                    if (station.info.diskusage.Count > 0)
                    {
                        float usage = FileUtility.ConvertBytesToMegaBytes(station.info.diskusage[0].used);
                        lblStorageUsageValue.Text = usage.ToString("F") + " MB";
                    }
                    else
                        lblStorageUsageValue.Text = "N/A";

                    if (station.isThisPC)
                    {
                        btnChangeLoc.Enabled = true;
                        btnUnlink.Enabled = true;
                    }
                    else
                    {
                        btnChangeLoc.Enabled = false;
                        btnUnlink.Enabled = false;
                    }
                }
            }
        }
    }
}