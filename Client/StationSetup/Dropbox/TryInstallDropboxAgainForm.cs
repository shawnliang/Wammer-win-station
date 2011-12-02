using System.Windows.Forms;

namespace Wammer.Station
{
    public partial class TryInstallDropboxAgainForm : Form
    {
        public TryInstallDropboxAgainForm()
        {
            InitializeComponent();
        }

        private void buttonSkip_Click(object sender, System.EventArgs e)
        {
            Hide();

            SetupCompletedForm _form = new SetupCompletedForm();
            _form.ShowDialog();
        }

        private void buttonRetry_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Retry;
            Close();
        }
    }
}
