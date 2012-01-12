#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using Waveface.API.V2;

#endregion

namespace Waveface.SettingUI
{
    public partial class SettingForm : Form
    {
        // private static Logger s_logger = LogManager.GetCurrentClassLogger();

        public bool IsUserSwitched { get; private set; }

        public SettingForm()
        {
            IsUserSwitched = false;

            InitializeComponent();
        }

        private void PreferenceForm_Load(object sender, EventArgs e)
        {
            lblUserName.Text = Main.Current.RT.Login.user.email;

            string _execPath = Assembly.GetExecutingAssembly().Location;
            FileVersionInfo version = FileVersionInfo.GetVersionInfo(_execPath);
            lblVersion.Text = version.FileVersion;

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
            }
        }

        private void bgworkerGetAllData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RefreshCloudStorage((StorageUsage) e.Result);
        }

        private void bgworkerGetAllData_DoWork(object sender, DoWorkEventArgs e)
        {
            WService _service = new WService();
            MR_storages_usage _storageUsage = _service.storages_usage(Main.Current.RT.Login.session_token);

            long _quota = _storageUsage.storages.waveface.quota.month_total_objects;
            long _usage = _storageUsage.storages.waveface.usage.month_total_objects;
            int _daysLeft = _storageUsage.storages.waveface.interval.quota_interval_left_days;

            e.Result = new StorageUsage {quota = _quota, usage = _usage, daysLeft = _daysLeft};
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
    }
}