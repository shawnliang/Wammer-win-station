
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Waveface
{
    public class BgPanel : Panel
    {
        private TextureBrush m_backgroundBrush;
        private Font m_font;

        public string UserName { get; set; }

        public BgPanel()
        {
            UserName = string.Empty;

            m_backgroundBrush = new TextureBrush(Properties.Resources.titlebar, WrapMode.Tile);
            m_font = new Font("Tahoma", 11, FontStyle.Bold);
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

            _g.FillRectangle(m_backgroundBrush, e.ClipRectangle);
            _g.DrawImage(Properties.Resources.desktop_logo, 8, 8);

            Size _size = TextRenderer.MeasureText(_g, UserName, m_font);
            _g.DrawString(UserName, m_font, SystemBrushes.WindowText, Width - _size.Width - 8, 8);

            _g.Dispose();
        }
    }
}
