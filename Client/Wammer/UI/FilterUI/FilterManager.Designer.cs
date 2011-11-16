namespace Waveface.FilterUI
{
    partial class FilterManager
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterManager));
            this.panelLeft = new System.Windows.Forms.Panel();
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.addFileButton = new System.Windows.Forms.ToolStripButton();
            this.removeFileButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.panelMain = new System.Windows.Forms.Panel();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageTimeRange = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.datePicker_TR_To = new CustomControls.DatePicker();
            this.datePicker_TR_From = new CustomControls.DatePicker();
            this.tabPageTimeStamp = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown_TS_Limit = new System.Windows.Forms.NumericUpDown();
            this.datePicker_TS_Time = new CustomControls.DatePicker();
            this.comboBox_TS_PN = new System.Windows.Forms.ComboBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label = new System.Windows.Forms.Label();
            this.btnUpdate = new Waveface.Component.XPButton();
            this.panelLeft.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageTimeRange.SuspendLayout();
            this.tabPageTimeStamp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_TS_Limit)).BeginInit();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelLeft.Controls.Add(this.listViewFiles);
            this.panelLeft.Controls.Add(this.toolStrip);
            this.panelLeft.Location = new System.Drawing.Point(8, 8);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(167, 260);
            this.panelLeft.TabIndex = 0;
            // 
            // listViewFiles
            // 
            this.listViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.GridLines = true;
            this.listViewFiles.Location = new System.Drawing.Point(0, 25);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(163, 231);
            this.listViewFiles.TabIndex = 3;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Tile;
            // 
            // toolStrip
            // 
            this.toolStrip.Font = new System.Drawing.Font("Tahoma", 9F);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFileButton,
            this.removeFileButton,
            this.toolStripSeparator1});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(163, 25);
            this.toolStrip.TabIndex = 2;
            // 
            // addFileButton
            // 
            this.addFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addFileButton.Image = ((System.Drawing.Image)(resources.GetObject("addFileButton.Image")));
            this.addFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addFileButton.Name = "addFileButton";
            this.addFileButton.Size = new System.Drawing.Size(23, 22);
            this.addFileButton.Text = "Add Files...";
            this.addFileButton.Click += new System.EventHandler(this.addFileButton_Click);
            // 
            // removeFileButton
            // 
            this.removeFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.removeFileButton.Image = ((System.Drawing.Image)(resources.GetObject("removeFileButton.Image")));
            this.removeFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeFileButton.Name = "removeFileButton";
            this.removeFileButton.Size = new System.Drawing.Size(23, 22);
            this.removeFileButton.Text = "Remove Selected Files";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // panelMain
            // 
            this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelMain.Controls.Add(this.btnUpdate);
            this.panelMain.Controls.Add(this.cmbType);
            this.panelMain.Controls.Add(this.label2);
            this.panelMain.Controls.Add(this.label1);
            this.panelMain.Controls.Add(this.tabControl);
            this.panelMain.Controls.Add(this.textBoxName);
            this.panelMain.Controls.Add(this.label);
            this.panelMain.Location = new System.Drawing.Point(184, 8);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(311, 258);
            this.panelMain.TabIndex = 1;
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "All",
            "Text",
            "Image",
            "Web Link",
            "Document",
            "Rich Text"});
            this.cmbType.Location = new System.Drawing.Point(73, 183);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(79, 22);
            this.cmbType.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 186);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 14);
            this.label2.TabIndex = 4;
            this.label2.Text = "Type:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 14);
            this.label1.TabIndex = 3;
            this.label1.Text = "Condition:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabControl
            // 
            this.tabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl.Controls.Add(this.tabPageTimeRange);
            this.tabControl.Controls.Add(this.tabPageTimeStamp);
            this.tabControl.Location = new System.Drawing.Point(73, 53);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(227, 120);
            this.tabControl.TabIndex = 2;
            // 
            // tabPageTimeRange
            // 
            this.tabPageTimeRange.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabPageTimeRange.Controls.Add(this.label3);
            this.tabPageTimeRange.Controls.Add(this.datePicker_TR_To);
            this.tabPageTimeRange.Controls.Add(this.datePicker_TR_From);
            this.tabPageTimeRange.Location = new System.Drawing.Point(4, 26);
            this.tabPageTimeRange.Name = "tabPageTimeRange";
            this.tabPageTimeRange.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTimeRange.Size = new System.Drawing.Size(219, 90);
            this.tabPageTimeRange.TabIndex = 0;
            this.tabPageTimeRange.Text = "Time Range";
            this.tabPageTimeRange.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(10, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "Between:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // datePicker_TR_To
            // 
            this.datePicker_TR_To.Location = new System.Drawing.Point(78, 50);
            this.datePicker_TR_To.Name = "datePicker_TR_To";
            this.datePicker_TR_To.PickerDayFont = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.datePicker_TR_To.Size = new System.Drawing.Size(114, 24);
            this.datePicker_TR_To.TabIndex = 1;
            this.datePicker_TR_To.Value = new System.DateTime(2011, 11, 11, 0, 0, 0, 0);
            // 
            // datePicker_TR_From
            // 
            this.datePicker_TR_From.Location = new System.Drawing.Point(78, 16);
            this.datePicker_TR_From.Name = "datePicker_TR_From";
            this.datePicker_TR_From.PickerDayFont = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.datePicker_TR_From.Size = new System.Drawing.Size(114, 24);
            this.datePicker_TR_From.TabIndex = 0;
            this.datePicker_TR_From.Value = new System.DateTime(2011, 11, 11, 0, 0, 0, 0);
            // 
            // tabPageTimeStamp
            // 
            this.tabPageTimeStamp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabPageTimeStamp.Controls.Add(this.label5);
            this.tabPageTimeStamp.Controls.Add(this.numericUpDown_TS_Limit);
            this.tabPageTimeStamp.Controls.Add(this.datePicker_TS_Time);
            this.tabPageTimeStamp.Controls.Add(this.comboBox_TS_PN);
            this.tabPageTimeStamp.Location = new System.Drawing.Point(4, 26);
            this.tabPageTimeStamp.Name = "tabPageTimeStamp";
            this.tabPageTimeStamp.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTimeStamp.Size = new System.Drawing.Size(219, 90);
            this.tabPageTimeStamp.TabIndex = 1;
            this.tabPageTimeStamp.Text = "Time Stamp";
            this.tabPageTimeStamp.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(154, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 14);
            this.label5.TabIndex = 9;
            this.label5.Text = "Posts";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDown_TS_Limit
            // 
            this.numericUpDown_TS_Limit.Location = new System.Drawing.Point(78, 50);
            this.numericUpDown_TS_Limit.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDown_TS_Limit.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDown_TS_Limit.Name = "numericUpDown_TS_Limit";
            this.numericUpDown_TS_Limit.Size = new System.Drawing.Size(70, 22);
            this.numericUpDown_TS_Limit.TabIndex = 8;
            this.numericUpDown_TS_Limit.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // datePicker_TS_Time
            // 
            this.datePicker_TS_Time.Location = new System.Drawing.Point(78, 16);
            this.datePicker_TS_Time.Name = "datePicker_TS_Time";
            this.datePicker_TS_Time.PickerDayFont = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.datePicker_TS_Time.Size = new System.Drawing.Size(114, 24);
            this.datePicker_TS_Time.TabIndex = 7;
            this.datePicker_TS_Time.Value = new System.DateTime(2011, 11, 11, 0, 0, 0, 0);
            // 
            // comboBox_TS_PN
            // 
            this.comboBox_TS_PN.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_TS_PN.FormattingEnabled = true;
            this.comboBox_TS_PN.Items.AddRange(new object[] {
            "<",
            ">"});
            this.comboBox_TS_PN.Location = new System.Drawing.Point(35, 17);
            this.comboBox_TS_PN.Name = "comboBox_TS_PN";
            this.comboBox_TS_PN.Size = new System.Drawing.Size(37, 22);
            this.comboBox_TS_PN.TabIndex = 6;
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(73, 13);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(227, 22);
            this.textBoxName.TabIndex = 1;
            // 
            // label
            // 
            this.label.Location = new System.Drawing.Point(5, 16);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(62, 14);
            this.label.TabIndex = 0;
            this.label.Text = "Name:";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnUpdate
            // 
            this.btnUpdate.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnUpdate.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnUpdate.Image = global::Waveface.Properties.Resources.disk;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(230, 219);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(74, 32);
            this.btnUpdate.TabIndex = 7;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            // 
            // FilterManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 280);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelLeft);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FilterManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Filter Manager";
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageTimeRange.ResumeLayout(false);
            this.tabPageTimeStamp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_TS_Limit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton addFileButton;
        private System.Windows.Forms.ToolStripButton removeFileButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageTimeRange;
        private System.Windows.Forms.TabPage tabPageTimeStamp;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label2;
        private CustomControls.DatePicker datePicker_TR_To;
        private CustomControls.DatePicker datePicker_TR_From;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_TS_PN;
        private System.Windows.Forms.NumericUpDown numericUpDown_TS_Limit;
        private CustomControls.DatePicker datePicker_TS_Time;
        private System.Windows.Forms.Label label5;
        private Component.XPButton btnUpdate;

    }
}