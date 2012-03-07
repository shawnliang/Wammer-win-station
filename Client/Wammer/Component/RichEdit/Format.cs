#region

using System.Drawing;

#endregion

namespace Waveface.Component.RichEdit
{
    public class Format
    {
        private Color backColor;
        private bool backColorValid;
        private bool bold;
        private bool boldValid;
        private bool disabled;
        private bool disabledValid;
        private Font font;
        private bool fontValid;
        private Color foreColor;
        private bool foreColorValid;
        private bool hidden;
        private bool hiddenValid;
        private bool italic;
        private bool italicValid;
        private bool link;
        private bool linkValid;
        private bool protect;
        private bool protectedValid;
        private bool subscript;
        private bool subSuperValid;
        private bool superscript;
        private bool strikethrough;
        private bool strikethroughValid;
        private UnderlineFormat underlineFormat;
        private bool underlineFormatValid;

        #region Properties

        /// <summary>
        /// Gets or sets the background color of the text
        /// </summary>
        public Color BackColor
        {
            get { return backColor; }
            set
            {
                backColorValid = true;
                backColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the value in the <see cref="BackColor"/> property 
        /// should be used. Automatically set to true when <see cref="BackColor"/> is set. 
        /// </summary>
        public bool BackColorValid
        {
            get { return backColorValid; }
            set { backColorValid = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the text should be rendered as bold. 
        /// </summary>
        public bool Bold
        {
            get { return bold; }
            set
            {
                boldValid = true;
                bold = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the value in the <see cref="Bold"/> property 
        /// should be used. Automatically set to true when <see cref="Bold"/> is set. 
        /// </summary>
        public bool BoldValid
        {
            get { return boldValid; }
            set { boldValid = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the text should be rendered as disabled.
        /// </summary>
        public bool Disabled
        {
            get { return disabled; }
            set
            {
                disabledValid = true;
                disabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the value in the <see cref="Disabled"/> property 
        /// should be used. Automatically set to true when <see cref="Disabled"/> is set. 
        /// </summary>
        public bool DisabledValid
        {
            get { return disabledValid; }
            set { disabledValid = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates what font the text should be rendered in. 
        /// </summary>
        public Font Font
        {
            get { return font; }
            set
            {
                fontValid = true;
                font = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the value in the <see cref="Font"/> property 
        /// should be used. Automatically set to true when <see cref="Font"/> is set. 
        /// </summary>
        public bool FontValid
        {
            get { return fontValid; }
            set { fontValid = value; }
        }

        /// <summary>
        /// Gets or sets the foreground color of the text
        /// </summary>
        public Color ForeColor
        {
            get { return foreColor; }
            set
            {
                foreColorValid = true;
                foreColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the value in the <see cref="ForeColor"/> property 
        /// should be used. Automatically set to true when <see cref="ForeColor"/> is set. 
        /// </summary>
        public bool ForeColorValid
        {
            get { return foreColorValid; }
            set { foreColorValid = true; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the text will be hidden (not rendered at all). 
        /// </summary>
        public bool Hidden
        {
            get { return hidden; }
            set
            {
                hiddenValid = true;
                hidden = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the value in the <see cref="Hidden"/> property 
        /// should be used. Automatically set to true when <see cref="Hidden"/> is set. 
        /// </summary>
        public bool HiddenValid
        {
            get { return hiddenValid; }
            set { hiddenValid = true; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the text will be drawn as italic. 
        /// </summary>
        public bool Italic
        {
            get { return italic; }
            set
            {
                italicValid = true;
                italic = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the value in the <see cref="Italic"/> property 
        /// should be used. Automatically set to true when <see cref="Italic"/> is set. 
        /// </summary>
        public bool ItalicValid
        {
            get { return italicValid; }
            set { italicValid = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the text should be clickable as a link.
        /// </summary>
        public bool Link
        {
            get { return link; }
            set
            {
                linkValid = true;
                link = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the value in the <see cref="Link"/> property 
        /// should be used. Automatically set to true when <see cref="Link"/> is set. 
        /// </summary>
        public bool LinkValid
        {
            get { return linkValid; }
            set { linkValid = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the text will be protected. Protected text cannot
        /// be altered. 
        /// </summary>
        public bool Protected
        {
            get { return protect; }
            set
            {
                protectedValid = true;
                protect = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the value in the <see cref="Protected"/> property 
        /// should be used. Automatically set to true when <see cref="Protected"/> is set. 
        /// </summary>
        public bool ProtectedValid
        {
            get { return protectedValid; }
            set { protectedValid = true; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the text will rendered as subscript text. Setting
        /// this value to true automatically sets <see cref="Superscript"/> to false.
        /// </summary>
        public bool Subscript
        {
            get { return subscript; }
            set
            {
                subSuperValid = true;
                subscript = value;
                if (superscript)
                {
                    subscript = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the value in the <see cref="Subscript"/> and 
        /// <see cref="Superscript"/> properties should be used. Automatically set to true when 
        /// <see cref="Subscript"/> or <see cref="Superscript"/> is set. 
        /// </summary>
        public bool SubSuperValid
        {
            get { return subSuperValid; }
            set { subSuperValid = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the text will rendered as superscript text. Setting
        /// this value to true automatically sets <see cref="Subscript"/> to false.
        /// </summary>
        public bool Superscript
        {
            get { return superscript; }
            set
            {
                subSuperValid = true;
                superscript = value;
                if (superscript)
                {
                    subscript = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the text will be rendered with a line drawn
        /// through it. 
        /// </summary>
        public bool Strikethrough
        {
            get { return strikethrough; }
            set
            {
                strikethroughValid = true;
                strikethrough = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the value in the <see cref="Strikethrough"/> property 
        /// should be used. Automatically set to true when <see cref="Strikethrough"/> is set. 
        /// </summary>
        public bool StrikethroughValid
        {
            get { return strikethroughValid; }
            set { strikethroughValid = true; }
        }

        /// <summary>
        /// Gets or sets the underline format for the text. Set to <c>UnderlineFormat.None</c> to 
        /// turn off underlining. Not all underlining formats are supported on all machines. 
        /// </summary>
        public UnderlineFormat UnderlineFormat
        {
            get { return underlineFormat; }
            set
            {
                underlineFormatValid = true;
                underlineFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the value in the <see cref="UnderlineFormat"/> property 
        /// should be used. Automatically set to true when <see cref="UnderlineFormat"/> is set. 
        /// </summary>
        public bool UnderlineFormatValid
        {
            get { return underlineFormatValid; }
            set { underlineFormatValid = value; }
        }

        #endregion
    }
}