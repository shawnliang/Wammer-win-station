
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Waveface
{
    public class TitlePanel : UserControl
    {
        private TextureBrush m_brush1;
        private Component.ImageButton btnRefresh;
        private Label labelStatus;
        private Component.ImageButton btnAccount;
        private Component.ImageButton btnSetting;
        private Component.ImageButton btnNewPost;

        private Bitmap m_bmpOffscreen;

        public TitlePanel()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            // SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            m_brush1 = new TextureBrush(Properties.Resources.titlebar_1, WrapMode.Tile);
        }

        private void TitlePanel_Load(object sender, System.EventArgs e)
        {
            show_labelStatus(false);
        }

        /*
        protected override void OnResize(System.EventArgs eventargs)
        {
            ArrangeButtons();

            Refresh();

            base.OnResize(eventargs);
        }
        */

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!DesignMode)
            {
                if (m_bmpOffscreen == null)
                    m_bmpOffscreen = new Bitmap(ClientSize.Width, ClientSize.Height);

                using (Graphics _g = Graphics.FromImage(m_bmpOffscreen))
                {
                    _g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    _g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    _g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    _g.SmoothingMode = SmoothingMode.HighQuality;

                    _g.FillRectangle(m_brush1, 0, 0, Width, Height);
                    //_g.DrawImage(Properties.Resources.titlebar_3, Width - 1, 0);

                    _g.DrawImage(Properties.Resources.desktop_logo, 4, -2);

                    e.Graphics.DrawImage(m_bmpOffscreen, 0, 0);
                }
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
            btnNewPost.Enabled = flag;
            btnAccount.Enabled = flag;
            btnSetting.Enabled = flag;
        }

        public void ShowStatusText(string msg)
        {
            labelStatus.Text = msg;

            show_labelStatus(msg != "");
        }

        private void show_labelStatus(bool flag)
        {
            btnNewPost.Enabled = !flag;

            btnRefresh.Enabled = !flag;
        }

        public void showRefreshUI(bool flag)
        {
            btnRefresh.Visible = flag;

            btnNewPost.Visible = true;
            btnAccount.Visible = true;
            btnSetting.Visible = true;
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.labelStatus = new System.Windows.Forms.Label();
            this.btnRefresh = new Waveface.Component.ImageButton();
            this.btnAccount = new Waveface.Component.ImageButton();
            this.btnSetting = new Waveface.Component.ImageButton();
            this.btnNewPost = new Waveface.Component.ImageButton();
            this.SuspendLayout();
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
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
            this.btnRefresh.CenterAlignImage = false;
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.Font = new System.Drawing.Font("Arial", 9F);
            this.btnRefresh.Image = global::Waveface.Properties.Resources.FBT_refresh;
            this.btnRefresh.ImageDisable = global::Waveface.Properties.Resources.FBT_refresh;
            this.btnRefresh.ImageHover = global::Waveface.Properties.Resources.FBT_refresh_hl;
            this.btnRefresh.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRefresh.Location = new System.Drawing.Point(290, 15);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(37, 37);
            this.btnRefresh.TabIndex = 12;
            this.btnRefresh.Visible = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnAccount
            // 
            this.btnAccount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
            this.btnAccount.CenterAlignImage = false;
            this.btnAccount.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAccount.Font = new System.Drawing.Font("Arial", 9F);
            this.btnAccount.Image = global::Waveface.Properties.Resources.FBT_account;
            this.btnAccount.ImageDisable = global::Waveface.Properties.Resources.FBT_account;
            this.btnAccount.ImageHover = global::Waveface.Properties.Resources.FBT_account_hl;
            this.btnAccount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAccount.Location = new System.Drawing.Point(350, 15);
            this.btnAccount.Name = "btnAccount";
            this.btnAccount.Size = new System.Drawing.Size(37, 37);
            this.btnAccount.TabIndex = 15;
            this.btnAccount.Visible = false;
            this.btnAccount.Click += new System.EventHandler(this.btnAccount_Click);
            // 
            // btnSetting
            // 
            this.btnSetting.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
            this.btnSetting.CenterAlignImage = false;
            this.btnSetting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSetting.Font = new System.Drawing.Font("Arial", 9F);
            this.btnSetting.Image = global::Waveface.Properties.Resources.FBT_setting;
            this.btnSetting.ImageDisable = global::Waveface.Properties.Resources.FBT_setting;
            this.btnSetting.ImageHover = global::Waveface.Properties.Resources.FBT_setting_hl;
            this.btnSetting.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSetting.Location = new System.Drawing.Point(410, 15);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(37, 37);
            this.btnSetting.TabIndex = 16;
            this.btnSetting.Visible = false;
            // 
            // btnNewPost
            // 
            this.btnNewPost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
            this.btnNewPost.CenterAlignImage = false;
            this.btnNewPost.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewPost.Font = new System.Drawing.Font("Arial", 9F);
            this.btnNewPost.Image = global::Waveface.Properties.Resources.FBT_newpost;
            this.btnNewPost.ImageDisable = global::Waveface.Properties.Resources.FBT_newpost;
            this.btnNewPost.ImageHover = global::Waveface.Properties.Resources.FBT_newpost_hl_hl;
            this.btnNewPost.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnNewPost.Location = new System.Drawing.Point(230, 15);
            this.btnNewPost.Name = "btnNewPost";
            this.btnNewPost.Size = new System.Drawing.Size(37, 37);
            this.btnNewPost.TabIndex = 17;
            this.btnNewPost.Visible = false;
            this.btnNewPost.Click += new System.EventHandler(this.btnNewPost_Click);
            // 
            // TitlePanel
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnNewPost);
            this.Controls.Add(this.btnSetting);
            this.Controls.Add(this.btnAccount);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.btnRefresh);
            this.Name = "TitlePanel";
            this.Size = new System.Drawing.Size(651, 64);
            this.Load += new System.EventHandler(this.TitlePanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void btnAccount_Click(object sender, System.EventArgs e)
        {
            Main.Current.AccountInformation();
        }

        private void btnNewPost_Click(object sender, System.EventArgs e)
        {
            Main.Current.Post();
        }
    }
}
