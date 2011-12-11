namespace Waveface
{
    partial class PostArea
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.labelDisplay = new System.Windows.Forms.Label();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.panelList = new System.Windows.Forms.Panel();
            this.postList = new Waveface.PostsList();
            this.panelButtom = new System.Windows.Forms.Panel();
            this.linkLabelReadMore = new System.Windows.Forms.LinkLabel();
            this.labelPostInfo = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelTop.SuspendLayout();
            this.panelList.SuspendLayout();
            this.panelButtom.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(96)))), ((int)(((byte)(0)))));
            this.panelTop.Controls.Add(this.labelDisplay);
            this.panelTop.Controls.Add(this.comboBoxType);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(386, 32);
            this.panelTop.TabIndex = 0;
            // 
            // labelDisplay
            // 
            this.labelDisplay.AutoSize = true;
            this.labelDisplay.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDisplay.ForeColor = System.Drawing.Color.White;
            this.labelDisplay.Location = new System.Drawing.Point(3, 9);
            this.labelDisplay.Name = "labelDisplay";
            this.labelDisplay.Size = new System.Drawing.Size(50, 14);
            this.labelDisplay.TabIndex = 3;
            this.labelDisplay.Text = "Display";
            this.labelDisplay.Visible = false;
            // 
            // comboBoxType
            // 
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Items.AddRange(new object[] {
            "All Posts",
            "Text",
            "Photo",
            "Web Link",
            "Rich Text",
            "Document"});
            this.comboBoxType.Location = new System.Drawing.Point(54, 6);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(82, 22);
            this.comboBoxType.TabIndex = 1;
            this.comboBoxType.Visible = false;
            this.comboBoxType.SelectedIndexChanged += new System.EventHandler(this.comboBoxType_SelectedIndexChanged);
            // 
            // panelList
            // 
            this.panelList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelList.Controls.Add(this.postList);
            this.panelList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelList.Location = new System.Drawing.Point(0, 0);
            this.panelList.Margin = new System.Windows.Forms.Padding(0);
            this.panelList.Name = "panelList";
            this.panelList.Size = new System.Drawing.Size(386, 407);
            this.panelList.TabIndex = 2;
            // 
            // postList
            // 
            this.postList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.postList.DetailView = null;
            this.postList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.postList.Font = new System.Drawing.Font("微軟正黑體", 9F);
            this.postList.Location = new System.Drawing.Point(0, 0);
            this.postList.Margin = new System.Windows.Forms.Padding(0);
            this.postList.Name = "postList";
            this.postList.Size = new System.Drawing.Size(384, 405);
            this.postList.TabIndex = 1;
            // 
            // panelButtom
            // 
            this.panelButtom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(96)))), ((int)(((byte)(0)))));
            this.panelButtom.Controls.Add(this.linkLabelReadMore);
            this.panelButtom.Controls.Add(this.labelPostInfo);
            this.panelButtom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtom.Location = new System.Drawing.Point(0, 407);
            this.panelButtom.Margin = new System.Windows.Forms.Padding(0);
            this.panelButtom.Name = "panelButtom";
            this.panelButtom.Size = new System.Drawing.Size(386, 28);
            this.panelButtom.TabIndex = 2;
            this.panelButtom.Visible = false;
            // 
            // linkLabelReadMore
            // 
            this.linkLabelReadMore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelReadMore.LinkColor = System.Drawing.Color.White;
            this.linkLabelReadMore.Location = new System.Drawing.Point(311, 4);
            this.linkLabelReadMore.Name = "linkLabelReadMore";
            this.linkLabelReadMore.Size = new System.Drawing.Size(70, 20);
            this.linkLabelReadMore.TabIndex = 1;
            this.linkLabelReadMore.TabStop = true;
            this.linkLabelReadMore.Text = "Read More";
            this.linkLabelReadMore.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkLabelReadMore.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelReadMore_LinkClicked);
            // 
            // labelPostInfo
            // 
            this.labelPostInfo.AutoSize = true;
            this.labelPostInfo.ForeColor = System.Drawing.Color.White;
            this.labelPostInfo.Location = new System.Drawing.Point(3, 7);
            this.labelPostInfo.Name = "labelPostInfo";
            this.labelPostInfo.Size = new System.Drawing.Size(17, 14);
            this.labelPostInfo.TabIndex = 0;
            this.labelPostInfo.Text = "[]";
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelList);
            this.panelMain.Controls.Add(this.panelButtom);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 32);
            this.panelMain.Margin = new System.Windows.Forms.Padding(0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(386, 435);
            this.panelMain.TabIndex = 3;
            // 
            // PostArea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "PostArea";
            this.Size = new System.Drawing.Size(386, 467);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelList.ResumeLayout(false);
            this.panelButtom.ResumeLayout(false);
            this.panelButtom.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private PostsList postList;
        private System.Windows.Forms.Panel panelList;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Panel panelButtom;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label labelPostInfo;
        private System.Windows.Forms.LinkLabel linkLabelReadMore;
        private System.Windows.Forms.Label labelDisplay;
    }
}
