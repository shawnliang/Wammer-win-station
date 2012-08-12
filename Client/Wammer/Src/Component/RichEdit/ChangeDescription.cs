#region

using System.Drawing;

#endregion

namespace Waveface.Component.RichEdit
{
    internal class ChangeDescription
    {
        private int m_start;
        private CharacterRange m_selectionAfter;
        private CharacterRange m_selectionBefore;
        private string m_textAfter;
        private string m_textBefore;

        public CharacterRange SelectionAfter
        {
            get { return m_selectionAfter; }
        }

        public CharacterRange SelectionBefore
        {
            get { return m_selectionBefore; }
        }

        public int Start
        {
            get { return m_start; }
        }

        public string TextAfter
        {
            get { return m_textAfter; }
        }

        public string TextBefore
        {
            get { return m_textBefore; }
        }

        public ChangeDescription(CharacterRange selectionBefore, CharacterRange selectionAfter)
        {
            m_start = -1;
            m_selectionBefore = selectionBefore;
            m_selectionAfter = selectionAfter;
        }

        public ChangeDescription(int start, CharacterRange selectionBefore, string textBefore,
                                 CharacterRange selectionAfter, string textAfter)
        {
            m_start = start;
            m_selectionBefore = selectionBefore;
            m_textBefore = textBefore;
            m_selectionAfter = selectionAfter;
            m_textAfter = textAfter;
        }
    }
}