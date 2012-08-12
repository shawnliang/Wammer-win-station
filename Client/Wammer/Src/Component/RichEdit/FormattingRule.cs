
#region

using System.Text.RegularExpressions;

#endregion

namespace Waveface.Component.RichEdit
{
    public class FormattingRule
    {
        private Format m_format;
        private Regex m_regex;

        public Regex Regex
        {
            get { return m_regex; }
        }

        public Format Format
        {
            get { return m_format; }
        }

        public FormattingRule(Regex regex, Format format)
        {
            m_regex = regex;
            m_format = format;
        }
    }
}