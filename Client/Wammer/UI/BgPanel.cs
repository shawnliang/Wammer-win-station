
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Waveface
{
    public class BgPanel : UserControl
    {
        private TextureBrush m_backgroundBrush;
        private Font m_fontName;

        private Rectangle m_rect;
        private ContextMenuStrip contextMenuStrip;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem miAccountStatus;
        private ToolStripMenuItem miLogout;
        private int m_nameLeft;
        private bool m_inRect;

        public string UserName { get; set; }

        public BgPanel()
        {
            InitializeComponent();

            m_rect = new Rectangle();

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            UserName = string.Empty;

            m_backgroundBrush = new TextureBrush(Properties.Resources.titlebar, WrapMode.Tile);
            m_fontName = new Font("Arial", 9, FontStyle.Underline);
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
            _g.DrawImage(Properties.Resources.desktop_logo, 4, -1);

            Size _sizeNameText = TextRenderer.MeasureText(_g, UserName, m_fontName);

            m_nameLeft = Width - _sizeNameText.Width - 30;

            m_rect.X = m_nameLeft;
            m_rect.Y = 0;
            m_rect.Width = Width - m_nameLeft - 12;
            m_rect.Height = Height;

            _g.DrawString(UserName, m_fontName, m_inRect ? Brushes.LightGray : Brushes.White, m_nameLeft, 4);

            int _dx = m_nameLeft + _sizeNameText.Width + 6;
            int _dy = 9;

            Point[] _pts = { new Point(_dx, _dy), new Point(_dx + 8, _dy), new Point(_dx + 4, _dy + 4) };
            _g.FillPolygon(m_inRect ? Brushes.LightGray : Brushes.White, _pts);

            _g.Dispose();
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BgPanel));
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miAccountStatus = new System.Windows.Forms.ToolStripMenuItem();
            this.miLogout = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAccountStatus,
            this.miLogout});
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            // 
            // miAccountStatus
            // 
            this.miAccountStatus.Name = "miAccountStatus";
            resources.ApplyResources(this.miAccountStatus, "miAccountStatus");
            this.miAccountStatus.Click += new System.EventHandler(this.miAccountStatus_Click);
            // 
            // miLogout
            // 
            this.miLogout.Name = "miLogout";
            resources.ApplyResources(this.miLogout, "miLogout");
            this.miLogout.Click += new System.EventHandler(this.miLogout_Click);
            // 
            // BgPanel
            // 
            this.Name = "BgPanel";
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.BgPanel_MouseClick);
            this.MouseLeave += new System.EventHandler(this.BgPanel_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BgPanel_MouseMove);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private void BgPanel_MouseMove(object sender, MouseEventArgs e)
        {
            bool _tempBool = m_inRect;

            if (m_rect.Contains(e.X, e.Y))
            {
                Cursor = Cursors.Hand;

                m_inRect = true;
            }
            else
            {
                Cursor = Cursors.Default;

                m_inRect = false;
            }

            if (m_inRect != _tempBool)
                Invalidate();
        }

        private void BgPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_rect.Contains(e.X, e.Y))
            {
                Point _pt = new Point(m_rect.Left, m_rect.Height);
                _pt = PointToScreen(_pt);

                contextMenuStrip.Show(_pt);
            }
        }

        private void BgPanel_MouseLeave(object sender, System.EventArgs e)
        {
            m_inRect = false;

            Invalidate();
        }

        private void miAccountStatus_Click(object sender, System.EventArgs e)
        {
            Main.Current.Setting();
        }

        private void miLogout_Click(object sender, System.EventArgs e)
        {
            Main.Current.Logout();
        }
    }
}
