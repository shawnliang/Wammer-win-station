#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Waveface.ImageCapture.Utils;

#endregion

namespace Waveface.ImageCapture.States
{
    internal class WindowState : BaseState
    {
        private readonly List<Rectangle> m_windows = new List<Rectangle>();
        private Rectangle m_rect;

        public WindowState(Form owner)
            : base(owner)
        {
            owner.Cursor = Cursors.Hand;
        }

        #region ICaptureState Members

        public override void Paint(Graphics graphics)
        {
            base.Paint(graphics, m_rect);
        }

        public override void Update(Point location)
        {
            GetAllVisibleWindows();
            m_rect = GetRectFromPoint(location);
            Owner.Invalidate();
        }

        public override Image Capture()
        {
            return base.Capture(m_rect);
        }

        #endregion

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
                throw new Exception(_errorMessage);
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