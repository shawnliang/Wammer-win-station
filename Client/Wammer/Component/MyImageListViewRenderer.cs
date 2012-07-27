#region

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
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

            ItemDrawOrder = ItemDrawOrder.ReverseItemIndex;
        }

        public override Size MeasureItem(View view)
        {
            if (view == View.Thumbnails)
                return ImageListView.ThumbnailSize + new Size(8, 8);

            return base.MeasureItem(view);
        }

        public override void DrawItem(Graphics g, ImageListViewItem item, ItemState state, Rectangle bounds)
        {
            if (ImageListView.View == View.Details)
            {
                base.DrawItem(g, item, state, bounds);
            }
            else
            {
                Rectangle _controlBounds = ClientBounds;

                // Get item image
                Image _img = item.GetCachedImage(CachedImageType.Thumbnail);

                if (_img != null)
                {
                    if ((_img.Width >= ImageListView.ThumbnailSize.Width) ||
                        (_img.Height >= ImageListView.ThumbnailSize.Height))
                    {
                        if (item.FileName != Main.Current.LoadingImagePath)
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

                    if (ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None))
                    {
                        using (Pen _pen = new Pen(ImageListView.Colors.SelectedColor2, 4))
                        {
                            g.DrawRectangle(_pen, new Rectangle(bounds.Left + 2, bounds.Top + 2, bounds.Width - 4, bounds.Height - 4));
                        }

                        using (Pen _pen = new Pen(Color.White, 2))
                        {
                            g.DrawRectangle(_pen, new Rectangle(bounds.Left + 4, bounds.Top + 4, bounds.Width - 8, bounds.Height - 8));
                        }
                    }

                    if (((state & ItemState.Hovered) != ItemState.None))
                    {
                        if (ShowHovered)
                        {
                            using (Brush _bHovered = new SolidBrush(ImageListView.Colors.HoverColor1))
                            {
                                g.FillRectangle(_bHovered, bounds);
                            }
                        }
                    }

                    // Draw the image
                    if (new FileInfo(item.FileName).Name == "LoadingImage.jpg")
                    {
                        int _maxW = 80;
                        int _w = bounds.Width / 2;
                        int _w2;

                        if (_w > _maxW)
                        {
                            _w = _maxW;
                        }

                        _w2 = (bounds.Width - _w) / 2;

                        g.DrawImage(Properties.Resources.photo_spinner, bounds.Left + _w2, bounds.Top + _w2, _w, _w);


                        using (Pen _pen = new Pen(Color.FromArgb(226, 226, 226), 3))
                        {
                            _pen.DashStyle = DashStyle.Dot;

                            g.DrawRectangle(_pen, new Rectangle(bounds.Left + 5, bounds.Top + 5, bounds.Width - 10, bounds.Height - 10));
                        }

                    }
                    else
                    {
                        if (ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None))
                        {
                            ColorMatrix _matrix = new ColorMatrix();
                            _matrix.Matrix33 = 0.618f;

                            ImageAttributes _attributes = new ImageAttributes();
                            _attributes.SetColorMatrix(_matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                            g.DrawImage(_img, _pos, 0, 0, _imageWidth, _imageHeight, GraphicsUnit.Pixel, _attributes);
                        }
                        else
                        {
                            g.DrawImage(_img, _imageX, _imageY, _imageWidth, _imageHeight);
                        }
                    }

                    // Draw image border
                    if (!ItemBorderless)
                    {
                        if (Math.Min(_imageWidth, _imageHeight) > 32)
                        {
                            using (Pen _pOuterBorder = new Pen(ImageListView.Colors.ImageOuterBorderColor))
                            {
                                g.DrawRectangle(_pOuterBorder, _imageX, _imageY, _imageWidth, _imageHeight);
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
                            DrawCoverImageLabel(g, bounds);
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
                        bool _isCoverImage = (item.Tag as EditModeImageListViewItemTag).IsCoverImage_UI;

                        if (_isCoverImage)
                        {
                            DrawCoverImageLabel(g, bounds);
                        }

                        EditModePhotoType _type = ((EditModeImageListViewItemTag)item.Tag).AddPhotoType;

                        if (_type == EditModePhotoType.EditModeNewAdd)
                        {
                            Image _badge = Properties.Resources.photoadded_badge;

                            g.DrawImage(_badge, bounds.Left + bounds.Width - _badge.Width - 4, bounds.Top + bounds.Height - _badge.Height - 4);
                        }
                    }
                }
            }
        }

        private static void DrawCoverImageLabel(Graphics g, Rectangle bounds)
        {
            using (Brush _brush = new SolidBrush(Color.FromArgb(127, 0, 0, 0)))
            {
                Rectangle _r = new Rectangle(bounds.Location, bounds.Size);
                _r.Inflate(-4, -4);
                int _h1 = _r.Height - 24;
                int _h2 = 24;

                Rectangle _rect = new Rectangle(_r.Left, _r.Top + _h1, _r.Width, _h2);

                g.FillRectangle(_brush, _rect);

                int _fontSize = 11;

                if (g.DpiX == 120)
                    _fontSize = 10;

                using (Font _font = new Font(I18n.L.T("DefaultFont"), _fontSize, FontStyle.Bold))
                {
                    TextRenderer.DrawText(g, I18n.L.T("CoverImage"), _font, _rect, Color.White,
                                          TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
        }

        public override void DrawBackground(Graphics g, Rectangle bounds)
        {
            base.DrawBackground(g, bounds);

            if (ImageListView.View == View.Gallery)
            {
                using (Brush _brush = new SolidBrush(Color.FromArgb(211, 207, 207)))
                {
                    Rectangle _r = new Rectangle(bounds.Left, bounds.Height - (ImageListView.ThumbnailSize.Height + 32),
                                                 bounds.Width, (ImageListView.ThumbnailSize.Height + 32));

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