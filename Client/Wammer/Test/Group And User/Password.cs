
using System.Windows.Forms;

namespace Waveface
{
    public partial class Password : Form
    {
        public string OldPassword
        {
            get { return textBoxOldPW.Text; }
            set { textBoxOldPW.Text = value; }
        }

        public string NewPassword
        {
            get { return textBoxNewPW.Text; }
            set { textBoxNewPW.Text = value; }
        }

        public Password()
        {
            InitializeComponent();

            (new TabOrderManager(this)).SetTabOrder(TabOrderManager.TabScheme.AcrossFirst);
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            if((textBoxOldPW.Text == "") || (textBoxNewPW.Text == ""))
            {
                MessageBox.Show("Please fill in missing fields.");
                return;
            }

            DialogResult = DialogResult.Yes;
            Close();
        }
    }
}
