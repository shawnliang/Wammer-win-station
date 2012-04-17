
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Waveface
{
    public class TitlePanel : UserControl
    {
        private TextureBrush m_brush1;
        private Component.ImageButton btnNewPost;
        private Component.ImageButton btnRefresh;
        private TextureBrush m_brush3;

        public TitlePanel()
        {
            InitializeComponent();

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            m_brush1 = new TextureBrush(Properties.Resources.Title1, WrapMode.Tile);
            m_brush3 = new TextureBrush(Properties.Resources.Title3, WrapMode.Tile);

            ArrangeButtons();
        }

        private void ArrangeButtons()
        {
            btnNewPost.Top = 6;
            btnNewPost.Left = Main.Current.GetLeftAreaWidth() + 24;

            btnRefresh.Top = 6;
            btnRefresh.Left = Width - 48;
        }

        protected override void OnResize(System.EventArgs eventargs)
        {
            base.OnResize(eventargs);

            ArrangeButtons();

            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics _g = e.Graphics;

            _g.FillRectangle(m_brush1, 0, 0, Width - 8 , Height);

            int _leftAreaWidth = Main.Current.GetLeftAreaWidth();

            _g.DrawImage(Properties.Resources.Title2, _leftAreaWidth, 0);

            int _offset = _leftAreaWidth + Properties.Resources.Title2.Width;

            int _offsetEnd = 16;

            _g.FillRectangle(m_brush3, _offset, 0, Width - _offset - _offsetEnd, Height);

            _g.DrawImage(Properties.Resources.Title4, Width - _offsetEnd, 0);

            _g.Dispose();
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.btnNewPost = new Waveface.Component.ImageButton();
            this.btnRefresh = new Waveface.Component.ImageButton();
            this.SuspendLayout();
            // 
            // btnNewPost
            // 
            this.btnNewPost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
            this.btnNewPost.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewPost.Font = new System.Drawing.Font("Arial", 9F);
            this.btnNewPost.Image = global::Waveface.Properties.Resources.FB_newpost;
            this.btnNewPost.ImageDisable = global::Waveface.Properties.Resources.FB_newpost_hl;
            this.btnNewPost.ImageHover = global::Waveface.Properties.Resources.FB_newpost_hl;
            this.btnNewPost.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnNewPost.Location = new System.Drawing.Point(225, 3);
            this.btnNewPost.Name = "btnNewPost";
            this.btnNewPost.Size = new System.Drawing.Size(20, 20);
            this.btnNewPost.TabIndex = 11;
            this.btnNewPost.TabStop = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.Font = new System.Drawing.Font("Arial", 9F);
            this.btnRefresh.Image = global::Waveface.Properties.Resources.FB_refresh;
            this.btnRefresh.ImageDisable = global::Waveface.Properties.Resources.FB_refresh_hl;
            this.btnRefresh.ImageHover = global::Waveface.Properties.Resources.FB_refresh_hl;
            this.btnRefresh.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRefresh.Location = new System.Drawing.Point(251, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(20, 20);
            this.btnRefresh.TabIndex = 12;
            this.btnRefresh.TabStop = false;
            // 
            // TitlePanel
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(175)))), ((int)(((byte)(198)))));
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnNewPost);
            this.Name = "TitlePanel";
            this.Size = new System.Drawing.Size(651, 39);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
