using System.Collections.Generic;
using System.Windows.Forms;
using Wammer.Station.Management;
using Waveface.Component.MultiPage;

namespace Wammer.Station
{
    public partial class DropboxForm : Form
    {
        private bool m_canExit;

        public DropboxForm()
        {
            InitializeComponent();

            gotoPage(Page_Welcome);
        }

        private void gotoPage(MultiPanelPage page)
        {
            multiPanel.SelectedPage = page;

            if ((page == Page_SetupCompleted) || (page == Page_ConnectionSuccessfully))
                m_canExit = true;
        }

        #region Welcome

        private void btn_Welcome_Skip_Click(object sender, System.EventArgs e)
        {
            gotoPage(Page_SetupCompleted);
        }

        private void btn_Welcome_UseDropbox_Click(object sender, System.EventArgs e)
        {
            if (CheckDropboxInstalled_1())
            {
                gotoPage(Page_Linkage_1);
            }
            else
            {
                gotoPage(Page_InstallDropbox_1);
            }
        }

        #endregion

        #region Install Dropbox

        private void btn_InstallDropbox_1_Install_Click(object sender, System.EventArgs e)
        {
            gotoPage(Page_InstallDropbox_2);

            //Open Dropbox Web
        }

        private void btn_InstallDropbox_2_InstallAgain_Click(object sender, System.EventArgs e)
        {
            //Open Dropbox Web
        }

        private void btn_InstallDropbox_2_continue_Click(object sender, System.EventArgs e)
        {
            if (CheckDropboxInstalled_2())
            {
                gotoPage(Page_Linkage_1);
            }
            else
            {
                gotoPage(Page_InstallationFailed);
            }
        }

        private bool CheckDropboxInstalled_1()
        {
            return false;

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

        private bool CheckDropboxInstalled_2()
        {
            return true;

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

        #endregion

        private void btn_ConnectionFailed_Skip_Click(object sender, System.EventArgs e)
        {
            gotoPage(Page_SetupCompleted);
        }

        private void btn_ConnectionFailed_Retry_Click(object sender, System.EventArgs e)
        {
            gotoPage(Page_Linkage_1);
        }

        private void btn_InstallationFailed_Skip_Click(object sender, System.EventArgs e)
        {
            gotoPage(Page_SetupCompleted);
        }

        private void btn_InstallationFailed_Retry_Click(object sender, System.EventArgs e)
        {
            gotoPage(Page_InstallDropbox_1);
        }

        private void btn_Linkage_1_Connect_Click(object sender, System.EventArgs e)
        {
            // Open linkage Web Page
            //

            gotoPage(Page_Linkage_2);
        }

        private void btn_Linkage_2_ConnectAgain_Click(object sender, System.EventArgs e)
        {
            // Open linkage Web Page
        }

        private void btn_Linkage_2_Verift_Click(object sender, System.EventArgs e)
        {
            doVerify();
        }

        private void doVerify()
        {
            gotoPage(Page_Verifying);

            doVerify_Real();
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

            if (_isOK)
            {
                gotoPage(Page_ConnectionSuccessfully);
            }
            else
            {
                gotoPage(Page_ConnectionFailed);
            }
        }

        private void btn_ConnectionSuccessfully_OpenWaveface_Click(object sender, System.EventArgs e)
        {
            doOpenWaveface();
        }

        private void DropboxForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_canExit)
            {
                doOpenWaveface();
            }
            else
            {
                DialogResult _dr = MessageBox.Show("Are you sure to quit the Dropbox setup process?", "Waveface", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (_dr == DialogResult.OK)
                {
                    gotoPage(Page_SetupCompleted);
                }

                e.Cancel = true;
            }
        }

        private void doOpenWaveface()
        {
            //
        }
    }
}
