#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using AppLimit.NetSparkle;
using Waveface.API.V2;

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


        #region StationInfo

        private class StationInfo
        {
            public Station Info;
            public bool IsThisPC;

            public StationInfo(Station info, bool isThisPC)
            {
                Info = info;
                IsThisPC = isThisPC;
            }

            public string Display
            {
                get
                {
                    if (IsThisPC)
                        return Info.computer_name + " " + I18n.L.T("ThisPC");
                    else
                        return Info.computer_name;
                }
            }

            public string Id
            {
                get { return Info.station_id; }
            }
        }

        #endregion

        // private static Logger s_logger = LogManager.GetCurrentClassLogger();

        public bool isUnlink;
        private Sparkle m_autoUpdator;
        private List<StationInfo> m_stationList = new List<StationInfo>();

        public SettingForm(Sparkle autoUpdator)
        {
            m_autoUpdator = autoUpdator;

            InitializeComponent();
        }

        private void PreferenceForm_Load(object sender, EventArgs e)
        {
            lblUserName.Text = Main.Current.RT.Login.user.email;

            string _execPath = Assembly.GetExecutingAssembly().Location;
            FileVersionInfo _version = FileVersionInfo.GetVersionInfo(_execPath);
            lblVersion.Text = _version.FileVersion;

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
                {
                    label_MonthlyLimitValue.Text = I18n.L.T("MonthlyUsage_Unlimited");
                    barCloudUsage.Value = (int)(storage.usage * 100 / int.MaxValue);
                }
                else
                {
                    label_MonthlyLimitValue.Text = storage.quota.ToString();
                    barCloudUsage.Value = (int)(storage.usage * 100 / storage.quota);
                }

                label_DaysLeftValue.Text = storage.daysLeft.ToString();
                label_UsedCountValue.Text = storage.usage.ToString();

                label_LoadingUsage.Visible = false;
                label_MonthlyLimit.Visible = true;
                label_MonthlyLimitValue.Visible = true;
                label_UsedCount.Visible = true;
                label_UsedCountValue.Visible = true;
                label_DaysLeft.Visible = true;
                label_DaysLeftValue.Visible = true;

                barCloudUsage.Visible = true;
            }
        }

        private void RefreshLinkedComputers(List<Station> myStations)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { RefreshLinkedComputers(myStations); }
                           ));
            }
            else
            {
                lblLoadingStations.Visible = false;
                StationInfo _thisPc = null;

                foreach (Station _station in myStations)
                {
                    StationInfo _stationInfo;

                    if (_station.station_id == (string)StationRegHelper.GetValue("stationId", ""))
                    {
                        _stationInfo = new StationInfo(_station, true);
                        _thisPc = _stationInfo;
                    }
                    else
                    {
                        _stationInfo = new StationInfo(_station, false);
                    }

                    m_stationList.Add(_stationInfo);

                    if (_station.type == "primary")
                    {
                        lblPrimaryStation.Text = _stationInfo.Display;
                    }
                }

                cmbStations.DataSource = m_stationList;
                cmbStations.DisplayMember = "Display";
                cmbStations.ValueMember = "Id";
                
                if (_thisPc != null)
                    cmbStations.SelectedValue = _thisPc.Id;
                else
                    cmbStations.SelectedValue = m_stationList[0].Id;

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
                AllData _allData = (AllData)e.Result;
                RefreshCloudStorage(_allData.storageusage);
                RefreshLinkedComputers(_allData.mystations);
            }
            else
            {
                label_LoadingUsage.Text = I18n.L.T("LoadDataFailed");
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

            e.Result = new AllData
                           {
                               storageusage = new StorageUsage { quota = _quota, usage = _usage, daysLeft = _daysLeft },
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
            NetSparkleAppCastItem _lastVersion;

            if (m_autoUpdator.IsUpdateRequired(m_autoUpdator.GetApplicationConfig(), out _lastVersion))
                e.Result = _lastVersion;
            else
                e.Result = null;
        }

        private void bgworkerUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            NetSparkleAppCastItem _lastVersion = e.Result as NetSparkleAppCastItem;

            if (_lastVersion != null)
            {
                m_autoUpdator.ShowUpdateNeededUI(_lastVersion);
            }
            else
            {
                MessageBox.Show(I18n.L.T("AlreadyUpdated"));
            }

            btnUpdate.Enabled = true;
            btnUpdate.Text = I18n.L.T("CheckForUpdates");
        }

        private void btnUnlink_Click(object sender, EventArgs e)
        {
            UnlinkForm _form = new UnlinkForm();
            DialogResult _res = _form.ShowDialog();

            if (_res == DialogResult.OK)
            {
                isUnlink = true;

                Close();
            }
        }

        private void cmbStations_SelectedValueChanged(object sender, EventArgs e)
        {
            string id = cmbStations.SelectedValue.ToString();

            foreach (StationInfo station in m_stationList)
            {
                if (id == station.Id)
                {
                    DateTime _lastSync = DateTimeHelp.ConvertUnixTimestampToDateTime(long.Parse(station.Info.last_seen));
                    string _lastSyncString = DateTimeHelp.PrettyDate(_lastSync.ToString(), false);
                    
                    if (_lastSyncString == _lastSync.ToString())
                        lblLastSyncValue.Text = _lastSync.ToString("MM/dd tt hh:mm:ss");
                    else
                        lblLastSyncValue.Text = _lastSyncString;

                    if (station.Info.diskusage.Count > 0)
                    {
                        float _usage = FileUtility.ConvertBytesToMegaBytes(station.Info.diskusage[0].used);
                        lblStorageUsageValue.Text = _usage.ToString("F") + " MB";
                    }
                    else
                    {
                        lblStorageUsageValue.Text = "N/A";
                    }

                    if (station.IsThisPC)
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