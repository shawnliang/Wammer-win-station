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
            this.pbDropArea = new System.Windows.Forms.PictureBox();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.panelCustomFilter = new System.Windows.Forms.Panel();
            this.tvCustomFilter = new System.Windows.Forms.TreeView();
            this.imageListCustomFilter = new System.Windows.Forms.ImageList(this.components);
            this.panelTimeline = new System.Windows.Forms.Panel();
            this.tvTimeline = new System.Windows.Forms.TreeView();
            this.panelCalendar = new System.Windows.Forms.Panel();
            this.monthCalendar = new CustomControls.MonthCalendar();
            this.vsNetListBarGroups = new Waveface.Component.ListBarControl.VSNetListBar();
            this.imageListLarge = new System.Windows.Forms.ImageList(this.components);
            this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
            this.imageListTimeline = new System.Windows.Forms.ImageList(this.components);
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDropArea)).BeginInit();
            this.panelMain.SuspendLayout();
            this.panelFilter.SuspendLayout();
            this.panelCustomFilter.SuspendLayout();
            this.panelTimeline.SuspendLayout();
            this.panelCalendar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelBottom.Controls.Add(this.pbDropArea);
            resources.ApplyResources(this.panelBottom, "panelBottom");
            this.panelBottom.Name = "panelBottom";
            // 
            // pbDropArea
            // 
            this.pbDropArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pbDropArea.BackgroundImage = global::Waveface.Properties.Resources.dragNdrop_area1;
            resources.ApplyResources(this.pbDropArea, "pbDropArea");
            this.pbDropArea.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbDropArea.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pbDropArea.Name = "pbDropArea";
            this.pbDropArea.TabStop = false;
            this.pbDropArea.Click += new System.EventHandler(this.pbDropArea_Click);
            this.pbDropArea.DragDrop += new System.Windows.Forms.DragEventHandler(this.DropArea_DragDrop);
            this.pbDropArea.DragEnter += new System.Windows.Forms.DragEventHandler(this.DropArea_DragEnter);
            this.pbDropArea.DragOver += new System.Windows.Forms.DragEventHandler(this.DropArea_DragOver);
            this.pbDropArea.DragLeave += new System.EventHandler(this.DropArea_DragLeave);
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelMain.Controls.Add(this.panelFilter);
            this.panelMain.Controls.Add(this.panelCalendar);
            this.panelMain.Controls.Add(this.vsNetListBarGroups);
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Name = "panelMain";
            // 
            // panelFilter
            // 
            this.panelFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelFilter.Controls.Add(this.panelCustomFilter);
            this.panelFilter.Controls.Add(this.panelTimeline);
            resources.ApplyResources(this.panelFilter, "panelFilter");
            this.panelFilter.Name = "panelFilter";
            // 
            // panelCustomFilter
            // 
            this.panelCustomFilter.Controls.Add(this.tvCustomFilter);
            resources.ApplyResources(this.panelCustomFilter, "panelCustomFilter");
            this.panelCustomFilter.Name = "panelCustomFilter";
            // 
            // tvCustomFilter
            // 
            resources.ApplyResources(this.tvCustomFilter, "tvCustomFilter");
            this.tvCustomFilter.ImageList = this.imageListCustomFilter;
            this.tvCustomFilter.Name = "tvCustomFilter";
            this.tvCustomFilter.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvCustomFilter_AfterSelect);
            // 
            // imageListCustomFilter
            // 
            this.imageListCustomFilter.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListCustomFilter.ImageStream")));
            this.imageListCustomFilter.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListCustomFilter.Images.SetKeyName(0, "bullet_wrench.png");
            this.imageListCustomFilter.Images.SetKeyName(1, "bullet_orange.png");
            // 
            // panelTimeline
            // 
            this.panelTimeline.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelTimeline.Controls.Add(this.tvTimeline);
            resources.ApplyResources(this.panelTimeline, "panelTimeline");
            this.panelTimeline.Name = "panelTimeline";
            // 
            // tvTimeline
            // 
            resources.ApplyResources(this.tvTimeline, "tvTimeline");
            this.tvTimeline.ImageList = this.imageListCustomFilter;
            this.tvTimeline.Name = "tvTimeline";
            this.tvTimeline.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvTimeline_AfterSelect);
            // 
            // panelCalendar
            // 
            this.panelCalendar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelCalendar.Controls.Add(this.monthCalendar);
            resources.ApplyResources(this.panelCalendar, "panelCalendar");
            this.panelCalendar.Name = "panelCalendar";
            // 
            // monthCalendar
            // 
            this.monthCalendar.ColorTable.BackgroundGradientBegin = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(231)))), ((int)(((byte)(209)))));
            this.monthCalendar.ColorTable.BackgroundGradientEnd = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(231)))), ((int)(((byte)(209)))));
            this.monthCalendar.ColorTable.Border = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(231)))), ((int)(((byte)(209)))));
            this.monthCalendar.ColorTable.DayActiveGradientBegin = System.Drawing.Color.NavajoWhite;
            this.monthCalendar.ColorTable.DaySelectedGradientBegin = System.Drawing.Color.NavajoWhite;
            this.monthCalendar.ColorTable.DayText = System.Drawing.Color.DarkGray;
            this.monthCalendar.ColorTable.HeaderActiveGradientBegin = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.monthCalendar.ColorTable.HeaderArrow = System.Drawing.Color.Silver;
            this.monthCalendar.ColorTable.HeaderGradientBegin = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.monthCalendar.ColorTable.MonthSeparator = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            resources.ApplyResources(this.monthCalendar, "monthCalendar");
            this.monthCalendar.HeaderFont = new System.Drawing.Font("Segoe UI", 10F);
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.SelectionMode = CustomControls.MonthCalendarSelectionMode.Day;
            this.monthCalendar.ShowWeekHeader = false;
            this.monthCalendar.UseShortestDayNames = true;
            this.monthCalendar.DateClicked += new System.EventHandler<CustomControls.DateEventArgs>(this.monthCalendar_DateClicked);
            // 
            // vsNetListBarGroups
            // 
            this.vsNetListBarGroups.AllowDragGroups = true;
            this.vsNetListBarGroups.AllowDragItems = true;
            this.vsNetListBarGroups.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.vsNetListBarGroups, "vsNetListBarGroups");
            this.vsNetListBarGroups.DrawStyle = Waveface.Component.ListBarControl.ListBarDrawStyle.ListBarDrawStyleNormal;
            this.vsNetListBarGroups.LargeImageList = this.imageListLarge;
            this.vsNetListBarGroups.Name = "vsNetListBarGroups";
            this.vsNetListBarGroups.SelectOnMouseDown = true;
            this.vsNetListBarGroups.SmallImageList = this.imageListSmall;
            this.vsNetListBarGroups.ToolTip = null;
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
            // imageListTimeline
            // 
            this.imageListTimeline.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTimeline.ImageStream")));
            this.imageListTimeline.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListTimeline.Images.SetKeyName(0, "AllTime.png");
            this.imageListTimeline.Images.SetKeyName(1, "Month.png");
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // LeftArea
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelBottom);
            this.Name = "LeftArea";
            this.Resize += new System.EventHandler(this.LeftArea_Resize);
            this.panelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbDropArea)).EndInit();
            this.panelMain.ResumeLayout(false);
            this.panelFilter.ResumeLayout(false);
            this.panelCustomFilter.ResumeLayout(false);
            this.panelTimeline.ResumeLayout(false);
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
        private System.Windows.Forms.Panel panelCalendar;
        private System.Windows.Forms.ImageList imageListCustomFilter;
        private System.Windows.Forms.ImageList imageListTimeline;
        private System.Windows.Forms.Panel panelCustomFilter;
        private System.Windows.Forms.Panel panelTimeline;
        private CustomControls.MonthCalendar monthCalendar;
        private Localization.CultureManager cultureManager;
        private System.Windows.Forms.TreeView tvCustomFilter;
        private System.Windows.Forms.TreeView tvTimeline;

    }
}
