namespace Waveface.SettingUI
{
    partial class PreferenceForm
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.ckbVirtualFolder = new System.Windows.Forms.CheckBox();
            this.ckbNotification = new System.Windows.Forms.CheckBox();
            this.ckbAutoStart = new System.Windows.Forms.CheckBox();
            this.tabPageWebStorage = new System.Windows.Forms.TabPage();
            this.panelLimitSpace = new System.Windows.Forms.Panel();
            this.ckbLimitDropSpace = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownGB = new System.Windows.Forms.NumericUpDown();
            this.ckbAllowDropbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageAdvanceSettings = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelDevices = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonChangeFolder = new System.Windows.Forms.Button();
            this.labelDataPath = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabPageWebStorage.SuspendLayout();
            this.panelLimitSpace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGB)).BeginInit();
            this.tabPageAdvanceSettings.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPageGeneral);
            this.tabControl.Controls.Add(this.tabPageWebStorage);
            this.tabControl.Controls.Add(this.tabPageAdvanceSettings);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(424, 247);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.ckbVirtualFolder);
            this.tabPageGeneral.Controls.Add(this.ckbNotification);
            this.tabPageGeneral.Controls.Add(this.ckbAutoStart);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 23);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(416, 220);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // ckbVirtualFolder
            // 
            this.ckbVirtualFolder.AutoSize = true;
            this.ckbVirtualFolder.Location = new System.Drawing.Point(16, 88);
            this.ckbVirtualFolder.Name = "ckbVirtualFolder";
            this.ckbVirtualFolder.Size = new System.Drawing.Size(181, 18);
            this.ckbVirtualFolder.TabIndex = 2;
            this.ckbVirtualFolder.Text = "Enable desktop virtual folder";
            this.ckbVirtualFolder.UseVisualStyleBackColor = true;
            // 
            // ckbNotification
            // 
            this.ckbNotification.AutoSize = true;
            this.ckbNotification.Location = new System.Drawing.Point(16, 52);
            this.ckbNotification.Name = "ckbNotification";
            this.ckbNotification.Size = new System.Drawing.Size(169, 18);
            this.ckbNotification.TabIndex = 1;
            this.ckbNotification.Text = "Show desktop notification";
            this.ckbNotification.UseVisualStyleBackColor = true;
            // 
            // ckbAutoStart
            // 
            this.ckbAutoStart.AutoSize = true;
            this.ckbAutoStart.Location = new System.Drawing.Point(16, 16);
            this.ckbAutoStart.Name = "ckbAutoStart";
            this.ckbAutoStart.Size = new System.Drawing.Size(237, 18);
            this.ckbAutoStart.TabIndex = 0;
            this.ckbAutoStart.Text = "Start Waveface on the system startup";
            this.ckbAutoStart.UseVisualStyleBackColor = true;
            // 
            // tabPageWebStorage
            // 
            this.tabPageWebStorage.Controls.Add(this.panelLimitSpace);
            this.tabPageWebStorage.Controls.Add(this.ckbAllowDropbox);
            this.tabPageWebStorage.Controls.Add(this.label1);
            this.tabPageWebStorage.Location = new System.Drawing.Point(4, 23);
            this.tabPageWebStorage.Name = "tabPageWebStorage";
            this.tabPageWebStorage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageWebStorage.Size = new System.Drawing.Size(416, 214);
            this.tabPageWebStorage.TabIndex = 1;
            this.tabPageWebStorage.Text = "Web Storage";
            this.tabPageWebStorage.UseVisualStyleBackColor = true;
            // 
            // panelLimitSpace
            // 
            this.panelLimitSpace.Controls.Add(this.ckbLimitDropSpace);
            this.panelLimitSpace.Controls.Add(this.label2);
            this.panelLimitSpace.Controls.Add(this.numericUpDownGB);
            this.panelLimitSpace.Enabled = false;
            this.panelLimitSpace.Location = new System.Drawing.Point(44, 87);
            this.panelLimitSpace.Name = "panelLimitSpace";
            this.panelLimitSpace.Size = new System.Drawing.Size(194, 59);
            this.panelLimitSpace.TabIndex = 5;
            // 
            // ckbLimitDropSpace
            // 
            this.ckbLimitDropSpace.AutoSize = true;
            this.ckbLimitDropSpace.Location = new System.Drawing.Point(4, 3);
            this.ckbLimitDropSpace.Name = "ckbLimitDropSpace";
            this.ckbLimitDropSpace.Size = new System.Drawing.Size(183, 18);
            this.ckbLimitDropSpace.TabIndex = 2;
            this.ckbLimitDropSpace.Text = "Limit the space usage up to:";
            this.ckbLimitDropSpace.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(95, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 14);
            this.label2.TabIndex = 4;
            this.label2.Text = "GB";
            // 
            // numericUpDownGB
            // 
            this.numericUpDownGB.DecimalPlaces = 1;
            this.numericUpDownGB.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownGB.Location = new System.Drawing.Point(41, 27);
            this.numericUpDownGB.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownGB.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.numericUpDownGB.Name = "numericUpDownGB";
            this.numericUpDownGB.Size = new System.Drawing.Size(48, 22);
            this.numericUpDownGB.TabIndex = 3;
            this.numericUpDownGB.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // ckbAllowDropbox
            // 
            this.ckbAllowDropbox.AutoSize = true;
            this.ckbAllowDropbox.Location = new System.Drawing.Point(19, 63);
            this.ckbAllowDropbox.Name = "ckbAllowDropbox";
            this.ckbAllowDropbox.Size = new System.Drawing.Size(369, 18);
            this.ckbAllowDropbox.TabIndex = 1;
            this.ckbAllowDropbox.Text = "Allow to access and save the data in [User]\'s dropbox account";
            this.ckbAllowDropbox.UseVisualStyleBackColor = true;
            this.ckbAllowDropbox.CheckedChanged += new System.EventHandler(this.ckbAllowDropbox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(386, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "Web storage can be a backup of the Waveface station. When the station id off, the" +
    " files can be accessible from anywhere.";
            // 
            // tabPageAdvanceSettings
            // 
            this.tabPageAdvanceSettings.Controls.Add(this.groupBox2);
            this.tabPageAdvanceSettings.Controls.Add(this.groupBox1);
            this.tabPageAdvanceSettings.Location = new System.Drawing.Point(4, 23);
            this.tabPageAdvanceSettings.Name = "tabPageAdvanceSettings";
            this.tabPageAdvanceSettings.Size = new System.Drawing.Size(416, 220);
            this.tabPageAdvanceSettings.TabIndex = 2;
            this.tabPageAdvanceSettings.Text = "Advance Settings";
            this.tabPageAdvanceSettings.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelDevices);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(16, 119);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(384, 85);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Connected Devices";
            // 
            // labelDevices
            // 
            this.labelDevices.AutoSize = true;
            this.labelDevices.Location = new System.Drawing.Point(143, 54);
            this.labelDevices.Name = "labelDevices";
            this.labelDevices.Size = new System.Drawing.Size(20, 16);
            this.labelDevices.TabIndex = 3;
            this.labelDevices.Text = "[]";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(16, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 14);
            this.label6.TabIndex = 2;
            this.label6.Text = "Connected device(s):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(16, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(341, 14);
            this.label5.TabIndex = 1;
            this.label5.Text = "Each account can have up to 5 devices to sccess the station";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonChangeFolder);
            this.groupBox1.Controls.Add(this.labelDataPath);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(16, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(384, 88);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Default Station Folder";
            // 
            // buttonChangeFolder
            // 
            this.buttonChangeFolder.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonChangeFolder.Location = new System.Drawing.Point(351, 49);
            this.buttonChangeFolder.Name = "buttonChangeFolder";
            this.buttonChangeFolder.Size = new System.Drawing.Size(27, 19);
            this.buttonChangeFolder.TabIndex = 2;
            this.buttonChangeFolder.Text = "...";
            this.buttonChangeFolder.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonChangeFolder.UseVisualStyleBackColor = true;
            this.buttonChangeFolder.Click += new System.EventHandler(this.buttonChangeFolder_Click);
            // 
            // labelDataPath
            // 
            this.labelDataPath.BackColor = System.Drawing.SystemColors.Control;
            this.labelDataPath.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDataPath.Location = new System.Drawing.Point(16, 51);
            this.labelDataPath.Name = "labelDataPath";
            this.labelDataPath.Size = new System.Drawing.Size(329, 17);
            this.labelDataPath.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(162, 14);
            this.label3.TabIndex = 0;
            this.label3.Text = "All the data will be saved at:";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(276, 267);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 28);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(357, 267);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 28);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // PreferenceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 303);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PreferenceForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Waveface Preference";
            this.Load += new System.EventHandler(this.PreferenceForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.tabPageWebStorage.ResumeLayout(false);
            this.tabPageWebStorage.PerformLayout();
            this.panelLimitSpace.ResumeLayout(false);
            this.panelLimitSpace.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGB)).EndInit();
            this.tabPageAdvanceSettings.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.TabPage tabPageWebStorage;
        private System.Windows.Forms.TabPage tabPageAdvanceSettings;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox ckbVirtualFolder;
        private System.Windows.Forms.CheckBox ckbNotification;
        private System.Windows.Forms.CheckBox ckbAutoStart;
        private System.Windows.Forms.CheckBox ckbLimitDropSpace;
        private System.Windows.Forms.CheckBox ckbAllowDropbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownGB;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonChangeFolder;
        private System.Windows.Forms.Label labelDataPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelLimitSpace;
        private System.Windows.Forms.Label labelDevices;
    }
}