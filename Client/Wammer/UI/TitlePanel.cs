
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Waveface
{
    public class TitlePanel : UserControl
    {
        private TextureBrush m_brush1;
        private Component.ImageButton btnCreatePost;
        private Component.ImageButton btnRefresh;
        private Label labelStatus;
        private TextureBrush m_brush3;

        private Bitmap m_bmpOffscreen;

        public TitlePanel()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            m_brush1 = new TextureBrush(Properties.Resources.Title1, WrapMode.Tile);
            m_brush3 = new TextureBrush(Properties.Resources.Title3, WrapMode.Tile);

            ArrangeButtons();

            show_labelStatus(false);
        }

        private void ArrangeButtons()
        {
            btnCreatePost.Top = 6;
            btnCreatePost.Left = Main.Current.GetLeftAreaWidth() + 24;

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
            if (m_bmpOffscreen == null)
                m_bmpOffscreen = new Bitmap(ClientSize.Width, ClientSize.Height);

            using (Graphics _g = Graphics.FromImage(m_bmpOffscreen))
            {
                _g.FillRectangle(m_brush1, 0, 0, Width - 8, Height);

                int _leftAreaWidth = Main.Current.GetLeftAreaWidth();

                _g.DrawImage(Properties.Resources.Title2, _leftAreaWidth, 0);

                int _offset = _leftAreaWidth + Properties.Resources.Title2.Width;

                int _offsetEnd = 16;

                _g.FillRectangle(m_brush3, _offset, 0, Width - _offset - _offsetEnd, Height);

                _g.DrawImage(Properties.Resources.Title4, Width - _offsetEnd, 0);

                e.Graphics.DrawImage(m_bmpOffscreen, 0, 0);
            }

            base.OnPaint(e);
        }

        private void btnRefresh_Click(object sender, System.EventArgs e)
        {
            //Test - ToDo
            Main.Current.checkNewPosts();

            //if (!Main.Current.CheckNetworkStatus())
            //    return;

            Main.Current.ReloadAllData();
        }

        public void updateRefreshUI(bool flag)
        {
            btnRefresh.Enabled = flag;
        }

        public void ShowStatusText(string msg)
        {
            labelStatus.Text = msg;

            show_labelStatus(msg != "");
        }

        private void show_labelStatus(bool flag)
        {
            btnCreatePost.Enabled = !flag;

            btnRefresh.Enabled = !flag;
        }

        public void showRefreshUI(bool flag)
        {
            btnCreatePost.Visible = true;

            btnRefresh.Visible = flag;
        }

        private void btnCreatePost_Click(object sender, System.EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            Main.Current.Post();
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.btnCreatePost = new Waveface.Component.ImageButton();
            this.btnRefresh = new Waveface.Component.ImageButton();
            this.labelStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCreatePost
            // 
            this.btnCreatePost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
            this.btnCreatePost.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCreatePost.Font = new System.Drawing.Font("Arial", 9F);
            this.btnCreatePost.Image = global::Waveface.Properties.Resources.FB_newpost;
            this.btnCreatePost.ImageDisable = global::Waveface.Properties.Resources.FB_newpost_hl;
            this.btnCreatePost.ImageHover = global::Waveface.Properties.Resources.FB_newpost_hl;
            this.btnCreatePost.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCreatePost.Location = new System.Drawing.Point(225, 3);
            this.btnCreatePost.Name = "btnCreatePost";
            this.btnCreatePost.Size = new System.Drawing.Size(20, 20);
            this.btnCreatePost.TabIndex = 11;
            this.btnCreatePost.TabStop = false;
            this.btnCreatePost.Click += new System.EventHandler(this.btnCreatePost_Click);
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
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(15, 11);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(54, 12);
            this.labelStatus.TabIndex = 13;
            this.labelStatus.Text = "labelStatus";
            this.labelStatus.Visible = false;
            // 
            // TitlePanel
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(175)))), ((int)(((byte)(198)))));
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnCreatePost);
            this.Name = "TitlePanel";
            this.Size = new System.Drawing.Size(651, 39);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
