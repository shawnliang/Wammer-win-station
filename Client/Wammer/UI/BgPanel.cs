
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Waveface
{
    public class BgPanel : UserControl
    {
        private TextureBrush m_backgroundBrush;
        private Font m_fontName;
        private Font m_fontLink;

        private Rectangle m_logoutRect;
        private Rectangle m_preferenceRect;

        public string UserName { get; set; }

        public BgPanel()
        {
            InitializeComponent();

            UserName = string.Empty;

            m_backgroundBrush = new TextureBrush(Properties.Resources.titlebar, WrapMode.Tile);
            m_fontName = new Font("Arial", 11, FontStyle.Bold);
            m_fontLink = new Font("Arial", 9, FontStyle.Underline);
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

            Size _sizeName = TextRenderer.MeasureText(_g, UserName, m_fontName);
            _g.DrawString(UserName, m_fontName, SystemBrushes.WindowText, Width - _sizeName.Width - 8, 8);

            if (!DesignMode)
            {
                Size _logoutSize = TextRenderer.MeasureText(_g, I18n.L.T("Main.Logout"), m_fontLink) + new Size(2, 2);
                m_logoutRect = new Rectangle(Width - _logoutSize.Width - 6, Height - _logoutSize.Height - 6,
                                             _logoutSize.Width, _logoutSize.Height);

                TextRenderer.DrawText(_g, I18n.L.T("Main.Logout"), m_fontLink, m_logoutRect, Color.WhiteSmoke);


                Size _preferenceSize = TextRenderer.MeasureText(_g, I18n.L.T("Main.Preference"), m_fontLink) +
                                       new Size(2, 2);
                m_preferenceRect = new Rectangle(Width - _preferenceSize.Width - _logoutSize.Width - 8,
                                                 Height - _preferenceSize.Height - 6, _preferenceSize.Width,
                                                 _preferenceSize.Height);

                TextRenderer.DrawText(_g, I18n.L.T("Main.Preference"), m_fontLink, m_preferenceRect, Color.WhiteSmoke);
            }

            _g.Dispose();
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BgPanel
            // 
            this.Name = "BgPanel";
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.BgPanel_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BgPanel_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private void BgPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if(m_logoutRect.Contains(e.X, e.Y) || m_preferenceRect.Contains(e.X, e.Y))
            {
                Cursor = Cursors.Hand;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        private void BgPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if(m_logoutRect.Contains(e.X, e.Y))
            {
                Main.Current.Logout();
            }
            
            if(m_preferenceRect.Contains(e.X, e.Y))
            {
                Main.Current.Setting();
            }            
        }
    }
}
