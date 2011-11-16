#region

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace Waveface.Component
{
    public class MouseWheelRedirector : IMessageFilter
    {
        private const int WM_MOUSEWHEEL = 522;

        private static MouseWheelRedirector s_instance;
        private static bool s_active;

        private Control m_currentControl;

        public static bool Active
        {
            get
            {
                return s_active;
            }
            set
            {
                if (s_active == value)
                    return;

                s_active = value;

                if (s_active)
                {
                    if (s_instance == null)
                    {
                        s_instance = new MouseWheelRedirector();
                    }

                    Application.AddMessageFilter(s_instance);
                }
                else
                {
                    if (s_instance != null)
                    {
                        Application.RemoveMessageFilter(s_instance);
                    }
                }
            }
        }

        private MouseWheelRedirector()
        {
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        #region IMessageFilter Members

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            if ((m_currentControl != null) && (m.Msg == WM_MOUSEWHEEL))
            {
                SendMessage(m_currentControl.Handle, m.Msg, m.WParam, m.LParam);
                return true;
            }

            return false;
        }

        #endregion

        public static void Attach(Control control)
        {
            if (!s_active)
            {
                Active = true;
            }

            control.MouseEnter += s_instance.ControlMouseEnter;
            control.MouseLeave += s_instance.ControlMouseLeaveOrDisposed;
            control.Disposed += s_instance.ControlMouseLeaveOrDisposed;
        }

        public static void Detach(Control control)
        {
            if (s_instance == null)
                return;

            control.MouseEnter -= s_instance.ControlMouseEnter;
            control.MouseLeave -= s_instance.ControlMouseLeaveOrDisposed;
            control.Disposed -= s_instance.ControlMouseLeaveOrDisposed;
        }

        private void ControlMouseEnter(object sender, EventArgs e)
        {
            Control _control = (Control)sender;

            m_currentControl = _control.Focused ? null : _control;
        }

        private void ControlMouseLeaveOrDisposed(object sender, EventArgs e)
        {
            if (m_currentControl == sender)
            {
                m_currentControl = null;
            }
        }
    }
}