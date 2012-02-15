#region

using System;

#endregion

namespace Waveface.Component.RichEdit
{
    public class TextChanged2EventArgs : EventArgs
    {
        private string m_after;
        private string m_before;
        private int m_start;

        public string After
        {
            get { return m_after; }
        }

        public string Before
        {
            get { return m_before; }
        }

        public int Start
        {
            get { return m_start; }
        }

        public TextChanged2EventArgs(int start, string before, string after)
        {
            m_start = start;
            m_before = before;
            m_after = after;
        }
    }
}