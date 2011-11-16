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
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.labelDisplay = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.postList = new Waveface.PostsList();
            this.panelTop.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.Gray;
            this.panelTop.Controls.Add(this.comboBoxType);
            this.panelTop.Controls.Add(this.labelDisplay);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(386, 28);
            this.panelTop.TabIndex = 0;
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
            this.comboBoxType.Location = new System.Drawing.Point(50, 4);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(82, 22);
            this.comboBoxType.TabIndex = 1;
            // 
            // labelDisplay
            // 
            this.labelDisplay.AutoSize = true;
            this.labelDisplay.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDisplay.ForeColor = System.Drawing.Color.White;
            this.labelDisplay.Location = new System.Drawing.Point(2, 7);
            this.labelDisplay.Name = "labelDisplay";
            this.labelDisplay.Size = new System.Drawing.Size(50, 14);
            this.labelDisplay.TabIndex = 0;
            this.labelDisplay.Text = "Display";
            // 
            // panelMain
            // 
            this.panelMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMain.Controls.Add(this.postList);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 28);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(386, 439);
            this.panelMain.TabIndex = 2;
            // 
            // postList
            // 
            this.postList.BackColor = System.Drawing.SystemColors.Window;
            this.postList.DetailView = null;
            this.postList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.postList.Font = new System.Drawing.Font("微軟正黑體", 9F);
            this.postList.Location = new System.Drawing.Point(0, 0);
            this.postList.Margin = new System.Windows.Forms.Padding(0);
            this.postList.Name = "postList";
            this.postList.Size = new System.Drawing.Size(384, 437);
            this.postList.TabIndex = 1;
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
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private PostsList postList;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Label labelDisplay;
    }
}
