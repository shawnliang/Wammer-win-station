using System;
using System.Windows.Forms;
using Wammer.Station.Management;

namespace Wammer.Station
{
    public partial class DropboxLinkForm : Form
    {
        public DropboxLinkForm()
        {
            InitializeComponent();
        }

        private void buttonVerift_Click(object sender, EventArgs e)
        {
            bool _isOK = true;

            multiPanel.SelectedPage = Page_Verify;
            
            try
            {
                StationController.ConnectDropbox(1024 * 1024 * 500);
            }
            catch(DropboxNoSyncFolderException)
            {
                _isOK = false;
            }
            catch(DropboxWrongAccountException)
            {
                _isOK = false;
            }

            if(_isOK)
            {
                multiPanel.SelectedPage = Page_successfully;
            }
            else
            {
                multiPanel.SelectedPage = Page_RetryOrSkip;
            }
        }
    }
}
