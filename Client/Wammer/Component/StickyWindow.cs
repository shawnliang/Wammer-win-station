#region

using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

#endregion

namespace Waveface.Component
{
    // A windows that Sticks to other windows of the same type when moved or resized.
    // You get a nice way of organizing multiple top-level windows.
    public class StickyWindow : NativeWindow
    {
        #region Win32

        public class MyWin32
        {
            [DllImport("user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            public static extern short GetAsyncKeyState(int vKey);

            [DllImport("user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetDesktopWindow();

            #region Nested type: Bit

            public class Bit
            {
                public static int HiWord(int iValue)
                {
                    return ((iValue >> 16) & 0xFFFF);
                }

                public static int LoWord(int iValue)
                {
                    return (iValue & 0xFFFF);
                }
            }

            #endregion

            #region Nested type: HT

            // HT is just a placeholder for HT (HitTest) definitions
            public class HT
            {
                public const int HTERROR = (-2);
                public const int HTTRANSPARENT = (-1);
                public const int HTNOWHERE = 0;
                public const int HTCLIENT = 1;
                public const int HTCAPTION = 2;
                public const int HTSYSMENU = 3;
                public const int HTGROWBOX = 4;
                public const int HTSIZE = HTGROWBOX;
                public const int HTMENU = 5;
                public const int HTHSCROLL = 6;
                public const int HTVSCROLL = 7;
                public const int HTMINBUTTON = 8;
                public const int HTMAXBUTTON = 9;
                public const int HTLEFT = 10;
                public const int HTRIGHT = 11;
                public const int HTTOP = 12;
                public const int HTTOPLEFT = 13;
                public const int HTTOPRIGHT = 14;
                public const int HTBOTTOM = 15;
                public const int HTBOTTOMLEFT = 16;
                public const int HTBOTTOMRIGHT = 17;
                public const int HTBORDER = 18;
                public const int HTREDUCE = HTMINBUTTON;
                public const int HTZOOM = HTMAXBUTTON;
                public const int HTSIZEFIRST = HTLEFT;
                public const int HTSIZELAST = HTBOTTOMRIGHT;

                public const int HTOBJECT = 19;
                public const int HTCLOSE = 20;
                public const int HTHELP = 21;
            }

            #endregion

            #region Nested type: VK

            // VK is just a placeholder for VK (VirtualKey) general definitions
            public class VK
            {
                public const int VK_SHIFT = 0x10;
                public const int VK_CONTROL = 0x11;
                public const int VK_MENU = 0x12;
                public const int VK_ESCAPE = 0x1B;

                public static bool IsKeyPressed(int KeyCode)
                {
                    return (GetAsyncKeyState(KeyCode) & 0x0800) == 0;
                }
            }

            #endregion

            #region Nested type: WM

            // WM is just a placeholder class for WM (WindowMessage) definitions
            public class WM
            {
                public const int WM_MOUSEMOVE = 0x0200;
                public const int WM_NCMOUSEMOVE = 0x00A0;
                public const int WM_NCLBUTTONDOWN = 0x00A1;
                public const int WM_NCLBUTTONUP = 0x00A2;
                public const int WM_NCLBUTTONDBLCLK = 0x00A3;
                public const int WM_LBUTTONDOWN = 0x0201;
                public const int WM_LBUTTONUP = 0x0202;
                public const int WM_KEYDOWN = 0x0100;
            }

            #endregion
        }

        #endregion

        // Global List of registered StickyWindows
        private static ArrayList s_globalStickyWindows = new ArrayList();

        // public properties
        private static int s_stickGap = 24; // distance to stick

        #region ResizeDir

        private enum ResizeDir
        {
            Top = 2,
            Bottom = 4,
            Left = 8,
            Right = 16
        };

        #endregion

        #region Message Processor

        // Internal Message Processor

        // Messages processors based on type
        private ProcessMessage m_defaultMessageProcessor;
        private ProcessMessage m_messageProcessor;
        private ProcessMessage m_moveMessageProcessor;
        private ProcessMessage m_resizeMessageProcessor;

        private delegate bool ProcessMessage(ref Message m);

        #endregion

        #region Internal properties

        // Move stuff
        private Point m_formOffsetPoint; // calculated offset rect to be added !! (min distances in all directions!!)
        private Rectangle m_formOffsetRect; // calculated rect to fix the size
        private Rectangle m_formOriginalRect; // bounds before last operation started
        private Rectangle m_formRect; // form bounds
        private Point m_mousePoint; // mouse position
        private bool m_movingForm;
        private Point m_offsetPoint; // primary offset

        // General Stuff
        private Form m_originalForm; // the form
        private ResizeDir m_resizeDirection;
        private bool m_resizingForm;

        #endregion

        private bool m_stickOnMove;
        private bool m_stickOnResize;
        private bool m_stickToOther;
        private bool m_stickToScreen;

        #region Public operations and properties

        // Distance in pixels betwen two forms or a form and the screen where the sticking should start
        // Default value = 20
        public int StickGap
        {
            get { return s_stickGap; }
            set { s_stickGap = value; }
        }

        // Allow the form to stick while resizing
        // Default value = true
        public bool StickOnResize
        {
            get { return m_stickOnResize; }
            set { m_stickOnResize = value; }
        }

        // Allow the form to stick while moving
        // Default value = true
        public bool StickOnMove
        {
            get { return m_stickOnMove; }
            set { m_stickOnMove = value; }
        }

        // Allow sticking to Screen Margins
        // Default value = true
        public bool StickToScreen
        {
            get { return m_stickToScreen; }
            set { m_stickToScreen = value; }
        }

        // Allow sticking to other StickWindows
        // Default value = true
        public bool StickToOther
        {
            get { return m_stickToOther; }
            set { m_stickToOther = value; }
        }

        // Register a new form as an external reference form.
        // All Sticky windows will try to stick to the external references
        // Use this to register your MainFrame so the child windows try to stick to it, when your MainFrame is NOT a sticky window
        public static void RegisterExternalReferenceForm(Form frmExternal)
        {
            s_globalStickyWindows.Add(frmExternal);
        }

        // Unregister a form from the external references.
        public static void UnregisterExternalReferenceForm(Form frmExternal)
        {
            s_globalStickyWindows.Remove(frmExternal);
        }

        #endregion

        public int OffectX { get; set; }
        public int OffectY { get; set; }

        public StickyWindow(Form form)
        {
            m_resizingForm = false;
            m_movingForm = false;

            m_originalForm = form;

            m_formRect = Rectangle.Empty;
            m_formOffsetRect = Rectangle.Empty;

            m_formOffsetPoint = Point.Empty;
            m_offsetPoint = Point.Empty;
            m_mousePoint = Point.Empty;

            m_stickOnMove = true;
            m_stickOnResize = true;
            m_stickToScreen = true;
            m_stickToOther = true;

            m_defaultMessageProcessor = DefaultMsgProcessor;
            m_moveMessageProcessor = MoveMsgProcessor;
            m_resizeMessageProcessor = ResizeMsgProcessor;
            m_messageProcessor = m_defaultMessageProcessor;

            AssignHandle(m_originalForm.Handle);
        }

        #region OnHandleChange

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void OnHandleChange()
        {
            if ((int)Handle != 0)
            {
                s_globalStickyWindows.Add(m_originalForm);
            }
            else
            {
                s_globalStickyWindows.Remove(m_originalForm);
            }
        }

        #endregion

        #region WndProc

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            if (!m_messageProcessor(ref m))
                base.WndProc(ref m);
        }

        #endregion

        #region DefaultMsgProcessor

        // Processes messages during normal operations (while the form is not resized or moved)
        private bool DefaultMsgProcessor(ref Message m)
        {
            switch (m.Msg)
            {
                case MyWin32.WM.WM_NCLBUTTONDOWN:
                    {
                        m_originalForm.Activate();
                        m_mousePoint.X = (short)MyWin32.Bit.LoWord((int)m.LParam);
                        m_mousePoint.Y = (short)MyWin32.Bit.HiWord((int)m.LParam);

                        if (OnNCLButtonDown((int)m.WParam, m_mousePoint))
                        {
                            m.Result = (IntPtr)((m_resizingForm || m_movingForm) ? 1 : 0);
                            return true;
                        }

                        break;
                    }
            }

            return false;
        }

        #endregion

        #region OnNCLButtonDown

        // Checks where the click was in the NC area and starts move or resize operation
        private bool OnNCLButtonDown(int iHitTest, Point point)
        {
            Rectangle _rParent = m_originalForm.Bounds;
            m_offsetPoint = point;

            switch (iHitTest)
            {
                case MyWin32.HT.HTCAPTION:
                    {
                        // request for move
                        if (m_stickOnMove)
                        {
                            //@ m_offsetPoint.Offset(-_rParent.Left, -_rParent.Top);

                            StartMove();
                            return true;
                        }
                        else
                            return false; // leave default processing
                    }

                // requests for resize
                case MyWin32.HT.HTTOPLEFT:
                    return StartResize(ResizeDir.Top | ResizeDir.Left);
                case MyWin32.HT.HTTOP:
                    return StartResize(ResizeDir.Top);
                case MyWin32.HT.HTTOPRIGHT:
                    return StartResize(ResizeDir.Top | ResizeDir.Right);
                case MyWin32.HT.HTRIGHT:
                    return StartResize(ResizeDir.Right);
                case MyWin32.HT.HTBOTTOMRIGHT:
                    return StartResize(ResizeDir.Bottom | ResizeDir.Right);
                case MyWin32.HT.HTBOTTOM:
                    return StartResize(ResizeDir.Bottom);
                case MyWin32.HT.HTBOTTOMLEFT:
                    return StartResize(ResizeDir.Bottom | ResizeDir.Left);
                case MyWin32.HT.HTLEFT:
                    return StartResize(ResizeDir.Left);
            }

            return false;
        }

        #endregion

        #region ResizeOperations

        private bool StartResize(ResizeDir resDir)
        {
            if (m_stickOnResize)
            {
                m_resizeDirection = resDir;
                m_formRect = m_originalForm.Bounds;
                m_formOriginalRect = m_originalForm.Bounds; // save the old bounds

                if (!m_originalForm.Capture) // start capturing messages
                    m_originalForm.Capture = true;

                m_messageProcessor = m_resizeMessageProcessor;

                return true; // catch the message
            }
            else
                return false; // leave default processing !
        }

        private bool ResizeMsgProcessor(ref Message m)
        {
            if (!m_originalForm.Capture)
            {
                Cancel();
                return false;
            }

            switch (m.Msg)
            {
                case MyWin32.WM.WM_LBUTTONUP:
                    {
                        // ok, resize finished !!!
                        EndResize();
                        break;
                    }
                case MyWin32.WM.WM_MOUSEMOVE:
                    {
                        m_mousePoint.X = (short)MyWin32.Bit.LoWord((int)m.LParam);
                        m_mousePoint.Y = (short)MyWin32.Bit.HiWord((int)m.LParam);
                        Resize(m_mousePoint);
                        break;
                    }
                case MyWin32.WM.WM_KEYDOWN:
                    {
                        if ((int)m.WParam == MyWin32.VK.VK_ESCAPE)
                        {
                            m_originalForm.Bounds = m_formOriginalRect; // set back old size
                            Cancel();
                        }

                        break;
                    }
            }

            return false;
        }

        private void EndResize()
        {
            Cancel();
        }

        #endregion

        #region Resize Computing

        private void Resize(Point p)
        {
            p = m_originalForm.PointToScreen(p);
            Screen _activeScr = Screen.FromPoint(p);
            m_formRect = m_originalForm.Bounds;

            int _iRight = m_formRect.Right;
            int _iBottom = m_formRect.Bottom;

            // no normalize required
            // first strech the window to the new position
            if ((m_resizeDirection & ResizeDir.Left) == ResizeDir.Left)
            {
                m_formRect.Width = m_formRect.X - p.X + m_formRect.Width;
                m_formRect.X = _iRight - m_formRect.Width;
            }

            if ((m_resizeDirection & ResizeDir.Right) == ResizeDir.Right)
                m_formRect.Width = p.X - m_formRect.Left;

            if ((m_resizeDirection & ResizeDir.Top) == ResizeDir.Top)
            {
                m_formRect.Height = m_formRect.Height - p.Y + m_formRect.Top;
                m_formRect.Y = _iBottom - m_formRect.Height;
            }

            if ((m_resizeDirection & ResizeDir.Bottom) == ResizeDir.Bottom)
                m_formRect.Height = p.Y - m_formRect.Top;

            // this is the real new position
            // now, try to snap it to different objects (first to screen)

            // CARE !!! We use "Width" and "Height" as Bottom & Right!! (C++ style)
            //formOffsetRect = new Rectangle ( stickGap + 1, stickGap + 1, 0, 0 );
            m_formOffsetRect.X = s_stickGap + 1;
            m_formOffsetRect.Y = s_stickGap + 1;
            m_formOffsetRect.Height = 0;
            m_formOffsetRect.Width = 0;

            if (m_stickToScreen)
                Resize_Stick(_activeScr.WorkingArea, false);

            if (m_stickToOther)
            {
                // now try to stick to other forms
                foreach (Form sw in s_globalStickyWindows)
                {
                    if (sw != m_originalForm)
                        Resize_Stick(sw.Bounds, true);
                }
            }

            // Fix (clear) the values that were not updated to stick
            if (m_formOffsetRect.X == s_stickGap + 1)
                m_formOffsetRect.X = 0;

            if (m_formOffsetRect.Width == s_stickGap + 1)
                m_formOffsetRect.Width = 0;

            if (m_formOffsetRect.Y == s_stickGap + 1)
                m_formOffsetRect.Y = 0;

            if (m_formOffsetRect.Height == s_stickGap + 1)
                m_formOffsetRect.Height = 0;

            // compute the new form size
            if ((m_resizeDirection & ResizeDir.Left) == ResizeDir.Left)
            {
                // left resize requires special handling of X & Width acording to MinSize and MinWindowTrackSize
                int iNewWidth = m_formRect.Width + m_formOffsetRect.Width + m_formOffsetRect.X;

                if (m_originalForm.MaximumSize.Width != 0)
                    iNewWidth = Math.Min(iNewWidth, m_originalForm.MaximumSize.Width);

                iNewWidth = Math.Min(iNewWidth, SystemInformation.MaxWindowTrackSize.Width);
                iNewWidth = Math.Max(iNewWidth, m_originalForm.MinimumSize.Width);
                iNewWidth = Math.Max(iNewWidth, SystemInformation.MinWindowTrackSize.Width);

                m_formRect.X = _iRight - iNewWidth;
                m_formRect.Width = iNewWidth;
            }
            else
            {
                // other resizes
                m_formRect.Width += m_formOffsetRect.Width + m_formOffsetRect.X;
            }

            if ((m_resizeDirection & ResizeDir.Top) == ResizeDir.Top)
            {
                int _iNewHeight = m_formRect.Height + m_formOffsetRect.Height + m_formOffsetRect.Y;

                if (m_originalForm.MaximumSize.Height != 0)
                    _iNewHeight = Math.Min(_iNewHeight, m_originalForm.MaximumSize.Height);

                _iNewHeight = Math.Min(_iNewHeight, SystemInformation.MaxWindowTrackSize.Height);
                _iNewHeight = Math.Max(_iNewHeight, m_originalForm.MinimumSize.Height);
                _iNewHeight = Math.Max(_iNewHeight, SystemInformation.MinWindowTrackSize.Height);

                m_formRect.Y = _iBottom - _iNewHeight;
                m_formRect.Height = _iNewHeight;
            }
            else
            {
                // all other resizing are fine 
                m_formRect.Height += m_formOffsetRect.Height + m_formOffsetRect.Y;
            }

            // Done !!
            m_originalForm.Bounds = m_formRect;
        }

        private void Resize_Stick(Rectangle toRect, bool bInsideStick)
        {
            if (m_formRect.Right >= (toRect.Left - s_stickGap) && m_formRect.Left <= (toRect.Right + s_stickGap))
            {
                if ((m_resizeDirection & ResizeDir.Top) == ResizeDir.Top)
                {
                    if (Math.Abs(m_formRect.Top - toRect.Bottom) <= Math.Abs(m_formOffsetRect.Top) && bInsideStick)
                        m_formOffsetRect.Y = m_formRect.Top - toRect.Bottom; // snap top to bottom
                    else if (Math.Abs(m_formRect.Top - toRect.Top) <= Math.Abs(m_formOffsetRect.Top))
                        m_formOffsetRect.Y = m_formRect.Top - toRect.Top; // snap top to top
                }

                if ((m_resizeDirection & ResizeDir.Bottom) == ResizeDir.Bottom)
                {
                    if (Math.Abs(m_formRect.Bottom - toRect.Top) <= Math.Abs(m_formOffsetRect.Bottom) && bInsideStick)
                        m_formOffsetRect.Height = toRect.Top - m_formRect.Bottom; // snap Bottom to top
                    else if (Math.Abs(m_formRect.Bottom - toRect.Bottom) <= Math.Abs(m_formOffsetRect.Bottom))
                        m_formOffsetRect.Height = toRect.Bottom - m_formRect.Bottom; // snap bottom to bottom
                }
            }

            if (m_formRect.Bottom >= (toRect.Top - s_stickGap) && m_formRect.Top <= (toRect.Bottom + s_stickGap))
            {
                if ((m_resizeDirection & ResizeDir.Right) == ResizeDir.Right)
                {
                    if (Math.Abs(m_formRect.Right - toRect.Left) <= Math.Abs(m_formOffsetRect.Right) && bInsideStick)
                        m_formOffsetRect.Width = toRect.Left - m_formRect.Right; // Stick right to left
                    else if (Math.Abs(m_formRect.Right - toRect.Right) <= Math.Abs(m_formOffsetRect.Right))
                        m_formOffsetRect.Width = toRect.Right - m_formRect.Right; // Stick right to right
                }

                if ((m_resizeDirection & ResizeDir.Left) == ResizeDir.Left)
                {
                    if (Math.Abs(m_formRect.Left - toRect.Right) <= Math.Abs(m_formOffsetRect.Left) && bInsideStick)
                        m_formOffsetRect.X = m_formRect.Left - toRect.Right; // Stick left to right
                    else if (Math.Abs(m_formRect.Left - toRect.Left) <= Math.Abs(m_formOffsetRect.Left))
                        m_formOffsetRect.X = m_formRect.Left - toRect.Left; // Stick left to left
                }
            }
        }

        #endregion

        #region Move Operations

        private void StartMove()
        {
            m_formRect = m_originalForm.Bounds;
            m_formOriginalRect = m_originalForm.Bounds; // save original position

            if (!m_originalForm.Capture) // start capturing messages
                m_originalForm.Capture = true;

            m_messageProcessor = m_moveMessageProcessor;
        }

        private bool MoveMsgProcessor(ref Message m)
        {
            // internal message loop
            if (!m_originalForm.Capture)
            {
                Cancel();
                return false;
            }

            switch (m.Msg)
            {
                case MyWin32.WM.WM_LBUTTONUP:
                    {
                        // ok, move finished !!!
                        EndMove();
                        break;
                    }
                case MyWin32.WM.WM_MOUSEMOVE:
                    {
                        m_mousePoint.X = (short)MyWin32.Bit.LoWord((int)m.LParam) - OffectX;
                        m_mousePoint.Y = (short)MyWin32.Bit.HiWord((int)m.LParam) - OffectY;
                        Move(m_mousePoint);
                        break;
                    }
                case MyWin32.WM.WM_KEYDOWN:
                    {
                        if ((int)m.WParam == MyWin32.VK.VK_ESCAPE)
                        {
                            m_originalForm.Bounds = m_formOriginalRect; // set back old size
                            Cancel();
                        }

                        break;
                    }
            }

            return false;
        }

        private void EndMove()
        {
            Cancel();
        }

        #endregion

        #region Move Computing

        private void Move(Point p)
        {
            p = m_originalForm.PointToScreen(p);
            Screen _activeScr = Screen.FromPoint(p); // get the screen from the point !!

            if (!_activeScr.WorkingArea.Contains(p))
            {
                p.X = NormalizeInside(p.X, _activeScr.WorkingArea.Left, _activeScr.WorkingArea.Right);
                p.Y = NormalizeInside(p.Y, _activeScr.WorkingArea.Top, _activeScr.WorkingArea.Bottom);
            }

            p.Offset(-m_offsetPoint.X, -m_offsetPoint.Y);

            // p is the exact location of the frame - so we can play with it
            // to detect the new position acording to different bounds
            m_formRect.Location = p; // this is the new positon of the form

            m_formOffsetPoint.X = s_stickGap + 1; // (more than) maximum gaps
            m_formOffsetPoint.Y = s_stickGap + 1;

            if (m_stickToScreen)
                Move_Stick(_activeScr.WorkingArea, false);

            // Now try to snap to other windows
            if (m_stickToOther)
            {
                foreach (Form sw in s_globalStickyWindows)
                {
                    if (sw != m_originalForm)
                        Move_Stick(sw.Bounds, true);
                }
            }

            if (m_formOffsetPoint.X == s_stickGap + 1)
                m_formOffsetPoint.X = 0;

            if (m_formOffsetPoint.Y == s_stickGap + 1)
                m_formOffsetPoint.Y = 0;

            m_formRect.Offset(m_formOffsetPoint);

            m_originalForm.Bounds = m_formRect;
        }

        private void Move_Stick(Rectangle toRect, bool bInsideStick)
        {
            // compare distance from toRect to formRect
            // and then with the found distances, compare the most closed position
            if (m_formRect.Bottom >= (toRect.Top - s_stickGap) && m_formRect.Top <= (toRect.Bottom + s_stickGap))
            {
                if (bInsideStick)
                {
                    if ((Math.Abs(m_formRect.Left - toRect.Right) <= Math.Abs(m_formOffsetPoint.X)))
                    {
                        // left 2 right
                        m_formOffsetPoint.X = toRect.Right - m_formRect.Left;
                    }

                    if ((Math.Abs(m_formRect.Left + m_formRect.Width - toRect.Left) <= Math.Abs(m_formOffsetPoint.X)))
                    {
                        // right 2 left
                        m_formOffsetPoint.X = toRect.Left - m_formRect.Width - m_formRect.Left;
                    }
                }

                if (Math.Abs(m_formRect.Left - toRect.Left) <= Math.Abs(m_formOffsetPoint.X))
                {
                    // snap left 2 left
                    m_formOffsetPoint.X = toRect.Left - m_formRect.Left;
                }

                if (Math.Abs(m_formRect.Left + m_formRect.Width - toRect.Left - toRect.Width) <= Math.Abs(m_formOffsetPoint.X))
                {
                    // snap right 2 right
                    m_formOffsetPoint.X = toRect.Left + toRect.Width - m_formRect.Width - m_formRect.Left;
                }
            }

            if (m_formRect.Right >= (toRect.Left - s_stickGap) && m_formRect.Left <= (toRect.Right + s_stickGap))
            {
                if (bInsideStick)
                {
                    if (Math.Abs(m_formRect.Top - toRect.Bottom) <= Math.Abs(m_formOffsetPoint.Y) && bInsideStick)
                    {
                        // Stick Top to Bottom
                        m_formOffsetPoint.Y = toRect.Bottom - m_formRect.Top;
                    }

                    if (Math.Abs(m_formRect.Top + m_formRect.Height - toRect.Top) <= Math.Abs(m_formOffsetPoint.Y) && bInsideStick)
                    {
                        // snap Bottom to Top
                        m_formOffsetPoint.Y = toRect.Top - m_formRect.Height - m_formRect.Top;
                    }
                }

                // try to snap top 2 top also
                if (Math.Abs(m_formRect.Top - toRect.Top) <= Math.Abs(m_formOffsetPoint.Y))
                {
                    // top 2 top
                    m_formOffsetPoint.Y = toRect.Top - m_formRect.Top;
                }

                if (Math.Abs(m_formRect.Top + m_formRect.Height - toRect.Top - toRect.Height) <= Math.Abs(m_formOffsetPoint.Y))
                {
                    // bottom 2 bottom
                    m_formOffsetPoint.Y = toRect.Top + toRect.Height - m_formRect.Height - m_formRect.Top;
                }
            }
        }

        #endregion

        #region Utilities

        private int NormalizeInside(int iP1, int iM1, int iM2)
        {
            if (iP1 <= iM1)
                return iM1;
            else if (iP1 >= iM2)
                return iM2;

            return iP1;
        }

        #endregion

        #region Cancel

        private void Cancel()
        {
            m_originalForm.Capture = false;
            m_movingForm = false;
            m_resizingForm = false;
            m_messageProcessor = m_defaultMessageProcessor;
        }

        #endregion
    }
}