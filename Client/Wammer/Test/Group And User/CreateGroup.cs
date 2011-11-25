
using System.Windows.Forms;

namespace Waveface
{
    public partial class CreateGroup : Form
    {
        public string name
        {
            get { return textBoxEName.Text; }
            set { textBoxEName.Text = value; }
        }

        public string description
        {
            get { return textBoxDescription.Text; }
            set { textBoxDescription.Text = value; }
        }

        public string WindowsCaption
        {
            set { Text = value;}
        }

        public CreateGroup()
        {
            InitializeComponent();

            (new TabOrderManager(this)).SetTabOrder(TabOrderManager.TabScheme.AcrossFirst);
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            if((textBoxEName.Text == "") || (textBoxDescription.Text == ""))
            {
                MessageBox.Show("Please fill in missing fields.");
                return;
            }

            DialogResult = DialogResult.Yes;
            Close();
        }
    }
}
