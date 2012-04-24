namespace MonthCalendarDemo
{
   partial class Form1
   {
      /// <summary>
      /// Erforderliche Designervariable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Verwendete Ressourcen bereinigen.
      /// </summary>
      /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Vom Windows Form-Designer generierter Code

      /// <summary>
      /// Erforderliche Methode für die Designerunterstützung.
      /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
      /// </summary>
      private void InitializeComponent()
      {
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.monthCalendar1 = new CustomControls.MonthCalendar();
            this.datePicker1 = new CustomControls.DatePicker();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Window;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 111);
            this.label1.TabIndex = 6;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(263, 522);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 21);
            this.button2.TabIndex = 9;
            this.button2.Text = "set max date";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(182, 522);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 21);
            this.button1.TabIndex = 8;
            this.button1.Text = "set min date";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(344, 527);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(446, 527);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "label3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(573, 526);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "label4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 505);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "label5";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"});
            this.comboBox1.Location = new System.Drawing.Point(686, 522);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(159, 20);
            this.comboBox1.TabIndex = 14;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.CellDimensions = new System.Drawing.Size(18, 18);
            this.monthCalendar1.ColorTable.DayActiveGradientBegin = System.Drawing.Color.Bisque;
            this.monthCalendar1.ColorTable.DayActiveGradientEnd = System.Drawing.Color.Gold;
            this.monthCalendar1.ColorTable.DayHeaderText = System.Drawing.Color.MediumBlue;
            this.monthCalendar1.ColorTable.DayTextBold = System.Drawing.Color.DarkBlue;
            this.monthCalendar1.ColorTable.FooterActiveGradientBegin = System.Drawing.Color.Transparent;
            this.monthCalendar1.ColorTable.FooterActiveGradientEnd = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.monthCalendar1.ColorTable.FooterActiveText = System.Drawing.Color.DarkRed;
            this.monthCalendar1.ColorTable.HeaderActiveArrow = System.Drawing.Color.MidnightBlue;
            this.monthCalendar1.ColorTable.HeaderActiveGradientEnd = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(149)))), ((int)(((byte)(255)))));
            this.monthCalendar1.ColorTable.HeaderGradientEnd = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(177)))), ((int)(((byte)(255)))));
            this.monthCalendar1.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.monthCalendar1.IsWaveface = true;
            this.monthCalendar1.Location = new System.Drawing.Point(206, 8);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.SelectionMode = CustomControls.MonthCalendarSelectionMode.Day;
            this.monthCalendar1.SelectionRange = new System.Windows.Forms.SelectionRange(new System.DateTime(2012, 4, 22, 0, 0, 0, 0), new System.DateTime(2012, 4, 22, 0, 0, 0, 0));
            this.monthCalendar1.ShowFooter = false;
            this.monthCalendar1.ShowWeekHeader = false;
            this.monthCalendar1.TabIndex = 7;
            this.monthCalendar1.UseShortestDayNames = true;
            this.monthCalendar1.DateSelected += new System.EventHandler<System.Windows.Forms.DateRangeEventArgs>(this.monthCalendar1_DateSelected);
            // 
            // datePicker1
            // 
            this.datePicker1.Culture = new System.Globalization.CultureInfo("de-DE");
            this.datePicker1.FormatProvider.ShortDatePattern = "dd.MMM.yyyy";
            this.datePicker1.InvalidBackColor = System.Drawing.SystemColors.Window;
            this.datePicker1.InvalidForeColor = System.Drawing.Color.Red;
            this.datePicker1.Location = new System.Drawing.Point(12, 122);
            this.datePicker1.Name = "datePicker1";
            this.datePicker1.PickerColorTable.DayActiveGradientBegin = System.Drawing.Color.Bisque;
            this.datePicker1.PickerColorTable.DayActiveGradientEnd = System.Drawing.Color.Gold;
            this.datePicker1.PickerColorTable.DayHeaderText = System.Drawing.Color.MediumBlue;
            this.datePicker1.PickerColorTable.DaySelectedGradientBegin = System.Drawing.Color.Bisque;
            this.datePicker1.PickerColorTable.DaySelectedGradientEnd = System.Drawing.Color.DarkOrange;
            this.datePicker1.PickerColorTable.DayTextBold = System.Drawing.Color.DarkBlue;
            this.datePicker1.PickerColorTable.FooterActiveGradientBegin = System.Drawing.Color.Transparent;
            this.datePicker1.PickerColorTable.FooterActiveGradientEnd = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.datePicker1.PickerColorTable.FooterActiveText = System.Drawing.Color.DarkRed;
            this.datePicker1.PickerColorTable.HeaderActiveArrow = System.Drawing.Color.MidnightBlue;
            this.datePicker1.PickerColorTable.HeaderActiveGradientEnd = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(149)))), ((int)(((byte)(255)))));
            this.datePicker1.PickerColorTable.HeaderGradientEnd = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(177)))), ((int)(((byte)(255)))));
            this.datePicker1.PickerDayFont = new System.Drawing.Font("Tahoma", 8.25F);
            this.datePicker1.PickerDimension = new System.Drawing.Size(1, 2);
            this.datePicker1.ShowPickerWeekHeader = false;
            this.datePicker1.Size = new System.Drawing.Size(164, 21);
            this.datePicker1.TabIndex = 5;
            this.datePicker1.ValueChanged += new System.EventHandler<CustomControls.CheckDateEventArgs>(this.customDateTimePicker1_ValueChanged);
            this.datePicker1.ActiveDateChanged += new System.EventHandler<CustomControls.ActiveDateChangedEventArgs>(this.customDateTimePicker1_ActiveDateChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(893, 546);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.monthCalendar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.datePicker1);
            this.Name = "Form1";
            this.Text = "Test";
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private CustomControls.DatePicker datePicker1;
      private System.Windows.Forms.Label label1;
      private CustomControls.MonthCalendar monthCalendar1;
      private System.Windows.Forms.Button button2;
      private System.Windows.Forms.Button button1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.ComboBox comboBox1;
   }
}