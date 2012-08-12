#region

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.ImageCapture.States
{
    internal abstract class BaseState : ICaptureState
    {
        protected readonly Form Owner;

        private readonly Pen m_borderPen = new Pen(Color.FromArgb(100, 100, 100), 1);
        private readonly SolidBrush m_fillBrush = new SolidBrush(Color.FromArgb(190, Color.Black));

        protected BaseState(Form owner)
        {
            Owner = owner;
        }

        #region ICaptureState Members

        public abstract void Paint(Graphics graphics);

        public virtual void Start(Point location)
        {
        }

        public virtual void Update(Point location)
        {
        }

        public virtual void End()
        {
        }

        public abstract Image Capture();

        #endregion

        protected void Paint(Graphics graphics, Rectangle rect)
        {
            if ((rect.Width > 0) && (rect.Height > 0))
            {
                graphics.ExcludeClip(new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 1, rect.Height - 1));
                graphics.FillRectangle(m_fillBrush, Owner.Bounds);
                graphics.DrawRectangle(m_borderPen, rect);

                //Text label
                string _label = string.Format(" {0} x {1}", rect.Width, rect.Height);
                DrawTextLabel(_label, new Point(rect.Right, rect.Bottom), graphics, false);
            }
            else
            {
                graphics.FillRectangle(m_fillBrush, Owner.Bounds);
            }
        }

        protected void DrawTextLabel(string label, Point pos, Graphics graphics, bool onRightSide)
        {
            SizeF _labelSize = TextRenderer.MeasureText(label, Owner.Font);

            int _x;

            if (onRightSide)
                _x = (pos.X + _labelSize.Width < Owner.Right) ? pos.X : pos.X - (int) _labelSize.Width;
            else
                _x = Math.Max(pos.X - (int) _labelSize.Width, 0);

            int _y = (pos.Y + _labelSize.Height < Owner.Bottom) ? pos.Y : pos.Y - (int) _labelSize.Height;

            if ((pos.Y > _y) && (pos.X + _labelSize.Width < Owner.Right))
                _x = pos.X;

            Rectangle _labelRect = new Rectangle(_x, _y, (int) _labelSize.Width, (int) _labelSize.Height);
            TextRenderer.DrawText(graphics, label, Owner.Font, _labelRect, Color.Black, Color.WhiteSmoke);
        }

        protected Image Capture(Rectangle rect)
        {
            if ((rect.Width == 0) || (rect.Height == 0) || (rect.X < 0) || (rect.Y < 0))
                return null;

            Bitmap _bmpImage = new Bitmap(Owner.BackgroundImage);
            Bitmap _bmpCrop = _bmpImage.Clone(rect, _bmpImage.PixelFormat);
            return (_bmpCrop);
        }
    }
}