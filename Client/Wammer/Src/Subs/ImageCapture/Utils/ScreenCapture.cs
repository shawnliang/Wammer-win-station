#region

using System;
using System.Drawing;
using System.Drawing.Imaging;

#endregion

namespace Waveface.ImageCapture.Utils
{
    public class ScreenCapture
    {
        public Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }

        public Image CaptureWindow(IntPtr handle)
        {
            // get te hDC of the target window
            IntPtr _hdcSrc = User32.GetWindowDC(handle);

            // get the size
            User32.RECT _windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref _windowRect);
            int _width = _windowRect.Right - _windowRect.Left;
            int _height = _windowRect.Bottom - _windowRect.Top;

            // create a device context we can copy to
            IntPtr _hdcDest = GDI32.CreateCompatibleDC(_hdcSrc);

            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr _hBitmap = GDI32.CreateCompatibleBitmap(_hdcSrc, _width, _height);

            // select the bitmap object
            IntPtr _hOld = GDI32.SelectObject(_hdcDest, _hBitmap);

            // bitblt over
            GDI32.BitBlt(_hdcDest, 0, 0, _width, _height, _hdcSrc, 0, 0, GDI32.SRCCOPY);

            // restore selection
            GDI32.SelectObject(_hdcDest, _hOld);

            // clean up 
            GDI32.DeleteDC(_hdcDest);
            User32.ReleaseDC(handle, _hdcSrc);

            // get a .NET image object for it
            Image _img = Image.FromHbitmap(_hBitmap);

            // free up the Bitmap object
            GDI32.DeleteObject(_hBitmap);

            return _img;
        }

        public Image CaptureRectangle(Rectangle windowRect)
        {
            if ((windowRect.Height <= 0) || windowRect.Width <= 0)
                return null; //throw new ArgumentException("The Rectangle has no size", "windowRect");

            IntPtr _handle = User32.GetDesktopWindow();

            // get te hDC of the target window
            IntPtr _hdcSrc = User32.GetWindowDC(_handle);

            // create a device context we can copy to
            IntPtr _hdcDest = GDI32.CreateCompatibleDC(_hdcSrc);

            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr _hBitmap = GDI32.CreateCompatibleBitmap(_hdcSrc, windowRect.Width, windowRect.Height);
            
            // select the bitmap object
            IntPtr _hOld = GDI32.SelectObject(_hdcDest, _hBitmap);
            
            // bitblt over
            GDI32.BitBlt(_hdcDest, 0, 0, windowRect.Width, windowRect.Height,
                         _hdcSrc, windowRect.Left, windowRect.Top, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(_hdcDest, _hOld);

            // clean up 
            GDI32.DeleteDC(_hdcDest);
            User32.ReleaseDC(_handle, _hdcSrc);

            // get a .NET image object for it
            Image _img = Image.FromHbitmap(_hBitmap);

            // free up the Bitmap object
            GDI32.DeleteObject(_hBitmap);

            return _img;
        }

        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image _img = CaptureWindow(handle);
            _img.Save(filename, format);
        }

        public void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image _img = CaptureScreen();
            _img.Save(filename, format);
        }

        public void CaptureRectangleToFile(Rectangle rect, string filename, ImageFormat format)
        {
            Image _img = CaptureRectangle(rect);
            _img.Save(filename, format);
        }
    }
}