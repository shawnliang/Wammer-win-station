#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using AxSHDocVw;
using Waveface.Component.COM;
using Waveface.Component.COM.IOleCommandTarget;
using mshtml;
using HtmlDocument = mshtml.HTMLDocument;
using HtmlBody = mshtml.HTMLBody;
using HtmlStyleSheet = mshtml.IHTMLStyleSheet;
using HtmlStyle = mshtml.IHTMLStyle;
using HtmlDomNode = mshtml.IHTMLDOMNode;
using HtmlDomTextNode = mshtml.IHTMLDOMTextNode;
using HtmlTextRange = mshtml.IHTMLTxtRange;
using HtmlSelection = mshtml.IHTMLSelectionObject;
using HtmlControlRange = mshtml.IHTMLControlRange;
using HtmlElement = mshtml.IHTMLElement;
using HtmlElementCollection = mshtml.IHTMLElementCollection;
using HtmlControlElement = mshtml.IHTMLControlElement;
using HtmlAnchorElement = mshtml.IHTMLAnchorElement;
using HtmlImageElement = mshtml.IHTMLImgElement;
using HtmlFontElement = mshtml.IHTMLFontElement;
using HtmlLineElement = mshtml.IHTMLHRElement;
using HtmlSpanElement = mshtml.IHTMLSpanFlow;
using HtmlScriptElement = mshtml.IHTMLScriptElement;
using HtmlTable = mshtml.IHTMLTable;
using HtmlTableCaption = mshtml.IHTMLTableCaption;
using HtmlTableRow = mshtml.IHTMLTableRow;
using HtmlTableCell = mshtml.IHTMLTableCell;
using HtmlTableRowMetrics = mshtml.IHTMLTableRowMetrics;
using HtmlTableColumn = mshtml.IHTMLTableCol;
using HtmlEventObject = mshtml.IHTMLEventObj;

#endregion

namespace Waveface.Component.HtmlEditor
{
    // This is the main UserControl class that defines the Html Editor
    // BodyHtml sets the complet Body including the body tag
    // InnerText and InnerHtml sets the contents of the Body
    // Property ReadOnly defines whether the content is editable
    [DefaultProperty("InnerText")]
    public sealed class HtmlEditorControl : UserControl
    {
        #region Public Events

        // public event that is raised if an internal processing exception is found
        [Category("Exception"), Description("An Internal Processing Exception was encountered")]
        public event HtmlExceptionEventHandler HtmlException;

        // public event that is raised if navigation event is captured
        [Category("Navigation"), Description("A Navigation Event was encountered")]
        public event HtmlNavigationEventHandler HtmlNavigation;

        #endregion

        #region Constant Defintions

        // general constants
        private const int HTML_BUFFER_SIZE = 256;

        // define the tags being used by the application
        private const string BODY_TAG = "BODY";
        private const string SCRIPT_TAG = "SCRIPT";
        private const string ANCHOR_TAG = "A";
        private const string FONT_TAG = "FONT";
        private const string BOLD_TAG = "STRONG";
        private const string UNDERLINE_TAG = "U";
        private const string ITALIC_TAG = "EM";
        private const string STRIKE_TAG = "STRIKE";
        private const string SUBSCRIPT_TAG = "SUB";
        private const string SUPERSCRIPT_TAG = "SUP";
        private const string HEAD_TAG = "HEAD";
        private const string IMAGE_TAG = "IMG";
        private const string TABLE_TAG = "TABLE";
        private const string TABLE_ROW_TAG = "TR";
        private const string TABLE_CELL_TAG = "TD";
        private const string TABLE_HEAD_TAG = "TH";
        private const string SPAN_TAG = "SPAN";
        private const string OPEN_TAG = "<";
        private const string CLOSE_TAG = ">";
        private const string SELECT_TYPE_TEXT = "text";
        private const string SELECT_TYPE_CONTROL = "control";
        private const string SELECT_TYPE_NONE = "none";
        private const string FORMATTED_PRE = "Formatted";
        private const string FORMATTED_NORMAL = "Normal";
        private const string FORMATTED_HEADING = "Heading";
        private const string EVENT_CONTEXT_MENU = "contextmenu";

        // define commands for mshtml execution execution
        private const string HTML_COMMAND_OVERWRITE = "OverWrite";
        private const string HTML_COMMAND_BOLD = "Bold";
        private const string HTML_COMMAND_UNDERLINE = "Underline";
        private const string HTML_COMMAND_ITALIC = "Italic";
        private const string HTML_COMMAND_SUBSCRIPT = "Subscript";
        private const string HTML_COMMAND_SUPERSCRIPT = "Superscript";
        private const string HTML_COMMAND_STRIKE_THROUGH = "StrikeThrough";
        private const string HTML_COMMAND_FONT_NAME = "FontName";
        private const string HTML_COMMAND_FONT_SIZE = "FontSize";
        private const string HTML_COMMAND_FORE_COLOR = "ForeColor";
        private const string HTML_COMMAND_INSERT_FORMAT_BLOCK = "FormatBlock";
        private const string HTML_COMMAND_REMOVE_FORMAT = "RemoveFormat";
        private const string HTML_COMMAND_JUSTIFY_LEFT = "JustifyLeft";
        private const string HTML_COMMAND_JUSTIFY_CENTER = "JustifyCenter";
        private const string HTML_COMMAND_JUSTIFY_RIGHT = "JustifyRight";
        private const string HTML_COMMAND_INDENT = "Indent";
        private const string HTML_COMMAND_OUTDENT = "Outdent";
        private const string HTML_COMMAND_INSERT_LINE = "InsertHorizontalRule";
        private const string HTML_COMMAND_INSERT_LIST = "Insert{0}List"; // replace with (Un)Ordered
        private const string HTML_COMMAND_INSERT_IMAGE = "InsertImage";
        private const string HTML_COMMAND_INSERT_LINK = "CreateLink";
        private const string HTML_COMMAND_REMOVE_LINK = "Unlink";
        private const string HTML_COMMAND_TEXT_CUT = "Cut";
        private const string HTML_COMMAND_TEXT_COPY = "Copy";
        private const string HTML_COMMAND_TEXT_PASTE = "Paste";
        private const string HTML_COMMAND_TEXT_DELETE = "Delete";
        private const string HTML_COMMAND_TEXT_UNDO = "Undo";
        private const string HTML_COMMAND_TEXT_REDO = "Redo";
        private const string HTML_COMMAND_TEXT_SELECT_ALL = "SelectAll";
        private const string HTML_COMMAND_TEXT_UNSELECT = "Unselect";
        private const string HTML_COMMAND_TEXT_PRINT = "Print";

        // internal command constants
        private const string INTERNAL_COMMAND_TEXTCUT = "TextCut";
        private const string INTERNAL_COMMAND_TEXTCOPY = "TextCopy";
        private const string INTERNAL_COMMAND_TEXTPASTE = "TextPaste";
        private const string INTERNAL_COMMAND_TEXTDELETE = "TextDelete";
        private const string INTERNAL_COMMAND_CLEARSELECT = "ClearSelect";
        private const string INTERNAL_COMMAND_SELECTALL = "SelectAll";
        private const string INTERNAL_COMMAND_EDITUNDO = "EditUndo";
        private const string INTERNAL_COMMAND_EDITREDO = "EditRedo";
        private const string INTERNAL_COMMAND_FORMATBOLD = "FormatBold";
        private const string INTERNAL_COMMAND_FORMATUNDERLINE = "FormatUnderline";
        private const string INTERNAL_COMMAND_FORMATITALIC = "FormatItalic";
        private const string INTERNAL_COMMAND_FORMATSUPERSCRIPT = "FormatSuperscript";
        private const string INTERNAL_COMMAND_FORMATSUBSCRIPT = "FormatSubscript";
        private const string INTERNAL_COMMAND_FORMATSTRIKEOUT = "FormatStrikeout";
        private const string INTERNAL_COMMAND_FONTDIALOG = "FontDialog";
        private const string INTERNAL_COMMAND_FONTNORMAL = "FontNormal";
        private const string INTERNAL_COMMAND_COLORDIALOG = "ColorDialog";
        private const string INTERNAL_COMMAND_FONTINCREASE = "FontIncrease";
        private const string INTERNAL_COMMAND_FONTDECREASE = "FontDecrease";
        private const string INTERNAL_COMMAND_JUSTIFYLEFT = "JustifyLeft";
        private const string INTERNAL_COMMAND_JUSTIFYCENTER = "JustifyCenter";
        private const string INTERNAL_COMMAND_JUSTIFYRIGHT = "JustifyRight";
        private const string INTERNAL_COMMAND_FONTINDENT = "FontIndent";
        private const string INTERNAL_COMMAND_FONTOUTDENT = "FontOutdent";
        private const string INTERNAL_COMMAND_LISTORDERED = "ListOrdered";
        private const string INTERNAL_COMMAND_LISTUNORDERED = "ListUnordered";
        private const string INTERNAL_COMMAND_INSERTLINE = "InsertLine";
        private const string INTERNAL_COMMAND_INSERTTABLE = "InsertTable";
        private const string INTERNAL_COMMAND_TABLEPROPERTIES = "TableModify";
        private const string INTERNAL_COMMAND_TABLEINSERTROW = "TableInsertRow";
        private const string INTERNAL_COMMAND_TABLEDELETEROW = "TableDeleteRow";
        private const string INTERNAL_COMMAND_INSERTIMAGE = "InsertImage";
        private const string INTERNAL_COMMAND_INSERTLINK = "InsertLink";
        private const string INTERNAL_COMMAND_INSERTTEXT = "InsertText";
        private const string INTERNAL_COMMAND_INSERTHTML = "InsertHtml";
        private const string INTERNAL_COMMAND_FINDREPLACE = "FindReplace";
        private const string INTERNAL_COMMAND_DOCUMENTPRINT = "DocumentPrint";
        private const string INTERNAL_COMMAND_OPENFILE = "OpenFile";
        private const string INTERNAL_COMMAND_SAVEFILE = "SaveFile";
        private const string INTERNAL_COMMAND_HTMLEDITOR = "HtmlEditor";
        private const string INTERNAL_TOGGLE_OVERWRITE = "ToggleOverwrite";
        private const string INTERNAL_TOGGLE_TOOLBAR = "ToggleToolbar";
        private const string INTERNAL_TOGGLE_SCROLLBAR = "ToggleScrollbar";
        private const string INTERNAL_TOGGLE_WORDWRAP = "ToggleWordwrap";

        // browser html constan expressions
        private const string EMPTY_SPACE = @"&nbsp;";
        private const string BLANK_HTML_PAGE = "about:blank";
        private const string TARGET_WINDOW_NEW = "_BLANK";
        private const string TARGET_WINDOW_SAME = "_SELF";

        // constants for displaying the HTML dialog
        private const string HTML_TITLE_EDIT = "Edit Html";
        private const string HTML_TITLE_VIEW = "View Html";
        private const string PASTE_TITLE_HTML = "Enter Html";
        private const string PASTE_TITLE_TEXT = "Enter Text";
        private const string HTML_TITLE_OPENFILE = "Open Html File";
        private const string HTML_TITLE_SAVEFILE = "Save Html File";
        private const string HTML_FILTER = "Html files (*.html,*.htm)|*.html;*htm|All files (*.*)|*.*";
        private const string HTML_EXTENSION = "html";
        private const string CONTENT_EDITABLE_INHERIT = "inherit";
        private const string DEFAULT_HTML_TEXT = "";

        // constants for regular expression work
        // BODY_INNER_TEXT_PARSE = @"(<)/*\w*/*(>)";
        // HREF_TEST_EXPRESSION = @"(http|ftp|https):\/\/[\w]+(.[\w]+)([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?";
        // BODY_PARSE_EXPRESSION = @"(?<preBody>.*)(?<bodyOpen><body.*?>)(?<innerBody>.*)(?<bodyClose></body>)(?<afterBody>.*)";
        private const string HREF_TEST_EXPRESSION =
            @"mailto\:|(news|(ht|f)tp(s?))\:\/\/[\w]+(.[\w]+)([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?";

        private const string BODY_PARSE_PRE_EXPRESSION = @"(<body).*?(</body)";

        private const string BODY_PARSE_EXPRESSION =
            @"(?<bodyOpen>(<body).*?>)(?<innerBody>.*?)(?<bodyClose>(</body\s*>))";

        private const string BODY_DEFAULT_TAG = @"<Body></Body>";
        private const string BODY_TAG_PARSE_MATCH = @"${bodyOpen}${bodyClose}";
        private const string BODY_INNER_PARSE_MATCH = @"${innerBody}";

        private const string CONTENTTYPE_PARSE_EXPRESSION =
            @"^(?<mainType>\w+)(\/?)(?<subType>\w*)((\s*;\s*charset=)?)(?<charSet>.*)";

        private const string CONTENTTYPE_PARSE_MAINTYPE = @"${mainType}";
        private const string CONTENTTYPE_PARSE_SUBTYPE = @"${subType}";
        private const string CONTENTTYPE_PARSE_CHARSET = @"${charSet}";

        #endregion

        # region Fields

        // browser constants and commands
        private object EMPTY_PARAMETER;

        // acceptable formatting commands
        // in case order to enable binary search
        private readonly string[] m_formatCommands = new[]
                                                         {
                                                             "Formatted", "Heading 1", "Heading 2", "Heading 3",
                                                             "Heading 4",
                                                             "Heading 5", "Normal"
                                                         };

        // document and body elements
        private HtmlDocument m_document;
        private HtmlBody m_body;
        private HtmlStyleSheet m_stylesheet;
        private HtmlScriptElement m_script;
        private volatile bool m_loading;
        private volatile bool m_codeNavigate;
        private volatile bool m_rebaseUrlsNeeded;

        // default values used to reset values
        private Color m_defaultBackColor;
        private Color m_defaultForeColor;
        private HtmlFontProperty m_defaultFont;

        // internal property values
        private bool m_readOnly;
        private bool m_toolbarVisible;
        private bool m_enableVisualStyles;
        private DockStyle m_toolbarDock;
        private string m_bodyText;
        private string m_bodyHtml;
        private string m_bodyUrl;

        // internal body property values
        private Color m_bodyBackColor;
        private Color m_bodyForeColor;
        private HtmlFontProperty m_bodyFont;
        private int[] m_customColors;
        private string m_htmlDirectory;
        private NavigateActionOption m_navigateWindow;
        private DisplayScrollBarOption m_scrollBars;
        private bool m_autoWordWrap;

        // find and replace internal text range
        private HtmlTextRange m_findRange;

        #endregion

        #region UI Gen Fields

        private AxWebBrowser editorWebBrowser;
        private ToolBar editorToolbar;
        private ImageList toolbarImageList;
        private IContainer components;
        private ToolBarButton toolBarCopy;
        private ToolBarButton toolBarPaste;
        private ToolBarButton toolBarBold;
        private ToolBarButton toolBarUnderline;
        private ToolBarButton toolBarItalic;
        private ToolBarButton toolBarUndo;
        private ToolBarButton toolBarRedo;
        private ToolBarButton toolBarFont;
        private ToolBarButton toolBarCut;
        private ToolBarButton toolBarNormal;
        private ToolBarButton toolBarJustLeft;
        private ToolBarButton toolBarJustCenter;
        private ToolBarButton toolBarJustRight;
        private ToolBarButton toolBarIndent;
        private ToolBarButton toolBarOutdent;
        private ToolBarButton toolBarListUnordered;
        private ToolBarButton toolBarFontUp;
        private ToolBarButton toolBarColor;
        private ToolBarButton toolBarInsertImage;
        private ToolBarButton toolBarInsertLink;
        private ToolBarButton toolBarFindReplace;
        private ToolBarButton toolBarPrint;
        private ToolBarButton toolBarEditSep1;
        private ToolBarButton toolBarEditSep2;
        private ToolBarButton toolBarEditSep3;
        private ToolBarButton toolBarEditSep4;
        private ToolBarButton toolBarEditSep5;
        private ToolBarButton toolBarEditSep6;
        private ToolBarButton toolBarInsertLine;
        private ToolBarButton toolBarListOrdered;
        private MenuItem menuTextUndo;
        private MenuItem menuTextRedo;
        private MenuItem menuTextSep1;
        private MenuItem menuTextCut;
        private MenuItem menuTextCopy;
        private MenuItem menuTextPaste;
        private MenuItem menuTextDelete;
        private MenuItem menuTextFont;
        private MenuItem menuTextFontIncrease;
        private MenuItem menuTextFontDecrease;
        private MenuItem menuTextFontBold;
        private MenuItem menuTextFontItalic;
        private MenuItem menuTextFontUnderline;
        private MenuItem menuTextFontIndent;
        private MenuItem menuTextFontOutdent;
        private MenuItem menuTextFontDialog;
        private MenuItem menuTextFontColor;
        private MenuItem menuTextSelectAll;
        private MenuItem menuJustify;
        private MenuItem menuJustifyLeft;
        private MenuItem menuJustifyCenter;
        private MenuItem menuJustifyRight;
        private MenuItem menuTextSelectNone;
        private MenuItem menuFormatting;
        private MenuItem menuTextFontSep1;
        private MenuItem menuTextFontSep2;
        private MenuItem menuTextFontSep3;
        private MenuItem menuTextFontSuperscript;
        private MenuItem menuTextFontSubscript;
        private MenuItem menuInsert;
        private MenuItem menuInsertLine;
        private MenuItem menuInsertLink;
        private MenuItem menuInsertImage;
        private MenuItem menuInsertText;
        private MenuItem menuInsertHtml;
        private MenuItem menuInsertTable;
        private ToolBarButton toolBarInsertTable;
        private MenuItem menuMainSep1;
        private MenuItem menuMainSep2;
        private MenuItem menuText;
        private MenuItem menuTableModify;
        private ContextMenu contextMenuMain;
        private MenuItem menuTextSep3;
        private MenuItem menuTextSep2;
        private MenuItem menuTextFindReplace;
        private MenuItem menuDocument;
        private MenuItem menuDocumentOpen;
        private MenuItem menuDocumentSave;
        private MenuItem menuDocumentSep1;
        private MenuItem menuDocumentPrint;
        private MenuItem menuTextFontNormal;
        private MenuItem menuTextFontStrikeout;
        private MenuItem menuTextFontListOrdered;
        private MenuItem menuTextFontListUnordered;
        private MenuItem menuDocumentSep2;
        private MenuItem menuDocumentToolbar;
        private MenuItem menuDocumentScrollbar;
        private MenuItem menuDocumentWordwrap;
        private MenuItem menuDocumentOverwrite;
        private MenuItem menuTableProperties;
        private MenuItem menuTableInsertRow;
        private MenuItem menuTableDeleteRow;
        private ToolBarButton toolBarEditSep7;
        private ToolBarButton toolBarHTML;
        private ToolBarButton toolBarFontDown;

        #endregion

        public AxWebBrowser WebBrowser
        {
            get
            {
                return editorWebBrowser;
            }
        }

        public HtmlEditorControl()
        {
            InitializeComponent();

            // define the context menu for format commands
            DefineFormatBlockMenu();

            // define the default values
            // browser constants and commands
            EMPTY_PARAMETER = Missing.Value;

            // default values used to reset values
            m_defaultBackColor = Color.White;
            m_defaultForeColor = Color.Black;
            m_defaultFont = new HtmlFontProperty(Font);

            // set browser default values to hide IE items
            editorWebBrowser.AddressBar = false;
            editorWebBrowser.MenuBar = false;
            editorWebBrowser.StatusBar = false;

            //obtain the underlying web browser and set options
            //SHDocVw.WebBrowser webBrowser = (SHDocVw.WebBrowser)this.editorWebBrowser.GetOcx();

            // define the default values of the properties
            m_readOnly = false;
            m_toolbarVisible = false;
            m_enableVisualStyles = false;
            m_toolbarDock = DockStyle.Bottom;
            m_bodyText = DEFAULT_HTML_TEXT;
            m_bodyHtml = DEFAULT_HTML_TEXT;
            m_bodyBackColor = m_defaultBackColor;
            m_bodyForeColor = m_defaultForeColor;
            m_bodyFont = m_defaultFont;
            m_scrollBars = DisplayScrollBarOption.Auto;
            m_htmlDirectory = string.Empty;
            m_navigateWindow = NavigateActionOption.Default;
            m_autoWordWrap = true;
            m_stylesheet = null;
            m_script = null;

            // define context menu state
            menuDocumentToolbar.Checked = true;
            menuDocumentScrollbar.Checked = true;
            menuDocumentWordwrap.Checked = true;

            // load the blank Html page to load the MsHtml object model
            BrowserCodeNavigate(BLANK_HTML_PAGE);

            // after load ensure document marked as editable
            ReadOnly = m_readOnly;
        }

        // once an html docuemnt has been loaded define the internal values
        private void DefineBodyAttributes()
        {
            // define the body colors based on the new body html
            if (m_body.bgColor == null)
            {
                m_bodyBackColor = m_defaultBackColor;
            }
            else
            {
                m_bodyBackColor = ColorTranslator.FromHtml((string)m_body.bgColor);
            }

            if (m_body.text == null)
            {
                m_bodyForeColor = m_defaultForeColor;
            }
            else
            {
                m_bodyForeColor = ColorTranslator.FromHtml((string)m_body.text);
            }

            // define the font object based on current font of new document
            // deafult used unless a style on the body modifies the value
            HtmlStyle _bodyStyle = m_body.style;

            if (_bodyStyle != null)
            {
                string _fontName = m_bodyFont.Name;
                HtmlFontSize _fontSize = m_bodyFont.Size;
                bool _fontBold = m_bodyFont.Bold;
                bool _fontItalic = m_bodyFont.Italic;

                // define the font name if defined in the style
                if (_bodyStyle.fontFamily != null)
                    _fontName = _bodyStyle.fontFamily;

                if (_bodyStyle.fontSize != null)
                    _fontSize = HtmlFontConversion.StyleSizeToHtml(_bodyStyle.fontSize.ToString());

                if (_bodyStyle.fontWeight != null)
                    _fontBold = HtmlFontConversion.IsStyleBold(_bodyStyle.fontWeight);

                if (_bodyStyle.fontStyle != null)
                    _fontItalic = HtmlFontConversion.IsStyleItalic(_bodyStyle.fontStyle);

                bool _fontUnderline = _bodyStyle.textDecorationUnderline;

                // define the new font object and set the property
                m_bodyFont = new HtmlFontProperty(_fontName, _fontSize, _fontBold, _fontItalic, _fontUnderline);
                BodyFont = m_bodyFont;
            }

            // define the content based on the current value
            ReadOnly = m_readOnly;
            ScrollBars = m_scrollBars;
            AutoWordWrap = m_autoWordWrap;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HtmlEditorControl));
            this.editorToolbar = new System.Windows.Forms.ToolBar();
            this.toolBarCut = new System.Windows.Forms.ToolBarButton();
            this.toolBarCopy = new System.Windows.Forms.ToolBarButton();
            this.toolBarPaste = new System.Windows.Forms.ToolBarButton();
            this.toolBarEditSep1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarUndo = new System.Windows.Forms.ToolBarButton();
            this.toolBarRedo = new System.Windows.Forms.ToolBarButton();
            this.toolBarEditSep2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarBold = new System.Windows.Forms.ToolBarButton();
            this.toolBarUnderline = new System.Windows.Forms.ToolBarButton();
            this.toolBarItalic = new System.Windows.Forms.ToolBarButton();
            this.toolBarFont = new System.Windows.Forms.ToolBarButton();
            this.toolBarNormal = new System.Windows.Forms.ToolBarButton();
            this.toolBarColor = new System.Windows.Forms.ToolBarButton();
            this.toolBarFontUp = new System.Windows.Forms.ToolBarButton();
            this.toolBarFontDown = new System.Windows.Forms.ToolBarButton();
            this.toolBarEditSep3 = new System.Windows.Forms.ToolBarButton();
            this.toolBarJustLeft = new System.Windows.Forms.ToolBarButton();
            this.toolBarJustCenter = new System.Windows.Forms.ToolBarButton();
            this.toolBarJustRight = new System.Windows.Forms.ToolBarButton();
            this.toolBarIndent = new System.Windows.Forms.ToolBarButton();
            this.toolBarOutdent = new System.Windows.Forms.ToolBarButton();
            this.toolBarEditSep4 = new System.Windows.Forms.ToolBarButton();
            this.toolBarListOrdered = new System.Windows.Forms.ToolBarButton();
            this.toolBarListUnordered = new System.Windows.Forms.ToolBarButton();
            this.toolBarEditSep5 = new System.Windows.Forms.ToolBarButton();
            this.toolBarInsertLine = new System.Windows.Forms.ToolBarButton();
            this.toolBarInsertTable = new System.Windows.Forms.ToolBarButton();
            this.toolBarInsertImage = new System.Windows.Forms.ToolBarButton();
            this.toolBarInsertLink = new System.Windows.Forms.ToolBarButton();
            this.toolBarEditSep6 = new System.Windows.Forms.ToolBarButton();
            this.toolBarFindReplace = new System.Windows.Forms.ToolBarButton();
            this.toolBarPrint = new System.Windows.Forms.ToolBarButton();
            this.toolBarEditSep7 = new System.Windows.Forms.ToolBarButton();
            this.toolBarHTML = new System.Windows.Forms.ToolBarButton();
            this.toolbarImageList = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuMain = new System.Windows.Forms.ContextMenu();
            this.menuTableModify = new System.Windows.Forms.MenuItem();
            this.menuTableProperties = new System.Windows.Forms.MenuItem();
            this.menuTableInsertRow = new System.Windows.Forms.MenuItem();
            this.menuTableDeleteRow = new System.Windows.Forms.MenuItem();
            this.menuText = new System.Windows.Forms.MenuItem();
            this.menuTextUndo = new System.Windows.Forms.MenuItem();
            this.menuTextRedo = new System.Windows.Forms.MenuItem();
            this.menuTextSep1 = new System.Windows.Forms.MenuItem();
            this.menuTextCut = new System.Windows.Forms.MenuItem();
            this.menuTextCopy = new System.Windows.Forms.MenuItem();
            this.menuTextPaste = new System.Windows.Forms.MenuItem();
            this.menuTextSep2 = new System.Windows.Forms.MenuItem();
            this.menuTextFindReplace = new System.Windows.Forms.MenuItem();
            this.menuTextSep3 = new System.Windows.Forms.MenuItem();
            this.menuTextSelectNone = new System.Windows.Forms.MenuItem();
            this.menuTextSelectAll = new System.Windows.Forms.MenuItem();
            this.menuTextDelete = new System.Windows.Forms.MenuItem();
            this.menuDocument = new System.Windows.Forms.MenuItem();
            this.menuDocumentOpen = new System.Windows.Forms.MenuItem();
            this.menuDocumentSave = new System.Windows.Forms.MenuItem();
            this.menuDocumentSep1 = new System.Windows.Forms.MenuItem();
            this.menuDocumentPrint = new System.Windows.Forms.MenuItem();
            this.menuDocumentSep2 = new System.Windows.Forms.MenuItem();
            this.menuDocumentToolbar = new System.Windows.Forms.MenuItem();
            this.menuDocumentScrollbar = new System.Windows.Forms.MenuItem();
            this.menuDocumentWordwrap = new System.Windows.Forms.MenuItem();
            this.menuDocumentOverwrite = new System.Windows.Forms.MenuItem();
            this.menuMainSep1 = new System.Windows.Forms.MenuItem();
            this.menuTextFont = new System.Windows.Forms.MenuItem();
            this.menuTextFontDialog = new System.Windows.Forms.MenuItem();
            this.menuTextFontColor = new System.Windows.Forms.MenuItem();
            this.menuTextFontSep1 = new System.Windows.Forms.MenuItem();
            this.menuTextFontNormal = new System.Windows.Forms.MenuItem();
            this.menuTextFontBold = new System.Windows.Forms.MenuItem();
            this.menuTextFontItalic = new System.Windows.Forms.MenuItem();
            this.menuTextFontUnderline = new System.Windows.Forms.MenuItem();
            this.menuTextFontSuperscript = new System.Windows.Forms.MenuItem();
            this.menuTextFontSubscript = new System.Windows.Forms.MenuItem();
            this.menuTextFontStrikeout = new System.Windows.Forms.MenuItem();
            this.menuTextFontSep2 = new System.Windows.Forms.MenuItem();
            this.menuTextFontIncrease = new System.Windows.Forms.MenuItem();
            this.menuTextFontDecrease = new System.Windows.Forms.MenuItem();
            this.menuTextFontIndent = new System.Windows.Forms.MenuItem();
            this.menuTextFontOutdent = new System.Windows.Forms.MenuItem();
            this.menuTextFontSep3 = new System.Windows.Forms.MenuItem();
            this.menuTextFontListOrdered = new System.Windows.Forms.MenuItem();
            this.menuTextFontListUnordered = new System.Windows.Forms.MenuItem();
            this.menuJustify = new System.Windows.Forms.MenuItem();
            this.menuJustifyLeft = new System.Windows.Forms.MenuItem();
            this.menuJustifyCenter = new System.Windows.Forms.MenuItem();
            this.menuJustifyRight = new System.Windows.Forms.MenuItem();
            this.menuFormatting = new System.Windows.Forms.MenuItem();
            this.menuMainSep2 = new System.Windows.Forms.MenuItem();
            this.menuInsert = new System.Windows.Forms.MenuItem();
            this.menuInsertLine = new System.Windows.Forms.MenuItem();
            this.menuInsertLink = new System.Windows.Forms.MenuItem();
            this.menuInsertImage = new System.Windows.Forms.MenuItem();
            this.menuInsertText = new System.Windows.Forms.MenuItem();
            this.menuInsertHtml = new System.Windows.Forms.MenuItem();
            this.menuInsertTable = new System.Windows.Forms.MenuItem();
            this.editorWebBrowser = new AxSHDocVw.AxWebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.editorWebBrowser)).BeginInit();
            this.SuspendLayout();
            // 
            // editorToolbar
            // 
            resources.ApplyResources(this.editorToolbar, "editorToolbar");
            this.editorToolbar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.editorToolbar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarCut,
            this.toolBarCopy,
            this.toolBarPaste,
            this.toolBarEditSep1,
            this.toolBarUndo,
            this.toolBarRedo,
            this.toolBarEditSep2,
            this.toolBarBold,
            this.toolBarUnderline,
            this.toolBarItalic,
            this.toolBarFont,
            this.toolBarNormal,
            this.toolBarColor,
            this.toolBarFontUp,
            this.toolBarFontDown,
            this.toolBarEditSep3,
            this.toolBarJustLeft,
            this.toolBarJustCenter,
            this.toolBarJustRight,
            this.toolBarIndent,
            this.toolBarOutdent,
            this.toolBarEditSep4,
            this.toolBarListOrdered,
            this.toolBarListUnordered,
            this.toolBarEditSep5,
            this.toolBarInsertLine,
            this.toolBarInsertTable,
            this.toolBarInsertImage,
            this.toolBarInsertLink,
            this.toolBarEditSep6,
            this.toolBarFindReplace,
            this.toolBarPrint,
            this.toolBarEditSep7,
            this.toolBarHTML});
            this.editorToolbar.Divider = false;
            this.editorToolbar.ImageList = this.toolbarImageList;
            this.editorToolbar.Name = "editorToolbar";
            this.editorToolbar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.editorToolbar_ButtonClick);
            // 
            // toolBarCut
            // 
            resources.ApplyResources(this.toolBarCut, "toolBarCut");
            this.toolBarCut.Name = "toolBarCut";
            this.toolBarCut.Tag = "TextCut";
            // 
            // toolBarCopy
            // 
            resources.ApplyResources(this.toolBarCopy, "toolBarCopy");
            this.toolBarCopy.Name = "toolBarCopy";
            this.toolBarCopy.Tag = "TextCopy";
            // 
            // toolBarPaste
            // 
            resources.ApplyResources(this.toolBarPaste, "toolBarPaste");
            this.toolBarPaste.Name = "toolBarPaste";
            this.toolBarPaste.Tag = "TextPaste";
            // 
            // toolBarEditSep1
            // 
            this.toolBarEditSep1.Name = "toolBarEditSep1";
            this.toolBarEditSep1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarUndo
            // 
            resources.ApplyResources(this.toolBarUndo, "toolBarUndo");
            this.toolBarUndo.Name = "toolBarUndo";
            this.toolBarUndo.Tag = "EditUndo";
            // 
            // toolBarRedo
            // 
            resources.ApplyResources(this.toolBarRedo, "toolBarRedo");
            this.toolBarRedo.Name = "toolBarRedo";
            this.toolBarRedo.Tag = "EditRedo";
            // 
            // toolBarEditSep2
            // 
            this.toolBarEditSep2.Name = "toolBarEditSep2";
            this.toolBarEditSep2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarBold
            // 
            resources.ApplyResources(this.toolBarBold, "toolBarBold");
            this.toolBarBold.Name = "toolBarBold";
            this.toolBarBold.Tag = "FormatBold";
            // 
            // toolBarUnderline
            // 
            resources.ApplyResources(this.toolBarUnderline, "toolBarUnderline");
            this.toolBarUnderline.Name = "toolBarUnderline";
            this.toolBarUnderline.Tag = "FormatUnderline";
            // 
            // toolBarItalic
            // 
            resources.ApplyResources(this.toolBarItalic, "toolBarItalic");
            this.toolBarItalic.Name = "toolBarItalic";
            this.toolBarItalic.Tag = "FormatItalic";
            // 
            // toolBarFont
            // 
            resources.ApplyResources(this.toolBarFont, "toolBarFont");
            this.toolBarFont.Name = "toolBarFont";
            this.toolBarFont.Tag = "FontDialog";
            // 
            // toolBarNormal
            // 
            resources.ApplyResources(this.toolBarNormal, "toolBarNormal");
            this.toolBarNormal.Name = "toolBarNormal";
            this.toolBarNormal.Tag = "FontNormal";
            // 
            // toolBarColor
            // 
            resources.ApplyResources(this.toolBarColor, "toolBarColor");
            this.toolBarColor.Name = "toolBarColor";
            this.toolBarColor.Tag = "ColorDialog";
            // 
            // toolBarFontUp
            // 
            resources.ApplyResources(this.toolBarFontUp, "toolBarFontUp");
            this.toolBarFontUp.Name = "toolBarFontUp";
            this.toolBarFontUp.Tag = "FontIncrease";
            // 
            // toolBarFontDown
            // 
            resources.ApplyResources(this.toolBarFontDown, "toolBarFontDown");
            this.toolBarFontDown.Name = "toolBarFontDown";
            this.toolBarFontDown.Tag = "FontDecrease";
            // 
            // toolBarEditSep3
            // 
            this.toolBarEditSep3.Name = "toolBarEditSep3";
            this.toolBarEditSep3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarJustLeft
            // 
            resources.ApplyResources(this.toolBarJustLeft, "toolBarJustLeft");
            this.toolBarJustLeft.Name = "toolBarJustLeft";
            this.toolBarJustLeft.Tag = "JustifyLeft";
            // 
            // toolBarJustCenter
            // 
            resources.ApplyResources(this.toolBarJustCenter, "toolBarJustCenter");
            this.toolBarJustCenter.Name = "toolBarJustCenter";
            this.toolBarJustCenter.Tag = "JustifyCenter";
            // 
            // toolBarJustRight
            // 
            resources.ApplyResources(this.toolBarJustRight, "toolBarJustRight");
            this.toolBarJustRight.Name = "toolBarJustRight";
            this.toolBarJustRight.Tag = "JustifyRight";
            // 
            // toolBarIndent
            // 
            resources.ApplyResources(this.toolBarIndent, "toolBarIndent");
            this.toolBarIndent.Name = "toolBarIndent";
            this.toolBarIndent.Tag = "FontIndent";
            // 
            // toolBarOutdent
            // 
            resources.ApplyResources(this.toolBarOutdent, "toolBarOutdent");
            this.toolBarOutdent.Name = "toolBarOutdent";
            this.toolBarOutdent.Tag = "FontOutdent";
            // 
            // toolBarEditSep4
            // 
            this.toolBarEditSep4.Name = "toolBarEditSep4";
            this.toolBarEditSep4.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarListOrdered
            // 
            resources.ApplyResources(this.toolBarListOrdered, "toolBarListOrdered");
            this.toolBarListOrdered.Name = "toolBarListOrdered";
            this.toolBarListOrdered.Tag = "ListOrdered";
            // 
            // toolBarListUnordered
            // 
            resources.ApplyResources(this.toolBarListUnordered, "toolBarListUnordered");
            this.toolBarListUnordered.Name = "toolBarListUnordered";
            this.toolBarListUnordered.Tag = "ListUnordered";
            // 
            // toolBarEditSep5
            // 
            this.toolBarEditSep5.Name = "toolBarEditSep5";
            this.toolBarEditSep5.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            resources.ApplyResources(this.toolBarEditSep5, "toolBarEditSep5");
            // 
            // toolBarInsertLine
            // 
            resources.ApplyResources(this.toolBarInsertLine, "toolBarInsertLine");
            this.toolBarInsertLine.Name = "toolBarInsertLine";
            this.toolBarInsertLine.Tag = "InsertLine";
            // 
            // toolBarInsertTable
            // 
            resources.ApplyResources(this.toolBarInsertTable, "toolBarInsertTable");
            this.toolBarInsertTable.Name = "toolBarInsertTable";
            this.toolBarInsertTable.Tag = "InsertTable";
            // 
            // toolBarInsertImage
            // 
            resources.ApplyResources(this.toolBarInsertImage, "toolBarInsertImage");
            this.toolBarInsertImage.Name = "toolBarInsertImage";
            this.toolBarInsertImage.Tag = "InsertImage";
            // 
            // toolBarInsertLink
            // 
            resources.ApplyResources(this.toolBarInsertLink, "toolBarInsertLink");
            this.toolBarInsertLink.Name = "toolBarInsertLink";
            this.toolBarInsertLink.Tag = "InsertLink";
            // 
            // toolBarEditSep6
            // 
            this.toolBarEditSep6.Name = "toolBarEditSep6";
            this.toolBarEditSep6.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            resources.ApplyResources(this.toolBarEditSep6, "toolBarEditSep6");
            // 
            // toolBarFindReplace
            // 
            resources.ApplyResources(this.toolBarFindReplace, "toolBarFindReplace");
            this.toolBarFindReplace.Name = "toolBarFindReplace";
            this.toolBarFindReplace.Tag = "FindReplace";
            // 
            // toolBarPrint
            // 
            resources.ApplyResources(this.toolBarPrint, "toolBarPrint");
            this.toolBarPrint.Name = "toolBarPrint";
            this.toolBarPrint.Tag = "DocumentPrint";
            // 
            // toolBarEditSep7
            // 
            this.toolBarEditSep7.Name = "toolBarEditSep7";
            this.toolBarEditSep7.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            resources.ApplyResources(this.toolBarEditSep7, "toolBarEditSep7");
            // 
            // toolBarHTML
            // 
            resources.ApplyResources(this.toolBarHTML, "toolBarHTML");
            this.toolBarHTML.Name = "toolBarHTML";
            this.toolBarHTML.Tag = "HtmlEditor";
            // 
            // toolbarImageList
            // 
            this.toolbarImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("toolbarImageList.ImageStream")));
            this.toolbarImageList.TransparentColor = System.Drawing.Color.Silver;
            this.toolbarImageList.Images.SetKeyName(0, "");
            this.toolbarImageList.Images.SetKeyName(1, "");
            this.toolbarImageList.Images.SetKeyName(2, "");
            this.toolbarImageList.Images.SetKeyName(3, "");
            this.toolbarImageList.Images.SetKeyName(4, "");
            this.toolbarImageList.Images.SetKeyName(5, "");
            this.toolbarImageList.Images.SetKeyName(6, "");
            this.toolbarImageList.Images.SetKeyName(7, "");
            this.toolbarImageList.Images.SetKeyName(8, "");
            this.toolbarImageList.Images.SetKeyName(9, "");
            this.toolbarImageList.Images.SetKeyName(10, "");
            this.toolbarImageList.Images.SetKeyName(11, "");
            this.toolbarImageList.Images.SetKeyName(12, "");
            this.toolbarImageList.Images.SetKeyName(13, "");
            this.toolbarImageList.Images.SetKeyName(14, "");
            this.toolbarImageList.Images.SetKeyName(15, "");
            this.toolbarImageList.Images.SetKeyName(16, "");
            this.toolbarImageList.Images.SetKeyName(17, "");
            this.toolbarImageList.Images.SetKeyName(18, "");
            this.toolbarImageList.Images.SetKeyName(19, "");
            this.toolbarImageList.Images.SetKeyName(20, "");
            this.toolbarImageList.Images.SetKeyName(21, "");
            this.toolbarImageList.Images.SetKeyName(22, "");
            this.toolbarImageList.Images.SetKeyName(23, "");
            this.toolbarImageList.Images.SetKeyName(24, "");
            this.toolbarImageList.Images.SetKeyName(25, "");
            this.toolbarImageList.Images.SetKeyName(26, "");
            this.toolbarImageList.Images.SetKeyName(27, "");
            this.toolbarImageList.Images.SetKeyName(28, "");
            this.toolbarImageList.Images.SetKeyName(29, "html.png");
            // 
            // contextMenuMain
            // 
            this.contextMenuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuTableModify,
            this.menuText,
            this.menuDocument,
            this.menuMainSep1,
            this.menuTextFont,
            this.menuJustify,
            this.menuFormatting,
            this.menuMainSep2,
            this.menuInsert});
            // 
            // menuTableModify
            // 
            this.menuTableModify.Index = 0;
            this.menuTableModify.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuTableProperties,
            this.menuTableInsertRow,
            this.menuTableDeleteRow});
            resources.ApplyResources(this.menuTableModify, "menuTableModify");
            // 
            // menuTableProperties
            // 
            this.menuTableProperties.Index = 0;
            resources.ApplyResources(this.menuTableProperties, "menuTableProperties");
            this.menuTableProperties.Click += new System.EventHandler(this.menuTableProperties_Click);
            // 
            // menuTableInsertRow
            // 
            this.menuTableInsertRow.Index = 1;
            resources.ApplyResources(this.menuTableInsertRow, "menuTableInsertRow");
            this.menuTableInsertRow.Click += new System.EventHandler(this.menuTableInsertRow_Click);
            // 
            // menuTableDeleteRow
            // 
            this.menuTableDeleteRow.Index = 2;
            resources.ApplyResources(this.menuTableDeleteRow, "menuTableDeleteRow");
            this.menuTableDeleteRow.Click += new System.EventHandler(this.menuTableDeleteRow_Click);
            // 
            // menuText
            // 
            this.menuText.Index = 1;
            this.menuText.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuTextUndo,
            this.menuTextRedo,
            this.menuTextSep1,
            this.menuTextCut,
            this.menuTextCopy,
            this.menuTextPaste,
            this.menuTextSep2,
            this.menuTextFindReplace,
            this.menuTextSep3,
            this.menuTextSelectNone,
            this.menuTextSelectAll,
            this.menuTextDelete});
            resources.ApplyResources(this.menuText, "menuText");
            // 
            // menuTextUndo
            // 
            this.menuTextUndo.Index = 0;
            resources.ApplyResources(this.menuTextUndo, "menuTextUndo");
            this.menuTextUndo.Click += new System.EventHandler(this.menuTextUndo_Click);
            // 
            // menuTextRedo
            // 
            this.menuTextRedo.Index = 1;
            resources.ApplyResources(this.menuTextRedo, "menuTextRedo");
            this.menuTextRedo.Click += new System.EventHandler(this.menuTextRedo_Click);
            // 
            // menuTextSep1
            // 
            this.menuTextSep1.Index = 2;
            resources.ApplyResources(this.menuTextSep1, "menuTextSep1");
            // 
            // menuTextCut
            // 
            this.menuTextCut.Index = 3;
            resources.ApplyResources(this.menuTextCut, "menuTextCut");
            this.menuTextCut.Click += new System.EventHandler(this.menuTextCut_Click);
            // 
            // menuTextCopy
            // 
            this.menuTextCopy.Index = 4;
            resources.ApplyResources(this.menuTextCopy, "menuTextCopy");
            this.menuTextCopy.Click += new System.EventHandler(this.menuTextCopy_Click);
            // 
            // menuTextPaste
            // 
            this.menuTextPaste.Index = 5;
            resources.ApplyResources(this.menuTextPaste, "menuTextPaste");
            this.menuTextPaste.Click += new System.EventHandler(this.menuTextPaste_Click);
            // 
            // menuTextSep2
            // 
            this.menuTextSep2.Index = 6;
            resources.ApplyResources(this.menuTextSep2, "menuTextSep2");
            // 
            // menuTextFindReplace
            // 
            this.menuTextFindReplace.Index = 7;
            resources.ApplyResources(this.menuTextFindReplace, "menuTextFindReplace");
            this.menuTextFindReplace.Click += new System.EventHandler(this.menuTextFindReplace_Click);
            // 
            // menuTextSep3
            // 
            this.menuTextSep3.Index = 8;
            resources.ApplyResources(this.menuTextSep3, "menuTextSep3");
            // 
            // menuTextSelectNone
            // 
            this.menuTextSelectNone.Index = 9;
            resources.ApplyResources(this.menuTextSelectNone, "menuTextSelectNone");
            this.menuTextSelectNone.Click += new System.EventHandler(this.menuTextSelectNone_Click);
            // 
            // menuTextSelectAll
            // 
            this.menuTextSelectAll.Index = 10;
            resources.ApplyResources(this.menuTextSelectAll, "menuTextSelectAll");
            this.menuTextSelectAll.Click += new System.EventHandler(this.menuTextSelectAll_Click);
            // 
            // menuTextDelete
            // 
            this.menuTextDelete.Index = 11;
            resources.ApplyResources(this.menuTextDelete, "menuTextDelete");
            this.menuTextDelete.Click += new System.EventHandler(this.menuTextDelete_Click);
            // 
            // menuDocument
            // 
            this.menuDocument.Index = 2;
            this.menuDocument.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuDocumentOpen,
            this.menuDocumentSave,
            this.menuDocumentSep1,
            this.menuDocumentPrint,
            this.menuDocumentSep2,
            this.menuDocumentToolbar,
            this.menuDocumentScrollbar,
            this.menuDocumentWordwrap,
            this.menuDocumentOverwrite});
            resources.ApplyResources(this.menuDocument, "menuDocument");
            // 
            // menuDocumentOpen
            // 
            this.menuDocumentOpen.Index = 0;
            resources.ApplyResources(this.menuDocumentOpen, "menuDocumentOpen");
            this.menuDocumentOpen.Click += new System.EventHandler(this.menuDocumentOpen_Click);
            // 
            // menuDocumentSave
            // 
            this.menuDocumentSave.Index = 1;
            resources.ApplyResources(this.menuDocumentSave, "menuDocumentSave");
            this.menuDocumentSave.Click += new System.EventHandler(this.menuDocumentSave_Click);
            // 
            // menuDocumentSep1
            // 
            this.menuDocumentSep1.Index = 2;
            resources.ApplyResources(this.menuDocumentSep1, "menuDocumentSep1");
            // 
            // menuDocumentPrint
            // 
            this.menuDocumentPrint.Index = 3;
            resources.ApplyResources(this.menuDocumentPrint, "menuDocumentPrint");
            this.menuDocumentPrint.Click += new System.EventHandler(this.menuDocumentPrint_Click);
            // 
            // menuDocumentSep2
            // 
            this.menuDocumentSep2.Index = 4;
            resources.ApplyResources(this.menuDocumentSep2, "menuDocumentSep2");
            // 
            // menuDocumentToolbar
            // 
            this.menuDocumentToolbar.Checked = true;
            this.menuDocumentToolbar.Index = 5;
            resources.ApplyResources(this.menuDocumentToolbar, "menuDocumentToolbar");
            this.menuDocumentToolbar.Click += new System.EventHandler(this.menuDocumentToolbar_Click);
            // 
            // menuDocumentScrollbar
            // 
            this.menuDocumentScrollbar.Index = 6;
            resources.ApplyResources(this.menuDocumentScrollbar, "menuDocumentScrollbar");
            this.menuDocumentScrollbar.Click += new System.EventHandler(this.menuDocumentScrollbar_Click);
            // 
            // menuDocumentWordwrap
            // 
            this.menuDocumentWordwrap.Index = 7;
            resources.ApplyResources(this.menuDocumentWordwrap, "menuDocumentWordwrap");
            this.menuDocumentWordwrap.Click += new System.EventHandler(this.menuDocumentWordwrap_Click);
            // 
            // menuDocumentOverwrite
            // 
            this.menuDocumentOverwrite.Index = 8;
            resources.ApplyResources(this.menuDocumentOverwrite, "menuDocumentOverwrite");
            this.menuDocumentOverwrite.Click += new System.EventHandler(this.menuDocumentOverwrite_Click);
            // 
            // menuMainSep1
            // 
            this.menuMainSep1.Index = 3;
            resources.ApplyResources(this.menuMainSep1, "menuMainSep1");
            // 
            // menuTextFont
            // 
            this.menuTextFont.Index = 4;
            this.menuTextFont.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuTextFontDialog,
            this.menuTextFontColor,
            this.menuTextFontSep1,
            this.menuTextFontNormal,
            this.menuTextFontBold,
            this.menuTextFontItalic,
            this.menuTextFontUnderline,
            this.menuTextFontSuperscript,
            this.menuTextFontSubscript,
            this.menuTextFontStrikeout,
            this.menuTextFontSep2,
            this.menuTextFontIncrease,
            this.menuTextFontDecrease,
            this.menuTextFontIndent,
            this.menuTextFontOutdent,
            this.menuTextFontSep3,
            this.menuTextFontListOrdered,
            this.menuTextFontListUnordered});
            resources.ApplyResources(this.menuTextFont, "menuTextFont");
            // 
            // menuTextFontDialog
            // 
            this.menuTextFontDialog.Index = 0;
            resources.ApplyResources(this.menuTextFontDialog, "menuTextFontDialog");
            this.menuTextFontDialog.Click += new System.EventHandler(this.menuTextFontDialog_Click);
            // 
            // menuTextFontColor
            // 
            this.menuTextFontColor.Index = 1;
            resources.ApplyResources(this.menuTextFontColor, "menuTextFontColor");
            this.menuTextFontColor.Click += new System.EventHandler(this.menuTextFontColor_Click);
            // 
            // menuTextFontSep1
            // 
            this.menuTextFontSep1.Index = 2;
            resources.ApplyResources(this.menuTextFontSep1, "menuTextFontSep1");
            // 
            // menuTextFontNormal
            // 
            this.menuTextFontNormal.Index = 3;
            resources.ApplyResources(this.menuTextFontNormal, "menuTextFontNormal");
            this.menuTextFontNormal.Click += new System.EventHandler(this.menuTextFontNormal_Click);
            // 
            // menuTextFontBold
            // 
            this.menuTextFontBold.Index = 4;
            resources.ApplyResources(this.menuTextFontBold, "menuTextFontBold");
            this.menuTextFontBold.Click += new System.EventHandler(this.menuTextFontBold_Click);
            // 
            // menuTextFontItalic
            // 
            this.menuTextFontItalic.Index = 5;
            resources.ApplyResources(this.menuTextFontItalic, "menuTextFontItalic");
            this.menuTextFontItalic.Click += new System.EventHandler(this.menuTextFontItalic_Click);
            // 
            // menuTextFontUnderline
            // 
            this.menuTextFontUnderline.Index = 6;
            resources.ApplyResources(this.menuTextFontUnderline, "menuTextFontUnderline");
            this.menuTextFontUnderline.Click += new System.EventHandler(this.menuTextFontUnderline_Click);
            // 
            // menuTextFontSuperscript
            // 
            this.menuTextFontSuperscript.Index = 7;
            resources.ApplyResources(this.menuTextFontSuperscript, "menuTextFontSuperscript");
            this.menuTextFontSuperscript.Click += new System.EventHandler(this.menuTextFontSuperscript_Click);
            // 
            // menuTextFontSubscript
            // 
            this.menuTextFontSubscript.Index = 8;
            resources.ApplyResources(this.menuTextFontSubscript, "menuTextFontSubscript");
            this.menuTextFontSubscript.Click += new System.EventHandler(this.menuTextFontSubscript_Click);
            // 
            // menuTextFontStrikeout
            // 
            this.menuTextFontStrikeout.Index = 9;
            resources.ApplyResources(this.menuTextFontStrikeout, "menuTextFontStrikeout");
            this.menuTextFontStrikeout.Click += new System.EventHandler(this.menuTextFontStrikeout_Click);
            // 
            // menuTextFontSep2
            // 
            this.menuTextFontSep2.Index = 10;
            resources.ApplyResources(this.menuTextFontSep2, "menuTextFontSep2");
            // 
            // menuTextFontIncrease
            // 
            this.menuTextFontIncrease.Index = 11;
            resources.ApplyResources(this.menuTextFontIncrease, "menuTextFontIncrease");
            this.menuTextFontIncrease.Click += new System.EventHandler(this.menuTextFontIncrease_Click);
            // 
            // menuTextFontDecrease
            // 
            this.menuTextFontDecrease.Index = 12;
            resources.ApplyResources(this.menuTextFontDecrease, "menuTextFontDecrease");
            this.menuTextFontDecrease.Click += new System.EventHandler(this.menuTextFontDecrease_Click);
            // 
            // menuTextFontIndent
            // 
            this.menuTextFontIndent.Index = 13;
            resources.ApplyResources(this.menuTextFontIndent, "menuTextFontIndent");
            this.menuTextFontIndent.Click += new System.EventHandler(this.menuTextFontIndent_Click);
            // 
            // menuTextFontOutdent
            // 
            this.menuTextFontOutdent.Index = 14;
            resources.ApplyResources(this.menuTextFontOutdent, "menuTextFontOutdent");
            this.menuTextFontOutdent.Click += new System.EventHandler(this.menuTextFontOutdent_Click);
            // 
            // menuTextFontSep3
            // 
            this.menuTextFontSep3.Index = 15;
            resources.ApplyResources(this.menuTextFontSep3, "menuTextFontSep3");
            // 
            // menuTextFontListOrdered
            // 
            this.menuTextFontListOrdered.Index = 16;
            resources.ApplyResources(this.menuTextFontListOrdered, "menuTextFontListOrdered");
            this.menuTextFontListOrdered.Click += new System.EventHandler(this.menuTextFontListOrdered_Click);
            // 
            // menuTextFontListUnordered
            // 
            this.menuTextFontListUnordered.Index = 17;
            resources.ApplyResources(this.menuTextFontListUnordered, "menuTextFontListUnordered");
            this.menuTextFontListUnordered.Click += new System.EventHandler(this.menuTextFontListUnordered_Click);
            // 
            // menuJustify
            // 
            this.menuJustify.Index = 5;
            this.menuJustify.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuJustifyLeft,
            this.menuJustifyCenter,
            this.menuJustifyRight});
            resources.ApplyResources(this.menuJustify, "menuJustify");
            // 
            // menuJustifyLeft
            // 
            this.menuJustifyLeft.Index = 0;
            resources.ApplyResources(this.menuJustifyLeft, "menuJustifyLeft");
            this.menuJustifyLeft.Click += new System.EventHandler(this.menuJustifyLeft_Click);
            // 
            // menuJustifyCenter
            // 
            this.menuJustifyCenter.Index = 1;
            resources.ApplyResources(this.menuJustifyCenter, "menuJustifyCenter");
            this.menuJustifyCenter.Click += new System.EventHandler(this.menuJustifyCenter_Click);
            // 
            // menuJustifyRight
            // 
            this.menuJustifyRight.Index = 2;
            resources.ApplyResources(this.menuJustifyRight, "menuJustifyRight");
            this.menuJustifyRight.Click += new System.EventHandler(this.menuJustifyRight_Click);
            // 
            // menuFormatting
            // 
            this.menuFormatting.Index = 6;
            resources.ApplyResources(this.menuFormatting, "menuFormatting");
            // 
            // menuMainSep2
            // 
            this.menuMainSep2.Index = 7;
            resources.ApplyResources(this.menuMainSep2, "menuMainSep2");
            // 
            // menuInsert
            // 
            this.menuInsert.Index = 8;
            this.menuInsert.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuInsertLine,
            this.menuInsertLink,
            this.menuInsertImage,
            this.menuInsertText,
            this.menuInsertHtml,
            this.menuInsertTable});
            resources.ApplyResources(this.menuInsert, "menuInsert");
            // 
            // menuInsertLine
            // 
            this.menuInsertLine.Index = 0;
            resources.ApplyResources(this.menuInsertLine, "menuInsertLine");
            this.menuInsertLine.Click += new System.EventHandler(this.menuInsertLine_Click);
            // 
            // menuInsertLink
            // 
            this.menuInsertLink.Index = 1;
            resources.ApplyResources(this.menuInsertLink, "menuInsertLink");
            this.menuInsertLink.Click += new System.EventHandler(this.menuInsertLink_Click);
            // 
            // menuInsertImage
            // 
            this.menuInsertImage.Index = 2;
            resources.ApplyResources(this.menuInsertImage, "menuInsertImage");
            this.menuInsertImage.Click += new System.EventHandler(this.menuInsertImage_Click);
            // 
            // menuInsertText
            // 
            this.menuInsertText.Index = 3;
            resources.ApplyResources(this.menuInsertText, "menuInsertText");
            this.menuInsertText.Click += new System.EventHandler(this.menuInsertText_Click);
            // 
            // menuInsertHtml
            // 
            this.menuInsertHtml.Index = 4;
            resources.ApplyResources(this.menuInsertHtml, "menuInsertHtml");
            this.menuInsertHtml.Click += new System.EventHandler(this.menuInsertHtml_Click);
            // 
            // menuInsertTable
            // 
            this.menuInsertTable.Index = 5;
            resources.ApplyResources(this.menuInsertTable, "menuInsertTable");
            this.menuInsertTable.Click += new System.EventHandler(this.menuInsertTable_Click);
            // 
            // editorWebBrowser
            // 
            resources.ApplyResources(this.editorWebBrowser, "editorWebBrowser");
            this.editorWebBrowser.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("editorWebBrowser.OcxState")));
            this.editorWebBrowser.BeforeNavigate2 += new AxSHDocVw.DWebBrowserEvents2_BeforeNavigate2EventHandler(this.BrowserBeforeNavigate);
            this.editorWebBrowser.DocumentComplete += new AxSHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(this.BrowserDocumentComplete);
            // 
            // HtmlEditorControl
            // 
            this.Controls.Add(this.editorWebBrowser);
            this.Controls.Add(this.editorToolbar);
            this.Name = "HtmlEditorControl";
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)(this.editorWebBrowser)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Control Methods and Events

        // define the context menu for the drop down of available styles
        // defined with a TAG value set to the appropriate format Tag
        private void DefineFormatBlockMenu()
        {
            // define the menu items
            MenuItem _menuFormatNormal = new MenuItem();
            MenuItem _menuFormatH1 = new MenuItem();
            MenuItem _menuFormatH2 = new MenuItem();
            MenuItem _menuFormatH3 = new MenuItem();
            MenuItem _menuFormatH4 = new MenuItem();
            MenuItem _menuFormatH5 = new MenuItem();
            MenuItem _menuFormatPre = new MenuItem();

            // define the Text for the appropriate format block command
            _menuFormatNormal.Text = FORMATTED_NORMAL;
            _menuFormatH1.Text = FORMATTED_HEADING + " 1";
            _menuFormatH2.Text = FORMATTED_HEADING + " 2";
            _menuFormatH3.Text = FORMATTED_HEADING + " 3";
            _menuFormatH4.Text = FORMATTED_HEADING + " 4";
            _menuFormatH5.Text = FORMATTED_HEADING + " 5";
            _menuFormatPre.Text = FORMATTED_PRE;

            // ensure all event handlers point to the same method
            _menuFormatNormal.Click += ProcessFormattingSelection;
            _menuFormatH1.Click += ProcessFormattingSelection;
            _menuFormatH2.Click += ProcessFormattingSelection;
            _menuFormatH3.Click += ProcessFormattingSelection;
            _menuFormatH4.Click += ProcessFormattingSelection;
            _menuFormatH5.Click += ProcessFormattingSelection;
            _menuFormatPre.Click += ProcessFormattingSelection;

            // create the submenu array to be added to the sub items
            MenuItem[] _formattingSubMenu = new[]
                                               {
                                                   _menuFormatNormal,
                                                   _menuFormatH1,
                                                   _menuFormatH2,
                                                   _menuFormatH3,
                                                   _menuFormatH4,
                                                   _menuFormatH5,
                                                   _menuFormatPre
                                               };

            // now have the formatting context menu add to the main context menu
            menuFormatting.MenuItems.AddRange(_formattingSubMenu);
        }

        // COM Event Handler for HTML Element Events
        [DispId(0)]
        public void DefaultMethod()
        {
            // obtain the event object and ensure a context menu has been applied to the document
            HtmlEventObject _eventObject = m_document.parentWindow.@event;
            string _eventType = _eventObject.type;

            if (IsStatedTag(_eventType, EVENT_CONTEXT_MENU))
            {
                // Call the custom Web Browser HTML event 
                ContextMenuShow(this, _eventObject);
            }
        }

        // method to perform the process of showing the context menus
        private void ContextMenuShow(object sender, HtmlEventObject e)
        {
            // if in readonly mode display the standard context menu
            // otherwise display the editing context menu
            if (!m_readOnly)
            {
                // should disable inappropriate commands
                if (IsParentTable())
                {
                    menuTableModify.Visible = true;
                }
                else
                {
                    menuTableModify.Visible = false;
                }

                // display the text processing context menu
                contextMenuMain.Show(this, new Point(e.x, e.y));

                // cancel the standard menu and event bubbling
                e.returnValue = false;
                e.cancelBubble = true;
            }
        }

        // method used to navigate to the required page
        // call made sync using a loading variable
        private void BrowserCodeNavigate(string url)
        {
            // once navigated to the href page wait until successful
            // need to do this to ensure properties are all correctly set
            m_codeNavigate = true;
            m_loading = true;

            // perform the navigation
            editorWebBrowser.Navigate(url, ref EMPTY_PARAMETER, ref EMPTY_PARAMETER, ref EMPTY_PARAMETER, ref EMPTY_PARAMETER);

            // wait for the navigate to complete using the loading variable
            // DoEvents needs to be called to enable the DocumentComplete to execute
            while (m_loading)
            {
                Application.DoEvents();
                Thread.Sleep(0);
            }
        }

        // this event can be used to canel the navigation and open a new window
        // if window set to same then nothing happens
        private void BrowserBeforeNavigate(object sender, DWebBrowserEvents2_BeforeNavigate2Event e)
        {
            string _url = e.uRL.ToString();

            if (m_codeNavigate)
            {
                // TODO_ Should ensure the following are no executed for the editor navigation
                //   Scripts
                //   Java
                //   ActiveX Controls
                //   Behaviors
                //   Dialogs

                // continue with current navigation
                e.cancel = false;
            }
            else
            {
                // call the appropriate event processing
                HtmlNavigationEventArgs _navigateArgs = new HtmlNavigationEventArgs(_url);
                OnHtmlNavigation(_navigateArgs);

                // process the event based on the navigation option
                if (_navigateArgs.Cancel)
                {
                    // cancel the navigation
                    e.cancel = true;
                }
                else if (m_navigateWindow == NavigateActionOption.NewWindow)
                {
                    // cancel the current navigation and load url into a new window
                    e.cancel = true;
                    NavigateToUrl(_url, true);
                }
                else
                {
                    // continue with current navigation
                    e.cancel = false;
                }
            }
        }

        //processing for the HtmlNavigation event
        private void OnHtmlNavigation(HtmlNavigationEventArgs args)
        {
            if (HtmlNavigation != null)
            {
                HtmlNavigation(this, args);
            }
        }

        // Document complete method for the web browser
        // initiated by navigating to the about:blank page (EMPTY_PARAMETER HTML document)
        private void BrowserDocumentComplete(object sender, DWebBrowserEvents2_DocumentCompleteEvent e)
        {
            // get access to the HTMLDocument
            m_document = (HtmlDocument)editorWebBrowser.Document;
            m_body = (HtmlBody)m_document.body;


            //----- COM Interop Start
            // once browsing has completed there is the need to setup some options
            // need to ensure URLs are not modified when html is pasted
            IOleCommandTarget _target = null;
            int _hResult = HRESULT.S_OK;

            // try to obtain the command target for the web browser document
            try
            {
                // cast the document to a command target
                _target = (IOleCommandTarget)m_document;

                // set the appropriate no url fixups on paste
                _hResult = _target.Exec(ref CommandGroup.CGID_MSHTML, (int)CommandId.IDM_NOFIXUPURLSONPASTE,
                                      (int)CommandOption.OLECMDEXECOPT_DONTPROMPTUSER, ref EMPTY_PARAMETER,
                                      ref EMPTY_PARAMETER);
            }
            // catch any exception and map back to the HRESULT
            catch (Exception ex)
            {
                _hResult = Marshal.GetHRForException(ex);
            }

            // test the HRESULT for a valid operation
            if (_hResult == HRESULT.S_OK)
            {
                // urls will not automatically be rebased
                m_rebaseUrlsNeeded = false;
            }
            else
            {
                //throw new HtmlEditorException(string.Format("Error executing NOFIXUPURLSONPASTE: Result {0}", hResult));
                m_rebaseUrlsNeeded = true;
            }
            //----- COM Interop End


            // at this point the document and body has been loaded
            // so define the event handler as the same class
            ((DispHTMLDocument)m_document).oncontextmenu = this;

            // signalled complete
            m_codeNavigate = false;
            m_loading = false;

            // after navigation define the document Url
            string _url = e.uRL.ToString();
            m_bodyUrl = IsStatedTag(_url, BLANK_HTML_PAGE) ? string.Empty : _url;

            ((HTMLBody)m_document.body).contentEditable = "true"; //@ Ren
        }

        // create a new focus method that ensure the body gets the focus
        // should be called when text processing command are called
        public new bool Focus()
        {
            // have the return value be the focus return from the user control
            bool _focus = base.Focus();

            // try to set the focus to the web browser
            try
            {
                editorWebBrowser.Focus();

                if (m_body != null)
                    m_body.focus();
            }
            catch
            {
            }

            return _focus;
        }

        #endregion

        #region Runtime Display Properties

        // defines the whether scroll bars should be displayed
        [Category("RuntimeDisplay"), Description("Controls the Display of Scrolls Bars")]
        [DefaultValue(DisplayScrollBarOption.Auto)]
        public DisplayScrollBarOption ScrollBars
        {
            get { return m_scrollBars; }
            set
            {
                m_scrollBars = value;

                // define the document scroll bar visibility
                m_body.scroll = m_scrollBars.ToString();

                // define the menu bar state
                menuDocumentScrollbar.Checked = (value != DisplayScrollBarOption.No);
            }
        }

        // defines the whether words will be auto wrapped
        [Category("RuntimeDisplay"), Description("Controls the auto wrapping of content")]
        [DefaultValue(true)]
        public bool AutoWordWrap
        {
            get { return m_autoWordWrap; }
            set
            {
                m_autoWordWrap = value;

                // define the document word wrap property
                m_body.noWrap = !m_autoWordWrap;

                // define the menu bar state
                menuDocumentWordwrap.Checked = value;
            }
        }

        // defines the default action when a user click on a link
        [Category("RuntimeDisplay"), Description("Window to use when clicking a Href")]
        [DefaultValue(NavigateActionOption.Default)]
        public NavigateActionOption NavigateAction
        {
            get { return m_navigateWindow; }
            set { m_navigateWindow = value; }
        }

        // Defines the editable status of the text
        [Category("RuntimeDisplay"), Description("Marks the content as ReadOnly")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return m_readOnly; }
            set
            {
                m_readOnly = value;

                // define the document editable property
                m_body.contentEditable = (!m_readOnly).ToString();

                // define the menu bar state
                editorToolbar.Enabled = (!m_readOnly);
            }
        }

        // defines the visibility of the defined toolbar
        [Category("RuntimeDisplay"), Description("Marks the toolbar as Visible")]
        [DefaultValue(false)]
        public bool ToolbarVisible
        {
            get { return m_toolbarVisible; }
            set
            {
                m_toolbarVisible = value;
                editorToolbar.Visible = m_toolbarVisible;
                menuDocumentToolbar.Checked = value;
            }
        }

        // defines the flat style of controls for visual styles
        [Category("RuntimeDisplay"),
         Description("Indicates if the Control Flat Style is set to System or Standard for all dialogs")]
        [DefaultValue(false)]
        public bool EnableVisualStyles
        {
            get { return m_enableVisualStyles; }
            set { m_enableVisualStyles = value; }
        }

        // defines the visibility of the defined toolbar
        [Category("RuntimeDisplay"), Description("Defines the docking location of the toolbar")]
        [DefaultValue(DockStyle.Bottom)]
        public DockStyle ToolbarDock
        {
            get { return m_toolbarDock; }
            set
            {
                if (value != DockStyle.Fill)
                {
                    m_toolbarDock = value;
                    editorToolbar.Dock = m_toolbarDock;

                    // ensure control is repainted as docking has been modified
                    Invalidate();
                }
            }
        }

        #endregion

        #region Body Properties (Text and HTML)

        // defines the base text for the body (design time only value)
        // HTML value can be used at runtime
        [Category("Textual"), Description("Set the initial Body Text")]
        [DefaultValue(DEFAULT_HTML_TEXT)]
        public string InnerText
        {
            get
            {
                m_bodyText = m_body.innerText;
                m_bodyHtml = m_body.innerHTML;
                return m_bodyText;
            }
            set
            {
                try
                {
                    // clear the defined body url
                    m_bodyUrl = string.Empty;

                    if (value == null)
                        value = string.Empty;

                    // set the body property
                    m_body.innerText = value;

                    // set the body text and html
                    m_bodyText = m_body.innerText;
                    m_bodyHtml = m_body.innerHTML;
                }
                catch (Exception _e)
                {
                    throw new HtmlEditorException("Inner Text for the body cannot be set.", "SetInnerText", _e);
                }
            }
        }

        // the HTML value for the body contents
        // it is this value that gets serialized by the designer
        [Category("Textual"), Description("The Inner HTML of the contents")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public string InnerHtml
        {
            get
            {
                m_bodyText = m_body.innerText;
                m_bodyHtml = m_body.innerHTML;
                return m_bodyHtml;
            }
            set
            {
                try
                {
                    // clear the defined body url
                    m_bodyUrl = string.Empty;

                    if (value == null)
                        value = string.Empty;

                    // set the body property
                    m_body.innerHTML = value;

                    // set the body text and html
                    m_bodyText = m_body.innerText;
                    m_bodyHtml = m_body.innerHTML;

                    // if needed rebase urls
                    RebaseAnchorUrl();
                }
                catch (Exception _e)
                {
                    throw new HtmlEditorException("Inner Html for the body cannot be set.", "SetInnerHtml", _e);
                }
            }
        }

        // returns and sets the body tag of the html
        // on set the body attributes need to be defined
        [Category("Textual"), Description("Complete Document including Body Tag")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public string BodyHtml
        {
            get
            {
                // set the read only property before return
                m_body.contentEditable = CONTENT_EDITABLE_INHERIT;
                string _html = m_body.outerHTML.Trim();
                ReadOnly = m_readOnly;
                return _html;
            }
            set
            {
                // clear the defined body url
                m_bodyUrl = string.Empty;

                // define some local working variables
                string _bodyElement = string.Empty;
                string _innerHtml = string.Empty;

                try
                {
                    // ensure have body open and close tags
                    if (Regex.IsMatch(value, BODY_PARSE_PRE_EXPRESSION, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline))
                    {
                        // define a regular expression for the Html Body parsing and obtain the match expression
                        Regex _expression = new Regex(BODY_PARSE_EXPRESSION,
                                                     RegexOptions.IgnoreCase | RegexOptions.Multiline |
                                                     RegexOptions.Singleline | RegexOptions.Compiled |
                                                     RegexOptions.ExplicitCapture);
                        Match _match = _expression.Match(value);

                        // see if a match was found
                        if (_match.Success)
                        {
                            // extract the body tag and the inner html
                            _bodyElement = _match.Result(BODY_TAG_PARSE_MATCH);
                            _innerHtml = _match.Result(BODY_INNER_PARSE_MATCH);

                            // remove whitespaces from the body and inner html tags
                            _bodyElement = _bodyElement.Trim();
                            _innerHtml = _innerHtml.Trim();
                        }
                    }

                    // ensure body was set
                    if (_bodyElement == string.Empty)
                    {
                        // assume the Html given is an inner html with no body
                        _bodyElement = BODY_DEFAULT_TAG;
                        _innerHtml = value.Trim();
                    }

                    // first navigate to a blank page to reset the html header
                    BrowserCodeNavigate(BLANK_HTML_PAGE);

                    // replace the body tag with the one passed in
                    HtmlDomNode _oldBodyNode = (HtmlDomNode)m_document.body;
                    HtmlDomNode _newBodyNode = (HtmlDomNode)m_document.createElement(_bodyElement);
                    _oldBodyNode.replaceNode(_newBodyNode);

                    // define the new inner html and body objects
                    m_body = (HtmlBody)m_document.body;
                    m_body.innerHTML = _innerHtml;

                    // now all successfully loaded need to review the body attributes
                    m_bodyText = m_body.innerText;
                    m_bodyHtml = m_body.innerHTML;

                    // set and define the appropriate properties
                    // this will set the appropriate read only property
                    DefineBodyAttributes();

                    // if needed rebase urls
                    RebaseAnchorUrl();
                }
                catch (Exception _e)
                {
                    throw new HtmlEditorException("Outer Html for the body cannot be set.", "SetBodyHtml", _e);
                }
            }
        }

        // return the html tag of the document
        // should never be set as contains the HEAD tag
        [Category("Textual"), Description("Complete Document including Head and Body Tag")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public string DocumentHtml
        {
            get { return m_document.documentElement.outerHTML.Trim(); }
        }

        // returns or sets the Text selected by the user
        [Category("Textual"), Description("The Text selected by the User")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public string SelectedText
        {
            get
            {
                // obtain the selected range object
                HtmlTextRange _range = GetTextRange();

                // return the text of the range
                if (_range.text != null)
                    return _range.text;
                else
                    return string.Empty;
            }
            set
            {
                try
                {
                    // obtain the selected range object
                    HtmlTextRange _range = GetTextRange();

                    // set the text range
                    if (_range != null)
                    {
                        // encode the text to encode any html markup
                        string _html = HttpUtility.HtmlEncode(value);

                        // once have a range execute the pasteHtml command
                        // this will overwrite the current selection
                        _range.pasteHTML(_html);
                    }
                }
                catch (Exception _e)
                {
                    throw new HtmlEditorException("Inner Text for the selection cannot be set.", "SetSelectedText", _e);
                }
            }
        }

        // returns or sets the Html selected by the user
        [Category("Textual"), Description("The Text selected by the User")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public string SelectedHtml
        {
            get
            {
                // obtain the selected range object
                HtmlTextRange _range = GetTextRange();

                // return the text of the range
                if (_range.text != null)
                    return _range.htmlText;
                else
                    return string.Empty;
            }
            set
            {
                try
                {
                    // obtain the selected range object
                    HtmlTextRange _range = GetTextRange();

                    // set the text range
                    if (_range != null)
                    {
                        // once have a range execute the pasteHtml command
                        // this will overwrite the current selection
                        _range.pasteHTML(value);

                        // if needed rebase urls
                        RebaseAnchorUrl();
                    }
                }
                catch (Exception _e)
                {
                    throw new HtmlEditorException("Inner Html for the selection cannot be set.", "SetSelectedHtml", _e);
                }
            }
        }

        // returns any Url that was used to load the current document
        [Category("Textual"), Description("Url used to load the Document")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public string DocumentUrl
        {
            get
            {
                //return document.baseUrl;
                return m_bodyUrl;
            }
        }

        #endregion

        #region Body Properties (Font and Color)

        // body background color
        // reset and serialize values defined
        [Category("Textual"), Description("Defines the Background Color of the Body")]
        public Color BodyBackColor
        {
            get { return m_bodyBackColor; }
            set
            {
                // set the new value using a default if Empty passed in
                if (value != Color.Empty)
                    m_bodyBackColor = value;
                else
                    m_bodyBackColor = m_defaultBackColor;

                // set the bgcolor attribute of the body
                if (m_body != null)
                {
                    if (m_bodyBackColor.ToArgb() == m_defaultBackColor.ToArgb())
                    {
                        m_document.bgColor = string.Empty;
                    }
                    else
                    {
                        m_document.bgColor = ColorTranslator.ToHtml(m_bodyBackColor);
                    }
                }
            }
        }

        public bool ShouldSerializeBodyBackColor()
        {
            return (m_bodyBackColor != m_defaultBackColor);
        }

        public void ResetBodyBackColor()
        {
            BodyBackColor = m_defaultBackColor;
        }

        // body foreground color
        // reset and serialize values defined
        [Category("Textual"), Description("Defines the Foreground Color of the Body")]
        public Color BodyForeColor
        {
            get { return m_bodyForeColor; }
            set
            {
                // set the new value using a default if Empty passed in
                if (value != Color.Empty)
                    m_bodyForeColor = value;
                else
                    m_bodyForeColor = m_defaultForeColor;

                // set the text attribute of the body
                if (m_body != null)
                {
                    if (m_bodyForeColor.ToArgb() == m_defaultForeColor.ToArgb())
                    {
                        m_document.fgColor = string.Empty;
                    }
                    else
                    {
                        m_document.fgColor = ColorTranslator.ToHtml(m_bodyForeColor);
                    }
                }
            }
        }

        public bool ShouldSerializeBodyForeColor()
        {
            return (m_bodyForeColor != m_defaultForeColor);
        }

        public void ResetBodyForeColor()
        {
            BodyForeColor = m_defaultForeColor;
        }

        // base font to use for text editing
        // can be overriden in code or through Font property
        [Category("Textual"), Description("Defines the base Font object for the Body")]
        [RefreshProperties(RefreshProperties.All)]
        public HtmlFontProperty BodyFont
        {
            get { return m_bodyFont; }
            set
            {
                // set the new value using the default if set to null
                if (HtmlFontProperty.IsNotNull(value))
                    m_bodyFont = value;
                else
                    m_bodyFont = m_defaultFont;

                // set the font attributes based on any body styles
                HtmlStyle _bodyStyle = m_body.style;

                if (_bodyStyle != null)
                {
                    if (HtmlFontProperty.IsEqual(m_bodyFont, m_defaultFont))
                    {
                        // ensure no values are set in the Body style
                        if (_bodyStyle.fontFamily != null)
                            _bodyStyle.fontFamily = string.Empty;

                        if (_bodyStyle.fontSize != null)
                            _bodyStyle.fontSize = string.Empty;

                        if (_bodyStyle.fontWeight != null)
                            _bodyStyle.fontWeight = string.Empty;
                    }
                    else
                    {
                        // set the body styles based on the defined value
                        _bodyStyle.fontFamily = m_bodyFont.Name;
                        _bodyStyle.fontSize = HtmlFontConversion.HtmlFontSizeString(m_bodyFont.Size);
                        _bodyStyle.fontWeight = HtmlFontConversion.HtmlFontBoldString(m_bodyFont.Bold);
                    }
                }
            }
        }

        public bool ShouldSerializeBodyFont()
        {
            return (HtmlFontProperty.IsNotEqual(m_bodyFont, m_defaultFont));
        }

        public void ResetBodyFont()
        {
            BodyFont = m_defaultFont;
        }

        #endregion

        #region Document Processing Operations

        // allow the user to load a document by navigation
        public void NavigateToUrl(string url)
        {
            // load the requested use Url
            BrowserCodeNavigate(url);

            // now all successfully laoded need to review the body attributes
            m_bodyText = m_body.innerText;
            m_bodyHtml = m_body.innerHTML;

            // set and define the appropriate properties
            DefineBodyAttributes();
        }

        // allow the user to load a document into the specified window
        public void NavigateToUrl(string url, bool newWindow)
        {
            if (newWindow)
            {
                // open the Url in a new window
                object _window = TARGET_WINDOW_NEW;
                editorWebBrowser.Navigate(url, ref EMPTY_PARAMETER, ref _window, ref EMPTY_PARAMETER, ref EMPTY_PARAMETER);
            }
            else
            {
                // if no new window required call the normal navigate method
                NavigateToUrl(url);
            }
        }

        // allow the user to load a document from a Url
        // the body tag is used and loaded
        public void LoadFromUrl(string url)
        {
            HttpWebRequest _webRqst = null;
            HttpWebResponse _webResp = null;
            Stream _stream = null;

            // load the body at the given url into the html editor
            try
            {
                // Creates an HttpWebRequest with the specified URL. 
                _webRqst = (HttpWebRequest)WebRequest.Create(url);

                // setup default credentials since may be in an intranet
                _webRqst.Credentials = CredentialCache.DefaultCredentials;

                // send the HttpWebRequest and waits for the response.            
                _webResp = (HttpWebResponse)_webRqst.GetResponse();

                // parse the content type to determine the maintype and subtype
                string _contenttype = _webResp.ContentType;
                string _maintype = string.Empty;
                string _subtype = string.Empty;
                string _charset = string.Empty;
                Regex _expression = new Regex(CONTENTTYPE_PARSE_EXPRESSION,
                                             RegexOptions.IgnoreCase | RegexOptions.Compiled |
                                             RegexOptions.ExplicitCapture);
                Match match = _expression.Match(_contenttype);

                // see if a match was found
                if (match.Success)
                {
                    // extract the content type elements
                    _maintype = match.Result(CONTENTTYPE_PARSE_MAINTYPE);
                    _subtype = match.Result(CONTENTTYPE_PARSE_SUBTYPE);
                    _charset = match.Result(CONTENTTYPE_PARSE_CHARSET);
                }

                // retrieves the text if the content type is of text/html.
                if (IsStatedTag(_maintype, "text") && IsStatedTag(_subtype, "html"))
                {
                    // define the encoder to use
                    Encoding _encoder;

                    if (_charset == string.Empty)
                        _charset = @"utf-8";

                    try
                    {
                        // base the encoder from the charset calculated
                        _encoder = Encoding.GetEncoding(_charset);
                    }
                    catch (Exception)
                    {
                        // use a default UTF8 encoder
                        _encoder = Encoding.UTF8;
                    }

                    // read the response stream
                    StringBuilder _html = new StringBuilder();

                    // read the response buffer and return the string representation
                    _stream = _webResp.GetResponseStream();
                    byte[] _outputBuffer = new byte[HTML_BUFFER_SIZE];

                    // read the stream in buffer size blocks
                    int _bytesRead = 0;

                    do
                    {
                        _bytesRead = _stream.Read(_outputBuffer, 0, HTML_BUFFER_SIZE);

                        if (_bytesRead > 0)
                        {
                            _html.Append(_encoder.GetString(_outputBuffer, 0, _bytesRead));
                        }
                    } while (_bytesRead > 0);

                    // populate the string value from the text
                    BodyHtml = _html.ToString();
                }
                else
                {
                    // navigated to a none Html document so throw exception
                    throw new HtmlEditorException(string.Format("Not a Html Document: {0}", url), "LoadFromUrl");
                }
            }
            catch (HtmlEditorException _e)
            {
                // cannot load so throw an exception
                throw _e;
            }
            catch (WebException _e)
            {
                // cannot load so throw an exception
                throw new HtmlEditorException(string.Format("Cannot load Url: {0}", url), "LoadFromUrl", _e);
            }
            catch (Exception _e)
            {
                // cannot load so throw an exception
                throw new HtmlEditorException(string.Format("Cannot load Document: {0}", url), "LoadFromUrl", _e);
            }
            finally
            {
                // close the stream reader
                if (_stream != null)
                    _stream.Close();

                // close the web response
                if (_webResp != null)
                    _webResp.Close();
            }
        }

        // allow a user to load a file given a file name
        public void LoadFromFile(string filename)
        {
            // init the container for the Html
            string _contents = string.Empty;

            // check to see if the file exists
            if (!File.Exists(filename))
            {
                throw new HtmlEditorException("Not a valid file name.", "LoadFromFile");
            }

            // read the file contents
            using (StreamReader _reader = new StreamReader(filename, Encoding.UTF8))
            {
                _contents = _reader.ReadToEnd();
            }

            // if the contents where successfully read write to document
            if (_contents != String.Empty)
            {
                BodyHtml = _contents;
            }
        }

        // allow the user to select a file and read the contents into the Html stream
        public void OpenFilePrompt()
        {
            // init the container for the Html
            string _contents = string.Empty;

            // create an open file dialog
            using (OpenFileDialog _dialog = new OpenFileDialog())
            {
                // init the input stream
                Stream _stream = null;

                // define the dialog structure
                _dialog.DefaultExt = HTML_EXTENSION;
                _dialog.Title = HTML_TITLE_OPENFILE;
                _dialog.AddExtension = true;
                _dialog.Filter = HTML_FILTER;
                _dialog.FilterIndex = 1;
                _dialog.RestoreDirectory = true;

                if (m_htmlDirectory != String.Empty)
                    _dialog.InitialDirectory = m_htmlDirectory;

                // show the dialog and see if the users enters OK
                if (_dialog.ShowDialog() == DialogResult.OK)
                {
                    // look to see if the file contains any contents and stream
                    if ((_stream = _dialog.OpenFile()) != null)
                    {
                        using (StreamReader _reader = new StreamReader(_stream, Encoding.UTF8))
                        {
                            // get the contents as text
                            _contents = _reader.ReadToEnd();

                            // persist the path navigation
                            m_htmlDirectory = Path.GetDirectoryName(_dialog.FileName);
                        }
                    }

                    // close the input stream
                    if (_stream != null)
                        _stream.Close();
                }
            }

            // if the contents where successfully read write to document
            if (_contents != String.Empty)
            {
                BodyHtml = _contents;
            }
        }


        // allow the user to persist the Html stream to a file
        public void SaveFilePrompt(bool all)
        {
            // obtain the html contents
            string _contents = BodyHtml;

            if (all)
                _contents = DocumentHtml;

            // create a file save dialog
            using (SaveFileDialog _dialog = new SaveFileDialog())
            {
                // init the outpu stream
                Stream _stream = null;

                // define the dialog structure
                _dialog.DefaultExt = HTML_EXTENSION;
                _dialog.Title = HTML_TITLE_SAVEFILE;
                _dialog.AddExtension = true;
                _dialog.Filter = HTML_FILTER;
                _dialog.FilterIndex = 1;
                _dialog.RestoreDirectory = true;

                if (m_htmlDirectory != String.Empty)
                    _dialog.InitialDirectory = m_htmlDirectory;

                // show the dialog and see if the users enters OK
                if (_dialog.ShowDialog() == DialogResult.OK)
                {
                    // look to see if the stream can be open for output
                    if ((_stream = _dialog.OpenFile()) != null)
                    {
                        // create the stream writer for the html
                        using (StreamWriter _writer = new StreamWriter(_stream))
                        {
                            // write out the body contents to the stream
                            _writer.Write(_contents);
                            _writer.Flush();

                            // persist the path navigation
                            m_htmlDirectory = Path.GetDirectoryName(_dialog.FileName);
                        }
                    }

                    // close the input stream
                    if (_stream != null)
                        _stream.Close();
                }
            }
        }

        // define the style sheet to be used for editing
        // can be used for standard templates
        public void LinkStyleSheet(string stylesheetHref)
        {
            if (m_stylesheet != null)
            {
                // if (document.styleSheets.length > 0)
                // Create an IEnumerator for the cells of the row
                // IEnumerator enumerator = document.styleSheets.GetEnumerator();
                // enumerator.MoveNext();
                // stylesheet = (HtmlStyleSheet)enumerator.Current;
                m_stylesheet.href = stylesheetHref;
            }
            else
            {
                m_stylesheet = m_document.createStyleSheet(stylesheetHref, 0);
            }
        }

        // return to the user the style sheet href being used
        public string GetStyleSheetHref()
        {
            if (m_stylesheet != null)
                return m_stylesheet.href;
            else
                return string.Empty;
        }

        // define a script element that is to be used by all documents
        // can be sued for document processing
        public void LinkScriptSource(string scriptSource)
        {
            if (m_script != null)
            {
                m_script.src = scriptSource;
            }
            else
            {
                // create the script element
                m_script = (HtmlScriptElement)m_document.createElement(SCRIPT_TAG);
                m_script.src = scriptSource;
                m_script.defer = true;

                // insert into the document
                HtmlDomNode _node = (HtmlDomNode)m_document.documentElement;
                _node.appendChild((HtmlDomNode)m_script);
            }
        }

        // return to the user the script block source being used
        public string GetScriptBlockSource()
        {
            if (m_script != null)
                return m_script.src;
            else
                return string.Empty;
        }

        // allow the user to edit the raw HTML
        // dialog presented and the body contents set
        public void HtmlContentsEdit()
        {
            using (EditHtmlForm _dialog = new EditHtmlForm())
            {
                _dialog.HTML = InnerHtml;
                _dialog.ReadOnly = false;
                _dialog.SetCaption(HTML_TITLE_EDIT);
                DefineDialogProperties(_dialog);

                if (_dialog.ShowDialog(ParentForm) == DialogResult.OK)
                {
                    InnerHtml = _dialog.HTML;
                }
            }
        }

        // allow the user to view the html contents
        // the complete Html markup is presented
        public void HtmlContentsView()
        {
            using (EditHtmlForm _dialog = new EditHtmlForm())
            {
                _dialog.HTML = DocumentHtml;
                _dialog.ReadOnly = true;
                _dialog.SetCaption(HTML_TITLE_VIEW);
                DefineDialogProperties(_dialog);
                _dialog.ShowDialog(ParentForm);
            }
        }

        // print the html text using the document print command
        // print preview is not supported
        public void DocumentPrint()
        {
            ExecuteCommandDocument(HTML_COMMAND_TEXT_PRINT);
        }

        // toggle the overwrite mode
        public void ToggleOverWrite()
        {
            ExecuteCommandDocument(HTML_COMMAND_OVERWRITE);
        }

        #endregion

        #region Document Text Operations

        // cut the currently selected text to the clipboard
        public void TextCut()
        {
            ExecuteCommandDocument(HTML_COMMAND_TEXT_CUT);
        }

        // copy the currently selected text to the clipboard
        public void TextCopy()
        {
            ExecuteCommandDocument(HTML_COMMAND_TEXT_COPY);
        }

        // paste the currently selected text from the clipboard
        public void TextPaste()
        {
            ExecuteCommandDocument(HTML_COMMAND_TEXT_PASTE);
        }

        // delete the currently selected text from the screen
        public void TextDelete()
        {
            ExecuteCommandDocument(HTML_COMMAND_TEXT_DELETE);
        }

        // select the entire document contents
        public void TextSelectAll()
        {
            ExecuteCommandDocument(HTML_COMMAND_TEXT_SELECT_ALL);
        }

        // clear the document selection
        public void TextClearSelect()
        {
            ExecuteCommandDocument(HTML_COMMAND_TEXT_UNSELECT);
        }

        // undo former commands
        public void EditUndo()
        {
            ExecuteCommandDocument(HTML_COMMAND_TEXT_UNDO);
        }

        // redo former undo
        public void EditRedo()
        {
            ExecuteCommandDocument(HTML_COMMAND_TEXT_REDO);
        }

        #endregion

        #region Selected Text Formatting Operations

        // using the document set the font name
        public void FormatFontName(string name)
        {
            ExecuteCommandRange(HTML_COMMAND_FONT_NAME, name);
        }

        // using the document set the Html font size
        public void FormatFontSize(HtmlFontSize size)
        {
            ExecuteCommandRange(HTML_COMMAND_FONT_SIZE, (int)size);
        }

        // using the document toggles the selection with a bold tag
        public void FormatBold()
        {
            ExecuteCommandRange(HTML_COMMAND_BOLD, null);
        }

        // using the document toggles the selection with a underline tag
        public void FormatUnderline()
        {
            ExecuteCommandRange(HTML_COMMAND_UNDERLINE, null);
        }

        // using the document toggles the selection with a italic tag
        public void FormatItalic()
        {
            ExecuteCommandRange(HTML_COMMAND_ITALIC, null);
        }

        // using the document toggles the selection with a Subscript tag
        public void FormatSubscript()
        {
            ExecuteCommandRange(HTML_COMMAND_SUBSCRIPT, null);
        }

        // using the document toggles the selection with a Superscript tag
        public void FormatSuperscript()
        {
            ExecuteCommandRange(HTML_COMMAND_SUPERSCRIPT, null);
        }

        // using the document toggles the selection with a Strikeout tag
        public void FormatStrikeout()
        {
            ExecuteCommandRange(HTML_COMMAND_STRIKE_THROUGH, null);
        }

        // increase the size of the font by 1 point
        public void FormatFontIncrease()
        {
            FormatFontChange(1);
        }

        // decrease the size of the font by 1 point
        public void FormatFontDecrease()
        {
            FormatFontChange(-1);
        }

        // given a int value increase the font size by that amount
        private void FormatFontChange(int change)
        {
            // ensure the command is acceptable
            if (Math.Abs(change) > 6)
            {
                throw new HtmlEditorException("Value can only be in the range [1,6].", "FontIncreaseDecrease");
            }
            else
            {
                // obtain the selected range object
                HtmlTextRange _range = GetTextRange();

                // obtain the original font value
                object _fontSize = QueryCommandRange(_range, HTML_COMMAND_FONT_SIZE);
                int _oldFontSize = (_fontSize is Int32) ? (int)_fontSize : (int)m_bodyFont.Size;

                // calc the new font size and modify
                int _newFontSize = _oldFontSize + change;
                ExecuteCommandRange(_range, HTML_COMMAND_FONT_SIZE, _newFontSize);
            }
        }

        // remove any formatting tags
        public void FormatRemove()
        {
            ExecuteCommandRange(HTML_COMMAND_REMOVE_FORMAT, null);
        }

        // Tab the current line to the right
        public void FormatTabRight()
        {
            ExecuteCommandRange(HTML_COMMAND_INDENT, null);
        }

        // Tab the current line to the left
        public void FormatTabLeft()
        {
            ExecuteCommandRange(HTML_COMMAND_OUTDENT, null);
        }

        // insert a ordered or unordered list
        public void FormatList(HtmlListType listtype)
        {
            string _command = string.Format(HTML_COMMAND_INSERT_LIST, listtype.ToString());
            ExecuteCommandRange(_command, null);
        }

        // define the font justification as LEFT
        public void JustifyLeft()
        {
            ExecuteCommandRange(HTML_COMMAND_JUSTIFY_LEFT, null);
        }

        // define the font justification as CENTER
        public void JustifyCenter()
        {
            ExecuteCommandRange(HTML_COMMAND_JUSTIFY_CENTER, null);
        }

        // define the font justification as Right
        public void JustifyRight()
        {
            ExecuteCommandRange(HTML_COMMAND_JUSTIFY_RIGHT, null);
        }

        #endregion

        #region Object Insert Operations

        // insert a horizontal line in the body
        // if have a control range rather than text range one could overwrite the controls with the line
        public void InsertLine()
        {
            HtmlTextRange _range = GetTextRange();

            if (_range != null)
            {
                ExecuteCommandRange(_range, HTML_COMMAND_INSERT_LINE, null);
            }
            else
            {
                throw new HtmlEditorException("Invalid Selection for Line insertion.", "InsertLine");
            }
        }

        // insert an image tag at the selected location
        public void InsertImage(string imageLocation)
        {
            ExecuteCommandRange(HTML_COMMAND_INSERT_IMAGE, imageLocation);
        }

        // public function to insert a image and prompt a user for the link
        // calls the public InsertImage method
        public void InsertImagePrompt()
        {
            // set default image and text tags
            string _imageText = string.Empty;
            string _imageHref = string.Empty;
            ImageAlignOption _imageAlign = ImageAlignOption.Left;
            HtmlElement _control = null;

            // look to see if an image has been selected
            _control = GetFirstControl();

            if (_control != null)
            {
                if (IsStatedTag(_control.tagName, IMAGE_TAG))
                {
                    HtmlImageElement _image = (HtmlImageElement)_control;
                    _imageHref = _image.href;
                    _imageText = _image.alt;

                    if (_image.align != null)
                        _imageAlign = (ImageAlignOption)TryParseEnum(typeof(ImageAlignOption), _image.align, ImageAlignOption.Left);
                }
                else
                {
                    throw new HtmlEditorException("Can only Insert an Image over a previous Image.", "InsertImage");
                }
            }

            // Obtain the image file name
            // prompt the user for the new href
            using (EnterImageForm _dialog = new EnterImageForm())
            {
                // set the dialog properties
                _dialog.ImageLink = _imageHref;
                _dialog.ImageText = _imageText;
                _dialog.ImageAlign = _imageAlign;

                DefineDialogProperties(_dialog);

                // based on the user interaction perform the neccessary action
                // after one has a valid image href
                if (_dialog.ShowDialog(ParentForm) == DialogResult.OK)
                {
                    _imageHref = _dialog.ImageLink;
                    _imageText = _dialog.ImageText;
                    _imageAlign = _dialog.ImageAlign;

                    if (_imageHref != string.Empty)
                    {
                        ExecuteCommandRange(HTML_COMMAND_INSERT_IMAGE, _imageHref);
                        _control = GetFirstControl();

                        if (_control != null)
                        {
                            if (_imageText == string.Empty)
                                _imageText = _imageHref;

                            if (IsStatedTag(_control.tagName, IMAGE_TAG))
                            {
                                HtmlImageElement _image = (HtmlImageElement)_control;
                                _image.alt = _imageText;

                                if (_imageAlign != ImageAlignOption.Left)
                                    _image.align = _imageAlign.ToString().ToLower();
                            }
                        }
                    }
                }
            }
        }

        // create a web link from the users selected text
        public void InsertLink(string href)
        {
            ExecuteCommandRange(HTML_COMMAND_INSERT_LINK, href);
        }

        // public function to insert a link and prompt a user for the href
        // calls the public InsertLink method
        public void InsertLinkPrompt()
        {
            // get the text range working with
            HtmlTextRange _range = GetTextRange();
            string _hrefText = (_range == null) ? null : _range.text;
            string _hrefLink = string.Empty;
            NavigateActionOption _target;

            // ensure have text in the range otherwise nothing works
            if (_hrefText != null)
            {
                // calculate the items working with
                HtmlAnchorElement _anchor = null;
                HtmlElement _element = _range.parentElement();

                // parse up the tree until the anchor element is found
                while (_element != null && !(_element is HtmlAnchorElement))
                {
                    _element = _element.parentElement;
                }

                // extract the HREF properties
                if (_element is HtmlAnchorElement)
                {
                    _anchor = (HtmlAnchorElement)_element;

                    if (_anchor.href != null)
                        _hrefLink = _anchor.href;
                }

                // if text is a valid href then set the link
                if (_hrefLink == string.Empty && IsValidHref(_hrefText))
                {
                    _hrefLink = _hrefText;
                }

                // prompt the user for the new href
                using (EnterHrefForm _dialog = new EnterHrefForm())
                {
                    _dialog.HrefText = _hrefText;
                    _dialog.HrefLink = _hrefLink;
                    DefineDialogProperties(_dialog);

                    DialogResult _result = _dialog.ShowDialog(ParentForm);

                    // based on the user interaction perform the neccessary action
                    // after one has a valid href
                    if (_result == DialogResult.Yes)
                    {
                        _hrefLink = _dialog.HrefLink;
                        _target = _dialog.HrefTarget;

                        if (IsValidHref(_hrefLink))
                        {
                            // insert or update the current link
                            if (_anchor == null)
                            {
                                ExecuteCommandRange(_range, HTML_COMMAND_INSERT_LINK, _hrefLink);
                                _element = _range.parentElement();

                                // parse up the tree until the anchor element is found
                                while (_element != null && !(_element is HtmlAnchorElement))
                                {
                                    _element = _element.parentElement;
                                }

                                if (_element != null)
                                    _anchor = (HtmlAnchorElement)_element;
                            }
                            else
                            {
                                _anchor.href = _hrefLink;
                            }

                            if (_target != NavigateActionOption.Default)
                            {
                                _anchor.target = (_target == NavigateActionOption.NewWindow)
                                                    ? TARGET_WINDOW_NEW
                                                    : TARGET_WINDOW_SAME;
                            }
                        }
                    }
                    else if (_result == DialogResult.No)
                    {
                        // remove the current link assuming present
                        if (_anchor != null)
                            ExecuteCommandRange(_range, HTML_COMMAND_REMOVE_LINK, null);
                    }
                }
            }
            else
            {
                throw new HtmlEditorException("Must Select Text from which to create a Link.", "InsertLink");
            }
        }

        // remove a web link from the users selected text
        public void RemoveLink()
        {
            ExecuteCommandRange(HTML_COMMAND_REMOVE_LINK, null);
        }

        #endregion

        #region Text Insert Operations

        // insert the given HTML into the selected range
        public void InsertHtmlPrompt()
        {
            // display the dialog to obtain the Html to enter
            using (EditHtmlForm _dialog = new EditHtmlForm())
            {
                _dialog.HTML = "";
                _dialog.ReadOnly = false;
                _dialog.SetCaption(PASTE_TITLE_HTML);
                DefineDialogProperties(_dialog);

                if (_dialog.ShowDialog(ParentForm) == DialogResult.OK)
                {
                    SelectedHtml = _dialog.HTML;
                }
            }
        }

        // insert the given Text into the selected range
        public void InsertTextPrompt()
        {
            // display the dialog to obtain the Html to enter
            using (EditHtmlForm _dialog = new EditHtmlForm())
            {
                _dialog.HTML = "";
                _dialog.ReadOnly = false;
                _dialog.SetCaption(PASTE_TITLE_TEXT);
                DefineDialogProperties(_dialog);

                if (_dialog.ShowDialog(ParentForm) == DialogResult.OK)
                {
                    SelectedText = _dialog.HTML;
                }
            }
        }

        // returns the acceptable values for the format block
        public string[] GetFormatBlockCommands()
        {
            return m_formatCommands;
        }

        // formats the selected text wrapping items in the given format tag
        public void InsertFormatBlock(string command)
        {
            // ensure the command is acceptable
            if (Array.BinarySearch(m_formatCommands, command) < 0)
            {
                throw new HtmlEditorException("Invalid Format Block selection.", "InsertFormatBlock");
            }
            else
            {
                ExecuteCommandRange(HTML_COMMAND_INSERT_FORMAT_BLOCK, command);
            }
        }

        // formats the selected text wrapping items in a PRE tag for direct editing
        public void InsertFormattedBlock()
        {
            ExecuteCommandRange(HTML_COMMAND_INSERT_FORMAT_BLOCK, FORMATTED_PRE);
        }

        // unformats the selected text removing heading and pre tags
        public void InsertNormalBlock()
        {
            ExecuteCommandRange(HTML_COMMAND_INSERT_FORMAT_BLOCK, FORMATTED_NORMAL);
        }

        // inserts a heading tag values Heading 1,2,3,4,5
        public void InsertHeading(HtmlHeadingType headingType)
        {
            // obtain the selected range object
            HtmlTextRange _range = GetTextRange();

            // determine the appropriate heading tag and insert the heading
            string _command = string.Format("{0} {1}", FORMATTED_HEADING, (int)headingType);
            InsertFormatBlock(_command);
        }

        #endregion

        #region System Prompt Dialog Functions

        // allows the insertion of an image using the system dialogs
        public void SystemInsertImage()
        {
            ExecuteCommandDocumentPrompt(HTML_COMMAND_INSERT_IMAGE);
        }

        // allows the insertion of an href using the system dialogs
        public void SystemInsertLink()
        {
            ExecuteCommandDocumentPrompt(HTML_COMMAND_INSERT_LINK);
        }

        #endregion

        #region Font and Color Processing Operations

        // using the exec command define font properties for the selected text
        public void FormatFontAttributes(HtmlFontProperty font)
        {
            // obtain the selected range object
            HtmlTextRange _range = GetTextRange();

            if (_range != null && HtmlFontProperty.IsNotNull(font))
            {
                // Use the FONT object to set the properties
                // FontName, FontSize, Bold, Italic, Underline, Strikeout
                ExecuteCommandRange(_range, HTML_COMMAND_FONT_NAME, font.Name);

                // set the font size provide set to a value
                if (font.Size != HtmlFontSize.Default)
                    ExecuteCommandRange(_range, HTML_COMMAND_FONT_SIZE, (int)font.Size);

                // determine the BOLD property and correct as neccessary
                object _currentBold = QueryCommandRange(_range, HTML_COMMAND_BOLD);
                bool _fontBold = (_currentBold is Boolean) ? _fontBold = (bool)_currentBold : false;

                if (font.Bold != _fontBold)
                    ExecuteCommandRange(HTML_COMMAND_BOLD, null);

                // determine the UNDERLINE property and correct as neccessary
                object _currentUnderline = QueryCommandRange(_range, HTML_COMMAND_UNDERLINE);
                bool _fontUnderline = (_currentUnderline is Boolean) ? _fontUnderline = (bool)_currentUnderline : false;

                if (font.Underline != _fontUnderline)
                    ExecuteCommandRange(HTML_COMMAND_UNDERLINE, null);

                // determine the ITALIC property and correct as neccessary
                object _currentItalic = QueryCommandRange(_range, HTML_COMMAND_ITALIC);
                bool _fontItalic = (_currentItalic is Boolean) ? _fontItalic = (bool)_currentItalic : false;

                if (font.Italic != _fontItalic)
                    ExecuteCommandRange(HTML_COMMAND_ITALIC, null);

                // determine the STRIKEOUT property and correct as neccessary
                object _currentStrikeout = QueryCommandRange(_range, HTML_COMMAND_STRIKE_THROUGH);
                bool _fontStrikeout = (_currentStrikeout is Boolean) ? _fontStrikeout = (bool)_currentStrikeout : false;

                if (font.Strikeout != _fontStrikeout)
                    ExecuteCommandRange(HTML_COMMAND_STRIKE_THROUGH, null);

                // determine the SUPERSCRIPT property and correct as neccessary
                object _currentSuperscript = QueryCommandRange(_range, HTML_COMMAND_SUPERSCRIPT);
                bool _fontSuperscript = (_currentSuperscript is Boolean)
                                           ? _fontSuperscript = (bool)_currentSuperscript
                                           : false;

                if (font.Superscript != _fontSuperscript)
                    ExecuteCommandRange(HTML_COMMAND_SUPERSCRIPT, null);

                // determine the SUBSCRIPT property and correct as neccessary
                object _currentSubscript = QueryCommandRange(_range, HTML_COMMAND_SUBSCRIPT);
                bool _fontSubscript = (_currentSubscript is Boolean) ? _fontSubscript = (bool)_currentSubscript : false;

                if (font.Subscript != _fontSubscript)
                    ExecuteCommandRange(HTML_COMMAND_SUBSCRIPT, null);
            }
            else
            {
                // do not have text selected or a valid font class
                throw new HtmlEditorException("Invalid Text selection made or Invalid Font selection.", "FormatFont");
            }
        }

        // using the exec command define color properties for the selected tag
        public void FormatFontColor(Color color)
        {
            // Use the COLOR object to set the property ForeColor
            string _colorHtml;

            if (color != Color.Empty)
                _colorHtml = ColorTranslator.ToHtml(color);
            else
                _colorHtml = null;

            ExecuteCommandRange(HTML_COMMAND_FORE_COLOR, _colorHtml);
        }

        // display the defined font dialog use to set the selected text FONT
        public void FormatFontAttributesPrompt()
        {
            using (FontAttributeForm _dialog = new FontAttributeForm())
            {
                DefineDialogProperties(_dialog);
                _dialog.HtmlFont = GetFontAttributes();

                if (_dialog.ShowDialog(ParentForm) == DialogResult.OK)
                {
                    HtmlFontProperty font = _dialog.HtmlFont;
                    FormatFontAttributes(new HtmlFontProperty(font.Name, font.Size, font.Bold, font.Italic,
                                                              font.Underline, font.Strikeout, font.Subscript,
                                                              font.Superscript));
                }
            }
        }

        // display the system color dialog and use to set the selected text
        public void FormatFontColorPrompt()
        {
            // display the Color dialog and use the selected color to modify text
            using (ColorDialog _colorDialog = new ColorDialog())
            {
                _colorDialog.AnyColor = true;
                _colorDialog.SolidColorOnly = true;
                _colorDialog.AllowFullOpen = true;
                _colorDialog.Color = GetFontColor();
                _colorDialog.CustomColors = m_customColors;

                if (_colorDialog.ShowDialog(ParentForm) == DialogResult.OK)
                {
                    m_customColors = _colorDialog.CustomColors;
                    FormatFontColor(_colorDialog.Color);
                }
            }
        }

        // determine the Font of the selected text range
        // set to the default value of not defined
        public HtmlFontProperty GetFontAttributes()
        {
            // get the text range working with
            HtmlTextRange _range = GetTextRange();

            if (_range != null)
            {
                // get font name to show
                object currentName = QueryCommandRange(_range, HTML_COMMAND_FONT_NAME);
                string fontName = (currentName is String) ? (string)currentName : m_bodyFont.Name;

                // determine font size to show
                object currentSize = QueryCommandRange(_range, HTML_COMMAND_FONT_SIZE);
                HtmlFontSize _fontSize = (currentSize is Int32) ? (HtmlFontSize)currentSize : m_bodyFont.Size;

                // determine the BOLD property
                bool _fontBold = IsFontBold(_range);

                // determine the UNDERLINE property
                bool _fontUnderline = IsFontUnderline(_range);

                // determine the ITALIC property
                bool _fontItalic = IsFontItalic(_range);

                // determine the STRIKEOUT property
                bool _fontStrikeout = IsFontStrikeout(_range);

                // determine the SUPERSCRIPT property
                bool _fontSuperscript = IsFontSuperscript(_range);

                // determine the SUBSCRIPT property
                bool _fontSubscript = IsFontSubscript(_range);

                // calculate the Font and return
                return new HtmlFontProperty(fontName, _fontSize, _fontBold, _fontUnderline, _fontItalic, _fontStrikeout,
                                            _fontSubscript, _fontSuperscript);
            }
            else
            {
                // no rnage selected so return null
                return m_defaultFont;
            }
        }

        // determine if the current font selected is bold given a range
        private bool IsFontBold(HtmlTextRange range)
        {
            // determine the BOLD property
            object _currentBold = QueryCommandRange(range, HTML_COMMAND_BOLD);
            return (_currentBold is Boolean) ? (bool)_currentBold : m_bodyFont.Bold;
        }

        // determine if the current font selected is bold given a range
        public bool IsFontBold()
        {
            // get the text range working with
            HtmlTextRange _range = GetTextRange();

            // return the font property
            return IsFontBold(_range);
        }

        // determine if the current font selected is Underline given a range
        private bool IsFontUnderline(HtmlTextRange range)
        {
            // determine the UNDERLINE property
            object _currentUnderline = QueryCommandRange(range, HTML_COMMAND_UNDERLINE);
            return (_currentUnderline is Boolean) ? (bool)_currentUnderline : m_bodyFont.Underline;
        }

        // determine if the current font selected is Underline
        public bool IsFontUnderline()
        {
            // get the text range working with
            HtmlTextRange _range = GetTextRange();

            // return the font property
            return IsFontUnderline(_range);
        }

        // determine if the current font selected is Italic given a range
        private bool IsFontItalic(HtmlTextRange range)
        {
            // determine the ITALIC property
            object _currentItalic = QueryCommandRange(range, HTML_COMMAND_ITALIC);
            return (_currentItalic is Boolean) ? (bool)_currentItalic : m_bodyFont.Italic;
        }

        // determine if the current font selected is Italic
        public bool IsFontItalic()
        {
            // get the text range working with
            HtmlTextRange _range = GetTextRange();

            // return the font property
            return IsFontItalic(_range);
        }

        // determine if the current font selected is Strikeout given a range
        private bool IsFontStrikeout(HtmlTextRange range)
        {
            // determine the STRIKEOUT property
            object _currentStrikeout = QueryCommandRange(range, HTML_COMMAND_STRIKE_THROUGH);
            return (_currentStrikeout is Boolean) ? (bool)_currentStrikeout : m_bodyFont.Strikeout;
        }

        // determine if the current font selected is Strikeout
        public bool IsFontStrikeout()
        {
            // get the text range working with
            HtmlTextRange _range = GetTextRange();

            // return the font property
            return IsFontStrikeout(_range);
        }

        // determine if the current font selected is Superscript given a range
        private bool IsFontSuperscript(HtmlTextRange range)
        {
            // determine the SUPERSCRIPT property
            object _currentSuperscript = QueryCommandRange(range, HTML_COMMAND_SUPERSCRIPT);
            return (_currentSuperscript is Boolean) ? (bool)_currentSuperscript : false;
        }

        // determine if the current font selected is Superscript
        public bool IsFontSuperscript()
        {
            // get the text range working with
            HtmlTextRange _range = GetTextRange();

            // return the font property
            return IsFontSuperscript(_range);
        }

        // determine if the current font selected is Subscript given a range
        private bool IsFontSubscript(HtmlTextRange range)
        {
            // determine the SUBSCRIPT property
            object _currentSubscript = QueryCommandRange(range, HTML_COMMAND_SUBSCRIPT);
            return (_currentSubscript is Boolean) ? (bool)_currentSubscript : false;
        }


        // determine if the current font selected is Subscript
        public bool IsFontSubscript()
        {
            // get the text range working with
            HtmlTextRange _range = GetTextRange();

            // return the font property
            return IsFontSubscript(_range);
        }

        // determine the color of the selected text range
        // set to the default value of not defined
        private Color GetFontColor()
        {
            object _fontColor = QueryCommandRange(HTML_COMMAND_FORE_COLOR);
            return (_fontColor is Int32) ? ColorTranslator.FromWin32((int)_fontColor) : m_bodyForeColor;
        }

        #endregion

        #region Find and Replace Operations

        // dialog to allow the user to perform a find and replace
        public void FindReplacePrompt()
        {
            // define a default value for the text to find
            HtmlTextRange _range = GetTextRange();
            string _initText = string.Empty;

            if (_range != null)
            {
                string _findText = _range.text;

                if (_findText != null)
                    _initText = _findText.Trim();
            }

            // prompt the user for the new href
            using (FindReplaceForm _dialog = new FindReplaceForm(_initText,
                                                                FindReplaceReset,
                                                                FindFirst,
                                                                FindNext,
                                                                FindReplaceOne,
                                                                FindReplaceAll))
            {
                DefineDialogProperties(_dialog);
                DialogResult _result = _dialog.ShowDialog(ParentForm);
            }
        }

        // reset the find and replace options to initialize a new search
        public void FindReplaceReset()
        {
            // reset the range being worked with
            m_findRange = m_body.createTextRange();
            (m_document.selection).empty();
        }

        // finds the first occurrence of the given text string
        // uses false for the search options
        public bool FindFirst(string findText)
        {
            return FindFirst(findText, false, false);
        }

        // finds the first occurrence of the given text string
        public bool FindFirst(string findText, bool matchWhole, bool matchCase)
        {
            // reset the text search range prior to making any calls
            FindReplaceReset();

            // calls the Find Next once search has been initialized
            return FindNext(findText, matchWhole, matchCase);
        }

        // finds the next occurrence of a given text string
        // assumes a previous search was made
        // uses false for the search options
        public bool FindNext(string findText)
        {
            return FindNext(findText, false, false);
        }

        // finds the next occurrence of a given text string
        // assumes a previous search was made
        public bool FindNext(string findText, bool matchWhole, bool matchCase)
        {
            return (FindText(findText, matchWhole, matchCase) != null);
        }

        // replace the first occurrence of the given string with the other
        // uses false for the search options
        public bool FindReplaceOne(string findText, string replaceText)
        {
            return FindReplaceOne(findText, replaceText, true, true);
        }

        // replace the first occurrence of the given string with the other
        public bool FindReplaceOne(string findText, string replaceText, bool matchWhole, bool matchCase)
        {
            // find the given text within the find range
            HtmlTextRange _replaceRange = FindText(findText, matchWhole, matchCase);

            if (_replaceRange != null)
            {
                // if text found perform a replace
                _replaceRange.text = replaceText;
                _replaceRange.select();

                // replace made to return success
                return true;
            }
            else
            {
                // no replace was made so return false
                return false;
            }
        }

        // replaces all the occurrence of the given string with the other
        // uses false for the search options
        public int FindReplaceAll(string findText, string replaceText)
        {
            return FindReplaceAll(findText, replaceText, false, false);
        }

        // replaces all the occurrences of the given string with the other
        public int FindReplaceAll(string findText, string replaceText, bool matchWhole, bool matchCase)
        {
            int _found = 0;
            HtmlTextRange _replaceRange;

            do
            {
                // find the given text within the find range
                _replaceRange = FindText(findText, matchWhole, matchCase);

                // if found perform a replace
                if (_replaceRange != null)
                {
                    _replaceRange.text = replaceText;
                    _found++;
                }
            } while (_replaceRange != null);

            // return count of items repalced
            return _found;
        }

        // function to perform the actual find of the given text
        private HtmlTextRange FindText(string findText, bool matchWhole, bool matchCase)
        {
            // define the search options
            int _searchOptions = 0;

            if (matchWhole)
                _searchOptions = _searchOptions + 2;

            if (matchCase)
                _searchOptions = _searchOptions + 4;

            // perform the search operation
            if (m_findRange.findText(findText, m_findRange.text.Length, _searchOptions))
            {
                // select the found text within the document
                m_findRange.select();

                // limit the new find range to be from the newly found text
                HtmlTextRange _foundRange = (HtmlTextRange)m_document.selection.createRange();
                m_findRange = m_body.createTextRange();
                m_findRange.setEndPoint("StartToEnd", _foundRange);

                // text found so return this selection
                return _foundRange;
            }
            else
            {
                // reset the find ranges
                FindReplaceReset();

                // no text found so return null range
                return null;
            }
        }

        #endregion

        #region Table Processing Operations

        // public function to create a table class
        // insert method then works on this table
        public void TableInsert(HtmlTableProperty tableProperties)
        {
            // call the private insert table method with a null table entry
            ProcessTable(null, tableProperties);
        }

        // public function to modify a tables properties
        // ensure a table is currently selected or insertion point is within a table
        public bool TableModify(HtmlTableProperty tableProperties)
        {
            // define the Html Table element
            HtmlTable _table = GetTableElement();

            // if a table has been selected then process
            if (_table != null)
            {
                ProcessTable(_table, tableProperties);
                return true;
            }
            else
            {
                return false;
            }
        }

        // present to the user the table properties dialog
        // using all the default properties for the table based on an insert operation
        public void TableInsertPrompt()
        {
            // if user has selected a table create a reference
            HtmlTable _table = GetFirstControl() as HtmlTable;
            ProcessTablePrompt(_table);
        }


        // present to the user the table properties dialog
        // ensure a table is currently selected or insertion point is within a table
        public bool TableModifyPrompt()
        {
            // define the Html Table element
            HtmlTable _table = GetTableElement();

            // if a table has been selected then process
            if (_table != null)
            {
                ProcessTablePrompt(_table);
                return true;
            }
            else
            {
                return false;
            }
        }

        // will insert a new row into the table
        // based on the current user row and insertion after
        public void TableInsertRow()
        {
            // see if a table selected or insertion point inside a table
            HtmlTable _table = null;
            HtmlTableRow _row = null;
            GetTableElement(out _table, out _row);

            // process according to table being defined
            if (_table != null && _row != null)
            {
                try
                {
                    // find the existing row the user is on and perform the insertion
                    int _index = _row.rowIndex + 1;
                    HtmlTableRow _insertedRow = (HtmlTableRow)_table.insertRow(_index);

                    // add the new columns to the end of each row
                    int _numberCols = _row.cells.length;

                    for (int idxCol = 0; idxCol < _numberCols; idxCol++)
                    {
                        _insertedRow.insertCell();
                    }
                }
                catch (Exception _e)
                {
                    throw new HtmlEditorException("Unable to insert a new Row", "TableinsertRow", _e);
                }
            }
            else
            {
                throw new HtmlEditorException("Row not currently selected within the table", "TableInsertRow");
            }
        }

        // will delete the currently selected row
        // based on the current user row location
        public void TableDeleteRow()
        {
            // see if a table selected or insertion point inside a table
            HtmlTable _table = null;
            HtmlTableRow _row = null;

            GetTableElement(out _table, out _row);

            // process according to table being defined
            if (_table != null && _row != null)
            {
                try
                {
                    // find the existing row the user is on and perform the deletion
                    int _index = _row.rowIndex;
                    _table.deleteRow(_index);
                }
                catch (Exception ex)
                {
                    throw new HtmlEditorException("Unable to delete the selected Row", "TableDeleteRow", ex);
                }
            }
            else
            {
                throw new HtmlEditorException("Row not currently selected within the table", "TableDeleteRow");
            }
        }

        // present to the user the table properties dialog
        // using all the default properties for the table based on an insert operation
        private void ProcessTablePrompt(HtmlTable table)
        {
            using (TablePropertyForm _dialog = new TablePropertyForm())
            {
                // define the base set of table properties
                HtmlTableProperty _tableProperties = GetTableProperties(table);

                // set the dialog properties
                _dialog.TableProperties = _tableProperties;
                DefineDialogProperties(_dialog);

                // based on the user interaction perform the neccessary action
                if (_dialog.ShowDialog(ParentForm) == DialogResult.OK)
                {
                    _tableProperties = _dialog.TableProperties;

                    if (table == null)
                        TableInsert(_tableProperties);
                    else
                        ProcessTable(table, _tableProperties);
                }
            }
        }

        // function to insert a basic table
        // will honour the existing table if passed in
        private void ProcessTable(HtmlTable table, HtmlTableProperty tableProperties)
        {
            try
            {
                // obtain a reference to the body node and indicate table present
                HtmlDomNode _bodyNode = (HtmlDomNode)m_document.body;
                bool _tableCreated = false;

                // ensure a table node has been defined to work with
                if (table == null)
                {
                    // create the table and indicate it was created
                    table = (HtmlTable)m_document.createElement(TABLE_TAG);
                    _tableCreated = true;
                }

                // define the table border, width, cell padding and spacing
                table.border = tableProperties.BorderSize;

                if (tableProperties.TableWidth > 0)
                {
                    table.width = (tableProperties.TableWidthMeasurement == MeasurementOption.Pixel)
                                      ? string.Format("{0}", tableProperties.TableWidth)
                                      : string.Format("{0}%", tableProperties.TableWidth);
                }
                else
                {
                    table.width = string.Empty;
                }

                if (tableProperties.TableAlignment != HorizontalAlignOption.Default)
                    table.align = tableProperties.TableAlignment.ToString().ToLower();
                else
                    table.align = string.Empty;

                table.cellPadding = tableProperties.CellPadding.ToString();
                table.cellSpacing = tableProperties.CellSpacing.ToString();

                // define the given table caption and alignment
                string _caption = tableProperties.CaptionText;
                HtmlTableCaption _tableCaption = table.caption;

                if (!string.IsNullOrEmpty(_caption))
                {
                    // ensure table caption correctly defined
                    if (_tableCaption == null)
                        _tableCaption = table.createCaption();

                    ((HtmlElement)_tableCaption).innerText = _caption;

                    if (tableProperties.CaptionAlignment != HorizontalAlignOption.Default)
                        _tableCaption.align = tableProperties.CaptionAlignment.ToString().ToLower();

                    if (tableProperties.CaptionLocation != VerticalAlignOption.Default)
                        _tableCaption.vAlign = tableProperties.CaptionLocation.ToString().ToLower();
                }
                else
                {
                    // if no caption specified remove the existing one
                    if (_tableCaption != null)
                    {
                        // prior to deleting the caption the contents must be cleared
                        ((HtmlElement)_tableCaption).innerText = null;
                        table.deleteCaption();
                    }
                }

                // determine the number of rows one has to insert
                int _numberRows, _numberCols;

                if (_tableCreated)
                {
                    _numberRows = Math.Max((int)tableProperties.TableRows, 1);
                }
                else
                {
                    _numberRows = Math.Max((int)tableProperties.TableRows, 1) - table.rows.length;
                }

                // layout the table structure in terms of rows and columns
                table.cols = tableProperties.TableColumns;

                if (_tableCreated)
                {
                    // this section is an optimization based on creating a new table
                    // the section below works but not as efficiently
                    _numberCols = Math.Max((int)tableProperties.TableColumns, 1);

                    // insert the appropriate number of rows
                    HtmlTableRow _tableRow;

                    for (int _idxRow = 0; _idxRow < _numberRows; _idxRow++)
                    {
                        _tableRow = (HtmlTableRow)table.insertRow();

                        // add the new columns to the end of each row
                        for (int _idxCol = 0; _idxCol < _numberCols; _idxCol++)
                        {
                            _tableRow.insertCell();
                        }
                    }
                }
                else
                {
                    // if the number of rows is increasing insert the decrepency
                    if (_numberRows > 0)
                    {
                        // insert the appropriate number of rows
                        for (int _idxRow = 0; _idxRow < _numberRows; _idxRow++)
                        {
                            table.insertRow();
                        }
                    }
                    else
                    {
                        // remove the extra rows from the table
                        for (int _idxRow = _numberRows; _idxRow < 0; _idxRow++)
                        {
                            table.deleteRow(table.rows.length - 1);
                        }
                    }

                    // have the rows constructed
                    // now ensure the columns are correctly defined for each row
                    HtmlElementCollection rows = table.rows;

                    foreach (HtmlTableRow _tableRow in rows)
                    {
                        _numberCols = Math.Max((int)tableProperties.TableColumns, 1) - _tableRow.cells.length;

                        if (_numberCols > 0)
                        {
                            // add the new column to the end of each row
                            for (int _idxCol = 0; _idxCol < _numberCols; _idxCol++)
                            {
                                _tableRow.insertCell();
                            }
                        }
                        else
                        {
                            // reduce the number of cells in the given row
                            // remove the extra rows from the table
                            for (int _idxCol = _numberCols; _idxCol < 0; _idxCol++)
                            {
                                _tableRow.deleteCell(_tableRow.cells.length - 1);
                            }
                        }
                    }
                }

                // if the table was created then it requires insertion into the DOM
                // otherwise property changes are sufficient
                if (_tableCreated)
                {
                    // table processing all complete so insert into the DOM
                    HtmlDomNode _tableNode = (HtmlDomNode)table;
                    HtmlElement _tableElement = (HtmlElement)table;
                    HtmlSelection _selection = m_document.selection;
                    HtmlTextRange _textRange = GetTextRange();

                    // final insert dependant on what user has selected
                    if (_textRange != null)
                    {
                        // text range selected so overwrite with a table
                        try
                        {
                            string _selectedText = _textRange.text;

                            if (_selectedText != null)
                            {
                                // place selected text into first cell
                                HtmlTableRow _tableRow = (HtmlTableRow)table.rows.item(0, null);
                                ((HtmlElement)_tableRow.cells.item(0, null)).innerText = _selectedText;
                            }

                            _textRange.pasteHTML(_tableElement.outerHTML);
                        }
                        catch (Exception _e)
                        {
                            throw new HtmlEditorException("Invalid Text selection for the Insertion of a Table.",
                                                          "ProcessTable", _e);
                        }
                    }
                    else
                    {
                        HtmlControlRange _controlRange = GetAllControls();

                        if (_controlRange != null)
                        {
                            // overwrite any controls the user has selected
                            try
                            {
                                // clear the selection and insert the table
                                // only valid if multiple selection is enabled
                                for (int _idx = 1; _idx < _controlRange.length; _idx++)
                                {
                                    _controlRange.remove(_idx);
                                }

                                _controlRange.item(0).outerHTML = _tableElement.outerHTML;
                                // this should work with initial count set to zero
                                // controlRange.add((HtmlControlElement)table);
                            }
                            catch (Exception _e)
                            {
                                throw new HtmlEditorException("Cannot Delete all previously Controls selected.",
                                                              "ProcessTable", _e);
                            }
                        }
                        else
                        {
                            // insert the table at the end of the HTML
                            _bodyNode.appendChild(_tableNode);
                        }
                    }
                }
                else
                {
                    // table has been correctly defined as being the first selected item
                    // need to remove other selected items
                    HtmlControlRange _controlRange = GetAllControls();

                    if (_controlRange != null)
                    {
                        // clear the controls selected other than than the first table
                        // only valid if multiple selection is enabled
                        for (int _idx = 1; _idx < _controlRange.length; _idx++)
                        {
                            _controlRange.remove(_idx);
                        }
                    }
                }
            }
            catch (Exception _e)
            {
                // throw an exception indicating table structure change error
                throw new HtmlEditorException("Unable to modify Html Table properties.", "ProcessTable", _e);
            }
        }

        // determine if the current selection is a table
        // return the table element
        private void GetTableElement(out HtmlTable table, out HtmlTableRow row)
        {
            table = null;
            row = null;

            HtmlTextRange _range = GetTextRange();

            try
            {
                // first see if the table element is selected
                table = GetFirstControl() as HtmlTable;

                // if table not selected then parse up the selection tree
                if (table == null && _range != null)
                {
                    HtmlElement _element = _range.parentElement();

                    // parse up the tree until the table element is found
                    while (_element != null && table == null)
                    {
                        _element = _element.parentElement;

                        // extract the Table properties
                        if (_element is HtmlTable)
                        {
                            table = (HtmlTable)_element;
                        }

                        // extract the Row  properties
                        if (_element is HtmlTableRow)
                        {
                            row = (HtmlTableRow)_element;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // have unknown error so set return to null
                table = null;
                row = null;
            }
        }

        private HtmlTable GetTableElement()
        {
            // define the table and row elements and obtain there values
            HtmlTable _table = null;
            HtmlTableRow _row = null;
            GetTableElement(out _table, out _row);

            // return the defined table element
            return _table;
        }

        // given an HtmlTable determine the table properties
        private HtmlTableProperty GetTableProperties(HtmlTable table)
        {
            // define a set of base table properties
            HtmlTableProperty _tableProperties = new HtmlTableProperty(true);

            // if user has selected a table extract those properties
            if (table != null)
            {
                try
                {
                    // have a table so extract the properties
                    HtmlTableCaption _caption = table.caption;

                    // if have a caption persist the values
                    if (_caption != null)
                    {
                        _tableProperties.CaptionText = ((HtmlElement)table.caption).innerText;

                        if (_caption.align != null)
                            _tableProperties.CaptionAlignment =
                                (HorizontalAlignOption)
                                TryParseEnum(typeof(HorizontalAlignOption), _caption.align,
                                             HorizontalAlignOption.Default);

                        if (_caption.vAlign != null)
                            _tableProperties.CaptionLocation =
                                (VerticalAlignOption)
                                TryParseEnum(typeof(VerticalAlignOption), _caption.vAlign, VerticalAlignOption.Default);
                    }

                    // look at the table properties
                    if (table.border != null)
                        _tableProperties.BorderSize = TryParseByte(table.border.ToString(), _tableProperties.BorderSize);

                    if (table.align != null)
                        _tableProperties.TableAlignment =
                            (HorizontalAlignOption)
                            TryParseEnum(typeof(HorizontalAlignOption), table.align, HorizontalAlignOption.Default);

                    // define the table rows and columns
                    int _rows = Math.Min(table.rows.length, Byte.MaxValue);
                    int _cols = Math.Min(table.cols, Byte.MaxValue);

                    if (_cols == 0 && _rows > 0)
                    {
                        // cols value not set to get the maxiumn number of cells in the rows
                        foreach (HtmlTableRow _tableRow in table.rows)
                        {
                            _cols = Math.Max(_cols, _tableRow.cells.length);
                        }
                    }

                    _tableProperties.TableRows = (byte)Math.Min(_rows, byte.MaxValue);
                    _tableProperties.TableColumns = (byte)Math.Min(_cols, byte.MaxValue);

                    // define the remaining table properties
                    if (table.cellPadding != null)
                        _tableProperties.CellPadding = TryParseByte(table.cellPadding.ToString(), _tableProperties.CellPadding);

                    if (table.cellSpacing != null)
                        _tableProperties.CellSpacing = TryParseByte(table.cellSpacing.ToString(), _tableProperties.CellSpacing);

                    if (table.width != null)
                    {
                        string _tableWidth = table.width.ToString();

                        if (_tableWidth.TrimEnd(null).EndsWith("%"))
                        {
                            _tableProperties.TableWidth =
                                TryParseUshort(_tableWidth.Remove(_tableWidth.LastIndexOf("%"), 1),
                                               _tableProperties.TableWidth);

                            _tableProperties.TableWidthMeasurement = MeasurementOption.Percent;
                        }
                        else
                        {
                            _tableProperties.TableWidth = TryParseUshort(_tableWidth, _tableProperties.TableWidth);
                            _tableProperties.TableWidthMeasurement = MeasurementOption.Pixel;
                        }
                    }
                    else
                    {
                        _tableProperties.TableWidth = 0;
                        _tableProperties.TableWidthMeasurement = MeasurementOption.Pixel;
                    }
                }
                catch (Exception _e)
                {
                    // throw an exception indicating table structure change be determined
                    throw new HtmlEditorException("Unable to determine Html Table properties.", "GetTableProperties", _e);
                }
            }

            // return the table properties
            return _tableProperties;
        }

        // based on the user selection return a table definition
        // if table selected (or insertion point within table) return these values
        public void GetTableDefinition(out HtmlTableProperty table, out bool tableFound)
        {
            // see if a table selected or insertion point inside a table
            HtmlTable _htmlTable = GetTableElement();

            // process according to table being defined
            if (_htmlTable == null)
            {
                table = new HtmlTableProperty(true);
                tableFound = false;
            }
            else
            {
                table = GetTableProperties(_htmlTable);
                tableFound = true;
            }
        }

        // Determine if the insertion point or selection is a table
        private bool IsParentTable()
        {
            // see if a table selected or insertion point inside a table
            HtmlTable _htmlTable = GetTableElement();

            // process according to table being defined
            if (_htmlTable == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region MsHtml Command Processing

        // executes the execCommand on the selected range
        private void ExecuteCommandRange(string command, object data)
        {
            // obtain the selected range object and execute command
            HtmlTextRange _range = GetTextRange();
            ExecuteCommandRange(_range, command, data);
        }

        // executes the execCommand on the selected range (given the range)
        private void ExecuteCommandRange(HtmlTextRange range, string command, object data)
        {
            try
            {
                if (range != null)
                {
                    // ensure command is a valid command and then enabled for the selection
                    if (range.queryCommandSupported(command))
                    {
                        if (range.queryCommandEnabled(command))
                        {
                            // mark the selection with the appropriate tag
                            range.execCommand(command, false, data);
                        }
                    }
                }
            }
            catch (Exception _e)
            {
                // Unknown error so inform user
                throw new HtmlEditorException("Unknown MSHTML Error.", command, _e);
            }
        }

        // executes the execCommand on the document
        private void ExecuteCommandDocument(string command)
        {
            ExecuteCommandDocument(command, false);
        }

        // executes the execCommand on the document with a system prompt
        private void ExecuteCommandDocumentPrompt(string command)
        {
            ExecuteCommandDocument(command, true);
        }

        // executes the execCommand on the document with a system prompt
        private void ExecuteCommandDocument(string command, bool prompt)
        {
            try
            {
                // ensure command is a valid command and then enabled for the selection
                if (m_document.queryCommandSupported(command))
                {
                    // if (document.queryCommandEnabled(command)) {}
                    // Test fails with a COM exception if command is Print

                    // execute the given command
                    m_document.execCommand(command, prompt, null);
                }
            }
            catch (Exception _e)
            {
                // Unknown error so inform user
                throw new HtmlEditorException("Unknown MSHTML Error.", command, _e);
            }
        }

        // determines the value of the command
        private object QueryCommandRange(string command)
        {
            // obtain the selected range object and execute command
            HtmlTextRange _range = GetTextRange();
            return QueryCommandRange(_range, command);
        }

        // determines the value of the command
        private object QueryCommandRange(HtmlTextRange range, string command)
        {
            object _retValue = null;

            if (range != null)
            {
                try
                {
                    // ensure command is a valid command and then enabled for the selection
                    if (range.queryCommandSupported(command))
                    {
                        if (range.queryCommandEnabled(command))
                        {
                            _retValue = range.queryCommandValue(command);
                        }
                    }
                }
                catch (Exception)
                {
                    // have unknown error so set return to null
                    _retValue = null;
                }
            }

            // return the obtained value
            return _retValue;
        }

        // get the selected range object
        private HtmlTextRange GetTextRange()
        {
            // define the selected range object
            HtmlSelection _selection;
            HtmlTextRange _range = null;

            try
            {
                // calculate the text range based on user selection
                _selection = m_document.selection;

                if (IsStatedTag(_selection.type, SELECT_TYPE_TEXT) || IsStatedTag(_selection.type, SELECT_TYPE_NONE))
                {
                    _range = (HtmlTextRange)_selection.createRange();
                }
            }
            catch (Exception)
            {
                // have unknown error so set return to null
                _range = null;
            }

            return _range;
        }

        // get the selected range object
        private HtmlElement GetFirstControl()
        {
            // define the selected range object
            HtmlSelection _selection;
            HtmlControlRange _range;
            HtmlElement _control = null;

            try
            {
                // calculate the first control based on the user selection
                _selection = m_document.selection;

                if (IsStatedTag(_selection.type, SELECT_TYPE_CONTROL))
                {
                    _range = (HtmlControlRange)_selection.createRange();

                    if (_range.length > 0)
                        _control = _range.item(0);
                }
            }
            catch (Exception)
            {
                // have unknown error so set return to null
                _control = null;
            }

            return _control;
        }

        // obtains a control range to be worked with
        private HtmlControlRange GetAllControls()
        {
            // define the selected range object
            HtmlSelection _selection;
            HtmlControlRange _range = null;

            try
            {
                // calculate the first control based on the user selection
                _selection = m_document.selection;

                if (IsStatedTag(_selection.type, SELECT_TYPE_CONTROL))
                {
                    _range = (HtmlControlRange)_selection.createRange();
                }
            }
            catch (Exception)
            {
                // have unknow error so set return to null
                _range = null;
            }

            return _range;
        }

        #endregion

        #region Utility Functions

        // performs a parse of the string into an enum
        private object TryParseEnum(Type enumType, string stringValue, object defaultValue)
        {
            // try the enum parse and return the default 
            object _result = defaultValue;

            try
            {
                // try the enum parse operation
                _result = Enum.Parse(enumType, stringValue, true);
            }
            catch (Exception)
            {
                // default value will be returned
                _result = defaultValue;
            }

            // return the enum value
            return _result;
        }

        // perform of a string into a byte number
        private byte TryParseByte(string stringValue, byte defaultValue)
        {
            byte _result = defaultValue;
            double _doubleValue;

            // try the conversion to a double number
            if (Double.TryParse(stringValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out _doubleValue))
            {
                try
                {
                    // try a cast to a byte
                    _result = (byte)_doubleValue;
                }
                catch (Exception)
                {
                    // default value will be returned
                    _result = defaultValue;
                }
            }

            // return the byte value
            return _result;
        }

        // perform of a string into a byte number
        private ushort TryParseUshort(string stringValue, ushort defaultValue)
        {
            ushort _result = defaultValue;
            double _doubleValue;

            // try the conversion to a double number
            if (Double.TryParse(stringValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out _doubleValue))
            {
                try
                {
                    // try a cast to a byte
                    _result = (ushort)_doubleValue;
                }
                catch (Exception)
                {
                    // default value will be returned
                    _result = defaultValue;
                }
            }

            // return the byte value
            return _result;
        }

        // ensure dialog resembles the user form characteristics
        private void DefineDialogProperties(Form dialog)
        {
            // set ambient control properties
            dialog.Font = ParentForm.Font;
            dialog.ForeColor = ParentForm.ForeColor;
            dialog.BackColor = ParentForm.BackColor;
            dialog.Cursor = ParentForm.Cursor;
            dialog.RightToLeft = ParentForm.RightToLeft;

            // define location and control style as system
            dialog.StartPosition = FormStartPosition.CenterParent;
        }

        // determine if a string url is valid
        private bool IsValidHref(string href)
        {
            return Regex.IsMatch(href, HREF_TEST_EXPRESSION, RegexOptions.IgnoreCase);
        }

        // determine if the tage name is of the correct type
        private bool IsStatedTag(string tagText, string tagType)
        {
            return (string.Compare(tagText, tagType, true) == 0);
        }

        // removes references to about:blank from the anchors
        private void RebaseAnchorUrl()
        {
            if (m_rebaseUrlsNeeded)
            {
                // review the anchors and remove any references to about:blank
                HtmlElementCollection _anchors = m_body.getElementsByTagName(ANCHOR_TAG);

                foreach (HtmlElement _element in _anchors)
                {
                    try
                    {
                        HtmlAnchorElement _anchor = (HtmlAnchorElement)_element;
                        string _href = _anchor.href;

                        // see if href need updating
                        if (_href != null && Regex.IsMatch(_href, BLANK_HTML_PAGE, RegexOptions.IgnoreCase))
                        {
                            _anchor.href = _href.Replace(BLANK_HTML_PAGE, string.Empty);
                        }
                    }
                    catch (Exception)
                    {
                        // ignore any errors
                    }
                }
            }
        }

        #endregion

        #region Internal Events and Error Processing

        // method to process the command and handle error exception
        private void ProcessCommand(string command)
        {
            try
            {
                // Evaluate the Button property to determine which button was clicked.
                switch (command)
                {
                    case INTERNAL_COMMAND_TEXTCUT:
                        // Browser CUT command
                        TextCut();
                        break;
                    case INTERNAL_COMMAND_TEXTCOPY:
                        // Browser COPY command
                        TextCopy();
                        break;
                    case INTERNAL_COMMAND_TEXTPASTE:
                        // Browser PASTE command
                        TextPaste();
                        break;
                    case INTERNAL_COMMAND_TEXTDELETE:
                        // Browser DELETE command
                        TextDelete();
                        break;
                    case INTERNAL_COMMAND_CLEARSELECT:
                        // Clears user selection
                        TextClearSelect();
                        break;
                    case INTERNAL_COMMAND_SELECTALL:
                        // Selects all document content
                        TextSelectAll();
                        break;
                    case INTERNAL_COMMAND_EDITUNDO:
                        // Undo the previous editing
                        EditUndo();
                        break;
                    case INTERNAL_COMMAND_EDITREDO:
                        // Redo the previous undo
                        EditRedo();
                        break;
                    case INTERNAL_COMMAND_FORMATBOLD:
                        // Selection BOLD command
                        FormatBold();
                        break;
                    case INTERNAL_COMMAND_FORMATUNDERLINE:
                        // Selection UNDERLINE command
                        FormatUnderline();
                        break;
                    case INTERNAL_COMMAND_FORMATITALIC:
                        // Selection ITALIC command
                        FormatItalic();
                        break;
                    case INTERNAL_COMMAND_FORMATSUPERSCRIPT:
                        // Selection SUPERSCRIPT command
                        FormatSuperscript();
                        break;
                    case INTERNAL_COMMAND_FORMATSUBSCRIPT:
                        // Selection SUBSCRIPT command
                        FormatSubscript();
                        break;
                    case INTERNAL_COMMAND_FORMATSTRIKEOUT:
                        // Selection STRIKEOUT command
                        FormatStrikeout();
                        break;
                    case INTERNAL_COMMAND_FONTDIALOG:
                        // FONT style creation
                        FormatFontAttributesPrompt();
                        break;
                    case INTERNAL_COMMAND_FONTNORMAL:
                        // FONT style remove
                        FormatRemove();
                        break;
                    case INTERNAL_COMMAND_COLORDIALOG:
                        // COLOR style creation
                        FormatFontColorPrompt();
                        break;
                    case INTERNAL_COMMAND_FONTINCREASE:
                        // FONTSIZE increase
                        FormatFontIncrease();
                        break;
                    case INTERNAL_COMMAND_FONTDECREASE:
                        // FONTSIZE decrease
                        FormatFontDecrease();
                        break;
                    case INTERNAL_COMMAND_JUSTIFYLEFT:
                        // Justify Left
                        JustifyLeft();
                        break;
                    case INTERNAL_COMMAND_JUSTIFYCENTER:
                        // Justify Center
                        JustifyCenter();
                        break;
                    case INTERNAL_COMMAND_JUSTIFYRIGHT:
                        // Justify Right
                        JustifyRight();
                        break;
                    case INTERNAL_COMMAND_FONTINDENT:
                        // Tab Right
                        FormatTabRight();
                        break;
                    case INTERNAL_COMMAND_FONTOUTDENT:
                        // Tab Left
                        FormatTabLeft();
                        break;
                    case INTERNAL_COMMAND_LISTORDERED:
                        // Ordered List
                        FormatList(HtmlListType.Ordered);
                        break;
                    case INTERNAL_COMMAND_LISTUNORDERED:
                        // Unordered List
                        FormatList(HtmlListType.Unordered);
                        break;
                    case INTERNAL_COMMAND_INSERTLINE:
                        // Horizontal Line
                        InsertLine();
                        break;
                    case INTERNAL_COMMAND_INSERTTABLE:
                        // Display a dialog to enable the user to insert a table
                        TableInsertPrompt();
                        break;
                    case INTERNAL_COMMAND_TABLEPROPERTIES:
                        // Display a dialog to enable the user to modify a table
                        TableModifyPrompt();
                        break;
                    case INTERNAL_COMMAND_TABLEINSERTROW:
                        // Display a dialog to enable the user to modify a table
                        TableInsertRow();
                        break;
                    case INTERNAL_COMMAND_TABLEDELETEROW:
                        // Display a dialog to enable the user to modify a table
                        TableDeleteRow();
                        break;
                    case INTERNAL_COMMAND_INSERTIMAGE:
                        // Display a dialog to enable the user to insert a image
                        InsertImagePrompt();
                        break;
                    case INTERNAL_COMMAND_INSERTLINK:
                        // Display a dialog to enable user to insert the href
                        InsertLinkPrompt();
                        break;
                    case INTERNAL_COMMAND_INSERTTEXT:
                        // Display a dialog to enable user to insert the text
                        InsertTextPrompt();
                        break;
                    case INTERNAL_COMMAND_INSERTHTML:
                        // Display a dialog to enable user to insert the html
                        InsertHtmlPrompt();
                        break;
                    case INTERNAL_COMMAND_FINDREPLACE:
                        // Display a dialog to enable user to perform find and replace operations
                        FindReplacePrompt();
                        break;
                    case INTERNAL_COMMAND_DOCUMENTPRINT:
                        // Print the current document
                        DocumentPrint();
                        break;
                    case INTERNAL_COMMAND_OPENFILE:
                        // Open a selected file
                        OpenFilePrompt();
                        break;
                    case INTERNAL_COMMAND_SAVEFILE:
                        // Saves the current document
                        SaveFilePrompt(false);
                        break;
                    case INTERNAL_COMMAND_HTMLEDITOR:
                        HtmlContentsEdit();
                        break;

                    case INTERNAL_TOGGLE_OVERWRITE:
                        // toggles the document overwrite method
                        ToggleOverWrite();
                        break;
                    case INTERNAL_TOGGLE_TOOLBAR:
                        // toggles the toolbar visibility
                        ToolbarVisible = !m_toolbarVisible;
                        break;
                    case INTERNAL_TOGGLE_SCROLLBAR:
                        // toggles the scrollbar visibility
                        ScrollBars = (m_scrollBars == DisplayScrollBarOption.No
                                          ? DisplayScrollBarOption.Auto
                                          : DisplayScrollBarOption.No);
                        break;
                    case INTERNAL_TOGGLE_WORDWRAP:
                        // toggles the document word wrapping
                        AutoWordWrap = !m_autoWordWrap;
                        break;
                    default:
                        throw new HtmlEditorException("Unknown Operation Being Performed.", command);
                }
            }
            catch (HtmlEditorException _e)
            {
                // process the html exception
                OnHtmlException(new HtmlExceptionEventArgs(_e.Operation, _e));
            }
            catch (Exception _e)
            {
                // process the exception
                OnHtmlException(new HtmlExceptionEventArgs(null, _e));
            }

            // ensure web browser has the focus after command execution
            Focus();
        }

        // function to perform the format block insertion
        private void ProcessFormatBlock(string command)
        {
            try
            {
                // execute the insertion command
                InsertFormatBlock(command);
            }
            catch (HtmlEditorException e)
            {
                // process the html exception
                OnHtmlException(new HtmlExceptionEventArgs(e.Operation, e));
            }
            catch (Exception _e)
            {
                // process the standard exception
                OnHtmlException(new HtmlExceptionEventArgs(null, _e));
            }

            // ensure web browser has the focus after command execution
            Focus();
        }

        // method to raise an event if a delegeate is assigned
        private void OnHtmlException(HtmlExceptionEventArgs args)
        {
            if (HtmlException == null)
            {
                // obtain the message and operation
                // concatenate the message with any inner message
                string _operation = args.Operation;
                Exception _ex = args.ExceptionObject;
                string _message = _ex.Message;

                if (_ex.InnerException != null)
                {
                    if (_ex.InnerException.Message != null)
                    {
                        _message = string.Format("{0}\n{1}", _message, _ex.InnerException.Message);
                    }
                }

                // define the title for the internal message box
                string _title;

                if (string.IsNullOrEmpty(_operation))
                {
                    _title = "Unknown Error";
                }
                else
                {
                    _title = _operation + " Error";
                }

                // display the error message box
                MessageBox.Show(this, _message, _title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                HtmlException(this, args);
            }
        }

        // function to perform the menu bar context menu
        // used to display formatting types
        private void ProcessFormattingSelection(object sender, EventArgs e)
        {
            // obtain the menu item that was clicked
            MenuItem _menuItem = (MenuItem)sender;
            string _command = _menuItem.Text;

            // process the format block
            ProcessFormatBlock(_command);
        }

        // toolbar processing
        // calls the processcommand with the selected command
        private void editorToolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            string _command = e.Button.Tag.ToString();
            ProcessCommand(_command);
        }

        // series of function based on the context menu
        // each should call the corresponding command processor

        // Text Commands
        private void menuTextUndo_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_EDITUNDO);
        }

        private void menuTextRedo_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_EDITREDO);
        }

        private void menuTextCut_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_TEXTCUT);
        }

        private void menuTextCopy_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_TEXTCOPY);
        }

        private void menuTextPaste_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_TEXTPASTE);
        }

        private void menuTextDelete_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_TEXTDELETE);
        }

        private void menuTextFindReplace_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_FINDREPLACE);
        }

        private void menuTextSelectNone_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_CLEARSELECT);
        }

        private void menuTextSelectAll_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_SELECTALL);
        }

        private void menuTextFontIncrease_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_FONTINCREASE);
        }

        private void menuTextFontDecrease_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_FONTDECREASE);
        }

        private void menuTextFontNormal_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_FONTNORMAL);
        }

        private void menuTextFontBold_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_FORMATBOLD);
        }

        private void menuTextFontItalic_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_FORMATITALIC);
        }

        private void menuTextFontUnderline_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_FORMATUNDERLINE);
        }

        private void menuTextFontSuperscript_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_FORMATSUPERSCRIPT);
        }

        private void menuTextFontSubscript_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_FORMATSUBSCRIPT);
        }

        private void menuTextFontStrikeout_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_FORMATSTRIKEOUT);
        }

        private void menuTextFontIndent_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_FONTINDENT);
        }

        private void menuTextFontOutdent_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_FONTOUTDENT);
        }

        private void menuTextFontDialog_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_FONTDIALOG);
        }

        private void menuTextFontColor_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_COLORDIALOG);
        }

        private void menuTextFontListOrdered_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_LISTORDERED);
        }

        private void menuTextFontListUnordered_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_LISTUNORDERED);
        }

        // Insert Commands
        private void menuInsertLine_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_INSERTLINE);
        }

        private void menuInsertLink_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_INSERTLINK);
        }

        private void menuInsertImage_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_INSERTIMAGE);
        }

        private void menuInsertText_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_INSERTTEXT);
        }

        private void menuInsertHtml_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_INSERTHTML);
        }

        // Table Commands
        private void menuInsertTable_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_INSERTTABLE);
        }

        private void menuTableProperties_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_TABLEPROPERTIES);
        }

        private void menuTableInsertRow_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_TABLEINSERTROW);
        }

        private void menuTableDeleteRow_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_TABLEDELETEROW);
        }

        // Justify Commands
        private void menuJustifyLeft_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_JUSTIFYLEFT);
        }

        private void menuJustifyCenter_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_JUSTIFYCENTER);
        }

        private void menuJustifyRight_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_JUSTIFYRIGHT);
        }

        // Document Commands
        private void menuDocumentOpen_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_OPENFILE);
        }

        private void menuDocumentSave_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_SAVEFILE);
        }

        private void menuDocumentPrint_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_COMMAND_DOCUMENTPRINT);
        }

        private void menuDocumentOverwrite_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_TOGGLE_OVERWRITE);
        }

        private void menuDocumentToolbar_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_TOGGLE_TOOLBAR);
        }

        private void menuDocumentScrollbar_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_TOGGLE_SCROLLBAR);
        }

        private void menuDocumentWordwrap_Click(object sender, EventArgs e)
        {
            ProcessCommand(INTERNAL_TOGGLE_WORDWRAP);
        }

        #endregion
    }
}