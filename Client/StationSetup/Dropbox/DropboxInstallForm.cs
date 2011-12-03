using System.Collections.Generic;
using System.Windows.Forms;
using Wammer.Station.Management;

namespace Wammer.Station
{
    public partial class DropboxInstallForm : Form
    {
        public DropboxInstallForm()
        {
            InitializeComponent();
        }

        private void buttonNext_Click(object sender, System.EventArgs e)
        {
            if(CheckDropboxInstalled())
            {
                DialogResult = DialogResult.Yes;
                Close();
            }
            else
            {
                TryInstallDropboxAgainForm _form = new TryInstallDropboxAgainForm();
                DialogResult _dr = _form.ShowDialog();

                if(_dr == DialogResult.Cancel)
                {
                    Hide();

                    SetupCompletedForm _completedForm = new SetupCompletedForm();
                    _completedForm.ShowDialog();
                }
            }
        }

        private bool CheckDropboxInstalled()
        {
            // Test ---------------------------------------------
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
    }
}
