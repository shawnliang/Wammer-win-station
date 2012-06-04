#region

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Waveface.Properties;

#endregion

namespace Waveface
{
    public class DVTopPanel : UserControl
    {
        private TextureBrush m_brush4;
        private TextureBrush m_brushMD;

        public DVTopPanel()
        {
            InitializeComponent();

            m_brush4 = new TextureBrush(Resources.titlebar_4, WrapMode.Tile);
            m_brushMD = new TextureBrush(Resources.MD, WrapMode.Tile);

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics _g = e.Graphics;

            _g.DrawImage(Resources.titlebar_3, -1, 0, Resources.titlebar_3.Width, Height);

            _g.FillRectangle(m_brush4, Resources.titlebar_3.Width - 2, 0, Width, Resources.titlebar_4.Height);

            _g.DrawImage(Resources.LD, 2, Height - 3, Resources.LD.Width, 1);
            _g.DrawImage(Resources.RD, Width - Resources.RD.Width - 128, Height - 3, Resources.RD.Width, 1);
            _g.FillRectangle(m_brushMD, Resources.LD.Width, Height - 3, Width - Resources.LD.Width - Resources.RD.Width - 128, 1);

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
            this.Size = new System.Drawing.Size(407, 85);
            this.ResumeLayout(false);

        }

        #endregion
    }
}