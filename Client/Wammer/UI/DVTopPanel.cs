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
        private TextureBrush m_brushMD;
        private Component.ImageButton btnClose;
        private Component.ImageButton btnMax;
        private Component.ImageButton btnMin;

        private Bitmap m_bmpOffscreen;

        public DVTopPanel()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            //SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            m_brush4 = new TextureBrush(Resources.titlebar_4, WrapMode.Tile);
            m_brushMD = new TextureBrush(Resources.MD, WrapMode.Tile);
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

            SetWinButtonProperties();
        }

        private void SetWinButtonProperties()
        {
            int _left = Width - btnClose.Width - 1;
            btnClose.Left = _left;

            _left -= btnMax.Width;
            btnMax.Left = _left;

            _left -= btnMin.Width;
            btnMin.Left = _left;

            /*
            if ((Main.Current != null) && (Main.Current.BorderlessFormTheme != null) && (Main.Current.BorderlessFormTheme.HostWindow != null))
            {
                if (Main.Current.BorderlessFormTheme.HostWindow.WinMaxed)
                {
                    btnMax.Image = Resources.MMC_Max;
                    btnMax.ImageDisable = Resources.MMC_Max;
                    btnMax.ImageHover = Resources.MMC_Max_hl;
                }
                else
                {
                    btnMax.Image = Resources.MMC_Restore;
                    btnMax.ImageDisable = Resources.MMC_Restore;
                    btnMax.ImageHover = Resources.MMC_Restore_hl;
                }
            }
            */
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

                _g.DrawImage(Resources.titlebar_3, -1, 0, Resources.titlebar_3.Width, Height);

                _g.FillRectangle(m_brush4, Resources.titlebar_3.Width - 2, 0, Width, Resources.titlebar_4.Height);

                _g.DrawImage(Resources.LD, 2, Height - 3, Resources.LD.Width, 1);
                _g.DrawImage(Resources.RD, Width - Resources.RD.Width - 128, Height - 3, Resources.RD.Width, 1);
                _g.FillRectangle(m_brushMD, Resources.LD.Width, Height - 3, Width - Resources.LD.Width - Resources.RD.Width - 128, 1);

                e.Graphics.DrawImage(m_bmpOffscreen, 0, 0);
            }

            base.OnPaint(e);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.btnMin = new Waveface.Component.ImageButton();
            this.btnMax = new Waveface.Component.ImageButton();
            this.btnClose = new Waveface.Component.ImageButton();
            this.SuspendLayout();
            // 
            // btnMin
            // 
            this.btnMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMin.CenterAlignImage = false;
            this.btnMin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMin.Image = global::Waveface.Properties.Resources.MMC_Min;
            this.btnMin.ImageDisable = global::Waveface.Properties.Resources.MMC_Min;
            this.btnMin.ImageHover = global::Waveface.Properties.Resources.MMC_Min_hl;
            this.btnMin.Location = new System.Drawing.Point(323, 0);
            this.btnMin.Name = "btnMin";
            this.btnMin.Size = new System.Drawing.Size(29, 16);
            this.btnMin.TabIndex = 2;
            this.btnMin.Visible = false;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            // 
            // btnMax
            // 
            this.btnMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMax.CenterAlignImage = false;
            this.btnMax.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMax.Image = global::Waveface.Properties.Resources.MMC_Restore;
            this.btnMax.ImageDisable = global::Waveface.Properties.Resources.MMC_Restore;
            this.btnMax.ImageHover = global::Waveface.Properties.Resources.MMC_Restore_hl;
            this.btnMax.Location = new System.Drawing.Point(351, 0);
            this.btnMax.Name = "btnMax";
            this.btnMax.Size = new System.Drawing.Size(29, 16);
            this.btnMax.TabIndex = 1;
            this.btnMax.Visible = false;
            this.btnMax.Click += new System.EventHandler(this.btnMax_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.CenterAlignImage = false;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Image = global::Waveface.Properties.Resources.MMC_Close;
            this.btnClose.ImageDisable = global::Waveface.Properties.Resources.MMC_Close;
            this.btnClose.ImageHover = global::Waveface.Properties.Resources.MMC_Close_hl;
            this.btnClose.Location = new System.Drawing.Point(380, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(28, 16);
            this.btnClose.TabIndex = 0;
            this.btnClose.Visible = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // DVTopPanel
            // 
            this.Controls.Add(this.btnMin);
            this.Controls.Add(this.btnMax);
            this.Controls.Add(this.btnClose);
            this.Name = "DVTopPanel";
            this.Size = new System.Drawing.Size(407, 85);
            this.DoubleClick += new System.EventHandler(this.DVTopPanel_DoubleClick);
            this.ResumeLayout(false);

        }

        #endregion

        private void btnMin_Click(object sender, EventArgs e)
        {
            //Main.Current.BorderlessFormTheme.DoMin();
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            DoMax();
        }

        private void DoMax()
        {
            //Main.Current.BorderlessFormTheme.DoMax();

            SetWinButtonProperties();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //Main.Current.BorderlessFormTheme.DoClose();
        }

        private void DVTopPanel_DoubleClick(object sender, EventArgs e)
        {
            DoMax();
        }
    }
}