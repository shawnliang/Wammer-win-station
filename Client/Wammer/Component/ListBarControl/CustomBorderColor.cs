#region

using System.Drawing;

#endregion

namespace Waveface.Component.ListBarControl
{
    /// <summary>
    /// A class for deriving border colours for colours other than
    /// the system control colour using luminance adjustment
    /// techniques.
    /// </summary>
    internal class CustomBorderColor
    {
        // For black we can't create a darker colour, and for
        // white we can't create a lighter one.  Therefore we
        // need to play with the colours a little bit to try
        // and achieve an equivalent effect.
        private static Color BlackDarkDark = Color.FromArgb(32, 32, 32);
        private static Color BlackDark = Color.FromArgb(48, 48, 48);
        private static Color BlackLight = Color.FromArgb(128, 128, 128);
        private static Color BlackLightLight = Color.FromArgb(192, 192, 192);
        private static Color WhiteDarkDark = Color.FromArgb(96, 96, 96);
        private static Color WhiteDark = Color.FromArgb(160, 160, 160);
        private static Color WhiteLight = Color.FromArgb(230, 230, 230);
        private static Color WhiteLightLight = Color.FromArgb(250, 250, 250);

        /// <summary>
        /// Private constructor - only static methods in this class.
        /// </summary>
        private CustomBorderColor()
        {
            // Intentionally blank
        }

        /// <summary>
        /// Returns the grey intensity of a colour using the ITU
        /// grey-scale standard.
        /// </summary>
        /// <param name="color">The <see cref="System.Drawing.Color"/> to get
        /// the grey scale value for.</param>
        /// <returns>A value between 0 and 255 holding the grey scale amount.
        /// This can be used as the r, g and b arguments to the
        /// <c>Color</c> class to construct the grey scale colour.</returns>
        public static int GreyScale(
            Color color
            )
        {
            return (222*color.R + 707*color.G + 71*color.B)/1000;
        }

        /// <summary>
        /// Draw a border on the specified <see cref="System.Drawing.Graphics"/>
        /// object.
        /// </summary>
        /// <param name="gfx">The <see cref="System.Drawing.Graphics"/> object
        /// to draw onto.</param>
        /// <param name="rect">The <see cref="System.Drawing.Rectangle"/> boundary
        /// for the border.</param>
        /// <param name="color">The <see cref="System.Drawing.Color"/> of the 
        /// object.  This is used to determine the border colours.</param>
        /// <param name="thin">Whether to draw a thin border or not.</param>
        /// <param name="pressed">Whether the border should be drawn pressed
        /// or raised.</param>
        public static void DrawBorder(
            Graphics gfx,
            Rectangle rect,
            Color color,
            bool thin,
            bool pressed
            )
        {
            Pen darkPen = new Pen(ColorDark(color), 1);
            Pen lightLightPen = new Pen(ColorLightLight(color), 1);

            if (thin)
            {
                if (pressed)
                {
                    gfx.DrawLine(darkPen, rect.Left, rect.Bottom - 2,
                                 rect.Left, rect.Top);
                    gfx.DrawLine(darkPen, rect.Left, rect.Top,
                                 rect.Right - 2, rect.Top);
                    gfx.DrawLine(lightLightPen, rect.Right - 1, rect.Top,
                                 rect.Right - 1, rect.Bottom - 1);
                    gfx.DrawLine(lightLightPen, rect.Right - 1, rect.Bottom - 1,
                                 rect.Left, rect.Bottom - 1);
                }
                else
                {
                    gfx.DrawLine(lightLightPen, rect.Left, rect.Bottom - 2,
                                 rect.Left, rect.Top);
                    gfx.DrawLine(lightLightPen, rect.Left, rect.Top,
                                 rect.Right - 1, rect.Top);
                    gfx.DrawLine(darkPen, rect.Right - 1, rect.Top + 1,
                                 rect.Right - 1, rect.Bottom - 1);
                    gfx.DrawLine(darkPen, rect.Right - 1, rect.Bottom - 1,
                                 rect.Left, rect.Bottom - 1);
                }
            }
            else
            {
                Pen lightPen = new Pen(ColorLight(color), 1);
                Pen darkDarkPen = new Pen(ColorDarkDark(color), 1);
                if (pressed)
                {
                    gfx.DrawLine(darkDarkPen, rect.Left, rect.Bottom - 1,
                                 rect.Left, rect.Top);
                    gfx.DrawLine(darkDarkPen, rect.Left, rect.Top,
                                 rect.Right - 1, rect.Top);
                    gfx.DrawLine(lightLightPen, rect.Right - 1, rect.Top + 1,
                                 rect.Right - 1, rect.Bottom - 1);
                    gfx.DrawLine(lightLightPen, rect.Right - 1, rect.Bottom - 1,
                                 rect.Left + 1, rect.Bottom - 1);
                    gfx.DrawLine(darkPen, rect.Left + 1, rect.Bottom - 2,
                                 rect.Left + 1, rect.Top + 1);
                    gfx.DrawLine(darkPen, rect.Left + 1, rect.Top + 1,
                                 rect.Right - 2, rect.Top + 1);
                }
                else
                {
                    gfx.DrawLine(lightLightPen, rect.Left, rect.Bottom - 1,
                                 rect.Left, rect.Top);
                    gfx.DrawLine(lightLightPen, rect.Left, rect.Top,
                                 rect.Right - 1, rect.Top);
                    gfx.DrawLine(darkPen, rect.Right - 1, rect.Top + 1,
                                 rect.Right - 1, rect.Bottom - 1);
                    gfx.DrawLine(darkPen, rect.Right - 1, rect.Bottom - 1,
                                 rect.Left + 1, rect.Bottom - 1);
                    gfx.DrawLine(lightPen, rect.Left + 1, rect.Bottom - 2,
                                 rect.Left + 1, rect.Top + 1);
                    gfx.DrawLine(lightPen, rect.Left + 1, rect.Top + 1,
                                 rect.Right - 2, rect.Top + 1);
                }
                darkDarkPen.Dispose();
                lightLightPen.Dispose();
            }
            darkPen.Dispose();
            lightLightPen.Dispose();
        }

        /// <summary>
        /// Returns the equivalent of the <see cref="System.Drawing.KnownColor.ControlDarkDark"/>
        /// colour for a specified colour.
        /// </summary>
        /// <param name="color">The <see cref="System.Drawing.Color"/> to get the darker
        /// shadow shade for.</param>
        /// <returns>The darker shadow <see cref="System.Drawing.Color"/> for the specified
        /// colour.</returns>
        public static Color ColorDarkDark(Color color)
        {
            if (color.Equals(Color.FromKnownColor(KnownColor.Control)))
            {
                return Color.FromKnownColor(KnownColor.ControlDarkDark);
            }
            else if (color.Equals(Color.Black))
            {
                return Color.FromArgb(color.A, BlackDarkDark);
            }
            else if (color.Equals(Color.White))
            {
                return Color.FromArgb(color.A, WhiteDarkDark);
            }
            else
            {
                int grey = GreyScale(color);
                HLSRGB hls = new HLSRGB(color.R, color.G, color.B);
                if (grey > 250)
                {
                    hls.Luminance = (WhiteDarkDark.R/255F);
                    return Color.FromArgb(color.A, hls.Color);
                }
                else if (grey < 64)
                {
                    hls.Luminance = (BlackDarkDark.R/255F);
                    return Color.FromArgb(color.A, hls.Color);
                }
                else
                {
                    hls.DarkenColor(0.5F);
                    return Color.FromArgb(color.A, hls.Color);
                }
            }
        }

        /// <summary>
        /// Returns the equivalent of the <see cref="System.Drawing.KnownColor.ControlDark"/>
        /// colour for a specified colour.
        /// </summary>
        /// <param name="color">The <see cref="System.Drawing.Color"/> to get the 
        /// shadow shade for.</param>
        /// <returns>The shadow <see cref="System.Drawing.Color"/> for the specified
        /// colour.</returns>
        public static Color ColorDark(Color color)
        {
            if (color.Equals(Color.FromKnownColor(KnownColor.Control)))
            {
                return Color.FromKnownColor(KnownColor.ControlDark);
            }
            else if (color.Equals(Color.Black))
            {
                return Color.FromArgb(color.A, BlackDark);
            }
            else if (color.Equals(Color.White))
            {
                return Color.FromArgb(color.A, WhiteDark);
            }
            else
            {
                int grey = GreyScale(color);
                HLSRGB hls = new HLSRGB(color.R, color.G, color.B);
                if (grey > 250)
                {
                    hls.Luminance = (WhiteDark.R/255F);
                    return Color.FromArgb(color.A, hls.Color);
                }
                else if (grey < 64)
                {
                    hls.Luminance = (BlackDark.R/255F);
                    return Color.FromArgb(color.A, hls.Color);
                }
                else
                {
                    hls.DarkenColor(0.7F);
                    return Color.FromArgb(color.A, hls.Color);
                }
            }
        }

        /// <summary>
        /// Returns the equivalent of the <see cref="System.Drawing.KnownColor.ControlLight"/>
        /// colour for a specified colour.
        /// </summary>
        /// <param name="color">The <see cref="System.Drawing.Color"/> to get the highlight
        /// shade for.</param>
        /// <returns>The highlight <see cref="System.Drawing.Color"/> for the specified
        /// colour.</returns>
        public static Color ColorLight(Color color)
        {
            if (color.Equals(Color.FromKnownColor(KnownColor.Control)))
            {
                return Color.FromKnownColor(KnownColor.ControlLight);
            }
            else if (color.Equals(Color.Black))
            {
                return Color.FromArgb(color.A, BlackLight);
            }
            else if (color.Equals(Color.White))
            {
                return Color.FromArgb(color.A, WhiteLight);
            }
            else
            {
                int grey = GreyScale(color);
                HLSRGB hls = new HLSRGB(color.R, color.G, color.B);
                if (grey > 250)
                {
                    hls.Luminance = (WhiteLight.R/255F);
                    return Color.FromArgb(color.A, hls.Color);
                }
                else if (grey < 64)
                {
                    hls.Luminance = (BlackLight.R/255F);
                    return Color.FromArgb(color.A, hls.Color);
                }
                else
                {
                    hls.LightenColor(0.1F);
                    return Color.FromArgb(color.A, hls.Color);
                }
            }
        }

        /// <summary>
        /// Returns the equivalent of the <see cref="System.Drawing.KnownColor.ControlLightLight"/>
        /// colour for a specified colour.
        /// </summary>
        /// <param name="color">The <see cref="System.Drawing.Color"/> to get the lightest
        /// highlight shade for.</param>
        /// <returns>The lightest highlight <see cref="System.Drawing.Color"/> for the specified
        /// colour.</returns>
        public static Color ColorLightLight(Color color)
        {
            if (color.Equals(Color.FromKnownColor(KnownColor.Control)))
            {
                return Color.FromKnownColor(KnownColor.ControlLightLight);
            }
            else if (color.Equals(Color.Black))
            {
                return Color.FromArgb(color.A, BlackLightLight);
            }
            else if (color.Equals(Color.White))
            {
                return Color.FromArgb(color.A, WhiteLightLight);
            }
            else
            {
                int grey = GreyScale(color);
                HLSRGB hls = new HLSRGB(color.R, color.G, color.B);
                if (grey > 250)
                {
                    hls.Luminance = (WhiteLightLight.R/255F);
                    return Color.FromArgb(color.A, hls.Color);
                }
                else if (grey < 64)
                {
                    hls.Luminance = (BlackLightLight.R/255F);
                    return Color.FromArgb(color.A, hls.Color);
                }
                else
                {
                    hls.LightenColor(0.5F);
                    return Color.FromArgb(color.A, hls.Color);
                }
            }
        }
    }
}