
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Manina.Windows.Forms;

namespace Waveface.Component
{
    public class MyImageListViewRenderer : ImageListView.ImageListViewRenderer
    {
        private float m_zoomRatio;

        public float ZoomRatio
        {
            get
            {
                return m_zoomRatio;
            }
            set
            {
                m_zoomRatio = value;

                if (m_zoomRatio < 0.0f)
                    m_zoomRatio = 0.0f;
            }
        }

        public MyImageListViewRenderer()
            : this(0.0f)
        {
        }

        public MyImageListViewRenderer(float zoomRatio)
        {
            if (zoomRatio < 0.0f)
                zoomRatio = 0.0f;

            m_zoomRatio = zoomRatio;
        }

        public override void InitializeGraphics(Graphics g)
        {
            base.InitializeGraphics(g);

            ItemDrawOrder = ItemDrawOrder.NormalSelectedHovered;
        }

        public override Size MeasureItem(View view)
        {
            if (view == View.Thumbnails)
                return ImageListView.ThumbnailSize + new Size(8, 8);
            else
                return base.MeasureItem(view);
        }

        public override void DrawItem(Graphics g, ImageListViewItem item, ItemState state, Rectangle bounds)
        {
            Clip = (ImageListView.View == View.Details);

            if (ImageListView.View == View.Details)
            {
                base.DrawItem(g, item, state, bounds);
            }
            else
            {
                Rectangle _controlBounds = ClientBounds;

                // Zoom on mouse over
                if ((state & ItemState.Hovered) != ItemState.None)
                {
                    bounds.Inflate((int)(bounds.Width * m_zoomRatio), (int)(bounds.Height * m_zoomRatio));

                    if (bounds.Bottom > _controlBounds.Bottom)
                        bounds.Y = _controlBounds.Bottom - bounds.Height;

                    if (bounds.Top < _controlBounds.Top)
                        bounds.Y = _controlBounds.Top;

                    if (bounds.Right > _controlBounds.Right)
                        bounds.X = _controlBounds.Right - bounds.Width;

                    if (bounds.Left < _controlBounds.Left)
                        bounds.X = _controlBounds.Left;
                }

                // Get item image
                Image _img = null;

                if ((state & ItemState.Hovered) != ItemState.None)
                    _img = GetImageAsync(item, new Size(bounds.Width - 8, bounds.Height - 8));

                if (_img == null)
                    _img = item.GetCachedImage(CachedImageType.Thumbnail);

                if (_img != null)
                {
                    // Calculate image bounds
                    Rectangle _pos = Utility.GetSizedImageBounds(_img, Rectangle.Inflate(bounds, -4, -4));
                    int _imageWidth = _pos.Width;
                    int _imageHeight = _pos.Height;
                    int _imageX = _pos.X;
                    int _imageY = _pos.Y;

                    // Paint background
                    if (ImageListView.Enabled)
                    {
                        using (Brush _bItemBack = new SolidBrush(ImageListView.Colors.BackColor))
                        {
                            g.FillRectangle(_bItemBack, bounds);
                        }
                    }
                    else
                    {
                        using (Brush _bItemBack = new SolidBrush(ImageListView.Colors.DisabledBackColor))
                        {
                            g.FillRectangle(_bItemBack, bounds);
                        }
                    }

                    if ((ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None)) ||
                        (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None) && ((state & ItemState.Hovered) != ItemState.None)))
                    {
                        using (Brush _bSelected = new LinearGradientBrush(bounds, ImageListView.Colors.SelectedColor1, ImageListView.Colors.SelectedColor2, LinearGradientMode.Vertical))
                        {
                            Utility.FillRoundedRectangle(g, _bSelected, bounds, 5);
                        }
                    }
                    else if (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None))
                    {
                        using (Brush _bGray64 = new LinearGradientBrush(bounds, ImageListView.Colors.UnFocusedColor1, ImageListView.Colors.UnFocusedColor2, LinearGradientMode.Vertical))
                        {
                            Utility.FillRoundedRectangle(g, _bGray64, bounds, 5);
                        }
                    }

                    if (((state & ItemState.Hovered) != ItemState.None))
                    {
                        using (Brush _bHovered = new LinearGradientBrush(bounds, ImageListView.Colors.HoverColor1, ImageListView.Colors.HoverColor2, LinearGradientMode.Vertical))
                        {
                            Utility.FillRoundedRectangle(g, _bHovered, bounds, 5);
                        }
                    }

                    // Draw the image
                    g.DrawImage(_img, _imageX, _imageY, _imageWidth, _imageHeight);

                    // Draw image border
                    if (Math.Min(_imageWidth, _imageHeight) > 32)
                    {
                        using (Pen _pOuterBorder = new Pen(ImageListView.Colors.ImageOuterBorderColor))
                        {
                            g.DrawRectangle(_pOuterBorder, _imageX, _imageY, _imageWidth, _imageHeight);
                        }

                        if (Math.Min(_imageWidth, _imageHeight) > 32)
                        {
                            using (Pen _pInnerBorder = new Pen(ImageListView.Colors.ImageInnerBorderColor))
                            {
                                g.DrawRectangle(_pInnerBorder, _imageX + 1, _imageY + 1, _imageWidth - 2, _imageHeight - 2);
                            }
                        }
                    }
                }
                else
                {
                    // Paint background
                    if (ImageListView.Enabled)
                    {
                        using (Brush _bItemBack = new SolidBrush(ImageListView.Colors.BackColor))
                        {
                            g.FillRectangle(_bItemBack, bounds);
                        }
                    }
                    else
                    {
                        using (Brush _bItemBack = new SolidBrush(ImageListView.Colors.DisabledBackColor))
                        {
                            g.FillRectangle(_bItemBack, bounds);
                        }
                    }
                }

                // Item border
                using (Pen _pWhite128 = new Pen(Color.FromArgb(128, ImageListView.Colors.ControlBackColor)))
                {
                    Utility.DrawRoundedRectangle(g, _pWhite128, bounds.Left + 1, bounds.Top + 1, bounds.Width - 3, bounds.Height - 3, 4);
                }

                if (ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None))
                {
                    using (Pen _pHighlight128 = new Pen(ImageListView.Colors.SelectedBorderColor))
                    {
                        Utility.DrawRoundedRectangle(g, _pHighlight128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, 4);
                    }
                }
                else if (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None))
                {
                    using (Pen _pGray128 = new Pen(ImageListView.Colors.UnFocusedBorderColor))
                    {
                        Utility.DrawRoundedRectangle(g, _pGray128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, 4);
                    }
                }
                else if ((state & ItemState.Selected) == ItemState.None)
                {
                    using (Pen _pGray64 = new Pen(ImageListView.Colors.BorderColor))
                    {
                        Utility.DrawRoundedRectangle(g, _pGray64, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, 4);
                    }
                }

                if (ImageListView.Focused && ((state & ItemState.Hovered) != ItemState.None))
                {
                    using (Pen _pHighlight64 = new Pen(ImageListView.Colors.HoverBorderColor))
                    {
                        Utility.DrawRoundedRectangle(g, _pHighlight64, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, 4);
                    }
                }
            }
        }

        public override void DrawCheckBox(Graphics g, ImageListViewItem item, Rectangle bounds)
        {
            if (ImageListView.View == View.Details)
                base.DrawCheckBox(g, item, bounds);
        }

        public override void DrawFileIcon(Graphics g, ImageListViewItem item, Rectangle bounds)
        {
            if (ImageListView.View == View.Details)
                base.DrawFileIcon(g, item, bounds);
        }
    }
}
