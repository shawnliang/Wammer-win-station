#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using Waveface.API.V2;
using AppLimit.NetSparkle;
#endregion

namespace Waveface.SettingUI
{
    public partial class SettingForm : Form
    {
        #region StorageUsage

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
    }
}