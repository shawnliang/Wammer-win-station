#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Microsoft.Win32;
using NLog;
using Waveface.API.V2;

#endregion

namespace Waveface.SettingUI
{
    #region PreferenceForm

    public partial class PreferenceForm : Form
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private static string AUTO_RUN_SUB_KEY = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private static string AUTO_RUN_REG_KEY = @"HKEY_CURRENT_USER\" + AUTO_RUN_SUB_KEY;
        private static string AUTO_RUN_VALUE_NAME = @"WavefaceStation";

        private string m_stationToken;
        private WService m_wavefaceService;
        private WavefaceConnectionTester m_connectionTester;

        public bool IsUserSwitched { get; private set; }

        public PreferenceForm(string stationToken, WService wavefaceService)
        {
            m_stationToken = stationToken;
            m_wavefaceService = wavefaceService;

            IsUserSwitched = false;

            m_connectionTester = new WavefaceConnectionTester(
                this,
                Main.Current.RT.Login.session_token,
                wavefaceService,
                Main.Current.RT.Login.user.user_id);

            InitializeComponent();
        }

        private void PreferenceForm_Load(object sender, EventArgs e)
        {
            lblUserName.Text = Main.Current.RT.Login.user.email;

            MR_station_status _stationStatus = Main.Current.RT.REST.GetStationStatus();

            long _usedSize = 0;

            foreach (DiskUsage _du in _stationStatus.station_status.diskusage)
            {
                _usedSize += _du.used;
            }

            lblLocalStorageUsage.Text = string.Format("{0:0.0} MB", _usedSize / (1024 * 1024));
            lblDeviceName.Text = _stationStatus.station_status.computer_name;

            string _execPath = Assembly.GetExecutingAssembly().Location;
            FileVersionInfo version = FileVersionInfo.GetVersionInfo(_execPath);
            lblVersion.Text = version.FileVersion;

            LoadDropboxUI();

            LoadAutoStartCheckbox();

            bgworkerGetAllData.RunWorkerAsync(Main.Current.RT.Login.session_token);
        }

        private void LoadDropboxUI()
        {
            try
            {
                MR_cloudstorage_list _cloudStorage = m_wavefaceService.cloudstorage_list(m_stationToken);

                if (_cloudStorage.cloudstorages.Count > 0 && _cloudStorage.cloudstorages[0].connected)
                {
                    btnDropboxAction.Click -= btnConnectDropbox_Click;
                    btnDropboxAction.Click -= btnUnlinkDropbox_Click;
                    btnDropboxAction.Click += btnUnlinkDropbox_Click;
                    btnDropboxAction.Text = I18n.L.T("DropboxUI_Disconnect");

                    label_dropboxAccount.Text = _cloudStorage.cloudstorages[0].account;
                }
                else
                {
                    ShowNoDropbox();
                }
            }
            catch (Exception _e)
            {
                s_logger.WarnException("Unable to list cloud storage", _e);
                MessageBox.Show("Unable to list cloud storage:" + _e.Message, "Waveface");

                ShowNoDropbox();
            }
        }

        private void ShowNoDropbox()
        {
            btnDropboxAction.Click -= btnUnlinkDropbox_Click;
            btnDropboxAction.Click -= btnConnectDropbox_Click;
            btnDropboxAction.Click += btnConnectDropbox_Click;
            btnDropboxAction.Text = I18n.L.T("DropboxUI_ConnectNow");

            label_dropboxAccount.Text = I18n.L.T("MonthlyUsage_NotConnectedYet");
        }

        private void LoadAutoStartCheckbox()
        {
            RegistryKey _key = Registry.CurrentUser.OpenSubKey(AUTO_RUN_SUB_KEY);

            if (_key == null)
                return;

            checkBox_autoStartWaveface.Checked = (_key.GetValue(AUTO_RUN_VALUE_NAME) != null);
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

                barCloudUsage.Value = (int)(storage.usage * 100 / storage.quota);
            }
        }

        private void bgworkerGetAllData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RefreshCloudStorage((StorageUsage)e.Result);
        }

        private void bgworkerGetAllData_DoWork(object sender, DoWorkEventArgs e)
        {
            string session_token = (string)e.Argument;
            WService _service = new WService();
            MR_storages_usage _storageUsage = _service.storages_usage(session_token);

            long _quota = _storageUsage.storages.waveface.quota.month_total_objects;
            long _usage = _storageUsage.storages.waveface.usage.month_total_objects;
            int _daysLeft = _storageUsage.storages.waveface.interval.quota_interval_left_days;

            e.Result = new StorageUsage { quota = _quota, usage = _usage, daysLeft = _daysLeft };
        }

        private void btnUnlinkDropbox_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                m_wavefaceService.cloudstorage_dropbox_disconnect(m_stationToken);
                ShowNoDropbox();
            }
            catch (Exception _e)
            {
                s_logger.ErrorException("Unable to unlink dropbox", _e);
                
                ShowNoDropbox();
                
                MessageBox.Show("Unable to unlink cloud storage:" + _e.Message, "Waveface");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnConnectDropbox_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                string _execPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                               "StationSetup.exe");
                Process _proc = Process.Start(_execPath, "--dropbox");
                Enabled = false;

                Thread _thread = new Thread(new WaitDropbopComplete(this, _proc).Do);
                _thread.Start();
            }
            catch (Exception _e)
            {
                s_logger.ErrorException("Unable to connect to dropbox", _e);

                ShowNoDropbox();

                MessageBox.Show("Unable to connect to cloud storage:" + _e.Message, "Waveface");
            }
        }

        public void ConnectDropboxComplete()
        {
            LoadDropboxUI();

            Enabled = true;

            Cursor.Current = Cursors.Default;
        }

        public void ConnectDropboxFailed()
        {
            ShowNoDropbox();

            Enabled = true;

            Cursor.Current = Cursors.Default;
        }

        public void TestConnectionComplete(string result)
        {
            labelConnectionStatus.Text = result;
            btnTestConnection.Enabled = true;
        }

        private void label_switchAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult _confirm = MessageBox.Show(I18n.L.T("Main.ChangeOwnerWarning", lblUserName.Text), "Waveface",
                                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (_confirm == DialogResult.No)
                return;

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                ProgramSetting _settings = new ProgramSetting();

                WService.RemoveOwner(_settings.Email, _settings.Password, m_stationToken);

                MessageBox.Show(I18n.L.T("Main.ChangeOwnerSuccess", _settings.Email), "waveface");

                _settings.Email = _settings.Password = "";
                _settings.IsLoggedIn = false;
                _settings.Save();

                IsUserSwitched = true;
                Close();
            }
            catch (Exception _e)
            {
                MessageBox.Show(I18n.L.T("ChangeOwnerError") + " : " + _e, "waveface");
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
                    string _installDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string _stationSetupPath = Path.Combine(_installDir, "StationSetup.exe");

                    Registry.SetValue(AUTO_RUN_REG_KEY, AUTO_RUN_VALUE_NAME, "\"" + _stationSetupPath + "\"");
                }
                else
                {
                    RegistryKey _curUserRegKey = Registry.CurrentUser.OpenSubKey(AUTO_RUN_SUB_KEY, true);

                    if (_curUserRegKey == null)
                        return;

                    _curUserRegKey.DeleteValue(AUTO_RUN_VALUE_NAME, false);
                }
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.Message);
            }
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            string _userProfileUrl = WService.WebURL + "/user/profile";
            Process.Start(WService.WebURL + "/login?cont=" + HttpUtility.UrlEncode(_userProfileUrl), null);
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            labelConnectionStatus.Text = I18n.L.T("Testing");
            btnTestConnection.Enabled = false;

            m_connectionTester.Start();
        }

        private void linkLegalNotice_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(WService.WebURL + "/page/privacy", null);
        }

        private void PreferenceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_connectionTester.Stop();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {
        }
    }

    #endregion

    #region StorageUsage

    public class StorageUsage
    {
        public long quota { get; set; }
        public long usage { get; set; }
        public int daysLeft { get; set; }
    }

    #endregion

    #region WaitDropbopComplete

    internal class WaitDropbopComplete
    {
        private PreferenceForm m_form;
        private Process m_configProcess;

        public WaitDropbopComplete(PreferenceForm form, Process configProcess)
        {
            m_form = form;
            m_configProcess = configProcess;
        }

        public void Do()
        {
            try
            {
                m_configProcess.WaitForExit();

                m_form.Invoke(new MethodInvoker(m_form.ConnectDropboxComplete));
            }
            catch (Exception _e)
            {
                MessageBox.Show("Waiting dropbox complete failed... " + _e.Message, "Waveface");

                m_form.Invoke(new MethodInvoker(m_form.ConnectDropboxFailed));
            }
        }
    }

    #endregion

    #region WavefaceConnectionTester

    internal class WavefaceConnectionTester
    {
        private PreferenceForm m_form;
        private string m_sessionToken;
        private WService m_wf;
        private string m_userId;
        private Thread m_thread;

        public WavefaceConnectionTester(PreferenceForm form, string sessionToken, WService wf, string userId)
        {
            m_form = form;
            m_sessionToken = sessionToken;
            m_wf = wf;
            m_userId = userId;
        }

        private void Do()
        {
            try
            {
                DateTime _startTime = DateTime.Now;

                try
                {
                    m_wf.pingMyStation(m_sessionToken);
                }
                catch
                {
                    NotifyTestDone(I18n.L.T("NotConnected"));

                    return;
                }

                string _result = I18n.L.T("NotConnected");

                do
                {
                    Thread.Sleep(2000);

                    try
                    {
                        MR_users_get _user = m_wf.users_get(m_sessionToken, m_userId);

                        if (_user.stations.Count > 0 &&
                            _user.stations[0].accessible != null &&
                            _user.stations[0].accessible == "available")
                        {
                            _result = I18n.L.T("Connected");

                            break;
                        }
                    }
                    catch
                    {
                    }
                } while (DateTime.Now - _startTime < TimeSpan.FromSeconds(10));

                NotifyTestDone(_result);
            }
            catch (ThreadAbortException)
            {
            }
        }

        public void Start()
        {
            m_thread = new Thread(Do);
            m_thread.Start();
        }

        public void Stop()
        {
            if (m_thread != null)
            {
                if (m_thread.IsAlive)
                {
                    m_thread.Abort();
                    m_thread.Join();
                }
            }
        }

        private void NotifyTestDone(string result)
        {
            m_form.Invoke(
                new MethodInvoker(delegate { m_form.TestConnectionComplete(result); })
                );
        }
    }

    #endregion
}