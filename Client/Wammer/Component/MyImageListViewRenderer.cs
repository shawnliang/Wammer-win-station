#region

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using Manina.Windows.Forms;
using Waveface.DetailUI;
using View = Manina.Windows.Forms.View;

#endregion

namespace Waveface.Component
{
    public class MyImageListViewRenderer : ImageListView.ImageListViewRenderer
    {
        private bool m_showHovered = true;

        public bool ItemBorderless { get; set; }

        public bool ShowHovered
        {
            get { return m_showHovered; }
            set { m_showHovered = value; }
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
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;

            // Clip = (ImageListView.View == View.Details);

            if (ImageListView.View == View.Details)
            {
                base.DrawItem(g, item, state, bounds);
            }
            else
            {
                Rectangle _controlBounds = ClientBounds;

                // Get item image
                Image _img = null;

                if ((state & ItemState.Hovered) != ItemState.None)
                    _img = GetImageAsync(item, new Size(bounds.Width - 8, bounds.Height - 8));

                if (_img == null)
                    _img = item.GetCachedImage(CachedImageType.Thumbnail);

                if (_img != null)
                {
                    if ((_img.Width >= ImageListView.ThumbnailSize.Width) ||
                        (_img.Height >= ImageListView.ThumbnailSize.Height))
                    {
                        _img = ImageUtility.GenerateSquareImage(_img, ImageListView.ThumbnailSize.Width);
                    }

                    // Calculate image bounds
                    Rectangle _pos = GetSizedImageBounds(_img, Rectangle.Inflate(bounds, -4, -4));
                    int _imageWidth = _pos.Width;
                    int _imageHeight = _pos.Height;
                    int _imageX = _pos.X;
                    int _imageY = _pos.Y;

                    // Paint background
                    if (ImageListView.Enabled)
                    {
                        if (ImageListView.View == View.Gallery)
                        {
                            using (Brush _brush = new SolidBrush(Color.FromArgb(211, 207, 207)))
                            {
                                g.FillRectangle(_brush, bounds);
                            }
                        }
                        else
                        {
                            using (Brush _bItemBack = new SolidBrush(ImageListView.Colors.BackColor))
                            {
                                g.FillRectangle(_bItemBack, bounds);
                            }
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
                        (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None) &&
                         ((state & ItemState.Hovered) != ItemState.None)))
                    {
                        using (
                            Brush _bSelected = new LinearGradientBrush(bounds, ImageListView.Colors.SelectedColor1,
                                                                       ImageListView.Colors.SelectedColor2,
                                                                       LinearGradientMode.Vertical))
                        {
                            //Utility.FillRoundedRectangle(g, _bSelected, bounds, 5);

                            g.FillRectangle(_bSelected, bounds);
                        }
                    }
                    else if (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None))
                    {
                        using (
                            Brush _bGray64 = new LinearGradientBrush(bounds, ImageListView.Colors.UnFocusedColor1,
                                                                     ImageListView.Colors.UnFocusedColor2,
                                                                     LinearGradientMode.Vertical))
                        {
                            //Utility.FillRoundedRectangle(g, _bGray64, bounds, 5);
                            g.FillRectangle(_bGray64, bounds);
                        }
                    }

                    if (((state & ItemState.Hovered) != ItemState.None))
                    {
                        if (ShowHovered)
                        {
                            using (
                                Brush _bHovered = new LinearGradientBrush(bounds, ImageListView.Colors.HoverColor1,
                                                                          ImageListView.Colors.HoverColor2,
                                                                          LinearGradientMode.Vertical))
                            {
                                // Utility.FillRoundedRectangle(g, _bHovered, bounds, 5);
                                g.FillRectangle(_bHovered, bounds);
                            }
                        }
                    }

                    // Draw the image
                    g.DrawImage(_img, _imageX, _imageY, _imageWidth, _imageHeight);

                    // Draw image border
                    if (!ItemBorderless)
                    {
                        if (Math.Min(_imageWidth, _imageHeight) > 32)
                        {
                            using (Pen _pOuterBorder = new Pen(ImageListView.Colors.ImageOuterBorderColor))
                            {
                                g.DrawRectangle(_pOuterBorder, _imageX, _imageY, _imageWidth, _imageHeight);
                            }

                            if (Math.Min(_imageWidth, _imageHeight) > 32)
                            {
                                /*
                                using (Pen _pInnerBorder = new Pen(ImageListView.Colors.ImageInnerBorderColor))
                                {
                                    g.DrawRectangle(_pInnerBorder, _imageX + 1, _imageY + 1, _imageWidth - 2,
                                                    _imageHeight - 2);
                                }
                                */
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
                /*
                using (Pen _pWhite128 = new Pen(Color.FromArgb(128, ImageListView.Colors.ControlBackColor)))
                {
                    Utility.DrawRoundedRectangle(g, _pWhite128, bounds.Left + 1, bounds.Top + 1, bounds.Width - 3,
                                                 bounds.Height - 3, 4);
                }

                if (ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None))
                {
                    using (Pen _pHighlight128 = new Pen(ImageListView.Colors.SelectedBorderColor))
                    {
                        Utility.DrawRoundedRectangle(g, _pHighlight128, bounds.Left, bounds.Top, bounds.Width - 1,
                                                     bounds.Height - 1, 4);
                    }
                }
                else if (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None))
                {
                    using (Pen _pGray128 = new Pen(ImageListView.Colors.UnFocusedBorderColor))
                    {
                        Utility.DrawRoundedRectangle(g, _pGray128, bounds.Left, bounds.Top, bounds.Width - 1,
                                                     bounds.Height - 1, 4);
                    }
                }
                else if ((state & ItemState.Selected) == ItemState.None)
                {
                    using (Pen _pGray64 = new Pen(ImageListView.Colors.BorderColor))
                    {
                        Utility.DrawRoundedRectangle(g, _pGray64, bounds.Left, bounds.Top, bounds.Width - 1,
                                                     bounds.Height - 1, 4);
                    }
                }
                */

                if (ImageListView.Focused && ((state & ItemState.Hovered) != ItemState.None))
                {
                    using (Pen _pHighlight64 = new Pen(ImageListView.Colors.HoverBorderColor))
                    {
                        Utility.DrawRoundedRectangle(g, _pHighlight64, bounds.Left, bounds.Top, bounds.Width - 1,
                                                     bounds.Height - 1, 4);
                    }
                }

                if (item.Tag != null)
                {
                    if (item.Tag is DetailViewImageListViewItemTag)
                    {
                        bool _isCoverImage = (item.Tag as DetailViewImageListViewItemTag).IsCoverImage;

                        if (_isCoverImage)
                        {
                            using (Brush _brush = new SolidBrush(Color.FromArgb(127, 0, 0, 0)))
                            {
                                Rectangle _r = new Rectangle(bounds.Location, bounds.Size);
                                _r.Inflate(-4, -4);
                                int _h1 = (int)(_r.Height * 0.8);
                                int _h2 = _r.Height - _h1;

                                Rectangle _rect = new Rectangle(_r.Left, _r.Top + _h1, _r.Width, _h2);

                                g.FillRectangle(_brush, _rect);

                                Font _font = new Font(I18n.L.T("DefaultFont"), _h2 * (1 - 0.6f), FontStyle.Bold);

                                TextRenderer.DrawText(g, I18n.L.T("CoverImage"), _font, _rect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                            }
                        }
                    }

                    if (item.Tag is ImageViewItemTAG)
                    {
                        string _type = (item.Tag as ImageViewItemTAG).Type;

                        if (_type == "Medium" || _type == "Loading")
                        {
                            g.FillRectangle(Brushes.OrangeRed, bounds.Left + 2, bounds.Top + 2, 1, 1);
                        }
                    }

                    if (item.Tag is EditModeImageListViewItemTag)
                    {
                        EditModePhotoType _type = ((EditModeImageListViewItemTag)item.Tag).AddPhotoType;

                        if (_type == EditModePhotoType.EditModeNewAdd)
                        {
                            using (Pen _pen = new Pen(Color.FromArgb(215, 131, 123), 3))
                            {
                                g.DrawRectangle(_pen, bounds.Left + 4, bounds.Top + 4, bounds.Width - 8, bounds.Height - 8);
                            }
                        }
                    }
                }
            }
        }

        /*
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
        */

        public override void DrawBackground(Graphics g, Rectangle bounds)
        {
            base.DrawBackground(g, bounds);

            if (ImageListView.View == View.Gallery)
            {
                using (Brush _brush = new SolidBrush(Color.FromArgb(211, 207, 207)))
                {
                    Rectangle _r = new Rectangle(bounds.Left, bounds.Height - (ImageListView.ThumbnailSize.Height + 32), bounds.Width, (ImageListView.ThumbnailSize.Height + 32));

                    g.FillRectangle(_brush, _r);
                }
            }
        }

        public override void DrawGalleryImage(Graphics g, ImageListViewItem item, Image image, Rectangle bounds)
        {
            /*
            if (item != null && image != null)
            {
                // Calculate image bounds
                Size _itemMargin = MeasureItemMargin(ImageListView.View);
                Rectangle _pos = GetSizedImageBounds(image,
                                                     new Rectangle(bounds.Location + _itemMargin,
                                                                   bounds.Size - _itemMargin - _itemMargin));

                //if ((bounds.Size.Width < item.Dimensions.Width) || (bounds.Size.Height < item.Dimensions.Height))
                //{
                //    Bitmap _bmp = new Bitmap(item.FileName);
                //    g.DrawImage(_bmp, _pos, new Rectangle(0, 0, _bmp.Width, _bmp.Height), GraphicsUnit.Pixel);
                //    _bmp = null;
                //}
                //else
                {
                    if (image.Width > ImageListView.ThumbnailSize.Width * 2)
                        g.DrawImage(image, _pos);
                }

                // Draw image border
                if (Math.Min(_pos.Width, _pos.Height) > ImageListView.ThumbnailSize.Width * 2)
                {
                    using (Pen _pOuterBorder = new Pen(ImageListView.Colors.ImageOuterBorderColor))
                    using (Pen _pInnerBorder = new Pen(ImageListView.Colors.ImageInnerBorderColor))
                    {
                        g.DrawRectangle(_pOuterBorder, _pos);
                        //g.DrawRectangle(_pInnerBorder, Rectangle.Inflate(_pos, -1, -1));
                    }
                }
            }
            */
        }

        #region GetSizedImageBounds

        internal static Size GetSizedImageBounds(Image image, Size fit)
        {
            float _f = Math.Max(image.Width / (float)fit.Width, image.Height / (float)fit.Height);

            if (_f < 1.0f)
                _f = 1.0f; // Do not upsize small images

            int _width = (int)Math.Round(image.Width / _f);
            int _height = (int)Math.Round(image.Height / _f);
            return new Size(_width, _height);
        }

        public static Rectangle GetSizedImageBounds(Image image, Rectangle fit, float hAlign, float vAlign)
        {
            if (hAlign < 0 || hAlign > 100.0f)
                throw new ArgumentException("hAlign must be between 0.0 and 100.0 (inclusive).", "hAlign");

            if (vAlign < 0 || vAlign > 100.0f)
                throw new ArgumentException("vAlign must be between 0.0 and 100.0 (inclusive).", "vAlign");

            Size _scaled = GetSizedImageBounds(image, fit.Size);
            int _x = fit.Left + (int)(hAlign / 100.0f * (fit.Width - _scaled.Width));
            int _y = fit.Top + (int)(vAlign / 100.0f * (fit.Height - _scaled.Height));

            return new Rectangle(_x, _y, _scaled.Width, _scaled.Height);
        }

        public static Rectangle GetSizedImageBounds(Image image, Rectangle fit)
        {
            return GetSizedImageBounds(image, fit, 50.0f, 50.0f);
        }

        #endregion
    }
}