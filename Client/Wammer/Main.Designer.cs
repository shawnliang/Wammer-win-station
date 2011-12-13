namespace Waveface
{
	partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelPost = new System.Windows.Forms.Panel();
            this.postsArea = new Waveface.PostArea();
            this.splitterLeft = new System.Windows.Forms.Splitter();
            this.panelLeftInfo = new System.Windows.Forms.Panel();
            this.leftArea = new Waveface.LeftArea();
            this.panelMain = new System.Windows.Forms.Panel();
            this.detailView = new Waveface.DetailView();
            this.itemCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.connectedStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.connectedImageLabel = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mnuTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.screenShotMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.regionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.timerDelayPost = new System.Windows.Forms.Timer(this.components);
            this.splitterRight = new System.Windows.Forms.Splitter();
            this.timerGetNewestPost = new System.Windows.Forms.Timer(this.components);
            this.timerFilterReadmore = new System.Windows.Forms.Timer(this.components);
            this.panelTop = new System.Windows.Forms.Panel();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.panelStation = new System.Windows.Forms.Panel();
            this.radioButtonStation = new System.Windows.Forms.RadioButton();
            this.radioButtonCloud = new System.Windows.Forms.RadioButton();
            this.labelName = new System.Windows.Forms.Label();
            this.pictureBoxAvatar = new System.Windows.Forms.PictureBox();
            this.bgWorkerGetAllData = new System.ComponentModel.BackgroundWorker();
            this.panelLeft.SuspendLayout();
            this.panelPost.SuspendLayout();
            this.panelLeftInfo.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.mnuTray.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.panelStation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(1078, 549);
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.panelPost);
            this.panelLeft.Controls.Add(this.splitterLeft);
            this.panelLeft.Controls.Add(this.panelLeftInfo);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 70);
            this.panelLeft.Margin = new System.Windows.Forms.Padding(0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(510, 597);
            this.panelLeft.TabIndex = 9;
            // 
            // panelPost
            // 
            this.panelPost.Controls.Add(this.postsArea);
            this.panelPost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPost.Location = new System.Drawing.Point(182, 0);
            this.panelPost.Margin = new System.Windows.Forms.Padding(0);
            this.panelPost.Name = "panelPost";
            this.panelPost.Size = new System.Drawing.Size(328, 597);
            this.panelPost.TabIndex = 7;
            // 
            // postsArea
            // 
            this.postsArea.BackColor = System.Drawing.SystemColors.Window;
            this.postsArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.postsArea.Font = new System.Drawing.Font("Tahoma", 9F);
            this.postsArea.Location = new System.Drawing.Point(0, 0);
            this.postsArea.Margin = new System.Windows.Forms.Padding(0);
            this.postsArea.Name = "postsArea";
            this.postsArea.Size = new System.Drawing.Size(328, 597);
            this.postsArea.TabIndex = 4;
            // 
            // splitterLeft
            // 
            this.splitterLeft.Location = new System.Drawing.Point(180, 0);
            this.splitterLeft.Margin = new System.Windows.Forms.Padding(0);
            this.splitterLeft.Name = "splitterLeft";
            this.splitterLeft.Size = new System.Drawing.Size(2, 597);
            this.splitterLeft.TabIndex = 5;
            this.splitterLeft.TabStop = false;
            // 
            // panelLeftInfo
            // 
            this.panelLeftInfo.Controls.Add(this.leftArea);
            this.panelLeftInfo.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeftInfo.Location = new System.Drawing.Point(0, 0);
            this.panelLeftInfo.Margin = new System.Windows.Forms.Padding(0);
            this.panelLeftInfo.Name = "panelLeftInfo";
            this.panelLeftInfo.Size = new System.Drawing.Size(180, 597);
            this.panelLeftInfo.TabIndex = 6;
            // 
            // leftArea
            // 
            this.leftArea.BackColor = System.Drawing.Color.Transparent;
            this.leftArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.leftArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftArea.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.leftArea.Location = new System.Drawing.Point(0, 0);
            this.leftArea.Name = "leftArea";
            this.leftArea.Size = new System.Drawing.Size(180, 597);
            this.leftArea.TabIndex = 3;
            this.leftArea.TabStop = false;
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.detailView);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(512, 70);
            this.panelMain.Margin = new System.Windows.Forms.Padding(0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(566, 597);
            this.panelMain.TabIndex = 11;
            // 
            // detailView
            // 
            this.detailView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.detailView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailView.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detailView.Location = new System.Drawing.Point(0, 0);
            this.detailView.Margin = new System.Windows.Forms.Padding(0);
            this.detailView.MinimumSize = new System.Drawing.Size(200, 2);
            this.detailView.Name = "detailView";
            this.detailView.Post = null;
            this.detailView.Size = new System.Drawing.Size(566, 597);
            this.detailView.TabIndex = 8;
            this.detailView.User = null;
            // 
            // itemCountLabel
            // 
            this.itemCountLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.itemCountLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.itemCountLabel.Name = "itemCountLabel";
            this.itemCountLabel.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.itemCountLabel.Size = new System.Drawing.Size(64, 20);
            this.itemCountLabel.Text = "{0} Items";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(739, 20);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // connectedStatusLabel
            // 
            this.connectedStatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.connectedStatusLabel.Name = "connectedStatusLabel";
            this.connectedStatusLabel.Size = new System.Drawing.Size(154, 20);
            this.connectedStatusLabel.Text = "All folders are up to date.";
            // 
            // connectedImageLabel
            // 
            this.connectedImageLabel.Image = ((System.Drawing.Image)(resources.GetObject("connectedImageLabel.Image")));
            this.connectedImageLabel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.connectedImageLabel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.connectedImageLabel.Margin = new System.Windows.Forms.Padding(0);
            this.connectedImageLabel.Name = "connectedImageLabel";
            this.connectedImageLabel.Size = new System.Drawing.Size(102, 25);
            this.connectedImageLabel.Text = "Connected";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusLabel4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(4, 20);
            this.toolStripStatusLabel4.Text = "toolStripStatusLabel4";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemCountLabel,
            this.toolStripStatusLabel2,
            this.connectedStatusLabel,
            this.connectedImageLabel,
            this.toolStripStatusLabel4});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1078, 25);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "19 Items";
            // 
            // mnuTray
            // 
            this.mnuTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreMenuItem,
            this.toolStripMenuItem1,
            this.screenShotMenu,
            this.toolStripMenuItem3,
            this.preferencesMenuItem,
            this.toolStripMenuItem2,
            this.mnuExit});
            this.mnuTray.Name = "mnuTree";
            this.mnuTray.Size = new System.Drawing.Size(153, 132);
            // 
            // restoreMenuItem
            // 
            this.restoreMenuItem.Name = "restoreMenuItem";
            this.restoreMenuItem.Size = new System.Drawing.Size(152, 22);
            this.restoreMenuItem.Text = "Restore";
            this.restoreMenuItem.Click += new System.EventHandler(this.restoreMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // screenShotMenu
            // 
            this.screenShotMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.regionMenuItem,
            this.windowsMenuItem,
            this.screenMenuItem});
            this.screenShotMenu.Name = "screenShotMenu";
            this.screenShotMenu.Size = new System.Drawing.Size(152, 22);
            this.screenShotMenu.Text = "Screen Shot";
            // 
            // regionMenuItem
            // 
            this.regionMenuItem.Name = "regionMenuItem";
            this.regionMenuItem.Size = new System.Drawing.Size(128, 22);
            this.regionMenuItem.Text = "Region";
            this.regionMenuItem.Click += new System.EventHandler(this.regionMenuItem_Click);
            // 
            // windowsMenuItem
            // 
            this.windowsMenuItem.Name = "windowsMenuItem";
            this.windowsMenuItem.Size = new System.Drawing.Size(128, 22);
            this.windowsMenuItem.Text = "Windows";
            this.windowsMenuItem.Click += new System.EventHandler(this.windowsMenuItem_Click);
            // 
            // screenMenuItem
            // 
            this.screenMenuItem.Name = "screenMenuItem";
            this.screenMenuItem.Size = new System.Drawing.Size(128, 22);
            this.screenMenuItem.Text = "Desktop";
            this.screenMenuItem.Click += new System.EventHandler(this.screenMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(149, 6);
            // 
            // preferencesMenuItem
            // 
            this.preferencesMenuItem.Name = "preferencesMenuItem";
            this.preferencesMenuItem.Size = new System.Drawing.Size(152, 22);
            this.preferencesMenuItem.Text = "Preferences...";
            this.preferencesMenuItem.Click += new System.EventHandler(this.preferencesMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(149, 6);
            // 
            // mnuExit
            // 
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(152, 22);
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.OnMenuExitClick);
            // 
            // timerDelayPost
            // 
            this.timerDelayPost.Enabled = true;
            this.timerDelayPost.Interval = 500;
            this.timerDelayPost.Tick += new System.EventHandler(this.timerDelayPost_Tick);
            // 
            // splitterRight
            // 
            this.splitterRight.Location = new System.Drawing.Point(510, 70);
            this.splitterRight.Margin = new System.Windows.Forms.Padding(0);
            this.splitterRight.Name = "splitterRight";
            this.splitterRight.Size = new System.Drawing.Size(2, 597);
            this.splitterRight.TabIndex = 10;
            this.splitterRight.TabStop = false;
            // 
            // timerGetNewestPost
            // 
            this.timerGetNewestPost.Enabled = true;
            this.timerGetNewestPost.Interval = 10000;
            this.timerGetNewestPost.Tick += new System.EventHandler(this.timerGetNewestPost_Tick);
            // 
            // timerFilterReadmore
            // 
            this.timerFilterReadmore.Interval = 500;
            this.timerFilterReadmore.Tick += new System.EventHandler(this.timerFilterReadmore_Tick);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.Chocolate;
            this.panelTop.Controls.Add(this.pictureBoxLogo);
            this.panelTop.Controls.Add(this.panelStation);
            this.panelTop.Controls.Add(this.labelName);
            this.panelTop.Controls.Add(this.pictureBoxAvatar);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1078, 70);
            this.panelTop.TabIndex = 12;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = global::Waveface.Properties.Resources.desktop_logo;
            this.pictureBoxLogo.Location = new System.Drawing.Point(11, 9);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(200, 52);
            this.pictureBoxLogo.TabIndex = 5;
            this.pictureBoxLogo.TabStop = false;
            // 
            // panelStation
            // 
            this.panelStation.BackColor = System.Drawing.SystemColors.Control;
            this.panelStation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelStation.Controls.Add(this.radioButtonStation);
            this.panelStation.Controls.Add(this.radioButtonCloud);
            this.panelStation.Location = new System.Drawing.Point(275, 12);
            this.panelStation.Name = "panelStation";
            this.panelStation.Size = new System.Drawing.Size(94, 47);
            this.panelStation.TabIndex = 4;
            this.panelStation.Visible = false;
            // 
            // radioButtonStation
            // 
            this.radioButtonStation.AutoSize = true;
            this.radioButtonStation.Checked = true;
            this.radioButtonStation.Location = new System.Drawing.Point(18, 22);
            this.radioButtonStation.Name = "radioButtonStation";
            this.radioButtonStation.Size = new System.Drawing.Size(64, 18);
            this.radioButtonStation.TabIndex = 1;
            this.radioButtonStation.TabStop = true;
            this.radioButtonStation.Text = "Station";
            this.radioButtonStation.UseVisualStyleBackColor = true;
            this.radioButtonStation.CheckedChanged += new System.EventHandler(this.radioButtonStation_CheckedChanged);
            // 
            // radioButtonCloud
            // 
            this.radioButtonCloud.AutoSize = true;
            this.radioButtonCloud.Location = new System.Drawing.Point(18, 3);
            this.radioButtonCloud.Name = "radioButtonCloud";
            this.radioButtonCloud.Size = new System.Drawing.Size(55, 18);
            this.radioButtonCloud.TabIndex = 0;
            this.radioButtonCloud.Text = "Cloud";
            this.radioButtonCloud.UseVisualStyleBackColor = true;
            this.radioButtonCloud.CheckedChanged += new System.EventHandler(this.radioButtonStation_CheckedChanged);
            // 
            // labelName
            // 
            this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelName.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.ForeColor = System.Drawing.Color.Black;
            this.labelName.Location = new System.Drawing.Point(884, 21);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(120, 25);
            this.labelName.TabIndex = 2;
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pictureBoxAvatar
            // 
            this.pictureBoxAvatar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxAvatar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxAvatar.Location = new System.Drawing.Point(1012, 5);
            this.pictureBoxAvatar.Name = "pictureBoxAvatar";
            this.pictureBoxAvatar.Size = new System.Drawing.Size(60, 60);
            this.pictureBoxAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxAvatar.TabIndex = 1;
            this.pictureBoxAvatar.TabStop = false;
            // 
            // bgWorkerGetAllData
            // 
            this.bgWorkerGetAllData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerGetAllData_DoWork);
            this.bgWorkerGetAllData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorkerGetAllData_RunWorkerCompleted);
            // 
            // Main
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1078, 667);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.splitterRight);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "Waveface";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.Form_DragOver);
            this.DragLeave += new System.EventHandler(this.Form_DragLeave);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.panelLeft.ResumeLayout(false);
            this.panelPost.ResumeLayout(false);
            this.panelLeftInfo.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.mnuTray.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.panelStation.ResumeLayout(false);
            this.panelStation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion
        
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private DetailView detailView;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Splitter splitterLeft;
        private System.Windows.Forms.Splitter splitterRight;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelPost;
        private System.Windows.Forms.Panel panelLeftInfo;
        private PostArea postsArea;
        private System.Windows.Forms.ToolStripStatusLabel itemCountLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel connectedStatusLabel;
        private System.Windows.Forms.ToolStripSplitButton connectedImageLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ContextMenuStrip mnuTray;
        private System.Windows.Forms.ToolStripMenuItem screenShotMenu;
        private System.Windows.Forms.ToolStripMenuItem regionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screenMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.ToolStripMenuItem restoreMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.Timer timerDelayPost;
        private System.Windows.Forms.Timer timerGetNewestPost;
        private System.Windows.Forms.Timer timerFilterReadmore;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.PictureBox pictureBoxAvatar;
        private System.Windows.Forms.Label labelName;
        private LeftArea leftArea;
        private System.Windows.Forms.ToolStripMenuItem preferencesMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.Panel panelStation;
        private System.Windows.Forms.RadioButton radioButtonStation;
        private System.Windows.Forms.RadioButton radioButtonCloud;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.ComponentModel.BackgroundWorker bgWorkerGetAllData;
	}
}

