namespace MonthCalendarDemo
{
   using System;
   using System.Drawing;
   using System.Windows.Forms;

   using CustomControls;

   public partial class Form1 : Form
   {
      private bool init;

      public Form1()
      {
         InitializeComponent();

         var cat1 = new BoldedDateCategory("Holiday") { ForeColor = Color.DarkBlue };
         var cat2 = new BoldedDateCategory("Vacation") { ForeColor = Color.DarkGreen, BackColorStart = Color.Gray, BackColorEnd = Color.White };
         this.monthCalendar1.BoldedDateCategoryCollection.AddRange(new[] { cat1, cat2 });
         this.datePicker1.BoldedDateCategoryCollection.AddRange(new[] { cat1, cat2 });

         this.monthCalendar1.BoldedDatesCollection.Add(new BoldedDate { Category = cat1, Value = DateTime.Today.AddDays(1) });
         this.monthCalendar1.BoldedDatesCollection.Add(new BoldedDate { Category = cat2, Value = DateTime.Today.AddDays(2) });

         this.datePicker1.BoldedDatesCollection.Add(new BoldedDate { Category = cat1, Value = DateTime.Today.AddDays(1) });
         this.datePicker1.BoldedDatesCollection.Add(new BoldedDate { Category = cat2, Value = DateTime.Today.AddDays(2) });

         this.label2.Text = this.monthCalendar1.MinDate.ToShortDateString();
         this.label3.Text = this.monthCalendar1.MaxDate.ToShortDateString();

         this.init = true;

         this.comboBox1.SelectedIndex = (int)this.monthCalendar1.FormatProvider.FirstDayOfWeek;

         this.init = false;
      }

      private void customDateTimePicker1_ValueChanged(object sender, CustomControls.CheckDateEventArgs e)
      {
         e.IsValid = e.Date.Year <= 2011;
         this.label5.Text = e.Date.ToShortDateString();
      }

      private void customDateTimePicker1_ActiveDateChanged(object sender, CustomControls.ActiveDateChangedEventArgs e)
      {
         if (e.IsBoldDate)
         {
            this.label1.Text = "Bolded date is : " + e.Date.ToShortDateString() + "\nText description here.";
         }
         else
         {
            this.label1.Text = "";
         }
      }

      private void button1_Click(object sender, EventArgs e)
      {
         this.monthCalendar1.ViewStart = this.monthCalendar1.MinDate;
      }

      private void button2_Click(object sender, EventArgs e)
      {
         this.monthCalendar1.ViewStart = this.monthCalendar1.MaxDate;
      }

      private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
      {
         this.label4.Text = e.Start.ToShortDateString();
      }

      private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (this.init || this.comboBox1.SelectedIndex < 0 || this.comboBox1.SelectedIndex > 6)
         {
            return;
         }

         this.monthCalendar1.BeginUpdate();
         this.monthCalendar1.FormatProvider.FirstDayOfWeek = (DayOfWeek)this.comboBox1.SelectedIndex;
         this.monthCalendar1.UpdateMonths();
         this.monthCalendar1.EndUpdate();

         this.datePicker1.FormatProvider.FirstDayOfWeek = (DayOfWeek)this.comboBox1.SelectedIndex;
      }
   }
}