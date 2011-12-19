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
            this.panelButtom2 = new System.Windows.Forms.Panel();
            this.labelDropInfor = new System.Windows.Forms.Label();
            this.pbDropArea = new System.Windows.Forms.PictureBox();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.panelCustomFilter = new System.Windows.Forms.Panel();
            this.taskPaneFilter = new XPExplorerBar.TaskPane();
            this.expandoQuicklist = new XPExplorerBar.Expando();
            this.panelTimeline = new System.Windows.Forms.Panel();
            this.panelCalendar = new System.Windows.Forms.Panel();
            this.monthCalendar = new CustomControls.MonthCalendar();
            this.vsNetListBarGroups = new Waveface.Component.ListBarControl.VSNetListBar();
            this.imageListLarge = new System.Windows.Forms.ImageList(this.components);
            this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
            this.imageListCustomFilter = new System.Windows.Forms.ImageList(this.components);
            this.imageListTimeline = new System.Windows.Forms.ImageList(this.components);
            this.panelBottom.SuspendLayout();
            this.panelButtom2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDropArea)).BeginInit();
            this.panelMain.SuspendLayout();
            this.panelFilter.SuspendLayout();
            this.panelCustomFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.taskPaneFilter)).BeginInit();
            this.taskPaneFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expandoQuicklist)).BeginInit();
            this.panelCalendar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelBottom.Controls.Add(this.panelButtom2);
            this.panelBottom.Controls.Add(this.pbDropArea);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 415);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(191, 192);
            this.panelBottom.TabIndex = 1;
            // 
            // panelButtom2
            // 
            this.panelButtom2.Controls.Add(this.labelDropInfor);
            this.panelButtom2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtom2.Location = new System.Drawing.Point(0, 170);
            this.panelButtom2.Name = "panelButtom2";
            this.panelButtom2.Size = new System.Drawing.Size(191, 22);
            this.panelButtom2.TabIndex = 2;
            // 
            // labelDropInfor
            // 
            this.labelDropInfor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelDropInfor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDropInfor.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDropInfor.Location = new System.Drawing.Point(0, 0);
            this.labelDropInfor.Name = "labelDropInfor";
            this.labelDropInfor.Size = new System.Drawing.Size(191, 22);
            this.labelDropInfor.TabIndex = 1;
            this.labelDropInfor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbDropArea
            // 
            this.pbDropArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pbDropArea.BackgroundImage = global::Waveface.Properties.Resources.dragNdrop_area1;
            this.pbDropArea.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbDropArea.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbDropArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbDropArea.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pbDropArea.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pbDropArea.Location = new System.Drawing.Point(0, 0);
            this.pbDropArea.Name = "pbDropArea";
            this.pbDropArea.Size = new System.Drawing.Size(191, 192);
            this.pbDropArea.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbDropArea.TabIndex = 0;
            this.pbDropArea.TabStop = false;
            this.pbDropArea.Text = "Drag && drop the file to start the sharing";
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelMain.Controls.Add(this.panelFilter);
            this.panelMain.Controls.Add(this.panelCalendar);
            this.panelMain.Controls.Add(this.vsNetListBarGroups);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Margin = new System.Windows.Forms.Padding(0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(191, 415);
            this.panelMain.TabIndex = 2;
            // 
            // panelFilter
            // 
            this.panelFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelFilter.Controls.Add(this.panelCustomFilter);
            this.panelFilter.Controls.Add(this.panelTimeline);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFilter.Location = new System.Drawing.Point(0, 195);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(191, 220);
            this.panelFilter.TabIndex = 5;
            // 
            // panelCustomFilter
            // 
            this.panelCustomFilter.Controls.Add(this.taskPaneFilter);
            this.panelCustomFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCustomFilter.Location = new System.Drawing.Point(0, 38);
            this.panelCustomFilter.Name = "panelCustomFilter";
            this.panelCustomFilter.Size = new System.Drawing.Size(191, 182);
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
            this.taskPaneFilter.Size = new System.Drawing.Size(191, 182);
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
            this.panelTimeline.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTimeline.Location = new System.Drawing.Point(0, 0);
            this.panelTimeline.Name = "panelTimeline";
            this.panelTimeline.Size = new System.Drawing.Size(191, 38);
            this.panelTimeline.TabIndex = 2;
            // 
            // panelCalendar
            // 
            this.panelCalendar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelCalendar.Controls.Add(this.monthCalendar);
            this.panelCalendar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCalendar.Location = new System.Drawing.Point(0, 25);
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
            this.panelButtom2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbDropArea)).EndInit();
            this.panelMain.ResumeLayout(false);
            this.panelFilter.ResumeLayout(false);
            this.panelCustomFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.taskPaneFilter)).EndInit();
            this.taskPaneFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.expandoQuicklist)).EndInit();
            this.panelCalendar.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.PictureBox pbDropArea;
        private Component.ListBarControl.VSNetListBar vsNetListBarGroups;
        private System.Windows.Forms.ImageList imageListLarge;
        private System.Windows.Forms.ImageList imageListSmall;
        private System.Windows.Forms.Panel panelFilter;
        private XPExplorerBar.TaskPane taskPaneFilter;
        private XPExplorerBar.Expando expandoQuicklist;
        private System.Windows.Forms.Panel panelCalendar;
        private System.Windows.Forms.ImageList imageListCustomFilter;
        private System.Windows.Forms.ImageList imageListTimeline;
        private System.Windows.Forms.Panel panelCustomFilter;
        private System.Windows.Forms.Panel panelTimeline;
        private CustomControls.MonthCalendar monthCalendar;
        private System.Windows.Forms.Panel panelButtom2;
        private System.Windows.Forms.Label labelDropInfor;

    }
}
