
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Waveface
{
    public class DVTopPanel : UserControl
    {
        public DVTopPanel()
        {
            InitializeComponent();

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnResize(System.EventArgs eventargs)
        {
            base.OnResize(eventargs);

            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Graphics _g = e.Graphics)
            {
                _g.DrawImage(Properties.Resources.Title4, -16, 0);

                using (Brush _brush = new TextureBrush(Properties.Resources.divider, WrapMode.Tile))
                {
                    _g.FillRectangle(_brush, 8, Height - 4, Width - 16, 4);
                }
            }

            base.OnPaint(e);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DVTopPanel
            // 
            this.Name = "DVTopPanel";
            this.ResumeLayout(false);

        }

        #endregion
    }
}
