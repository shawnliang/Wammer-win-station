using System;
using System.Windows.Forms;

namespace Waveface.SettingUI
{
    public partial class PreferenceForm : Form
    {
        public PreferenceForm()
        {
            InitializeComponent();
        }

        private void PreferenceForm_Load(object sender, EventArgs e)
        {

        }

        private void ckbAllowDropbox_CheckedChanged(object sender, EventArgs e)
        {
            panelLimitSpace.Enabled = ckbAllowDropbox.Checked;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonChangeFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog _dialog = new FolderBrowserDialog())
            {
                _dialog.Description = "Default Station Folder";
                _dialog.ShowNewFolderButton = true;
                _dialog.RootFolder = Environment.SpecialFolder.Desktop;
                
                if (_dialog.ShowDialog() == DialogResult.OK)
                {
                    string _folder = _dialog.SelectedPath;
                    labelDataPath.Text = _folder;
                }
            }
        }
    }
}
