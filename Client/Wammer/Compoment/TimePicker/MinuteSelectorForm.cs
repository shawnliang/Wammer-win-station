#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.TimePickerEx
{
    // The MinuteSelectorForm is used to choose a minute element of a time.  It displays
    // a form where the user can simply click on a button that represents the minute
    // that he wants.  To exit the form, the user can do a left click outside the form 
    // or do a right click on a button.  Remember that a right click will also 
    // select the button where your mouse is currently placed.
    internal partial class MinuteSelectorForm : Form
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
                SetMinute(value.Minutes);
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

        private Button m_oOldMinuteButton;

        public MinuteSelectorForm()
        {
            InitializeComponent();

            m_oOldMinuteButton = new Button();

            m_sSelectedTime = TimeSpan.MinValue;
            SelectedColor = Color.White;
            ResetMinute = true;
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
                SetMinute(DateTime.Now.TimeOfDay.Minutes);
            }
            else
            {
                SetMinute(m_sSelectedTime.Minutes);
            }

            base.Show();
        }

        private void MinuteSelectorForm_Deactivate(object sender, EventArgs e)
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

        private void LeftClick(object sender, EventArgs e)
        {
            Button _clickedButton = (Button) sender;

            m_oOldMinuteButton.BackColor = _clickedButton.BackColor;
            m_oOldMinuteButton = _clickedButton;

            _clickedButton.BackColor = SelectedColor;
            _clickedButton.Select();

            m_sSelectedTime = new TimeSpan(m_sSelectedTime.Hours, Convert.ToInt32(_clickedButton.Text), 0);
        }

        private void RightClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                LeftClick(sender, e);
                Hide();
            }
        }

        private void m_oBtn_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == KeyToHidePicker)
            {
                Hide();
            }
        }

        private void SetMinute(int nMinute)
        {
            switch (nMinute)
            {
                case 0:
                    LeftClick(m_oBtn0Minute, null);
                    break;
                case 1:
                    LeftClick(m_oBtn1Minute, null);
                    break;
                case 2:
                    LeftClick(m_oBtn2Minute, null);
                    break;
                case 3:
                    LeftClick(m_oBtn3Minute, null);
                    break;
                case 4:
                    LeftClick(m_oBtn4Minute, null);
                    break;
                case 5:
                    LeftClick(m_oBtn5Minute, null);
                    break;
                case 6:
                    LeftClick(m_oBtn6Minute, null);
                    break;
                case 7:
                    LeftClick(m_oBtn7Minute, null);
                    break;
                case 8:
                    LeftClick(m_oBtn8Minute, null);
                    break;
                case 9:
                    LeftClick(m_oBtn9Minute, null);
                    break;
                case 10:
                    LeftClick(m_oBtn10Minute, null);
                    break;
                case 11:
                    LeftClick(m_oBtn11Minute, null);
                    break;
                case 12:
                    LeftClick(m_oBtn12Minute, null);
                    break;
                case 13:
                    LeftClick(m_oBtn13Minute, null);
                    break;
                case 14:
                    LeftClick(m_oBtn14Minute, null);
                    break;
                case 15:
                    LeftClick(m_oBtn15Minute, null);
                    break;
                case 16:
                    LeftClick(m_oBtn16Minute, null);
                    break;
                case 17:
                    LeftClick(m_oBtn17Minute, null);
                    break;
                case 18:
                    LeftClick(m_oBtn18Minute, null);
                    break;
                case 19:
                    LeftClick(m_oBtn19Minute, null);
                    break;
                case 20:
                    LeftClick(m_oBtn20Minute, null);
                    break;
                case 21:
                    LeftClick(m_oBtn21Minute, null);
                    break;
                case 22:
                    LeftClick(m_oBtn22Minute, null);
                    break;
                case 23:
                    LeftClick(m_oBtn23Minute, null);
                    break;
                case 24:
                    LeftClick(m_oBtn24Minute, null);
                    break;
                case 25:
                    LeftClick(m_oBtn25Minute, null);
                    break;
                case 26:
                    LeftClick(m_oBtn26Minute, null);
                    break;
                case 27:
                    LeftClick(m_oBtn27Minute, null);
                    break;
                case 28:
                    LeftClick(m_oBtn28Minute, null);
                    break;
                case 29:
                    LeftClick(m_oBtn29Minute, null);
                    break;
                case 30:
                    LeftClick(m_oBtn30Minute, null);
                    break;
                case 31:
                    LeftClick(m_oBtn31Minute, null);
                    break;
                case 32:
                    LeftClick(m_oBtn32Minute, null);
                    break;
                case 33:
                    LeftClick(m_oBtn33Minute, null);
                    break;
                case 34:
                    LeftClick(m_oBtn34Minute, null);
                    break;
                case 35:
                    LeftClick(m_oBtn35Minute, null);
                    break;
                case 36:
                    LeftClick(m_oBtn36Minute, null);
                    break;
                case 37:
                    LeftClick(m_oBtn37Minute, null);
                    break;
                case 38:
                    LeftClick(m_oBtn38Minute, null);
                    break;
                case 39:
                    LeftClick(m_oBtn39Minute, null);
                    break;
                case 40:
                    LeftClick(m_oBtn40Minute, null);
                    break;
                case 41:
                    LeftClick(m_oBtn41Minute, null);
                    break;
                case 42:
                    LeftClick(m_oBtn42Minute, null);
                    break;
                case 43:
                    LeftClick(m_oBtn43Minute, null);
                    break;
                case 44:
                    LeftClick(m_oBtn44Minute, null);
                    break;
                case 45:
                    LeftClick(m_oBtn45Minute, null);
                    break;
                case 46:
                    LeftClick(m_oBtn46Minute, null);
                    break;
                case 47:
                    LeftClick(m_oBtn47Minute, null);
                    break;
                case 48:
                    LeftClick(m_oBtn48Minute, null);
                    break;
                case 49:
                    LeftClick(m_oBtn49Minute, null);
                    break;
                case 50:
                    LeftClick(m_oBtn50Minute, null);
                    break;
                case 51:
                    LeftClick(m_oBtn51Minute, null);
                    break;
                case 52:
                    LeftClick(m_oBtn52Minute, null);
                    break;
                case 53:
                    LeftClick(m_oBtn53Minute, null);
                    break;
                case 54:
                    LeftClick(m_oBtn54Minute, null);
                    break;
                case 55:
                    LeftClick(m_oBtn55Minute, null);
                    break;
                case 56:
                    LeftClick(m_oBtn56Minute, null);
                    break;
                case 57:
                    LeftClick(m_oBtn57Minute, null);
                    break;
                case 58:
                    LeftClick(m_oBtn58Minute, null);
                    break;
                case 59:
                    LeftClick(m_oBtn59Minute, null);
                    break;
            }
        }
    }
}