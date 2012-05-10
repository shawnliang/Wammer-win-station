#region

using System;
using System.ComponentModel;
using System.Windows.Forms;
using Waveface.Component.RichEdit;

#endregion

namespace Waveface
{
    public partial class CommentForm : Form
    {
        public RichTextEditor CommentTextBox
        {
            get { return textBoxComment; }
            set { textBoxComment = value; }
        }

        public CommentForm()
        {
            InitializeComponent();

            textBoxComment.WaterMarkText = I18n.L.T("PostForm.PuretextWaterMark");
        }

        #region richTextBox

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(textBoxComment.SelectedText);
            textBoxComment.SelectedText = "";
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(textBoxComment.SelectedText);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataFormats.Format _format = DataFormats.GetFormat(DataFormats.Text);

            if (textBoxComment.CanPaste(_format))
            {
                textBoxComment.Paste(_format);
                textBoxComment.Refresh();
            }
        }

        private void contextMenuStripEdit_Opening(object sender, CancelEventArgs e)
        {
            pasteToolStripMenuItem.Enabled = Clipboard.ContainsData(DataFormats.Text);
        }

        #endregion

        private void buttonAddComment_Click(object sender, EventArgs e)
        {
            if (textBoxComment.Text.Trim().Equals(string.Empty))
            {
                MessageBox.Show(I18n.L.T("DetailView.CommentEmpty"), "Waveface Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;

            Close();
        }

        private void textBoxComment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (textBoxComment.CanPaste(DataFormats.GetFormat(DataFormats.Bitmap)))
                {
                    e.SuppressKeyPress = true;
                }
                else if (textBoxComment.CanPaste(DataFormats.GetFormat(DataFormats.Text)))
                {
                    textBoxComment.Paste(DataFormats.GetFormat(DataFormats.Text));
                    e.SuppressKeyPress = true;
                }
            }

            textBoxComment.Refresh();
        }
    }
}