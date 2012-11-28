using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
    public partial class LogMessageComponent : UserControl, IDebugComponent
    {
        public LogMessageComponent()
        {
            InitializeComponent();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            logMessageListBox1.Items.Clear();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedItem = logMessageListBox1.SelectedItem;

            if (selectedItem == null)
                return;

            Clipboard.SetText(selectedItem.ToString());
        }
    }
}
