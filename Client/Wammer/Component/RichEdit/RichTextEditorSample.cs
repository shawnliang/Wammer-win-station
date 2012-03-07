#region

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.RichEdit
{
    public class RichTextEditorSample : Form
    {
        #region Fields

        private Random m_r = new Random();
        private FormattingRule[] m_rules;
        private int m_selectionChanged;

        private Panel panel1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label lbSelectionLength;
        private Label lbSelectionStart;
        private Label lbLength;
        private Label lbPosition;
        private Label label5;
        private Panel panel2;
        private MainMenu mainMenu1;
        private MenuItem menuItem1;
        private MenuItem miViewRefreshStats;
        private MenuItem menuItem2;
        private MenuItem miEditUndo;
        private MenuItem menuItem3;
        private Label lbCanUndo;
        private Label label6;
        private Label lbCanRedo;
        private Label label8;
        private MenuItem menuItem4;
        private MenuItem miFileSave;
        private MenuItem miFileExit;
        private Label lbSelectionChanges;
        private Label label7;
        private MenuItem miToolsHighlightWordEnd;
        private MenuItem miToolsSyntaxHighlighting;
        private Button bApplyFormatting;
        private ComboBox cbForeColor;
        private ComboBox cbBackColor;
        private CheckBox cbBold;
        private ComboBox cbFont;
        private CheckBox cbItalic;
        private CheckBox cbHidden;
        private CheckBox cbProtected;
        private ComboBox cbUnderlineColor;
        private ComboBox cbUnderlineStyle;
        private CheckBox cbUseForeColor;
        private CheckBox cbUseBackColor;
        private CheckBox cbUseFont;
        private CheckBox cbUseBold;
        private CheckBox cbUseItalic;
        private CheckBox cbUseHidden;
        private CheckBox cbUseProtected;
        private CheckBox cbUseUnderlineStyle;
        private CheckBox cbUseScript;
        private CheckBox cbSuperscript;
        private CheckBox cbSubscript;
        private MenuItem miToolsShowCharacterClasses;
        private Label lbCharacterClass;
        private Label label9;
        private MenuItem miToolsHighlightWordStart;
        private MenuItem miEditClearUndo;
        private Label label4;
        private NumericUpDown nudUndoHistory;
        private Label lbChangeType;
        private Label label11;
        private GroupBox groupBox1;
        private Label label10;
        private NumericUpDown nudZoom;

        #endregion

        private RichTextEditor m_editor;
        private MenuItem miViewAutocomplete;
        private CheckBox cbUseLink;
        private CheckBox cbLink;
        private CheckBox cbUseDisabled;
        private CheckBox cbDisabled;
        private IContainer components;

        public RichTextEditorSample()
        {
            InitializeComponent();

            // Here we build up a list of formatting rules that will be applied to the 
            // text in the editor. It's just a series of regular expressions, along with 
            // a Format that will be applied to regions matching those regular expressions
            ArrayList _al = new ArrayList();

            Format _format = new Format();

            // Turn some keywords like public and private blue
            _format.ForeColor = Color.Blue;
            _al.Add(new FormattingRule(new Regex("public|private|protected|huge|all", RegexOptions.Compiled), _format));

            // Turn words like big and large, well, big
            _format = new Format();
            _format.Font = new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold);
            _al.Add(new FormattingRule(new Regex("big|large|huge|all", RegexOptions.Compiled | RegexOptions.IgnoreCase), _format));

            // Underline the words underlined and all
            _format = new Format();
            _format.UnderlineFormat = new UnderlineFormat(UnderlineStyle.Word, UnderlineColor.DarkMagenta);
            _al.Add(new FormattingRule(new Regex("underlined|all", RegexOptions.Compiled | RegexOptions.IgnoreCase), _format));

            // Put a red wavy underline under the word misspelled or the word all
            _format = new Format();
            _format.UnderlineFormat = new UnderlineFormat(UnderlineStyle.Normal, UnderlineColor.Black);
            _al.Add(new FormattingRule(new Regex("misspelled|all", RegexOptions.Compiled | RegexOptions.IgnoreCase), _format));

            _format = new Format();
            _format.Link = true;
            _al.Add(new FormattingRule(
                       new Regex(
                           "(?i)\\b((?:https?://|www\\d{0,3}[.]|[a-z0-9.\\-]+[.][a-z]{2,4}/)(?:[^\\s()<>]+|\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\))+(?:\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\)|[^\\s`!()\\[\\]{};:'\".,<>?«»“”‘’]))",
                           RegexOptions.None),
                       _format));

            m_rules = (FormattingRule[])_al.ToArray(typeof(FormattingRule));

            nudUndoHistory.Value = m_editor.UndoLength;

            UpdateStats();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RichTextEditorSample));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbChangeType = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.nudUndoHistory = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.lbCharacterClass = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lbSelectionChanges = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbCanRedo = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lbCanUndo = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbPosition = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbSelectionLength = new System.Windows.Forms.Label();
            this.lbSelectionStart = new System.Windows.Forms.Label();
            this.lbLength = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.nudZoom = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbUseDisabled = new System.Windows.Forms.CheckBox();
            this.cbDisabled = new System.Windows.Forms.CheckBox();
            this.cbUseLink = new System.Windows.Forms.CheckBox();
            this.cbLink = new System.Windows.Forms.CheckBox();
            this.cbSubscript = new System.Windows.Forms.CheckBox();
            this.cbSuperscript = new System.Windows.Forms.CheckBox();
            this.cbUseScript = new System.Windows.Forms.CheckBox();
            this.cbUseUnderlineStyle = new System.Windows.Forms.CheckBox();
            this.cbUseProtected = new System.Windows.Forms.CheckBox();
            this.cbUseHidden = new System.Windows.Forms.CheckBox();
            this.cbUseItalic = new System.Windows.Forms.CheckBox();
            this.cbUseBold = new System.Windows.Forms.CheckBox();
            this.cbUseFont = new System.Windows.Forms.CheckBox();
            this.cbUseBackColor = new System.Windows.Forms.CheckBox();
            this.cbUseForeColor = new System.Windows.Forms.CheckBox();
            this.bApplyFormatting = new System.Windows.Forms.Button();
            this.cbUnderlineStyle = new System.Windows.Forms.ComboBox();
            this.cbUnderlineColor = new System.Windows.Forms.ComboBox();
            this.cbProtected = new System.Windows.Forms.CheckBox();
            this.cbHidden = new System.Windows.Forms.CheckBox();
            this.cbItalic = new System.Windows.Forms.CheckBox();
            this.cbFont = new System.Windows.Forms.ComboBox();
            this.cbBackColor = new System.Windows.Forms.ComboBox();
            this.cbBold = new System.Windows.Forms.CheckBox();
            this.cbForeColor = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.miFileSave = new System.Windows.Forms.MenuItem();
            this.miFileExit = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.miEditUndo = new System.Windows.Forms.MenuItem();
            this.miEditClearUndo = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.miViewAutocomplete = new System.Windows.Forms.MenuItem();
            this.miViewRefreshStats = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.miToolsHighlightWordStart = new System.Windows.Forms.MenuItem();
            this.miToolsHighlightWordEnd = new System.Windows.Forms.MenuItem();
            this.miToolsSyntaxHighlighting = new System.Windows.Forms.MenuItem();
            this.miToolsShowCharacterClasses = new System.Windows.Forms.MenuItem();
            this.m_editor = new Waveface.Component.RichEdit.RichTextEditor();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUndoHistory)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudZoom)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbChangeType);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.nudUndoHistory);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.lbCharacterClass);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.lbSelectionChanges);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.lbCanRedo);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.lbCanUndo);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.lbPosition);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.lbSelectionLength);
            this.panel1.Controls.Add(this.lbSelectionStart);
            this.panel1.Controls.Add(this.lbLength);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 429);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(880, 116);
            this.panel1.TabIndex = 1;
            // 
            // lbChangeType
            // 
            this.lbChangeType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbChangeType.Location = new System.Drawing.Point(760, 9);
            this.lbChangeType.Name = "lbChangeType";
            this.lbChangeType.Size = new System.Drawing.Size(100, 27);
            this.lbChangeType.TabIndex = 19;
            this.lbChangeType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(656, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(100, 27);
            this.label11.TabIndex = 18;
            this.label11.Text = "Change Type:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudUndoHistory
            // 
            this.nudUndoHistory.Location = new System.Drawing.Point(544, 65);
            this.nudUndoHistory.Name = "nudUndoHistory";
            this.nudUndoHistory.Size = new System.Drawing.Size(120, 22);
            this.nudUndoHistory.TabIndex = 0;
            this.nudUndoHistory.ValueChanged += new System.EventHandler(this.nudUndoHistory_ValueChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(440, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 26);
            this.label4.TabIndex = 16;
            this.label4.Text = "Undo History:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbCharacterClass
            // 
            this.lbCharacterClass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCharacterClass.Location = new System.Drawing.Point(544, 37);
            this.lbCharacterClass.Name = "lbCharacterClass";
            this.lbCharacterClass.Size = new System.Drawing.Size(100, 26);
            this.lbCharacterClass.TabIndex = 15;
            this.lbCharacterClass.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(440, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 26);
            this.label9.TabIndex = 14;
            this.label9.Text = "Character Class:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbSelectionChanges
            // 
            this.lbSelectionChanges.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbSelectionChanges.Location = new System.Drawing.Point(544, 9);
            this.lbSelectionChanges.Name = "lbSelectionChanges";
            this.lbSelectionChanges.Size = new System.Drawing.Size(100, 27);
            this.lbSelectionChanges.TabIndex = 13;
            this.lbSelectionChanges.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(440, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 27);
            this.label7.TabIndex = 12;
            this.label7.Text = "Sel Changes:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbCanRedo
            // 
            this.lbCanRedo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCanRedo.Location = new System.Drawing.Point(336, 65);
            this.lbCanRedo.Name = "lbCanRedo";
            this.lbCanRedo.Size = new System.Drawing.Size(100, 26);
            this.lbCanRedo.TabIndex = 11;
            this.lbCanRedo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(232, 65);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 26);
            this.label8.TabIndex = 10;
            this.label8.Text = "Can Redo:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbCanUndo
            // 
            this.lbCanUndo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCanUndo.Location = new System.Drawing.Point(336, 37);
            this.lbCanUndo.Name = "lbCanUndo";
            this.lbCanUndo.Size = new System.Drawing.Size(100, 26);
            this.lbCanUndo.TabIndex = 9;
            this.lbCanUndo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(232, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 26);
            this.label6.TabIndex = 8;
            this.label6.Text = "Can Undo:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbPosition
            // 
            this.lbPosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbPosition.Location = new System.Drawing.Point(336, 9);
            this.lbPosition.Name = "lbPosition";
            this.lbPosition.Size = new System.Drawing.Size(100, 27);
            this.lbPosition.TabIndex = 7;
            this.lbPosition.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(232, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 27);
            this.label5.TabIndex = 6;
            this.label5.Text = "Position:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbSelectionLength
            // 
            this.lbSelectionLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbSelectionLength.Location = new System.Drawing.Point(112, 65);
            this.lbSelectionLength.Name = "lbSelectionLength";
            this.lbSelectionLength.Size = new System.Drawing.Size(100, 26);
            this.lbSelectionLength.TabIndex = 5;
            this.lbSelectionLength.Text = "Selection Length:";
            this.lbSelectionLength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbSelectionStart
            // 
            this.lbSelectionStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbSelectionStart.Location = new System.Drawing.Point(112, 37);
            this.lbSelectionStart.Name = "lbSelectionStart";
            this.lbSelectionStart.Size = new System.Drawing.Size(100, 26);
            this.lbSelectionStart.TabIndex = 4;
            this.lbSelectionStart.Text = "Selection Start:";
            this.lbSelectionStart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbLength
            // 
            this.lbLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbLength.Location = new System.Drawing.Point(112, 9);
            this.lbLength.Name = "lbLength";
            this.lbLength.Size = new System.Drawing.Size(100, 27);
            this.lbLength.TabIndex = 3;
            this.lbLength.Text = "Length:";
            this.lbLength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 26);
            this.label3.TabIndex = 2;
            this.label3.Text = "Selection Length:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "Selection Start:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "Length:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.nudZoom);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(688, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(192, 429);
            this.panel2.TabIndex = 2;
            // 
            // nudZoom
            // 
            this.nudZoom.DecimalPlaces = 1;
            this.nudZoom.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudZoom.Location = new System.Drawing.Point(64, 471);
            this.nudZoom.Maximum = new decimal(new int[] {
            639,
            0,
            0,
            65536});
            this.nudZoom.Minimum = new decimal(new int[] {
            7,
            0,
            0,
            65536});
            this.nudZoom.Name = "nudZoom";
            this.nudZoom.Size = new System.Drawing.Size(120, 22);
            this.nudZoom.TabIndex = 1;
            this.nudZoom.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudZoom.ValueChanged += new System.EventHandler(this.nudZoom_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbUseDisabled);
            this.groupBox1.Controls.Add(this.cbDisabled);
            this.groupBox1.Controls.Add(this.cbUseLink);
            this.groupBox1.Controls.Add(this.cbLink);
            this.groupBox1.Controls.Add(this.cbSubscript);
            this.groupBox1.Controls.Add(this.cbSuperscript);
            this.groupBox1.Controls.Add(this.cbUseScript);
            this.groupBox1.Controls.Add(this.cbUseUnderlineStyle);
            this.groupBox1.Controls.Add(this.cbUseProtected);
            this.groupBox1.Controls.Add(this.cbUseHidden);
            this.groupBox1.Controls.Add(this.cbUseItalic);
            this.groupBox1.Controls.Add(this.cbUseBold);
            this.groupBox1.Controls.Add(this.cbUseFont);
            this.groupBox1.Controls.Add(this.cbUseBackColor);
            this.groupBox1.Controls.Add(this.cbUseForeColor);
            this.groupBox1.Controls.Add(this.bApplyFormatting);
            this.groupBox1.Controls.Add(this.cbUnderlineStyle);
            this.groupBox1.Controls.Add(this.cbUnderlineColor);
            this.groupBox1.Controls.Add(this.cbProtected);
            this.groupBox1.Controls.Add(this.cbHidden);
            this.groupBox1.Controls.Add(this.cbItalic);
            this.groupBox1.Controls.Add(this.cbFont);
            this.groupBox1.Controls.Add(this.cbBackColor);
            this.groupBox1.Controls.Add(this.cbBold);
            this.groupBox1.Controls.Add(this.cbForeColor);
            this.groupBox1.Location = new System.Drawing.Point(0, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(192, 443);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Selection Formatting";
            // 
            // cbUseDisabled
            // 
            this.cbUseDisabled.Location = new System.Drawing.Point(32, 360);
            this.cbUseDisabled.Name = "cbUseDisabled";
            this.cbUseDisabled.Size = new System.Drawing.Size(32, 28);
            this.cbUseDisabled.TabIndex = 23;
            this.cbUseDisabled.CheckedChanged += new System.EventHandler(this.cbUseDisabled_CheckedChanged);
            // 
            // cbDisabled
            // 
            this.cbDisabled.Enabled = false;
            this.cbDisabled.Location = new System.Drawing.Point(72, 360);
            this.cbDisabled.Name = "cbDisabled";
            this.cbDisabled.Size = new System.Drawing.Size(104, 28);
            this.cbDisabled.TabIndex = 24;
            this.cbDisabled.Text = "Disabled";
            // 
            // cbUseLink
            // 
            this.cbUseLink.Location = new System.Drawing.Point(32, 332);
            this.cbUseLink.Name = "cbUseLink";
            this.cbUseLink.Size = new System.Drawing.Size(32, 28);
            this.cbUseLink.TabIndex = 21;
            this.cbUseLink.CheckedChanged += new System.EventHandler(this.cbUseLink_CheckedChanged);
            // 
            // cbLink
            // 
            this.cbLink.Enabled = false;
            this.cbLink.Location = new System.Drawing.Point(72, 332);
            this.cbLink.Name = "cbLink";
            this.cbLink.Size = new System.Drawing.Size(104, 28);
            this.cbLink.TabIndex = 22;
            this.cbLink.Text = "Link";
            // 
            // cbSubscript
            // 
            this.cbSubscript.Enabled = false;
            this.cbSubscript.Location = new System.Drawing.Point(72, 157);
            this.cbSubscript.Name = "cbSubscript";
            this.cbSubscript.Size = new System.Drawing.Size(104, 28);
            this.cbSubscript.TabIndex = 11;
            this.cbSubscript.Text = "Subscript";
            this.cbSubscript.CheckedChanged += new System.EventHandler(this.cbSubscript_CheckedChanged);
            // 
            // cbSuperscript
            // 
            this.cbSuperscript.Enabled = false;
            this.cbSuperscript.Location = new System.Drawing.Point(72, 129);
            this.cbSuperscript.Name = "cbSuperscript";
            this.cbSuperscript.Size = new System.Drawing.Size(104, 28);
            this.cbSuperscript.TabIndex = 10;
            this.cbSuperscript.Text = "Superscript";
            this.cbSuperscript.CheckedChanged += new System.EventHandler(this.cbSuperscript_CheckedChanged);
            // 
            // cbUseScript
            // 
            this.cbUseScript.Location = new System.Drawing.Point(32, 138);
            this.cbUseScript.Name = "cbUseScript";
            this.cbUseScript.Size = new System.Drawing.Size(32, 28);
            this.cbUseScript.TabIndex = 9;
            this.cbUseScript.CheckedChanged += new System.EventHandler(this.cbUseScript_CheckedChanged);
            // 
            // cbUseUnderlineStyle
            // 
            this.cbUseUnderlineStyle.Location = new System.Drawing.Point(32, 286);
            this.cbUseUnderlineStyle.Name = "cbUseUnderlineStyle";
            this.cbUseUnderlineStyle.Size = new System.Drawing.Size(32, 28);
            this.cbUseUnderlineStyle.TabIndex = 18;
            this.cbUseUnderlineStyle.CheckedChanged += new System.EventHandler(this.cbUseUnderlineStyle_CheckedChanged);
            // 
            // cbUseProtected
            // 
            this.cbUseProtected.Location = new System.Drawing.Point(32, 102);
            this.cbUseProtected.Name = "cbUseProtected";
            this.cbUseProtected.Size = new System.Drawing.Size(32, 27);
            this.cbUseProtected.TabIndex = 6;
            this.cbUseProtected.CheckedChanged += new System.EventHandler(this.cbUseProtected_CheckedChanged);
            // 
            // cbUseHidden
            // 
            this.cbUseHidden.Location = new System.Drawing.Point(32, 74);
            this.cbUseHidden.Name = "cbUseHidden";
            this.cbUseHidden.Size = new System.Drawing.Size(32, 28);
            this.cbUseHidden.TabIndex = 4;
            this.cbUseHidden.CheckedChanged += new System.EventHandler(this.cbUseHidden_CheckedChanged);
            // 
            // cbUseItalic
            // 
            this.cbUseItalic.Location = new System.Drawing.Point(32, 46);
            this.cbUseItalic.Name = "cbUseItalic";
            this.cbUseItalic.Size = new System.Drawing.Size(32, 28);
            this.cbUseItalic.TabIndex = 2;
            this.cbUseItalic.CheckedChanged += new System.EventHandler(this.cbUseItalic_CheckedChanged);
            // 
            // cbUseBold
            // 
            this.cbUseBold.Location = new System.Drawing.Point(32, 18);
            this.cbUseBold.Name = "cbUseBold";
            this.cbUseBold.Size = new System.Drawing.Size(32, 28);
            this.cbUseBold.TabIndex = 0;
            this.cbUseBold.CheckedChanged += new System.EventHandler(this.cbUseBold_CheckedChanged);
            // 
            // cbUseFont
            // 
            this.cbUseFont.Location = new System.Drawing.Point(32, 249);
            this.cbUseFont.Name = "cbUseFont";
            this.cbUseFont.Size = new System.Drawing.Size(32, 28);
            this.cbUseFont.TabIndex = 16;
            this.cbUseFont.CheckedChanged += new System.EventHandler(this.cbUseFont_CheckedChanged);
            // 
            // cbUseBackColor
            // 
            this.cbUseBackColor.Location = new System.Drawing.Point(32, 222);
            this.cbUseBackColor.Name = "cbUseBackColor";
            this.cbUseBackColor.Size = new System.Drawing.Size(32, 27);
            this.cbUseBackColor.TabIndex = 14;
            this.cbUseBackColor.CheckedChanged += new System.EventHandler(this.cbUseBackColor_CheckedChanged);
            // 
            // cbUseForeColor
            // 
            this.cbUseForeColor.Location = new System.Drawing.Point(32, 194);
            this.cbUseForeColor.Name = "cbUseForeColor";
            this.cbUseForeColor.Size = new System.Drawing.Size(32, 28);
            this.cbUseForeColor.TabIndex = 12;
            this.cbUseForeColor.CheckedChanged += new System.EventHandler(this.cbUseForeColor_CheckedChanged);
            // 
            // bApplyFormatting
            // 
            this.bApplyFormatting.Location = new System.Drawing.Point(72, 397);
            this.bApplyFormatting.Name = "bApplyFormatting";
            this.bApplyFormatting.Size = new System.Drawing.Size(75, 26);
            this.bApplyFormatting.TabIndex = 0;
            this.bApplyFormatting.Text = "Apply";
            this.bApplyFormatting.Click += new System.EventHandler(this.bApplyFormatting_Click);
            // 
            // cbUnderlineStyle
            // 
            this.cbUnderlineStyle.Enabled = false;
            this.cbUnderlineStyle.Items.AddRange(new object[] {
            "<<Underline Style>>",
            "None",
            "Normal",
            "Word",
            "Double",
            "Dotted",
            "Dash",
            "DashDot",
            "DashDotDot",
            "Wave  ",
            "Thick",
            "Hairline",
            "DoubleWave",
            "HeavyWave",
            "LongDash",
            "ThickDash",
            "ThickDashDot",
            "ThickDashDotDot",
            "ThickDotted",
            "ThickLongDash"});
            this.cbUnderlineStyle.Location = new System.Drawing.Point(72, 277);
            this.cbUnderlineStyle.Name = "cbUnderlineStyle";
            this.cbUnderlineStyle.Size = new System.Drawing.Size(121, 20);
            this.cbUnderlineStyle.TabIndex = 19;
            this.cbUnderlineStyle.Text = "<<Underline Style>>";
            // 
            // cbUnderlineColor
            // 
            this.cbUnderlineColor.Enabled = false;
            this.cbUnderlineColor.Items.AddRange(new object[] {
            "<<Underline Color>>",
            "Black",
            "Blue",
            "Cyan",
            "LimeGreen",
            "Magenta",
            "Red",
            "Yellow",
            "White",
            "DarkBlue",
            "DarkCyan",
            "Green",
            "DarkMagenta",
            "Brown",
            "OliveGreen",
            "DarkGray",
            "Gray"});
            this.cbUnderlineColor.Location = new System.Drawing.Point(72, 305);
            this.cbUnderlineColor.Name = "cbUnderlineColor";
            this.cbUnderlineColor.Size = new System.Drawing.Size(121, 20);
            this.cbUnderlineColor.TabIndex = 20;
            this.cbUnderlineColor.Text = "<<Underline Color>>";
            // 
            // cbProtected
            // 
            this.cbProtected.Enabled = false;
            this.cbProtected.Location = new System.Drawing.Point(72, 102);
            this.cbProtected.Name = "cbProtected";
            this.cbProtected.Size = new System.Drawing.Size(104, 27);
            this.cbProtected.TabIndex = 7;
            this.cbProtected.Text = "Protected";
            // 
            // cbHidden
            // 
            this.cbHidden.Enabled = false;
            this.cbHidden.Location = new System.Drawing.Point(72, 74);
            this.cbHidden.Name = "cbHidden";
            this.cbHidden.Size = new System.Drawing.Size(104, 28);
            this.cbHidden.TabIndex = 5;
            this.cbHidden.Text = "Hidden";
            // 
            // cbItalic
            // 
            this.cbItalic.Enabled = false;
            this.cbItalic.Location = new System.Drawing.Point(72, 46);
            this.cbItalic.Name = "cbItalic";
            this.cbItalic.Size = new System.Drawing.Size(104, 28);
            this.cbItalic.TabIndex = 3;
            this.cbItalic.Text = "Italic";
            // 
            // cbFont
            // 
            this.cbFont.Enabled = false;
            this.cbFont.Items.AddRange(new object[] {
            "<<Font>>",
            "Arial",
            "Courier New",
            "Lucida Console"});
            this.cbFont.Location = new System.Drawing.Point(72, 249);
            this.cbFont.Name = "cbFont";
            this.cbFont.Size = new System.Drawing.Size(121, 20);
            this.cbFont.TabIndex = 17;
            this.cbFont.Text = "<<Font>>";
            // 
            // cbBackColor
            // 
            this.cbBackColor.Enabled = false;
            this.cbBackColor.Items.AddRange(new object[] {
            "<<Fore Color>>",
            "Red",
            "Yellow",
            "Orange",
            "Green",
            "Blue",
            "Purple",
            "White",
            "Black",
            "Gray"});
            this.cbBackColor.Location = new System.Drawing.Point(72, 222);
            this.cbBackColor.Name = "cbBackColor";
            this.cbBackColor.Size = new System.Drawing.Size(121, 20);
            this.cbBackColor.TabIndex = 15;
            this.cbBackColor.Text = "<<Back Color>>";
            // 
            // cbBold
            // 
            this.cbBold.Enabled = false;
            this.cbBold.Location = new System.Drawing.Point(72, 18);
            this.cbBold.Name = "cbBold";
            this.cbBold.Size = new System.Drawing.Size(104, 28);
            this.cbBold.TabIndex = 1;
            this.cbBold.Text = "Bold";
            // 
            // cbForeColor
            // 
            this.cbForeColor.Enabled = false;
            this.cbForeColor.Items.AddRange(new object[] {
            "<<Fore Color>>",
            "Red",
            "Yellow",
            "Orange",
            "Green",
            "Blue",
            "Purple",
            "White",
            "Black",
            "Gray"});
            this.cbForeColor.Location = new System.Drawing.Point(72, 194);
            this.cbForeColor.Name = "cbForeColor";
            this.cbForeColor.Size = new System.Drawing.Size(121, 20);
            this.cbForeColor.TabIndex = 13;
            this.cbForeColor.Text = "<<Fore Color>>";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(8, 471);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 26);
            this.label10.TabIndex = 0;
            this.label10.Text = "Zoom:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem4,
            this.menuItem2,
            this.menuItem1,
            this.menuItem3});
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 0;
            this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miFileSave,
            this.miFileExit});
            this.menuItem4.Text = "&File";
            // 
            // miFileSave
            // 
            this.miFileSave.Index = 0;
            this.miFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.miFileSave.Text = "&Save";
            this.miFileSave.Click += new System.EventHandler(this.miFileSave_Click);
            // 
            // miFileExit
            // 
            this.miFileExit.Index = 1;
            this.miFileExit.Text = "E&xit";
            this.miFileExit.Click += new System.EventHandler(this.miFileExit_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miEditUndo,
            this.miEditClearUndo});
            this.menuItem2.Text = "&Edit";
            // 
            // miEditUndo
            // 
            this.miEditUndo.Index = 0;
            this.miEditUndo.Text = "&Undo";
            this.miEditUndo.Click += new System.EventHandler(this.miEditUndo_Click);
            // 
            // miEditClearUndo
            // 
            this.miEditClearUndo.Index = 1;
            this.miEditClearUndo.Text = "&Clear Undo History";
            this.miEditClearUndo.Click += new System.EventHandler(this.miEditClearUndo_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 2;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miViewAutocomplete,
            this.miViewRefreshStats});
            this.menuItem1.Text = "&View";
            // 
            // miViewAutocomplete
            // 
            this.miViewAutocomplete.Index = 0;
            this.miViewAutocomplete.Text = "&Autocomplete List";
            this.miViewAutocomplete.Click += new System.EventHandler(this.miViewAutocomplete_Click);
            // 
            // miViewRefreshStats
            // 
            this.miViewRefreshStats.Index = 1;
            this.miViewRefreshStats.Shortcut = System.Windows.Forms.Shortcut.F5;
            this.miViewRefreshStats.Text = "&Refresh Stats";
            this.miViewRefreshStats.Click += new System.EventHandler(this.miViewRefreshStats_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 3;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miToolsHighlightWordStart,
            this.miToolsHighlightWordEnd,
            this.miToolsSyntaxHighlighting,
            this.miToolsShowCharacterClasses});
            this.menuItem3.Text = "&Tools";
            // 
            // miToolsHighlightWordStart
            // 
            this.miToolsHighlightWordStart.Index = 0;
            this.miToolsHighlightWordStart.Text = "Highlight Word St&art Positions";
            this.miToolsHighlightWordStart.Click += new System.EventHandler(this.miToolsHighlightWordStart_Click);
            // 
            // miToolsHighlightWordEnd
            // 
            this.miToolsHighlightWordEnd.Index = 1;
            this.miToolsHighlightWordEnd.Text = "Highlight Word &End Positions";
            this.miToolsHighlightWordEnd.Click += new System.EventHandler(this.miToolsHighlightWordEnd_Click);
            // 
            // miToolsSyntaxHighlighting
            // 
            this.miToolsSyntaxHighlighting.Index = 2;
            this.miToolsSyntaxHighlighting.Text = "&Syntax Highlighting";
            this.miToolsSyntaxHighlighting.Click += new System.EventHandler(this.miToolsSyntaxHighlighting_Click);
            // 
            // miToolsShowCharacterClasses
            // 
            this.miToolsShowCharacterClasses.Index = 3;
            this.miToolsShowCharacterClasses.Text = "Show &Character Classes";
            this.miToolsShowCharacterClasses.Click += new System.EventHandler(this.miToolsShowCharacterClasses_Click);
            // 
            // editor
            // 
            this.m_editor.AcceptsTab = true;
            this.m_editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_editor.HideSelection = false;
            this.m_editor.Location = new System.Drawing.Point(0, 0);
            this.m_editor.Name = "m_editor";
            this.m_editor.Size = new System.Drawing.Size(688, 429);
            this.m_editor.TabIndex = 0;
            this.m_editor.Text = resources.GetString("editor.Text");
            this.m_editor.UndoLength = 100;
            this.m_editor.ModifiedChanged += new System.EventHandler(this.editor_ModifiedChanged);
            this.m_editor.TextChanged2 += new Waveface.Component.RichEdit.TextChanged2EventHandler(this.editor_TextChanged2);
            this.m_editor.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.editor_LinkClicked);
            this.m_editor.SelectionChanged += new System.EventHandler(this.editor_SelectionChanged);
            // 
            // RichTextEditorSample
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            this.ClientSize = new System.Drawing.Size(880, 545);
            this.Controls.Add(this.m_editor);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Menu = this.mainMenu1;
            this.Name = "RichTextEditorSample";
            this.Text = "RichTextEditor Demo";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudUndoHistory)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudZoom)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        [STAThread]
        private static void Main()
        {
            Application.Run(new RichTextEditorSample());
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Hook the ctrl-space key sequence and pop up the autocomplete dialog
            if (keyData == (Keys.Control | Keys.Space))
            {
                ShowAutoComplete();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void bApplyFormatting_Click(object sender, EventArgs e)
        {
            // Based on the checkboxes selected, we apply a bunch of formatting all at once, by specifying a set of FormattingInstructions
            FormattingInstructionCollection _instructions = new FormattingInstructionCollection();
            Format _format = new Format();

            if (cbUseBackColor.Checked)
            {
                _format.BackColor = StringToColor(cbBackColor.Text);
            }

            if (cbUseForeColor.Checked)
            {
                _format.ForeColor = StringToColor(cbForeColor.Text);
            }

            if (cbUseFont.Checked)
            {
                _format.Font = StringToFont(cbFont.Text);
            }

            if (cbUseBold.Checked)
            {
                _format.Bold = cbBold.Checked;
            }

            if (cbUseItalic.Checked)
            {
                _format.Italic = cbItalic.Checked;
            }

            if (cbUseHidden.Checked)
            {
                _format.Hidden = cbHidden.Checked;
            }

            if (cbUseProtected.Checked)
            {
                _format.Protected = cbProtected.Checked;
            }

            if (cbUseScript.Checked)
            {
                _format.Superscript = cbSuperscript.Checked;
                _format.Subscript = cbSubscript.Checked;
            }

            if (cbUseUnderlineStyle.Checked)
            {
                UnderlineStyle _style = (UnderlineStyle)Enum.Parse(typeof(UnderlineStyle), cbUnderlineStyle.Text);
                UnderlineColor _color = (UnderlineColor)Enum.Parse(typeof(UnderlineColor), cbUnderlineColor.Text);
                _format.UnderlineFormat = new UnderlineFormat(_style, _color);
            }

            if (cbUseLink.Checked)
            {
                _format.Link = cbLink.Checked;
            }

            if (cbUseDisabled.Checked)
            {
                _format.Disabled = cbDisabled.Checked;
            }

            _instructions.Add(new FormattingInstruction(m_editor.SelectionStart, m_editor.SelectionLength, _format));

            m_editor.BatchFormat(_instructions);
        }

        private void cbSubscript_CheckedChanged(object sender, EventArgs e)
        {
            // Subscript and superscript are mutually exclusive
            if (cbSubscript.Checked)
            {
                cbSuperscript.Checked = false;
            }
        }

        private void cbSuperscript_CheckedChanged(object sender, EventArgs e)
        {
            // Subscript and superscript are mutually exclusive
            if (cbSuperscript.Checked)
            {
                cbSubscript.Checked = false;
            }
        }

        private void cbUseBackColor_CheckedChanged(object sender, EventArgs e)
        {
            cbBackColor.Enabled = cbUseBackColor.Checked;
        }

        private void cbUseBold_CheckedChanged(object sender, EventArgs e)
        {
            cbBold.Enabled = cbUseBold.Checked;
        }

        private void cbUseDisabled_CheckedChanged(object sender, EventArgs e)
        {
            cbDisabled.Enabled = cbUseDisabled.Checked;
        }

        private void cbUseForeColor_CheckedChanged(object sender, EventArgs e)
        {
            cbForeColor.Enabled = cbUseForeColor.Checked;
        }

        private void cbUseFont_CheckedChanged(object sender, EventArgs e)
        {
            cbFont.Enabled = cbUseFont.Checked;
        }

        private void cbUseHidden_CheckedChanged(object sender, EventArgs e)
        {
            cbHidden.Enabled = cbUseHidden.Checked;
        }

        private void cbUseItalic_CheckedChanged(object sender, EventArgs e)
        {
            cbItalic.Enabled = cbUseItalic.Checked;
        }

        private void cbUseLink_CheckedChanged(object sender, EventArgs e)
        {
            cbLink.Enabled = cbUseLink.Checked;
        }

        private void cbUseProtected_CheckedChanged(object sender, EventArgs e)
        {
            cbProtected.Enabled = cbUseProtected.Checked;
        }

        private void cbUseScript_CheckedChanged(object sender, EventArgs e)
        {
            bool _enabled = cbUseScript.Checked;
            cbSuperscript.Enabled = _enabled;
            cbSubscript.Enabled = _enabled;
        }

        private void cbUseUnderlineStyle_CheckedChanged(object sender, EventArgs e)
        {
            bool _enabled = cbUseUnderlineStyle.Checked;
            cbUnderlineColor.Enabled = _enabled;
            cbUnderlineStyle.Enabled = _enabled;
        }

        private void editor_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            // This event is fired whenever a piece of text that is marked as a link gets clicked. 
            Process.Start(e.LinkText);
        }

        private void editor_ModifiedChanged(object sender, EventArgs e)
        {
            // Update the title bar to give the user some feedback about whether or
            // not changes have been made to the document. 
            if (m_editor.Modified)
            {
                Text = "TestHarness *";
            }
            else
            {
                Text = "TestHarness";
            }
        }

        private void editor_SelectionChanged(object sender, EventArgs e)
        {
            // Keep track of the number of times SelectionChanged has fired. This is purely
            // diagnostic - was interesting while I was developing redo and undo
            ++m_selectionChanged;

            UpdateStats();
        }

        private void editor_TextChanged2(object sender, TextChanged2EventArgs args)
        {
            // If the "syntax highlighting" feature is turned on, colorize all the text that meets the rules set up in the constructor. 
            if (miToolsSyntaxHighlighting.Checked)
            {
                SyntaxFormat();
            }

            // Provide feedback about what sort of change just occurred
            string _type = "Unknown";

            string _before = "";

            if (args.Before != null)
            {
                _before = args.Before;
            }

            string after = "";

            if (args.After != null)
            {
                after = args.After;
            }

            if (_before.Length == 0)
            {
                if (after.Length == 0)
                {
                    _type = "No change";
                }
                else
                {
                    _type = "Insertion";
                }
            }
            else
            {
                if (after.Length == 0)
                {
                    _type = "Deletion";
                }
                else
                {
                    _type = "Replacement";
                }
            }

            lbChangeType.Text = string.Format("{0} [{1}] -> [{2}]", _type, _before, after);
        }

        private void miEditClearUndo_Click(object sender, EventArgs e)
        {
            // Clear out the undo history
            m_editor.ClearUndo();
            UpdateStats();
        }

        private void miEditUndo_Click(object sender, EventArgs e)
        {
            m_editor.Undo();
        }

        private void miFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void miFileSave_Click(object sender, EventArgs e)
        {
            // This doesn't actually save anything to disk, just simulates it by 
            // setting the Modified flag to false
            m_editor.Modified = false;
        }

        private void miToolsHighlightWordEnd_Click(object sender, EventArgs e)
        {
            // Demonstrates both how to find the end of a word and another use of BatchFormat
            string _contents = m_editor.Text;
            int _position = 0;
            FormattingInstructionCollection _instructions = new FormattingInstructionCollection();

            Format _format = new Format();
            _format.BackColor = m_editor.BackColor;
            _instructions.Add(new FormattingInstruction(0, _contents.Length, _format));

            _format = new Format();
            _format.BackColor = Color.Yellow;

            while (_position < _contents.Length)
            {
                _position = m_editor.FindWordEnd(_position);
                _instructions.Add(new FormattingInstruction(_position, 1, _format));
                ++_position;
            }

            m_editor.BatchFormat(_instructions);
        }

        private void miToolsHighlightWordStart_Click(object sender, EventArgs e)
        {
            // Demonstrates both how to find the start of a word and another use of BatchFormat
            string _contents = m_editor.Text;
            int _position = _contents.Length;
            FormattingInstructionCollection _instructions = new FormattingInstructionCollection();

            Format _format = new Format();
            _format.BackColor = m_editor.BackColor;
            _instructions.Add(new FormattingInstruction(0, _contents.Length, _format));

            _format = new Format();
            _format.BackColor = Color.Orange;

            while (_position > 0)
            {
                _position = m_editor.FindWordStart(_position);
                _instructions.Add(new FormattingInstruction(_position, 1, _format));
                --_position;
            }

            m_editor.BatchFormat(_instructions);
        }

        private void miToolsShowCharacterClasses_Click(object sender, EventArgs e)
        {
            // Character classes are used internally by RichTextBox to figure out where word 
            // breaks are, etc. This shows how to get the character class for each character, 
            // then highlight every character with a color that corresponds. 
            string _contents = m_editor.Text;

            Color[] _colors = new[]
                                 {
                                     Color.Red, Color.Orange, Color.Yellow, Color.Green,
                                     Color.Blue, Color.Purple, Color.Brown, Color.White,
                                     Color.Pink, Color.Beige, Color.Chartreuse, Color.DarkMagenta,
                                     Color.Gold, Color.Gray, Color.PeachPuff, Color.PaleGreen
                                 };

            FormattingInstructionCollection _instructions = new FormattingInstructionCollection();

            for (int i = 0; i < _contents.Length; ++i)
            {
                CharacterInfo _info = m_editor.GetCharacterInfo(i);
                Format _format = new Format();
                _format.BackColor = _colors[_info.CharacterClass & 0x0F];
                _instructions.Add(new FormattingInstruction(i, 1, _format));
            }

            m_editor.BatchFormat(_instructions);
        }

        private void miToolsSyntaxHighlighting_Click(object sender, EventArgs e)
        {
            // Toggle the checkbox on the menu
            miToolsSyntaxHighlighting.Checked = !miToolsSyntaxHighlighting.Checked;

            // If it was just turned on, format the document according to the rules defined
            // in the constructor
            if (miToolsSyntaxHighlighting.Checked)
            {
                SyntaxFormat();
            }
        }

        private void miViewAutocomplete_Click(object sender, EventArgs e)
        {
            ShowAutoComplete();
        }

        private void miViewRefreshStats_Click(object sender, EventArgs e)
        {
            UpdateStats();
        }

        private void nudUndoHistory_ValueChanged(object sender, EventArgs e)
        {
            m_editor.UndoLength = Convert.ToInt32(nudUndoHistory.Value);
        }

        private void nudZoom_ValueChanged(object sender, EventArgs e)
        {
            m_editor.ZoomFactor = (float)Convert.ToDouble(nudZoom.Value);
        }

        private void ShowAutoComplete()
        {
            // We backtrack to the beginning of the current word, so if the user
            // has already typed in "on" they'll be positioned at "one" in the
            // autocomplete list. 
            int _startFrom = m_editor.FindWordStart(m_editor.SelectionStart);
            string[] _choices = new[]
                                   {
                                       "I雘褰n漮i獼跐iz犚i鷢", "a", "one", "two", "three",
                                       "fourfourfour fourfourfour", "fives", "sixes", "sevens"
                                   };
            m_editor.ShowAutoComplete(_choices, _startFrom);
        }

        private Color StringToColor(string s)
        {
            switch (s)
            {
                case "Red":
                    return Color.Red;
                case "Yellow":
                    return Color.Yellow;
                case "Orange":
                    return Color.Orange;
                case "Green":
                    return Color.Green;
                case "Blue":
                    return Color.Blue;
                case "Purple":
                    return Color.Purple;
                case "White":
                    return Color.White;
                case "Black":
                    return Color.Black;
                default:
                    return Color.Gray;
            }
        }

        private Font StringToFont(string s)
        {
            return new Font(s, 12);
        }

        private void SyntaxFormat()
        {
            // Note that you don't really want to use this method to style a document in 
            // real life, because it results in a complete reparse of the document on every 
            // change. Better would be to use the detailed information provided by TextChanged2 
            // to only reparse and reformat those parts of the document that need it. 
            // However, for demonstration purposes on short documents, this illustrates the 
            // concepts well enough

            string _contents = m_editor.Text;
            FormattingInstructionCollection _instructions = new FormattingInstructionCollection();

            Format _format = new Format();
            _format.ForeColor = Color.Black;
            _format.UnderlineFormat = new UnderlineFormat(UnderlineStyle.None, UnderlineColor.Black);
            _instructions.Add(new FormattingInstruction(0, _contents.Length, _format));

            // Run every regexp in our collection, and apply the corresponding format to whatever matches
            foreach (FormattingRule _rule in m_rules)
            {
                foreach (Match _match in _rule.Regex.Matches(_contents))
                {
                    _instructions.Add(new FormattingInstruction(_match.Index, _match.Length, _rule.Format));
                }
            }

            m_editor.BatchFormat(_instructions);
        }

        private void UpdateStats()
        {
            // Display some simple statistics about the state of the RichTextEditor
            lbLength.Text = m_editor.Text.Length.ToString();
            lbSelectionStart.Text = m_editor.SelectionStart.ToString();
            lbSelectionLength.Text = m_editor.SelectionLength.ToString();
            lbPosition.Text = m_editor.GetPositionFromCharIndex(m_editor.SelectionStart).ToString();
            lbCanRedo.Text = m_editor.CanRedo.ToString();
            lbCanUndo.Text = m_editor.CanUndo.ToString();
            lbSelectionChanges.Text = m_selectionChanged.ToString();

            if (m_editor.SelectionLength == 0 || m_editor.SelectionLength == 1)
            {
                lbCharacterClass.Text = m_editor.GetCharacterInfo(m_editor.SelectionStart).CharacterClass.ToString();
            }
            else
            {
                lbCharacterClass.Text = "";
            }

            int _undoHistoryLength = Convert.ToInt32(nudUndoHistory.Value);

            if (_undoHistoryLength != m_editor.UndoLength)
            {
                m_editor.UndoLength = _undoHistoryLength;
            }
        }
    }
}