#region

using System;
using System.ComponentModel;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.HtmlEditor
{
    // Form designed to control Find and Replace operations
    // Find and Replace operations performed by the user control class
    // Delegates need to be defined to reference the class instances
    internal class FindReplaceForm : Form
    {
        private TabPage tabFind;
        private TabPage tabReplace;
        private Label labelFind;
        private TabControl tabControl;
        private Button bCancel;
        private TextBox textFind;
        private Button bFindNext;
        private Label labelReplace;
        private Button bReplaceAll;
        private Button bReplace;
        private Button bOptions;
        private CheckBox optionMatchCase;
        private CheckBox optionMatchWhole;
        private TextBox textReplace;
        private Panel panelOptions;
        private Panel panelInput;

        // private variables defining the state of the form
        private bool options;
        private bool findNotReplace = true;
        private string findText;
        private string replaceText;

        // internal delegate reference
        private FindReplaceResetDelegate FindReplaceReset;
        private FindReplaceOneDelegate FindReplaceOne;
        private FindReplaceAllDelegate FindReplaceAll;
        //private FindFirstDelegate FindFirst;
        private FindNextDelegate FindNext;


        // public constructor that defines the required delegates
        // delegates must be defined for the find and replace to operate
        public FindReplaceForm(string initText, FindReplaceResetDelegate resetDelegate,
                               FindFirstDelegate findFirstDelegate, FindNextDelegate findNextDelegate,
                               FindReplaceOneDelegate replaceOneDelegate, FindReplaceAllDelegate replaceAllDelegate)
        {
            InitializeComponent();

            // Define the initial state of the form assuming a Find command to be displayed first
            DefineFindWindow(findNotReplace);
            DefineOptionsWindow(options);

            // ensure buttons not initially enabled
            bFindNext.Enabled = false;
            bReplace.Enabled = false;
            bReplaceAll.Enabled = false;

            // save the delegates used to perform find and replcae operations
            FindReplaceReset = resetDelegate;
            //FindFirst = findFirstDelegate;
            FindNext = findNextDelegate;
            FindReplaceOne = replaceOneDelegate;
            FindReplaceAll = replaceAllDelegate;

            // define the original text
            textFind.Text = initText;
        }

        // setup the properties based on the find or repalce functionality
        private void DefineFindWindow(bool find)
        {
            textReplace.Visible = !find;
            labelReplace.Visible = !find;
            bReplace.Visible = !find;
            bReplaceAll.Visible = !find;
            textFind.Focus();
        }

        // define if the options dialog is shown
        private void DefineOptionsWindow(bool options)
        {
            if (options)
            {
                // Form displayed with the options shown
                bOptions.Text = "Less Options";
                panelOptions.Visible = true;
                tabControl.Height = 242;
                Height = 290;
                optionMatchCase.Focus();
            }
            else
            {
                // Form displayed without the options shown
                bOptions.Text = "More Options";
                panelOptions.Visible = false;
                tabControl.Height = 172;
                Height = 220;
                textFind.Focus();
            }
        }

        // Clean up any resources being used.
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindReplaceForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabFind = new System.Windows.Forms.TabPage();
            this.tabReplace = new System.Windows.Forms.TabPage();
            this.labelFind = new System.Windows.Forms.Label();
            this.bCancel = new System.Windows.Forms.Button();
            this.textFind = new System.Windows.Forms.TextBox();
            this.bFindNext = new System.Windows.Forms.Button();
            this.labelReplace = new System.Windows.Forms.Label();
            this.textReplace = new System.Windows.Forms.TextBox();
            this.bReplaceAll = new System.Windows.Forms.Button();
            this.bReplace = new System.Windows.Forms.Button();
            this.bOptions = new System.Windows.Forms.Button();
            this.optionMatchCase = new System.Windows.Forms.CheckBox();
            this.optionMatchWhole = new System.Windows.Forms.CheckBox();
            this.panelOptions = new System.Windows.Forms.Panel();
            this.panelInput = new System.Windows.Forms.Panel();
            this.tabControl.SuspendLayout();
            this.panelOptions.SuspendLayout();
            this.panelInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabFind);
            this.tabControl.Controls.Add(this.tabReplace);
            this.tabControl.Location = new System.Drawing.Point(8, 9);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.ShowToolTips = true;
            this.tabControl.Size = new System.Drawing.Size(440, 37);
            this.tabControl.TabIndex = 0;
            this.tabControl.TabStop = false;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabFind
            // 
            this.tabFind.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabFind.Location = new System.Drawing.Point(4, 23);
            this.tabFind.Name = "tabFind";
            this.tabFind.Size = new System.Drawing.Size(432, 10);
            this.tabFind.TabIndex = 0;
            this.tabFind.Text = "Find";
            this.tabFind.ToolTipText = "Find Text";
            // 
            // tabReplace
            // 
            this.tabReplace.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabReplace.Location = new System.Drawing.Point(4, 23);
            this.tabReplace.Name = "tabReplace";
            this.tabReplace.Size = new System.Drawing.Size(432, 10);
            this.tabReplace.TabIndex = 1;
            this.tabReplace.Text = "Replace";
            this.tabReplace.ToolTipText = "Find and Replace Text";
            // 
            // labelFind
            // 
            this.labelFind.Location = new System.Drawing.Point(8, 18);
            this.labelFind.Name = "labelFind";
            this.labelFind.Size = new System.Drawing.Size(96, 27);
            this.labelFind.TabIndex = 0;
            this.labelFind.Text = "Find What:";
            // 
            // bCancel
            // 
            this.bCancel.BackColor = System.Drawing.SystemColors.Control;
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(356, 92);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(63, 27);
            this.bCancel.TabIndex = 4;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = false;
            // 
            // textFind
            // 
            this.textFind.Location = new System.Drawing.Point(112, 18);
            this.textFind.Name = "textFind";
            this.textFind.Size = new System.Drawing.Size(296, 22);
            this.textFind.TabIndex = 1;
            this.textFind.TextChanged += new System.EventHandler(this.textFind_TextChanged);
            // 
            // bFindNext
            // 
            this.bFindNext.BackColor = System.Drawing.SystemColors.Control;
            this.bFindNext.Location = new System.Drawing.Point(275, 92);
            this.bFindNext.Name = "bFindNext";
            this.bFindNext.Size = new System.Drawing.Size(75, 27);
            this.bFindNext.TabIndex = 3;
            this.bFindNext.Text = "Find Next";
            this.bFindNext.UseVisualStyleBackColor = false;
            this.bFindNext.Click += new System.EventHandler(this.bFindNext_Click);
            // 
            // labelReplace
            // 
            this.labelReplace.Location = new System.Drawing.Point(8, 55);
            this.labelReplace.Name = "labelReplace";
            this.labelReplace.Size = new System.Drawing.Size(96, 27);
            this.labelReplace.TabIndex = 0;
            this.labelReplace.Text = "Replace  With:";
            // 
            // textReplace
            // 
            this.textReplace.Location = new System.Drawing.Point(112, 55);
            this.textReplace.Name = "textReplace";
            this.textReplace.Size = new System.Drawing.Size(296, 22);
            this.textReplace.TabIndex = 2;
            this.textReplace.TextChanged += new System.EventHandler(this.textReplace_TextChanged);
            // 
            // bReplaceAll
            // 
            this.bReplaceAll.BackColor = System.Drawing.SystemColors.Control;
            this.bReplaceAll.Location = new System.Drawing.Point(194, 92);
            this.bReplaceAll.Name = "bReplaceAll";
            this.bReplaceAll.Size = new System.Drawing.Size(75, 27);
            this.bReplaceAll.TabIndex = 7;
            this.bReplaceAll.Text = "Replace All";
            this.bReplaceAll.UseVisualStyleBackColor = false;
            this.bReplaceAll.Click += new System.EventHandler(this.bReplaceAll_Click);
            // 
            // bReplace
            // 
            this.bReplace.BackColor = System.Drawing.SystemColors.Control;
            this.bReplace.Location = new System.Drawing.Point(130, 92);
            this.bReplace.Name = "bReplace";
            this.bReplace.Size = new System.Drawing.Size(58, 27);
            this.bReplace.TabIndex = 6;
            this.bReplace.Text = "Replace";
            this.bReplace.UseVisualStyleBackColor = false;
            this.bReplace.Click += new System.EventHandler(this.bReplace_Click);
            // 
            // bOptions
            // 
            this.bOptions.BackColor = System.Drawing.SystemColors.Control;
            this.bOptions.Location = new System.Drawing.Point(8, 92);
            this.bOptions.Name = "bOptions";
            this.bOptions.Size = new System.Drawing.Size(116, 27);
            this.bOptions.TabIndex = 5;
            this.bOptions.Text = "Options";
            this.bOptions.UseVisualStyleBackColor = false;
            this.bOptions.Click += new System.EventHandler(this.bOptions_Click);
            // 
            // optionMatchCase
            // 
            this.optionMatchCase.Location = new System.Drawing.Point(8, 9);
            this.optionMatchCase.Name = "optionMatchCase";
            this.optionMatchCase.Size = new System.Drawing.Size(240, 28);
            this.optionMatchCase.TabIndex = 8;
            this.optionMatchCase.Text = "Match Exact Case";
            // 
            // optionMatchWhole
            // 
            this.optionMatchWhole.Location = new System.Drawing.Point(8, 37);
            this.optionMatchWhole.Name = "optionMatchWhole";
            this.optionMatchWhole.Size = new System.Drawing.Size(240, 28);
            this.optionMatchWhole.TabIndex = 9;
            this.optionMatchWhole.Text = "Match Whole Word Only";
            // 
            // panelOptions
            // 
            this.panelOptions.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelOptions.Controls.Add(this.optionMatchCase);
            this.panelOptions.Controls.Add(this.optionMatchWhole);
            this.panelOptions.Location = new System.Drawing.Point(16, 175);
            this.panelOptions.Name = "panelOptions";
            this.panelOptions.Size = new System.Drawing.Size(424, 74);
            this.panelOptions.TabIndex = 8;
            // 
            // panelInput
            // 
            this.panelInput.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelInput.Controls.Add(this.labelFind);
            this.panelInput.Controls.Add(this.textFind);
            this.panelInput.Controls.Add(this.labelReplace);
            this.panelInput.Controls.Add(this.textReplace);
            this.panelInput.Controls.Add(this.bOptions);
            this.panelInput.Controls.Add(this.bReplace);
            this.panelInput.Controls.Add(this.bReplaceAll);
            this.panelInput.Controls.Add(this.bFindNext);
            this.panelInput.Controls.Add(this.bCancel);
            this.panelInput.Location = new System.Drawing.Point(16, 46);
            this.panelInput.Name = "panelInput";
            this.panelInput.Size = new System.Drawing.Size(424, 129);
            this.panelInput.TabIndex = 9;
            // 
            // FindReplaceForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(453, 262);
            this.Controls.Add(this.panelOptions);
            this.Controls.Add(this.panelInput);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindReplaceForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Find and Replace";
            this.tabControl.ResumeLayout(false);
            this.panelOptions.ResumeLayout(false);
            this.panelInput.ResumeLayout(false);
            this.panelInput.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        // define the visibility of the options
        // based on the user clicking the options button
        private void bOptions_Click(object sender, EventArgs e)
        {
            options = !options;
            DefineOptionsWindow(options);
        }

        // set the state of the form based on the index slected
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                findNotReplace = true;
            }
            else
            {
                findNotReplace = false;
            }

            DefineFindWindow(findNotReplace);
        }

        // replace a given text string with the replace value
        private void bReplace_Click(object sender, EventArgs e)
        {
            // find and replace the given text
            if (!FindReplaceOne(findText, replaceText, optionMatchWhole.Checked, optionMatchCase.Checked))
            {
                MessageBox.Show("All occurrences have been replaced!", "Find and Replace");
            }
        }

        // replace all occurences of a string with the replace value
        private void bReplaceAll_Click(object sender, EventArgs e)
        {
            int _found = FindReplaceAll(findText, replaceText, optionMatchWhole.Checked, optionMatchCase.Checked);

            // indicate the number of replaces found
            MessageBox.Show(string.Format("{0} occurrences replaced", _found), "Find and Replace");
        }

        // find the next occurence of the given string
        private void bFindNext_Click(object sender, EventArgs e)
        {
            // once find has completed indicate to the user success or failure
            if (!FindNext(findText, optionMatchWhole.Checked, optionMatchCase.Checked))
            {
                MessageBox.Show("No more occurrences found!", "Find and Replace");
            }
        }

        // once the text has been changed reset the ranges to be worked with
        // initially defined by the set in the constructor
        private void textFind_TextChanged(object sender, EventArgs e)
        {
            ResetTextState();
        }

        // once the text has been changed reset the ranges to be worked with
        // initially defined by the set in the constructor
        private void textReplace_TextChanged(object sender, EventArgs e)
        {
            ResetTextState();
        }

        // sets the form state based on user input for Replace
        private void ResetTextState()
        {
            // reset the range being worked with
            FindReplaceReset();

            // determine the text values
            findText = textFind.Text.Trim();
            replaceText = textReplace.Text.Trim();

            // if no find text available disable find button
            if (findText != string.Empty)
            {
                bFindNext.Enabled = true;
            }
            else
            {
                bFindNext.Enabled = false;
            }

            // if no find text available disable replace button
            if (textFind.Text.Trim() != string.Empty && replaceText != string.Empty)
            {
                bReplace.Enabled = true;
                bReplaceAll.Enabled = true;
            }
            else
            {
                bReplace.Enabled = false;
                bReplaceAll.Enabled = false;
            }
        }
    }
}