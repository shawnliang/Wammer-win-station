using System.Globalization;
using System.Windows.Forms;
using Waveface.Localization;

namespace Wammer.Station
{
    public partial class RemoveStationForm : Form
    {
        public RemoveStationForm(string computerName)
        {
            InitializeComponent();

            labelName.Text = computerName;
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

        private void RemoveStationForm_DoubleClick(object sender, System.EventArgs e)
        {
            if (CultureManager.ApplicationUICulture.Name == "en-US")
            {
                CultureManager.ApplicationUICulture = new CultureInfo("zh-TW");
                return;
            }

            if (CultureManager.ApplicationUICulture.Name == "zh-TW")
            {
                CultureManager.ApplicationUICulture = new CultureInfo("en-US");
                return;
            }
        }
    }
}
