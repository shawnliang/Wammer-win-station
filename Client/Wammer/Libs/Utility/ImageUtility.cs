﻿#region

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Windows.Forms;

#endregion

namespace Waveface
{
    public class ImageUtility
    {
        public static Image GetAvatarImage(string creatorId, string avatarUrl)
        {
            Image _img;

            string _localAvatarPath = MainForm.GCONST.CachePath + creatorId + ".jpg";

            if (System.IO.File.Exists(_localAvatarPath))
            {
                _img = Image.FromFile(_localAvatarPath);
            }
            else
            {
                if (string.IsNullOrEmpty(avatarUrl))
                {
                    return null;
                }
                else
                {
                    WebRequest _wReq = WebRequest.Create(avatarUrl);
                    WebResponse _wRep = _wReq.GetResponse();
                    _img = Image.FromStream(_wRep.GetResponseStream());
                    _img.Save(_localAvatarPath);
                }
            }

            return _img;
        }

        #region RotateImage

        /// <summary>
        /// Creates a new Image containing the same image only rotated
        /// </summary>
        /// <param name="image">The <see cref="System.Drawing.Image"/> to rotate</param>
        /// <param name="angle">The amount to rotate the image, clockwise, in degrees</param>
        /// <returns>A new <see cref="System.Drawing.Bitmap"/> that is just large enough
        /// to contain the rotated image without cutting any corners off.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <see cref="image"/> is null.</exception>
        public static Bitmap RotateImage(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            const double pi2 = Math.PI/2.0;

            // Why can't C# allow these to be const, or at least readonly
            // *sigh*  I'm starting to talk like Christian Graus :omg:
            double oldWidth = image.Width;
            double oldHeight = image.Height;

            // Convert degrees to radians
            double theta = (angle)*Math.PI/180.0;
            double locked_theta = theta;

            // Ensure theta is now [0, 2pi)
            while (locked_theta < 0.0)
                locked_theta += 2*Math.PI;

            #region Explaination of the calculations

            /*
			 * The trig involved in calculating the new width and height
			 * is fairly simple; the hard part was remembering that when 
			 * PI/2 <= theta <= PI and 3PI/2 <= theta < 2PI the width and 
			 * height are switched.
			 * 
			 * When you rotate a rectangle, r, the bounding box surrounding r
			 * contains for right-triangles of empty space.  Each of the 
			 * triangles hypotenuse's are a known length, either the width or
			 * the height of r.  Because we know the length of the hypotenuse
			 * and we have a known angle of rotation, we can use the trig
			 * function identities to find the length of the other two sides.
			 * 
			 * sine = opposite/hypotenuse
			 * cosine = adjacent/hypotenuse
			 * 
			 * solving for the unknown we get
			 * 
			 * opposite = sine * hypotenuse
			 * adjacent = cosine * hypotenuse
			 * 
			 * Another interesting point about these triangles is that there
			 * are only two different triangles. The proof for which is easy
			 * to see, but its been too long since I've written a proof that
			 * I can't explain it well enough to want to publish it.  
			 * 
			 * Just trust me when I say the triangles formed by the lengths 
			 * width are always the same (for a given theta) and the same 
			 * goes for the height of r.
			 * 
			 * Rather than associate the opposite/adjacent sides with the
			 * width and height of the original bitmap, I'll associate them
			 * based on their position.
			 * 
			 * adjacent/oppositeTop will refer to the triangles making up the 
			 * upper right and lower left corners
			 * 
			 * adjacent/oppositeBottom will refer to the triangles making up 
			 * the upper left and lower right corners
			 * 
			 * The names are based on the right side corners, because thats 
			 * where I did my work on paper (the right side).
			 * 
			 * Now if you draw this out, you will see that the width of the 
			 * bounding box is calculated by adding together adjacentTop and 
			 * oppositeBottom while the height is calculate by adding 
			 * together adjacentBottom and oppositeTop.
			 */

            #endregion

            double adjacentTop, oppositeTop;
            double adjacentBottom, oppositeBottom;

            // We need to calculate the sides of the triangles based
            // on how much rotation is being done to the bitmap.
            //   Refer to the first paragraph in the explaination above for 
            //   reasons why.
            if ((locked_theta >= 0.0 && locked_theta < pi2) ||
                (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2)))
            {
                adjacentTop = Math.Abs(Math.Cos(locked_theta))*oldWidth;
                oppositeTop = Math.Abs(Math.Sin(locked_theta))*oldWidth;

                adjacentBottom = Math.Abs(Math.Cos(locked_theta))*oldHeight;
                oppositeBottom = Math.Abs(Math.Sin(locked_theta))*oldHeight;
            }
            else
            {
                adjacentTop = Math.Abs(Math.Sin(locked_theta))*oldHeight;
                oppositeTop = Math.Abs(Math.Cos(locked_theta))*oldHeight;

                adjacentBottom = Math.Abs(Math.Sin(locked_theta))*oldWidth;
                oppositeBottom = Math.Abs(Math.Cos(locked_theta))*oldWidth;
            }

            double newWidth = adjacentTop + oppositeBottom;
            double newHeight = adjacentBottom + oppositeTop;

            int nWidth = (int) Math.Ceiling(newWidth);
            int nHeight = (int) Math.Ceiling(newHeight);

            Bitmap rotatedBmp = new Bitmap(nWidth, nHeight);

            using (Graphics g = Graphics.FromImage(rotatedBmp))
            {
                // This array will be used to pass in the three points that 
                // make up the rotated image
                Point[] points;

                /*
                 * The values of opposite/adjacentTop/Bottom are referring to 
                 * fixed locations instead of in relation to the
                 * rotating image so I need to change which values are used
                 * based on the how much the image is rotating.
                 * 
                 * For each point, one of the coordinates will always be 0, 
                 * nWidth, or nHeight.  This because the Bitmap we are drawing on
                 * is the bounding box for the rotated bitmap.  If both of the 
                 * corrdinates for any of the given points wasn't in the set above
                 * then the bitmap we are drawing on WOULDN'T be the bounding box
                 * as required.
                 */
                if (locked_theta >= 0.0 && locked_theta < pi2)
                {
                    points = new[]
                                 {
                                     new Point((int) oppositeBottom, 0),
                                     new Point(nWidth, (int) oppositeTop),
                                     new Point(0, (int) adjacentBottom)
                                 };
                }
                else if (locked_theta >= pi2 && locked_theta < Math.PI)
                {
                    points = new[]
                                 {
                                     new Point(nWidth, (int) oppositeTop),
                                     new Point((int) adjacentTop, nHeight),
                                     new Point((int) oppositeBottom, 0)
                                 };
                }
                else if (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2))
                {
                    points = new[]
                                 {
                                     new Point((int) adjacentTop, nHeight),
                                     new Point(0, (int) adjacentBottom),
                                     new Point(nWidth, (int) oppositeTop)
                                 };
                }
                else
                {
                    points = new[]
                                 {
                                     new Point(0, (int) adjacentBottom),
                                     new Point((int) oppositeBottom, 0),
                                     new Point((int) adjacentTop, nHeight)
                                 };
                }

                g.DrawImage(image, points);
            }

            return rotatedBmp;
        }

        #endregion

        public static Bitmap MakeGrayscale(Bitmap original)
        {
            //make an empty bitmap the same size as original
            Bitmap _ret = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    //get the pixel from the original image
                    Color _originalColor = original.GetPixel(i, j);

                    //create the grayscale version of the pixel
                    int _grayScale = (int)((_originalColor.R * .3) + (_originalColor.G * .59)
                        + (_originalColor.B * .11));

                    //create the color object
                    Color _newColor = Color.FromArgb(_grayScale, _grayScale, _grayScale);

                    //set the new image's pixel to the grayscale version
                    _ret.SetPixel(i, j, _newColor);
                }
            }

            return _ret;
        }

        #region Resize Image

        public static string ResizeImage(string orgImageFilePath, string fileName, string resizeRatio)
        {
            string _resize = resizeRatio;

            if (_resize.Equals(string.Empty) || _resize.Equals("100%"))
                return orgImageFilePath;

            int _longestSide = int.Parse(_resize);

            try
            {
                Bitmap _bmp = ResizeImage_Impl(orgImageFilePath, _longestSide);

                if (_bmp == null)
                    return orgImageFilePath;

                string _newPath = MainForm.GCONST.TempPath + DateTime.Now.ToString("yyyyMMddHHmmssff") + "_" +
                                  fileName;

                _bmp.Save(_newPath);

                return _newPath;
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.Message);
            }

            return orgImageFilePath;
        }

        private static Bitmap ResizeImage_Impl(string imagePath, int longestSide)
        {
            Bitmap _newImage = null;
            Bitmap _image = null;

            try
            {
                _image = new Bitmap(imagePath);

                float _scale = (_image.Width > _image.Height
                                    ? (longestSide)/((float) _image.Width)
                                    : (longestSide)/((float) _image.Height));

                int _width = (int) (_image.Width*_scale);
                int _height = (int) (_image.Height*_scale);
                _newImage = new Bitmap(_width, _height);

                Graphics _g = Graphics.FromImage(_newImage);
                _g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                _g.DrawImage(_image, new Rectangle(0, 0, _width, _height), 0, 0, _image.Width, _image.Height,
                             GraphicsUnit.Pixel);
                _g.Dispose();
            }
            catch
            {
            }
            finally
            {
                if ((_image != null))
                    _image.Dispose();
            }

            return _newImage;
        }

        #endregion
    }
}