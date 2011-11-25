
using System.Windows.Forms;

namespace Waveface
{
    public partial class Signup : Form
    {
        public string email
        {
            get { return textBoxEMail.Text; }
        }

        public string Password
        {
            get { return textBoxPassword.Text; }
        }

        public string Nickname
        {
            get { return textBoxNickname.Text; }
        }

        public string Avatar
        {
            get { return textBoxAvatar.Text; }
        }

        public Signup()
        {
            InitializeComponent();

            (new TabOrderManager(this)).SetTabOrder(TabOrderManager.TabScheme.AcrossFirst);
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            if((textBoxEMail.Text == "") || (textBoxPassword.Text == "") || (textBoxNickname.Text == ""))
            {
                MessageBox.Show("Please fill in missing fields.");
                return;
            }

            DialogResult = DialogResult.Yes;
            Close();
        }
    }
}
