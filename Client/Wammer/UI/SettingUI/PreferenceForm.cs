using System;
using System.Windows.Forms;
using System.ComponentModel;
using NLog;
using System.IO;
using Microsoft.Win32;
using System.Reflection;

using Waveface.API.V2;
using NLog;
using System.Diagnostics;
using System.Threading;
using System.Web;

namespace Waveface.SettingUI
{
    public partial class PreferenceForm : Form
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private static string AUTO_RUN_SUB_KEY = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private static string AUTO_RUN_REG_KEY = @"HKEY_CURRENT_USER\" + AUTO_RUN_SUB_KEY;
        private static string AUTO_RUN_VALUE_NAME = @"WavefaceStation";
        private string stationToken;
        private WService wavefaceService;
        private WavefaceConnectionTester connectionTester;

        public bool IsUserSwitched { get; private set; }

        public PreferenceForm(string stationToken, WService wavefaceService)
        {
            this.stationToken = stationToken;
            this.wavefaceService = wavefaceService;
            this.IsUserSwitched = false;

            this.connectionTester = new WavefaceConnectionTester(
                this,
                Main.Current.RT.Login.session_token,
                wavefaceService,
                Main.Current.RT.Login.user.user_id);

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
            string execPath = Assembly.GetExecutingAssembly().Location;
            FileVersionInfo version = FileVersionInfo.GetVersionInfo(execPath);
            this.lblVersion.Text = version.FileVersion.ToString();
            LoadDropboxUI();
            LoadAutoStartCheckbox();
            bgworkerGetAllData.RunWorkerAsync(Main.Current.RT.Login.session_token);
        }

        private void LoadDropboxUI()
        {
            try
            {
                MR_cloudstorage_list cloudStorage = wavefaceService.cloudstorage_list(stationToken);
                if (cloudStorage.cloudstorages.Count > 0 && cloudStorage.cloudstorages[0].connected)
                {
                    ShowDropboxPanel(true);

                    label_dropboxAccount.Text = cloudStorage.cloudstorages[0].account;
                }
                else
                {
                    ShowDropboxPanel(false);
                }
            }
            catch (Exception e)
            {
                s_logger.WarnException("Unable to list cloud storage", e);
                MessageBox.Show("Unable to list cloud storage:" + e.Message, "Waveface");

                ShowDropboxPanel(false);
            }
        }

        private void LoadAutoStartCheckbox()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(AUTO_RUN_SUB_KEY);
            if (key == null)
                return;

            checkBox_autoStartWaveface.Checked = (key.GetValue(AUTO_RUN_VALUE_NAME) != null);
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
                this.label_MonthlyLimit.Text = I18n.L.T("MonthlyUsage_Limit", storage.quota);
                this.label_UsageStarting.Text = I18n.L.T("MonthlyUsage_Starting", storage.startTime.ToLocalTime());
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

        private void btnUnlinkDropbox_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                wavefaceService.cloudstorage_dropbox_disconnect(stationToken);
                ShowDropboxPanel(false);
            }
            catch (Exception ex)
            {
                s_logger.ErrorException("Unable to unlink dropbox", ex);
                ShowDropboxPanel(false);
                MessageBox.Show("Unable to unlink cloud storage:" + ex.Message, "Waveface");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void ShowDropboxPanel(bool dropboxInUse)
        {
            if (dropboxInUse)
            {
                panel_DropboxInUse.Show();
                panel_DropboxNotInUse.Hide();
            }
            else
            {
                panel_DropboxInUse.Hide();
                panel_DropboxNotInUse.Show();
            }
        }

        private void btnConnectDropbox_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                Process proc = Process.Start("StationSetup.exe", "--dropbox");
                this.Enabled = false;


                Thread thread = new Thread(new WaitDropbopComplete(this, proc).Do);
                thread.Start();
            }
            catch (Exception ex)
            {
                s_logger.ErrorException("Unable to connect to dropbox", ex);
                ShowDropboxPanel(false);
                MessageBox.Show("Unable to connect to cloud storage:" + ex.Message, "Waveface");
            }
        }

        public void ConnectDropboxComplete()
        {
            LoadDropboxUI();
            this.Enabled = true;
            Cursor.Current = Cursors.Default;
        }

        public void ConnectDropboxFailed()
        {
            ShowDropboxPanel(false);
            this.Enabled = true;
            Cursor.Current = Cursors.Default;
        }

        public void TestConnectionComplete(string result)
        {
            labelConnectionStatus.Text = result;
            btnTestConnection.Enabled = true;
        }

        private void label_switchAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult confirm = MessageBox.Show(I18n.L.T("Main.ChangeOwnerWarning", lblUserName.Text), "Waveface",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.No)
                return;

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                ProgramSetting settings = new ProgramSetting();

                WService.RemoveOwner(settings.Email, settings.Password, stationToken);

                MessageBox.Show(I18n.L.T("Main.ChangeOwnerSuccess", settings.Email), "waveface");

                settings.Email = settings.Password = "";
                settings.IsLoggedIn = false;
                settings.Save();

                IsUserSwitched = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(I18n.L.T("ChangeOwnerError") + " : " + ex, "waveface");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void checkBox_autoStartWaveface_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkBox_autoStartWaveface.Checked)
                {
                    string installDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string stationSetupPath = System.IO.Path.Combine(installDir, "StationSetup.exe");
                    Registry.SetValue(AUTO_RUN_REG_KEY, AUTO_RUN_VALUE_NAME, "\"" + stationSetupPath + "\"");
                }
                else
                {
                    RegistryKey CurUserRegKey = Registry.CurrentUser.OpenSubKey(AUTO_RUN_SUB_KEY, true);

                    if (CurUserRegKey == null)
                        return;

                    CurUserRegKey.DeleteValue(AUTO_RUN_VALUE_NAME, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            string userProfileUrl = WService.WebURL + "/user/profile";
            Process.Start(WService.WebURL + "/login?cont=" + HttpUtility.UrlEncode(userProfileUrl), null);
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            labelConnectionStatus.Text = I18n.L.T("Testing");
            btnTestConnection.Enabled = false;

            connectionTester.Start();
        }

        private void linkLegalNotice_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(WService.WebURL + "/page/privacy", null);
        }

        private void PreferenceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            connectionTester.Stop();
        }

		private void label_MonthlyLimit_Click(object sender, EventArgs e)
		{

		}
    }

    public class StorageUsage
    {
        public long quota { get; set; }
        public long usage { get; set; }
        public DateTime startTime { get; set; }
    }

    class WaitDropbopComplete
    {
        PreferenceForm form;
        Process configProcess;

        public WaitDropbopComplete(PreferenceForm form, Process configProcess)
        {
            this.form = form;
            this.configProcess = configProcess;
        }

        public void Do()
        {
            try
            {
                configProcess.WaitForExit();
                form.Invoke(new MethodInvoker(form.ConnectDropboxComplete));
            }
            catch (Exception e)
            {
                MessageBox.Show("Waiting dropbox complete failed... " + e.Message, "Waveface");
                form.Invoke(new MethodInvoker(form.ConnectDropboxFailed));
            }
        }
    }

    class WavefaceConnectionTester
    {
        PreferenceForm form;
        string sessionToken;
        WService wf;
        string userId;
        Thread thread;

        public WavefaceConnectionTester(PreferenceForm form, string sessionToken, WService wf, string userId)
        {
            this.form = form;
            this.sessionToken = sessionToken;
            this.wf = wf;
            this.userId = userId;
        }

        private void Do()
        {
            try
            {
                DateTime startTime = DateTime.Now;

                try
                {
                    wf.pingMyStation(sessionToken);
                }
                catch
                {
                    NotifyTestDone(I18n.L.T("NotConnected"));
                    return;
                }

                string result = I18n.L.T("NotConnected");
                do
                {
                    Thread.Sleep(2000);

                    try
                    {
                        MR_users_get user = wf.users_get(sessionToken, userId);

                        if (user.stations.Count > 0 &&
                            user.stations[0].accessible != null &&
                            user.stations[0].accessible == "available")
                        {
                            result = I18n.L.T("Connected");
                            break;
                        }
                    }
                    catch
                    {
                    }
                }
                while (DateTime.Now - startTime < TimeSpan.FromSeconds(10));

                NotifyTestDone(result);
            }
            catch (ThreadAbortException)
            {

            }
        }

        public void Start()
        {
            thread = new Thread(this.Do);
            thread.Start();
        }

        public void Stop()
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
                thread.Join();
            }
        }

        private void NotifyTestDone(string result)
        {
            form.Invoke(
                    new MethodInvoker(delegate
                    {
                        form.TestConnectionComplete(result);
                    })
                );
        }
    }
}
