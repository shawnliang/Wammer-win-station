#region

using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

#endregion

namespace Waveface.Component.DropableNotifyIcon
{
    internal class MouseHoldTimed
    {
        private Timer m_mouseDownTimer = new Timer();
        private MouseButtons m_mouseHoldWhich;

        public MouseHoldTimed(int duration)
        {
            // Detecting mouse down/up events
            FormDrop.MouseHook.MouseDown += MouseHook_MouseDown;
            FormDrop.MouseHook.MouseUp += MouseHook_MouseUp;

            // The mouse down timer
            m_mouseDownTimer.Stop();
            m_mouseDownTimer.Interval = duration;
            m_mouseDownTimer.Elapsed += mouseDownTimer_Elapsed;
        }

        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseHoldTimeout;

        public void Cancel()
        {
            m_mouseDownTimer.Stop();
        }

        private void mouseDownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_mouseDownTimer.Stop();

            // TODO_ detect scroll wheel clicks (last param of MouseEventArgs)
            if (MouseHoldTimeout != null)
                MouseHoldTimeout(this, new MouseEventArgs(m_mouseHoldWhich, 1, Cursor.Position.X, Cursor.Position.Y, 0));
        }

        private void MouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            m_mouseHoldWhich = e.Button;
            m_mouseDownTimer.Start();

            if (MouseDown != null)
                MouseDown(this, e);
        }

        private void MouseHook_MouseUp(object sender, MouseEventArgs e)
        {
            m_mouseHoldWhich = MouseButtons.None;
            m_mouseDownTimer.Stop();

            if (MouseUp != null)
                MouseUp(this, e);
        }
    }
}