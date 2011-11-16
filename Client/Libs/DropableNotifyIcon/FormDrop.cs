
#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.DropableNotifyIcon
{
    internal partial class FormDrop : Form
    {
        public static MouseHook MouseHook { get; set; }

        private DropableNotifyIcon m_owner;
        private bool m_mouseLeftDown;
        private Point m_lastNotifyIconPoint = new Point(-1, -1);
        private Timer m_taskbarAutoHideTimer = new Timer();
        private bool m_mouseWasInTaskbar;

        static FormDrop()
        {
            MouseHook = new MouseHook();
            MouseHook.Start();
        }

        private static readonly StaticDestructor s_sd = new StaticDestructor();

        private class StaticDestructor
        {
            ~StaticDestructor()
            {
                MouseHook.Stop();
            }
        }

        public FormDrop(DropableNotifyIcon owner)
        {
            InitializeComponent();

            m_owner = owner;

            Visible = false;

            // Keeping on top of things
            ControlBox = false;
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Text = "";

            Deactivate += (sender2, e2) => Activate();
            VisibleChanged += (sender2, e2) => WindowState = FormWindowState.Normal;
            SizeChanged += (sender2, e2) => WindowState = FormWindowState.Normal;

            // Obviously we don't want to be obvious! (Obviously.)
            ShowInTaskbar = false;

            Opacity = 0.01; //@

            // Drop support
            AllowDrop = true;
            DragLeave += (sender2, e2) => TopMost = false;
            MouseEnter += (sender2, e2) => TopMost = false;
            MouseLeave += (sender2, e2) => TopMost = false;

            // Drop events
            DragDrop += m_owner.HandleDragDrop;
            DragEnter += m_owner.HandleDragEnter;
            DragLeave += m_owner.HandleDragLeave;
            DragOver += m_owner.HandleDragOver;

            // Whether the left mouse button is down
            MouseHook.MouseDown += (sender2, e2) => m_mouseLeftDown = e2.Button == MouseButtons.Left;
            MouseHook.MouseUp += (sender2, e2) => m_mouseLeftDown = false;

            // And now to initialise the behaviour...
            Init();

            ShowDrop();
        }

        private bool m_mouseWasInNotifyArea;

        private void Init()
        {
            // Does the owner have an icon set?
            if (m_owner.NotifyIcon.Icon == null)
            {
                throw new InvalidOperationException(
                    "SuperNotifyIcon: Dropping cannot be initialised without an icon set!");
            }

            // When the mouse is close
            MouseHook.MouseMove += MouseHook_MouseMove;

            // Cancel the drop refreshing below if we do an actual click on the NotifyIcon
            m_owner.NotifyIcon.MouseUp += (sender, e) => { m_mouseWasInNotifyArea = false; };

            // Refresh the drop position if we click in the notification area on Windows 7; we might've moved an icon!
            if (OperatingSystem.GteWindows7)
            {
                MouseHook.MouseDown += (sender, e) =>
                                           {
                                               m_mouseWasInNotifyArea = MouseInNotifyArea();

                                               // Shall we cancel, then?
                                               if (e.Button != MouseButtons.Left)
                                               {
                                                   m_mouseWasInNotifyArea = false;
                                               }
                                           };
                MouseHook.MouseUp += (sender, e) =>
                                         {
                                             if (MouseInNotifyArea() && m_mouseWasInNotifyArea)
                                             {
                                                 // We should wait for the icon to settle in before doing anything
                                                 Timer _wait = new Timer();
                                                 _wait.Tick += (sender2, e2) =>
                                                                  {
                                                                      if (m_mouseWasInNotifyArea)
                                                                          ShowDrop();
                                                                      m_mouseWasInNotifyArea = false;
                                                                      _wait.Dispose();
                                                                  };
                                                 _wait.Interval = 200;
                                                 _wait.Start();
                                             }
                                         };
            }

            // Refresh the drop position if the size of the notification area changes
            Size _notifyAreaLastSize = NotifyArea.GetRectangle().Size;
            Timer _notifyAreaTimer = new Timer();
            _notifyAreaTimer.Tick += (sender, e) =>
                                        {
                                            if (NotifyArea.GetRectangle().Size != _notifyAreaLastSize)
                                            {
                                                _notifyAreaLastSize = NotifyArea.GetRectangle().Size;
                                                ShowDrop();
                                            }
                                        };
            _notifyAreaTimer.Interval = 1000;
            _notifyAreaTimer.Start();

            // Is the drop even in the right place at all?
            int _unsuccessfulRefreshes = 0;
            Timer _dropPlaceTimer = new Timer();
            _dropPlaceTimer.Tick += (sender, e) =>
                                       {
                                           if (!NotifyArea.GetRectangle().Contains(new Point(Location.X + 2, Location.Y + 2)))
                                           {
                                               ShowDrop();
                                               _unsuccessfulRefreshes++;

                                               // Don't keep refreshing every second if we can't find our icon!
                                               if (_unsuccessfulRefreshes >= 3)
                                                   _dropPlaceTimer.Interval = _unsuccessfulRefreshes * 1000;
                                           }
                                           else
                                           {
                                               _unsuccessfulRefreshes = 0;
                                               _dropPlaceTimer.Interval = 1000;
                                           }
                                       };
            _dropPlaceTimer.Interval = 1000;
            _dropPlaceTimer.Start();

            // Okay... still no success? Let's fall back to the mouse timer...
            //// TODO_ See whether this should only be run on WinXP/Vista systems and whether this should
            //// run even if we have a valid drop position
            MouseHoldTimed _mouseHold = new MouseHoldTimed(500);
            _mouseHold.MouseDown += (sender, e) =>
                                       {
                                           if (e.Button != MouseButtons.Left || OwnApplicationActive() ||
                                               m_lastNotifyIconPoint != new Point(-1, -1))
                                               _mouseHold.Cancel();
                                       };
            _mouseHold.MouseHoldTimeout += (sender, e) =>
                                              {
                                                  if (m_lastNotifyIconPoint == new Point(-1, -1))
                                                      ShowDrop();
                                              };
        }

        private bool OwnApplicationActive()
        {
            // This checks whether any form from another assembly but in the same application is active. We grab the forms here...
            Assembly _currentAssembly = Assembly.GetExecutingAssembly();
            List<string> _formsFromOtherAssemblies = new List<string>();
            
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType().Assembly != _currentAssembly)
                {
                    _formsFromOtherAssemblies.Add(form.GetType().Name);
                }
            }

            // And here's our check!
            return (ActiveForm != null && _formsFromOtherAssemblies.Contains(ActiveForm.GetType().Name));
        }

        private void MouseHook_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_mouseLeftDown && !OwnApplicationActive())
            {
                // Of course we have to be visible!
                if (Visible)
                {
                    // Don't get the form on top until we're close to it
                    if (Math.Sqrt(Math.Pow(Location.X + Size.Width / 2 - Cursor.Position.X, 2)
                                  + Math.Pow(Location.Y + Size.Height / 2 - Cursor.Position.Y, 2)) <= 48)
                    {
                        TopMost = true;
                    }
                    else
                    {
                        TopMost = false;
                    }
                }

                // Autohide stuff
                if (TaskBar.GetTaskBarState() == TaskBar.TaskBarState.AutoHide)
                {
                    if (!MouseInTaskbar())
                    {
                        m_mouseWasInTaskbar = false;
                    }
                    else
                    {
                        if (!m_mouseWasInTaskbar)
                        {
                            m_mouseWasInTaskbar = true;
                            m_taskbarAutoHideTimer.Interval = 750;
                            m_taskbarAutoHideTimer.Tick += (sender2, e2) =>
                                                             {
                                                                 ShowDrop();
                                                                 m_taskbarAutoHideTimer.Stop();
                                                             };
                        }
                    }
                }
            }
        }

        private bool m_firstFind;

        public void ShowDrop()
        {
            // Somehow this can be run even when disposed...
            if (IsDisposed)
                return;

            // Don't do anything if we're out of the taskbar with auto-hide
            if (TaskBar.GetTaskBarState() == TaskBar.TaskBarState.AutoHide
                && !MouseInTaskbar())
            {
                return;
            }

            // Now then...
            m_lastNotifyIconPoint = m_owner.GetLocation();
            Point _notifyIconLocation = m_lastNotifyIconPoint;
           
            Hide();

            // Stuff
            Size _taskbarSize = TaskBar.GetTaskBarSize();
            Point _taskbarLocation = TaskBar.GetTaskBarLocation();

            // If we get (-1, -1), or a screwed up position, then don't bother with 
            if (_notifyIconLocation != new Point(-1, -1))
            {
                // We've got a find, yessiree!
                m_firstFind = true;

                // Anyway, the task at hand; where does our drop zone go?
                switch (TaskBar.GetTaskBarEdge())
                {
                    case TaskBar.TaskBarEdge.Bottom:
                        Top = _taskbarLocation.Y + 2;
                        Left = _notifyIconLocation.X;
                        Width = 24;
                        Height = _taskbarSize.Height;
                        break;
                    case TaskBar.TaskBarEdge.Top:
                        Top = -2;
                        Left = _notifyIconLocation.X;
                        Width = 24;
                        Height = _taskbarSize.Height;
                        break;
                    case TaskBar.TaskBarEdge.Left:
                        Top = _notifyIconLocation.Y;
                        Left = -2;
                        Width = _taskbarSize.Width;
                        Height = 24;
                        break;
                    case TaskBar.TaskBarEdge.Right:
                        Top = _notifyIconLocation.Y;
                        Left = _taskbarLocation.X + 2;
                        Width = _taskbarSize.Width;
                        Height = 24;
                        break;
                }
            }

            // We still want to show again even if we get (-1, -1), but only if we've found it at least once!
            if (m_firstFind)
            {
                // Post-disposal exception horror fix pt.2
                try
                {
                    Show();
                    TopMost = false;
                }
                catch
                {
                }
            }
        }

        private static bool MouseInTaskbar()
        {
            return TaskBar.GetTaskBarRectangle().Contains(Cursor.Position);
        }

        private static bool MouseInNotifyArea()
        {
            return NotifyArea.GetRectangle().Contains(Cursor.Position);
        }
    }
}