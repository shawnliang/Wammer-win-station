#region

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace Waveface.Component.RichEdit
{
    // Adds a few useful methods to and fixes a few problems with System.Windows.Forms.RichTextBox. 
    public class RichTextBox2 : WaterMarkRichTextBox
    {
        private int m_oldEventMask;
        private int m_updating;

        // Gets or sets a CHARFORMAT2 from or to the underlying RichEdit control 
        // describing the format of the currently selected text. 
        protected Interop.CHARFORMAT2 SelectionFormat
        {
            get
            {
                Interop.CHARFORMAT2 _format = new Interop.CHARFORMAT2();
                _format.cbSize = Marshal.SizeOf(_format);
                Interop.SendMessage(Handle, RichEditConstants.EM_GETCHARFORMAT,
                                    new IntPtr(RichEditConstants.SCF_SELECTION), ref _format);

                return _format;
            }
            set
            {
                Interop.SendMessage(Handle, RichEditConstants.EM_SETCHARFORMAT,
                                    new IntPtr(RichEditConstants.SCF_SELECTION), ref value);
            }
        }

        public CharacterRange SelectionRange
        {
            get
            {
                Interop.CHARRANGE _range = new Interop.CHARRANGE();
                Interop.SendMessage(Handle, RichEditConstants.EM_EXGETSEL, 0, ref _range);
                return Interop.CHARRANGE.ToManaged(_range);
            }
            set
            {
                Interop.CHARRANGE _range = Interop.CHARRANGE.FromManaged(value);
                Interop.SendMessage(Handle, RichEditConstants.EM_EXSETSEL, 0, ref _range);
            }
        }

        // Gets or sets the contents of the control
        public override string Text
        {
            // Props to http://weblogs.asp.net/pwelter34/archive/2004/08/06/210174.aspx for this fix
            get
            {
                Interop.GETTEXTLENGTHEX _getLength = new Interop.GETTEXTLENGTHEX();
                _getLength.flags = RichEditConstants.GTL_CLOSE; //get buffer size
                _getLength.codepage = 1200; //Unicode

                int _textLength = Interop.SendMessage(Handle, RichEditConstants.EM_GETTEXTLENGTHEX,
                                                     ref _getLength, 0);

                Interop.GETTEXTEX _getText = new Interop.GETTEXTEX();
                _getText.cb = _textLength + 2; //add space for null terminator
                _getText.flags = RichEditConstants.GT_DEFAULT;
                _getText.codepage = 1200; //Unicode

                StringBuilder _sb = new StringBuilder(_getText.cb);

                Interop.SendMessage(Handle, RichEditConstants.EM_GETTEXTEX, ref _getText, _sb);

                return _sb.ToString();
            }
            set { base.Text = value; }
        }

        // Gets the length of the text in the control
        public override int TextLength
        {
            // Props to http://weblogs.asp.net/pwelter34/archive/2004/08/06/210174.aspx for this fix      
            get
            {
                Interop.GETTEXTLENGTHEX _getLength = new Interop.GETTEXTLENGTHEX();
                _getLength.flags = RichEditConstants.GTL_DEFAULT; //Returns the number of characters
                _getLength.codepage = 1200; //Unicode

                return Interop.SendMessage(Handle, RichEditConstants.EM_GETTEXTLENGTHEX,
                                           ref _getLength, 0);
            }
        }

        // Enables the control to paint itself
        public void AllowRedraw()
        {
            Interop.SendMessage(Handle, WinUserConstants.WM_SETREDRAW, new IntPtr(1), new IntPtr(0));
            Invalidate();
        }

        public void BatchFormat(FormattingInstructionCollection instructions)
        {
            BeginUpdate();

            CharacterRange _oldRange = SelectionRange;

            Format _oldFormat = null;

            if (_oldRange.Length == 0)
            {
                _oldFormat = GetSelectionFormat();
            }

            foreach (FormattingInstruction _instruction in instructions)
            {
                SetFormat(_instruction);
            }

            SelectionRange = _oldRange;

            if (_oldRange.Length == 0)
            {
                SetSelectionFormat(_oldFormat);
            }

            EndUpdate();
        }

        // Suppresses events from firing while the RichTextBox2 is being updated. 
        // Occasional odd behavior related to events not firing has 
        // been observed when using this method. Using PreventRedraw instead is recommended.
        public void BeginUpdate()
        {
            ++m_updating;

            if (m_updating > 1)
            {
                return;
            }

            m_oldEventMask = SetEventMask(0);
            PreventRedraw();
        }

        // Resumes firing events from the RichTextBox2 after they were suspended
        // by a call to BeginUpdate
        // Occasional odd behavior related to events not firing has 
        // been observed when using this method. Using AllowRedraw instead is recommended.
        public void EndUpdate()
        {
            --m_updating;

            if (m_updating > 0)
            {
                return;
            }

            AllowRedraw();
            SetEventMask(m_oldEventMask);
            Invalidate();
        }

        public int FindNextWordBreak(int position)
        {
            return Interop.SendMessage(Handle, RichEditConstants.EM_FINDWORDBREAK, RichEditConstants.WB_NEXTBREAK,
                                       position);
        }

        public int FindPreviousWordBreak(int position)
        {
            return Interop.SendMessage(Handle, RichEditConstants.EM_FINDWORDBREAK, RichEditConstants.WB_PREVBREAK,
                                       position);
        }

        public int FindWordEnd(int position)
        {
            int last = TextLength;

            while (position < last && GetCharacterInfo(position).CharacterClass == 0)
            {
                ++position;
            }

            return position;
        }

        public int FindWordStart(int position)
        {
            while (position > 0 && GetCharacterInfo(position - 1).CharacterClass == 0)
            {
                --position;
            }

            return position;
        }

        public CharacterInfo GetCharacterInfo(int position)
        {
            int _result = Interop.SendMessage(Handle, RichEditConstants.EM_FINDWORDBREAK, RichEditConstants.WB_CLASSIFY,
                                             position);

            byte _characterClass = (byte) ((byte) _result & RichEditConstants.WBF_CLASS);
            bool _canBreakAfter = (_result & RichEditConstants.WBF_BREAKAFTER) > 0;
            bool _canBreakLine = (_result & RichEditConstants.WBF_BREAKLINE) > 0;
            bool _isWhitespace = (_result & RichEditConstants.WBF_ISWHITE) > 0;

            return new CharacterInfo(_characterClass, _canBreakAfter, _canBreakLine, _isWhitespace);
        }

        public string GetTextRange(CharacterRange range)
        {
            Interop.TEXTRANGE _textRange = new Interop.TEXTRANGE();
            _textRange.chrg = Interop.CHARRANGE.FromManaged(range);
            _textRange.lpstrText = Marshal.StringToHGlobalAuto(new string(' ', range.Length));
            int _result = Interop.SendMessage(Handle, RichEditConstants.EM_GETTEXTRANGE, 0, _textRange);
            string _text = Marshal.PtrToStringAuto(_textRange.lpstrText);
            Marshal.FreeHGlobal(_textRange.lpstrText);
            return _text;
        }

        // Prevents the control from painting itself until AllowRedraw is called
        public void PreventRedraw()
        {
            Interop.SendMessage(Handle, WinUserConstants.WM_SETREDRAW, new IntPtr(0), new IntPtr(0));
        }

        // Sets the control's event mask, which prevents certain events from firing. 
        protected int SetEventMask(int mask)
        {
            return Interop.SendMessage(Handle, RichEditConstants.EM_SETEVENTMASK, new IntPtr(0), new IntPtr(mask));
        }

        private Format GetSelectionFormat()
        {
            Format _format = new Format();

            Interop.CHARFORMAT2 _charFormat = new Interop.CHARFORMAT2();
            _charFormat.cbSize = Marshal.SizeOf(_charFormat);

            Interop.SendMessage(Handle, RichEditConstants.EM_GETCHARFORMAT, RichEditConstants.SCF_SELECTION,
                                ref _charFormat);

            if ((_charFormat.dwMask & RichEditConstants.CFM_BACKCOLOR) != 0)
            {
                _format.BackColor = ColorTranslator.FromWin32(_charFormat.crBackColor);
            }

            if ((_charFormat.dwMask & RichEditConstants.CFM_COLOR) != 0)
            {
                _format.ForeColor = ColorTranslator.FromWin32(_charFormat.crTextColor);
            }

            SetFontFormatProperties(_charFormat, _format);

            if ((_charFormat.dwMask & RichEditConstants.CFM_BOLD) != 0)
            {
                _format.Bold = (_charFormat.dwEffects & RichEditConstants.CFE_BOLD) != 0;
            }

            if ((_charFormat.dwMask & RichEditConstants.CFM_ITALIC) != 0)
            {
                _format.Italic = (_charFormat.dwEffects & RichEditConstants.CFE_ITALIC) != 0;
            }

            if ((_charFormat.dwMask & RichEditConstants.CFM_STRIKEOUT) != 0)
            {
                _format.Strikethrough = (_charFormat.dwEffects & RichEditConstants.CFE_STRIKEOUT) != 0;
            }

            if ((_charFormat.dwMask & RichEditConstants.CFM_PROTECTED) != 0)
            {
                _format.Protected = (_charFormat.dwEffects & RichEditConstants.CFE_PROTECTED) != 0;
            }

            if ((_charFormat.dwMask & RichEditConstants.CFM_HIDDEN) != 0)
            {
                _format.Protected = (_charFormat.dwEffects & RichEditConstants.CFE_HIDDEN) != 0;
            }

            if ((_charFormat.dwMask & RichEditConstants.CFM_UNDERLINETYPE) != 0)
            {
                _format.UnderlineFormat = UnderlineTypeToUnderlineFormat(_charFormat.bUnderlineType);
            }

            return _format;
        }

        private uint SetBitState(uint value, uint mask, bool state)
        {
            return (uint) SetBitState((int) value, (int) mask, state);
        }

        private int SetBitState(int value, int mask, bool state)
        {
            if (state)
            {
                return value | mask;
            }
            else
            {
                return value & ~mask;
            }
        }

        private void SetFontFormatProperties(Format format, ref Interop.CHARFORMAT2 charFormat)
        {
            Font _font = format.Font;
            Interop.LOGFONT logfont = new Interop.LOGFONT();
            _font.ToLogFont(logfont);

            uint _mask = RichEditConstants.CFM_SIZE | RichEditConstants.CFM_FACE |
                        RichEditConstants.CFM_BOLD | RichEditConstants.CFM_ITALIC | RichEditConstants.CFM_UNDERLINE |
                        RichEditConstants.CFM_STRIKEOUT;

            int _effects = 0;

            if (_font.Bold)
            {
                _effects |= RichEditConstants.CFE_BOLD;
            }

            if (_font.Italic)
            {
                _effects |= RichEditConstants.CFE_ITALIC;
            }

            if (_font.Strikeout)
            {
                _effects |= RichEditConstants.CFE_STRIKEOUT;
            }

            if (_font.Underline)
            {
                _effects |= RichEditConstants.CFE_UNDERLINE;
            }

            charFormat.dwMask = _mask;
            charFormat.dwEffects = (uint) _effects;
            charFormat.lfFaceName = logfont.lfFaceName;
            charFormat.yHeight = ((int) (_font.SizeInPoints*20f));
            charFormat.bCharSet = logfont.lfCharSet;
            charFormat.bPitchAndFamily = logfont.lfPitchAndFamily;
        }

        private void SetFontFormatProperties(Interop.CHARFORMAT2 charFormat, Format format)
        {
            if ((charFormat.dwMask & RichEditConstants.CFM_FACE) == 0)
            {
                return;
            }

            string _familyName = charFormat.lfFaceName;
            float _emSize = 13f;
            
            if ((charFormat.dwMask & RichEditConstants.CFM_CHARSET) != 0)
            {
                _emSize = ((charFormat.yHeight)/20f);

                if ((_emSize == 0f) && (charFormat.yHeight > 0))
                {
                    _emSize = 1f;
                }
            }

            FontStyle _style = FontStyle.Regular;
            
            if (((charFormat.dwMask & RichEditConstants.CFM_BOLD) != 0) &&
                ((charFormat.dwEffects & RichEditConstants.CFE_BOLD) != 0))
            {
                _style |= FontStyle.Bold;
            }

            if (((charFormat.dwMask & RichEditConstants.CFM_ITALIC) != 0) &&
                ((charFormat.dwEffects & RichEditConstants.CFE_ITALIC) != 0))
            {
                _style |= FontStyle.Italic;
            }

            if (((charFormat.dwMask & RichEditConstants.CFM_STRIKEOUT) != 0) &&
                ((charFormat.dwEffects & RichEditConstants.CFE_STRIKEOUT) != 0))
            {
                _style |= FontStyle.Strikeout;
            }

            if (((charFormat.dwMask & RichEditConstants.CFM_UNDERLINE) != 0) &&
                ((charFormat.dwEffects & RichEditConstants.CFE_UNDERLINE) != 0))
            {
                _style |= FontStyle.Underline;
            }

            try
            {
                format.Font = new Font(_familyName, _emSize, _style, GraphicsUnit.Point, charFormat.bCharSet);
            }
            catch
            {
            }
        }

        private void SetFormat(FormattingInstruction instruction)
        {
            SelectionStart = instruction.Start;
            SelectionLength = instruction.Length;
            SetSelectionFormat(instruction.Format);
        }

        private void SetSelectionFormat(Format format)
        {
            Interop.CHARFORMAT2 _charFormat = new Interop.CHARFORMAT2();
            _charFormat.cbSize = Marshal.SizeOf(_charFormat);

            if (format.BackColorValid)
            {
                _charFormat.dwMask |= RichEditConstants.CFM_BACKCOLOR;
                _charFormat.crBackColor = ColorTranslator.ToWin32(format.BackColor);
            }

            if (format.ForeColorValid)
            {
                _charFormat.dwMask |= RichEditConstants.CFM_COLOR;
                _charFormat.crTextColor = ColorTranslator.ToWin32(format.ForeColor);
            }

            if (format.FontValid)
            {
                SetFontFormatProperties(format, ref _charFormat);
            }

            if (format.BoldValid)
            {
                _charFormat.dwMask |= RichEditConstants.CFM_BOLD;
                _charFormat.dwEffects = SetBitState(_charFormat.dwEffects, RichEditConstants.CFE_BOLD, format.Bold);
            }

            if (format.ItalicValid)
            {
                _charFormat.dwMask |= RichEditConstants.CFM_ITALIC;
                _charFormat.dwEffects = SetBitState(_charFormat.dwEffects, RichEditConstants.CFE_ITALIC, format.Italic);
            }

            if (format.StrikethroughValid)
            {
                _charFormat.dwMask |= RichEditConstants.CFM_STRIKEOUT;
                _charFormat.dwEffects = SetBitState(_charFormat.dwEffects, RichEditConstants.CFE_STRIKEOUT,
                                                   format.Strikethrough);
            }

            if (format.ProtectedValid)
            {
                _charFormat.dwMask |= RichEditConstants.CFM_PROTECTED;
                _charFormat.dwEffects |= SetBitState(_charFormat.dwEffects, RichEditConstants.CFE_PROTECTED,
                                                    format.Protected);
            }

            if (format.HiddenValid)
            {
                _charFormat.dwMask |= RichEditConstants.CFM_HIDDEN;
                _charFormat.dwEffects |= SetBitState(_charFormat.dwEffects, RichEditConstants.CFE_HIDDEN, format.Hidden);
            }

            if (format.SubSuperValid)
            {
                _charFormat.dwMask |= RichEditConstants.CFM_SUBSCRIPT | RichEditConstants.CFM_SUPERSCRIPT;
                _charFormat.dwEffects |= SetBitState(_charFormat.dwEffects, RichEditConstants.CFE_SUBSCRIPT,
                                                    format.Subscript);
                _charFormat.dwEffects |= SetBitState(_charFormat.dwEffects, RichEditConstants.CFE_SUPERSCRIPT,
                                                    format.Superscript);
            }

            if (format.UnderlineFormatValid)
            {
                _charFormat.dwMask |= RichEditConstants.CFM_UNDERLINETYPE;
                _charFormat.bUnderlineType = UnderlineFormatToUnderlineType(format.UnderlineFormat);
            }

            if (format.LinkValid)
            {
                _charFormat.dwMask |= RichEditConstants.CFM_LINK;
                _charFormat.dwEffects |= SetBitState(_charFormat.dwEffects, RichEditConstants.CFE_LINK, format.Link);
            }

            if (format.DisabledValid)
            {
                _charFormat.dwMask |= RichEditConstants.CFM_DISABLED;
                _charFormat.dwEffects |= SetBitState(_charFormat.dwEffects, RichEditConstants.CFE_DISABLED,
                                                    format.Disabled);
            }

            Interop.SendMessage(Handle, RichEditConstants.EM_SETCHARFORMAT, RichEditConstants.SCF_SELECTION,
                                ref _charFormat);
        }

        private byte UnderlineFormatToUnderlineType(UnderlineFormat value)
        {
            byte _type = 0x00;

            if (value.Style != UnderlineStyle.None)
            {
                _type = (byte) ((byte) value.Style | (byte) value.Color);
            }

            return _type;
        }

        private UnderlineFormat UnderlineTypeToUnderlineFormat(byte type)
        {
            return new UnderlineFormat((UnderlineStyle) (type & 0x0F),
                                       (UnderlineColor) (type & 0xF0));
        }
    }
}