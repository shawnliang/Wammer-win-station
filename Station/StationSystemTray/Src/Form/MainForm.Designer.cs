
namespace StationSystemTray
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuServiceAction = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiOpenStream = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.settingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemContactUs = new System.Windows.Forms.ToolStripMenuItem();
			this.menuQuit = new System.Windows.Forms.ToolStripMenuItem();
			this.checkStationTimer = new System.Windows.Forms.Timer(this.components);
			this.tabControl = new StationSystemTray.TabControlEx();
			this.tabNewOrOldUser = new System.Windows.Forms.TabPage();
			this.oldUserButton = new System.Windows.Forms.Button();
			this.newUserButton = new System.Windows.Forms.Button();
			this.TrayMenu.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabNewOrOldUser.SuspendLayout();
			this.SuspendLayout();
			// 
			// TrayIcon
			// 
			this.TrayIcon.ContextMenuStrip = this.TrayMenu;
			resources.ApplyResources(this.TrayIcon, "TrayIcon");
			this.TrayIcon.DoubleClick += new System.EventHandler(this.trayIcon_DoubleClicked);
			// 
			// TrayMenu
			// 
			this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuServiceAction,
            this.toolStripSeparator1,
            this.tsmiOpenStream,
            this.toolStripMenuItem1,
            this.settingToolStripMenuItem,
            this.toolStripSeparator3,
            this.menuItemContactUs,
            this.menuQuit});
			this.TrayMenu.Name = "TrayMenu";
			resources.ApplyResources(this.TrayMenu, "TrayMenu");
			this.TrayMenu.Opening += new System.ComponentModel.CancelEventHandler(this.TrayMenu_Opening);
			this.TrayMenu.VisibleChanged += new System.EventHandler(this.TrayMenu_VisibleChanged);
			// 
			// menuServiceAction
			// 
			this.menuServiceAction.Name = "menuServiceAction";
			resources.ApplyResources(this.menuServiceAction, "menuServiceAction");
			this.menuServiceAction.Click += new System.EventHandler(this.menuServiceAction_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// tsmiOpenStream
			// 
			this.tsmiOpenStream.Name = "tsmiOpenStream";
			resources.ApplyResources(this.tsmiOpenStream, "tsmiOpenStream");
			this.tsmiOpenStream.Click += new System.EventHandler(this.tsmiOpenStream_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
			// 
			// settingToolStripMenuItem
			// 
			this.settingToolStripMenuItem.Name = "settingToolStripMenuItem";
			resources.ApplyResources(this.settingToolStripMenuItem, "settingToolStripMenuItem");
			this.settingToolStripMenuItem.Click += new System.EventHandler(this.settingToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
			// 
			// menuItemContactUs
			// 
			this.menuItemContactUs.Name = "menuItemContactUs";
			resources.ApplyResources(this.menuItemContactUs, "menuItemContactUs");
			this.menuItemContactUs.Click += new System.EventHandler(this.menuItemContactUs_Click);
			// 
			// menuQuit
			// 
			this.menuQuit.Name = "menuQuit";
			resources.ApplyResources(this.menuQuit, "menuQuit");
			this.menuQuit.Click += new System.EventHandler(this.menuQuit_Click);
			// 
			// checkStationTimer
			// 
			this.checkStationTimer.Interval = 3000;
			this.checkStationTimer.Tick += new System.EventHandler(this.checkStationTimer_Tick);
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabNewOrOldUser);
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.HideTabs = true;
			this.tabControl.Multiline = true;
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.TabStop = false;
			// 
			// tabNewOrOldUser
			// 
			this.tabNewOrOldUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
			this.tabNewOrOldUser.Controls.Add(this.oldUserButton);
			this.tabNewOrOldUser.Controls.Add(this.newUserButton);
			resources.ApplyResources(this.tabNewOrOldUser, "tabNewOrOldUser");
			this.tabNewOrOldUser.Name = "tabNewOrOldUser";
			// 
			// oldUserButton
			// 
			resources.ApplyResources(this.oldUserButton, "oldUserButton");
			this.oldUserButton.Name = "oldUserButton";
			this.oldUserButton.UseVisualStyleBackColor = true;
			this.oldUserButton.Click += new System.EventHandler(this.oldUserButton_Click);
			// 
			// newUserButton
			// 
			resources.ApplyResources(this.newUserButton, "newUserButton");
			this.newUserButton.Name = "newUserButton";
			this.newUserButton.UseVisualStyleBackColor = true;
			this.newUserButton.Click += new System.EventHandler(this.newUserButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tabControl);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.Activated += new System.EventHandler(this.MainForm_Shown);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.TrayMenu.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.tabNewOrOldUser.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NotifyIcon TrayIcon;
		private System.Windows.Forms.ContextMenuStrip TrayMenu;
		private System.Windows.Forms.ToolStripMenuItem menuServiceAction;
		private System.Windows.Forms.ToolStripMenuItem menuQuit;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.Timer checkStationTimer;
		private TabControlEx tabControl;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem tsmiOpenStream;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem settingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuItemContactUs;
		private System.Windows.Forms.TabPage tabNewOrOldUser;
		private System.Windows.Forms.Button oldUserButton;
		private System.Windows.Forms.Button newUserButton;
	}
}

