#region

using System.Drawing;

#endregion

namespace Waveface.Compoment.PopupControl
{
    internal struct GripBounds
    {
        private const int GripSize = 6;
        private const int CornerGripSize = GripSize << 1;

        private Rectangle m_clientRectangle;

        public GripBounds(Rectangle clientRectangle)
        {
            m_clientRectangle = clientRectangle;
        }

        public Rectangle ClientRectangle
        {
            get
            {
                return m_clientRectangle;
            }
        }

        public Rectangle Bottom
        {
            get
            {
                Rectangle _rect = ClientRectangle;
                _rect.Y = _rect.Bottom - GripSize + 1;
                _rect.Height = GripSize;
                return _rect;
            }
        }

        public Rectangle BottomRight
        {
            get
            {
                Rectangle _rect = ClientRectangle;
                _rect.Y = _rect.Bottom - CornerGripSize + 1;
                _rect.Height = CornerGripSize;
                _rect.X = _rect.Width - CornerGripSize + 1;
                _rect.Width = CornerGripSize;
                return _rect;
            }
        }

        public Rectangle Top
        {
            get
            {
                Rectangle _rect = ClientRectangle;
                _rect.Height = GripSize;
                return _rect;
            }
        }

        public Rectangle TopRight
        {
            get
            {
                Rectangle _rect = ClientRectangle;
                _rect.Height = CornerGripSize;
                _rect.X = _rect.Width - CornerGripSize + 1;
                _rect.Width = CornerGripSize;
                return _rect;
            }
        }

        public Rectangle Left
        {
            get
            {
                Rectangle _rect = ClientRectangle;
                _rect.Width = GripSize;
                return _rect;
            }
        }

        public Rectangle BottomLeft
        {
            get
            {
                Rectangle _rect = ClientRectangle;
                _rect.Width = CornerGripSize;
                _rect.Y = _rect.Height - CornerGripSize + 1;
                _rect.Height = CornerGripSize;
                return _rect;
            }
        }

        public Rectangle Right
        {
            get
            {
                Rectangle _rect = ClientRectangle;
                _rect.X = _rect.Right - GripSize + 1;
                _rect.Width = GripSize;
                return _rect;
            }
        }

        public Rectangle TopLeft
        {
            get
            {
                Rectangle _rect = ClientRectangle;
                _rect.Width = CornerGripSize;
                _rect.Height = CornerGripSize;
                return _rect;
            }
        }
    }
}