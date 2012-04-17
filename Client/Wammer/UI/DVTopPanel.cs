
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
            base.OnPaint(e);

            Graphics _g = e.Graphics;

            _g.DrawImage(Properties.Resources.Title4, -16, 0);

            using (Brush _brush = new TextureBrush(Properties.Resources.divider, WrapMode.Tile))
            {
                _g.FillRectangle(_brush, 12, Height - 4, Width - 40, 4);
            }

            _g.Dispose();
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
