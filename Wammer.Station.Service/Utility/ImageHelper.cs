using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Wammer.Utility
{
	public static class ImageHelper
	{
		public static Bitmap Scale(Bitmap original, int sideLength)
		{
			float ratio1 = (float)sideLength / (float) original.Width;
			float ratio2 = (float)sideLength / (float) original.Height;
			float ratio = (original.Width > original.Height) ? ratio1 : ratio2;


			int scaledWidth = (int)(original.Width * ratio);
			int scaledHeight = (int)(original.Height * ratio);
			Bitmap scaledImage = new Bitmap(scaledWidth, scaledHeight);

			using (Graphics g = Graphics.FromImage(scaledImage))
			{
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
				g.DrawImage(original, new Rectangle(0, 0, scaledWidth, scaledHeight),
						new Rectangle(0, 0, original.Width, original.Height), GraphicsUnit.Pixel);
			}

			return scaledImage;
		}

		public static Bitmap Crop(Bitmap original, int width, int height)
		{
			Bitmap cropedImage = new Bitmap(width, height);

			using (Graphics g = Graphics.FromImage(cropedImage))
			{
				g.DrawImage(original, new Rectangle(0, 0, width, height),
											new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
			}

			return cropedImage;
		}

		public static int LongSizeLength(Bitmap img)
		{
			return img.Width > img.Height ? img.Width : img.Height;
		}

		public static int ShortSizeLength(Bitmap img)
		{
			return img.Width < img.Height ? img.Width : img.Height;
		}
	}
}
