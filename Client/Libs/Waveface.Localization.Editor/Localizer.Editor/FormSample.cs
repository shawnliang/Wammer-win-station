#region

using System;
using System.Globalization;
using System.Windows.Forms;

#endregion

namespace Waveface.Localization.Samples
{
    public partial class FormSample : Form
    {
        public string CurrentCultureName = String.Empty;
        private Localizer L;

        public FormSample()
        {
            InitializeComponent();
        }

        private void FormSample1_Load(object sender, EventArgs e)
        {
            L = new Localizer();
            L.WItemsFullPath = Application.StartupPath + "\\witems.xml";
            L.CurrentCulture = new CultureInfo(CurrentCultureName);

            label1.Text = L.T(label1.Text);
            label2.Text = L.T(label2.Text);
            btnShow.Text = L.T(btnShow.Text);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            MessageBox.Show(L.T("Form1.Message"));
        }
    }
}