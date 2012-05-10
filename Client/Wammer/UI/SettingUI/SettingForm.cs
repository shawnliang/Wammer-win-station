#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using AppLimit.NetSparkle;
using Waveface.API.V2;
using Waveface.Configuration;

#endregion

namespace Waveface.SettingUI
{
    public partial class SettingForm : Form
    {
        // private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private Sparkle m_autoUpdator;
        private FormSettings m_formSettings;

        public SettingForm(Sparkle autoUpdator)
        {
            m_autoUpdator = autoUpdator;

            InitializeComponent();

            m_formSettings = new FormSettings(this);
            m_formSettings.UseLocation = true;
            m_formSettings.UseSize = false;
            m_formSettings.UseWindowState = true;
            m_formSettings.AllowMinimized = false;
            m_formSettings.SaveOnClose = true;
        }

        private void PreferenceForm_Load(object sender, EventArgs e)
        {
            string _execPath = Assembly.GetExecutingAssembly().Location;
            FileVersionInfo _version = FileVersionInfo.GetVersionInfo(_execPath);
            lblVersion.Text = _version.FileVersion;

            btnOK.Select();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
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

        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_formSettings.Save();
        }
    }
}