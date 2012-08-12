#region

using System;
using System.Drawing;

#endregion

namespace Waveface.Component.ListBarControl
{
    /// <summary>
    /// Provides a helper class allowing bi-directional conversion
    /// between RGB and HLS (Hue, Luminance, Saturation) colour
    /// models.  The HLS model is helpful if you want to produce
    /// darker or ligher shades of colours (by varying the Luminance)
    /// or if you want to produce a brigher or more washed out 
    /// variant (by changing the Saturation).  These operations are
    /// typically very difficult to do in the RGB space.
    /// </summary>
    internal class HLSRGB
    {
        private byte blue;
        private byte green;

        private float hue;
        private float luminance;
        private byte red;
        private float saturation;

        /// <summary>
        /// Constructs an instance of the class from the specified
        /// System.Drawing.Color
        /// </summary>
        /// <param name="c">The System.Drawing.Color to use to initialise the class</param>
        public HLSRGB(Color c)
        {
            red = c.R;
            green = c.G;
            blue = c.B;
            ToHLS();
        }

        /// <summary>
        /// Constructs an instance of the class with the specified hue, luminance and saturation values.
        /// </summary>
        /// <param name="hue">The Hue (between 0.0 and 360.0)</param>
        /// <param name="luminance">The Luminance (between 0.0 and 1.0)</param>
        /// <param name="saturation">The Saturation (between 0.0 and 1.0)</param>
        /// <exception cref="ArgumentOutOfRangeException">If any of the H,L,S values are out of the acceptable range (0.0-360.0 for Hue and 0.0-1.0 for Luminance and Saturation)</exception>
        public HLSRGB(float hue, float luminance, float saturation)
        {
            if ((saturation < 0.0f) || (saturation > 1.0f))
            {
                throw new ArgumentOutOfRangeException("Saturation", "Saturation must be between 0.0 and 1.0");
            }
            if ((hue < 0.0f) || (hue > 360.0f))
            {
                throw new ArgumentOutOfRangeException("Hue", "Hue must be between 0.0 and 360.0");
            }
            if ((luminance < 0.0f) || (luminance > 1.0f))
            {
                throw new ArgumentOutOfRangeException("Luminance", "Luminance must be between 0.0 and 1.0");
            }
            this.hue = hue;
            this.luminance = luminance;
            this.saturation = saturation;
            ToRGB();
        }

        /// <summary>
        /// Constructs an instance of the class with the specified red, green and blue values.
        /// </summary>
        /// <param name="red">The red component.</param>
        /// <param name="green">The green component.</param>
        /// <param name="blue">The blue component.</param>
        public HLSRGB(byte red, byte green, byte blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
            ToHLS();
        }

        /// <summary>
        /// Constructs an instance of the class using the settings of another instance.
        /// </summary>
        /// <param name="hlsrgb">The instance to clone.</param>
        public HLSRGB(HLSRGB hlsrgb)
        {
            red = hlsrgb.Red;
            blue = hlsrgb.Blue;
            green = hlsrgb.Green;
            luminance = hlsrgb.Luminance;
            hue = hlsrgb.Hue;
            saturation = hlsrgb.Saturation;
        }

        /// <summary>
        /// Constructs a new instance of the class initialised to black.
        /// </summary>
        public HLSRGB()
        {
        }

        /// <summary>
        /// Gets or sets the Red component of the colour.
        /// </summary>
        public byte Red
        {
            get
            {
                return red;
            }
            set
            {
                red = value;
                ToHLS();
            }
        }

        /// <summary>
        /// Gets or sets the Green component of the colour.
        /// </summary>
        public byte Green
        {
            get
            {
                return green;
            }
            set
            {
                green = value;
                ToHLS();
            }
        }

        /// <summary>
        /// Gets or sets the Blue component of the colour.
        /// </summary>
        public byte Blue
        {
            get
            {
                return blue;
            }
            set
            {
                blue = value;
                ToHLS();
            }
        }

        /// <summary>
        /// Gets or sets the Luminance (0.0 to 1.0) of the colour.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the value is out of the acceptable range for luminance (0.0 to 1.0)</exception>
        public float Luminance
        {
            get
            {
                return luminance;
            }
            set
            {
                if ((value < 0.0f) || (value > 1.0f))
                {
                    throw new ArgumentOutOfRangeException("Luminance", "Luminance must be between 0.0 and 1.0");
                }
                luminance = value;
                ToRGB();
            }
        }

        /// <summary>
        /// Gets or sets the Hue (0.0 to 360.0) of the colour.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the value is out of the acceptable range for hue (0.0 to 360.0)</exception>
        public float Hue
        {
            get
            {
                return hue;
            }
            set
            {
                if ((value < 0.0f) || (value > 360.0f))
                {
                    throw new ArgumentOutOfRangeException("Hue", "Hue must be between 0.0 and 360.0");
                }
                hue = value;
                ToRGB();
            }
        }

        /// <summary>
        /// Gets or sets the Saturation (0.0 to 1.0) of the colour.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the value is out of the acceptable range for saturation (0.0 to 1.0)</exception>
        public float Saturation
        {
            get
            {
                return saturation;
            }
            set
            {
                if ((value < 0.0f) || (value > 1.0f))
                {
                    throw new ArgumentOutOfRangeException("Saturation", "Saturation must be between 0.0 and 1.0");
                }
                saturation = value;
                ToRGB();
            }
        }

        /// <summary>
        /// Gets or sets the Color as a System.Drawing.Color instance
        /// </summary>
        public Color Color
        {
            get
            {
                Color c = Color.FromArgb(red, green, blue);
                return c;
            }
            set
            {
                red = value.R;
                green = value.G;
                blue = value.B;
                ToHLS();
            }
        }

        /// <summary>
        /// Lightens the colour by the specified amount by modifying
        /// the luminance (for example, 0.2 would lighten the colour by 20%)
        /// </summary>
        public void LightenColor(float lightenBy)
        {
            luminance *= (1.0f + lightenBy);
            if (luminance > 1.0f)
            {
                luminance = 1.0f;
            }
            ToRGB();
        }

        /// <summary>
        /// Darkens the colour by the specified amount by modifying
        /// the luminance (for example, 0.2 would darken the colour by 20%)
        /// </summary>
        public void DarkenColor(float darkenBy)
        {
            luminance *= darkenBy;
            ToRGB();
        }


        private void ToHLS()
        {
            byte minval = Math.Min(red, Math.Min(green, blue));
            byte maxval = Math.Max(red, Math.Max(green, blue));

            float mdiff = (maxval - minval);
            float msum = (maxval + minval);

            luminance = msum/510.0f;

            if (maxval == minval)
            {
                saturation = 0.0f;
                hue = 0.0f;
            }
            else
            {
                float rnorm = (maxval - red)/mdiff;
                float gnorm = (maxval - green)/mdiff;
                float bnorm = (maxval - blue)/mdiff;

                saturation = (luminance <= 0.5f) ? (mdiff/msum) : (mdiff/(510.0f - msum));

                if (red == maxval)
                {
                    hue = 60.0f*(6.0f + bnorm - gnorm);
                }
                if (green == maxval)
                {
                    hue = 60.0f*(2.0f + rnorm - bnorm);
                }
                if (blue == maxval)
                {
                    hue = 60.0f*(4.0f + gnorm - rnorm);
                }
                if (hue > 360.0f)
                {
                    hue = hue - 360.0f;
                }
            }
        }

        private void ToRGB()
        {
            if (saturation == 0.0)
            {
                red = (byte) (luminance*255.0F);
                green = red;
                blue = red;
            }
            else
            {
                float rm1;
                float rm2;

                if (luminance <= 0.5f)
                {
                    rm2 = luminance + luminance*saturation;
                }
                else
                {
                    rm2 = luminance + saturation - luminance*saturation;
                }
                rm1 = 2.0f*luminance - rm2;
                red = ToRGB1(rm1, rm2, hue + 120.0f);
                green = ToRGB1(rm1, rm2, hue);
                blue = ToRGB1(rm1, rm2, hue - 120.0f);
            }
        }

        private byte ToRGB1(float rm1, float rm2, float rh)
        {
            if (rh > 360.0f)
            {
                rh -= 360.0f;
            }
            else if (rh < 0.0f)
            {
                rh += 360.0f;
            }

            if (rh < 60.0f)
            {
                rm1 = rm1 + (rm2 - rm1)*rh/60.0f;
            }
            else if (rh < 180.0f)
            {
                rm1 = rm2;
            }
            else if (rh < 240.0f)
            {
                rm1 = rm1 + (rm2 - rm1)*(240.0f - rh)/60.0f;
            }

            return (byte) (rm1*255);
        }
    }
}