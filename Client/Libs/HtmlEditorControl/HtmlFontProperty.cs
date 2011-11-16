#region

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

#endregion

namespace Waveface.Component.HtmlEditor
{

    #region HtmlFontSize enumeration

    // Enum used to modify the font size
    public enum HtmlFontSize
    {
        Default = 0,
        xxSmall = 1, // 8 points
        xSmall = 2, // 10 points
        Small = 3, // 12 points
        Medium = 4, // 14 points
        Large = 5, // 18 points
        xLarge = 6, // 24 points
        xxLarge = 7 // 36 points
    }

    //HtmlFontSize

    #endregion

    #region HtmlFontProperty struct

    // Struct used to define a Html Font
    // Supports Name Size Bold Italic Subscript Superscript Strikeout
    // Specialized TypeConvertor used for Designer Support
    // If Name is Empty or Null Struct is consider Null
    [Serializable]
    [TypeConverter(typeof (HtmlFontPropertyConverter))]
    public struct HtmlFontProperty
    {
        // properties defined for the Font
        private string m_name;
        private HtmlFontSize m_size;
        private bool m_bold;
        private bool m_italic;
        private bool m_underline;
        private bool m_strikeout;
        private bool m_subscript;
        private bool m_superscript;

        #region Properties

        // public property Name
        [Description("The Name of the Font")]
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        // public property Size
        [Description("The Size of the Font")]
        public HtmlFontSize Size
        {
            get { return m_size; }
            set { m_size = value; }
        }

        // public property Bold
        [Description("Indicates if the font is Bold")]
        public bool Bold
        {
            get { return m_bold; }
            set { m_bold = value; }
        }

        // public property Italic
        [Description("Indicates if the font is Italic")]
        public bool Italic
        {
            get { return m_italic; }
            set { m_italic = value; }
        }

        // public property Underline
        [Description("Indicates if the font is Underline")]
        public bool Underline
        {
            get { return m_underline; }
            set { m_underline = value; }
        }

        // public property Strikeout
        [Description("Indicates if the font is Strikeout")]
        public bool Strikeout
        {
            get { return m_strikeout; }
            set { m_strikeout = value; }
        }

        // public property Subscript
        [Description("Indicates if the font is Subscript")]
        public bool Subscript
        {
            get { return m_subscript; }
            set { m_subscript = value; }
        }

        // public property Superscript
        [Description("Indicates if the font is Superscript")]
        public bool Superscript
        {
            get { return m_superscript; }
            set { m_superscript = value; }
        }

        #endregion

        public HtmlFontProperty(string name)
        {
            m_name = name;
            m_size = HtmlFontSize.Default;
            m_bold = false;
            m_italic = false;
            m_underline = false;
            m_strikeout = false;
            m_subscript = false;
            m_superscript = false;
        }

        public HtmlFontProperty(string name, HtmlFontSize size)
        {
            m_name = name;
            m_size = size;
            m_bold = false;
            m_italic = false;
            m_underline = false;
            m_strikeout = false;
            m_subscript = false;
            m_superscript = false;
        }

        //HtmlFontProperty

        // constrctor for all standard attributes
        public HtmlFontProperty(string name, HtmlFontSize size, bool bold, bool italic, bool underline)
        {
            m_name = name;
            m_size = size;
            m_bold = bold;
            m_italic = italic;
            m_underline = underline;
            m_strikeout = false;
            m_subscript = false;
            m_superscript = false;
        }

        // constrctor for all attributes
        public HtmlFontProperty(string name, HtmlFontSize size, bool bold, bool italic, bool underline, bool strikeout,
                                bool subscript, bool superscript)
        {
            m_name = name;
            m_size = size;
            m_bold = bold;
            m_italic = italic;
            m_underline = underline;
            m_strikeout = strikeout;
            m_subscript = subscript;
            m_superscript = superscript;
        }

        // constructor given a system Font
        public HtmlFontProperty(Font font)
        {
            m_name = font.Name;
            m_size = HtmlFontConversion.FontSizeToHtml(font.SizeInPoints);
            m_bold = font.Bold;
            m_italic = font.Italic;
            m_underline = font.Underline;
            m_strikeout = font.Strikeout;
            m_subscript = false;
            m_superscript = false;
        }

        // public method to convert the html into a readable format
        // used by the designer to display the font name
        public override string ToString()
        {
            return string.Format("{0}, {1}", Name, Size);
        }

        // compares two Html Fonts for equality
        // equality opertors not defined (Design Time issue with override of Equals)
        public static bool IsEqual(HtmlFontProperty font1, HtmlFontProperty font2)
        {
            // assume not equal
            bool _equals = false;

            // perform the comparsion
            if (IsNotNull(font1) && IsNotNull(font2))
            {
                if (font1.Name == font2.Name &&
                    font1.Size == font2.Size &&
                    font1.Bold == font2.Bold &&
                    font1.Italic == font2.Italic &&
                    font1.Underline == font2.Underline &&
                    font1.Strikeout == font2.Strikeout &&
                    font1.Subscript == font2.Subscript &&
                    font1.Superscript == font2.Superscript)
                {
                    _equals = true;
                }
            }

            // return the calculated value
            return _equals;
        }

        // compares two Html Fonts for equality
        // equality opertors not defined (Design Time issue with override of Equals)
        public static bool IsNotEqual(HtmlFontProperty font1, HtmlFontProperty font2)
        {
            return (!IsEqual(font1, font2));
        }

        // based on a font name being null the font can be assumed to be null
        // default constructor will give a null object
        public static bool IsNull(HtmlFontProperty font)
        {
            return (font.Name == null || font.Name.Trim() == string.Empty);
        }

        // based on a font name being null the font can be assumed to be null
        // default constructor will give a null object
        public static bool IsNotNull(HtmlFontProperty font)
        {
            return (!IsNull(font));
        }
    }

    #endregion

    #region HtmlFontConversion utilities

    // Utility Class to perform Font Attribute conversions
    // Takes data to and from the expected Html Format
    public class HtmlFontConversion
    {
        // return the correct string size description from a HtmlFontSize
        public static string HtmlFontSizeString(HtmlFontSize fontSize)
        {
            // set the size to blank as the default
            // this will ensure font size blanked out if not set
            string _size = string.Empty;

            switch (fontSize)
            {
                case HtmlFontSize.xxSmall:
                    _size = "xx-small";
                    break;
                case HtmlFontSize.xSmall:
                    _size = "x-small";
                    break;
                case HtmlFontSize.Small:
                    _size = "small";
                    break;
                case HtmlFontSize.Medium:
                    _size = "medium";
                    break;
                case HtmlFontSize.Large:
                    _size = "large";
                    break;
                case HtmlFontSize.xLarge:
                    _size = "x-large";
                    break;
                case HtmlFontSize.xxLarge:
                    _size = "xx-large";
                    break;
                case HtmlFontSize.Default:
                    _size = string.Empty; //small
                    break;
            }

            // return the calculated size
            return _size;
        }

        // return the correct bold description for the bold attribute
        public static string HtmlFontBoldString(bool fontBold)
        {
            return (fontBold ? "Bold" : "Normal");
        }

        // return the correct bold description for the bold attribute
        public static string HtmlFontItalicString(bool fontItalic)
        {
            return (fontItalic ? "Italic" : "Normal");
        }

        // determine the font size given a selected font in points
        public static HtmlFontSize FontSizeToHtml(float fontSize)
        {
            // make the following mapping
            // 1:8pt
            // 2:10pt
            // 3:12pt
            // 4:14pt
            // 5:18pt
            // 6:24pt
            // 7:36pt
            int _calcFont = 0;

            if (fontSize < 10) _calcFont = 1; // 8pt
            else if (fontSize < 12) _calcFont = 2; // 10pt
            else if (fontSize < 14) _calcFont = 3; // 12pt
            else if (fontSize < 18) _calcFont = 4; // 14pt
            else if (fontSize < 24) _calcFont = 5; // 24pt
            else if (fontSize < 36) _calcFont = 6; // 36pt
            else _calcFont = 7;

            return (HtmlFontSize) _calcFont;
        }

        // determine the font size given the html font size
        public static float FontSizeFromHtml(HtmlFontSize fontSize)
        {
            return FontSizeFromHtml((int) fontSize);
        }

        // determine the font size given the html int size
        public static float FontSizeFromHtml(int fontSize)
        {
            // make the following mapping
            // 1:8pt
            // 2:10pt
            // 3:12pt
            // 4:14pt
            // 5:18pt
            // 6:24pt
            // 7:36pt
            float _calcFont = 0;

            switch (fontSize)
            {
                case 1:
                    _calcFont = 8F;
                    break;
                case 2:
                    _calcFont = 10F;
                    break;
                case 3:
                    _calcFont = 12F;
                    break;
                case 4:
                    _calcFont = 14F;
                    break;
                case 5:
                    _calcFont = 18F;
                    break;
                case 6:
                    _calcFont = 24F;
                    break;
                case 7:
                    _calcFont = 36F;
                    break;
                default:
                    _calcFont = 12F;
                    break;
            }

            return _calcFont;
        }

        // Used to determine the HtmlFontSize from a style attribute
        public static HtmlFontSize StyleSizeToHtml(string sizeDesc)
        {
            // currently assume the value is a fixed point
            // should take into account relative and absolute values
            float _size;

            try
            {
                _size = Single.Parse(Regex.Replace(sizeDesc, @"[^\d|\.]", ""));
            }
            catch (Exception)
            {
                // set size to zero to return HtmlFontSize.Default
                _size = 0;
            }

            // return value as a HtmlFontSize
            return FontSizeToHtml(_size);
        }

        // Used to determine the the style attribute is for Bold
        public static bool IsStyleBold(string style)
        {
            return Regex.IsMatch(style, "bold|bolder|700|800|900", RegexOptions.IgnoreCase);
        }

        // Used to determine the the style attribute is for Italic
        public static bool IsStyleItalic(string style)
        {
            return Regex.IsMatch(style, "style|oblique", RegexOptions.IgnoreCase);
        }
    }

    #endregion

    #region HtmlFontPropertyConverter class

    // Expandable object converter for the HtmlFontProperty
    // Allows it to be viewable from the property browser
    // String format based on "Name, FontSize"
    public class HtmlFontPropertyConverter : ExpandableObjectConverter
    {
        // constants used for the property names
        private const string PROP_NAME = "Name";
        private const string PROP_SIZE = "Size";
        private const string PROP_BOLD = "Bold";
        private const string PROP_ITALIC = "Italic";
        private const string PROP_UNDERLINE = "Underline";
        private const string PROP_STRIKEOUT = "Strikeout";
        private const string PROP_SUBSCRIPT = "Subscript";
        private const string PROP_SUPERSCRIPT = "Superscript";

        // regular expression parse 
        private const string FONT_PARSE_EXPRESSION = @"^(?<name>(\w| )+)((\s*,\s*)?)(?<size>\w*)";
        private const string FONT_PARSE_NAME = @"${name}";
        private const string FONT_PARSE_SIZE = @"${size}";

        // Allows expansion sub property change to have string updated
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            // always return a new instance
            return true;
        }

        // creates a new HtmlFontProperty from a series of values
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary values)
        {
            // obtain the HtmlFontProperty properties
            string _name = (string) values[PROP_NAME];
            HtmlFontSize _size = (HtmlFontSize) values[PROP_SIZE];
            bool _bold = (bool) values[PROP_BOLD];
            bool _italic = (bool) values[PROP_ITALIC];
            bool _underline = (bool) values[PROP_UNDERLINE];
            bool _strikeout = (bool) values[PROP_STRIKEOUT];
            bool _subscript = (bool) values[PROP_SUBSCRIPT];
            bool _superscript = (bool) values[PROP_SUPERSCRIPT];

            // return the new HtmlFontProperty
            return new HtmlFontProperty(_name, _size, _bold, _italic, _underline, _strikeout, _subscript, _superscript);
        }

        // Indicates if a conversion can take place from a HtmlFontProperty
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof (string) || destinationType == typeof (InstanceDescriptor))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        // Performs the conversion from HtmlFontProperty to a string (only)
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            // ensure working with the intented type HtmlFontProperty
            if (value is HtmlFontProperty)
            {
                HtmlFontProperty _font = (HtmlFontProperty) value;

                if (destinationType == typeof (string))
                {
                    return _font.ToString();
                }

                if (destinationType == typeof (InstanceDescriptor))
                {
                    // define array to hold the properties and values
                    Object[] _properties = new Object[8];
                    Type[] _types = new Type[8];

                    // Name property
                    _properties[0] = _font.Name;
                    _types[0] = typeof (string);

                    // Size property
                    _properties[1] = _font.Size;
                    _types[1] = typeof (HtmlFontSize);

                    // Bold property
                    _properties[2] = _font.Bold;
                    _types[2] = typeof (bool);

                    // Italic property
                    _properties[3] = _font.Italic;
                    _types[3] = typeof (bool);

                    // Underline property
                    _properties[4] = _font.Underline;
                    _types[4] = typeof (bool);

                    // Strikeout property
                    _properties[5] = _font.Strikeout;
                    _types[5] = typeof (bool);

                    // Subscript property
                    _properties[6] = _font.Subscript;
                    _types[6] = typeof (bool);

                    // Superscript property
                    _properties[7] = _font.Superscript;
                    _types[7] = typeof (bool);

                    // create the instance constructor to return
                    ConstructorInfo ci = typeof (HtmlFontProperty).GetConstructor(_types);
                    return new InstanceDescriptor(ci, _properties);
                }
            }

            // have something other than InstanceDescriptor or sting
            return base.ConvertTo(context, culture, value, destinationType);
        }

        // Indicates if a conversion can take place from s string
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof (string))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }

        // Performs the conversion from string to a HtmlFontProperty (only)
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                // define a new font property
                string _fontString = (string) value;
                HtmlFontProperty _font = new HtmlFontProperty(string.Empty);
                
                try
                {
                    // parse the contents of the given string using a regex
                    Regex _expression = new Regex(FONT_PARSE_EXPRESSION,
                                                 RegexOptions.IgnoreCase | RegexOptions.Compiled |
                                                 RegexOptions.ExplicitCapture);
                    Match _match = _expression.Match(_fontString);
                    
                    // see if a match was found
                    if (_match.Success)
                    {
                        // extract the content type elements
                        string _fontName = _match.Result(FONT_PARSE_NAME);
                        string _fontSize = _match.Result(FONT_PARSE_SIZE);

                        // set the fontname
                        TextInfo _text = Thread.CurrentThread.CurrentCulture.TextInfo;
                        _font.Name = _text.ToTitleCase(_fontName);

                        // determine size from given string using Small if blank
                        if (_fontSize == string.Empty) 
                            _fontSize = "Small";

                        _font.Size = (HtmlFontSize) Enum.Parse(typeof (HtmlFontSize), _fontSize, true);
                    }
                }
                catch (Exception)
                {
                    // do nothing but ensure font is a null font
                    _font.Name = string.Empty;
                }
                if (HtmlFontProperty.IsNull(_font))
                {
                    // error performing the string conversion so throw exception given possible format
                    string _error =
                        string.Format(
                            @"Cannot convert '{0}' to Type HtmlFontProperty. Format: 'FontName, HtmlSize', Font Size values: {1}",
                            _fontString, string.Join(", ", Enum.GetNames(typeof (HtmlFontSize))));
                    
                    throw new ArgumentException(_error);
                }
                else
                {
                    // return the font
                    return _font;
                }
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }

    #endregion
}