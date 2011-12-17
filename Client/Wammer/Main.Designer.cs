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
            this.splitterLeft = new System.Windows.Forms.Splitter();
            this.panelLeftInfo = new System.Windows.Forms.Panel();
            this.panelMain = new System.Windows.Forms.Panel();
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
            this.changeOwnerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.timerDelayPost = new System.Windows.Forms.Timer(this.components);
            this.splitterRight = new System.Windows.Forms.Splitter();
            this.timerGetNewestPost = new System.Windows.Forms.Timer(this.components);
            this.timerFilterReadmore = new System.Windows.Forms.Timer(this.components);
            this.bgWorkerGetAllData = new System.ComponentModel.BackgroundWorker();
            this.timerReloadAllData = new System.Windows.Forms.Timer(this.components);
            this.detailView = new Waveface.DetailView();
            this.postsArea = new Waveface.PostArea();
            this.leftArea = new Waveface.LeftArea();
            this.panelTop = new Waveface.BgPanel();
            this.panelStation = new System.Windows.Forms.Panel();
            this.radioButtonStation = new System.Windows.Forms.RadioButton();
            this.radioButtonCloud = new System.Windows.Forms.RadioButton();
            this.panelLeft.SuspendLayout();
            this.panelPost.SuspendLayout();
            this.panelLeftInfo.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.mnuTray.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelStation.SuspendLayout();
            this.SuspendLayout();
            // 
            // BottomToolStripPanel
            // 
            resources.ApplyResources(this.BottomToolStripPanel, "BottomToolStripPanel");
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // TopToolStripPanel
            // 
            resources.ApplyResources(this.TopToolStripPanel, "TopToolStripPanel");
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // RightToolStripPanel
            // 
            resources.ApplyResources(this.RightToolStripPanel, "RightToolStripPanel");
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // LeftToolStripPanel
            // 
            resources.ApplyResources(this.LeftToolStripPanel, "LeftToolStripPanel");
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // ContentPanel
            // 
            resources.ApplyResources(this.ContentPanel, "ContentPanel");
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.panelPost);
            this.panelLeft.Controls.Add(this.splitterLeft);
            this.panelLeft.Controls.Add(this.panelLeftInfo);
            resources.ApplyResources(this.panelLeft, "panelLeft");
            this.panelLeft.Name = "panelLeft";
            // 
            // panelPost
            // 
            this.panelPost.Controls.Add(this.postsArea);
            resources.ApplyResources(this.panelPost, "panelPost");
            this.panelPost.Name = "panelPost";
            // 
            // splitterLeft
            // 
            resources.ApplyResources(this.splitterLeft, "splitterLeft");
            this.splitterLeft.Name = "splitterLeft";
            this.splitterLeft.TabStop = false;
            // 
            // panelLeftInfo
            // 
            this.panelLeftInfo.Controls.Add(this.leftArea);
            resources.ApplyResources(this.panelLeftInfo, "panelLeftInfo");
            this.panelLeftInfo.Name = "panelLeftInfo";
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.detailView);
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Name = "panelMain";
            // 
            // itemCountLabel
            // 
            this.itemCountLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.itemCountLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.itemCountLabel.Name = "itemCountLabel";
            this.itemCountLabel.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            resources.ApplyResources(this.itemCountLabel, "itemCountLabel");
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            resources.ApplyResources(this.toolStripStatusLabel2, "toolStripStatusLabel2");
            this.toolStripStatusLabel2.Spring = true;
            // 
            // connectedStatusLabel
            // 
            this.connectedStatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.connectedStatusLabel.Name = "connectedStatusLabel";
            resources.ApplyResources(this.connectedStatusLabel, "connectedStatusLabel");
            // 
            // connectedImageLabel
            // 
            resources.ApplyResources(this.connectedImageLabel, "connectedImageLabel");
            this.connectedImageLabel.Margin = new System.Windows.Forms.Padding(0);
            this.connectedImageLabel.Name = "connectedImageLabel";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusLabel4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            resources.ApplyResources(this.toolStripStatusLabel4, "toolStripStatusLabel4");
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemCountLabel,
            this.toolStripStatusLabel2,
            this.connectedStatusLabel,
            this.connectedImageLabel,
            this.toolStripStatusLabel4});
            this.statusStrip1.Name = "statusStrip1";
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
            this.changeOwnerMenuItem,
            this.logoutMenuItem,
            this.mnuExit});
            this.mnuTray.Name = "mnuTree";
            resources.ApplyResources(this.mnuTray, "mnuTray");
            // 
            // restoreMenuItem
            // 
            this.restoreMenuItem.Name = "restoreMenuItem";
            resources.ApplyResources(this.restoreMenuItem, "restoreMenuItem");
            this.restoreMenuItem.Click += new System.EventHandler(this.restoreMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // screenShotMenu
            // 
            this.screenShotMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.regionMenuItem,
            this.windowsMenuItem,
            this.screenMenuItem});
            this.screenShotMenu.Name = "screenShotMenu";
            resources.ApplyResources(this.screenShotMenu, "screenShotMenu");
            // 
            // regionMenuItem
            // 
            this.regionMenuItem.Name = "regionMenuItem";
            resources.ApplyResources(this.regionMenuItem, "regionMenuItem");
            this.regionMenuItem.Click += new System.EventHandler(this.regionMenuItem_Click);
            // 
            // windowsMenuItem
            // 
            this.windowsMenuItem.Name = "windowsMenuItem";
            resources.ApplyResources(this.windowsMenuItem, "windowsMenuItem");
            this.windowsMenuItem.Click += new System.EventHandler(this.windowsMenuItem_Click);
            // 
            // screenMenuItem
            // 
            this.screenMenuItem.Name = "screenMenuItem";
            resources.ApplyResources(this.screenMenuItem, "screenMenuItem");
            this.screenMenuItem.Click += new System.EventHandler(this.screenMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            // 
            // preferencesMenuItem
            // 
            this.preferencesMenuItem.Name = "preferencesMenuItem";
            resources.ApplyResources(this.preferencesMenuItem, "preferencesMenuItem");
            this.preferencesMenuItem.Click += new System.EventHandler(this.preferencesMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // changeOwnerMenuItem
            // 
            this.changeOwnerMenuItem.Name = "changeOwnerMenuItem";
            resources.ApplyResources(this.changeOwnerMenuItem, "changeOwnerMenuItem");
            this.changeOwnerMenuItem.Click += new System.EventHandler(this.changeOwnerMenuItem_Click);
            // 
            // logoutMenuItem
            // 
            this.logoutMenuItem.Name = "logoutMenuItem";
            resources.ApplyResources(this.logoutMenuItem, "logoutMenuItem");
            this.logoutMenuItem.Click += new System.EventHandler(this.logoutMenuItem_Click);
            // 
            // mnuExit
            // 
            this.mnuExit.Name = "mnuExit";
            resources.ApplyResources(this.mnuExit, "mnuExit");
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
            resources.ApplyResources(this.splitterRight, "splitterRight");
            this.splitterRight.Name = "splitterRight";
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
            // bgWorkerGetAllData
            // 
            this.bgWorkerGetAllData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerGetAllData_DoWork);
            this.bgWorkerGetAllData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorkerGetAllData_RunWorkerCompleted);
            // 
            // timerReloadAllData
            // 
            this.timerReloadAllData.Interval = 500;
            this.timerReloadAllData.Tick += new System.EventHandler(this.timerReloadAllData_Tick);
            // 
            // detailView
            // 
            this.detailView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.detailView, "detailView");
            this.detailView.MinimumSize = new System.Drawing.Size(200, 2);
            this.detailView.Name = "detailView";
            this.detailView.Post = null;
            this.detailView.User = null;
            // 
            // postsArea
            // 
            this.postsArea.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.postsArea, "postsArea");
            this.postsArea.Name = "postsArea";
            // 
            // leftArea
            // 
            this.leftArea.BackColor = System.Drawing.Color.Transparent;
            this.leftArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.leftArea, "leftArea");
            this.leftArea.Name = "leftArea";
            this.leftArea.TabStop = false;
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.Chocolate;
            this.panelTop.Controls.Add(this.panelStation);
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Name = "panelTop";
            this.panelTop.UserName = "";
            // 
            // panelStation
            // 
            this.panelStation.BackColor = System.Drawing.SystemColors.Control;
            this.panelStation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelStation.Controls.Add(this.radioButtonStation);
            this.panelStation.Controls.Add(this.radioButtonCloud);
            resources.ApplyResources(this.panelStation, "panelStation");
            this.panelStation.Name = "panelStation";
            // 
            // radioButtonStation
            // 
            resources.ApplyResources(this.radioButtonStation, "radioButtonStation");
            this.radioButtonStation.Checked = true;
            this.radioButtonStation.Name = "radioButtonStation";
            this.radioButtonStation.TabStop = true;
            this.radioButtonStation.UseVisualStyleBackColor = true;
            this.radioButtonStation.CheckedChanged += new System.EventHandler(this.radioButtonStation_CheckedChanged);
            // 
            // radioButtonCloud
            // 
            resources.ApplyResources(this.radioButtonCloud, "radioButtonCloud");
            this.radioButtonCloud.Name = "radioButtonCloud";
            this.radioButtonCloud.UseVisualStyleBackColor = true;
            this.radioButtonCloud.CheckedChanged += new System.EventHandler(this.radioButtonStation_CheckedChanged);
            // 
            // Main
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.splitterRight);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelTop);
            this.Name = "Main";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Form_Load);
            this.SizeChanged += new System.EventHandler(this.Main_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.Form_DragOver);
            this.DragLeave += new System.EventHandler(this.Form_DragLeave);
            this.panelLeft.ResumeLayout(false);
            this.panelPost.ResumeLayout(false);
            this.panelLeftInfo.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.mnuTray.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelStation.ResumeLayout(false);
            this.panelStation.PerformLayout();
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
        private BgPanel panelTop;
        private LeftArea leftArea;
        private System.Windows.Forms.ToolStripMenuItem preferencesMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.Panel panelStation;
        private System.Windows.Forms.RadioButton radioButtonStation;
        private System.Windows.Forms.RadioButton radioButtonCloud;
        private System.ComponentModel.BackgroundWorker bgWorkerGetAllData;
        private System.Windows.Forms.ToolStripMenuItem logoutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeOwnerMenuItem;
        private System.Windows.Forms.Timer timerReloadAllData;
	}
}

