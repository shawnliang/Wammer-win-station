#region

using System;
using System.ComponentModel;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.HtmlEditor
{
    // Form used to enter an Html Image attribute
    // Consists of Href, Text and Image Alignment
    internal class EnterImageForm : Form
    {
        private Button bInsert;
        private Button bCancel;
        private Label labelText;
        private Label labelHref;
        private TextBox hrefText;
        private TextBox hrefLink;
        private Label labelAlign;
        private ComboBox listAlign;

        // property for the text to display
        public string ImageText
        {
            get { return hrefText.Text; }
            set { hrefText.Text = value; }
        }

        // property for the href for the image
        public string ImageLink
        {
            get { return hrefLink.Text.Trim(); }
            set { hrefLink.Text = value.Trim(); }
        }

        //property for the image align
        public ImageAlignOption ImageAlign
        {
            get { return (ImageAlignOption) listAlign.SelectedIndex; }
            set { listAlign.SelectedIndex = (int) value; }
        }

        public EnterImageForm()
        {
            InitializeComponent();

            // define the text for the alignment
            listAlign.Items.AddRange(Enum.GetNames(typeof (ImageAlignOption)));

            // ensure default value set for target
            listAlign.SelectedIndex = 4;
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
                new System.ComponentModel.ComponentResourceManager(typeof (EnterImageForm));
            this.bInsert = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.labelText = new System.Windows.Forms.Label();
            this.labelHref = new System.Windows.Forms.Label();
            this.hrefText = new System.Windows.Forms.TextBox();
            this.hrefLink = new System.Windows.Forms.TextBox();
            this.labelAlign = new System.Windows.Forms.Label();
            this.listAlign = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // bInsert
            // 
            this.bInsert.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                 ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bInsert.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bInsert.Location = new System.Drawing.Point(244, 101);
            this.bInsert.Name = "bInsert";
            this.bInsert.Size = new System.Drawing.Size(100, 27);
            this.bInsert.TabIndex = 4;
            this.bInsert.Text = "Insert Image";
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
            this.bCancel.TabIndex = 5;
            this.bCancel.Text = "Cancel";
            // 
            // labelText
            // 
            this.labelText.Location = new System.Drawing.Point(8, 46);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(50, 27);
            this.labelText.TabIndex = 3;
            this.labelText.Text = "Text:";
            // 
            // labelHref
            // 
            this.labelHref.Location = new System.Drawing.Point(8, 9);
            this.labelHref.Name = "labelHref";
            this.labelHref.Size = new System.Drawing.Size(50, 27);
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
            this.hrefText.Location = new System.Drawing.Point(64, 46);
            this.hrefText.Name = "hrefText";
            this.hrefText.Size = new System.Drawing.Size(360, 22);
            this.hrefText.TabIndex = 2;
            // 
            // hrefLink
            // 
            this.hrefLink.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                 ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                   | System.Windows.Forms.AnchorStyles.Right)));
            this.hrefLink.Location = new System.Drawing.Point(64, 9);
            this.hrefLink.Name = "hrefLink";
            this.hrefLink.Size = new System.Drawing.Size(360, 22);
            this.hrefLink.TabIndex = 1;
            // 
            // labelAlign
            // 
            this.labelAlign.Location = new System.Drawing.Point(8, 92);
            this.labelAlign.Name = "labelAlign";
            this.labelAlign.Size = new System.Drawing.Size(50, 27);
            this.labelAlign.TabIndex = 7;
            this.labelAlign.Text = "Align:";
            // 
            // listAlign
            // 
            this.listAlign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listAlign.Location = new System.Drawing.Point(64, 92);
            this.listAlign.Name = "listAlign";
            this.listAlign.Size = new System.Drawing.Size(105, 22);
            this.listAlign.TabIndex = 3;
            // 
            // EnterImageForm
            // 
            this.AcceptButton = this.bInsert;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(432, 136);
            this.Controls.Add(this.listAlign);
            this.Controls.Add(this.labelAlign);
            this.Controls.Add(this.hrefLink);
            this.Controls.Add(this.hrefText);
            this.Controls.Add(this.labelHref);
            this.Controls.Add(this.labelText);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bInsert);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnterImageForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Enter Image";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}