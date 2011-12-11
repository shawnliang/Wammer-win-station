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
            this.panelCustomFilter = new System.Windows.Forms.Panel();
            this.taskPaneFilter = new XPExplorerBar.TaskPane();
            this.expandoQuicklist = new XPExplorerBar.Expando();
            this.panelTimeline = new System.Windows.Forms.Panel();
            this.buttonCreatePost = new System.Windows.Forms.Button();
            this.panelCalendar = new System.Windows.Forms.Panel();
            this.monthCalendar = new CustomControls.MonthCalendar();
            this.panelBatchPost = new System.Windows.Forms.Panel();
            this.taskPaneBatchPost = new XPExplorerBar.TaskPane();
            this.vsNetListBarGroups = new Waveface.Component.ListBarControl.VSNetListBar();
            this.imageListLarge = new System.Windows.Forms.ImageList(this.components);
            this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
            this.imageListCustomFilter = new System.Windows.Forms.ImageList(this.components);
            this.imageListTimeline = new System.Windows.Forms.ImageList(this.components);
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbHintDrop)).BeginInit();
            this.panelMain.SuspendLayout();
            this.panelFilter.SuspendLayout();
            this.panelCustomFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.taskPaneFilter)).BeginInit();
            this.taskPaneFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expandoQuicklist)).BeginInit();
            this.panelTimeline.SuspendLayout();
            this.panelCalendar.SuspendLayout();
            this.panelBatchPost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.taskPaneBatchPost)).BeginInit();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelBottom.Controls.Add(this.pbHintDrop);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 425);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(191, 182);
            this.panelBottom.TabIndex = 1;
            // 
            // pbHintDrop
            // 
            this.pbHintDrop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pbHintDrop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbHintDrop.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pbHintDrop.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pbHintDrop.Image = global::Waveface.Properties.Resources.dragNdrop_area;
            this.pbHintDrop.Location = new System.Drawing.Point(0, 0);
            this.pbHintDrop.Name = "pbHintDrop";
            this.pbHintDrop.Size = new System.Drawing.Size(191, 182);
            this.pbHintDrop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbHintDrop.TabIndex = 0;
            this.pbHintDrop.TabStop = false;
            this.pbHintDrop.Text = "Drag && drop the file to start the sharing";
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelMain.Controls.Add(this.panelFilter);
            this.panelMain.Controls.Add(this.panelCalendar);
            this.panelMain.Controls.Add(this.panelBatchPost);
            this.panelMain.Controls.Add(this.vsNetListBarGroups);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Margin = new System.Windows.Forms.Padding(0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(191, 425);
            this.panelMain.TabIndex = 2;
            // 
            // panelFilter
            // 
            this.panelFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelFilter.Controls.Add(this.panelCustomFilter);
            this.panelFilter.Controls.Add(this.panelTimeline);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFilter.Location = new System.Drawing.Point(0, 220);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(191, 205);
            this.panelFilter.TabIndex = 5;
            // 
            // panelCustomFilter
            // 
            this.panelCustomFilter.Controls.Add(this.taskPaneFilter);
            this.panelCustomFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCustomFilter.Location = new System.Drawing.Point(0, 38);
            this.panelCustomFilter.Name = "panelCustomFilter";
            this.panelCustomFilter.Size = new System.Drawing.Size(191, 167);
            this.panelCustomFilter.TabIndex = 3;
            // 
            // taskPaneFilter
            // 
            this.taskPaneFilter.AllowExpandoDragging = true;
            this.taskPaneFilter.AutoScroll = true;
            this.taskPaneFilter.AutoScrollMargin = new System.Drawing.Size(12, 12);
            this.taskPaneFilter.CustomSettings.GradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.taskPaneFilter.CustomSettings.GradientStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.taskPaneFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taskPaneFilter.Expandos.AddRange(new XPExplorerBar.Expando[] {
            this.expandoQuicklist});
            this.taskPaneFilter.Location = new System.Drawing.Point(0, 0);
            this.taskPaneFilter.Margin = new System.Windows.Forms.Padding(0);
            this.taskPaneFilter.Name = "taskPaneFilter";
            this.taskPaneFilter.Size = new System.Drawing.Size(191, 167);
            this.taskPaneFilter.TabIndex = 1;
            this.taskPaneFilter.Text = "Filter";
            this.taskPaneFilter.Visible = false;
            this.taskPaneFilter.Resize += new System.EventHandler(this.taskPaneFilter_Resize);
            // 
            // expandoQuicklist
            // 
            this.expandoQuicklist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.expandoQuicklist.Animate = true;
            this.expandoQuicklist.AutoLayout = true;
            this.expandoQuicklist.ExpandedHeight = 46;
            this.expandoQuicklist.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.expandoQuicklist.Location = new System.Drawing.Point(12, 12);
            this.expandoQuicklist.Name = "expandoQuicklist";
            this.expandoQuicklist.Size = new System.Drawing.Size(167, 46);
            this.expandoQuicklist.TabIndex = 1;
            this.expandoQuicklist.Text = "Quick list";
            this.expandoQuicklist.Visible = false;
            // 
            // panelTimeline
            // 
            this.panelTimeline.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelTimeline.Controls.Add(this.buttonCreatePost);
            this.panelTimeline.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTimeline.Location = new System.Drawing.Point(0, 0);
            this.panelTimeline.Name = "panelTimeline";
            this.panelTimeline.Size = new System.Drawing.Size(191, 38);
            this.panelTimeline.TabIndex = 2;
            // 
            // buttonCreatePost
            // 
            this.buttonCreatePost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreatePost.BackColor = System.Drawing.Color.Black;
            this.buttonCreatePost.ForeColor = System.Drawing.Color.White;
            this.buttonCreatePost.Location = new System.Drawing.Point(12, 6);
            this.buttonCreatePost.Name = "buttonCreatePost";
            this.buttonCreatePost.Size = new System.Drawing.Size(163, 26);
            this.buttonCreatePost.TabIndex = 1;
            this.buttonCreatePost.Text = "Create a New Post";
            this.buttonCreatePost.UseVisualStyleBackColor = false;
            this.buttonCreatePost.Visible = false;
            this.buttonCreatePost.Click += new System.EventHandler(this.buttonCreatePost_Click);
            // 
            // panelCalendar
            // 
            this.panelCalendar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelCalendar.Controls.Add(this.monthCalendar);
            this.panelCalendar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCalendar.Location = new System.Drawing.Point(0, 50);
            this.panelCalendar.Name = "panelCalendar";
            this.panelCalendar.Size = new System.Drawing.Size(191, 170);
            this.panelCalendar.TabIndex = 4;
            this.panelCalendar.Visible = false;
            // 
            // monthCalendar
            // 
            this.monthCalendar.ColorTable.DayText = System.Drawing.Color.DarkGray;
            this.monthCalendar.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthCalendar.Location = new System.Drawing.Point(3, 3);
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.SelectionMode = CustomControls.MonthCalendarSelectionMode.Day;
            this.monthCalendar.ShowWeekHeader = false;
            this.monthCalendar.TabIndex = 0;
            this.monthCalendar.UseShortestDayNames = true;
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
            this.taskPaneBatchPost.CustomSettings.GradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.taskPaneBatchPost.CustomSettings.GradientStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.taskPaneBatchPost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taskPaneBatchPost.Location = new System.Drawing.Point(0, 0);
            this.taskPaneBatchPost.Margin = new System.Windows.Forms.Padding(0);
            this.taskPaneBatchPost.Name = "taskPaneBatchPost";
            this.taskPaneBatchPost.Size = new System.Drawing.Size(191, 25);
            this.taskPaneBatchPost.TabIndex = 0;
            this.taskPaneBatchPost.Text = "taskPaneBatchPost";
            this.taskPaneBatchPost.Visible = false;
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
            // imageListCustomFilter
            // 
            this.imageListCustomFilter.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListCustomFilter.ImageStream")));
            this.imageListCustomFilter.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListCustomFilter.Images.SetKeyName(0, "bullet_wrench.png");
            this.imageListCustomFilter.Images.SetKeyName(1, "bullet_orange.png");
            // 
            // imageListTimeline
            // 
            this.imageListTimeline.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTimeline.ImageStream")));
            this.imageListTimeline.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListTimeline.Images.SetKeyName(0, "AllTime.png");
            this.imageListTimeline.Images.SetKeyName(1, "Month.png");
            // 
            // LeftArea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelBottom);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Name = "LeftArea";
            this.Size = new System.Drawing.Size(191, 607);
            this.panelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbHintDrop)).EndInit();
            this.panelMain.ResumeLayout(false);
            this.panelFilter.ResumeLayout(false);
            this.panelCustomFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.taskPaneFilter)).EndInit();
            this.taskPaneFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.expandoQuicklist)).EndInit();
            this.panelTimeline.ResumeLayout(false);
            this.panelCalendar.ResumeLayout(false);
            this.panelBatchPost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.taskPaneBatchPost)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.PictureBox pbHintDrop;
        private XPExplorerBar.TaskPane taskPaneBatchPost;
        private Component.ListBarControl.VSNetListBar vsNetListBarGroups;
        private System.Windows.Forms.ImageList imageListLarge;
        private System.Windows.Forms.ImageList imageListSmall;
        private System.Windows.Forms.Panel panelBatchPost;
        private System.Windows.Forms.Panel panelFilter;
        private XPExplorerBar.TaskPane taskPaneFilter;
        private XPExplorerBar.Expando expandoQuicklist;
        private System.Windows.Forms.Panel panelCalendar;
        private System.Windows.Forms.ImageList imageListCustomFilter;
        private System.Windows.Forms.ImageList imageListTimeline;
        private System.Windows.Forms.Panel panelCustomFilter;
        private System.Windows.Forms.Panel panelTimeline;
        private CustomControls.MonthCalendar monthCalendar;
        private System.Windows.Forms.Button buttonCreatePost;

    }
}
