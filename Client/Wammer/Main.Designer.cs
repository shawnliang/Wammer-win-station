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

				if (m_dropableNotifyIcon != null)
				{
					m_dropableNotifyIcon.Dispose();
					m_dropableNotifyIcon = null;
				}
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
			this.postsArea = new Waveface.PostArea();
			this.leftArea = new Waveface.LeftArea();
			this.panelTitle = new Waveface.TitlePanel();
			this.detailView = new Waveface.DetailView();
			this.mnuTray = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.screenShotMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.regionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.windowsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.screenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.logoutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.timerDelayPost = new System.Windows.Forms.Timer(this.components);
			this.bgWorkerGetAllData = new System.ComponentModel.BackgroundWorker();
			this.cultureManager = new Waveface.Localization.CultureManager(this.components);
			this.timerShowStatuMessage = new System.Windows.Forms.Timer(this.components);
			this.backgroundWorkerPreloadAllImages = new System.ComponentModel.BackgroundWorker();
			this.timerPolling = new System.Windows.Forms.Timer(this.components);
			this.panelLeft = new System.Windows.Forms.Panel();
			this.mnuTray.SuspendLayout();
			this.panelLeft.SuspendLayout();
			this.SuspendLayout();
			// 
			// ContentPanel
			// 
			resources.ApplyResources(this.ContentPanel, "ContentPanel");
			// 
			// postsArea
			// 
			this.postsArea.BackColor = System.Drawing.SystemColors.Window;
			resources.ApplyResources(this.postsArea, "postsArea");
			this.postsArea.Name = "postsArea";
			// 
			// leftArea
			// 
			this.leftArea.AllowDrop = true;
			this.leftArea.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.leftArea, "leftArea");
			this.leftArea.Name = "leftArea";
			this.leftArea.TabStop = false;
			// 
			// panelTitle
			// 
			this.panelTitle.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.panelTitle, "panelTitle");
			this.panelTitle.Name = "panelTitle";
			// 
			// detailView
			// 
			resources.ApplyResources(this.detailView, "detailView");
			this.detailView.Name = "detailView";
			this.detailView.Post = null;
			this.detailView.User = null;
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
			// bgWorkerGetAllData
			// 
			this.bgWorkerGetAllData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerGetAllData_DoWork);
			this.bgWorkerGetAllData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorkerGetAllData_RunWorkerCompleted);
			// 
			// cultureManager
			// 
			this.cultureManager.ManagedControl = this;
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
			// timerPolling
			// 
			this.timerPolling.Interval = 5000;
			this.timerPolling.Tick += new System.EventHandler(this.timerPolling_Tick);
			// 
			// panelLeft
			// 
			this.panelLeft.Controls.Add(this.postsArea);
			this.panelLeft.Controls.Add(this.leftArea);
			this.panelLeft.Controls.Add(this.panelTitle);
			resources.ApplyResources(this.panelLeft, "panelLeft");
			this.panelLeft.Name = "panelLeft";
			// 
			// Main
			// 
			this.AllowDrop = true;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.detailView);
			this.Controls.Add(this.panelLeft);
			this.Name = "Main";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Activated += new System.EventHandler(this.Main_Activated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Shown += new System.EventHandler(this.Main_Shown);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form_DragEnter);
			this.DragOver += new System.Windows.Forms.DragEventHandler(this.Form_DragOver);
			this.DragLeave += new System.EventHandler(this.Form_DragLeave);
			this.mnuTray.ResumeLayout(false);
			this.panelLeft.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private DetailView detailView;
        private PostArea postsArea;
        private System.Windows.Forms.ContextMenuStrip mnuTray;
        private System.Windows.Forms.ToolStripMenuItem screenShotMenu;
        private System.Windows.Forms.ToolStripMenuItem regionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screenMenuItem;
        private System.Windows.Forms.Timer timerDelayPost;
        private LeftArea leftArea;
        private System.ComponentModel.BackgroundWorker bgWorkerGetAllData;
        private System.Windows.Forms.ToolStripMenuItem logoutMenuItem;
		private Localization.CultureManager cultureManager;
		private System.Windows.Forms.Timer timerShowStatuMessage;
		private System.ComponentModel.BackgroundWorker backgroundWorkerPreloadAllImages;
        private System.Windows.Forms.Timer timerPolling;
        private TitlePanel panelTitle;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
		private System.Windows.Forms.Panel panelLeft;
	}
}

