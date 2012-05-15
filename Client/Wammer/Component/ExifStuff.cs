#region

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

#endregion

namespace Waveface.Component
{
    public static class ExifStuff
    {
        // Orientations.
        private const int OrientationId = 0x0112;

        public enum ExifOrientations : byte
        {
            Unknown = 0,
            TopLeft = 1,
            TopRight = 2,
            BottomRight = 3,
            BottomLeft = 4,
            LeftTop = 5,
            RightTop = 6,
            RightBottom = 7,
            LeftBottom = 8,
        }

        // Return the image's orientation.
        public static ExifOrientations ImageOrientation(Image img)
        {
            // Get the index of the orientation property.
            int _orientationIndex = Array.IndexOf(img.PropertyIdList, OrientationId);

            // If there is no such property, return Unknown.
            if (_orientationIndex < 0) return ExifOrientations.Unknown;

            // Return the orientation value.
            return (ExifOrientations) img.GetPropertyItem(OrientationId).Value[0];
        }

        // Orient the image properly.
        public static void OrientImage(Image img)
        {
            // Get the image's orientation.
            ExifOrientations _orientation = ImageOrientation(img);

            // Orient the image.
            switch (_orientation)
            {
                case ExifOrientations.Unknown:
                case ExifOrientations.TopLeft:
                    break;

                case ExifOrientations.TopRight:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;

                case ExifOrientations.BottomRight:
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;

                case ExifOrientations.BottomLeft:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    break;

                case ExifOrientations.LeftTop:
                    img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;

                case ExifOrientations.RightTop:
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;

                case ExifOrientations.RightBottom:
                    img.RotateFlip(RotateFlipType.Rotate90FlipY);
                    break;

                case ExifOrientations.LeftBottom:
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            // Set the image's orientation to TopLeft.
            SetImageOrientation(img, ExifOrientations.TopLeft);
        }

        // Set the image's orientation.
        public static void SetImageOrientation(Image img, ExifOrientations orientation)
        {
            // Get the index of the orientation property.
            int _orientationIndex = Array.IndexOf(img.PropertyIdList, OrientationId);

            // If there is no such property, do nothing.
            if (_orientationIndex < 0) return;

            // Set the orientation value.
            PropertyItem _item = img.GetPropertyItem(OrientationId);
            _item.Value[0] = (byte) orientation;
            img.SetPropertyItem(_item);
        }

        // Make an image to demonstrate orientations.
        public static Image OrientationImage(ExifOrientations orientation)
        {
            const int size = 64;
            Bitmap _bm = new Bitmap(size, size);

            using (Graphics _g = Graphics.FromImage(_bm))
            {
                _g.Clear(Color.White);
                _g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                // Orient the result.
                switch (orientation)
                {
                    case ExifOrientations.TopLeft:
                        break;

                    case ExifOrientations.TopRight:
                        _g.ScaleTransform(-1, 1);
                        break;

                    case ExifOrientations.BottomRight:
                        _g.RotateTransform(180);
                        break;

                    case ExifOrientations.BottomLeft:
                        _g.ScaleTransform(1, -1);
                        break;

                    case ExifOrientations.LeftTop:
                        _g.RotateTransform(90);
                        _g.ScaleTransform(-1, 1, MatrixOrder.Append);
                        break;

                    case ExifOrientations.RightTop:
                        _g.RotateTransform(-90);
                        break;

                    case ExifOrientations.RightBottom:
                        _g.RotateTransform(90);
                        _g.ScaleTransform(1, -1, MatrixOrder.Append);
                        break;

                    case ExifOrientations.LeftBottom:
                        _g.RotateTransform(90);
                        break;
                }

                // Translate the result to the center of the bitmap.
                _g.TranslateTransform(size/2, size/2, MatrixOrder.Append);

                using (StringFormat _stringFormat = new StringFormat())
                {
                    _stringFormat.LineAlignment = StringAlignment.Center;
                    _stringFormat.Alignment = StringAlignment.Center;

                    using (Font _font = new Font("Times New Roman", 40, GraphicsUnit.Point))
                    {
                        if (orientation == ExifOrientations.Unknown)
                        {
                            _g.DrawString("?", _font, Brushes.Black, 0, 0, _stringFormat);
                        }
                        else
                        {
                            _g.DrawString("F", _font, Brushes.Black, 0, 0, _stringFormat);
                        }
                    }
                }
            }

            return _bm;
        }
    }
}