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
			this.menuRelogin = new System.Windows.Forms.ToolStripMenuItem();
			this.menuServiceAction = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuPreference = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.menuQuit = new System.Windows.Forms.ToolStripMenuItem();
			this.checkStationTimer = new System.Windows.Forms.Timer(this.components);
			this.TrayMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// TrayIcon
			// 
			this.TrayIcon.ContextMenuStrip = this.TrayMenu;
			this.TrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TrayIcon.Icon")));
			this.TrayIcon.Text = "Waveface Station";
			this.TrayIcon.Visible = true;
			this.TrayIcon.DoubleClick += new System.EventHandler(this.menuPreference_Click);
			// 
			// TrayMenu
			// 
			this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRelogin,
            this.menuServiceAction,
            this.toolStripSeparator1,
            this.menuPreference,
            this.toolStripSeparator2,
            this.menuQuit});
			this.TrayMenu.Name = "TrayMenu";
			this.TrayMenu.Size = new System.Drawing.Size(146, 104);
			this.TrayMenu.Opening += new System.ComponentModel.CancelEventHandler(this.TrayMenu_Opening);
			// 
			// menuRelogin
			// 
			this.menuRelogin.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.menuRelogin.Name = "menuRelogin";
			this.menuRelogin.Size = new System.Drawing.Size(145, 22);
			this.menuRelogin.Text = "Re-Login";
			this.menuRelogin.Visible = false;
			this.menuRelogin.Click += new System.EventHandler(this.menuRelogin_Click);
			// 
			// menuServiceAction
			// 
			this.menuServiceAction.Name = "menuServiceAction";
			this.menuServiceAction.Size = new System.Drawing.Size(145, 22);
			this.menuServiceAction.Text = "Pause Service";
			this.menuServiceAction.Click += new System.EventHandler(this.menuServiceAction_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(142, 6);
			// 
			// menuPreference
			// 
			this.menuPreference.Name = "menuPreference";
			this.menuPreference.Size = new System.Drawing.Size(145, 22);
			this.menuPreference.Text = "Preference...";
			this.menuPreference.Click += new System.EventHandler(this.menuPreference_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(142, 6);
			// 
			// menuQuit
			// 
			this.menuQuit.Name = "menuQuit";
			this.menuQuit.Size = new System.Drawing.Size(145, 22);
			this.menuQuit.Text = "Quit";
			this.menuQuit.Click += new System.EventHandler(this.menuQuit_Click);
			// 
			// checkStationTimer
			// 
			this.checkStationTimer.Interval = 1000;
			this.checkStationTimer.Tick += new System.EventHandler(this.checkStationTimer_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.ShowInTaskbar = false;
			this.Text = "Waveface Station";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.TrayMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NotifyIcon TrayIcon;
		private System.Windows.Forms.ContextMenuStrip TrayMenu;
		private System.Windows.Forms.ToolStripMenuItem menuPreference;
		private System.Windows.Forms.ToolStripMenuItem menuServiceAction;
		private System.Windows.Forms.ToolStripMenuItem menuQuit;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.Timer checkStationTimer;
		private System.Windows.Forms.ToolStripMenuItem menuRelogin;
	}
}

