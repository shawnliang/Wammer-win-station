#region

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using Waveface.Properties;

#endregion

namespace Waveface
{
    public class DVTopPanel : UserControl
    {
        private TextureBrush m_brush4;
        private TextureBrush m_brushDivider;

        private Bitmap m_bmpOffscreen;

        public DVTopPanel()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            //SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            m_brush4 = new TextureBrush(Resources.titlebar_4, WrapMode.Tile);
            m_brushDivider = new TextureBrush(Resources.divider, WrapMode.Tile);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            if ((Width != 0) && (Height != 0))
            {
                m_bmpOffscreen = null;
                m_bmpOffscreen = new Bitmap(Width, Height);
            }

            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_bmpOffscreen == null)
                m_bmpOffscreen = new Bitmap(Width, Height);

            using (Graphics _g = Graphics.FromImage(m_bmpOffscreen))
            {
                _g.TextRenderingHint = TextRenderingHint.AntiAlias;
                _g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                _g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                _g.SmoothingMode = SmoothingMode.HighQuality;

                // _g.FillRectangle(m_brushDivider, 0, Height - 3, Width, 3);

                _g.DrawImage(Resources.titlebar_3, -16, 0, Resources.titlebar_3.Width, Height);

                _g.FillRectangle(m_brush4, Resources.titlebar_3.Width - 17, 0, Width - 16, Resources.titlebar_4.Height);

                // _g.FillRectangle(m_brushDivider, 0, Height - 4, Width, 4);

                e.Graphics.DrawImage(m_bmpOffscreen, 0, 0);
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