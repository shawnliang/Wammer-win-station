using Waveface.Component;

namespace Waveface
{
	partial class LeftArea
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LeftArea));
            this.panelBottom = new System.Windows.Forms.Panel();
            this.pbHintDrop = new System.Windows.Forms.PictureBox();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.taskPaneFilter = new XPExplorerBar.TaskPane();
            this.expandoTimeline = new XPExplorerBar.Expando();
            this.expandoQuicklist = new XPExplorerBar.Expando();
            this.panelCalendar = new System.Windows.Forms.Panel();
            this.monthCalendar = new CustomControls.MonthCalendar();
            this.panelBatchPost = new System.Windows.Forms.Panel();
            this.taskPaneBatchPost = new XPExplorerBar.TaskPane();
            this.imageListLarge = new System.Windows.Forms.ImageList(this.components);
            this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
            this.imageListSmallFilter = new System.Windows.Forms.ImageList(this.components);
            this.AntTimer = new System.Windows.Forms.Timer(this.components);
            this.vsNetListBarGroups = new Waveface.Component.ListBarControl.VSNetListBar();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbHintDrop)).BeginInit();
            this.panelMain.SuspendLayout();
            this.panelFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.taskPaneFilter)).BeginInit();
            this.taskPaneFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expandoTimeline)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.expandoQuicklist)).BeginInit();
            this.panelCalendar.SuspendLayout();
            this.panelBatchPost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.taskPaneBatchPost)).BeginInit();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.panelBottom.Controls.Add(this.pbHintDrop);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 404);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(191, 120);
            this.panelBottom.TabIndex = 1;
            // 
            // pbHintDrop
            // 
            this.pbHintDrop.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbHintDrop.BackColor = System.Drawing.SystemColors.Info;
            this.pbHintDrop.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pbHintDrop.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pbHintDrop.Location = new System.Drawing.Point(0, 0);
            this.pbHintDrop.Name = "pbHintDrop";
            this.pbHintDrop.Size = new System.Drawing.Size(191, 120);
            this.pbHintDrop.TabIndex = 0;
            this.pbHintDrop.TabStop = false;
            this.pbHintDrop.Text = "Drag && drop the file to start the sharing";
            this.pbHintDrop.Click += new System.EventHandler(this.pbHintDrop_Click);
            this.pbHintDrop.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxHintDrop_Paint);
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.Gainsboro;
            this.panelMain.Controls.Add(this.panelFilter);
            this.panelMain.Controls.Add(this.panelCalendar);
            this.panelMain.Controls.Add(this.panelBatchPost);
            this.panelMain.Controls.Add(this.vsNetListBarGroups);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Margin = new System.Windows.Forms.Padding(0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(191, 404);
            this.panelMain.TabIndex = 2;
            // 
            // panelFilter
            // 
            this.panelFilter.BackColor = System.Drawing.SystemColors.Window;
            this.panelFilter.Controls.Add(this.taskPaneFilter);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFilter.Location = new System.Drawing.Point(0, 218);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(191, 186);
            this.panelFilter.TabIndex = 5;
            // 
            // taskPaneFilter
            // 
            this.taskPaneFilter.AllowExpandoDragging = true;
            this.taskPaneFilter.AutoScrollMargin = new System.Drawing.Size(12, 12);
            this.taskPaneFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taskPaneFilter.Expandos.AddRange(new XPExplorerBar.Expando[] {
            this.expandoTimeline,
            this.expandoQuicklist});
            this.taskPaneFilter.Location = new System.Drawing.Point(0, 0);
            this.taskPaneFilter.Margin = new System.Windows.Forms.Padding(0);
            this.taskPaneFilter.Name = "taskPaneFilter";
            this.taskPaneFilter.Size = new System.Drawing.Size(191, 186);
            this.taskPaneFilter.TabIndex = 1;
            this.taskPaneFilter.Text = "Filter";
            this.taskPaneFilter.Visible = false;
            // 
            // expandoTimeline
            // 
            this.expandoTimeline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.expandoTimeline.Animate = true;
            this.expandoTimeline.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.expandoTimeline.Location = new System.Drawing.Point(12, 12);
            this.expandoTimeline.Name = "expandoTimeline";
            this.expandoTimeline.Size = new System.Drawing.Size(167, 100);
            this.expandoTimeline.TabIndex = 0;
            this.expandoTimeline.Text = "Timeline";
            // 
            // expandoQuicklist
            // 
            this.expandoQuicklist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.expandoQuicklist.Animate = true;
            this.expandoQuicklist.Collapsed = true;
            this.expandoQuicklist.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.expandoQuicklist.Location = new System.Drawing.Point(12, 124);
            this.expandoQuicklist.Name = "expandoQuicklist";
            this.expandoQuicklist.Size = new System.Drawing.Size(167, 25);
            this.expandoQuicklist.TabIndex = 1;
            this.expandoQuicklist.Text = "Quick list";
            // 
            // panelCalendar
            // 
            this.panelCalendar.BackColor = System.Drawing.SystemColors.Window;
            this.panelCalendar.Controls.Add(this.monthCalendar);
            this.panelCalendar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCalendar.Location = new System.Drawing.Point(0, 50);
            this.panelCalendar.Name = "panelCalendar";
            this.panelCalendar.Size = new System.Drawing.Size(191, 168);
            this.panelCalendar.TabIndex = 4;
            // 
            // monthCalendar
            // 
            this.monthCalendar.ColorTable.DayText = System.Drawing.Color.DarkGray;
            this.monthCalendar.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthCalendar.Location = new System.Drawing.Point(3, 3);
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.SelectionMode = CustomControls.MonthCalendarSelectionMode.Day;
            this.monthCalendar.SelectionRange = new System.Windows.Forms.SelectionRange(new System.DateTime(2011, 11, 15, 0, 0, 0, 0), new System.DateTime(2011, 11, 15, 0, 0, 0, 0));
            this.monthCalendar.ShowWeekHeader = false;
            this.monthCalendar.TabIndex = 0;
            this.monthCalendar.UseShortestDayNames = true;
            this.monthCalendar.Visible = false;
            this.monthCalendar.DateClicked += new System.EventHandler<CustomControls.DateEventArgs>(this.monthCalendar_DateClicked);
            // 
            // panelBatchPost
            // 
            this.panelBatchPost.Controls.Add(this.taskPaneBatchPost);
            this.panelBatchPost.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBatchPost.Location = new System.Drawing.Point(0, 25);
            this.panelBatchPost.Name = "panelBatchPost";
            this.panelBatchPost.Size = new System.Drawing.Size(191, 25);
            this.panelBatchPost.TabIndex = 3;
            this.panelBatchPost.Visible = false;
            // 
            // taskPaneBatchPost
            // 
            this.taskPaneBatchPost.AllowExpandoDragging = true;
            this.taskPaneBatchPost.AutoScrollMargin = new System.Drawing.Size(12, 12);
            this.taskPaneBatchPost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taskPaneBatchPost.Location = new System.Drawing.Point(0, 0);
            this.taskPaneBatchPost.Margin = new System.Windows.Forms.Padding(0);
            this.taskPaneBatchPost.Name = "taskPaneBatchPost";
            this.taskPaneBatchPost.Size = new System.Drawing.Size(191, 25);
            this.taskPaneBatchPost.TabIndex = 0;
            this.taskPaneBatchPost.Text = "taskPaneBatchPost";
            this.taskPaneBatchPost.Visible = false;
            // 
            // imageListLarge
            // 
            this.imageListLarge.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListLarge.ImageStream")));
            this.imageListLarge.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListLarge.Images.SetKeyName(0, "image.png");
            // 
            // imageListSmall
            // 
            this.imageListSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSmall.ImageStream")));
            this.imageListSmall.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListSmall.Images.SetKeyName(0, "image.png");
            // 
            // imageListSmallFilter
            // 
            this.imageListSmallFilter.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSmallFilter.ImageStream")));
            this.imageListSmallFilter.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListSmallFilter.Images.SetKeyName(0, "image.png");
            // 
            // AntTimer
            // 
            this.AntTimer.Interval = 1500;
            this.AntTimer.Tick += new System.EventHandler(this.AntTimer_Tick);
            // 
            // vsNetListBarGroups
            // 
            this.vsNetListBarGroups.AllowDragGroups = true;
            this.vsNetListBarGroups.AllowDragItems = true;
            this.vsNetListBarGroups.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.vsNetListBarGroups.Dock = System.Windows.Forms.DockStyle.Top;
            this.vsNetListBarGroups.DrawStyle = Waveface.Component.ListBarControl.ListBarDrawStyle.ListBarDrawStyleNormal;
            this.vsNetListBarGroups.LargeImageList = this.imageListLarge;
            this.vsNetListBarGroups.Location = new System.Drawing.Point(0, 0);
            this.vsNetListBarGroups.Name = "vsNetListBarGroups";
            this.vsNetListBarGroups.SelectOnMouseDown = true;
            this.vsNetListBarGroups.Size = new System.Drawing.Size(191, 25);
            this.vsNetListBarGroups.SmallImageList = this.imageListSmall;
            this.vsNetListBarGroups.TabIndex = 1;
            this.vsNetListBarGroups.ToolTip = null;
            this.vsNetListBarGroups.Visible = false;
            this.vsNetListBarGroups.SelectedGroupChanged += new System.EventHandler(this.vsNetListBarGroups_SelectedGroupChanged);
            // 
            // LeftArea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelBottom);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "LeftArea";
            this.Size = new System.Drawing.Size(191, 524);
            this.Resize += new System.EventHandler(this.LeftArea_Resize);
            this.panelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbHintDrop)).EndInit();
            this.panelMain.ResumeLayout(false);
            this.panelFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.taskPaneFilter)).EndInit();
            this.taskPaneFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.expandoTimeline)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.expandoQuicklist)).EndInit();
            this.panelCalendar.ResumeLayout(false);
            this.panelBatchPost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.taskPaneBatchPost)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.PictureBox pbHintDrop;
        private System.Windows.Forms.Timer AntTimer;
        private XPExplorerBar.TaskPane taskPaneBatchPost;
        private Component.ListBarControl.VSNetListBar vsNetListBarGroups;
        private System.Windows.Forms.ImageList imageListLarge;
        private System.Windows.Forms.ImageList imageListSmall;
        private System.Windows.Forms.Panel panelBatchPost;
        private System.Windows.Forms.Panel panelCalendar;
        private CustomControls.MonthCalendar monthCalendar;
        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.ImageList imageListSmallFilter;
        private XPExplorerBar.TaskPane taskPaneFilter;
        private XPExplorerBar.Expando expandoTimeline;
        private XPExplorerBar.Expando expandoQuicklist;

    }
}
