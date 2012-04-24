#region

using System;
using System.Drawing;
using System.Windows.Forms;
using Waveface.Component.PopupControl;
using Waveface.Component;
using Waveface.Component.RichEdit;
using Waveface.Localization;

#endregion

namespace Waveface
{
    public partial class DateTimePopupPanel : UserControl
    {
        public Popup MyParent { set; get; }

        public DateTime DateTime
        {
            set
            {
                timer.Enabled = false;

                analogClock.Time = value;

                monthCalendar.SelectionStart = value.Date;
                monthCalendar.SelectionEnd = value.Date;
                monthCalendar.ViewStart = value.Date;
                monthCalendar.Invalidate();

                string _tt = value.ToString("tt", CultureManager.ApplicationUICulture);
                labelM.Text = _tt;

                timer.Enabled = true;
            }
        }

        public DateTimePopupPanel()
        {
            InitializeComponent();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;

            MyParent.Hide();
        }
    }
}