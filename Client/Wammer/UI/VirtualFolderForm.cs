#region

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Waveface.Component;

#endregion

namespace Waveface
{
    public partial class VirtualFolderForm : Form
    {
        #region Fields

        private int MAX_HEIGHT = 300;
        private int MAX_WIDTH = 600;
        private int MIN_HEIGHT = 10;
        private int MIN_WIDTH = 20;

        private bool m_isMoving;
        private bool m_isResizingBottom;
        private bool m_isResizingBottomLeft;
        private bool m_isResizingBottomRight;
        private bool m_isResizingLeft;
        private bool m_isResizingRight;

        private bool m_isResizingTop;
        private bool m_isresizingtopleft;
        private bool m_isresizingtopright;
        private bool m_isrezising;

        private Point m_mousePos;

        // Define Rectangles & Booleans for all 9 + 1 areas of the Form.
        private Rectangle m_r0;
        private Rectangle m_r1;
        private Rectangle m_r2;
        private Rectangle m_r3;
        private Rectangle m_r4;
        private Rectangle m_r5;
        private Rectangle m_r6;
        private Rectangle m_r7;
        private Rectangle m_r8;
        private Rectangle m_r9;

        private Point m_resizeDestination;
        private Point m_resizeStart;
        private bool m_showVirtualBorders = true;
        private int m_virtualBorder = 5;

        #endregion

        private StickyWindow m_stickyWindow;

        private DragDrop_Clipboard_Helper m_dragDropClipboardHelper;

        public VirtualFolderForm()
        {
            InitializeComponent();

            m_dragDropClipboardHelper = new DragDrop_Clipboard_Helper();

            FormBorderStyle = FormBorderStyle.None;
            DoubleBuffered = true;
            Opacity = 0.5;

            m_stickyWindow = new StickyWindow(this);
        }

        private void VirtualFolderForm_Load(object sender, EventArgs e)
        {
            Width = 64;
            Height = 64;
        }

        #region Borderless

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ReleaseCapture();


        private void frmBounds_Paint(object sender, PaintEventArgs e)
        {
            // Every time the form paints, set the location and size of the form areas...

            m_r1 = new Rectangle(new Point(ClientRectangle.X, ClientRectangle.Y), new Size(m_virtualBorder, m_virtualBorder));

            m_r2 = new Rectangle(new Point(ClientRectangle.X + m_r1.Width, ClientRectangle.Y),
                               new Size(ClientRectangle.Width - (m_r1.Width * 2), m_r1.Height));

            m_r3 = new Rectangle(new Point(ClientRectangle.X + m_r1.Width + m_r2.Width, ClientRectangle.Y),
                               new Size(m_virtualBorder, m_virtualBorder));

            m_r4 = new Rectangle(new Point(ClientRectangle.X, ClientRectangle.Y + m_r1.Height),
                               new Size(m_r1.Width, ClientRectangle.Height - (m_r1.Width * 2)));

            m_r5 = new Rectangle(new Point(ClientRectangle.X + m_r4.Width, ClientRectangle.Y + m_r1.Height),
                               new Size(m_r2.Width, m_r4.Height));

            m_r6 = new Rectangle(new Point(ClientRectangle.X + m_r4.Width + m_r5.Width, ClientRectangle.Y + m_r1.Height),
                               new Size(m_r3.Width, m_r4.Height));

            m_r7 = new Rectangle(new Point(ClientRectangle.X, ClientRectangle.Y + m_r1.Height + m_r4.Height),
                               new Size(m_virtualBorder, m_virtualBorder));

            m_r8 = new Rectangle(new Point(ClientRectangle.X + m_r7.Width, ClientRectangle.Y + m_r1.Height + m_r4.Height),
                               new Size(ClientRectangle.Width - (m_r7.Width * 2), m_r7.Height));
            m_r9 =
                new Rectangle(
                    new Point(ClientRectangle.X + m_r7.Width + m_r8.Width, ClientRectangle.Y + m_r1.Height + m_r4.Height),
                    new Size(m_virtualBorder, m_virtualBorder));

            if (m_showVirtualBorders)
            {
                Graphics _r = e.Graphics;
                _r.FillRectangle(Brushes.White, m_r5);

                _r.FillRectangle(Brushes.Gold, m_r1);
                _r.FillRectangle(Brushes.Gold, m_r3);
                _r.FillRectangle(Brushes.Gold, m_r7);
                _r.FillRectangle(Brushes.Gold, m_r9);

                _r.FillRectangle(Brushes.Red, m_r2);
                _r.FillRectangle(Brushes.Red, m_r8);
                _r.FillRectangle(Brushes.Red, m_r4);
                _r.FillRectangle(Brushes.Red, m_r6);
            }
        }

        private void frmBounds_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:

                    if (m_r1.Contains(m_mousePos))
                    {
                        m_isrezising = true;
                        m_isresizingtopleft = true;
                        m_resizeStart = PointToScreen(new Point(e.X, e.Y));
                        return;
                    }

                    if (m_r2.Contains(m_mousePos))
                    {
                        m_isrezising = true;
                        m_isResizingTop = true;
                        m_resizeStart = PointToScreen(new Point(e.X, e.Y));
                        return;
                    }

                    if (m_r3.Contains(m_mousePos))
                    {
                        m_isrezising = true;
                        m_isresizingtopright = true;
                        m_resizeStart = PointToScreen(new Point(e.X, e.Y));
                        return;
                    }

                    if (m_r4.Contains(m_mousePos))
                    {
                        m_isrezising = true;
                        m_isResizingLeft = true;
                        m_resizeStart = PointToScreen(new Point(e.X, e.Y));
                        return;
                    }

                    /*
                    if (m_r5.Contains(m_mousePos))
                    {
                        // If the center area of the form is pressed (R5), then we should be able to move the form.
                        m_isMoving = true;
                        m_isrezising = false;
                        m_mousePos = new Point(e.X, e.Y);
                        Cursor = Cursors.SizeAll;
                        return;
                    }
                    */

                    if (m_r6.Contains(m_mousePos))
                    {
                        m_isrezising = true;
                        m_isResizingRight = true;
                        m_resizeStart = PointToScreen(new Point(e.X, e.Y));
                        return;
                    }

                    if (m_r7.Contains(m_mousePos))
                    {
                        m_isrezising = true;
                        m_isResizingBottomLeft = true;
                        m_resizeStart = PointToScreen(new Point(e.X, e.Y));
                        return;
                    }

                    if (m_r8.Contains(m_mousePos))
                    {
                        m_isrezising = true;
                        m_isResizingBottom = true;
                        m_resizeStart = PointToScreen(new Point(e.X, e.Y));
                        return;
                    }

                    if (m_r9.Contains(m_mousePos))
                    {
                        m_isrezising = true;
                        m_isResizingBottomRight = true;
                        m_resizeStart = PointToScreen(new Point(e.X, e.Y));
                        return;
                    }

                    ReleaseCapture();
                    m_stickyWindow.OffectX = e.X;
                    m_stickyWindow.OffectY = e.Y;
                    SendMessage(Handle, 0x00A1, (IntPtr)0x02, IntPtr.Zero); // WM_NCLBUTTONDOWN, HTCAPTION

                    break;
            }
        }

        private void frmBounds_MouseMove(object sender, MouseEventArgs e)
        {
            m_resizeDestination = PointToScreen(new Point(e.X, e.Y));
            m_r0 = Bounds;

            // If the form has captured the mouse...
            if (Capture)
            {
                if (m_isMoving)
                {
                    m_isrezising = false;

                    // ISMOVING is true if the R5 rectangle is pressed. Allow the form to be moved around the screen.
                    Location = new Point(MousePosition.X - m_mousePos.X, MousePosition.Y - m_mousePos.Y);
                }

                if (m_isrezising)
                {
                    m_isMoving = false;

                    if (m_isresizingtopleft)
                    {
                        Bounds = new Rectangle(m_r0.X + m_resizeDestination.X - m_resizeStart.X,
                                               m_r0.Y + m_resizeDestination.Y - m_resizeStart.Y,
                                               m_r0.Width - m_resizeDestination.X + m_resizeStart.X,
                                               m_r0.Height - m_resizeDestination.Y + m_resizeStart.Y);
                    }

                    if (m_isResizingTop)
                    {
                        Bounds = new Rectangle(m_r0.X, m_r0.Y + m_resizeDestination.Y - m_resizeStart.Y, m_r0.Width,
                                               m_r0.Height - m_resizeDestination.Y + m_resizeStart.Y);
                    }

                    if (m_isresizingtopright)
                    {
                        Bounds = new Rectangle(m_r0.X, m_r0.Y + m_resizeDestination.Y - m_resizeStart.Y,
                                               m_r0.Width + m_resizeDestination.X - m_resizeStart.X,
                                               m_r0.Height - m_resizeDestination.Y + m_resizeStart.Y);
                    }

                    if (m_isResizingLeft)
                    {
                        Bounds = new Rectangle(m_r0.X + m_resizeDestination.X - m_resizeStart.X, m_r0.Y,
                                               m_r0.Width - m_resizeDestination.X + m_resizeStart.X, m_r0.Height);
                    }

                    if (m_isResizingRight)
                    {
                        Bounds = new Rectangle(m_r0.X, m_r0.Y, m_r0.Width + m_resizeDestination.X - m_resizeStart.X, m_r0.Height);
                    }

                    if (m_isResizingBottomLeft)
                    {
                        Bounds = new Rectangle(m_r0.X + m_resizeDestination.X - m_resizeStart.X, m_r0.Y,
                                               m_r0.Width - m_resizeDestination.X + m_resizeStart.X,
                                               m_r0.Height + m_resizeDestination.Y - m_resizeStart.Y);
                    }

                    if (m_isResizingBottom)
                    {
                        Bounds = new Rectangle(m_r0.X, m_r0.Y, m_r0.Width, m_r0.Height + m_resizeDestination.Y - m_resizeStart.Y);
                    }

                    if (m_isResizingBottomRight)
                    {
                        Bounds = new Rectangle(m_r0.X, m_r0.Y, m_r0.Width + m_resizeDestination.X - m_resizeStart.X,
                                               m_r0.Height + m_resizeDestination.Y - m_resizeStart.Y);
                    }

                    m_resizeStart = m_resizeDestination;

                    Invalidate();
                }
            }

                // If the form has not captured the mouse; the mouse is just hovering the form...
            else
            {
                m_mousePos = new Point(e.X, e.Y);

                // Changes Cursor depending where the mousepointer is at the form...
                if (m_r1.Contains(m_mousePos))
                {
                    Cursor = Cursors.SizeNWSE;
                }

                if (m_r2.Contains(m_mousePos))
                {
                    Cursor = Cursors.SizeNS;
                }

                if (m_r3.Contains(m_mousePos))
                {
                    Cursor = Cursors.SizeNESW;
                }

                if (m_r4.Contains(m_mousePos))
                {
                    Cursor = Cursors.SizeWE;
                }

                if (m_r5.Contains(m_mousePos))
                {
                    Cursor = Cursors.Default;
                }

                if (m_r6.Contains(m_mousePos))
                {
                    Cursor = Cursors.SizeWE;
                }

                if (m_r7.Contains(m_mousePos))
                {
                    Cursor = Cursors.SizeNESW;
                }

                if (m_r8.Contains(m_mousePos))
                {
                    Cursor = Cursors.SizeNS;
                }

                if (m_r9.Contains(m_mousePos))
                {
                    Cursor = Cursors.SizeNWSE;
                }
            }
        }

        private void frmBounds_Resize(object sender, EventArgs e)
        {
            //CODE TO LIMIT THE SIZE OF THE FORM
            if (Height > MAX_HEIGHT)
            {
                Capture = false;
                Height = MAX_HEIGHT;
            }

            if (Height < MIN_HEIGHT)
            {
                Capture = false;
                Height = MIN_HEIGHT;
            }

            if (Width > MAX_WIDTH)
            {
                Capture = false;
                Width = MAX_WIDTH;
            }

            if (Width < MIN_WIDTH)
            {
                Capture = false;
                Width = MIN_WIDTH;
            }
        }

        private void frmBounds_MouseUp(object sender, MouseEventArgs e)
        {
            m_isMoving = false;
            m_isrezising = false;

            m_isResizingLeft = false;
            m_isResizingRight = false;

            m_isResizingTop = false;
            m_isResizingBottom = false;

            m_isresizingtopright = false;
            m_isresizingtopleft = false;

            m_isResizingBottomRight = false;
            m_isResizingBottomLeft = false;

            Invalidate();
        }

        #endregion

        private void VirtualFolderForm_DragEnter(object sender, DragEventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Enter(e);
        }

        private void VirtualFolderForm_DragDrop(object sender, DragEventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Drop(e);
        }

        private void VirtualFolderForm_DragOver(object sender, DragEventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Over(e);
        }

        private void VirtualFolderForm_DragLeave(object sender, EventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Leave();
        }
    }
}