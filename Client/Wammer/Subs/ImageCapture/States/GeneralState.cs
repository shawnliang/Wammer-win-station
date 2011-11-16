#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Waveface.ImageCapture.Utils;

#endregion

namespace Waveface.ImageCapture.States
{
    internal class GeneralState : BaseState
    {
        private readonly List<Rectangle> m_windows = new List<Rectangle>();
        private bool m_ctrlPressed;
        private Point m_end;
        private bool m_pressed;
        private Point m_start;

        public GeneralState(Form owner)
            : base(owner)
        {
            owner.KeyDown += OnKeyDown;
            owner.KeyUp += OnKeyDown;
        }

        #region ICaptureState Members

        public override void Paint(Graphics graphics)
        {
            m_end = Cursor.Position;

            if (!m_pressed)
            {
                Screen _currentScreen = Screen.FromPoint(m_end);

                string _label = string.Format(" {0} x {1}", m_end.X - _currentScreen.Bounds.X, m_end.Y - _currentScreen.Bounds.Y);
                DrawTextLabel(_label, m_end, graphics, true);

                if (m_ctrlPressed)
                {
                    //draw cross
                    DrawCross(graphics);
                }
            }
            else
            {
                Rectangle _rect = CalculateRegion(m_start, m_end);
                base.Paint(graphics, _rect);
            }
        }

        public override void Start(Point location)
        {
            m_start = location;
            m_pressed = true;
        }

        public override void End()
        {
            m_pressed = false;
        }

        public override Image Capture()
        {
            Rectangle _shotRect;

            if (m_start == m_end)
            {
                //get window shot
                GetAllVisibleWindows();
                _shotRect = GetRectFromPoint(m_start);
            }
            else
            {
                //get region shot
                _shotRect = CalculateRegion(m_start, m_end);
            }

            return base.Capture(_shotRect);
        }

        #endregion

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            m_ctrlPressed = e.Control;
            Owner.Invalidate();
        }

        private void DrawCross(Graphics graphics)
        {
            using (Pen _pen = new Pen(Color.FromArgb(180, Color.Gray)))
            {
                Point _pos = Cursor.Position;
                graphics.DrawLine(_pen, _pos.X, Owner.Top, _pos.X, Owner.Bottom);
                graphics.DrawLine(_pen, Owner.Left, _pos.Y, Owner.Right, _pos.Y);
            }
        }

        private static Rectangle CalculateRegion(Point start, Point end)
        {
            Rectangle _result = new Rectangle();

            _result.X = Math.Min(start.X, end.X);
            _result.Width = Math.Abs(start.X - end.X);

            _result.Y = Math.Min(start.Y, end.Y);
            _result.Height = Math.Abs(start.Y - end.Y);

            return _result;
        }

        private Rectangle GetRectFromPoint(Point pt)
        {
            foreach (Rectangle _rect in m_windows)
            {
                if (_rect.Contains(pt))
                {
                    _rect.Intersect(Owner.Bounds);
                    return _rect;
                }
            }

            return Rectangle.Empty;
        }

        private void GetAllVisibleWindows()
        {
            User32.EnumDelegate _enumfunc = EnumWindowsProc;
            IntPtr _hDesktop = IntPtr.Zero; // current desktop
            bool _success = User32.EnumDesktopWindows(_hDesktop, _enumfunc, IntPtr.Zero);

            if (!_success)
            {
                // Get the last Win32 error code
                int _errorCode = Marshal.GetLastWin32Error();

                string _errorMessage = String.Format("EnumDesktopWindows failed with code {0}.", _errorCode);
                
                Trace.WriteLine(_errorMessage);
            }
        }

        private bool EnumWindowsProc(IntPtr hWnd, int lParam)
        {
            //исключаем наше окно
            if (Owner.Handle == hWnd)
                return true;

            //окна перечисляются в порядке z-order
            User32.RECT _rect = new User32.RECT();
            User32.GetWindowRect(hWnd, ref _rect);

            if (User32.IsWindowVisible(hWnd))
                m_windows.Add(new Rectangle(_rect.Left, _rect.Top, _rect.Right - _rect.Left, _rect.Bottom - _rect.Top));

            return true;
        }
    }
}