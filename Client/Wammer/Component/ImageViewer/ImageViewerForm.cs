#region

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.Windows.Forms
{
    public partial class ImageViewerForm : Form
    {
        public string ImagePath
        {
            set
            {
                imageBox.Image = new Bitmap(value);
            }
        }

        public ImageViewerForm()
        {
            InitializeComponent();
        }

        private void imageBox_Scroll(object sender, ScrollEventArgs e)
        {
            positionToolStripStatusLabel.Text = imageBox.AutoScrollPosition.ToString();
        }

        private void addToPostToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }
    }
}