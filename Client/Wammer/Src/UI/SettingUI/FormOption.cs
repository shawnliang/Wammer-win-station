
#region

using System;
using System.Windows.Forms;

#endregion

namespace Waveface.SettingUI
{
    public partial class FormOption : Form
    {
        public FormOption()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            firefoxDialog.ImageList = imageList;

            firefoxDialog.AddPage("General", new PageMain());
            firefoxDialog.AddPage("Server", new PageEmail());

            firefoxDialog.Init();
        }
    }
}