#region

using System;
using System.ComponentModel;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.HtmlEditor
{
    // Form used to enter an Html Anchor attribute
    // Consists of Href, Text and Target Frame
    internal class EnterHrefForm : Form
    {
        private Button bInsert;
        private Button bRemove;
        private Button bCancel;
        private Label labelText;
        private Label labelHref;
        private TextBox hrefText;
        private TextBox hrefLink;
        private Label labelTarget;
        private ComboBox listTargets;

        // property for the text to display
        public string HrefText
        {
            get { return hrefText.Text; }
            set { hrefText.Text = value; }
        }

        //property for the href target
        public NavigateActionOption HrefTarget
        {
            get { return (NavigateActionOption)listTargets.SelectedIndex; }
        }

        // property for the href for the text
        public string HrefLink
        {
            get { return hrefLink.Text.Trim(); }
            set { hrefLink.Text = value.Trim(); }
        }

        public EnterHrefForm()
        {
            InitializeComponent();

            // define the text for the targets
            listTargets.Items.AddRange(Enum.GetNames(typeof (NavigateActionOption)));

            // ensure default value set for target
            listTargets.SelectedIndex = 0;
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
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof (EnterHrefForm));
            this.bInsert = new System.Windows.Forms.Button();
            this.bRemove = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.labelText = new System.Windows.Forms.Label();
            this.labelHref = new System.Windows.Forms.Label();
            this.hrefText = new System.Windows.Forms.TextBox();
            this.hrefLink = new System.Windows.Forms.TextBox();
            this.labelTarget = new System.Windows.Forms.Label();
            this.listTargets = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // bInsert
            // 
            this.bInsert.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                 ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bInsert.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.bInsert.Location = new System.Drawing.Point(184, 101);
            this.bInsert.Name = "bInsert";
            this.bInsert.Size = new System.Drawing.Size(75, 27);
            this.bInsert.TabIndex = 0;
            this.bInsert.Text = "Insert Href";
            // 
            // bRemove
            // 
            this.bRemove.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                 ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bRemove.DialogResult = System.Windows.Forms.DialogResult.No;
            this.bRemove.Location = new System.Drawing.Point(264, 101);
            this.bRemove.Name = "bRemove";
            this.bRemove.Size = new System.Drawing.Size(80, 27);
            this.bRemove.TabIndex = 1;
            this.bRemove.Text = "Remove Href";
            // 
            // bCancel
            // 
            this.bCancel.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                 ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(352, 101);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 27);
            this.bCancel.TabIndex = 2;
            this.bCancel.Text = "Cancel";
            // 
            // labelText
            // 
            this.labelText.Location = new System.Drawing.Point(8, 18);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(40, 27);
            this.labelText.TabIndex = 3;
            this.labelText.Text = "Text:";
            // 
            // labelHref
            // 
            this.labelHref.Location = new System.Drawing.Point(8, 55);
            this.labelHref.Name = "labelHref";
            this.labelHref.Size = new System.Drawing.Size(40, 27);
            this.labelHref.TabIndex = 4;
            this.labelHref.Text = "Href:";
            // 
            // hrefText
            // 
            this.hrefText.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                 ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                   | System.Windows.Forms.AnchorStyles.Right)));
            this.hrefText.Location = new System.Drawing.Point(56, 18);
            this.hrefText.Name = "hrefText";
            this.hrefText.ReadOnly = true;
            this.hrefText.Size = new System.Drawing.Size(368, 22);
            this.hrefText.TabIndex = 5;
            // 
            // hrefLink
            // 
            this.hrefLink.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                 ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                   | System.Windows.Forms.AnchorStyles.Right)));
            this.hrefLink.Location = new System.Drawing.Point(56, 55);
            this.hrefLink.Name = "hrefLink";
            this.hrefLink.Size = new System.Drawing.Size(368, 22);
            this.hrefLink.TabIndex = 6;
            // 
            // labelTarget
            // 
            this.labelTarget.Location = new System.Drawing.Point(8, 92);
            this.labelTarget.Name = "labelTarget";
            this.labelTarget.Size = new System.Drawing.Size(40, 27);
            this.labelTarget.TabIndex = 7;
            this.labelTarget.Text = "Target:";
            // 
            // listTargets
            // 
            this.listTargets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listTargets.Location = new System.Drawing.Point(56, 92);
            this.listTargets.Name = "listTargets";
            this.listTargets.Size = new System.Drawing.Size(121, 22);
            this.listTargets.TabIndex = 8;
            // 
            // EnterHrefForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(432, 136);
            this.Controls.Add(this.listTargets);
            this.Controls.Add(this.labelTarget);
            this.Controls.Add(this.hrefLink);
            this.Controls.Add(this.hrefText);
            this.Controls.Add(this.labelHref);
            this.Controls.Add(this.labelText);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bRemove);
            this.Controls.Add(this.bInsert);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnterHrefForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Enter Href";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}