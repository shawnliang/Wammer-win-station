using Waveface.Component;
using Waveface.Component.RichEdit;

namespace Waveface
{
    partial class DateTimePopupPanel
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
            this.monthCalendar = new CustomControls.MonthCalendar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.labelM = new System.Windows.Forms.Label();
            this.analogClock = new Waveface.Component.AnalogClock();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // monthCalendar
            // 
            this.monthCalendar.CellDimensions = new System.Drawing.Size(17, 10);
            this.monthCalendar.ColorTable.BackgroundGradientBegin = System.Drawing.Color.Gainsboro;
            this.monthCalendar.ColorTable.BackgroundGradientEnd = System.Drawing.Color.Gainsboro;
            this.monthCalendar.ColorTable.Border = System.Drawing.Color.Silver;
            this.monthCalendar.ColorTable.DayActiveGradientBegin = System.Drawing.Color.NavajoWhite;
            this.monthCalendar.ColorTable.DayActiveTodayCircleBorder = System.Drawing.Color.Gainsboro;
            this.monthCalendar.ColorTable.DaySelectedGradientBegin = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(154)))), ((int)(((byte)(174)))));
            this.monthCalendar.ColorTable.DaySelectedGradientEnd = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(154)))), ((int)(((byte)(174)))));
            this.monthCalendar.ColorTable.DaySelectedText = System.Drawing.Color.White;
            this.monthCalendar.ColorTable.DaySelectedTodayCircleBorder = System.Drawing.Color.Gainsboro;
            this.monthCalendar.ColorTable.DayText = System.Drawing.Color.DarkGray;
            this.monthCalendar.ColorTable.DayTodayCircleBorder = System.Drawing.Color.Gainsboro;
            this.monthCalendar.ColorTable.DayTrailingText = System.Drawing.Color.Gainsboro;
            this.monthCalendar.ColorTable.HeaderActiveGradientBegin = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.monthCalendar.ColorTable.HeaderArrow = System.Drawing.Color.Silver;
            this.monthCalendar.ColorTable.HeaderGradientBegin = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.monthCalendar.ColorTable.MonthSeparator = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.monthCalendar.Font = new System.Drawing.Font("Tahoma", 9F);
            this.monthCalendar.HeaderFont = new System.Drawing.Font("Segoe UI", 10F);
            this.monthCalendar.IsWaveface = true;
            this.monthCalendar.Location = new System.Drawing.Point(74, 9);
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.SelectionMode = CustomControls.MonthCalendarSelectionMode.Day;
            this.monthCalendar.ShowFooter = false;
            this.monthCalendar.ShowWeekHeader = false;
            this.monthCalendar.TabIndex = 1;
            this.monthCalendar.UseShortestDayNames = true;
            // 
            // timer
            // 
            this.timer.Interval = 4000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // pictureBox
            // 
            this.pictureBox.Image = global::Waveface.Properties.Resources.ToLeft;
            this.pictureBox.Location = new System.Drawing.Point(1, 57);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(67, 66);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // labelM
            // 
            this.labelM.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelM.Location = new System.Drawing.Point(239, 140);
            this.labelM.Name = "labelM";
            this.labelM.Size = new System.Drawing.Size(153, 21);
            this.labelM.TabIndex = 3;
            this.labelM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // analogClock
            // 
            this.analogClock.DrawDropShadow = false;
            this.analogClock.DrawHourHand = true;
            this.analogClock.DrawHourHandShadow = true;
            this.analogClock.DrawMinuteHand = true;
            this.analogClock.DrawMinuteHandShadow = true;
            this.analogClock.DrawSecondHand = false;
            this.analogClock.DrawSecondHandShadow = false;
            this.analogClock.DropShadowColor = System.Drawing.Color.Black;
            this.analogClock.DropShadowOffset = new System.Drawing.Point(0, 0);
            this.analogClock.FaceColorHigh = System.Drawing.Color.RoyalBlue;
            this.analogClock.FaceColorLow = System.Drawing.Color.SkyBlue;
            this.analogClock.FaceGradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal;
            this.analogClock.FaceImage = null;
            this.analogClock.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.analogClock.HourHandColor = System.Drawing.Color.Gainsboro;
            this.analogClock.HourHandDropShadowColor = System.Drawing.Color.Gray;
            this.analogClock.Location = new System.Drawing.Point(239, -2);
            this.analogClock.MinuteHandColor = System.Drawing.Color.WhiteSmoke;
            this.analogClock.MinuteHandDropShadowColor = System.Drawing.Color.Gray;
            this.analogClock.MinuteHandTickStyle = Waveface.Component.TickStyle.Smooth;
            this.analogClock.Name = "analogClock";
            this.analogClock.NumeralColor = System.Drawing.Color.WhiteSmoke;
            this.analogClock.RimColorHigh = System.Drawing.Color.RoyalBlue;
            this.analogClock.RimColorLow = System.Drawing.Color.SkyBlue;
            this.analogClock.RimGradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            this.analogClock.SecondHandColor = System.Drawing.Color.Tomato;
            this.analogClock.SecondHandDropShadowColor = System.Drawing.Color.Gray;
            this.analogClock.SecondHandEndCap = System.Drawing.Drawing2D.LineCap.Round;
            this.analogClock.SecondHandTickStyle = Waveface.Component.TickStyle.Normal;
            this.analogClock.Size = new System.Drawing.Size(150, 150);
            this.analogClock.TabIndex = 0;
            this.analogClock.Time = new System.DateTime(((long)(0)));
            // 
            // DateTimePopupPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.Controls.Add(this.labelM);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.monthCalendar);
            this.Controls.Add(this.analogClock);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DateTimePopupPanel";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.Size = new System.Drawing.Size(392, 169);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AnalogClock analogClock;
        private CustomControls.MonthCalendar monthCalendar;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label labelM;


    }
}
