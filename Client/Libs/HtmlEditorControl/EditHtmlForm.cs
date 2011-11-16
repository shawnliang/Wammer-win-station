#region

using System;
using System.ComponentModel;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.HtmlEditor
{
    // Form used to Edit or View Html contents
    // If a property RedOnly is true contents are considered viewable
    // No Html parsing is performed on the resultant data
    internal class EditHtmlForm : Form
    {
        private TextBox m_htmlText;
        private Button bOK;
        private Button bCancel;
        private Container components;

        // read only property for the form
        private bool m_readOnly;

        // string values for the form title
        private const string editCommand = "Cancel";
        private Button btnRemoveStyle;
        private const string viewCommand = "Close";

        #region Properties

        // property to set and get the HTML contents
        public string HTML
        {
            get { return m_htmlText.Text.Trim(); }
            set
            {
                m_htmlText.Text = (value != null) ? value.Trim() : string.Empty;
                m_htmlText.SelectionStart = 0;
                m_htmlText.SelectionLength = 0;
            }
        }

        // property that determines if the html is editable
        public bool ReadOnly
        {
            get { return m_readOnly; }
            set
            {
                m_readOnly = value;
                bOK.Visible = !m_readOnly;
                m_htmlText.ReadOnly = m_readOnly;
                bCancel.Text = m_readOnly ? viewCommand : editCommand;
            }
        }

        #endregion

        public EditHtmlForm()
        {
            InitializeComponent();

            // ensure content is empty
            m_htmlText.Text = string.Empty;
            ReadOnly = true;
        }

        // option to modify the caption of the display
        public void SetCaption(string caption)
        {
            Text = caption;
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
            this.m_htmlText = new System.Windows.Forms.TextBox();
            this.bOK = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.btnRemoveStyle = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_htmlText
            // 
            this.m_htmlText.AcceptsReturn = true;
            this.m_htmlText.AcceptsTab = true;
            this.m_htmlText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_htmlText.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_htmlText.Location = new System.Drawing.Point(8, 9);
            this.m_htmlText.Multiline = true;
            this.m_htmlText.Name = "m_htmlText";
            this.m_htmlText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.m_htmlText.Size = new System.Drawing.Size(758, 349);
            this.m_htmlText.TabIndex = 0;
            // 
            // bOK
            // 
            this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bOK.Location = new System.Drawing.Point(598, 368);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 26);
            this.bOK.TabIndex = 1;
            this.bOK.Text = "OK";
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(686, 368);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 26);
            this.bCancel.TabIndex = 2;
            this.bCancel.Text = "Cancel";
            // 
            // btnRemoveStyle
            // 
            this.btnRemoveStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveStyle.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnRemoveStyle.Location = new System.Drawing.Point(8, 368);
            this.btnRemoveStyle.Name = "btnRemoveStyle";
            this.btnRemoveStyle.Size = new System.Drawing.Size(96, 26);
            this.btnRemoveStyle.TabIndex = 3;
            this.btnRemoveStyle.Text = "Remove Style";
            this.btnRemoveStyle.Click += new System.EventHandler(this.btnRemoveStyle_Click);
            // 
            // EditHtmlForm
            // 
            this.AcceptButton = this.bOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(774, 402);
            this.Controls.Add(this.btnRemoveStyle);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.m_htmlText);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.MinimizeBox = false;
            this.Name = "EditHtmlForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Html";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void btnRemoveStyle_Click(object sender, System.EventArgs e)
        {
            string _html = m_htmlText.Text;

            try
            {
                _html = RemoveClassTag(_html);
            }
            catch
            {
            }

            try
            {
                _html = RemoveStyleTag1(_html);
            }
            catch
            {
            }

            try
            {
                _html = RemoveStyleTag2(_html);
            }
            catch
            {
            }

            m_htmlText.Text = _html;
        }

        #region HTML Utility

        public static string RemoveClassTag(string html)
        {
            string _html = html;

            int _idxS = 0;
            int _idxE1 = 0;
            int _idxE2 = 0;
            string _buf;

            while (true)
            {
                try
                {
                    _idxS = _html.IndexOf("class=", StringComparison.OrdinalIgnoreCase);

                    if (_idxS == -1)
                        break;

                    _buf = _html.Substring(_idxS + "class=".Length);

                    _idxE1 = _buf.IndexOf(">", StringComparison.OrdinalIgnoreCase);
                    _idxE2 = _buf.IndexOf(" ", StringComparison.OrdinalIgnoreCase);

                    _html = _html.Substring(0, _idxS - 1) + _buf.Substring(Math.Min(_idxE1, _idxE2));
                }
                catch
                {
                    return html;
                }
            }

            return _html;
        }

        public static string RemoveStyleTag1(string source)
        {
            string _html = source;

            int _idxS;
            int _idxE;
            string _buf;

            while (true)
            {
                try
                {
                    _idxS = _html.IndexOf("style=\"", StringComparison.OrdinalIgnoreCase);

                    if (_idxS == -1)
                        break;

                    _buf = _html.Substring(_idxS + "style=\"".Length);

                    _idxE = _buf.IndexOf("\"", StringComparison.OrdinalIgnoreCase);

                    _html = _html.Substring(0, _idxS - 1) + _buf.Substring(_idxE + 1);
                }
                catch
                {
                    return source;
                }
            }

            return _html;
        }

        public static string RemoveStyleTag2(string source)
        {
            string _ret = source;

            _ret = System.Text.RegularExpressions.Regex.Replace(_ret,
                    @"<( )*style([^>])*>", "<style>",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            _ret = System.Text.RegularExpressions.Regex.Replace(_ret,
                     @"(<( )*(/)( )*style( )*>)", "</style>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            _ret = System.Text.RegularExpressions.Regex.Replace(_ret,
                     "(<style>).*(</style>)", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return _ret;
        }


        #endregion
    }
}