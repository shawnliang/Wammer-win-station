using System.Collections.Generic;
using System.Windows.Forms;
using Wammer.Station.Management;

namespace Wammer.Station
{
    public partial class DropboxForm : Form
    {
        public DropboxForm()
        {
            InitializeComponent();
        }

        private void buttonUserDropbox_Click(object sender, System.EventArgs e)
        {
            Hide();

            if (!CheckDropboxInstalled())
            {
                DropboxInstallForm _installForm = new DropboxInstallForm();
                DialogResult _dr = _installForm.ShowDialog();

                if(_dr != DialogResult.Yes)
                {
                    SetupCompletedForm _form = new SetupCompletedForm();
                    _form.ShowDialog();
                }
            }

            DropboxLinkForm _linkForm = new DropboxLinkForm();
            _linkForm.ShowDialog();
        }

        private bool CheckDropboxInstalled()
        {
            // ----------------------------
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

        private void buttonSkip_Click(object sender, System.EventArgs e)
        {
            Hide();

            SetupCompletedForm _form = new SetupCompletedForm();
            _form.ShowDialog();
        }
    }
}
