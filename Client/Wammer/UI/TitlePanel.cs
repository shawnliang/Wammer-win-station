
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Web;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Localization;

namespace Waveface
{
    public class TitlePanel : UserControl
    {
        private TextureBrush m_brush1;
        private Component.ImageButton btnRefresh;
        private Label labelStatus;
        private Component.ImageButton btnAccount;
        private Component.ImageButton btnSetting;
        private Component.ImageButton btnRemovePost;
        private ToolTip toolTip;
        private System.ComponentModel.IContainer components;

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
            Main.Current.ReloadAllData();
        }

        public void updateRefreshUI(bool flag)
        {
            //DPI Hack
            Height = Properties.Resources.titlebar_1.Height;

            btnRefresh.Enabled = flag;
            btnRemovePost.Enabled = flag;
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
            btnRefresh.Enabled = !flag;
        }

        public void showRefreshUI(bool flag)
        {
            btnRefresh.Visible = flag;

            btnRemovePost.Visible = true;
            btnAccount.Visible = true;
            btnSetting.Visible = true;
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TitlePanel));
            this.labelStatus = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnRemovePost = new Waveface.Component.ImageButton();
            this.btnSetting = new Waveface.Component.ImageButton();
            this.btnAccount = new Waveface.Component.ImageButton();
            this.btnRefresh = new Waveface.Component.ImageButton();
            this.SuspendLayout();
            // 
            // labelStatus
            // 
            resources.ApplyResources(this.labelStatus, "labelStatus");
            this.labelStatus.Name = "labelStatus";
            // 
            // btnRemovePost
            // 
            this.btnRemovePost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
            this.btnRemovePost.CenterAlignImage = false;
            this.btnRemovePost.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.btnRemovePost, "btnRemovePost");
            this.btnRemovePost.Image = global::Waveface.Properties.Resources.FB_remove_post;
            this.btnRemovePost.ImageDisable = global::Waveface.Properties.Resources.FB_remove_post;
            this.btnRemovePost.ImageFront = null;
            this.btnRemovePost.ImageHover = global::Waveface.Properties.Resources.FB_remove_post_hl;
            this.btnRemovePost.Name = "btnRemovePost";
            this.btnRemovePost.TextShadow = true;
            this.toolTip.SetToolTip(this.btnRemovePost, resources.GetString("btnRemovePost.ToolTip"));
            this.btnRemovePost.Click += new System.EventHandler(this.btnRemovePost_Click);
            // 
            // btnSetting
            // 
            this.btnSetting.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
            this.btnSetting.CenterAlignImage = false;
            this.btnSetting.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.btnSetting, "btnSetting");
            this.btnSetting.Image = global::Waveface.Properties.Resources.FB_info;
            this.btnSetting.ImageDisable = global::Waveface.Properties.Resources.FB_info;
            this.btnSetting.ImageFront = null;
            this.btnSetting.ImageHover = global::Waveface.Properties.Resources.FB_info_hl;
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.TextShadow = true;
            this.toolTip.SetToolTip(this.btnSetting, resources.GetString("btnSetting.ToolTip"));
            this.btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
            // 
            // btnAccount
            // 
            this.btnAccount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
            this.btnAccount.CenterAlignImage = false;
            this.btnAccount.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.btnAccount, "btnAccount");
            this.btnAccount.Image = global::Waveface.Properties.Resources.FBT_account;
            this.btnAccount.ImageDisable = global::Waveface.Properties.Resources.FBT_account;
            this.btnAccount.ImageFront = null;
            this.btnAccount.ImageHover = global::Waveface.Properties.Resources.FBT_account_hl;
            this.btnAccount.Name = "btnAccount";
            this.btnAccount.TextShadow = true;
            this.toolTip.SetToolTip(this.btnAccount, resources.GetString("btnAccount.ToolTip"));
            this.btnAccount.Click += new System.EventHandler(this.btnAccount_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
            this.btnRefresh.CenterAlignImage = false;
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.btnRefresh, "btnRefresh");
            this.btnRefresh.Image = global::Waveface.Properties.Resources.FBT_refresh;
            this.btnRefresh.ImageDisable = global::Waveface.Properties.Resources.FBT_refresh;
            this.btnRefresh.ImageFront = null;
            this.btnRefresh.ImageHover = global::Waveface.Properties.Resources.FBT_refresh_hl;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.TextShadow = true;
            this.toolTip.SetToolTip(this.btnRefresh, resources.GetString("btnRefresh.ToolTip"));
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // TitlePanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnRemovePost);
            this.Controls.Add(this.btnSetting);
            this.Controls.Add(this.btnAccount);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.btnRefresh);
            resources.ApplyResources(this, "$this");
            this.Name = "TitlePanel";
            this.Load += new System.EventHandler(this.TitlePanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void btnAccount_Click(object sender, System.EventArgs e)
        {
            AccountInfoForm _form = new AccountInfoForm();
			_form.StartPosition = FormStartPosition.CenterParent;
			_form.ShowDialog(this);
        }

        private void btnRemovePost_Click(object sender, System.EventArgs e)
        {
            Main.Current.RemovePost();
        }

        private void btnSetting_Click(object sender, System.EventArgs e)
        {
            Main.Current.AccountInformation();
        }
    }
}
