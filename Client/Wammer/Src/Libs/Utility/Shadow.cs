//	http://www.codeproject.com/cs/media/shadow.asp

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace Waveface
{

	#region FastBitmap

	internal class FastBitmap : IDisposable, ICloneable
	{
		internal Bitmap _bitmap;
		private BitmapData _bitmapData;

		public FastBitmap(Int32 width, Int32 height, PixelFormat fmt)
		{
			_bitmap = new Bitmap(width, height, fmt);
		}

		public FastBitmap(String filename)
		{
			_bitmap = new Bitmap(filename);
		}

		~FastBitmap()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}

		protected virtual void Dispose(Boolean disposing)
		{
			Unlock();

			if (disposing)
				_bitmap.Dispose();
		}

		private FastBitmap()
		{
		}

		public Object Clone()
		{
			FastBitmap clone = new FastBitmap();
			clone._bitmap = (Bitmap) _bitmap.Clone();
			return clone;
		}

		public Int32 Width
		{
			get { return _bitmap.Width; }
		}

		public Int32 Height
		{
			get { return _bitmap.Height; }
		}

		public void Lock()
		{
			_bitmapData = _bitmap.LockBits(
				new Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
				ImageLockMode.ReadWrite, _bitmap.PixelFormat);
		}

		public unsafe Color GetPixel(Int32 x, Int32 y)
		{
			if (_bitmapData.PixelFormat == PixelFormat.Format32bppArgb)
			{
				Byte* b = (Byte*) _bitmapData.Scan0 + (y*_bitmapData.Stride) + (x*4);
				return Color.FromArgb(*(b + 3), *(b + 2), *(b + 1), *b);
			}

			if (_bitmapData.PixelFormat == PixelFormat.Format24bppRgb)
			{
				Byte* b = (Byte*) _bitmapData.Scan0 + (y*_bitmapData.Stride) + (x*3);
				return Color.FromArgb(*(b + 2), *(b + 1), *b);
			}

			return Color.Empty;
		}

		public unsafe void SetPixel(Int32 x, Int32 y, Color c)
		{
			if (_bitmapData.PixelFormat == PixelFormat.Format32bppArgb)
			{
				Byte* b = (Byte*) _bitmapData.Scan0 + (y*_bitmapData.Stride) + (x*4);
				*b = c.B;
				*(b + 1) = c.G;
				*(b + 2) = c.R;
				*(b + 3) = c.A;
			}

			if (_bitmapData.PixelFormat == PixelFormat.Format24bppRgb)
			{
				Byte* b = (Byte*) _bitmapData.Scan0 + (y*_bitmapData.Stride) + (x*3);
				*b = c.B;
				*(b + 1) = c.G;
				*(b + 2) = c.R;
			}
		}

		public void CopyTo(FastBitmap bitmap, Int32 destx, Int32 desty, Int32 srcx, Int32 srcy, Int32 width, Int32 height)
		{
			try
			{
				Lock();
				bitmap.Lock();

				for (Int32 y = 0; y < height; y++)
				{
					for (Int32 x = 0; x < width; x++)
					{
						Color c = GetPixel(srcx + x, srcy + y);
						bitmap.SetPixel(destx + x, desty + y, c);
					}
				}
			}
			finally
			{
				Unlock();
				bitmap.Unlock();
			}
		}

		public Byte GetIntensity(Int32 x, Int32 y)
		{
			Color c = GetPixel(x, y);
			return (Byte) ((c.R*0.30 + c.G*0.59 + c.B*0.11) + 0.5);
		}

		public void Unlock()
		{
			if (_bitmapData != null)
			{
				_bitmap.UnlockBits(_bitmapData);
				_bitmapData = null;
			}
		}

		public void Save(String filename, ImageFormat format)
		{
			_bitmap.Save(filename, format);
		}

		public void Save(String filename)
		{
			_bitmap.Save(filename);
		}
	}

	#endregion

	#region Layer

	internal class Layer : ICloneable
	{
		internal FastBitmap _bitmap;
		private FastBitmap _mask;
		public Double _opacity;
		private Int32 _offsetx, _offsety;

		public Layer(Int32 width, Int32 height)
		{
			_bitmap = new FastBitmap(width, height, PixelFormat.Format32bppArgb);
			Clear(Color.Transparent);
			_opacity = 1.0;
		}

		public Layer(FastBitmap bitmap)
		{
			_bitmap = bitmap;
			_opacity = 1.0;
		}

		public Double Opacity
		{
			get { return _opacity; }
			set { _opacity = value; }
		}

		public FastBitmap Bitmap
		{
			get { return _bitmap; }
		}

		public FastBitmap Mask
		{
			get { return _mask; }
			set { _mask = value; }
		}

		public Int32 OffsetX
		{
			get { return _offsetx; }
			set { _offsetx = value; }
		}

		public Int32 OffsetY
		{
			get { return _offsety; }
			set { _offsety = value; }
		}

		public Object Clone()
		{
			Layer clone = new Layer(_bitmap.Width, _bitmap.Height);
			clone._bitmap = (FastBitmap) _bitmap.Clone();
			return clone;
		}

		public void Clear(Color c)
		{
			_bitmap.Lock();

			for (Int32 y = 0; y < _bitmap.Height; y++)
				for (Int32 x = 0; x < _bitmap.Width; x++)
					_bitmap.SetPixel(x, y, c);

			_bitmap.Unlock();
		}

		public void DrawText(Int32 x, Int32 y, String text, Font font, Brush brush)
		{
			Graphics g = Graphics.FromImage(_bitmap._bitmap);
			g.DrawString(text, font, brush, x, y, StringFormat.GenericTypographic);
			g.Dispose();
		}

		public void FillRectangle(Int32 x, Int32 y, Int32 width, Int32 height, Brush brush)
		{
			Graphics g = Graphics.FromImage(_bitmap._bitmap);
			g.FillRectangle(brush, x, y, width, height);
			g.Dispose();
		}

		public Color GetPixel(Int32 x, Int32 y)
		{
			return _bitmap.GetPixel(x, y);
		}

		public void Invert()
		{
			_bitmap.Lock();

			for (Int32 y = 0; y < _bitmap.Height; y++)
			{
				for (Int32 x = 0; x < _bitmap.Width; x++)
				{
					Color c = _bitmap.GetPixel(x, y);
					_bitmap.SetPixel(x, y, Color.FromArgb(c.A, 255 - c.R, 255 - c.G, 255 - c.B));
				}
			}

			_bitmap.Unlock();
		}

		private Single Gauss(Single x, Single middle, Single width)
		{
			if (width == 0)
				return 1F;

			Double t = - (1.0/width)*((middle - x)*(middle - x));
			return (Single) Math.Pow( /*Math.E*/1.5, t);
		}

		public void Blur(Int32 horz, Int32 vert)
		{
			Single weightsum;
			Single[] weights;

			FastBitmap t = (FastBitmap) _bitmap.Clone();

			_bitmap.Lock();
			t.Lock();

			// horizontal blur

			weights = new Single[horz*2 + 1];

			for (Int32 i = 0; i < horz*2 + 1; i++)
			{
				Single y = Gauss(- horz + i, 0, horz);
				weights[i] = y;
			}

			for (Int32 row = 0; row < _bitmap.Height; row++)
			{
				for (Int32 col = 0; col < _bitmap.Width; col++)
				{
					Double r = 0;
					Double g = 0;
					Double b = 0;
					Double a = 0;
					weightsum = 0;

					for (Int32 i = 0; i < horz*2 + 1; i++)
					{
						Int32 x = col - horz + i;

						if (x < 0)
						{
							i += - x;
							x = 0;
						}

						if (x > _bitmap.Width - 1)
							break;

						Color c = _bitmap.GetPixel(x, row);
						r += c.R*weights[i]/255.0*c.A;
						g += c.G*weights[i]/255.0*c.A;
						b += c.B*weights[i]/255.0*c.A;
						a += c.A*weights[i];
						weightsum += weights[i];
					}

					r /= weightsum;
					g /= weightsum;
					b /= weightsum;
					a /= weightsum;
					Byte br = (Byte) Math.Round(r);
					Byte bg = (Byte) Math.Round(g);
					Byte bb = (Byte) Math.Round(b);
					Byte ba = (Byte) Math.Round(a);

					if (br > 255)
						br = 255;

					if (bg > 255)
						bg = 255;

					if (bb > 255)
						bb = 255;

					if (ba > 255)
						ba = 255;

					t.SetPixel(col, row, Color.FromArgb(ba, br, bg, bb));
				}
			}

			// vertical blur
			weights = new Single[vert*2 + 1];

			for (Int32 i = 0; i < vert*2 + 1; i++)
			{
				Single y = Gauss(- vert + i, 0, vert);
				weights[i] = y;
			}

			for (Int32 col = 0; col < _bitmap.Width; col++)
			{
				for (Int32 row = 0; row < _bitmap.Height; row++)
				{
					Double r = 0;
					Double g = 0;
					Double b = 0;
					Double a = 0;
					weightsum = 0;

					for (Int32 i = 0; i < vert*2 + 1; i++)
					{
						Int32 y = row - vert + i;

						if (y < 0)
						{
							i += - y;
							y = 0;
						}

						if (y > _bitmap.Height - 1)
							break;

						Color c = t.GetPixel(col, y);
						r += c.R*weights[i]/255.0*c.A;
						g += c.G*weights[i]/255.0*c.A;
						b += c.B*weights[i]/255.0*c.A;
						a += c.A*weights[i];
						weightsum += weights[i];
					}

					r /= weightsum;
					g /= weightsum;
					b /= weightsum;
					a /= weightsum;
					Byte br = (Byte) Math.Round(r);
					Byte bg = (Byte) Math.Round(g);
					Byte bb = (Byte) Math.Round(b);
					Byte ba = (Byte) Math.Round(a);

					if (br > 255)
						br = 255;

					if (bg > 255)
						bg = 255;

					if (bb > 255)
						bb = 255;

					if (ba > 255)
						ba = 255;

					_bitmap.SetPixel(col, row, Color.FromArgb(ba, br, bg, bb));
				}
			}

			t.Dispose(); // will unlock
			_bitmap.Unlock();
		}

		private Byte GetBumpMapPixel(FastBitmap bmp, Int32 x, Int32 y)
		{
			// create a single number from R, G and B values at point (x, y)
			// if point (x, y) lays outside the bitmap, GetBumpMapPixel()
			// returns the closest pixel within the bitmap

			if (x < 0)
				x = 0;

			if (x > _bitmap.Width - 1)
				x = _bitmap.Width - 1;

			if (y < 0)
				y = 0;

			if (y > _bitmap.Height - 1)
				y = _bitmap.Height - 1;

			return bmp.GetIntensity(x, y);
		}

		public void BumpMap(Layer bumpmap, Int32 azimuth, Int32 elevation, Int32 bevelwidth, Boolean lightzalways1)
		{
			bumpmap._bitmap.Lock();
			_bitmap.Lock();

			for (Int32 row = 0; row < _bitmap.Height; row++)
			{
				for (Int32 col = 0; col < _bitmap.Width; col++)
				{
					// calculate normal for point (col, row)
					// this is an approximation of the derivative
					// I personally haven't figured out why it's
					// the way it is
					// normal_z is constant, I think this means
					// the longer the vector is, the more it lays
					// in the xy plane

					Byte[] x = new Byte[6];

					x[0] = GetBumpMapPixel(bumpmap._bitmap, col - 1, row - 1);
					x[1] = GetBumpMapPixel(bumpmap._bitmap, col - 1, row - 0);
					x[2] = GetBumpMapPixel(bumpmap._bitmap, col - 1, row + 1);
					x[3] = GetBumpMapPixel(bumpmap._bitmap, col + 1, row - 1);
					x[4] = GetBumpMapPixel(bumpmap._bitmap, col + 1, row - 0);
					x[5] = GetBumpMapPixel(bumpmap._bitmap, col + 1, row + 1);

					Single normal_x = x[0] + x[1] + x[2] - x[3] - x[4] - x[5];

					x[0] = GetBumpMapPixel(bumpmap._bitmap, col - 1, row + 1);
					x[1] = GetBumpMapPixel(bumpmap._bitmap, col - 0, row + 1);
					x[2] = GetBumpMapPixel(bumpmap._bitmap, col + 1, row + 1);
					x[3] = GetBumpMapPixel(bumpmap._bitmap, col - 1, row - 1);
					x[4] = GetBumpMapPixel(bumpmap._bitmap, col - 0, row - 1);
					x[5] = GetBumpMapPixel(bumpmap._bitmap, col + 1, row - 1);

					Single normal_y = x[0] + x[1] + x[2] - x[3] - x[4] - x[5];

					Single normal_z = (6F*255F)/bevelwidth;

					Single length = (Single) Math.Sqrt(
						normal_x*normal_x +
							normal_y*normal_y +
							normal_z*normal_z);

					if (length != 0)
					{
						normal_x /= length;
						normal_y /= length;
						normal_z /= length;
					}

					// convert to radians
					Double azimuth_rad = azimuth/180.0*Math.PI;
					Double elevation_rad = elevation/180.0*Math.PI;

					// light vector -- this is the location of the light
					// source but it also serves as the direction with
					// origin <0, 0, 0>
					// the formulas to calculate x, y and z are those to
					// rotate a point in 3D space
					// if lightzalways1 is true, light_z is set to 1
					// because we want full color intensity for that pixel;
					// if we set light_z to (Single)Math.Sin(elevation_rad),
					// which is the correct way to calculate light_z, the
					// color is more dark, but when we ignore light_z, the
					// light source is straight above the pixel and
					// therefore sin(elevation_rad) is always 1

					Single light_x = (Single) (Math.Cos(azimuth_rad)*Math.Cos(elevation_rad));
					Single light_y = (Single) (Math.Sin(azimuth_rad)*Math.Cos(elevation_rad));
					Single light_z = 1F;

					if (!lightzalways1)
						light_z = (Single) Math.Sin(elevation_rad);

					// the normal and light vector are unit vectors, so
					// taking the dot product of these two yields the
					// cosine of the angle between them

					Single cos_light_normal = 0;
					cos_light_normal += normal_x*light_x;
					cos_light_normal += normal_y*light_y;
					cos_light_normal += normal_z*light_z;

					// get pixel (col, row) of this layer, calculate color
					// and set pixel back with new color

					Color c = _bitmap.GetPixel(col, row);
					Single r = c.R;
					Single g = c.G;
					Single b = c.B;
					r *= cos_light_normal;
					g *= cos_light_normal;
					b *= cos_light_normal;
					Byte red = (Byte) Math.Min(Math.Round(r), 255);
					Byte green = (Byte) Math.Min(Math.Round(g), 255);
					Byte blue = (Byte) Math.Min(Math.Round(b), 255);
					_bitmap.SetPixel(col, row, Color.FromArgb(red, green, blue));
				}
			}

			_bitmap.Unlock();
			bumpmap._bitmap.Unlock();
		}
	}

	#endregion

	#region Layers

	internal class Layers
	{
		private LayeredImage _image;
		private ArrayList _layers = new ArrayList();

		public Layers(LayeredImage image)
		{
			_image = image;
		}

		public Int32 Count
		{
			get { return _layers.Count; }
		}

		public Layer Add()
		{
			Layer layer = new Layer(_image.Width, _image.Height);
			_layers.Add(layer);
			return layer;
		}

		public Layer Add(FastBitmap bitmap)
		{
			Layer layer = new Layer(bitmap);
			_layers.Add(layer);
			return layer;
		}

		public Layer Add(Int32 width, Int32 height)
		{
			Layer layer = new Layer(width, height);
			_layers.Add(layer);
			return layer;
		}

		public Layer Copy(Layer layer)
		{
			Layer copy = (Layer) layer.Clone();
			_layers.Add(copy);
			return copy;
		}

		public Layer this[Int32 i]
		{
			get { return (Layer) _layers[i]; }
		}
	}

	#endregion

	#region LayeredImage

	internal class LayeredImage
	{
		private Int32 _width, _height;
		private Bitmap _checkerboard;
		private Layers _layers;

		public LayeredImage(Int32 width, Int32 height)
		{
			_width = width;
			_height = height;
			_layers = new Layers(this);

			// checker board brush
			_checkerboard = new Bitmap(32, 32, PixelFormat.Format24bppRgb);
			Color darkgray = Color.FromArgb(102, 102, 102);
			Color lightgray = Color.FromArgb(153, 153, 153);

			for (Int32 i = 0; i < 32; i++)
			{
				for (Int32 j = 0; j < 32; j++)
				{
					if ((i < 16 && j < 16) || (i >= 16 && j >= 16))
						_checkerboard.SetPixel(j, i, lightgray);
					else
						_checkerboard.SetPixel(j, i, darkgray);
				}
			}

			// background layer
			Layer bglayer = _layers.Add();
			Graphics g = Graphics.FromImage(bglayer._bitmap._bitmap);
			TextureBrush brush = new TextureBrush(_checkerboard);
			g.FillRectangle(brush, 0, 0, _width, _height);
			brush.Dispose();
			g.Dispose();
		}

		public Int32 Width
		{
			get { return _width; }
		}

		public Int32 Height
		{
			get { return _height; }
		}

		public Layers Layers
		{
			get { return _layers; }
		}

		internal FastBitmap Flatten()
		{
			// create a bitmap for result image
			FastBitmap final = new FastBitmap(_width, _height, PixelFormat.Format24bppRgb);

			// lock all bitmaps
			final.Lock();

			for (Int32 i = 0; i < _layers.Count; i++)
			{
				Layer l = _layers[i];
				l._bitmap.Lock();

				if (l.Mask != null)
					l.Mask.Lock();
			}

			// calculate colors of flattened image
			// 1. take offsetx, offsety into consideration
			// 2. calculate alpha of color (alpha, opacity, mask)
			// 3. mix colors of current layer and layer below
			for (Int32 y = 0; y < _height; y++)
			{
				for (Int32 x = 0; x < _width; x++)
				{
					Color c0 = _layers[0]._bitmap.GetPixel(x, y);

					for (Int32 i = 1; i < _layers.Count; i++)
					{
						Layer layer = _layers[i];
						Color c1 = Color.Transparent;

						if (x >= layer.OffsetX &&
							x <= layer.OffsetX + layer._bitmap.Width - 1 &&
							y >= layer.OffsetY &&
							y <= layer.OffsetY + layer._bitmap.Height - 1)
						{
							c1 = layer._bitmap.GetPixel(x - layer.OffsetX,
							                            y - layer.OffsetY);
						}

						if (c1.A == 255 && layer.Opacity == 1.0 && layer.Mask == null)
						{
							c0 = c1;
						}
						else
						{
							Double tr, tg, tb, a;
							a = c1.A/255.0*layer.Opacity;

							if (layer.Mask != null)
								a *= layer.Mask.GetIntensity(x, y)/255.0;

							tr = c1.R*a + c0.R*(1.0 - a);
							tg = c1.G*a + c0.G*(1.0 - a);
							tb = c1.B*a + c0.B*(1.0 - a);
							tr = Math.Round(tr);
							tg = Math.Round(tg);
							tb = Math.Round(tb);
							tr = Math.Min(tr, 255);
							tg = Math.Min(tg, 255);
							tb = Math.Min(tb, 255);
							c0 = Color.FromArgb((Byte) tr, (Byte) tg, (Byte) tb);
						}
					}
					final.SetPixel(x, y, c0);
				}
			}

			// unlock all bitmaps
			for (Int32 i = 0; i < _layers.Count; i++)
			{
				Layer l = _layers[i];
				l._bitmap.Unlock();

				if (l.Mask != null)
					l.Mask.Unlock();
			}

			final.Unlock();

			return final;
		}
	}

	#endregion

	#region Shadow

	internal class Shadow
	{
		private static Color HexColorToColor(String s)
		{
			if (s.Length != 6)
				return Color.Empty;

			UInt32 r, g, b;
			r = g = b = 0;

			for (Int32 i = 0; i < 6; i++)
			{
				Int32 n = "0123456789ABCDEF".IndexOf(Char.ToUpper(s[i]));

				if (n == -1)
					return Color.Empty;

				UInt32 x = (UInt32) n;

				switch (i)
				{
					case 0:
						r |= x << 4;
						break;
					case 1:
						r |= x;
						break;
					case 2:
						g |= x << 4;
						break;
					case 3:
						g |= x;
						break;
					case 4:
						b |= x << 4;
						break;
					case 5:
						b |= x;
						break;
				}
			}
			return Color.FromArgb((Byte) r, (Byte) g, (Byte) b);
		}

		private static Boolean GetArgParam(String arg, out Int32 num)
		{
			num = 0;

			if (arg.Length < 4)
				return false;

			String[] str = arg.Split(new Char[] {':'}, 2);

			if (str.Length != 2)
				return false;

			try
			{
				num = Convert.ToInt32(str[1]);
				return true;
			}
			catch
			{
				// catch all exceptions Convert.ToInt32() can throw;
				// no other exceptions can be thrown
				return false;
			}
		}

		private static Boolean GetArgParam(String arg, out Double num)
		{
			num = 0.0;

			if (arg.Length < 4)
				return false;

			String[] str = arg.Split(new Char[] {':'}, 2);

			if (str.Length != 2)
				return false;

			try
			{
				num = Convert.ToDouble(str[1], CultureInfo.InvariantCulture);
				return true;
			}
			catch
			{
				// catch all exceptions Convert.ToInt32() can throw;
				// no other exceptions can be thrown
				return false;
			}
		}

		private static Boolean GetArgParam(String arg, out Color col)
		{
			col = Color.Empty;

			if (arg.Length != 9)
				return false;

			String[] str = arg.Split(new Char[] {':'}, 2);

			if (str.Length != 2)
				return false;

			col = HexColorToColor(str[1]);
			return (col != Color.Empty);
		}

		public static void CreateShadowImage(string fileName, int swidth)
		{
			Int32 shadowwidth = swidth;
			Int32 borderwidth = 0;
			Int32 margin = shadowwidth;
			Int32 shadowdir = 0;
			Double shadowtrans = 0.0;
			Color bkcolor = Color.FromArgb(255, 255, 255);
			Color shadowcolor = Color.FromArgb(0, 0, 0);
			Color bordercolor = Color.FromArgb(0, 0, 0);
			Boolean softshadow = true;

			String outputfile = fileName;
			String inputfile = fileName;

			FastBitmap tmp, bmp;

			if (System.IO.File.Exists(inputfile))
			{
				tmp = new FastBitmap(inputfile);
			}
			else
			{
				Console.WriteLine("Error: Could not find file '{0}'", inputfile);
				return;
			}

			bmp = new FastBitmap(tmp.Width + borderwidth*2, tmp.Height + borderwidth*2, PixelFormat.Format32bppArgb);

			// add border if necessary
			if (borderwidth > 0)
			{
				SolidBrush br = new SolidBrush(bordercolor);
				Graphics g = Graphics.FromImage(bmp._bitmap);
				g.FillRectangle(br, 0, 0, borderwidth*2 + tmp.Width, borderwidth*2 + tmp.Height);
				g.Dispose();
				br.Dispose();
			}

			tmp.CopyTo(bmp, borderwidth, borderwidth, 0, 0, tmp.Width, tmp.Height);
			tmp.Dispose();

			// create image
			Int32 width = bmp.Width + shadowwidth + margin*2;
			Int32 height = bmp.Height + shadowwidth + margin*2;
			LayeredImage image = new LayeredImage(width, height);

			Int32 shadowx = 0, shadowy = 0, imgx = 0, imgy = 0;

			if (softshadow)
			{
				switch (shadowdir)
				{
					case 0:
						{
							shadowx = margin - shadowwidth/2;
							shadowy = margin - shadowwidth/2;
							imgx = margin;
							imgy = margin;
							break;
						}

					case 1:
						{
							shadowx = margin + shadowwidth - 3*(shadowwidth/2);
							shadowy = margin - shadowwidth/2;
							imgx = margin + shadowwidth;
							imgy = margin;
							break;
						}

					case 2:
						{
							shadowx = margin + shadowwidth - 3*(shadowwidth/2);
							shadowy = margin + shadowwidth - 3*(shadowwidth/2);
							imgx = margin + shadowwidth;
							imgy = margin + shadowwidth;
							break;
						}

					case 3:
						{
							shadowx = margin - shadowwidth/2;
							shadowy = margin + shadowwidth - 3*(shadowwidth/2);
							imgx = margin;
							imgy = margin + shadowwidth;
							break;
						}
				}
			}
			else
			{
				switch (shadowdir)
				{
					case 0:
						{
							shadowx = margin;
							shadowy = margin;
							imgx = margin;
							imgy = margin;
							break;
						}

					case 1:
						{
							shadowx = margin - shadowwidth;
							shadowy = margin;
							imgx = margin + shadowwidth;
							imgy = margin;
							break;
						}

					case 2:
						{
							shadowx = margin - shadowwidth;
							shadowy = margin - shadowwidth;
							imgx = margin + shadowwidth;
							imgy = margin + shadowwidth;
							break;
						}

					case 3:
						{
							shadowx = margin;
							shadowy = margin - shadowwidth;
							imgx = margin;
							imgy = margin + shadowwidth;
							break;
						}
				}
			}

			// background
			Layer bg = image.Layers.Add();
			bg.Clear(bkcolor);

			// shadow -- layer must be larger because of blur
			Layer shadow = image.Layers.Add(width + shadowwidth, height + shadowwidth);
			SolidBrush brush = new SolidBrush(shadowcolor);
			shadow.FillRectangle(shadowwidth, shadowwidth, bmp.Width, bmp.Height, brush);

			if (softshadow)
				shadow.Blur(shadowwidth, shadowwidth);

			brush.Dispose();
			shadow.OffsetX = shadowx;
			shadow.OffsetY = shadowy;
			shadow.Opacity = 1.0 - shadowtrans;

			// image
			Layer img = image.Layers.Add(bmp);
			img.OffsetX = imgx;
			img.OffsetY = imgy;

			// result
			FastBitmap result = image.Flatten();

			// save
			String filename = outputfile != "" ? outputfile : inputfile;
			String ext = Path.GetExtension(filename);

			if (ext == "")
				ext = ".bmp";

			ext = ext.ToLower();
			ImageFormat imgf = ImageFormat.Bmp;

			switch (ext)
			{
				case ".bmp":
					ext = ".bmp";
					imgf = ImageFormat.Bmp;
					break;

				case ".jpg":
					ext = ".jpg";
					imgf = ImageFormat.Jpeg;
					break;

				case ".jpeg":
					ext = ".jpeg";
					imgf = ImageFormat.Jpeg;
					break;

				case ".png":
					ext = ".png";
					imgf = ImageFormat.Png;
					break;

				case ".gif":
					ext = ".gif";
					imgf = ImageFormat.Gif;
					break;

				default:
					ext = ".bmp";
					imgf = ImageFormat.Bmp;
					break;
			}

			result.Save(filename, imgf);
		}
	}

	#endregion
}