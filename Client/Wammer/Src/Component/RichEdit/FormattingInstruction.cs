
namespace Waveface.Component.RichEdit
{
    public class FormattingInstruction
    {
        private int m_length;
        private int m_start;
        private Format m_format;

        public int Length
        {
            get { return m_length; }
            set { m_length = value; }
        }

        public int Start
        {
            get { return m_start; }
            set { m_start = value; }
        }

        public Format Format
        {
            get { return m_format; }
            set { m_format = value; }
        }

        public FormattingInstruction()
        {
        }

        public FormattingInstruction(int start, int length, Format format)
        {
            m_start = start;
            m_length = length;
            Format = format;
        }
    }
}