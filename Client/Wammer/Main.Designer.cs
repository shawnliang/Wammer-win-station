﻿namespace Waveface
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
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelPost = new System.Windows.Forms.Panel();
            this.postsArea = new Waveface.PostArea();
            this.splitterLeft = new System.Windows.Forms.Splitter();
            this.panelLeftInfo = new System.Windows.Forms.Panel();
            this.leftArea = new Waveface.LeftArea();
            this.panelMain = new System.Windows.Forms.Panel();
            this.detailView = new Waveface.DetailView();
            this.mnuTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.screenShotMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.regionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerDelayPost = new System.Windows.Forms.Timer(this.components);
            this.splitterRight = new System.Windows.Forms.Splitter();
            this.bgWorkerGetAllData = new System.ComponentModel.BackgroundWorker();
            this.timerReloadAllData = new System.Windows.Forms.Timer(this.components);
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusLabelPost = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabelUpload = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabelServiceStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabelNetwork = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerShowStatuMessage = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorkerPreloadAllImages = new System.ComponentModel.BackgroundWorker();
            this.panelTop = new Waveface.BgPanel();
            this.panelStation = new System.Windows.Forms.Panel();
            this.radioButtonStation = new System.Windows.Forms.RadioButton();
            this.radioButtonCloud = new System.Windows.Forms.RadioButton();
            this.panelLeft.SuspendLayout();
            this.panelPost.SuspendLayout();
            this.panelLeftInfo.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.mnuTray.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelStation.SuspendLayout();
            this.SuspendLayout();
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
            // postsArea
            // 
            this.postsArea.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.postsArea, "postsArea");
            this.postsArea.MinimumSize = new System.Drawing.Size(337, 2);
            this.postsArea.Name = "postsArea";
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
            // leftArea
            // 
            this.leftArea.AllowDrop = true;
            this.leftArea.BackColor = System.Drawing.Color.Transparent;
            this.leftArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.leftArea, "leftArea");
            this.leftArea.Name = "leftArea";
            this.leftArea.TabStop = false;
            // 
            // detailView
            // 
            this.detailView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.detailView, "detailView");
		    this.detailView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailView.Name = "detailView";
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.detailView);
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Name = "panelMain";
            // 
            // mnuTray
            // 
            this.mnuTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.screenShotMenu,
            this.logoutMenuItem});
            this.mnuTray.Name = "mnuTree";
            resources.ApplyResources(this.mnuTray, "mnuTray");
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
            // logoutMenuItem
            // 
            this.logoutMenuItem.Name = "logoutMenuItem";
            resources.ApplyResources(this.logoutMenuItem, "logoutMenuItem");
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
            this.splitterRight.SplitterMoving += new System.Windows.Forms.SplitterEventHandler(this.splitterRight_SplitterMoving);
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
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // statusStrip
            // 
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabelPost,
            this.StatusLabelUpload,
            this.StatusLabelServiceStatus,
            this.StatusLabelNetwork});
            this.statusStrip.Name = "statusStrip";
            // 
            // StatusLabelPost
            // 
            this.StatusLabelPost.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.StatusLabelPost.Name = "StatusLabelPost";
            resources.ApplyResources(this.StatusLabelPost, "StatusLabelPost");
            // 
            // StatusLabelUpload
            // 
            this.StatusLabelUpload.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusLabelUpload.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.StatusLabelUpload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.StatusLabelUpload.Name = "StatusLabelUpload";
            resources.ApplyResources(this.StatusLabelUpload, "StatusLabelUpload");
            this.StatusLabelUpload.Spring = true;
            // 
            // StatusLabelServiceStatus
            // 
            this.StatusLabelServiceStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusLabelServiceStatus.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.StatusLabelServiceStatus.Name = "StatusLabelServiceStatus";
            this.StatusLabelServiceStatus.Padding = new System.Windows.Forms.Padding(8, 0, 4, 0);
            resources.ApplyResources(this.StatusLabelServiceStatus, "StatusLabelServiceStatus");
            // 
            // StatusLabelNetwork
            // 
            this.StatusLabelNetwork.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusLabelNetwork.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.StatusLabelNetwork.Name = "StatusLabelNetwork";
            this.StatusLabelNetwork.Padding = new System.Windows.Forms.Padding(8, 0, 4, 0);
            resources.ApplyResources(this.StatusLabelNetwork, "StatusLabelNetwork");
            // 
            // timerShowStatuMessage
            // 
            this.timerShowStatuMessage.Interval = 6000;
            this.timerShowStatuMessage.Tick += new System.EventHandler(this.timerShowStatuMessage_Tick);
            // 
            // backgroundWorkerPreloadAllImages
            // 
            this.backgroundWorkerPreloadAllImages.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreloadAllImages_DoWork);
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
            this.Controls.Add(this.statusStrip);
            this.Name = "Main";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Activated += new System.EventHandler(this.Main_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
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
            this.mnuTray.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelStation.ResumeLayout(false);
            this.panelStation.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion
        
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private DetailView detailView;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Splitter splitterLeft;
        private System.Windows.Forms.Splitter splitterRight;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelPost;
        private System.Windows.Forms.Panel panelLeftInfo;
        private PostArea postsArea;
        private System.Windows.Forms.ContextMenuStrip mnuTray;
        private System.Windows.Forms.ToolStripMenuItem screenShotMenu;
        private System.Windows.Forms.ToolStripMenuItem regionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screenMenuItem;
        private System.Windows.Forms.Timer timerDelayPost;
        private BgPanel panelTop;
        private LeftArea leftArea;
        private System.Windows.Forms.Panel panelStation;
        private System.Windows.Forms.RadioButton radioButtonStation;
        private System.Windows.Forms.RadioButton radioButtonCloud;
        private System.ComponentModel.BackgroundWorker bgWorkerGetAllData;
        private System.Windows.Forms.ToolStripMenuItem logoutMenuItem;
        private System.Windows.Forms.Timer timerReloadAllData;
        private Localization.CultureManager cultureManager;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelUpload;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelNetwork;
        private System.Windows.Forms.Timer timerShowStatuMessage;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelPost;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreloadAllImages;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelServiceStatus;
	}
}

