#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;
using Panel = System.Windows.Forms.Panel;

#endregion

namespace Waveface.Component
{
    #region BorderlessForm

    public class BorderlessForm : Form
    {
        #region Fields

        internal Panel rightpnl;
        internal Panel nwsize;
        internal Panel swresize;
        internal Panel bottompnl;
        private IContainer components;
        private Panel panelTitle;
        internal Panel toppnl;
        public Label titleCaption;
        internal Panel leftpnl;
        public Panel bodypanel;
        internal Panel bottompnl2;
        internal Panel rightpnl2;
        internal Panel leftpnl2;
        private int originalleft;
        private int originaltop;
        internal bool winMaxed;
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
            this.rightpnl = new System.Windows.Forms.Panel();
            this.bottompnl = new System.Windows.Forms.Panel();
            this.nwsize = new System.Windows.Forms.Panel();
            this.swresize = new System.Windows.Forms.Panel();
            this.leftpnl = new System.Windows.Forms.Panel();
            this.bodypanel = new Panel();
            this.bottompnl2 = new System.Windows.Forms.Panel();
            this.rightpnl2 = new System.Windows.Forms.Panel();
            this.leftpnl2 = new System.Windows.Forms.Panel();
            this.panelTitle = new Panel();
            this.toppnl = new System.Windows.Forms.Panel();
            this.titleCaption = new System.Windows.Forms.Label();
            this.bodypanel.SuspendLayout();
            this.panelTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // rightpnl
            // 
            this.rightpnl.BackColor = System.Drawing.Color.Black;
            this.rightpnl.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.rightpnl.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightpnl.Location = new System.Drawing.Point(650, 22);
            this.rightpnl.Name = "rightpnl";
            this.rightpnl.Size = new System.Drawing.Size(1, 378);
            this.rightpnl.TabIndex = 1;
            this.rightpnl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDown);
            this.rightpnl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rightpnl_MouseMove);
            this.rightpnl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // bottompnl
            // 
            this.bottompnl.BackColor = System.Drawing.Color.Black;
            this.bottompnl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bottompnl.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.bottompnl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottompnl.Location = new System.Drawing.Point(0, 400);
            this.bottompnl.Name = "bottompnl";
            this.bottompnl.Size = new System.Drawing.Size(651, 1);
            this.bottompnl.TabIndex = 2;
            this.bottompnl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDown);
            this.bottompnl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bottompnl_MouseMove);
            this.bottompnl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // nwsize
            // 
            this.nwsize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nwsize.Cursor = System.Windows.Forms.Cursors.SizeNESW;
            this.nwsize.Location = new System.Drawing.Point(1, 398);
            this.nwsize.Name = "nwsize";
            this.nwsize.Size = new System.Drawing.Size(2, 2);
            this.nwsize.TabIndex = 3;
            this.nwsize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDown);
            this.nwsize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.nwsize_MouseMove);
            this.nwsize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // swresize
            // 
            this.swresize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.swresize.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.swresize.Location = new System.Drawing.Point(648, 398);
            this.swresize.Name = "swresize";
            this.swresize.Size = new System.Drawing.Size(2, 2);
            this.swresize.TabIndex = 4;
            this.swresize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDown);
            this.swresize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.swresize_MouseMove);
            this.swresize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // leftpnl
            // 
            this.leftpnl.BackColor = System.Drawing.Color.Black;
            this.leftpnl.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.leftpnl.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftpnl.Location = new System.Drawing.Point(0, 22);
            this.leftpnl.Name = "leftpnl";
            this.leftpnl.Size = new System.Drawing.Size(1, 378);
            this.leftpnl.TabIndex = 0;
            this.leftpnl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDown);
            this.leftpnl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.leftpnl_MouseMove);
            this.leftpnl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // bodypanel
            // 
            this.bodypanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bodypanel.BackColor = System.Drawing.Color.Transparent;
            this.bodypanel.Controls.Add(this.bottompnl2);
            this.bodypanel.Controls.Add(this.rightpnl2);
            this.bodypanel.Controls.Add(this.leftpnl2);
            this.bodypanel.Location = new System.Drawing.Point(1, 1);
            this.bodypanel.Name = "bodypanel";
            this.bodypanel.Size = new System.Drawing.Size(649, 397);
            this.bodypanel.TabIndex = 6;
            // 
            // bottompnl2
            // 
            this.bottompnl2.BackColor = System.Drawing.Color.Transparent;
            this.bottompnl2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bottompnl2.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.bottompnl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottompnl2.Location = new System.Drawing.Point(2, 394);
            this.bottompnl2.Name = "bottompnl2";
            this.bottompnl2.Size = new System.Drawing.Size(644, 3);
            this.bottompnl2.TabIndex = 11;
            this.bottompnl2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDown);
            this.bottompnl2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bottompnl_MouseMove);
            this.bottompnl2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // rightpnl2
            // 
            this.rightpnl2.BackColor = System.Drawing.Color.Transparent;
            this.rightpnl2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rightpnl2.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.rightpnl2.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightpnl2.Location = new System.Drawing.Point(646, 0);
            this.rightpnl2.Name = "rightpnl2";
            this.rightpnl2.Size = new System.Drawing.Size(3, 397);
            this.rightpnl2.TabIndex = 10;
            this.rightpnl2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDown);
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
            this.leftpnl2.Size = new System.Drawing.Size(2, 397);
            this.leftpnl2.TabIndex = 9;
            this.leftpnl2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDown);
            this.leftpnl2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.leftpnl_MouseMove);
            this.leftpnl2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // panelTitle
            // 
            this.panelTitle.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTitle.Controls.Add(this.toppnl);
            this.panelTitle.Controls.Add(this.titleCaption);
            this.panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitle.Location = new System.Drawing.Point(0, 0);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(651, 22);
            this.panelTitle.TabIndex = 5;
            this.panelTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseMove);
            // 
            // toppnl
            // 
            this.toppnl.BackColor = System.Drawing.Color.Transparent;
            this.toppnl.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.toppnl.Dock = System.Windows.Forms.DockStyle.Top;
            this.toppnl.Location = new System.Drawing.Point(0, 0);
            this.toppnl.Name = "toppnl";
            this.toppnl.Size = new System.Drawing.Size(649, 3);
            this.toppnl.TabIndex = 9;
            this.toppnl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDown);
            this.toppnl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.toppnl_MouseMove);
            this.toppnl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
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
            this.titleCaption.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDown);
            this.titleCaption.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseMove);
            this.titleCaption.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bufferedPanel1_MouseUp);
            // 
            // BorderlessForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.ClientSize = new System.Drawing.Size(651, 401);
            this.Controls.Add(this.bodypanel);
            this.Controls.Add(this.leftpnl);
            this.Controls.Add(this.rightpnl);
            this.Controls.Add(this.swresize);
            this.Controls.Add(this.nwsize);
            this.Controls.Add(this.bottompnl);
            this.Controls.Add(this.panelTitle);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(250, 25);
            this.Name = "BorderlessForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "WindowsFormApplication";
            this.Activated += new System.EventHandler(this.mactheme_Activated);
            this.Deactivate += new System.EventHandler(this.mactheme_Deactivate);
            this.Resize += new System.EventHandler(this.mactheme_Resize);
            this.bodypanel.ResumeLayout(false);
            this.panelTitle.ResumeLayout(false);
            this.panelTitle.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        //Windows API for resizing the window.
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, long lParam, long wParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

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

        private void resize(ResizeLocation e)
        {
            int dir = -1;

            switch (e)
            {
                case ResizeLocation.top:
                    dir = HTTOP;
                    break;
                case ResizeLocation.left:
                    dir = HTLEFT;
                    break;
                case ResizeLocation.right:
                    dir = HTRIGHT;
                    break;
                case ResizeLocation.bottom:
                    dir = HTBOTTOM;
                    break;
                case ResizeLocation.bottomleft:
                    dir = HTBOTTOMLEFT;
                    break;
                case ResizeLocation.bottomright:
                    dir = HTBOTTOMRIGHT;
                    break;
            }

            if (dir != -1)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, dir, 0);
            }
        }

        private void panelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            Cursor.Clip = Screen.PrimaryScreen.WorkingArea;

            //For the resizing edges...
            try
            {
                Panel panel = (Panel)sender;

                switch (panel.Name)
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

        private void panelTitle_MouseMove(object sender, MouseEventArgs e)
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

        private void mactheme_Resize(object sender, EventArgs e)
        {
            //Function to center the caption position and update the form to avoid flickers.
            //Always determine the maximum workingArea of the screen.
            MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);

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

        private void mactheme_Activated(object sender, EventArgs e)
        {
        }

        private void mactheme_Deactivate(object sender, EventArgs e)
        {
            Cursor.Clip = Screen.PrimaryScreen.Bounds;
        }

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
            if (winMaxed == false)
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
                toppnl.Cursor = Cursors.Arrow;

                leftpnl.MouseMove -= leftpnl_MouseMove;
                leftpnl2.MouseMove -= leftpnl_MouseMove;
                toppnl.MouseMove -= toppnl_MouseMove;
                bottompnl.MouseMove -= bottompnl_MouseMove;
                bottompnl2.MouseMove -= bottompnl_MouseMove;
                rightpnl.MouseMove -= rightpnl_MouseMove;
                rightpnl2.MouseMove -= rightpnl_MouseMove;
                swresize.MouseMove -= swresize_MouseMove;
                nwsize.MouseMove -= nwsize_MouseMove;
                titleCaption.MouseMove -= panelTitle_MouseMove;
                panelTitle.MouseMove -= panelTitle_MouseMove;

                winMaxed = true;
            }
            else
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
                toppnl.Cursor = Cursors.SizeNS;

                leftpnl.MouseMove += leftpnl_MouseMove;
                leftpnl2.MouseMove += leftpnl_MouseMove;
                toppnl.MouseMove += toppnl_MouseMove;
                bottompnl.MouseMove += bottompnl_MouseMove;
                bottompnl2.MouseMove += bottompnl_MouseMove;
                rightpnl.MouseMove += rightpnl_MouseMove;
                rightpnl2.MouseMove += rightpnl_MouseMove;
                swresize.MouseMove += swresize_MouseMove;
                nwsize.MouseMove += nwsize_MouseMove;
                titleCaption.MouseMove += panelTitle_MouseMove;
                panelTitle.MouseMove += panelTitle_MouseMove;

                winMaxed = false;
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

        public override string Text
        {
            //updates the titleCaption...
            get { return base.Text; }
            set
            {
                titleCaption.Text = value;

                base.Text = value;
            }
        }
    }

    #endregion

    #region ThemeManager

    public class BorderlessFormTheme
    {
        private BorderlessForm m_form;

        public void DoMax()
        {
            m_form.DoMax();
        }

        public void DoMin()
        {
            m_form.DoMin();
        }

        public void DoClose()
        {
            m_form.DoClose();
        }

        //Functions for applying the  form theme.
        public void ApplyFormThemeSizable(Form clientForm)
        {
            //This thread makes the specified form to be a control of the created Custom themed form (Resizable).
            m_form = new BorderlessForm(); //Creates a new Custom themed borderless form (generated by the mactheme class).
            m_form.TopMost = clientForm.TopMost; //Determines if the themed form should be in the TopMost level.
            m_form.ShowInTaskbar = clientForm.ShowInTaskbar; //Determines if the themed form should appear in the taskbar.
            m_form.ShowIcon = clientForm.ShowIcon; //Determines if the themed form should show its icon in the taskbar.

            //Checks if the user wants to disable some sizing buttons...
            if (clientForm.MaximizeBox == false)
            {
                m_form.ControlBox = false;
                m_form.MaximizeBox = false;
            }

            if (clientForm.MinimizeBox == false)
            {
                m_form.MinimizeBox = false;
            }

            //////////////////////////////////////////////////////////

            clientForm.TopLevel = false;

            //Sets the TopLevel property of the clientForm to false so that we can add it as a client control on our custom themed form.
            clientForm.FormBorderStyle = FormBorderStyle.None; //Makes the clientForm borderless.
            m_form.Width = clientForm.Width + 8; //Adjusts the width of the Custom themed form.
            m_form.Top = 0; //Sets the default top location to 0.
            m_form.Left = 0; //Sets the default left location to 0.

            m_form.StartPosition = clientForm.StartPosition;

            //Sets the themed form's StartPosition same as the clientForm's StartPositon. If {0,0} location is used and it is needed to be applied, just set the clientForm's StartPosition to manual.
            if (clientForm.Top != 0)
            {
                m_form.StartPosition = FormStartPosition.Manual; //Sets the Form's Startup position to manual.
                m_form.Top = clientForm.Top; //Sets the themed form's top the same as the clientForm's top position.
            }

            if (clientForm.Left != 0)
            {
                m_form.StartPosition = FormStartPosition.Manual; //Sets the Form's Startup position to manual.
                m_form.Left = clientForm.Left; //Sets the themed form's left the same as the clientForm's Left position.
            }

            m_form.Height = clientForm.Height;// +28; //Adjusts the height of the Custom themed form.
            clientForm.Top = 3; //Sets the clientForm's top location to 0.
            clientForm.Left = 3; //Sets the clientForms' left location.
            m_form.Text = clientForm.Text;

            clientForm.Dock = DockStyle.Fill;

            //Anchors the clientForm so it fits the themed form during resizing process.
            m_form.titleCaption.Text = clientForm.Text;
            //Sets the themed form's titleCaption Text property to the specified title (param = formTitle).
            m_form.bodypanel.Controls.Add(clientForm); //Adds the clientForm to the bodypanel's Controls collection.
            m_form.Icon = clientForm.Icon;
            //Sets the themed form's Icon property the same as the clientForm's Icon property.

            Size zeroSize = new Size(0, 0); //The default minimum size of a form (0,0).

            if (clientForm.MinimumSize != zeroSize)
            //If the minimum width and height of the clientForm is not on default (0,0)....
            {
                m_form.MinimumSize = new Size(clientForm.MinimumSize.Width + 8, clientForm.MinimumSize.Height + 8);
                //Sets the minimum size of the themed form.
            }

            if (clientForm.Width < m_form.MinimumSize.Width)
            {
                clientForm.Width = m_form.MinimumSize.Width;
            }

            if (clientForm.Height < m_form.MinimumSize.Height)
            {
                clientForm.Height = m_form.MinimumSize.Height;
            }

            clientForm.FormClosed += onThemedForm_Close;

            //If the client form is closed, the themed form should also close as well.
            m_form.Show(); //Show the generated themed form with the clientForm as it's child control.
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
    }

    #endregion
}