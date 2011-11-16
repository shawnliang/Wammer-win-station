#region

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.ImageCapture.States
{
    internal class RegionState : BaseState
    {
        private Point m_end;
        private bool m_pressed;
        private Point m_start;

        public RegionState(Form owner) : base(owner)
        {
            owner.Cursor = Cursors.Cross;
        }

        #region ICaptureState Members

        public override void Paint(Graphics graphics)
        {
            if (!m_pressed)
                m_start = m_end;

            Rectangle _rect = CalculateRegion(m_start, m_end);
            base.Paint(graphics, _rect);
        }

        public override void Start(Point location)
        {
            m_start = location;
            m_pressed = true;
        }

        public override void Update(Point location)
        {
            m_end = location;
            Owner.Invalidate();
        }

        public override void End()
        {
            m_pressed = false;
        }

        public override Image Capture()
        {
            Rectangle _captureRect = CalculateRegion(m_start, m_end);
            return base.Capture(_captureRect);
        }

        #endregion

        private static Rectangle CalculateRegion(Point start, Point end)
        {
            Rectangle _result = new Rectangle();

            _result.X = Math.Min(start.X, end.X);
            _result.Width = Math.Abs(start.X - end.X);

            _result.Y = Math.Min(start.Y, end.Y);
            _result.Height = Math.Abs(start.Y - end.Y);

            return _result;
        }
    }
}