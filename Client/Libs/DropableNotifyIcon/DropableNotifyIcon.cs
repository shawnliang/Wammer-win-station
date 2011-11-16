
#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.DropableNotifyIcon
{
    public class DropableNotifyIcon : IDisposable
    {
        public NotifyIcon NotifyIcon { get; set; }

        private string m_text;

        public string Text
        {
            get { return m_text; }
            set
            {
                // Code from http://stackoverflow.com/q/579665/580264#580264
                if (value.Length >= 128)
                    throw new ArgumentOutOfRangeException("ToolTip text must be less than 128 characters long");

                m_text = value;

                Type _t = typeof (NotifyIcon);
                BindingFlags _hidden = BindingFlags.NonPublic | BindingFlags.Instance;
                _t.GetField("text", _hidden).SetValue(NotifyIcon, m_text);
               
                if ((bool) _t.GetField("added", _hidden).GetValue(NotifyIcon))
                    _t.GetMethod("UpdateIcon", _hidden).Invoke(NotifyIcon, new object[] {true});
            }
        }

        public DropableNotifyIcon()
        {
            NotifyIcon = new NotifyIcon();

            m_iconAnimationTimer.Tick += _iconAnimationTimer_Tick;
        }

        #region Disposal

        private bool m_disposed;

        ~DropableNotifyIcon()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this); // The finalise process no longer needs to be run for this
        }

        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (!m_disposed)
            {
                if (disposeManagedResources)
                {
                    try
                    {
                        NotifyIcon.Dispose();
                        m_formDrop.Dispose();
                    }
                    catch
                    {
                    }
                }

                m_disposed = true;
            }
        }

        #endregion

        #region Drop handling

        // Activating the drop is done through this instead of something like a property "AllowDrop" because of the lag
        public void InitDrop()
        {
            m_formDrop = new FormDrop(this);
        }

        //public void InitDrop()
        //{
        //    InitDrop(false);
        //}

        // These don't attach/detach from the events in FormDrop as we should allow attaching and detaching even
        // when FormDrop isn't initialised.
        public event DragEventHandler DragDrop;
        public event DragEventHandler DragEnter;
        public event EventHandler DragLeave;
        public event DragEventHandler DragOver;

        internal void HandleDragDrop(object sender, DragEventArgs e)
        {
            if (DragDrop != null)
                DragDrop(sender, e);
        }

        internal void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (DragEnter != null)
                DragEnter(sender, e);
        }

        internal void HandleDragLeave(object sender, EventArgs e)
        {
            if (DragLeave != null)
                DragLeave(sender, e);
        }

        internal void HandleDragOver(object sender, DragEventArgs e)
        {
            if (DragOver != null)
                DragOver(sender, e);
        }

        #endregion

        #region Location detection

        public Point GetLocation()
        {
            return GetLocation(0);
        }

        public Point GetLocation(int accuracy)
        {
            if (OperatingSystem.GteWindows7)
            {
                return NotifyArea.GetNotifyIconRectangle(NotifyIcon).Location;
            }
            else
            {
                return GetLocationPre7(accuracy);
            }
        }

        #region Pre-Windows 7 fallback

        /**
         * =====================
         * READ THIS, SERIOUSLY.
         * =====================
         * Here be dragons. This bit of the code was coded before coming across Shell_NotifyIconGetRect.
         * Then again, it was introduced in Windows 7. So... expect it to be quite, well, kludgey. Very.
         */

        private static Color s_nearColor;
        private static bool s_nearColorSet;
        private FormDrop m_formDrop;

        public static void GetLocationPre7Prepare()
        {
            // Get a screenshot of the notification area...
            Rectangle _notifyAreaRect = NotifyArea.GetRectangle();
            Size _notifyAreaSize = _notifyAreaRect.Size;

            using (Bitmap notifyAreaBitmap = GetNotifyAreaScreenshot())
            {
                // Something gone wrong? Give up.
                if (notifyAreaBitmap == null)
                    return;

                // Determine a good spot...
                if (_notifyAreaSize.Width > _notifyAreaSize.Height)
                    s_nearColor = notifyAreaBitmap.GetPixel(_notifyAreaSize.Width - 3, _notifyAreaSize.Height/2);
                else
                    s_nearColor = notifyAreaBitmap.GetPixel(_notifyAreaSize.Width/2, _notifyAreaSize.Height - 3);

                // And now we have our base colour!
                s_nearColorSet = true;
            }
        }

        public Point GetLocationPre7(int accuracy)
        {
            // Got something fullscreen running? Of course we can't find our icon!
            if (FullScreen.Detect())
                return new Point(-1, -1);

            // The accuracy can't be below 0!
            if (accuracy < 0)
                throw new ArgumentOutOfRangeException("accuracy", "The accuracy value provided can't be negative!");

            // The notification area
            Rectangle _notifyAreaRect = NotifyArea.GetRectangle();

            // Invalid size? Don't bother doing anything.
            if (_notifyAreaRect.Width < 1 || _notifyAreaRect.Height < 1)
                return new Point(-1, -1);

            // Back up the NotifyIcon's icon so we can reset it later on
            Icon _notifyIconIcon = NotifyIcon.Icon;

            // Have we got a colour we could base the find pixel off?
            if (!s_nearColorSet)
                GetLocationPre7Prepare();

            List<int> _colMatchIndexes = new List<int>();
            Point _last = new Point(-1, -1);
            int _hits = 0;
            int _hitsMax = accuracy + 1;

            // Our wonderful loop
            for (int _attempt = 0; _attempt < 5 && _hits < _hitsMax; _attempt++)
            {
                // Set the notify icon thingy to a random colour
                Random _random = new Random();
                int _rgbRange = 32;
                Color _col;

                if (s_nearColorSet)
                    _col = Color.FromArgb(
                        SafeColourVal(s_nearColor.R + _random.Next(_rgbRange) - 8),
                        SafeColourVal(s_nearColor.G + _random.Next(_rgbRange) - 8),
                        SafeColourVal(s_nearColor.B + _random.Next(_rgbRange) - 8));
                else
                    _col = Color.FromArgb(
                        SafeColourVal(255 - _random.Next(_rgbRange)),
                        SafeColourVal(255 - _random.Next(_rgbRange)),
                        SafeColourVal(255 - _random.Next(_rgbRange)));

                // Set the find colour...
                SetFindColour(_col);

                // Take a screenshot...
                Color[] _taskbarPixels;

                using (Bitmap _notifyAreaBitmap = GetNotifyAreaScreenshot())
                {
                    // If something goes wrong, let's just assume we don't know where we should be
                    if (_notifyAreaBitmap == null)
                        return new Point(-1, -1);

                    // We can reset the NotifyIcon now, and then...
                    NotifyIcon.Icon = _notifyIconIcon;

                    // Grab the pixels of the taskbar using my very own Pfz-derived bitmap to pixel array awesomeness
                    _taskbarPixels = BitmapToPixelArray.From(_notifyAreaBitmap);
                }

                // Get every occurence of our lovely colour next to something the same...
                bool _colMatched = false;
                int _colMatchIndex = -1;
                int _colAttempt = 0; // this determines whether we -1 any of the RGB
                
                while (true)
                {
                    Color _col2 = Color.FromArgb(0, 0, 0);
                    int _colMod1 = (_colAttempt%8) < 4 ? 0 : -1;
                    int _colMod2 = (_colAttempt%8) < 4 ? -1 : 0;

                    switch (_colAttempt%4)
                    {
                        case 0:
                            _col2 = Color.FromArgb(SafeColourVal(_col.R + _colMod1), SafeColourVal(_col.G + _colMod1),
                                                  SafeColourVal(_col.B + _colMod1));
                            break;
                        case 1:
                            _col2 = Color.FromArgb(SafeColourVal(_col.R + _colMod1), SafeColourVal(_col.G + _colMod1),
                                                  SafeColourVal(_col.B + _colMod2));
                            break;
                        case 2:
                            _col2 = Color.FromArgb(SafeColourVal(_col.R + _colMod1), SafeColourVal(_col.G + _colMod2),
                                                  SafeColourVal(_col.B + _colMod1));
                            break;
                        case 3:
                            _col2 = Color.FromArgb(SafeColourVal(_col.R + _colMod1), SafeColourVal(_col.G + _colMod2),
                                                  SafeColourVal(_col.B + _colMod2));
                            break;
                    }

                    _colAttempt++;

                    _colMatchIndex = Array.FindIndex(_taskbarPixels, _colMatchIndex + 1, c => { return c == _col2; });

                    if (_colMatchIndex == -1)
                    {
                        if (_colAttempt < 8)
                            continue;
                        else
                            break;
                    }
                    else
                    {
                        if (_taskbarPixels[_colMatchIndex + 1] == _col2)
                        {
                            _colMatched = true;
                            break;
                        }
                    }
                }

                if (_colMatched)
                {
                    _hits++;
                    _last.X = _colMatchIndex%_notifyAreaRect.Width;
                    _last.Y = _colMatchIndex/_notifyAreaRect.Width; // Integer rounding is always downwards
                }
                else
                {
                    _hits = 0;
                    _last.X = -1;
                    _last.Y = -1;
                }
            }

            // Don't forget, our current values are relative to the notification area and are at the bottom right of the icon!
            Point _location = new Point(_last.X, _last.Y);

            if (_location != new Point(-1, -1))
            {
                _location = new Point(_notifyAreaRect.X + (_last.X - 16), _notifyAreaRect.Y + (_last.Y - 14));
            }

            // And so we return the value now!
            return _location;
        }

        private static Bitmap GetNotifyAreaScreenshot()
        {
            Rectangle _notifyAreaRect = NotifyArea.GetRectangle();
            Bitmap _notifyAreaBitmap = new Bitmap(_notifyAreaRect.Width, _notifyAreaRect.Height);
            
            using (Graphics _notifyAreaGraphics = Graphics.FromImage(_notifyAreaBitmap))
            {
                try
                {
                    _notifyAreaGraphics.CopyFromScreen(_notifyAreaRect.X, _notifyAreaRect.Y, 0, 0, _notifyAreaRect.Size);
                }
                catch (Win32Exception)
                {
                    return null;
                }
            }

            return _notifyAreaBitmap;
        }

        private void SetFindColour(Color col)
        {
            // Grab the notification icon
            Bitmap _iconBitmap = NotifyIcon.Icon.ToBitmap();

            // Draw on it
            Graphics _iconGraphics = Graphics.FromImage(_iconBitmap);
            _iconGraphics.DrawRectangle(new Pen(col, 1), 12, 14, 3, 2);
            
            NotifyIcon.Icon = Icon.FromHandle(_iconBitmap.GetHicon());
            
            _iconGraphics.Dispose();
        }

        private static int SafeColourVal(int val)
        {
            return Math.Min(255, Math.Max(0, val) + 0);
        }

        #endregion

        #endregion

        #region Animation

        private Image m_iconAnimationBase;
        private Image[] m_iconAnimationImages = new Image[] {};
        private int m_iconAnimationFrame;
        private int m_iconAnimationFramesToFade;
        private bool m_iconAnimationOpacityReversing;
        private float m_iconAnimationOpacity;
        private Timer m_iconAnimationTimer = new Timer();

        public void BeginOverlayAnimation(Icon baseIcon, Image[] images, int interval, int framesToFade)
        {
            m_iconAnimationBase = baseIcon.ToBitmap();
            m_iconAnimationImages = images;
            m_iconAnimationFramesToFade = Math.Max(1, framesToFade);
            m_iconAnimationTimer.Interval = interval;
            m_iconAnimationOpacity = 0;
            m_iconAnimationOpacityReversing = false;
            m_iconAnimationFrame = 0;
            m_iconAnimationTimer.Start();
        }

        public void EndAnimation(int framesToFade)
        {
            if (framesToFade > 0)
            {
                m_iconAnimationFramesToFade = framesToFade;
                m_iconAnimationOpacityReversing = true;
            }
            else
            {
                m_iconAnimationTimer.Stop();
            }
        }

        public void _iconAnimationTimer_Tick(object sender, EventArgs e)
        {
            // What frame are we on?
            m_iconAnimationFrame++;

            if (m_iconAnimationFrame == m_iconAnimationImages.Length)
                m_iconAnimationFrame = 0;

            using (Bitmap _bitmap = new Bitmap(16, 16))
            {
                using (Graphics _graphics = Graphics.FromImage(_bitmap))
                {
                    // Opacity!
                    if (!m_iconAnimationOpacityReversing)
                    {
                        if (m_iconAnimationOpacity < 1)
                            m_iconAnimationOpacity = Math.Min(100f, m_iconAnimationOpacity + 1f/m_iconAnimationFramesToFade);
                    }
                    else
                    {
                        if (m_iconAnimationOpacity > 0)
                            m_iconAnimationOpacity = Math.Max(0f, m_iconAnimationOpacity - 1f/m_iconAnimationFramesToFade);

                        // If we're finished, stop the timer!
                        if (m_iconAnimationOpacity == 0)
                            m_iconAnimationTimer.Stop();
                    }

                    ImageAttributes _attributes = new ImageAttributes();

                    _attributes.SetColorMatrix(
                        new ColorMatrix {Matrix33 = m_iconAnimationOpacity},
                        ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    // And now to draw the icon!
                    _graphics.DrawImage(m_iconAnimationBase, 0, 0, 16, 16);
                    _graphics.DrawImage(m_iconAnimationImages[m_iconAnimationFrame],
                                       new Rectangle(0, 0, 16, 16),
                                       0, 0, 16, 16, // Oh dear god this code is ugly
                                       GraphicsUnit.Pixel,
                                       _attributes);
                    NotifyIcon.Icon = Icon.FromHandle(_bitmap.GetHicon());
                }
            }
        }

        #endregion
    }
}