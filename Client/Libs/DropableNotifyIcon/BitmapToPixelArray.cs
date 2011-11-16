/**
 * BitmapToPixelArray.cs
 * Class derived from code by Paulo Zemek
 * http://www.codeproject.com/KB/graphics/ManagedBitmaps.aspx
 */

#region

using System.Drawing;
using System.Drawing.Imaging;

#endregion

namespace Waveface.Component.DropableNotifyIcon
{
    internal static class BitmapToPixelArray
    {
        public static Color[] From(Bitmap bitmap)
        {
            Bitmap _fOriginalSystemBitmap = bitmap;
            Color[] _cols = new Color[_fOriginalSystemBitmap.Size.Width*_fOriginalSystemBitmap.Size.Height];

            BitmapData _sourceData = null;

            {
                _sourceData = _fOriginalSystemBitmap.LockBits(new Rectangle(new Point(), _fOriginalSystemBitmap.Size),
                                                              ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            }

            {
                Size _size = _fOriginalSystemBitmap.Size;

                unsafe
                {
                    byte* _sourceScanlineBytes = (byte*) _sourceData.Scan0;

                    for (int y = 0; y < _size.Height; y++)
                    {
                        int* _sourceScanline = (int*) _sourceScanlineBytes;

                        for (int x = 0; x < _size.Width; x++)
                        {
                            int _color = _sourceScanline[x];
                            int _index = x%_size.Width + y*_size.Width;
                            _cols[_index] = Color.FromArgb((_color >> 16) & 0xFF, (_color >> 8) & 0xFF, _color & 0xFF);
                        }

                        _sourceScanlineBytes += _sourceData.Stride;
                    }
                }
            }

            {
                if (_sourceData != null)
                    _fOriginalSystemBitmap.UnlockBits(_sourceData);
            }

            return _cols;
        }
    }
}