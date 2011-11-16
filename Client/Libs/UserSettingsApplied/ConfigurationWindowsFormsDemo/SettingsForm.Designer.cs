namespace Waveface.Solutions.Community.ConfigurationWindowsFormsDemo
{

	partial class SettingsForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.itemsList = new Waveface.Solutions.Community.ConfigurationWindowsFormsDemo.SettingListBox();
            this.panelSplitter = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.collectedSettingsButton = new System.Windows.Forms.Button();
            this.dataGridViewSettngsButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.selectedItemLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.openUserConfigButton = new System.Windows.Forms.Button();
            this.userConfigNameLabel = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.hostEdit = new System.Windows.Forms.TextBox();
            this.hostLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.colorButton = new System.Windows.Forms.Button();
            this.fontButton = new System.Windows.Forms.Button();
            this.saveSettingsButton = new System.Windows.Forms.Button();
            this.saveOnCloseOption = new System.Windows.Forms.CheckBox();
            this.reloadSettingsButton = new System.Windows.Forms.Button();
            this.alignmentLeftOption = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.resetSettingsButton = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.splitterPositionLabel = new System.Windows.Forms.Label();
            this.alignmentCenterOption = new System.Windows.Forms.RadioButton();
            this.fontText = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.colorText = new System.Windows.Forms.Label();
            this.alignmentRightOption = new System.Windows.Forms.RadioButton();
            this.spltterPositionText = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.windowStateLabel = new System.Windows.Forms.Label();
            this.windowLocationLabel = new System.Windows.Forms.Label();
            this.windowSizeLabel = new System.Windows.Forms.Label();
            this.formToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.colorSetupDialog = new System.Windows.Forms.ColorDialog();
            this.fontSetupDialog = new System.Windows.Forms.FontDialog();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.itemsList);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(141, 443);
            this.panel1.TabIndex = 0;
            // 
            // itemsList
            // 
            this.itemsList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.itemsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemsList.FormattingEnabled = true;
            this.itemsList.ItemHeight = 14;
            this.itemsList.Location = new System.Drawing.Point(0, 0);
            this.itemsList.Margin = new System.Windows.Forms.Padding(0);
            this.itemsList.Name = "itemsList";
            this.itemsList.Size = new System.Drawing.Size(141, 443);
            this.itemsList.TabIndex = 0;
            this.itemsList.SelectedIndexChanged += new System.EventHandler(this.ItemChanged);
            // 
            // panelSplitter
            // 
            this.panelSplitter.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelSplitter.Location = new System.Drawing.Point(141, 0);
            this.panelSplitter.Name = "panelSplitter";
            this.panelSplitter.Size = new System.Drawing.Size(3, 443);
            this.panelSplitter.TabIndex = 1;
            this.panelSplitter.TabStop = false;
            this.panelSplitter.SplitterMoving += new System.Windows.Forms.SplitterEventHandler(this.SplitterMoving);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.collectedSettingsButton);
            this.panel2.Controls.Add(this.dataGridViewSettngsButton);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.closeButton);
            this.panel2.Controls.Add(this.groupBox4);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(144, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(575, 443);
            this.panel2.TabIndex = 2;
            // 
            // collectedSettingsButton
            // 
            this.collectedSettingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.collectedSettingsButton.Location = new System.Drawing.Point(190, 408);
            this.collectedSettingsButton.Name = "collectedSettingsButton";
            this.collectedSettingsButton.Size = new System.Drawing.Size(117, 24);
            this.collectedSettingsButton.TabIndex = 16;
            this.collectedSettingsButton.Text = "Collected Settings";
            this.collectedSettingsButton.UseVisualStyleBackColor = true;
            this.collectedSettingsButton.Click += new System.EventHandler(this.CollectedSettings);
            // 
            // dataGridViewSettngsButton
            // 
            this.dataGridViewSettngsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewSettngsButton.Location = new System.Drawing.Point(314, 408);
            this.dataGridViewSettngsButton.Name = "dataGridViewSettngsButton";
            this.dataGridViewSettngsButton.Size = new System.Drawing.Size(117, 24);
            this.dataGridViewSettngsButton.TabIndex = 16;
            this.dataGridViewSettngsButton.Text = "DataGridView";
            this.formToolTip.SetToolTip(this.dataGridViewSettngsButton, "Save/Load DataGridView column layout");
            this.dataGridViewSettngsButton.UseVisualStyleBackColor = true;
            this.dataGridViewSettngsButton.Click += new System.EventHandler(this.GridViewSettings);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.selectedItemLabel);
            this.groupBox2.Location = new System.Drawing.Point(10, 203);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(555, 47);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Control Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 14);
            this.label2.TabIndex = 6;
            this.label2.Text = "Selected Item:";
            // 
            // selectedItemLabel
            // 
            this.selectedItemLabel.AutoSize = true;
            this.selectedItemLabel.Location = new System.Drawing.Point(117, 21);
            this.selectedItemLabel.Name = "selectedItemLabel";
            this.selectedItemLabel.Size = new System.Drawing.Size(36, 14);
            this.selectedItemLabel.TabIndex = 7;
            this.selectedItemLabel.Text = "index";
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Location = new System.Drawing.Point(437, 408);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(117, 24);
            this.closeButton.TabIndex = 14;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.Close);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.openUserConfigButton);
            this.groupBox4.Controls.Add(this.userConfigNameLabel);
            this.groupBox4.Location = new System.Drawing.Point(10, 316);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(555, 82);
            this.groupBox4.TabIndex = 12;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "User Configuration";
            // 
            // openUserConfigButton
            // 
            this.openUserConfigButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openUserConfigButton.Location = new System.Drawing.Point(429, 21);
            this.openUserConfigButton.Name = "openUserConfigButton";
            this.openUserConfigButton.Size = new System.Drawing.Size(117, 24);
            this.openUserConfigButton.TabIndex = 2;
            this.openUserConfigButton.Text = "Open";
            this.formToolTip.SetToolTip(this.openUserConfigButton, "Open the user.config file");
            this.openUserConfigButton.UseVisualStyleBackColor = true;
            this.openUserConfigButton.Click += new System.EventHandler(this.OpenUserConfig);
            // 
            // userConfigNameLabel
            // 
            this.userConfigNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.userConfigNameLabel.Location = new System.Drawing.Point(13, 21);
            this.userConfigNameLabel.Name = "userConfigNameLabel";
            this.userConfigNameLabel.Size = new System.Drawing.Size(407, 48);
            this.userConfigNameLabel.TabIndex = 1;
            this.userConfigNameLabel.Text = "file";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.hostEdit);
            this.groupBox3.Controls.Add(this.hostLabel);
            this.groupBox3.Location = new System.Drawing.Point(10, 257);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(555, 54);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Application Settings";
            // 
            // hostEdit
            // 
            this.hostEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hostEdit.Location = new System.Drawing.Point(110, 21);
            this.hostEdit.Name = "hostEdit";
            this.hostEdit.Size = new System.Drawing.Size(433, 22);
            this.hostEdit.TabIndex = 4;
            // 
            // hostLabel
            // 
            this.hostLabel.AutoSize = true;
            this.hostLabel.Location = new System.Drawing.Point(9, 21);
            this.hostLabel.Name = "hostLabel";
            this.hostLabel.Size = new System.Drawing.Size(40, 14);
            this.hostLabel.TabIndex = 3;
            this.hostLabel.Text = "Host: ";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.colorButton);
            this.groupBox1.Controls.Add(this.fontButton);
            this.groupBox1.Controls.Add(this.saveSettingsButton);
            this.groupBox1.Controls.Add(this.saveOnCloseOption);
            this.groupBox1.Controls.Add(this.reloadSettingsButton);
            this.groupBox1.Controls.Add(this.alignmentLeftOption);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.resetSettingsButton);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.splitterPositionLabel);
            this.groupBox1.Controls.Add(this.alignmentCenterOption);
            this.groupBox1.Controls.Add(this.fontText);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.colorText);
            this.groupBox1.Controls.Add(this.alignmentRightOption);
            this.groupBox1.Controls.Add(this.spltterPositionText);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.windowStateLabel);
            this.groupBox1.Controls.Add(this.windowLocationLabel);
            this.groupBox1.Controls.Add(this.windowSizeLabel);
            this.groupBox1.Location = new System.Drawing.Point(10, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(555, 184);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Form Settings";
            // 
            // colorButton
            // 
            this.colorButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.colorButton.Location = new System.Drawing.Point(385, 125);
            this.colorButton.Name = "colorButton";
            this.colorButton.Size = new System.Drawing.Size(29, 21);
            this.colorButton.TabIndex = 16;
            this.colorButton.Text = "...";
            this.formToolTip.SetToolTip(this.colorButton, "Change color");
            this.colorButton.UseVisualStyleBackColor = true;
            this.colorButton.Click += new System.EventHandler(this.ChangeColor);
            // 
            // fontButton
            // 
            this.fontButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fontButton.Location = new System.Drawing.Point(385, 152);
            this.fontButton.Name = "fontButton";
            this.fontButton.Size = new System.Drawing.Size(29, 21);
            this.fontButton.TabIndex = 16;
            this.fontButton.Text = "...";
            this.formToolTip.SetToolTip(this.fontButton, "Change font");
            this.fontButton.UseVisualStyleBackColor = true;
            this.fontButton.Click += new System.EventHandler(this.ChangeFont);
            // 
            // saveSettingsButton
            // 
            this.saveSettingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveSettingsButton.Location = new System.Drawing.Point(431, 104);
            this.saveSettingsButton.Name = "saveSettingsButton";
            this.saveSettingsButton.Size = new System.Drawing.Size(117, 24);
            this.saveSettingsButton.TabIndex = 15;
            this.saveSettingsButton.Text = "Save Settings";
            this.formToolTip.SetToolTip(this.saveSettingsButton, "Save current form settings");
            this.saveSettingsButton.UseVisualStyleBackColor = true;
            this.saveSettingsButton.Click += new System.EventHandler(this.SaveSettings);
            // 
            // saveOnCloseOption
            // 
            this.saveOnCloseOption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveOnCloseOption.AutoSize = true;
            this.saveOnCloseOption.Checked = true;
            this.saveOnCloseOption.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saveOnCloseOption.Location = new System.Drawing.Point(451, 16);
            this.saveOnCloseOption.Name = "saveOnCloseOption";
            this.saveOnCloseOption.Size = new System.Drawing.Size(95, 16);
            this.saveOnCloseOption.TabIndex = 13;
            this.saveOnCloseOption.Text = "Save as Default";
            this.saveOnCloseOption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.formToolTip.SetToolTip(this.saveOnCloseOption, "Save form settings on close");
            this.saveOnCloseOption.UseVisualStyleBackColor = true;
            // 
            // reloadSettingsButton
            // 
            this.reloadSettingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.reloadSettingsButton.Location = new System.Drawing.Point(431, 72);
            this.reloadSettingsButton.Name = "reloadSettingsButton";
            this.reloadSettingsButton.Size = new System.Drawing.Size(117, 24);
            this.reloadSettingsButton.TabIndex = 15;
            this.reloadSettingsButton.Text = "Reload Settings";
            this.formToolTip.SetToolTip(this.reloadSettingsButton, "Reload last saved form settings");
            this.reloadSettingsButton.UseVisualStyleBackColor = true;
            this.reloadSettingsButton.Click += new System.EventHandler(this.ReloadSettings);
            // 
            // alignmentLeftOption
            // 
            this.alignmentLeftOption.AutoSize = true;
            this.alignmentLeftOption.Checked = true;
            this.alignmentLeftOption.Location = new System.Drawing.Point(110, 105);
            this.alignmentLeftOption.Name = "alignmentLeftOption";
            this.alignmentLeftOption.Size = new System.Drawing.Size(42, 16);
            this.alignmentLeftOption.TabIndex = 1;
            this.alignmentLeftOption.TabStop = true;
            this.alignmentLeftOption.Text = "Left";
            this.alignmentLeftOption.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 14);
            this.label3.TabIndex = 6;
            this.label3.Text = "Location:";
            // 
            // resetSettingsButton
            // 
            this.resetSettingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.resetSettingsButton.Location = new System.Drawing.Point(431, 41);
            this.resetSettingsButton.Name = "resetSettingsButton";
            this.resetSettingsButton.Size = new System.Drawing.Size(117, 24);
            this.resetSettingsButton.TabIndex = 15;
            this.resetSettingsButton.Text = "Reset Settings";
            this.formToolTip.SetToolTip(this.resetSettingsButton, "Reset form settings to the default");
            this.resetSettingsButton.UseVisualStyleBackColor = true;
            this.resetSettingsButton.Click += new System.EventHandler(this.ResetSettings);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 156);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(36, 14);
            this.label10.TabIndex = 0;
            this.label10.Text = "Font:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 129);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 14);
            this.label8.TabIndex = 0;
            this.label8.Text = "Color:";
            // 
            // splitterPositionLabel
            // 
            this.splitterPositionLabel.AutoSize = true;
            this.splitterPositionLabel.Location = new System.Drawing.Point(7, 87);
            this.splitterPositionLabel.Name = "splitterPositionLabel";
            this.splitterPositionLabel.Size = new System.Drawing.Size(96, 14);
            this.splitterPositionLabel.TabIndex = 0;
            this.splitterPositionLabel.Text = "Splitter Position:";
            // 
            // alignmentCenterOption
            // 
            this.alignmentCenterOption.AutoSize = true;
            this.alignmentCenterOption.Location = new System.Drawing.Point(167, 105);
            this.alignmentCenterOption.Name = "alignmentCenterOption";
            this.alignmentCenterOption.Size = new System.Drawing.Size(54, 16);
            this.alignmentCenterOption.TabIndex = 1;
            this.alignmentCenterOption.TabStop = true;
            this.alignmentCenterOption.Text = "Center";
            this.alignmentCenterOption.UseVisualStyleBackColor = true;
            // 
            // fontText
            // 
            this.fontText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fontText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.fontText.Location = new System.Drawing.Point(110, 153);
            this.fontText.Name = "fontText";
            this.fontText.Size = new System.Drawing.Size(268, 20);
            this.fontText.TabIndex = 5;
            this.fontText.Text = "{font}";
            this.fontText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 14);
            this.label5.TabIndex = 8;
            this.label5.Text = "Window State:";
            // 
            // colorText
            // 
            this.colorText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.colorText.Location = new System.Drawing.Point(110, 126);
            this.colorText.Name = "colorText";
            this.colorText.Size = new System.Drawing.Size(268, 20);
            this.colorText.TabIndex = 5;
            this.colorText.Text = "{color}";
            this.colorText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // alignmentRightOption
            // 
            this.alignmentRightOption.AutoSize = true;
            this.alignmentRightOption.Location = new System.Drawing.Point(239, 105);
            this.alignmentRightOption.Name = "alignmentRightOption";
            this.alignmentRightOption.Size = new System.Drawing.Size(49, 16);
            this.alignmentRightOption.TabIndex = 1;
            this.alignmentRightOption.TabStop = true;
            this.alignmentRightOption.Text = "Right";
            this.alignmentRightOption.UseVisualStyleBackColor = true;
            // 
            // spltterPositionText
            // 
            this.spltterPositionText.AutoSize = true;
            this.spltterPositionText.Location = new System.Drawing.Point(110, 87);
            this.spltterPositionText.Name = "spltterPositionText";
            this.spltterPositionText.Size = new System.Drawing.Size(11, 14);
            this.spltterPositionText.TabIndex = 5;
            this.spltterPositionText.Text = "-";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 14);
            this.label4.TabIndex = 6;
            this.label4.Text = "Size:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 106);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 14);
            this.label1.TabIndex = 2;
            this.label1.Text = "Alignment:";
            // 
            // windowStateLabel
            // 
            this.windowStateLabel.AutoSize = true;
            this.windowStateLabel.Location = new System.Drawing.Point(110, 55);
            this.windowStateLabel.Name = "windowStateLabel";
            this.windowStateLabel.Size = new System.Drawing.Size(35, 14);
            this.windowStateLabel.TabIndex = 7;
            this.windowStateLabel.Text = "state";
            // 
            // windowLocationLabel
            // 
            this.windowLocationLabel.AutoSize = true;
            this.windowLocationLabel.Location = new System.Drawing.Point(110, 17);
            this.windowLocationLabel.Name = "windowLocationLabel";
            this.windowLocationLabel.Size = new System.Drawing.Size(27, 14);
            this.windowLocationLabel.TabIndex = 7;
            this.windowLocationLabel.Text = "x, y";
            // 
            // windowSizeLabel
            // 
            this.windowSizeLabel.AutoSize = true;
            this.windowSizeLabel.Location = new System.Drawing.Point(110, 35);
            this.windowSizeLabel.Name = "windowSizeLabel";
            this.windowSizeLabel.Size = new System.Drawing.Size(38, 14);
            this.windowSizeLabel.TabIndex = 7;
            this.windowSizeLabel.Text = "w x h";
            // 
            // colorSetupDialog
            // 
            this.colorSetupDialog.FullOpen = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(719, 443);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelSplitter);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(697, 481);
            this.Name = "SettingsForm";
            this.Text = "User Settings Applied - Windows Forms";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter panelSplitter;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label splitterPositionLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton alignmentRightOption;
		private System.Windows.Forms.RadioButton alignmentCenterOption;
		private System.Windows.Forms.RadioButton alignmentLeftOption;
		private System.Windows.Forms.Label hostLabel;
		private System.Windows.Forms.TextBox hostEdit;
		private System.Windows.Forms.Label spltterPositionText;
		private SettingListBox itemsList;
		private System.Windows.Forms.Label selectedItemLabel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label windowStateLabel;
		private System.Windows.Forms.Label windowSizeLabel;
		private System.Windows.Forms.Label windowLocationLabel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label userConfigNameLabel;
		private System.Windows.Forms.Button resetSettingsButton;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.CheckBox saveOnCloseOption;
		private System.Windows.Forms.Button saveSettingsButton;
		private System.Windows.Forms.Button reloadSettingsButton;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ToolTip formToolTip;
		private System.Windows.Forms.Button openUserConfigButton;
		private System.Windows.Forms.Button dataGridViewSettngsButton;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label fontText;
		private System.Windows.Forms.Label colorText;
		private System.Windows.Forms.ColorDialog colorSetupDialog;
		private System.Windows.Forms.FontDialog fontSetupDialog;
		private System.Windows.Forms.Button colorButton;
		private System.Windows.Forms.Button fontButton;
		private System.Windows.Forms.Button collectedSettingsButton;
	}
}

