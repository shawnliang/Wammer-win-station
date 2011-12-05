#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using StationSetup;
using Wammer.Station.Management;
using Waveface.Component.MultiPage;

#endregion

namespace Wammer.Station
{
    public partial class DropboxForm : Form
    {
        private const string HOST = "http://develop.waveface.com:4343/";

        //Debug
        private bool m_debug = false;
        private bool m_installedDropbox_1 = true;
        private bool m_installedDropbox_2 = true;
        private bool m_Verify_OK;
        //

        private bool m_canExit;
        private bool m_doAutoPost;
        private string m_email;
        private string m_password;
        private string m_dropboxOAuthUrl = string.Empty;

        public DropboxForm(string email, string password)
        {
            m_email = email;
            m_password = password;

            InitializeComponent();

            gotoPage(Page_Welcome); // 1-1
        }

        private void gotoPage(MultiPanelPage page)
        {
            multiPanel.SelectedPage = page;

            if ((page == Page_SetupCompleted) || (page == Page_ConnectionSuccessfully))
                m_canExit = true;
        }

        private void btn_Welcome_Skip_Click(object sender, EventArgs e)
        {
            gotoPage(Page_SetupCompleted); // 1-2
        }

        private void btn_Welcome_UseDropbox_Click(object sender, EventArgs e)
        {
            if (CheckDropboxInstalled_1())
            {
                gotoPage(Page_Linkage_1); // 3-1
            }
            else
            {
                gotoPage(Page_InstallDropbox_1); // 2-1
            }
        }

        private void btn_InstallDropbox_1_Install_Click(object sender, EventArgs e)
        {
            gotoPage(Page_InstallDropbox_2); // 2-2

            OpenDropboxInstallWeb();
        }

        private void btn_InstallDropbox_2_InstallAgain_Click(object sender, EventArgs e)
        {
            OpenDropboxInstallWeb();
        }

        private void btn_InstallDropbox_2_continue_Click(object sender, EventArgs e)
        {
            if (CheckDropboxInstalled_2())
            {
                gotoPage(Page_Linkage_1); // 3-1
            }
            else
            {
                gotoPage(Page_InstallationFailed); // 2-3
            }
        }

        private void btn_ConnectionFailed_Skip_Click(object sender, EventArgs e)
        {
            gotoPage(Page_SetupCompleted); //1-2
        }

        private void btn_ConnectionFailed_Retry_Click(object sender, EventArgs e)
        {
            gotoPage(Page_Linkage_1); // 3-1
        }

        private void btn_InstallationFailed_Skip_Click(object sender, EventArgs e)
        {
            gotoPage(Page_SetupCompleted); // 1-2
        }

        private void btn_InstallationFailed_Retry_Click(object sender, EventArgs e)
        {
            gotoPage(Page_InstallDropbox_1);
        }

        private void btn_Linkage_1_Connect_Click(object sender, EventArgs e)
        {
            OpenLinkageWebPage();

            gotoPage(Page_Linkage_2); //3-2
        }

        private void btn_Linkage_2_ConnectAgain_Click(object sender, EventArgs e)
        {
            OpenLinkageWebPage();
        }

        private void btn_Linkage_2_Verift_Click(object sender, EventArgs e)
        {
            doVerify();
        }

        private void doVerify()
        {
            gotoPage(Page_Verifying); //4-1

            doVerify_Real();
        }

        private void btn_ConnectionSuccessfully_OpenWaveface_Click(object sender, EventArgs e)
        {
            OpenWindowsClient();
        }

        private void btn_SetupCompleted_OpenWaveface_Click(object sender, EventArgs e)
        {
            OpenWindowsClient();
        }

        private void btn_SetupCompleted_InstallAgain_Click(object sender, EventArgs e)
        {
            m_canExit = false;

            gotoPage(Page_Welcome); //1-1
        }

        private void DropboxForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_doAutoPost)
            {
                e.Cancel = true;
                return;
            }

            if (m_canExit)
            {
                OpenWindowsClient();
            }
            else
            {
                DialogResult _dr = MessageBox.Show(I18n.L.T("QuitDropboxSetupProcess"), "Waveface",
                                                   MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (_dr == DialogResult.OK)
                {
                    gotoPage(Page_SetupCompleted); // 1-2
                }

                e.Cancel = true;
            }
        }

        #region Real Work

        // CheckDropboxInstalled_1() 與 CheckDropboxInstalled_2() 程式碼一樣, 為了測試流程方便
        private bool CheckDropboxInstalled_1()
        {
            if (m_debug)
            {
                return m_installedDropbox_1;
            }

            return CheckDropboxInstalled_Real();
        }

        private bool CheckDropboxInstalled_2()
        {
            if (m_debug)
            {
                return m_installedDropbox_2;
            }

            return CheckDropboxInstalled_Real();
        }

        private bool CheckDropboxInstalled_Real()
        {
            List<StorageStatus> _storageStatuses = StationController.ListCloudStorage();

            if (_storageStatuses.Count == 0)
                return false;

            foreach (StorageStatus _status in _storageStatuses)
            {
                if (_status.type == "dropbox")
                    return true;
            }

            return false;
        }

        private void doVerify_Real()
        {
            bool _isOK = true;

            try
            {
                StationController.ConnectDropbox(1024 * 1024 * 500);
            }
            catch (DropboxNoSyncFolderException)
            {
                _isOK = false;
            }
            catch (DropboxWrongAccountException)
            {
                _isOK = false;
            }
            catch
            {
                _isOK = false;
            }

            if (m_debug)
            {
                _isOK = m_Verify_OK;
            }

            if (_isOK)
            {
                gotoPage(Page_ConnectionSuccessfully); //5-1
            }
            else
            {
                gotoPage(Page_ConnectionFailed); //4-2
            }
        }

        #endregion

        private void OpenWindowsClient()
        {
            gotoPage(Page_DefaultPosts);

            m_doAutoPost = true;

            backgroundWorker.RunWorkerAsync();
        }

        private void OpenDropboxInstallWeb()
        {
            Process.Start(HOST + "to?url=" + HttpUtility.UrlEncode("http://www.dropbox.com/"), null);
        }

        private void OpenLinkageWebPage()
        {
            if (m_dropboxOAuthUrl == string.Empty)
            {
                m_dropboxOAuthUrl = StationController.GetDropboxOAuthUrl();
            }

            Process.Start(HOST + "to?url=" + HttpUtility.UrlEncode(m_dropboxOAuthUrl), null);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DefaultPosts _posts = new DefaultPosts();
            _posts.AutoPost(m_email, m_password);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            m_doAutoPost = false;

            WavefaceWindowsClientHelper.StartWavefaceWindowsClient(m_email, m_password);
            Close();
        }
    }
}