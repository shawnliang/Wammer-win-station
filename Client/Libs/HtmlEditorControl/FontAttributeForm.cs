#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.HtmlEditor
{
    // Form used to enter an Html Font attribute
    // Input based on the HtmlFontAttribute struct
    internal class FontAttributeForm : Form
    {
        private Button bCancel;
        private Button bApply;
        private Container components;
        private CheckBox checkBold;
        private CheckBox checkUnderline;
        private CheckBox checkItalic;
        private Label labelSize;
        private CheckBox checkStrikeout;
        private CheckBox checkSubscript;
        private CheckBox checkSuperscript;
        private ComboBox listFontName;
        private ComboBox listFontSize;
        private Label labelName;
        private Label labelSample;

        // variable for passing back and forth the font attributes
        private HtmlFontProperty m_font;

        // property to define the Font attribute for the text
        public HtmlFontProperty HtmlFont
        {
            get
            {
                // define the font attributes
                string _fontName = listFontName.Text;
                HtmlFontSize _fontSize = (HtmlFontSize) listFontSize.SelectedIndex;
                bool _fontBold = checkBold.Checked;
                bool _fontUnderline = checkUnderline.Checked;
                bool _fontItalic = checkItalic.Checked;
                bool _fontStrikeout = checkStrikeout.Checked;
                bool _fontSuperscript = checkSuperscript.Checked;
                bool _fontSubscript = checkSubscript.Checked;
                m_font = new HtmlFontProperty(_fontName, _fontSize, _fontBold, _fontItalic, _fontUnderline, _fontStrikeout,
                                             _fontSubscript, _fontSuperscript);
                return m_font;
            }
            set
            {
                m_font = value;

                // define font name
                int selection = listFontName.FindString(m_font.Name);
                listFontName.SelectedIndex = selection;
                
                // define font size
                listFontSize.SelectedIndex = (int) m_font.Size;
                
                // define font properties
                checkBold.Checked = m_font.Bold;
                checkUnderline.Checked = m_font.Underline;
                checkItalic.Checked = m_font.Italic;
                checkStrikeout.Checked = m_font.Strikeout;
                checkSubscript.Checked = m_font.Subscript;
                checkSuperscript.Checked = m_font.Superscript;
                
                // set the sample text font
                SetFontTextSample();
            }
        }

        public FontAttributeForm()
        {
            InitializeComponent();

            // populate the list of available fonts for selection
            LoadFonts();
        }

        // loads into the list of font names
        // a series of font objects that represent the available fonts
        private void LoadFonts()
        {
            // suspend drawing
            listFontName.SuspendLayout();

            // load the installed fonts and iterate through the collections
            InstalledFontCollection _fonts = new InstalledFontCollection();

            foreach (FontFamily _family in _fonts.Families) // FontFamily.Families
            {
                // ensure font supports regular, bolding, underlining, and italics
                if (_family.IsStyleAvailable(FontStyle.Regular & FontStyle.Bold & FontStyle.Italic & FontStyle.Underline))
                {
                    listFontName.Items.Add(_family.Name);
                }
            }

            // define the selected item and resume drawing
            listFontName.SelectedIndex = 0;
            listFontName.ResumeLayout();
        }

        // event handler for all functions that affect font sample
        // font name list and checkboxes for bold, itaic, underline
        private void FontSelectionChanged(object sender, EventArgs e)
        {
            SetFontTextSample();
        }

        // sets the sample font text based on the user selection
        private void SetFontTextSample()
        {
            string _fontName = ((string) listFontName.SelectedItem);
            float _fontSize = Font.Size + 2;
            bool _fontBold = checkBold.Checked;
            bool _fontUnderline = checkUnderline.Checked;
            bool _fontItalic = checkItalic.Checked;
            bool _fontStrikeout = checkStrikeout.Checked;
            FontStyle _fontStyle = (_fontBold ? FontStyle.Bold : FontStyle.Regular) |
                                  (_fontItalic ? FontStyle.Italic : FontStyle.Regular) |
                                  (_fontUnderline ? FontStyle.Underline : FontStyle.Regular) |
                                  (_fontStrikeout ? FontStyle.Strikeout : FontStyle.Regular);
          
            // create the font object and define the labels fonts
            Font _font = new Font(_fontName, _fontSize, _fontStyle);
            labelSample.Font = _font;
        }

        // Clean up any resources being used.
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
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof (FontAttributeForm));
            this.bCancel = new System.Windows.Forms.Button();
            this.bApply = new System.Windows.Forms.Button();
            this.checkBold = new System.Windows.Forms.CheckBox();
            this.checkUnderline = new System.Windows.Forms.CheckBox();
            this.checkItalic = new System.Windows.Forms.CheckBox();
            this.labelSize = new System.Windows.Forms.Label();
            this.checkStrikeout = new System.Windows.Forms.CheckBox();
            this.checkSubscript = new System.Windows.Forms.CheckBox();
            this.checkSuperscript = new System.Windows.Forms.CheckBox();
            this.listFontName = new System.Windows.Forms.ComboBox();
            this.listFontSize = new System.Windows.Forms.ComboBox();
            this.labelName = new System.Windows.Forms.Label();
            this.labelSample = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bCancel
            // 
            this.bCancel.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                 ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(240, 224);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 26);
            this.bCancel.TabIndex = 0;
            this.bCancel.Text = "Cancel";
            // 
            // bApply
            // 
            this.bApply.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                 ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bApply.Location = new System.Drawing.Point(160, 224);
            this.bApply.Name = "bApply";
            this.bApply.Size = new System.Drawing.Size(75, 26);
            this.bApply.TabIndex = 1;
            this.bApply.Text = "Apply";
            // 
            // checkBold
            // 
            this.checkBold.Location = new System.Drawing.Point(160, 74);
            this.checkBold.Name = "checkBold";
            this.checkBold.Size = new System.Drawing.Size(104, 18);
            this.checkBold.TabIndex = 2;
            this.checkBold.Text = "Bold";
            this.checkBold.CheckStateChanged += new System.EventHandler(this.FontSelectionChanged);
            // 
            // checkUnderline
            // 
            this.checkUnderline.Location = new System.Drawing.Point(160, 111);
            this.checkUnderline.Name = "checkUnderline";
            this.checkUnderline.Size = new System.Drawing.Size(104, 18);
            this.checkUnderline.TabIndex = 3;
            this.checkUnderline.Text = "Underline";
            this.checkUnderline.CheckStateChanged += new System.EventHandler(this.FontSelectionChanged);
            // 
            // checkItalic
            // 
            this.checkItalic.Location = new System.Drawing.Point(160, 92);
            this.checkItalic.Name = "checkItalic";
            this.checkItalic.Size = new System.Drawing.Size(104, 19);
            this.checkItalic.TabIndex = 4;
            this.checkItalic.Text = "Italic";
            this.checkItalic.CheckStateChanged += new System.EventHandler(this.FontSelectionChanged);
            // 
            // labelSize
            // 
            this.labelSize.Location = new System.Drawing.Point(160, 9);
            this.labelSize.Name = "labelSize";
            this.labelSize.Size = new System.Drawing.Size(120, 19);
            this.labelSize.TabIndex = 6;
            this.labelSize.Text = "Font Size";
            // 
            // checkStrikeout
            // 
            this.checkStrikeout.Location = new System.Drawing.Point(160, 129);
            this.checkStrikeout.Name = "checkStrikeout";
            this.checkStrikeout.Size = new System.Drawing.Size(104, 19);
            this.checkStrikeout.TabIndex = 7;
            this.checkStrikeout.Text = "Strikeout";
            this.checkStrikeout.CheckStateChanged += new System.EventHandler(this.FontSelectionChanged);
            // 
            // checkSubscript
            // 
            this.checkSubscript.Location = new System.Drawing.Point(160, 166);
            this.checkSubscript.Name = "checkSubscript";
            this.checkSubscript.Size = new System.Drawing.Size(104, 19);
            this.checkSubscript.TabIndex = 8;
            this.checkSubscript.Text = "Subscript";
            // 
            // checkSuperscript
            // 
            this.checkSuperscript.Location = new System.Drawing.Point(160, 185);
            this.checkSuperscript.Name = "checkSuperscript";
            this.checkSuperscript.Size = new System.Drawing.Size(104, 18);
            this.checkSuperscript.TabIndex = 9;
            this.checkSuperscript.Text = "Superscript";
            // 
            // listFontName
            // 
            this.listFontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.listFontName.Location = new System.Drawing.Point(16, 28);
            this.listFontName.Name = "listFontName";
            this.listFontName.Size = new System.Drawing.Size(121, 184);
            this.listFontName.TabIndex = 10;
            this.listFontName.SelectedIndexChanged += new System.EventHandler(this.FontSelectionChanged);
            // 
            // listFontSize
            // 
            this.listFontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listFontSize.Items.AddRange(new object[]
                                                 {
                                                     "Default",
                                                     "1 : 8  points",
                                                     "2 : 10 points",
                                                     "3 : 12 points",
                                                     "4 : 14 points",
                                                     "5 : 18 points",
                                                     "6 : 24 points",
                                                     "7 : 36 points"
                                                 });
            this.listFontSize.Location = new System.Drawing.Point(160, 28);
            this.listFontSize.Name = "listFontSize";
            this.listFontSize.Size = new System.Drawing.Size(121, 22);
            this.listFontSize.TabIndex = 11;
            // 
            // labelName
            // 
            this.labelName.Location = new System.Drawing.Point(16, 9);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(120, 19);
            this.labelName.TabIndex = 12;
            this.labelName.Text = "Font Name";
            // 
            // labelSample
            // 
            this.labelSample.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelSample.Location = new System.Drawing.Point(16, 222);
            this.labelSample.Name = "labelSample";
            this.labelSample.Size = new System.Drawing.Size(120, 26);
            this.labelSample.TabIndex = 13;
            this.labelSample.Text = "Sample AaZa";
            this.labelSample.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // FontAttributeForm
            // 
            this.AcceptButton = this.bApply;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(322, 261);
            this.Controls.Add(this.labelSample);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.listFontSize);
            this.Controls.Add(this.listFontName);
            this.Controls.Add(this.checkSuperscript);
            this.Controls.Add(this.checkSubscript);
            this.Controls.Add(this.checkStrikeout);
            this.Controls.Add(this.labelSize);
            this.Controls.Add(this.checkItalic);
            this.Controls.Add(this.checkUnderline);
            this.Controls.Add(this.checkBold);
            this.Controls.Add(this.bApply);
            this.Controls.Add(this.bCancel);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FontAttributeForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Font Attributes";
            this.ResumeLayout(false);
        }

        #endregion
    }
}