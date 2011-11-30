using System.Windows.Forms;

namespace Wammer.Station
{
    public partial class RemoveStationForm : Form
    {
        public RemoveStationForm(string name, string location)
        {
            InitializeComponent();

            labelName.Text = name;
            labelLocation.Text = location;
        }

        private void buttonYes_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void buttonNo_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
