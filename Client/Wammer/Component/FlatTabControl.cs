#region

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

#endregion

namespace Waveface.Windows.Forms
{
    /// <summary>
    /// FlatTabControl is an owner-draw TabControl that paints the tab non-client area in a
    /// specified background color rather than always painting in the default system colors.
    /// Also, paints a flat border around the control rather than the default Windows
    /// 3D border.
    /// 
    /// Since this control derives from the WinForms TabControl, it supports all of the mouse,
    /// keyboard, tab pages, control containment, etc that the base control does.  All we're 
    /// over-writing the way that its drawn.
    /// 
    /// This control also exposes additional virtual drawing methods.  That way, any developer
    /// that wants to customize or specialize an specific aspect of the control drawing can
    /// just override that method.
    /// 
    /// Finally, this control also supports owner draw (DrawMode property and DrawItem event),
    /// so that developers that want to customize it in that way can set the DrawMode and 
    /// respond to the event.  However, the owner draw just gives you a chance to draw the
    /// inner portion of the tab page button itself not the button borders or any other part
    /// of the control.
    /// </summary>
    public class FlatTabControl : DragDropTabControl
    {
        #region FlatTabAlignment enum

        public enum FlatTabAlignment
        {
            Top = 0,
            Bottom
        }

        #endregion

        #region private members

        // initial colors with windows defaults for them.
        private Color backColor = Color.FromKnownColor(KnownColor.ControlLight);
        // private Color trackingColor = Color.FromKnownColor(KnownColor.Highlight);
        private Color borderDarkColor = Color.FromKnownColor(KnownColor.ControlLightLight);
        private Color borderLightColor = Color.FromKnownColor(KnownColor.ControlDarkDark);
        private ImageAttributes imageAttr;
        private int radius = 3;
        private StringFormat stringFormat;
        private Color textColor = Color.FromKnownColor(KnownColor.ControlText);
        private bool m_showTabsHeader = true;

        #endregion

        #region public properties

        [Browsable(true), Category("Appearance")]
        public bool ShowTabsHeader
        {
            get
            {
                return m_showTabsHeader;
            }
            set
            {
                m_showTabsHeader = value;
                Invalidate();
            }
        }

        [Browsable(true), Category("Appearance")]
        // Expose an ControlBackColor property for this TabControl.
            public virtual Color ControlBackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
                Invalidate();
            }
        }

        [Browsable(true), Category("Appearance")]
        // Expose an TextColor property for this TabControl.
            public virtual Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                textColor = value;
                Invalidate();
            }
        }

        [Browsable(true), Category("Appearance")]
        // Expose an BorderDarkColor property for this TabControl.
            public virtual Color BorderDarkColor
        {
            get
            {
                return borderDarkColor;
            }
            set
            {
                borderDarkColor = value;
                Invalidate();
            }
        }

        [Browsable(true), Category("Appearance")]
        // Expose an BorderLightColor property for this TabControl.
            public virtual Color BorderLightColor
        {
            get
            {
                return borderLightColor;
            }
            set
            {
                borderLightColor = value;
                Invalidate();
            }
        }

        [Browsable(true), Category("Behavior")]
        // Expose an TabAlignment property for this control -- we only support alignment
            // on top and bottom (not on sides).
            public FlatTabAlignment TabAlignment
        {
            get
            {
                return (FlatTabAlignment) base.Alignment;
            }
            set
            {
                base.Alignment = (TabAlignment) value;
            }
        }

        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        // Hide the Alignment so it can't be set.  This control uses FlatTabAlignment instead.
            public new TabAlignment Alignment
        {
            get
            {
                return base.Alignment;
            }
            set
            {
            }
        }

        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        // Hide the HotTrack so it can't be set.  This control doesn't draw that.
            public new bool HotTrack
        {
            get
            {
                return base.HotTrack;
            }
            set
            {
            }
        }

        [Browsable(false)]
        // set the StringFormat the first time it's asked for.
            protected StringFormat StringFormat
        {
            get
            {
                if (stringFormat == null)
                {
                    // set up the format for the text display.
                    stringFormat = new StringFormat(StringFormatFlags.NoWrap);
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;
                    stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                }

                return stringFormat;
            }
        }

        [Browsable(false)]
        // set the ImageAttributes the first time it's asked for.
            protected ImageAttributes ImageAttributes
        {
            get
            {
                if (imageAttr == null)
                {
                    imageAttr = new ImageAttributes();
                    imageAttr.SetColorKey(ImageList.TransparentColor, ImageList.TransparentColor,
                                          ColorAdjustType.Default);
                }

                return imageAttr;
            }
        }

        #endregion

        public FlatTabControl()
        {
            // Initialize the control styles for painting.
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        #region TabControl overrides

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // draw the tab background in the same color as the main window's background.
            using (Brush brushBkGnd = new SolidBrush(ControlBackColor))
            using (Pen penBorderDark = new Pen(BorderDarkColor))
            using (Pen penBorderLight = new Pen(BorderLightColor))
            {
                // first, paint in the control's background color.
                g.FillRectangle(brushBkGnd, ClientRectangle);

                // then, draw in its flat border.
                DrawControlBorder(g, penBorderLight, penBorderDark);

                if (m_showTabsHeader)
                {
                    // finally, loop through each tab page button and draw it in.
                    for (int ctr = 0; ctr < TabCount; ctr++)
                        DrawTabButtonFull(g, ctr, brushBkGnd, penBorderLight, penBorderDark);
                }
            }

            base.OnPaint(e);
        }

        #endregion

        #region FlatTabControl virtual drawing methods

        protected virtual void DrawControlBorder(Graphics g, Pen penBorderLight, Pen penBorderDark)
        {
            Rectangle rect;
            if (Appearance == TabAppearance.Normal)
            {
                // it it's normal mode, then only draw the rectangle around the display area
                // not the area where the page tabs are painted.
                int border = DisplayRectangle.X - ClientRectangle.X;
                int displayBottom = (TabAlignment == FlatTabAlignment.Top) ?
                                                                               ClientRectangle.Bottom - DisplayRectangle.Y : DisplayRectangle.Bottom - 1;

                rect = new Rectangle(0, DisplayRectangle.Y - border, ClientRectangle.Width - 1,
                                     displayBottom + border - 1);
            }
            else
            {
                // if not normal mode, then draw the border around the whole control.
                rect = new Rectangle(0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
            }

            // draw the control's border.
            g.DrawLine(penBorderLight, rect.Left, rect.Bottom, rect.Left, rect.Top);
            g.DrawLine(penBorderLight, rect.Left, rect.Top, rect.Right, rect.Top);
            g.DrawLine(penBorderDark, rect.Right, rect.Top, rect.Right, rect.Bottom);
            g.DrawLine(penBorderDark, rect.Right, rect.Bottom, rect.Left, rect.Bottom);
        }

        protected virtual void DrawButtonBorder(Graphics g, Pen penBorderLight, Pen penBorderDark,
                                                Rectangle rect)
        {
            // if it's not normal appearance (either buttons or flat buttons), then we
            // drawn rectangular borders with no rounding.
            bool isNormal = (Appearance == TabAppearance.Normal);
            int rad = (isNormal) ? radius : 0;

            if (TabAlignment == FlatTabAlignment.Top)
            {
                // if its aligned to top, then draw the rounded parts on the top as well.
                g.DrawLine(penBorderLight, rect.Left, rect.Bottom, rect.Left, rect.Top + rad);
                if (isNormal)
                    g.DrawLine(penBorderLight, rect.Left, rect.Top + rad, rect.Left + rad, rect.Top);
                g.DrawLine(penBorderLight, rect.Left + rad, rect.Top, rect.Right - rad, rect.Top);
                if (isNormal)
                    g.DrawLine(penBorderDark, rect.Right - rad, rect.Top, rect.Right, rect.Top + rad);
                g.DrawLine(penBorderDark, rect.Right, rect.Top + rad, rect.Right, rect.Bottom);
                if (!isNormal)
                    g.DrawLine(penBorderDark, rect.Right, rect.Bottom, rect.Left, rect.Bottom);
            }
            else
            {
                // if its aligned to bottom, then the rounded parts must also be on the bottom.
                g.DrawLine(penBorderLight, rect.Left, rect.Bottom - rad, rect.Left, rect.Top);
                if (!isNormal)
                    g.DrawLine(penBorderLight, rect.Left, rect.Top, rect.Right, rect.Top);
                g.DrawLine(penBorderDark, rect.Right, rect.Top, rect.Right, rect.Bottom - rad);
                if (isNormal)
                    g.DrawLine(penBorderDark, rect.Right, rect.Bottom - rad, rect.Right - rad, rect.Bottom);
                g.DrawLine(penBorderDark, rect.Right - rad, rect.Bottom, rect.Left + rad, rect.Bottom);
                if (isNormal)
                    g.DrawLine(penBorderLight, rect.Left + rad, rect.Bottom, rect.Left, rect.Bottom - rad);
            }
        }

        protected virtual void DrawTabButton(Graphics g, int pageIndex, Brush brushBkGnd,
                                             Brush brushText, Rectangle bounds, bool isSelected)
        {
            Font fontText = Font;
            // bold the font for the selected tab.
            if (isSelected)
            {
                fontText = new Font(Font, FontStyle.Bold);
            }
            g.FillRectangle(brushBkGnd, bounds);

            // draw the image if one is specified.
            int imageIndex = TabPages[pageIndex].ImageIndex;
            if (ImageList != null && imageIndex != -1 && imageIndex < ImageList.Images.Count)
            {
                ImageList imageList = ImageList;
                Size imageSize = imageList.ImageSize;

                // need to DrawImage with the appropriate ImageAttributes, so that it will
                // blend colors correctly when selection is set.
                g.DrawImage(imageList.Images[imageIndex], new Rectangle(bounds.Location, imageSize),
                            0, 0, imageSize.Width, imageSize.Height, GraphicsUnit.Pixel,
                            ImageAttributes);

                bounds.Width -= ImageList.ImageSize.Width;
                bounds.X += ImageList.ImageSize.Width;
            }

            // now draw the text for the tab.
            g.DrawString(TabPages[pageIndex].Text, fontText, brushText, bounds, StringFormat);

            // finally if a new font was created in this method, then dispose of it.
            if (fontText != Font)
            {
                fontText.Dispose();
            }
        }

        protected virtual void FixupSelectedBorder(Graphics g, Rectangle bounds, bool isSelected)
        {
            // only need to fixup the border if we're in normal mode, and it's the currently
            // selected tab page.
            if (isSelected && Appearance == TabAppearance.Normal)
            {
                using (Pen penBkGnd = new Pen(ControlBackColor))
                {
                    // draw the fixup line either on top or bottom of button's bounds.
                    int y = (TabAlignment == FlatTabAlignment.Top) ? bounds.Bottom : bounds.Top;
                    g.DrawLine(penBkGnd, bounds.Left + 1, y, bounds.Right - 1, y);
                }
            }
        }

        protected virtual void PerformDrawItem(Graphics g, Rectangle bounds, int pageIndex,
                                               Color textColor, bool isSelected)
        {
            // if we're in owner-draw mode, then we need to create the appropriate
            // EventArgs before firing the DrawItem event.
            DrawItemState state = DrawItemState.Default;
            if (!TabPages[pageIndex].Enabled)
                state = DrawItemState.Disabled;
            if (isSelected)
                state |= DrawItemState.Selected;

            DrawItemEventArgs e = new DrawItemEventArgs(g, Font, bounds, pageIndex, state,
                                                        textColor, ControlBackColor);
            OnDrawItem(e);
        }

        protected virtual void DrawTabButtonFull(Graphics g, int pageIndex, Brush brushBkGnd,
                                                 Pen penBorderLight, Pen penBorderDark)
        {
            bool isSelected = (pageIndex == SelectedIndex);
            Rectangle rect = GetTabRect(pageIndex);

            // update the drawing rectangle, and draw the button borders.
            rect.Height -= 2;
            rect.Width--;
            DrawButtonBorder(g, penBorderLight, penBorderDark, rect);

            // fixup the selected button bottom border.
            FixupSelectedBorder(g, rect, isSelected);

            // make sure we calculate the appropriate size for the inner piece of a tab button.
            rect.Inflate(-radius, (Appearance == TabAppearance.Normal) ? -2 : -radius);

            // if it's normal draw mode, then we draw it.  if it's owner draw, we need
            // to fire the appropriate DrawItem event.
            if (DrawMode == TabDrawMode.Normal)
            {
                // draw the tab button itself.
                using (Brush brushText = new SolidBrush(textColor))
                {
                    DrawTabButton(g, pageIndex, brushBkGnd, brushText, rect, isSelected);
                }
            }
            else
            {
                // if we're in owner-draw mode, then we need to create the appropriate
                // EventArgs before firing the DrawItem event.
                PerformDrawItem(g, rect, pageIndex, textColor, isSelected);
            }
        }

        #endregion
    }
}