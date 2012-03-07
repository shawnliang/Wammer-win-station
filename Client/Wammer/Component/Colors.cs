using System;
using System.Drawing;

namespace Waveface.Component
{
	public class Colors
	{
		private Colors()
		{
		}

		public static string[] KnownColorNames =
			{
				"Transparent", "Black", "DimGray", "Gray", "DarkGray", "Silver", "LightGray", "Gainsboro", "WhiteSmoke", "White",
				"RosyBrown", "IndianRed", "Brown", "Firebrick", "LightCoral", "Maroon", "DarkRed", "Red", "Snow", "MistyRose",
				"Salmon", "Tomato", "DarkSalmon", "Coral", "OrangeRed", "LightSalmon", "Sienna", "SeaShell", "Chocalate",
				"SaddleBrown", "SandyBrown", "PeachPuff", "Peru", "Linen", "Bisque", "DarkOrange", "BurlyWood", "Tan", "AntiqueWhite",
				"NavajoWhite", "BlanchedAlmond", "PapayaWhip", "Mocassin", "Orange", "Wheat", "OldLace", "FloralWhite", "DarkGoldenrod",
				"Cornsilk", "Gold", "Khaki", "LemonChiffon", "PaleGoldenrod", "DarkKhaki", "Beige", "LightGoldenrod", "Olive",
				"Yellow", "LightYellow", "Ivory", "OliveDrab", "YellowGreen", "DarkOliveGreen", "GreenYellow", "Chartreuse", "LawnGreen",
				"DarkSeaGreen", "ForestGreen", "LimeGreen", "PaleGreen", "DarkGreen", "Green", "Lime", "Honeydew", "SeaGreen", "MediumSeaGreen",
				"SpringGreen", "MintCream", "MediumSpringGreen", "MediumAquaMarine", "YellowAquaMarine", "Turquoise", "LightSeaGreen",
				"MediumTurquoise", "DarkSlateGray", "PaleTurquoise", "Teal", "DarkCyan", "Aqua", "Cyan", "LightCyan", "Azure", "DarkTurquoise",
				"CadetBlue", "PowderBlue", "LightBlue", "DeepSkyBlue", "SkyBlue", "LightSkyBlue", "SteelBlue", "AliceBlue", "DodgerBlue",
				"SlateGray", "LightSlateGray", "LightSteelBlue", "CornflowerBlue", "RoyalBlue", "MidnightBlue", "Lavender", "Navy",
				"DarkBlue", "MediumBlue", "Blue", "GhostWhite", "SlateBlue", "DarkSlateBlue", "MediumSlateBlue", "MediumPurple",
				"BlueViolet", "Indigo", "DarkOrchid", "DarkViolet", "MediumOrchid", "Thistle", "Plum", "Violet", "Purple", "DarkMagenta",
				"Magenta", "Fuchsia", "Orchid", "MediumVioletRed", "DeepPink", "HotPink", "LavenderBlush", "PaleVioletRed", "Crimson",
				"Pink", "LightPink"
			};

		public static string[] SystemColorNames =
			{
				"ActiveBorder", "ActiveCaption", "ActiveCaptionText", "AppWorkspace", "Control", "ControlDark", "ControlDarkDark",
				"ControlLight", "ControlLightLight", "ControlText", "Desktop", "GrayText", "HighLight", "HighLightText",
				"HotTrack", "InactiveBorder", "InactiveCaption", "InactiveCaptionText", "Info", "InfoText", "Menu", "MenuText",
				"ScrollBar", "Window", "WindowFrame", "WindowText"
			};

		/// <summary>
		/// Provides RGB equivalent of a color in HSL
		/// </summary>
		/// <param name="h">Hue Component</param>
		/// <param name="s">Saturation Component</param>
		/// <param name="l">Lightness Component</param>
		/// <param name="r">Red Component returned by reference</param>
		/// <param name="g">Green Component returned by reference</param>
		/// <param name="b">Blue Component returned by reference</param>
		public static void HslToRgb(float h, float s, float l, ref float r, ref float g, ref float b)
		{
			// given h,s,l,[240 and r,g,b [0-255]
			// convert h [0-360], s,l,r,g,b [0-1]
			h = (h/240)*360;
			s /= 240;
			l /= 240;
			r /= 255;
			g /= 255;
			b /= 255;

			// Begin Foley
			float m1, m2;

			// Calc m2
			if (l <= 0.5f)
			{
				//m2=(l*(l+s)); seems to be typo in Foley??, replace l for 1
				m2 = (l*(1 + s));
			}
			else
			{
				m2 = (l + s - l*s);
			}

			//calc m1
			m1 = 2.0f*l - m2;

			//calc r,g,b in [0-1]
			if (s == 0.0f)
			{ // Achromatic: There is no hue
				// leave out the UNDEFINED part, h will always have value
				r = g = b = l;
			}
			else
			{ // Chromatic: There is a hue
				r = getRGBValue(m1, m2, h + 120.0f);
				g = getRGBValue(m1, m2, h);
				b = getRGBValue(m1, m2, h - 120.0f);
			}

			// End Foley
			// convert to 0-255 ranges
			r *= 255;
			g *= 255;
			b *= 255;

		}

		private static float getRGBValue(float n1, float n2, float hue)
		{
			// Helper function for the HSLToRGB function above
			if (hue > 360.0f)
			{
				hue -= 360.0f;
			}
			else if (hue < 0.0f)
			{
				hue += 360.0f;
			}

			if (hue < 60.0)
			{
				return n1 + (n2 - n1)*hue/60.0f;
			}
			else if (hue < 180.0f)
			{
				return n2;
			}
			else if (hue < 240.0f)
			{
				return n1 + (n2 - n1)*(240.0f - hue)/60.0f;
			}
			else
			{
				return n1;
			}
		}

		/// <summary>
		/// Provides HSL values for a Color in RGB
		/// </summary>
		/// <param name="r">Red Component</param>
		/// <param name="g">Green Component</param>
		/// <param name="b">Blue Component</param>
		/// <param name="h">Hue Component returned by reference</param>
		/// <param name="s">Stauration Component returned by reference</param>
		/// <param name="l">Lightness Component returned by reference</param>
		public static void RgbToHsl(int r, int g, int b, ref float h, ref float s, ref float l)
		{
			float delta;
			float fr = (float) r/255;
			float fg = (float) g/255;
			float fb = (float) b/255;
			float max = Math.Max(fr, Math.Max(fg, fb));
			float min = Math.Min(fr, Math.Min(fg, fb));

			//calc the lightness
			l = (max + min)/2;

			if (max == min)
			{
				//should be undefined but this works for what we need
				s = 0;
				h = 240.0f;
			}
			else
			{
				delta = max - min;

				//calc the Saturation
				if (l < 0.5)
				{
					s = delta/(max + min);
				}
				else
				{
					s = delta/(2.0f - (max + min));
				}

				//calc the hue
				if (fr == max)
				{
					h = (fg - fb)/delta;
				}
				else if (fg == max)
				{
					h = 2.0f + (fb - fr)/delta;
				}
				else if (fb == max)
				{
					h = 4.0f + (fr - fg)/delta;
				}

				//convert hue to degrees
				h *= 60.0f;
				if (h < 0.0f)
				{
					h += 360.0f;
				}
			}

			//convert to 0-255 ranges
			//h [0-360], h,l [0-1]
			l *= 240;
			s *= 240;
			h = (h/360)*240;

		}

		/// <summary>
		/// Use alpha blending to brigthen the colors but don't use it
		/// directly. Instead derive an opaque color that we can use.
		/// </summary>
		/// <param name="front">Foreground Color</param>
		/// <param name="back">Background Color</param>
		/// <param name="alpha">Alpha Blending</param>
		/// <returns>Color formed by Alpha blending of the foreground color
		/// over the background color</returns>
		public static Color CalculateColor(Color front, Color back, int alpha)
		{
			Color frontColor = Color.FromArgb(255, front);
			Color backColor = Color.FromArgb(255, back);

			float frontRed = frontColor.R;
			float frontGreen = frontColor.G;
			float frontBlue = frontColor.B;
			float backRed = backColor.R;
			float backGreen = backColor.G;
			float backBlue = backColor.B;

			float fRed = frontRed*alpha/255 + backRed*((float) (255 - alpha)/255);
			byte newRed = (byte) fRed;
			float fGreen = frontGreen*alpha/255 + backGreen*((float) (255 - alpha)/255);
			byte newGreen = (byte) fGreen;
			float fBlue = frontBlue*alpha/255 + backBlue*((float) (255 - alpha)/255);
			byte newBlue = (byte) fBlue;

			return Color.FromArgb(255, newRed, newGreen, newBlue);
		}

		public static bool IsKnownColor(Color color, ref Color knownColor, bool useTransparent)
		{
			// Using the Color structrure "FromKnowColor" does not work if 
			// we did not create the color as a known color to begin with
			// we need to compare the rgbs of both color 
			Color currentColor = Color.Empty;
			bool badColor = false;
			for (KnownColor enumValue = 0; enumValue <= KnownColor.YellowGreen; enumValue++)
			{
				currentColor = Color.FromKnownColor(enumValue);
				string colorName = currentColor.Name;
				if (!useTransparent)
					badColor = (colorName == "Transparent");
				if (color.A == currentColor.A && color.R == currentColor.R && color.G == currentColor.G
					&& color.B == currentColor.B && !currentColor.IsSystemColor
					&& !badColor)
				{
					knownColor = currentColor;
					return true;
				}

			}
			return false;

		}

		public static bool IsSystemColor(Color color, ref Color knownColor)
		{
			// Using the Color structrure "FromKnowColor" does not work if 
			// we did not create the color as a known color to begin with
			// we need to compare the rgbs of both color 
			Color currentColor = Color.Empty;
			for (KnownColor enumValue = 0; enumValue <= KnownColor.YellowGreen; enumValue++)
			{
				currentColor = Color.FromKnownColor(enumValue);
				string colorName = currentColor.Name;
				if (color.R == currentColor.R && color.G == currentColor.G
					&& color.B == currentColor.B && currentColor.IsSystemColor)
				{
					knownColor = currentColor;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Gives a lighter shade of the specified color
		/// </summary>
		/// <param name="color">The input color</param>
		/// <param name="inc">Amount by which to lighten the color</param>
		/// <returns>Lighter shade of input color</returns>
		public static Color LightColor(Color color, int inc)
		{
			int red = color.R;
			int green = color.G;
			int blue = color.B;

			if (red + inc <= 255)
				red += inc;
			if (green + inc <= 255)
				green += inc;
			if (blue + inc <= 255)
				blue += inc;

			return Color.FromArgb(red, green, blue);
		}

		/// <summary>
		/// Gives a darker shade of the specified color
		/// </summary>
		/// <param name="color">The input color</param>
		/// <param name="inc">Amount by which to darken</param>
		/// <returns>Darker shade of input color</returns>
		public static Color DarkColor(Color color, int inc)
		{
			int red = color.R;
			int green = color.G;
			int blue = color.B;

			if (red >= inc)
				red -= inc;
			if (green >= inc)
				green -= inc;
			if (blue >= inc)
				blue -= inc;

			return Color.FromArgb(red, green, blue);

		}
	}
}