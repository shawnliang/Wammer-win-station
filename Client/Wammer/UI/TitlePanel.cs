
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
using Waveface.Component;

namespace Waveface
{
	public class TitlePanel : UserControl
	{
		#region Var
		int _refreshImageIndex;
		private TextureBrush _backgroundBrush;
		private ImageButton _btnRefresh;
		public ImageButton _btnAccount;
		private ImageButton _btnSetting;
		private ImageButton _btnRemovePost;
		private ToolTip _toolTip;
		private Timer _timer1;
		private System.ComponentModel.IContainer components;
		private Bitmap _bmpOffscreen;
		#endregion



		#region Private Property
		/// <summary>
		/// Gets or sets the m_ BMP offscreen.
		/// </summary>
		/// <value>The m_ BMP offscreen.</value>
		private Bitmap m_BmpOffscreen
		{
			get
			{
				if (_bmpOffscreen == null)
				{
					_bmpOffscreen = new Bitmap(ClientSize.Width, ClientSize.Height);
					using (Graphics g = Graphics.FromImage(m_BmpOffscreen))
					{
						g.TextRenderingHint = TextRenderingHint.AntiAlias;
						g.InterpolationMode = InterpolationMode.HighQualityBilinear;
						g.PixelOffsetMode = PixelOffsetMode.HighQuality;
						g.SmoothingMode = SmoothingMode.HighQuality;
						g.FillRectangle(m_BackgroundBrush, 0, 0, Width, Height);
						g.DrawImage(Properties.Resources.desktop_logo, 4, -2);
					}
				}
				return _bmpOffscreen;
			}
			set
			{
				if (_bmpOffscreen == value)
					return;

				if (_bmpOffscreen != null)
				{
					_bmpOffscreen.Dispose();
					_bmpOffscreen = null;
				}

				_bmpOffscreen = value;
			}
		}

		/// <summary>
		/// Gets the m_ background brush.
		/// </summary>
		/// <value>The m_ background brush.</value>
		private Brush m_BackgroundBrush
		{
			get
			{
				return _backgroundBrush ?? (_backgroundBrush = new TextureBrush(Properties.Resources.titlebar_1, WrapMode.Tile));
			}
		}
		#endregion



		#region Event Process
		public event EventHandler AccountInfoClosed;
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="TitlePanel"/> class.
		/// </summary>
		public TitlePanel()
		{
			InitializeComponent();

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);
		}
		#endregion




		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TitlePanel));
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this._btnRemovePost = new Waveface.Component.ImageButton();
			this._btnSetting = new Waveface.Component.ImageButton();
			this._btnAccount = new Waveface.Component.ImageButton();
			this._btnRefresh = new Waveface.Component.ImageButton();
			this._timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
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
			this.Controls.Add(this._btnRefresh);
			resources.ApplyResources(this, "$this");
			this.Name = "TitlePanel";
			this.Load += new System.EventHandler(this.TitlePanel_Load);
			this.ResumeLayout(false);

		}

		#endregion



		#region Protected Method
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.DrawImage(m_BmpOffscreen, 0, 0);
		}

		protected void OnAccountInfoClosed(EventArgs e)
		{
			if (AccountInfoClosed == null)
				return;
			AccountInfoClosed(this, e);
		}

		protected override void Dispose(bool disposing)
		{
			if (IsDisposed)
				return;

			m_BmpOffscreen = null;

			if (_backgroundBrush != null)
			{
				_backgroundBrush.Dispose();
				_backgroundBrush = null;
			}
			base.Dispose(disposing);
		}
		#endregion


		#region Public Method
		public void showRefreshUI(bool flag)
		{
			_btnRefresh.Visible = flag;

			_btnRemovePost.Visible = true;
			_btnAccount.Visible = true;
			_btnSetting.Visible = true;
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
		#endregion



		#region Public Method
		public void StartRefreshing()
		{
			_timer1.Start();
		}

		public void StopRefreshing()
		{
			_timer1.Stop();
			_btnRefresh.Image = Resources.FBT_refresh;
			_btnRefresh.ImageHover = Resources.refresh_hl;
		}
		#endregion


		#region Event Process
		private void TitlePanel_Load(object sender, EventArgs e)
		{
			_toolTip.SetToolTip(this._btnRefresh, Resources.REFRESH_TOOL_TIP);
			_toolTip.SetToolTip(this._btnRemovePost, Resources.REMOVE_TOOL_TIP);
			_toolTip.SetToolTip(this._btnAccount, Resources.ACCOUNT_TOOL_TIP);
			_toolTip.SetToolTip(this._btnSetting, Resources.SETTING_TOOL_TIP);
			this.SizeChanged += new EventHandler(TitlePanel_SizeChanged);
		}


		private void btnAccount_Click(object sender, System.EventArgs e)
		{
			using (var _form = new AccountInfoForm())
			{
				_form.StartPosition = FormStartPosition.CenterParent;
				_form.ShowDialog(this);
				OnAccountInfoClosed(EventArgs.Empty);
			}
		}

		private void btnRemovePost_Click(object sender, System.EventArgs e)
		{
			Main.Current.RemovePost();
		}

		private void btnSetting_Click(object sender, System.EventArgs e)
		{
			Main.Current.AccountInformation();
		}


		void TitlePanel_SizeChanged(object sender, EventArgs e)
		{
			m_BmpOffscreen = null;
		}


		private void btnRefresh_Click(object sender, System.EventArgs e)
		{
			Main.Current.ReloadAllData();
		}


		private void timer1_Tick(object sender, EventArgs e)
		{

			if (_refreshImageIndex == 0)
			{
				_btnRefresh.Image = Resources.refresh2;
				_btnRefresh.ImageHover = Resources.refresh_hl2;
			}
			else if (_refreshImageIndex == 1)
			{
				_btnRefresh.Image = Resources.refresh3;
				_btnRefresh.ImageHover = Resources.refresh_hl3;
			}
			else if (_refreshImageIndex == 2)
			{
				_btnRefresh.Image = Resources.refresh4;
				_btnRefresh.ImageHover = Resources.refresh_hl4;
			}
			else if (_refreshImageIndex == 3)
			{
				_btnRefresh.Image = Resources.FBT_refresh;
				_btnRefresh.ImageHover = Resources.refresh_hl;
			}

			_refreshImageIndex = (_refreshImageIndex + 1) % 4;
		}
		#endregion
	}
}
