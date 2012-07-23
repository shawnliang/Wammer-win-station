
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Web;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Localization;
using System;
using Waveface.Properties;

namespace Waveface
{
    public class TitlePanel : UserControl
	{
		#region Var
		private TextureBrush _brush1;
		private Component.ImageButton _btnRefresh;
		private Label _labelStatus;
		public Component.ImageButton _btnAccount;
		private Component.ImageButton _btnSetting;
		private Component.ImageButton _btnRemovePost;
		private ToolTip _toolTip;
		private System.ComponentModel.IContainer _components;
		private Timer _timer1;
		private System.ComponentModel.IContainer components;

		private Bitmap _bmpOffscreen;
		#endregion



		#region Event Process
		public event EventHandler AccountInfoClosed; 
		#endregion

        public TitlePanel()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            // SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            _brush1 = new TextureBrush(Properties.Resources.titlebar_1, WrapMode.Tile);

			//_btnRefresh.Paint += new PaintEventHandler(_btnRefresh_Paint);
        }

		//void _btnRefresh_Paint(object sender, PaintEventArgs e)
		//{
		//    OnPaint(e);
		//}


		//#region Private Method
		//private static Image RotateImage(Image img, float rotationAngle)
		//{
		//    //create an empty Bitmap image
		//    Bitmap bmp = new Bitmap(img.Width, img.Height);

		//    //turn the Bitmap into a Graphics object
		//    Graphics gfx = Graphics.FromImage(bmp);

		//    //now we set the rotation point to the center of our image
		//    gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

		//    //now rotate the image
		//    gfx.RotateTransform(rotationAngle);

		//    gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

		//    //set the InterpolationMode to HighQualityBicubic so to ensure a high
		//    //quality image once it is transformed to the specified size
		//    gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

		//    //now draw our new image onto the graphics object
		//    gfx.DrawImage(img, new Point(0, 0));

		//    //dispose of our Graphics object
		//    gfx.Dispose();

		//    //return the image
		//    return bmp;
		//}
		//#endregion

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
                if (_bmpOffscreen == null)
                    _bmpOffscreen = new Bitmap(ClientSize.Width, ClientSize.Height);

                using (Graphics _g = Graphics.FromImage(_bmpOffscreen))
                {
                    _g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    _g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    _g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    _g.SmoothingMode = SmoothingMode.HighQuality;

                    _g.FillRectangle(_brush1, 0, 0, Width, Height);
                    //_g.DrawImage(Properties.Resources.titlebar_3, Width - 1, 0);

                    _g.DrawImage(Properties.Resources.desktop_logo, 4, -2);

                    e.Graphics.DrawImage(_bmpOffscreen, 0, 0);
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

            _btnRefresh.Enabled = flag;
            _btnRemovePost.Enabled = flag;
            _btnAccount.Enabled = flag;
            _btnSetting.Enabled = flag;
        }

        public void ShowStatusText(string msg)
        {
            _labelStatus.Text = msg;

            show_labelStatus(msg != "");
        }

        private void show_labelStatus(bool flag)
        {
            _btnRefresh.Enabled = !flag;
        }

        public void showRefreshUI(bool flag)
        {
            _btnRefresh.Visible = flag;

            _btnRemovePost.Visible = true;
            _btnAccount.Visible = true;
            _btnSetting.Visible = true;
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TitlePanel));
			this._labelStatus = new System.Windows.Forms.Label();
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this._btnRemovePost = new Waveface.Component.ImageButton();
			this._btnSetting = new Waveface.Component.ImageButton();
			this._btnAccount = new Waveface.Component.ImageButton();
			this._btnRefresh = new Waveface.Component.ImageButton();
			this._timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// _labelStatus
			// 
			resources.ApplyResources(this._labelStatus, "_labelStatus");
			this._labelStatus.Name = "_labelStatus";
			// 
			// _btnRemovePost
			// 
			this._btnRemovePost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
			this._btnRemovePost.CenterAlignImage = true;
			this._btnRemovePost.Cursor = System.Windows.Forms.Cursors.Hand;
			resources.ApplyResources(this._btnRemovePost, "_btnRemovePost");
			this._btnRemovePost.Image = global::Waveface.Properties.Resources.FB_remove_post;
			this._btnRemovePost.ImageDisable = global::Waveface.Properties.Resources.FB_remove_post;
			this._btnRemovePost.ImageFront = null;
			this._btnRemovePost.ImageHover = global::Waveface.Properties.Resources.FB_remove_post_hl;
			this._btnRemovePost.Name = "_btnRemovePost";
			this._btnRemovePost.TextShadow = true;
			this._toolTip.SetToolTip(this._btnRemovePost, resources.GetString("_btnRemovePost.ToolTip"));
			this._btnRemovePost.Click += new System.EventHandler(this.btnRemovePost_Click);
			// 
			// _btnSetting
			// 
			this._btnSetting.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
			this._btnSetting.CenterAlignImage = true;
			this._btnSetting.Cursor = System.Windows.Forms.Cursors.Hand;
			resources.ApplyResources(this._btnSetting, "_btnSetting");
			this._btnSetting.Image = global::Waveface.Properties.Resources.FBT_setting;
			this._btnSetting.ImageDisable = global::Waveface.Properties.Resources.FBT_setting;
			this._btnSetting.ImageFront = null;
			this._btnSetting.ImageHover = global::Waveface.Properties.Resources.FBT_setting_hl;
			this._btnSetting.Name = "_btnSetting";
			this._btnSetting.TextShadow = true;
			this._toolTip.SetToolTip(this._btnSetting, resources.GetString("_btnSetting.ToolTip"));
			this._btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
			// 
			// _btnAccount
			// 
			this._btnAccount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
			this._btnAccount.CenterAlignImage = true;
			this._btnAccount.Cursor = System.Windows.Forms.Cursors.Hand;
			resources.ApplyResources(this._btnAccount, "_btnAccount");
			this._btnAccount.Image = global::Waveface.Properties.Resources.FBT_account;
			this._btnAccount.ImageDisable = global::Waveface.Properties.Resources.FBT_account;
			this._btnAccount.ImageFront = null;
			this._btnAccount.ImageHover = global::Waveface.Properties.Resources.FBT_account_hl;
			this._btnAccount.Name = "_btnAccount";
			this._btnAccount.TextShadow = true;
			this._toolTip.SetToolTip(this._btnAccount, resources.GetString("_btnAccount.ToolTip"));
			this._btnAccount.Click += new System.EventHandler(this.btnAccount_Click);
			// 
			// _btnRefresh
			// 
			this._btnRefresh.CenterAlignImage = true;
			this._btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
			resources.ApplyResources(this._btnRefresh, "_btnRefresh");
			this._btnRefresh.Image = global::Waveface.Properties.Resources.FBT_refresh;
			this._btnRefresh.ImageDisable = global::Waveface.Properties.Resources.FBT_refresh;
			this._btnRefresh.ImageFront = null;
			this._btnRefresh.ImageHover = global::Waveface.Properties.Resources.FBT_refresh_hl;
			this._btnRefresh.Name = "_btnRefresh";
			this._btnRefresh.TextShadow = true;
			this._toolTip.SetToolTip(this._btnRefresh, resources.GetString("_btnRefresh.ToolTip"));
			this._btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
			// 
			// _timer1
			// 
			this._timer1.Interval = 500;
			this._timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// TitlePanel
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
			this.Controls.Add(this._btnRemovePost);
			this.Controls.Add(this._btnSetting);
			this.Controls.Add(this._btnAccount);
			this.Controls.Add(this._labelStatus);
			this.Controls.Add(this._btnRefresh);
			resources.ApplyResources(this, "$this");
			this.Name = "TitlePanel";
			this.Load += new System.EventHandler(this.TitlePanel_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		#region Protected Method
		protected void OnAccountInfoClosed(EventArgs e)
		{
			if (AccountInfoClosed == null)
				return;
			AccountInfoClosed(this, e);
		}
		#endregion


		private void btnAccount_Click(object sender, System.EventArgs e)
        {
            AccountInfoForm _form = new AccountInfoForm();
			_form.StartPosition = FormStartPosition.CenterParent;
			_form.ShowDialog(this);
			OnAccountInfoClosed(EventArgs.Empty);
        }

        private void btnRemovePost_Click(object sender, System.EventArgs e)
        {
            Main.Current.RemovePost();
        }

        private void btnSetting_Click(object sender, System.EventArgs e)
        {
            Main.Current.AccountInformation();
		}


		#region Public Method
		public void StartRefreshing()
		{
			_timer1.Start();
		}

		public void StopRefreshing()
		{
			_timer1.Stop();
			_btnRefresh.Image = Resources.FBT_refresh;
		}
		#endregion

		int refreshImageIndex = 0;
		private void timer1_Tick(object sender, EventArgs e)
		{

			if (refreshImageIndex == 0)
			{
				_btnRefresh.Image = Resources.refresh2;
				_btnRefresh.ImageHover = Resources.refresh_hl2;
			}
			else if (refreshImageIndex == 1)
			{
				_btnRefresh.Image = Resources.refresh3;
				_btnRefresh.ImageHover = Resources.refresh_hl3;
			}
			else if (refreshImageIndex == 2)
			{
				_btnRefresh.Image = Resources.refresh4;
				_btnRefresh.ImageHover = Resources.refresh_hl4;
			}
			else if (refreshImageIndex == 3)
			{
				_btnRefresh.Image = Resources.FBT_refresh;
				_btnRefresh.ImageHover = Resources.refresh_hl;
			}

			refreshImageIndex = (refreshImageIndex + 1) % 4;
		}
	}
}
