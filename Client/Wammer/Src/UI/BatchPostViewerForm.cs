#region

using System.Collections.Generic;
using System.Windows.Forms;

#endregion

namespace Waveface
{
    public partial class BatchPostViewerForm : Form
    {
        public BatchPostViewerForm(string text, List<string> pics)
        {
            InitializeComponent();

            foreach (string _pic in pics)
            {
                imageListView.Items.Add(_pic);
            }

            richTextBox.Text = text;
        }
    }
}