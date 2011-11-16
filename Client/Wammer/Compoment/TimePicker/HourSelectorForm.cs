#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.TimePickerEx
{
    // The HourSelectorForm is used to choose an hour element of a time.  It displays
    // a form where the user can simply click on a button that represents the hour 
    // that he wants.  To exit the form, the user can do a left click outside the form 
    // or do a right click on a button.  Remember that a right click will also 
    // select the button where your mouse is currently placed.
    internal partial class HourSelectorForm : Form
    {
        private Color m_oButtonColor;
        private TimeSpan m_sSelectedTime;

        #region Properties

        public TimeSpan SelectedTime
        {
            get { return m_sSelectedTime; }
            set
            {
                m_sSelectedTime = value;
                SetHour(value.Hours);
            }
        }

        public Keys KeyToHidePicker { get; set; }

        public Color ButtonColor
        {
            get { return m_oButtonColor; }
            set
            {
                m_oButtonColor = value;

                foreach (Control _c in Controls)
                {
                    if (_c is Button)
                    {
                        _c.BackColor = value;
                    }
                }
            }
        }

        public Color SelectedColor { get; set; }

        public bool ResetMinute { get; set; }

        #endregion

        private Container components;

        private Button m_oOldHourButton;

        public HourSelectorForm()
        {
            InitializeComponent();

            m_oOldHourButton = new Button();

            m_sSelectedTime = TimeSpan.MinValue;
            SelectedColor = Color.White;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        public new void Show()
        {
            if (m_sSelectedTime == TimeSpan.MinValue)
            {
                SetHour(DateTime.Now.TimeOfDay.Hours);
            }
            else
            {
                SetHour(m_sSelectedTime.Hours);
            }

            base.Show();
        }

        private void SetHour(int nHour)
        {
            switch (nHour)
            {
                case 0:
                    LeftClick(m_oBtn0Hour, null);
                    break;
                case 1:
                    LeftClick(m_oBtn1Hour, null);
                    break;
                case 2:
                    LeftClick(m_oBtn2Hour, null);
                    break;
                case 3:
                    LeftClick(m_oBtn3Hour, null);
                    break;
                case 4:
                    LeftClick(m_oBtn4Hour, null);
                    break;
                case 5:
                    LeftClick(m_oBtn5Hour, null);
                    break;
                case 6:
                    LeftClick(m_oBtn6Hour, null);
                    break;
                case 7:
                    LeftClick(m_oBtn7Hour, null);
                    break;
                case 8:
                    LeftClick(m_oBtn8Hour, null);
                    break;
                case 9:
                    LeftClick(m_oBtn9Hour, null);
                    break;
                case 10:
                    LeftClick(m_oBtn10Hour, null);
                    break;
                case 11:
                    LeftClick(m_oBtn11Hour, null);
                    break;
                case 12:
                    LeftClick(m_oBtn12Hour, null);
                    break;
                case 13:
                    LeftClick(m_oBtn13Hour, null);
                    break;
                case 14:
                    LeftClick(m_oBtn14Hour, null);
                    break;
                case 15:
                    LeftClick(m_oBtn15Hour, null);
                    break;
                case 16:
                    LeftClick(m_oBtn16Hour, null);
                    break;
                case 17:
                    LeftClick(m_oBtn17Hour, null);
                    break;
                case 18:
                    LeftClick(m_oBtn18Hour, null);
                    break;
                case 19:
                    LeftClick(m_oBtn19Hour, null);
                    break;
                case 20:
                    LeftClick(m_oBtn20Hour, null);
                    break;
                case 21:
                    LeftClick(m_oBtn21Hour, null);
                    break;
                case 22:
                    LeftClick(m_oBtn22Hour, null);
                    break;
                case 23:
                    LeftClick(m_oBtn23Hour, null);
                    break;
            }
        }

        #region Handlers for the events in the HourSelectorForm

        private void LeftClick(object sender, EventArgs e)
        {
            Button _clickedButton = (Button) sender;

            m_oOldHourButton.BackColor = _clickedButton.BackColor;
            m_oOldHourButton = _clickedButton;

            _clickedButton.BackColor = SelectedColor;
            _clickedButton.Select();

            m_sSelectedTime = new TimeSpan(Convert.ToInt32(_clickedButton.Text), m_sSelectedTime.Minutes, 0);
        }

        private void RightClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                LeftClick(sender, e);
                Hide();
            }
        }

        private void HourSelectorForm_Deactivate(object sender, EventArgs e)
        {
            if (ResetMinute)
            {
                m_sSelectedTime = new TimeSpan(m_sSelectedTime.Hours, 0, 0);
            }
            else
            {
                m_sSelectedTime = new TimeSpan(m_sSelectedTime.Hours, m_sSelectedTime.Minutes, 0);
                ResetMinute = true;
            }

            Hide();
        }

        #endregion
    }
}