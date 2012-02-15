namespace Waveface.Component.RichEdit
{
    public class UnderlineFormat
    {
        private UnderlineStyle m_style;
        private UnderlineColor m_color;
        
        public UnderlineStyle Style
        {
            get { return m_style; }
            set { m_style = value; }
        }

        public UnderlineColor Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        public UnderlineFormat(UnderlineStyle style, UnderlineColor color)
        {
            m_style = style;
            m_color = color;
        }
    }
}