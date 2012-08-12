#region

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using Waveface.API.V2;

#endregion

namespace Waveface
{
    public partial class NewPostManagerUI : Form
    {

        public NewPostManagerUI()
        {
            InitializeComponent();

            (new TabOrderManager(this)).SetTabOrder(TabOrderManager.TabScheme.AcrossFirst);

            FillListview();
        }

        private void FillListview()
        {
        }

        private void listViewFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
            }
        }
    }
}