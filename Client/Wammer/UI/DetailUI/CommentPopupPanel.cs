#region

using System.Drawing;
using System.Windows.Forms;
using Waveface.Component.PopupControl;
using Waveface.Component;
using Waveface.Component.RichEdit;

#endregion

namespace Waveface
{
    public partial class CommentPopupPanel : UserControl
    {
        public WaterMarkRichTextBox CommentTextBox
        {
            get { return textBoxComment; }
            set { textBoxComment = value; }
        }

        public CommentPopupPanel()
        {
            InitializeComponent();

            MinimumSize = Size;

            textBoxComment.WaterMarkText = I18n.L.T("PostForm.PuretextWaterMark");
        }

        protected override void WndProc(ref Message m)
        {
            if ((Parent as Popup).ProcessResizing(ref m))
            {
                return;
            }

            base.WndProc(ref m);
        }

        private void textBoxComment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (textBoxComment.CanPaste(DataFormats.GetFormat(DataFormats.Bitmap)))
                {
                    e.SuppressKeyPress = true;
                }
            }
        }
    }
}