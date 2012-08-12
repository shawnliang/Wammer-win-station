#region

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.Component
{
    /// <summary>
    /// A customizable Dialog box with 3 buttons, custom icon, and checkbox.
    /// </summary>
    internal partial class MsgBox : Form
    {
        /// <summary>
        /// Create a new instance of the dialog box with a message and title.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="title">Dialog Box title.</param>
        public MsgBox(string message, string title)
            : this(message, title, MessageBoxIcon.None)
        {
        }

        /// <summary>
        /// Create a new instance of the dialog box with a message and title and a standard windows messagebox icon.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="title">Dialog Box title.</param>
        /// <param name="icon">Standard system messagebox icon.</param>
        public MsgBox(string message, string title, MessageBoxIcon icon)
            : this(message, title, getMessageBoxIcon(icon))
        {
        }

        /// <summary>
        /// Create a new instance of the dialog box with a message and title and a custom windows icon.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="title">Dialog Box title.</param>
        /// <param name="icon">Custom icon.</param>
        public MsgBox(string message, string title, Icon icon)
        {
            InitializeComponent();

            messageLbl.Text = message;
            Text = title;

            m_sysIcon = icon;

            if (m_sysIcon == null)
                messageLbl.Location = new Point(FORM_X_MARGIN, FORM_Y_MARGIN);
        }

        /// <summary>
        /// Get system icon for MessageBoxIcon.
        /// </summary>
        /// <param name="icon">The MessageBoxIcon value.</param>
        /// <returns>SystemIcon type Icon.</returns>
        private static Icon getMessageBoxIcon(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Asterisk:
                    return SystemIcons.Asterisk;
                case MessageBoxIcon.Error:
                    return SystemIcons.Error;
                case MessageBoxIcon.Exclamation:
                    return SystemIcons.Exclamation;
                case MessageBoxIcon.Question:
                    return SystemIcons.Question;
                default:
                    return null;
            }
        }

        #region Setup API

        /// <summary>
        /// Min set width.
        /// </summary>
        private int m_minWidth;

        /// <summary>
        /// Min set height.
        /// </summary>
        private int m_minHeight;

        /// <summary>
        /// Sets the min size of the dialog box. If the text or button row needs more size then the dialog box will size to fit the text.
        /// </summary>
        /// <param name="width">Min width value.</param>
        /// <param name="height">Min height value.</param>
        public void SetMinSize(int width, int height)
        {
            m_minWidth = width;
            m_minHeight = height;
        }

        /// <summary>
        /// Create up to 3 buttons with no DialogResult values.
        /// </summary>
        /// <param name="names">Array of button names. Must of length 1-3.</param>
        public void SetButtons(params string[] names)
        {
            DialogResult[] drs = new DialogResult[names.Length];
            for (int i = 0; i < names.Length; i++)
                drs[i] = DialogResult.None;
            SetButtons(names, drs);
        }

        /// <summary>
        /// Create up to 3 buttons with given DialogResult values.
        /// </summary>
        /// <param name="names">Array of button names. Must of length 1-3.</param>
        /// <param name="results">Array of DialogResult values. Must be same length as names.</param>
        public void SetButtons(string[] names, DialogResult[] results)
        {
            SetButtons(names, results, 1);
        }

        /// <summary>
        /// Create up to 3 buttons with given DialogResult values.
        /// </summary>
        /// <param name="names">Array of button names. Must of length 1-3.</param>
        /// <param name="results">Array of DialogResult values. Must be same length as names.</param>
        /// <param name="def">Default Button number. Must be 1-3.</param>
        public void SetButtons(string[] names, DialogResult[] results, int def)
        {
            if (names == null)
                throw new ArgumentNullException("btnText", "Button Text is null");

            int count = names.Length;

            if (count < 1 || count > 3)
                throw new ArgumentException("Invalid number of buttons. Must be between 1 and 3.");

            //---- Set Button 1
            m_minButtonRowWidth += setButtonParams(btn1, names[0], def == 1 ? 1 : 2, results[0]);

            //---- Set Button 2
            if (count > 1)
            {
                m_minButtonRowWidth += setButtonParams(btn2, names[1], def == 2 ? 1 : 3, results[1]) + BUTTON_SPACE;
            }

            //---- Set Button 3
            if (count > 2)
            {
                m_minButtonRowWidth += setButtonParams(btn3, names[2], def == 3 ? 1 : 4, results[2]) + BUTTON_SPACE;
            }
        }

        /// <summary>
        /// The min required width of the button and checkbox row. Sum of button widths + checkbox width + margins.
        /// </summary>
        private int m_minButtonRowWidth;

        /// <summary>
        /// Sets button text and returns the width.
        /// </summary>
        /// <param name="btn">Button object.</param>
        /// <param name="text">Text of the button.</param>
        /// <param name="tab">TabIndex of the button.</param>
        /// <param name="dr">DialogResult of the button.</param>
        /// <returns>Width of the button.</returns>
        private static int setButtonParams(Button btn, string text, int tab, DialogResult dr)
        {
            btn.Text = text;
            btn.Visible = true;
            btn.DialogResult = dr;
            btn.TabIndex = tab;
            return btn.Size.Width;
        }

        /// <summary>
        /// Enables the checkbox. By default the checkbox is unchecked.
        /// </summary>
        /// <param name="text">Text of the checkbox.</param>
        public void SetCheckbox(string text)
        {
            SetCheckbox(text, false);
        }

        /// <summary>
        /// Enables the checkbox and the default checked state.
        /// </summary>
        /// <param name="text">Text of the checkbox.</param>
        /// <param name="chcked">Default checked state of the box.</param>
        public void SetCheckbox(string text, bool chcked)
        {
            chkBx.Visible = true;
            chkBx.Text = text;
            chkBx.Checked = chcked;
            m_minButtonRowWidth += chkBx.Size.Width + CHECKBOX_SPACE;
        }

        #endregion

        #region Sizes and Locations

        private void DialogBox_Load(object sender, EventArgs e)
        {
            if (!btn1.Visible)
                SetButtons(new[] {"OK"}, new[] {DialogResult.OK});

            m_minButtonRowWidth += 2*FORM_X_MARGIN; //add margin to the ends

            setDialogSize();

            setButtonRowLocations();
        }

        private const int FORM_Y_MARGIN = 10;
        private const int FORM_X_MARGIN = 16;
        private const int BUTTON_SPACE = 5;
        private const int CHECKBOX_SPACE = 15;
        private const int TEXT_Y_MARGIN = 30;

        /// <summary>
        /// Auto fits the dialog box to fit the text and the buttons.
        /// </summary>
        private void setDialogSize()
        {
            int requiredWidth = messageLbl.Location.X + messageLbl.Size.Width + FORM_X_MARGIN;
            requiredWidth = requiredWidth > m_minButtonRowWidth ? requiredWidth : m_minButtonRowWidth;

            int requiredHeight = messageLbl.Location.Y + messageLbl.Size.Height - btn2.Location.Y + ClientSize.Height +
                                 TEXT_Y_MARGIN;

            int minSetWidth = ClientSize.Width > m_minWidth ? ClientSize.Width : m_minWidth;
            int minSetHeight = ClientSize.Height > m_minHeight ? ClientSize.Height : m_minHeight;

            Size s = new Size();
            s.Width = requiredWidth > minSetWidth ? requiredWidth : minSetWidth;
            s.Height = requiredHeight > minSetHeight ? requiredHeight : minSetHeight;
            ClientSize = s;
        }

        /// <summary>
        /// Sets the buttons and checkboxe location.
        /// </summary>
        private void setButtonRowLocations()
        {
            int formWidth = ClientRectangle.Width;

            int x = formWidth - FORM_X_MARGIN;
            int y = btn1.Location.Y;

            if (btn3.Visible)
            {
                x -= btn3.Size.Width;
                btn3.Location = new Point(x, y);
                x -= BUTTON_SPACE;
            }

            if (btn2.Visible)
            {
                x -= btn2.Size.Width;
                btn2.Location = new Point(x, y);
                x -= BUTTON_SPACE;
            }

            x -= btn1.Size.Width;
            btn1.Location = new Point(x, y);

            if (chkBx.Visible)
                chkBx.Location = new Point(FORM_X_MARGIN, chkBx.Location.Y);
        }

        #endregion

        #region Icon Pain

        /// <summary>
        /// The icon to paint.
        /// </summary>
        private Icon m_sysIcon;

        /// <summary>
        /// Paint the System Icon in the top left corner.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_sysIcon != null)
            {
                Graphics g = e.Graphics;
                g.DrawIconUnstretched(m_sysIcon,
                                      new Rectangle(FORM_X_MARGIN, FORM_Y_MARGIN, m_sysIcon.Width, m_sysIcon.Height));
            }

            base.OnPaint(e);
        }

        #endregion

        #region Result API

        /// <summary>
        /// If visible checkbox was checked.
        /// </summary>
        public bool CheckboxChecked
        {
            get { return chkBx.Checked; }
        }

        private DialogBoxResult m_result;

        /// <summary>
        /// Gets the button that was pressed.
        /// </summary>
        public DialogBoxResult DialogBoxResult
        {
            get { return m_result; }
        }

        private void btn_Click(object sender, EventArgs e)
        {
            if (sender == btn1)
                m_result = DialogBoxResult.Button1;
            else if (sender == btn2)
                m_result = DialogBoxResult.Button2;
            else if (sender == btn3)
                m_result = DialogBoxResult.Button3;

            if (((Button) sender).DialogResult == DialogResult.None)
                Close();
        }

        #endregion
    }

    internal enum DialogBoxResult
    {
        Button1,
        Button2,
        Button3
    }

    /*
     
             private void button1_Click(object sender, EventArgs e)
        {
            MsgBox db = new MsgBox("This is a simple message", "This is a test title");
            db.ShowDialog(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MsgBox db = new MsgBox("There are 2 buttons with custom names.\nUse \"SetButtons\" API.",
                                   "This is a test title", MessageBoxIcon.Information);
            db.SetButtons("First Button", "Second Button");
            db.ShowDialog(this);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MsgBox db = new MsgBox("There is a checkbox.\nUse \"SetCheckbox\" API.", "This is a test title",
                                   MessageBoxIcon.Information);
            db.SetButtons("First Button", "Second Button");
            db.SetCheckbox("There is also a checkbox");
            db.ShowDialog(this);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MsgBox db = new MsgBox("Set the size of the dialog box. Use \"SetMinSize\" API.", "This is a test title",
                                   MessageBoxIcon.Information);
            db.SetButtons("First Button", "Second Button");
            db.SetCheckbox("There is also a checkbox");
            db.SetMinSize(500, 500);
            db.ShowDialog(this);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MsgBox db =
                new MsgBox(
                    "There are 2 buttons with default second button.\nUse \"SetButtons\" API and set the \"results\" and \"def\" arguments.",
                    "This is a test title", MessageBoxIcon.Information);
            db.SetButtons(new[] {"First Button", "Second Button"}, new[] {DialogResult.Yes, DialogResult.No}, 2);
            DialogResult r = db.ShowDialog(this);
        }
      
    */
}