#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace Waveface.Component
{

    #region BorderlessForm

    public class BorderlessForm : Form
    {
        #region API

        //Windows API for resizing the window.
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, long lParam, long wParam);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

        #endregion

        private enum ResizeLocation
        {
            //Determines the position of the cursor.
            top = 0,
            left = 1,
            right = 2,
            bottom = 3,
            bottomleft = 4,
            bottomright = 5,
        }

        #region Fields

        //controls, variables, blah blah blah...
        internal Panel rightpnl;
        internal Panel nwsize;
        internal Panel swresize;
        internal Panel bottompnl;
        private ToolTip controlboxToolTip;
        private IContainer components;
        public Panel panelTitlebar;
        public Panel cmdMaxRes;
        internal Panel cmdMin;
        private Panel cmdClose;
        public Panel topPanel;
        public Label titleCaption;
        internal Panel leftpnl;
        public Panel bodyPanel;
        internal Panel bottompnl2;
        internal Panel rightpnl2;
        internal Panel leftpnl2;
        private int originalleft;
        private int originaltop;
        private bool winMaxed;
        internal Size originalSize;
        private ResizeLocation rLoc;

        //Windows API Constants (Form Resize)
        internal const int WM_NCLBUTTONDOWN = 161;
        internal const int HT_CAPTION = 0x2;
        internal const int HTBOTTOM = 15;
        internal const int HTBOTTOMLEFT = 16;
        internal const int HTBOTTOMRIGHT = 17;
        internal const int HTRIGHT = 11;
        internal const int HTLEFT = 10;
        internal const int HTTOP = 12;
        internal const int WS_MAXIMIZE = 0x01000000;
        internal const int WM_COMMAND = 0x111;
        internal const int SIZE_MAXIMIZED = 2;
        internal const int WM_SIZE = 0x0005;
        internal const int SW_MAXIMIZE = 3;
        internal const int SW_NORMAL = 1;

        #endregion

        public bool WinMaxed
        {
            get { return winMaxed; }
        }

        public override string Text
        {
            get { return base.Text; }
            set
            {
                titleCaption.Text = value;

                base.Text = value;
            }
        }

        public BorderlessForm()
        {
            InitializeComponent();

            MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;

                param.ClassStyle |= 0x20000; // dropShadow;
                param.Style |= 0x20000;//Enables the form to be minimized through the taskbar.
                return param;
            }
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BorderlessForm));
            this.rightpnl = new System.Windows.Forms.Panel();
            this.bottompnl = new System.Windows.Forms.Panel();
            this.nwsize = new System.Windows.Forms.Panel();
            this.swresize = new System.Windows.Forms.Panel();
            this.controlboxToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.cmdMaxRes = new System.Windows.Forms.Panel();
            this.cmdMin = new System.Windows.Forms.Panel();
            this.cmdClose = new System.Windows.Forms.Panel();
            this.leftpnl = new System.Windows.Forms.Panel();
            this.bodyPanel = new System.Windows.Forms.Panel();
            this.bottompnl2 = new System.Windows.Forms.Panel();
            this.rightpnl2 = new System.Windows.Forms.Panel();
            this.leftpnl2 = new System.Windows.Forms.Panel();
            this.panelTitlebar = new System.Windows.Forms.Panel();
            this.topPanel = new System.Windows.Forms.Panel();
            this.titleCaption = new System.Windows.Forms.Label();
            this.bodyPanel.SuspendLayout();
            this.panelTitlebar.SuspendLayout();
            this.SuspendLayout();
            // 
            // rightpnl
            // 
            this.rightpnl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(196)))), ((int)(((byte)(213)))));
            this.rightpnl.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.rightpnl.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightpnl.Location = new System.Drawing.Point(649, 22);
            this.rightpnl.Margin = new System.Windows.Forms.Padding(0);
            this.rightpnl.Name = "rightpnl";
            this.rightpnl.Size = new System.Drawing.Size(2, 377);
            this.rightpnl.TabIndex = 1;
            this.rightpnl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titlebar_MouseDown);
            this.rightpnl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rightpnl_MouseMove);
            this.rightpnl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // bottompnl
            // 
            this.bottompnl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(196)))), ((int)(((byte)(213)))));
            this.bottompnl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bottompnl.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.bottompnl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottompnl.Location = new System.Drawing.Point(0, 399);
            this.bottompnl.Margin = new System.Windows.Forms.Padding(0);
            this.bottompnl.Name = "bottompnl";
            this.bottompnl.Size = new System.Drawing.Size(651, 2);
            this.bottompnl.TabIndex = 2;
            this.bottompnl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titlebar_MouseDown);
            this.bottompnl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bottompnl_MouseMove);
            this.bottompnl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // nwsize
            // 
            this.nwsize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nwsize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(196)))), ((int)(((byte)(213)))));
            this.nwsize.Cursor = System.Windows.Forms.Cursors.SizeNESW;
            this.nwsize.Location = new System.Drawing.Point(0, 398);
            this.nwsize.Name = "nwsize";
            this.nwsize.Size = new System.Drawing.Size(4, 4);
            this.nwsize.TabIndex = 3;
            this.nwsize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titlebar_MouseDown);
            this.nwsize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.nwsize_MouseMove);
            this.nwsize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // swresize
            // 
            this.swresize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.swresize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(196)))), ((int)(((byte)(213)))));
            this.swresize.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.swresize.Location = new System.Drawing.Point(649, 399);
            this.swresize.Name = "swresize";
            this.swresize.Size = new System.Drawing.Size(4, 4);
            this.swresize.TabIndex = 4;
            this.swresize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titlebar_MouseDown);
            this.swresize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.swresize_MouseMove);
            this.swresize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // cmdMaxRes
            // 
            this.cmdMaxRes.BackColor = System.Drawing.Color.Transparent;
            this.cmdMaxRes.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cmdMaxRes.BackgroundImage")));
            this.cmdMaxRes.Location = new System.Drawing.Point(47, 3);
            this.cmdMaxRes.Name = "cmdMaxRes";
            this.cmdMaxRes.Size = new System.Drawing.Size(16, 16);
            this.cmdMaxRes.TabIndex = 12;
            this.controlboxToolTip.SetToolTip(this.cmdMaxRes, "Maximize/Restore");
            this.cmdMaxRes.Click += new System.EventHandler(this.cmdMaxRes_Click);
            // 
            // cmdMin
            // 
            this.cmdMin.BackColor = System.Drawing.Color.Transparent;
            this.cmdMin.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cmdMin.BackgroundImage")));
            this.cmdMin.Location = new System.Drawing.Point(25, 3);
            this.cmdMin.Name = "cmdMin";
            this.cmdMin.Size = new System.Drawing.Size(16, 16);
            this.cmdMin.TabIndex = 11;
            this.controlboxToolTip.SetToolTip(this.cmdMin, "Minimize");
            this.cmdMin.Click += new System.EventHandler(this.cmdMin_Click);
            // 
            // cmdClose
            // 
            this.cmdClose.BackColor = System.Drawing.Color.Transparent;
            this.cmdClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cmdClose.BackgroundImage")));
            this.cmdClose.Location = new System.Drawing.Point(3, 3);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(16, 16);
            this.cmdClose.TabIndex = 10;
            this.controlboxToolTip.SetToolTip(this.cmdClose, "Close");
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // leftpnl
            // 
            this.leftpnl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(196)))), ((int)(((byte)(213)))));
            this.leftpnl.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.leftpnl.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftpnl.Location = new System.Drawing.Point(0, 22);
            this.leftpnl.Margin = new System.Windows.Forms.Padding(0);
            this.leftpnl.Name = "leftpnl";
            this.leftpnl.Size = new System.Drawing.Size(2, 377);
            this.leftpnl.TabIndex = 0;
            this.leftpnl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titlebar_MouseDown);
            this.leftpnl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.leftpnl_MouseMove);
            this.leftpnl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // bodyPanel
            // 
            this.bodyPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bodyPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(196)))), ((int)(((byte)(213)))));
            this.bodyPanel.Controls.Add(this.bottompnl2);
            this.bodyPanel.Controls.Add(this.rightpnl2);
            this.bodyPanel.Controls.Add(this.leftpnl2);
            this.bodyPanel.Location = new System.Drawing.Point(1, 22);
            this.bodyPanel.Margin = new System.Windows.Forms.Padding(0);
            this.bodyPanel.Name = "bodyPanel";
            this.bodyPanel.Size = new System.Drawing.Size(649, 377);
            this.bodyPanel.TabIndex = 6;
            // 
            // bottompnl2
            // 
            this.bottompnl2.BackColor = System.Drawing.Color.Transparent;
            this.bottompnl2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bottompnl2.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.bottompnl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottompnl2.Location = new System.Drawing.Point(1, 377);
            this.bottompnl2.Name = "bottompnl2";
            this.bottompnl2.Size = new System.Drawing.Size(647, 0);
            this.bottompnl2.TabIndex = 11;
            this.bottompnl2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titlebar_MouseDown);
            this.bottompnl2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bottompnl_MouseMove);
            this.bottompnl2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // rightpnl2
            // 
            this.rightpnl2.BackColor = System.Drawing.Color.Transparent;
            this.rightpnl2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rightpnl2.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.rightpnl2.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightpnl2.Location = new System.Drawing.Point(648, 0);
            this.rightpnl2.Name = "rightpnl2";
            this.rightpnl2.Size = new System.Drawing.Size(1, 377);
            this.rightpnl2.TabIndex = 10;
            this.rightpnl2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titlebar_MouseDown);
            this.rightpnl2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rightpnl_MouseMove);
            this.rightpnl2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // leftpnl2
            // 
            this.leftpnl2.BackColor = System.Drawing.Color.Transparent;
            this.leftpnl2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.leftpnl2.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.leftpnl2.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftpnl2.Location = new System.Drawing.Point(0, 0);
            this.leftpnl2.Name = "leftpnl2";
            this.leftpnl2.Size = new System.Drawing.Size(1, 377);
            this.leftpnl2.TabIndex = 9;
            this.leftpnl2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titlebar_MouseDown);
            this.leftpnl2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.leftpnl_MouseMove);
            this.leftpnl2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // panelTitlebar
            // 
            this.panelTitlebar.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.panelTitlebar.Controls.Add(this.cmdMaxRes);
            this.panelTitlebar.Controls.Add(this.cmdMin);
            this.panelTitlebar.Controls.Add(this.cmdClose);
            this.panelTitlebar.Controls.Add(this.topPanel);
            this.panelTitlebar.Controls.Add(this.titleCaption);
            this.panelTitlebar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitlebar.Location = new System.Drawing.Point(0, 0);
            this.panelTitlebar.Name = "panelTitlebar";
            this.panelTitlebar.Size = new System.Drawing.Size(651, 22);
            this.panelTitlebar.TabIndex = 5;
            this.panelTitlebar.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelmod1_MouseDoubleClick);
            this.panelTitlebar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titlebar_MouseDown);
            this.panelTitlebar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.titlebar_MouseMove);
            this.panelTitlebar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.topPanel.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Margin = new System.Windows.Forms.Padding(0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(651, 0);
            this.topPanel.TabIndex = 9;
            this.topPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titlebar_MouseDown);
            this.topPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.toppnl_MouseMove);
            this.topPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // titleCaption
            // 
            this.titleCaption.AutoEllipsis = true;
            this.titleCaption.AutoSize = true;
            this.titleCaption.BackColor = System.Drawing.Color.Transparent;
            this.titleCaption.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleCaption.Location = new System.Drawing.Point(257, 3);
            this.titleCaption.Name = "titleCaption";
            this.titleCaption.Size = new System.Drawing.Size(130, 14);
            this.titleCaption.TabIndex = 13;
            this.titleCaption.Text = "WindowsFormApplication";
            this.titleCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.titleCaption.Layout += new System.Windows.Forms.LayoutEventHandler(this.titleCaption_Layout);
            this.titleCaption.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelmod1_MouseDoubleClick);
            this.titleCaption.MouseDown += new System.Windows.Forms.MouseEventHandler(this.titlebar_MouseDown);
            this.titleCaption.MouseMove += new System.Windows.Forms.MouseEventHandler(this.titlebar_MouseMove);
            this.titleCaption.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // BorderlessForm
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(651, 401);
            this.Controls.Add(this.bodyPanel);
            this.Controls.Add(this.leftpnl);
            this.Controls.Add(this.rightpnl);
            this.Controls.Add(this.panelTitlebar);
            this.Controls.Add(this.swresize);
            this.Controls.Add(this.nwsize);
            this.Controls.Add(this.bottompnl);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(250, 25);
            this.Name = "BorderlessForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "WindowsFormApplication";
            this.Deactivate += new System.EventHandler(this.mactheme_Deactivate);
            this.Resize += new System.EventHandler(this.this_Resize);
            this.bodyPanel.ResumeLayout(false);
            this.panelTitlebar.ResumeLayout(false);
            this.panelTitlebar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        #region Public

        public void DoClose()
        {
            //Closes the form.
            Close();
            Dispose();
        }

        public void DoMin()
        {
            //minimizes the form.
            WindowState = FormWindowState.Minimized;
        }

        public void DoMax()
        {
            if (winMaxed)
            {
                //Restores the form and returns the resizing cursors of the edges.
                ShowWindow(Handle, SW_NORMAL);
                Top = originaltop;
                Left = originalleft;
                Size = originalSize;
                leftpnl.Cursor = Cursors.SizeWE;
                leftpnl2.Cursor = Cursors.SizeWE;
                rightpnl.Cursor = Cursors.SizeWE;
                rightpnl2.Cursor = Cursors.SizeWE;
                bottompnl.Cursor = Cursors.SizeNS;
                bottompnl2.Cursor = Cursors.SizeNS;
                nwsize.Cursor = Cursors.SizeNESW;
                swresize.Cursor = Cursors.SizeNWSE;
                topPanel.Cursor = Cursors.SizeNS;

                leftpnl.MouseMove += leftpnl_MouseMove;
                leftpnl2.MouseMove += leftpnl_MouseMove;
                topPanel.MouseMove += toppnl_MouseMove;
                bottompnl.MouseMove += bottompnl_MouseMove;
                bottompnl2.MouseMove += bottompnl_MouseMove;
                rightpnl.MouseMove += rightpnl_MouseMove;
                rightpnl2.MouseMove += rightpnl_MouseMove;
                swresize.MouseMove += swresize_MouseMove;
                nwsize.MouseMove += nwsize_MouseMove;
                titleCaption.MouseMove += titlebar_MouseMove;
                panelTitlebar.MouseMove += titlebar_MouseMove;

                winMaxed = false;
            }
            else
            {
                //Maximizes the form and removes the resizing cursors of the edges.
                originalleft = Left;
                originaltop = Top;
                originalSize = Size;
                DesktopLocation = new Point(0, 0);
                Size = MaximumSize;
                leftpnl.Cursor = Cursors.Arrow;
                leftpnl2.Cursor = Cursors.Arrow;
                rightpnl.Cursor = Cursors.Arrow;
                rightpnl2.Cursor = Cursors.Arrow;
                bottompnl.Cursor = Cursors.Arrow;
                bottompnl2.Cursor = Cursors.Arrow;
                nwsize.Cursor = Cursors.Arrow;
                swresize.Cursor = Cursors.Arrow;
                topPanel.Cursor = Cursors.Arrow;

                leftpnl.MouseMove -= leftpnl_MouseMove;
                leftpnl2.MouseMove -= leftpnl_MouseMove;
                topPanel.MouseMove -= toppnl_MouseMove;
                bottompnl.MouseMove -= bottompnl_MouseMove;
                bottompnl2.MouseMove -= bottompnl_MouseMove;
                rightpnl.MouseMove -= rightpnl_MouseMove;
                rightpnl2.MouseMove -= rightpnl_MouseMove;
                swresize.MouseMove -= swresize_MouseMove;
                nwsize.MouseMove -= nwsize_MouseMove;
                titleCaption.MouseMove -= titlebar_MouseMove;
                panelTitlebar.MouseMove -= titlebar_MouseMove;

                winMaxed = true;
            }
        }

        #endregion

        private void resize(ResizeLocation e)
        {
            //The resize function using the Windows API (SendMessage() and ReleaseCapture()).
            int _dir = -1;

            switch (e)
            {
                case ResizeLocation.top:
                    _dir = HTTOP;
                    break;
                case ResizeLocation.left:
                    _dir = HTLEFT;
                    break;
                case ResizeLocation.right:
                    _dir = HTRIGHT;
                    break;
                case ResizeLocation.bottom:
                    _dir = HTBOTTOM;
                    break;
                case ResizeLocation.bottomleft:
                    _dir = HTBOTTOMLEFT;
                    break;
                case ResizeLocation.bottomright:
                    _dir = HTBOTTOMRIGHT;
                    break;
            }

            if (_dir != -1)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, _dir, 0);
            }
        }

        private void titlebar_MouseDown(object sender, MouseEventArgs e)
        {
            Cursor.Clip = Screen.PrimaryScreen.WorkingArea;

            //For the resizing edges...
            try
            {
                Panel _p = (Panel)sender;

                switch (_p.Name)
                {
                    case "toppnl":
                        rLoc = ResizeLocation.top;
                        break;
                    case "leftpnl":
                        rLoc = ResizeLocation.left;
                        break;
                    case "leftpnl2":
                        rLoc = ResizeLocation.left;
                        break;
                    case "rightpnl":
                        rLoc = ResizeLocation.right;
                        break;
                    case "rightpnl2":
                        rLoc = ResizeLocation.right;
                        break;
                    case "bottompnl":
                        rLoc = ResizeLocation.bottom;
                        break;
                    case "bottompnl2":
                        rLoc = ResizeLocation.bottom;
                        break;
                    case "nwsize":
                        rLoc = ResizeLocation.bottomleft;
                        break;
                    case "swresize":
                        rLoc = ResizeLocation.bottomright;
                        break;
                }
            }
            catch
            {
            }
        }

        private void titlebar_MouseMove(object sender, MouseEventArgs e)
        {
            //Function for form dragging.
            if (WindowState != FormWindowState.Maximized)
            {
                if (MouseButtons.ToString() == "Left")
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
        }

        internal void rightpnl_MouseMove(object sender, MouseEventArgs e)
        {
            //Resize Function (right):
            if (MouseButtons.ToString() == "Left")
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    resize(rLoc);
                }
            }
        }

        internal void bottompnl_MouseMove(object sender, MouseEventArgs e)
        {
            //Resize Function (bottom);
            if (MouseButtons.ToString() == "Left")
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    resize(rLoc);
                }
            }
        }

        internal void swresize_MouseMove(object sender, MouseEventArgs e)
        {
            //The bottom right corner resize function.
            if (MouseButtons.ToString() == "Left")
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    resize(rLoc);
                }
            }
        }

        private void this_Resize(object sender, EventArgs e)
        {
            //Function to center the caption position and update the form to avoid flickers.
            //Always determine the maximum workingArea of the screen.
            MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);

            int _formWidth = Width;
            int _captionWidth = titleCaption.Width;

            if (titleCaption.Left < cmdMaxRes.Right)
            {
                titleCaption.Left = cmdMaxRes.Right + 5;
            }
            else if (titleCaption.Left > cmdMaxRes.Right + 5)
            {
                titleCaption.Left = Width / 2 - (titleCaption.Width / 2);
            }

            if ((_formWidth / 2) - (_captionWidth / 2) > cmdMaxRes.Right)
            {
                titleCaption.Left = Width / 2 - (titleCaption.Width / 2);
            }

            Update();
        }

        internal void leftpnl_MouseMove(object sender, MouseEventArgs e)
        {
            //Resize function (left side)
            if (MouseButtons.ToString() == "Left")
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    resize(rLoc);
                }
            }
        }

        private void bufferedPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            //releases the cursors limit when dragging the form around, to allow cursor to go to the taskbar.
            Cursor.Clip = Screen.PrimaryScreen.Bounds;
        }

        private void mactheme_Deactivate(object sender, EventArgs e)
        {
            // Sets the titlebar's background to a lighter color to determine that it's inactive and reset the cursor limits.
            // panelTitlebar.BackgroundImage = Resources.titlebarunfocused1;
            Cursor.Clip = Screen.PrimaryScreen.Bounds;
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            DoClose();
        }

        private void cmdMin_Click(object sender, EventArgs e)
        {
            DoMin();
        }

        private void cmdMaxRes_Click(object sender, EventArgs e)
        {
            DoMax();
        }

        private void titleCaption_Layout(object sender, LayoutEventArgs e)
        {
            //recenter the caption
            int _formWidth = Width;
            int _captionWidth = titleCaption.Width;

            if (titleCaption.Left < cmdMaxRes.Right)
            {
                titleCaption.Left = cmdMaxRes.Right + 5;
            }
            else if (titleCaption.Left > cmdMaxRes.Right + 5)
            {
                titleCaption.Left = Width / 2 - (titleCaption.Width / 2);
            }

            if ((_formWidth / 2) - (_captionWidth / 2) > cmdMaxRes.Right)
            {
                titleCaption.Left = Width / 2 - (titleCaption.Width / 2);
            }
        }

        internal void toppnl_MouseMove(object sender, MouseEventArgs e)
        {
            //Resize function (top)
            if (MouseButtons.ToString() == "Left")
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    resize(rLoc);
                }
            }
        }

        internal void nwsize_MouseMove(object sender, MouseEventArgs e)
        {
            //Resize function (left and bottom side)
            if (MouseButtons.ToString() == "Left")
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    resize(rLoc);
                }
            }
        }

        private void panelmod1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //Maximizes or Restores the window when you double-click the titlebar, similar to a regular window.
            switch (WindowState)
            {
                case FormWindowState.Maximized:
                    cmdMaxRes_Click(cmdMaxRes, null);
                    break;
                case FormWindowState.Normal:
                    cmdMaxRes_Click(cmdMaxRes, null);
                    break;
            }
        }
    }

    #endregion

    #region BorderlessFormTheme

    public class BorderlessFormTheme
    {
        private BorderlessForm m_form;

        public BorderlessForm HostWindow
        {
            get { return m_form; }
        }

        public void DoMax()
        {
            HostWindow.DoMax();
        }

        public void DoMin()
        {
            HostWindow.DoMin();
        }

        public void DoClose()
        {
            HostWindow.DoClose();
        }

        public void ApplyFormThemeSizable(Form clientForm, bool showDefaultTitleBar)
        {
            //This thread makes the specified form to be a control of the created Custom themed form (Resizable).
            m_form = new BorderlessForm();

            //Creates a new Custom themed borderless form (generated by the mactheme class).
            HostWindow.TopMost = clientForm.TopMost; //Determines if the themed form should be in the TopMost level.
            HostWindow.ShowInTaskbar = clientForm.ShowInTaskbar; //Determines if the themed form should appear in the taskbar.
            HostWindow.ShowIcon = clientForm.ShowIcon; //Determines if the themed form should show its icon in the taskbar.

            //Checks if the user wants to disable some sizing buttons...
            if (clientForm.MaximizeBox == false)
            {
                HostWindow.ControlBox = false;
                HostWindow.cmdMaxRes.Visible = false;
                HostWindow.MaximizeBox = false;
            }

            if (clientForm.MinimizeBox == false)
            {
                HostWindow.cmdMaxRes.Left = HostWindow.cmdMin.Left;
                HostWindow.cmdMin.Visible = false;
                HostWindow.MinimizeBox = false;
            }

            if (!showDefaultTitleBar)
            {
                HostWindow.panelTitlebar.Visible = false;

                HostWindow.bodyPanel.Top = HostWindow.topPanel.Height;
                HostWindow.bodyPanel.Height += HostWindow.panelTitlebar.Height;
            }

            clientForm.TopLevel = false;
            //Sets the TopLevel property of the clientForm to false so that we can add it as a client control on our custom themed form.

            clientForm.FormBorderStyle = FormBorderStyle.None; //Makes the clientForm borderless.
            HostWindow.Width = clientForm.Width + 8; //Adjusts the width of the Custom themed form.
            HostWindow.Top = 0; //Sets the default top location to 0.
            HostWindow.Left = 0; //Sets the default left location to 0.

            HostWindow.StartPosition = clientForm.StartPosition;
            //Sets the themed form's StartPosition same as the clientForm's StartPositon. If {0,0} location is used and it is needed to be applied, just set the clientForm's StartPosition to manual.

            if (clientForm.Top != 0)
            {
                HostWindow.StartPosition = FormStartPosition.Manual; //Sets the Form's Startup position to manual.
                HostWindow.Top = clientForm.Top; //Sets the themed form's top the same as the clientForm's top position.
            }
            if (clientForm.Left != 0)
            {
                HostWindow.StartPosition = FormStartPosition.Manual; //Sets the Form's Startup position to manual.
                HostWindow.Left = clientForm.Left; //Sets the themed form's left the same as the clientForm's Left position.
            }

            if (showDefaultTitleBar)
            {
                HostWindow.Height = clientForm.Height + 28; //Adjusts the height of the Custom themed form.
            }
            else
            {
                HostWindow.Height = clientForm.Height;
            }

            clientForm.Top = 0; //Sets the clientForm's top location to 0.
            clientForm.Left = 0; //Sets the clientForms' left location.

            HostWindow.Text = clientForm.Text;
            //Sets the custom themed form's Text property to the specified title (param = formTitle).

            // clientForm.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            clientForm.Dock = DockStyle.Fill;

            HostWindow.titleCaption.Text = clientForm.Text;
            //Sets the themed form's titleCaption Text property to the specified title (param = formTitle).

            HostWindow.bodyPanel.Controls.Add(clientForm); //Adds the clientForm to the bodypanel's Controls collection.
            HostWindow.Icon = clientForm.Icon;

            Size zeroSize = new Size(0, 0); //The default minimum size of a form (0,0).

            if (clientForm.MinimumSize != zeroSize)            //If the minimum width and height of the clientForm is not on default (0,0)....
            {
                if (showDefaultTitleBar)
                {
                    HostWindow.MinimumSize = new Size(clientForm.MinimumSize.Width + 8, clientForm.MinimumSize.Height + 28);
                }
                else
                {
                    HostWindow.MinimumSize = new Size(clientForm.MinimumSize.Width + 8, clientForm.MinimumSize.Height + 8);
                }
            }

            if (clientForm.Width < HostWindow.MinimumSize.Width)
            {
                clientForm.Width = HostWindow.MinimumSize.Width;
            }

            if (clientForm.Height < HostWindow.MinimumSize.Height)
            {
                clientForm.Height = HostWindow.MinimumSize.Height;
            }

            clientForm.FormClosed += onThemedForm_Close;
            //If the client form is closed, the themed form should also close as well.

            HostWindow.SizeGripStyle = clientForm.SizeGripStyle;

            HostWindow.Show(); //Show the generated themed form with the clientForm as it's child control.
        }

        public void ApplyFormThemeDialog(Form clientForm, Form parentForm)
        {
            //This thread makes the specified form to be a control of the created Custom themed form (Fixed Single).
            m_form = new BorderlessForm();
            //Creates a new Custom themed borderless form (generated by this library).

            clientForm.FormBorderStyle = FormBorderStyle.None; //Makes the clientForm borderless.
            HostWindow.Width = clientForm.Width + 8; //Adjusts the width of the Custom themed form.
            HostWindow.Height = clientForm.Height + 28; //Adjusts the height of the Custom themed form.
            HostWindow.Owner = parentForm; //Sets the owner of the themed form to the specified parentForm.
            HostWindow.StartPosition = clientForm.StartPosition;
            //Sets the themed form's StartPosition same as the clientForm's StartPositon. If {0,0} location is used and it is needed to be applied, just set the clientForm's StartPosition to manual.

            if (clientForm.Top != 0)
            {
                HostWindow.StartPosition = FormStartPosition.Manual; //Sets the Form's Startup position to manual.
                HostWindow.Top = clientForm.Top; //Sets the themed form's top the same as the clientForm's top position.
            }

            if (clientForm.Left != 0)
            {
                HostWindow.StartPosition = FormStartPosition.Manual; //Sets the Form's Startup position to manual.
                HostWindow.Left = clientForm.Left; //Sets the themed form's left the same as the clientForm's Left position.
            }

            clientForm.TopLevel = false;
            //Sets the TopLevel property of the clientForm to false so that we can add it as a client control on our custom themed form.

            //Sets the edges' cursor to normal and disable their resizing function;
            HostWindow.leftpnl.Cursor = Cursors.Arrow;
            HostWindow.leftpnl2.Cursor = Cursors.Arrow;
            HostWindow.rightpnl.Cursor = Cursors.Arrow;
            HostWindow.rightpnl2.Cursor = Cursors.Arrow;
            HostWindow.bottompnl2.Cursor = Cursors.Arrow;
            HostWindow.bottompnl.Cursor = Cursors.Arrow;
            HostWindow.topPanel.Cursor = Cursors.Arrow;
            HostWindow.leftpnl.MouseMove -= HostWindow.leftpnl_MouseMove;
            HostWindow.leftpnl2.MouseMove -= HostWindow.leftpnl_MouseMove;
            HostWindow.rightpnl.MouseMove -= HostWindow.rightpnl_MouseMove;
            HostWindow.rightpnl2.MouseMove -= HostWindow.rightpnl_MouseMove;
            HostWindow.bottompnl.MouseMove -= HostWindow.bottompnl_MouseMove;
            HostWindow.bottompnl2.MouseMove -= HostWindow.bottompnl_MouseMove;
            HostWindow.topPanel.MouseMove -= HostWindow.toppnl_MouseMove;

            ///////////////////////////////////////////////////////////

            //Do not show in taskbar and removes the minimize and maximize/restore buttons...
            HostWindow.cmdMaxRes.Visible = false;
            HostWindow.cmdMin.Visible = false;
            HostWindow.ShowInTaskbar = false;

            ///////////////////////////////////////////////////////////////////

            clientForm.Top = 0; //Sets the clientForm's top location to 0.
            clientForm.Left = 3; //Sets the clientForms' left location.
            HostWindow.Text = clientForm.Text;
            //Sets the custom themed form's Text property to the specified title (param = formTitle).

            clientForm.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            //Anchors the clientForm so it fits the themed form during resizing process.

            HostWindow.titleCaption.Text = clientForm.Text;
            //Sets the themed form's titleCaption Text property to the specified title (param = formTitle).

            HostWindow.bodyPanel.Controls.Add(clientForm); //Adds the clientForm to the bodypanel's Controls collection.
            HostWindow.Icon = clientForm.Icon;
            //Sets the themed form's Icon property the same as the clientForm's Icon property.

            Size zeroSize = new Size(0, 0); //The default minimum size of a form (0,0).

            if (clientForm.MinimumSize != zeroSize)
            //If the minimum width and height of the clientForm is not on default (0,0)....
            {
                HostWindow.MinimumSize = new Size(clientForm.MinimumSize.Width + 8, clientForm.MinimumSize.Height + 28);
                //Sets the minimum size of the themed form.
            }

            if (clientForm.Width < HostWindow.MinimumSize.Width)
            {
                clientForm.Width = HostWindow.MinimumSize.Width;
            }

            if (clientForm.Height < HostWindow.MinimumSize.Height)
            {
                clientForm.Height = HostWindow.MinimumSize.Height;
            }

            clientForm.FormClosed += onThemedForm_Close; //If the clientForm closes, the themed dialog closes as well.
            HostWindow.Show(); //Show the generated themed form with the clientForm as it's child control.
        }

        private void onThemedForm_Close(object sender, FormClosedEventArgs e)
        {
            //An event handler for closing the themed form to avoid leaving it running.
            try
            {
                //Gets the sender and checks if it is a custom themed form...
                Form _frm = (Form)sender;
                BorderlessForm thm = (BorderlessForm)_frm.ParentForm;
                thm.Close(); //closes the themed form.
            }
            catch
            {
                //Error message not displayed.
            }
        }

        public void ApplyFormThemeSingleSizable(Form clientForm)
        {
            //This thread makes the specified form to be a control of the created Custom themed form (Resizable, Thin Single Pixel borders, Corner resizing not available).
            m_form = new BorderlessForm();
            //Creates a new Custom themed borderless form (generated by the mactheme class).

            HostWindow.TopMost = clientForm.TopMost; //Determines if the themed form should be in the TopMost level.
            HostWindow.ShowInTaskbar = clientForm.ShowInTaskbar; //Determines if the themed form should appear in the taskbar.
            HostWindow.ShowIcon = clientForm.ShowIcon; //Determines if the themed form should show its icon in the taskbar.

            //Checks if the user wants to disable some sizing buttons...
            if (clientForm.MaximizeBox == false)
            {
                HostWindow.ControlBox = false;
                HostWindow.cmdMaxRes.Visible = false;
                HostWindow.MaximizeBox = false;
            }

            if (clientForm.MinimizeBox == false)
            {
                HostWindow.cmdMaxRes.Left = HostWindow.cmdMin.Left;
                HostWindow.cmdMin.Visible = false;
                HostWindow.MinimizeBox = false;
            }

            //////////////////////////////////////////////////////////

            clientForm.TopLevel = false;
            //Sets the TopLevel property of the clientForm to false so that we can add it as a client control on our custom themed form.

            clientForm.FormBorderStyle = FormBorderStyle.None; //Makes the clientForm borderless.
            HostWindow.Width = clientForm.Width + 8; //Adjusts the width of the Custom themed form.
            HostWindow.Height = clientForm.Height + 28; //Adjusts the height of the Custom themed form.
            HostWindow.leftpnl2.Width = 0; //Makes the thick resizable edge disappear.
            HostWindow.rightpnl2.Width = 0; //Makes the thick resizable edge disappear.
            HostWindow.bottompnl2.Height = 0; //Makes the thick resizalbe edge disappear.
            HostWindow.bodyPanel.Left = 1; //Sets the themed form's form container (bodypanel) near the gray edge.
            HostWindow.Top = 0; //Sets the default top location to 0.
            HostWindow.Left = 0; //Sets the default left location to 0.

            HostWindow.StartPosition = clientForm.StartPosition;
            //Sets the themed form's StartPosition same as the clientForm's StartPositon. If {0,0} location is used and it is needed to be applied, just set the clientForm's StartPosition to manual.

            if (clientForm.Top != 0)
            {
                HostWindow.StartPosition = FormStartPosition.Manual; //Sets the Form's Startup position to manual.
                HostWindow.Top = clientForm.Top; //Sets the themed form's top the same as the clientForm's top position.
            }

            if (clientForm.Left != 0)
            {
                HostWindow.StartPosition = FormStartPosition.Manual; //Sets the Form's Startup position to manual.
                HostWindow.Left = clientForm.Left; //Sets the themed form's left the same as the clientForm's Left position.
            }

            clientForm.Top = 0; //Sets the clientForm's top location to 0.
            clientForm.Left = 0; //Sets the clientForms' left location.
            clientForm.Width += 6; //Adds width to the clientForm to reach the right edge.
            clientForm.Height += 5; //Adds height to the clientform to reach the bottom edge.
            HostWindow.bodyPanel.Height += 2; //Adds height to the clientForm container to reach the bottom edge.
            HostWindow.Text = clientForm.Text;
           
            clientForm.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            //Anchors the clientForm so it fits the themed form during resizing process.

            HostWindow.titleCaption.Text = clientForm.Text;
            //Sets the themed form's titleCaption Text property to the specified title (param = formTitle).

            HostWindow.bodyPanel.Controls.Add(clientForm); //Adds the clientForm to the bodypanel's Controls collection.
            HostWindow.Icon = clientForm.Icon;
            //Sets the themed form's Icon property the same as the clientForm's Icon property.

            Size _zeroSize = new Size(0, 0); //The default minimum size of a form (0,0).

            if (clientForm.MinimumSize != _zeroSize)
            //If the minimum width and height of the clientForm is not on default (0,0)....
            {
                HostWindow.MinimumSize = new Size(clientForm.MinimumSize.Width + 8, clientForm.MinimumSize.Height + 28);
                //Sets the minimum size of the themed form.
            }

            if (clientForm.Width < HostWindow.MinimumSize.Width)
            {
                clientForm.Width = HostWindow.MinimumSize.Width;
            }

            if (clientForm.Height < HostWindow.MinimumSize.Height)
            {
                clientForm.Height = HostWindow.MinimumSize.Height;
            }

            clientForm.FormClosed += onThemedForm_Close;
            //If the client form is closed, the themed form should also close as well.          

            HostWindow.Show(); //Show the generated themed form with the clientForm as it's child control.
        }
    }

    #endregion
}