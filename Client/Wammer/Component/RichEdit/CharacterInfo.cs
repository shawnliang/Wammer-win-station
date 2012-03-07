namespace Waveface.Component.RichEdit
{
    public struct CharacterInfo
    {
        private bool m_canBreakAfter;
        private bool m_canBreakLine;
        private byte m_characterClass;
        private bool m_isWhitespace;

        public bool CanBreakAfter
        {
            get { return m_canBreakAfter; }
        }

        public bool CanBreakLine
        {
            get { return m_canBreakLine; }
        }

        // Gets a value between 0x00 and 0x0F indicating the class of this character. 
        public byte CharacterClass
        {
            get { return m_characterClass; }
        }

        // Gets a value indicating if the current character is whitespace. 
        public bool IsWhitespace
        {
            get { return m_isWhitespace; }
        }

        public CharacterInfo(byte characterClass, bool canBreakAfter, bool canBreakLine, bool isWhitespace)
        {
            m_characterClass = characterClass;
            m_canBreakAfter = canBreakAfter;
            m_canBreakLine = canBreakLine;
            m_isWhitespace = isWhitespace;
        }
    }
}