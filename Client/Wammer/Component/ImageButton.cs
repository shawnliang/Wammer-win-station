using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Waveface.Component
{
    public class ImageButton : Button
    {
        private Image m_image;
        private Image m_imageFront;
        private Bitmap m_bmpOffscreen;
        private Image m_imageDisable;
        private Image m_imageHover;

        private Brush m_shadowBrush;

        private bool m_hover;
        private bool m_down;

        #region Properties

        public bool CenterAlignImage { get; set; }
        public bool TextShadow { get; set; }

        public Image Image
        {
            get { return m_image; }
            set
            {
                m_image = value;

                Invalidate();
            }
        }

        public Image ImageFront
        {
            get { return m_imageFront; }
            set
            {
                m_imageFront = value;

                Invalidate();
            }
        }

        public Image ImageDisable
        {
            get { return m_imageDisable; }
            set
            {
                m_imageDisable = value;

                Invalidate();
            }
        }

        public Image ImageHover
        {
            get { return m_imageHover; }
            set
            {
                m_imageHover = value;

                Invalidate();
            }
        }

        #endregion

        public ImageButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            // SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            m_shadowBrush = new SolidBrush(Color.FromArgb(127, 0, 0, 0));

            TextShadow = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_bmpOffscreen == null)
                m_bmpOffscreen = new Bitmap(Width, Height);

            Graphics _g = Graphics.FromImage(m_bmpOffscreen);

            _g.TextRenderingHint = TextRenderingHint.AntiAlias;
            _g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            _g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _g.SmoothingMode = SmoothingMode.HighQuality;

            _g.Clear(BackColor);

            if (m_image != null)
            {
                //Center the image relativelly to the control
                int _imageLeft = (Width - m_image.Width) / 2;
                int _imageTop = (Height - m_image.Height) / 2;

                Rectangle _imgRect;

                if (CenterAlignImage)
                {
                    _imgRect = new Rectangle(_imageLeft, _imageTop, m_image.Width, m_image.Height);
                }
                else
                {
                    _imgRect = new Rectangle(0, 0, m_image.Width, m_image.Height);
                }

                //Set transparent key
                ImageAttributes _imageAttr = new ImageAttributes();
                _imageAttr.SetColorKey(BackgroundImageColor(m_image), BackgroundImageColor(m_image));

                Image _img = m_image;

                if (Enabled)
                {
                    if (m_hover)
                    {
                        if (m_imageHover != null)
                            _img = m_imageHover;
                    }
                }
                else
                {
                    if (m_imageDisable != null)
                        _img = m_imageDisable;
                }

                //Draw image
                _g.DrawImage(_img, _imgRect, 0, 0, _img.Width, _img.Height, GraphicsUnit.Pixel, _imageAttr);

                if (m_imageFront != null)
                {
                    _g.DrawImage(m_imageFront, new Rectangle(6, 3, m_imageFront.Width, m_imageFront.Height), 0, 0, m_imageFront.Width, m_imageFront.Height, GraphicsUnit.Pixel, _imageAttr);
                }
            }

            if (!string.IsNullOrEmpty(Text))
            {
                Size _size = TextRenderer.MeasureText(Text, Font);

                if (m_imageFront == null)
                {
                    if (TextShadow)
                    {
                        _g.DrawString(Text, Font, m_shadowBrush, ((Width - _size.Width) / 2) + 3,
                                      ((Height - _size.Height) / 2) + 2);
                    }

                    _g.DrawString(Text, Font, new SolidBrush(ForeColor), ((Width - _size.Width) / 2) + 2,
                                  ((Height - _size.Height) / 2) + 1);
                }
                else
                {
                    int _offX = m_imageFront.Width + 5;

                    if (TextShadow)
                    {
                        _g.DrawString(Text, Font, m_shadowBrush, _offX + (((Width - _offX) - _size.Width) / 2) + 1,
                                      ((Height - _size.Height) / 2) + 1);
                    }

                    _g.DrawString(Text, Font, new SolidBrush(ForeColor), _offX + ((Width - _offX) - _size.Width) / 2,
                                  ((Height - _size.Height) / 2));
                }
            }

            //Draw from the memory bitmap
            e.Graphics.DrawImage(m_bmpOffscreen, 0, 0);

            //base.OnPaint(e);
        }

        private Color BackgroundImageColor(Image img)
        {
            // Bitmap _bmp = new Bitmap(img);
            // return _bmp.GetPixel(0, 0);

            return BackColor;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            m_hover = true;

            Refresh();

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            m_hover = false;

            Refresh();

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_down = true;

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            m_down = false;

            base.OnMouseUp(e);
        }
    }
}