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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterManager));
            this.panelLeft = new System.Windows.Forms.Panel();
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.addFileButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelTimeRange = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.datePicker_TR_To = new CustomControls.DatePicker();
            this.datePicker_TR_From = new CustomControls.DatePicker();
            this.rbTimeStamp = new System.Windows.Forms.RadioButton();
            this.rbTimeRange = new System.Windows.Forms.RadioButton();
            this.panelTimeStamp = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_TS_PN = new System.Windows.Forms.ComboBox();
            this.numericUpDown_TS_Limit = new System.Windows.Forms.NumericUpDown();
            this.datePicker_TS_Time = new CustomControls.DatePicker();
            this.btnSaveUpdate = new Waveface.Component.XPButton();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label = new System.Windows.Forms.Label();
            this.panelLeft.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelTimeRange.SuspendLayout();
            this.panelTimeStamp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_TS_Limit)).BeginInit();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelLeft.Controls.Add(this.listViewFiles);
            this.panelLeft.Controls.Add(this.toolStrip);
            this.panelLeft.Location = new System.Drawing.Point(8, 8);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(240, 269);
            this.panelLeft.TabIndex = 0;
            // 
            // listViewFiles
            // 
            this.listViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.GridLines = true;
            this.listViewFiles.LargeImageList = this.imageList;
            this.listViewFiles.Location = new System.Drawing.Point(0, 25);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(236, 240);
            this.listViewFiles.SmallImageList = this.imageList;
            this.listViewFiles.TabIndex = 3;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Tile;
            this.listViewFiles.SelectedIndexChanged += new System.EventHandler(this.listViewFiles_SelectedIndexChanged);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "edit.png");
            // 
            // toolStrip
            // 
            this.toolStrip.Font = new System.Drawing.Font("Tahoma", 9F);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFileButton,
            this.toolStripSeparator1});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(236, 25);
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // panelMain
            // 
            this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelMain.Controls.Add(this.panelTimeRange);
            this.panelMain.Controls.Add(this.rbTimeStamp);
            this.panelMain.Controls.Add(this.rbTimeRange);
            this.panelMain.Controls.Add(this.panelTimeStamp);
            this.panelMain.Controls.Add(this.btnSaveUpdate);
            this.panelMain.Controls.Add(this.cmbType);
            this.panelMain.Controls.Add(this.label2);
            this.panelMain.Controls.Add(this.label1);
            this.panelMain.Controls.Add(this.tbName);
            this.panelMain.Controls.Add(this.label);
            this.panelMain.Location = new System.Drawing.Point(257, 8);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(311, 269);
            this.panelMain.TabIndex = 1;
            // 
            // panelTimeRange
            // 
            this.panelTimeRange.Controls.Add(this.label3);
            this.panelTimeRange.Controls.Add(this.datePicker_TR_To);
            this.panelTimeRange.Controls.Add(this.datePicker_TR_From);
            this.panelTimeRange.Location = new System.Drawing.Point(89, 80);
            this.panelTimeRange.Name = "panelTimeRange";
            this.panelTimeRange.Size = new System.Drawing.Size(200, 102);
            this.panelTimeRange.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "Between:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // datePicker_TR_To
            // 
            this.datePicker_TR_To.Location = new System.Drawing.Point(62, 61);
            this.datePicker_TR_To.Name = "datePicker_TR_To";
            this.datePicker_TR_To.PickerDayFont = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.datePicker_TR_To.Size = new System.Drawing.Size(114, 24);
            this.datePicker_TR_To.TabIndex = 1;
            this.datePicker_TR_To.Value = new System.DateTime(2011, 11, 11, 0, 0, 0, 0);
            // 
            // datePicker_TR_From
            // 
            this.datePicker_TR_From.Location = new System.Drawing.Point(62, 18);
            this.datePicker_TR_From.Name = "datePicker_TR_From";
            this.datePicker_TR_From.PickerDayFont = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.datePicker_TR_From.Size = new System.Drawing.Size(114, 24);
            this.datePicker_TR_From.TabIndex = 0;
            this.datePicker_TR_From.Value = new System.DateTime(2011, 11, 11, 0, 0, 0, 0);
            // 
            // rbTimeStamp
            // 
            this.rbTimeStamp.AutoSize = true;
            this.rbTimeStamp.Location = new System.Drawing.Point(169, 54);
            this.rbTimeStamp.Name = "rbTimeStamp";
            this.rbTimeStamp.Size = new System.Drawing.Size(91, 18);
            this.rbTimeStamp.TabIndex = 11;
            this.rbTimeStamp.Text = "Time Stamp";
            this.rbTimeStamp.UseVisualStyleBackColor = true;
            this.rbTimeStamp.CheckedChanged += new System.EventHandler(this.rbTime_CheckedChanged);
            // 
            // rbTimeRange
            // 
            this.rbTimeRange.AutoSize = true;
            this.rbTimeRange.Checked = true;
            this.rbTimeRange.Location = new System.Drawing.Point(73, 54);
            this.rbTimeRange.Name = "rbTimeRange";
            this.rbTimeRange.Size = new System.Drawing.Size(90, 18);
            this.rbTimeRange.TabIndex = 10;
            this.rbTimeRange.TabStop = true;
            this.rbTimeRange.Text = "Time Range";
            this.rbTimeRange.UseVisualStyleBackColor = true;
            this.rbTimeRange.CheckedChanged += new System.EventHandler(this.rbTime_CheckedChanged);
            // 
            // panelTimeStamp
            // 
            this.panelTimeStamp.Controls.Add(this.label5);
            this.panelTimeStamp.Controls.Add(this.comboBox_TS_PN);
            this.panelTimeStamp.Controls.Add(this.numericUpDown_TS_Limit);
            this.panelTimeStamp.Controls.Add(this.datePicker_TS_Time);
            this.panelTimeStamp.Location = new System.Drawing.Point(89, 80);
            this.panelTimeStamp.Name = "panelTimeStamp";
            this.panelTimeStamp.Size = new System.Drawing.Size(201, 102);
            this.panelTimeStamp.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(134, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 14);
            this.label5.TabIndex = 9;
            this.label5.Text = "Posts";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBox_TS_PN
            // 
            this.comboBox_TS_PN.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_TS_PN.FormattingEnabled = true;
            this.comboBox_TS_PN.Items.AddRange(new object[] {
            "<",
            ">"});
            this.comboBox_TS_PN.Location = new System.Drawing.Point(15, 18);
            this.comboBox_TS_PN.Name = "comboBox_TS_PN";
            this.comboBox_TS_PN.Size = new System.Drawing.Size(37, 22);
            this.comboBox_TS_PN.TabIndex = 6;
            // 
            // numericUpDown_TS_Limit
            // 
            this.numericUpDown_TS_Limit.Location = new System.Drawing.Point(58, 61);
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
            this.datePicker_TS_Time.Location = new System.Drawing.Point(58, 17);
            this.datePicker_TS_Time.Name = "datePicker_TS_Time";
            this.datePicker_TS_Time.PickerDayFont = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.datePicker_TS_Time.Size = new System.Drawing.Size(114, 24);
            this.datePicker_TS_Time.TabIndex = 7;
            this.datePicker_TS_Time.Value = new System.DateTime(2011, 11, 11, 0, 0, 0, 0);
            // 
            // btnSaveUpdate
            // 
            this.btnSaveUpdate.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnSaveUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveUpdate.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSaveUpdate.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSaveUpdate.Image = global::Waveface.Properties.Resources.disk;
            this.btnSaveUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSaveUpdate.Location = new System.Drawing.Point(222, 224);
            this.btnSaveUpdate.Name = "btnSaveUpdate";
            this.btnSaveUpdate.Size = new System.Drawing.Size(74, 32);
            this.btnSaveUpdate.TabIndex = 7;
            this.btnSaveUpdate.Text = "Save";
            this.btnSaveUpdate.UseVisualStyleBackColor = true;
            this.btnSaveUpdate.Click += new System.EventHandler(this.btnSaveUpdate_Click);
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "All",
            "Text",
            "Image",
            "Web Link"});
            this.cmbType.Location = new System.Drawing.Point(77, 188);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(79, 22);
            this.cmbType.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 191);
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
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(73, 13);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(227, 22);
            this.tbName.TabIndex = 1;
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
            // FilterManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 288);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelLeft);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(522, 326);
            this.Name = "FilterManager";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Filter Manager [New]";
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.panelTimeRange.ResumeLayout(false);
            this.panelTimeStamp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_TS_Limit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton addFileButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label2;
        private CustomControls.DatePicker datePicker_TR_To;
        private CustomControls.DatePicker datePicker_TR_From;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_TS_PN;
        private System.Windows.Forms.NumericUpDown numericUpDown_TS_Limit;
        private CustomControls.DatePicker datePicker_TS_Time;
        private System.Windows.Forms.Label label5;
        private Component.XPButton btnSaveUpdate;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.RadioButton rbTimeStamp;
        private System.Windows.Forms.RadioButton rbTimeRange;
        private System.Windows.Forms.Panel panelTimeStamp;
        private System.Windows.Forms.Panel panelTimeRange;

    }
}