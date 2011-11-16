#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.TimePickerEx
{
    // The HourMinuteSelectorForm is used to choose an hour and a minute element of
    // a time.  It displays a form where the user can simply click on a button that
    // represents the time that he wants.  To exit the form, the user can do a left
    // click outside the form or do a right click on a button.  Remember that a
    // right click will also select the button where your mouse is currently placed.
    internal partial class HourMinuteSelectorForm : Form
    {
        private static String H = "H";
        private static String M = "M";

        private Keys m_eKeyToHidePicker;
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
                SetMinute(value.Minutes);
            }
        }

        public Keys KeyToHidePicker
        {
            get { return m_eKeyToHidePicker; }
            set { m_eKeyToHidePicker = value; }
        }

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

        public Color SeparatorColor
        {
            get { return m_oLblSeparator.BackColor; }
            set { m_oLblSeparator.BackColor = value; }
        }

        #endregion

        private Container components;

        private Button m_oOldHourButton;
        private Button m_oOldMinuteButton;

        public HourMinuteSelectorForm()
        {
            InitializeComponent();

            m_oOldHourButton = new Button();
            m_oOldMinuteButton = new Button();

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
                SetMinute(DateTime.Now.TimeOfDay.Minutes);
            }
            else
            {
                SetHour(m_sSelectedTime.Hours);
                SetMinute(m_sSelectedTime.Minutes);
            }

            base.Show();
        }

        private void SetHour(int hour)
        {
            switch (hour)
            {
                case 0:
                    LeftHourClick(m_oBtn0Hour, null);
                    break;
                case 1:
                    LeftHourClick(m_oBtn1Hour, null);
                    break;
                case 2:
                    LeftHourClick(m_oBtn2Hour, null);
                    break;
                case 3:
                    LeftHourClick(m_oBtn3Hour, null);
                    break;
                case 4:
                    LeftHourClick(m_oBtn4Hour, null);
                    break;
                case 5:
                    LeftHourClick(m_oBtn5Hour, null);
                    break;
                case 6:
                    LeftHourClick(m_oBtn6Hour, null);
                    break;
                case 7:
                    LeftHourClick(m_oBtn7Hour, null);
                    break;
                case 8:
                    LeftHourClick(m_oBtn8Hour, null);
                    break;
                case 9:
                    LeftHourClick(m_oBtn9Hour, null);
                    break;
                case 10:
                    LeftHourClick(m_oBtn10Hour, null);
                    break;
                case 11:
                    LeftHourClick(m_oBtn11Hour, null);
                    break;
                case 12:
                    LeftHourClick(m_oBtn12Hour, null);
                    break;
                case 13:
                    LeftHourClick(m_oBtn13Hour, null);
                    break;
                case 14:
                    LeftHourClick(m_oBtn14Hour, null);
                    break;
                case 15:
                    LeftHourClick(m_oBtn15Hour, null);
                    break;
                case 16:
                    LeftHourClick(m_oBtn16Hour, null);
                    break;
                case 17:
                    LeftHourClick(m_oBtn17Hour, null);
                    break;
                case 18:
                    LeftHourClick(m_oBtn18Hour, null);
                    break;
                case 19:
                    LeftHourClick(m_oBtn19Hour, null);
                    break;
                case 20:
                    LeftHourClick(m_oBtn20Hour, null);
                    break;
                case 21:
                    LeftHourClick(m_oBtn21Hour, null);
                    break;
                case 22:
                    LeftHourClick(m_oBtn22Hour, null);
                    break;
                case 23:
                    LeftHourClick(m_oBtn23Hour, null);
                    break;
            }
        }

        private void SetMinute(int minute)
        {
            switch (minute)
            {
                case 0:
                    LeftMinuteClick(m_oBtn0Minute, null);
                    break;
                case 1:
                    LeftMinuteClick(m_oBtn1Minute, null);
                    break;
                case 2:
                    LeftMinuteClick(m_oBtn2Minute, null);
                    break;
                case 3:
                    LeftMinuteClick(m_oBtn3Minute, null);
                    break;
                case 4:
                    LeftMinuteClick(m_oBtn4Minute, null);
                    break;
                case 5:
                    LeftMinuteClick(m_oBtn5Minute, null);
                    break;
                case 6:
                    LeftMinuteClick(m_oBtn6Minute, null);
                    break;
                case 7:
                    LeftMinuteClick(m_oBtn7Minute, null);
                    break;
                case 8:
                    LeftMinuteClick(m_oBtn8Minute, null);
                    break;
                case 9:
                    LeftMinuteClick(m_oBtn9Minute, null);
                    break;
                case 10:
                    LeftMinuteClick(m_oBtn10Minute, null);
                    break;
                case 11:
                    LeftMinuteClick(m_oBtn11Minute, null);
                    break;
                case 12:
                    LeftMinuteClick(m_oBtn12Minute, null);
                    break;
                case 13:
                    LeftMinuteClick(m_oBtn13Minute, null);
                    break;
                case 14:
                    LeftMinuteClick(m_oBtn14Minute, null);
                    break;
                case 15:
                    LeftMinuteClick(m_oBtn15Minute, null);
                    break;
                case 16:
                    LeftMinuteClick(m_oBtn16Minute, null);
                    break;
                case 17:
                    LeftMinuteClick(m_oBtn17Minute, null);
                    break;
                case 18:
                    LeftMinuteClick(m_oBtn18Minute, null);
                    break;
                case 19:
                    LeftMinuteClick(m_oBtn19Minute, null);
                    break;
                case 20:
                    LeftMinuteClick(m_oBtn20Minute, null);
                    break;
                case 21:
                    LeftMinuteClick(m_oBtn21Minute, null);
                    break;
                case 22:
                    LeftMinuteClick(m_oBtn22Minute, null);
                    break;
                case 23:
                    LeftMinuteClick(m_oBtn23Minute, null);
                    break;
                case 24:
                    LeftMinuteClick(m_oBtn24Minute, null);
                    break;
                case 25:
                    LeftMinuteClick(m_oBtn25Minute, null);
                    break;
                case 26:
                    LeftMinuteClick(m_oBtn26Minute, null);
                    break;
                case 27:
                    LeftMinuteClick(m_oBtn27Minute, null);
                    break;
                case 28:
                    LeftMinuteClick(m_oBtn28Minute, null);
                    break;
                case 29:
                    LeftMinuteClick(m_oBtn29Minute, null);
                    break;
                case 30:
                    LeftMinuteClick(m_oBtn30Minute, null);
                    break;
                case 31:
                    LeftMinuteClick(m_oBtn31Minute, null);
                    break;
                case 32:
                    LeftMinuteClick(m_oBtn32Minute, null);
                    break;
                case 33:
                    LeftMinuteClick(m_oBtn33Minute, null);
                    break;
                case 34:
                    LeftMinuteClick(m_oBtn34Minute, null);
                    break;
                case 35:
                    LeftMinuteClick(m_oBtn35Minute, null);
                    break;
                case 36:
                    LeftMinuteClick(m_oBtn36Minute, null);
                    break;
                case 37:
                    LeftMinuteClick(m_oBtn37Minute, null);
                    break;
                case 38:
                    LeftMinuteClick(m_oBtn38Minute, null);
                    break;
                case 39:
                    LeftMinuteClick(m_oBtn39Minute, null);
                    break;
                case 40:
                    LeftMinuteClick(m_oBtn40Minute, null);
                    break;
                case 41:
                    LeftMinuteClick(m_oBtn41Minute, null);
                    break;
                case 42:
                    LeftMinuteClick(m_oBtn42Minute, null);
                    break;
                case 43:
                    LeftMinuteClick(m_oBtn43Minute, null);
                    break;
                case 44:
                    LeftMinuteClick(m_oBtn44Minute, null);
                    break;
                case 45:
                    LeftMinuteClick(m_oBtn45Minute, null);
                    break;
                case 46:
                    LeftMinuteClick(m_oBtn46Minute, null);
                    break;
                case 47:
                    LeftMinuteClick(m_oBtn47Minute, null);
                    break;
                case 48:
                    LeftMinuteClick(m_oBtn48Minute, null);
                    break;
                case 49:
                    LeftMinuteClick(m_oBtn49Minute, null);
                    break;
                case 50:
                    LeftMinuteClick(m_oBtn50Minute, null);
                    break;
                case 51:
                    LeftMinuteClick(m_oBtn51Minute, null);
                    break;
                case 52:
                    LeftMinuteClick(m_oBtn52Minute, null);
                    break;
                case 53:
                    LeftMinuteClick(m_oBtn53Minute, null);
                    break;
                case 54:
                    LeftMinuteClick(m_oBtn54Minute, null);
                    break;
                case 55:
                    LeftMinuteClick(m_oBtn55Minute, null);
                    break;
                case 56:
                    LeftMinuteClick(m_oBtn56Minute, null);
                    break;
                case 57:
                    LeftMinuteClick(m_oBtn57Minute, null);
                    break;
                case 58:
                    LeftMinuteClick(m_oBtn58Minute, null);
                    break;
                case 59:
                    LeftMinuteClick(m_oBtn59Minute, null);
                    break;
            }
        }

        #region Handlers for the events in the HourMinuteSelectorForm

        // Method that handles the left click on a hour button.
        private void LeftHourClick(object sender, EventArgs e)
        {
            Button _clickedButton = (Button) sender;

            m_oOldHourButton.BackColor = _clickedButton.BackColor;
            m_oOldHourButton = _clickedButton;

            _clickedButton.BackColor = SelectedColor;
            _clickedButton.Select();

            m_sSelectedTime = new TimeSpan(Convert.ToInt32(_clickedButton.Text), m_sSelectedTime.Minutes, 0);
        }

        // Method that handle the right click on any button.  To know if the button
        // clicked was an hour or a minute, we check the value of the Tag property.
        private void RightClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Button _clickedButton = (Button) sender;

                if (_clickedButton.Tag.Equals(H))
                {
                    LeftHourClick(sender, e);
                }
                else if (_clickedButton.Tag.Equals(M))
                {
                    LeftMinuteClick(sender, e);
                }

                Hide();
            }
        }

        // Method that handles the left click on a minute button.
        private void LeftMinuteClick(object sender, EventArgs e)
        {
            Button _clickedButton = (Button) sender;

            m_oOldMinuteButton.BackColor = _clickedButton.BackColor;
            m_oOldMinuteButton = _clickedButton;

            _clickedButton.BackColor = SelectedColor;
            _clickedButton.Select();

            m_sSelectedTime = new TimeSpan(m_sSelectedTime.Hours, Convert.ToInt32(_clickedButton.Text), 0);
        }

        // Method to handle the click on the separator (which is a label control) that
        // is between the hours and the minute.  
        private void m_oLblSeparator_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void TimeSelectorForm_Deactivate(object sender, EventArgs e)
        {
            Hide();
        }

        private void m_oBtn_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == m_eKeyToHidePicker)
            {
                Hide();
            }
        }

        #endregion
    }
}