using System;
using System.Windows.Forms;
using Waveface.API.V2;
using NLog;
using System.Diagnostics;
using System.Threading;


namespace Waveface.SettingUI
{
    public partial class PreferenceForm : Form
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private string stationToken;
        private WService wavefaceService;

        public PreferenceForm(string stationToken, WService wavefaceService)
        {
            this.stationToken = stationToken;
            this.wavefaceService = wavefaceService;

            InitializeComponent();
        }

        private void PreferenceForm_Load(object sender, EventArgs e)
        {
            LoadDropboxUI();
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


        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
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
            panel_DropboxInUse.Visible = dropboxInUse;
            panel_DropboxNotInUse.Visible = !dropboxInUse;
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
}
