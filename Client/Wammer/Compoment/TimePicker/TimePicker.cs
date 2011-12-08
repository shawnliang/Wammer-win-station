#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.TimePickerEx
{
    // The TimePicker class is used to select a time that is formed of an hour
    // and a minute element.  By clicking on the drop down arrow on the right
    // side of the component, a panel appears where the user can select his time
    // by simply clicking on buttons.
    // 
    // The user also has a few important properties to customize its TimePicker
    // object.  He can change the format of the time displayed with the Format
    // property.  He can change the colors of the buttons with the ButtonColor
    // and the SelectorColor.  He can also use the Value property to set a default
    // time to the TimePicker object.  You can refer to the Design-time property
    // region to look at the property of the TimePicker control.
    public partial class TimePicker : UserControl
    {
        #region Constants

        private static String ERROR_WRONG_FORMAT_TITLE = "Time format is incorrect";

        private static String ERROR_WRONG_FORMAT_TEXT =
            "The format you have entered is inapropriate.  Please review your format.";

        private static TimeSpan TWELVE_HOURS = new TimeSpan(12, 0, 0);
        private static String UNKNOWN = "??";
        private static String HHMM = "HH:MM";
        private static String ZERO = "0";
        private static String HH = "HH";
        private static String H = "H";
        private static String hh = "hh";
        private static String h = "h";
        private static String MM = "MM";
        private static String M = "M";
        private static String AM = "AM";
        private static String PM = "PM";
        private static char SEPARATOR = ':';
        private static SolidBrush EnableColor = new SolidBrush(Color.Black);
        private static SolidBrush DisableColor = new SolidBrush(Color.Gray);

        #endregion

        private char m_cSeparator;
        private Keys m_eKeyToHidePicker;
        private RightToLeft m_eRightToLeft;
        private TimePickerFormat m_eTimeSelector;
        private Color m_oButtonColor;
        private Color m_oSelectedColor;
        private Color m_oSeparatorColor;
        private TimeSpan m_sSelectedTime;
        private String m_szSeparatorAsString;
        private String m_szTimeFormat;

        #region Design-time property

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(typeof (TimeSpan), "00:00")]
        [Editor(typeof (TimePickerValueEditor), typeof (UITypeEditor))]
        [Description("The time of the time picker object when it is instanciated")]
        public TimeSpan Value
        {
            get { return m_sSelectedTime; }
            set
            {
                m_sSelectedTime = value;
                GenerateFormatedText();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(TimePickerFormat.HoursAndMinutes)]
        [Description("Choose what kind of time that you want to select")]
        public TimePickerFormat TimeSelector
        {
            get { return m_eTimeSelector; }
            set
            {
                m_eTimeSelector = value;
                RightToLeft = RightToLeft;
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(typeof (String), "HH:MM")]
        [Description("Choose the format to display the time")]
        public String Format
        {
            get { return m_szTimeFormat; }
            set
            {
                m_szTimeFormat = value;
                GenerateFormatedText();
                RightToLeft = RightToLeft;
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(Keys.Escape)]
        [Description("Choose the key that will hide the TimePicker")]
        public Keys KeyToHidePicker
        {
            get { return m_eKeyToHidePicker; }
            set
            {
                m_eKeyToHidePicker = value;
                m_oHourForm.KeyToHidePicker = value;
                m_oHourMinuteForm.KeyToHidePicker = value;
                m_oMinuteForm.KeyToHidePicker = value;
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(':')]
        [Description("The character that separates the hour from the minute")]
        public char Separator
        {
            get { return m_cSeparator; }
            set
            {
                m_szTimeFormat = m_szTimeFormat.Replace(m_cSeparator, value);
                m_cSeparator = value;
                m_szSeparatorAsString = new String(new[]
                                                       {
                                                           m_cSeparator
                                                       });
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Place true to see the checkbox of the control being clicked")]
        public bool Checked
        {
            get { return m_oChkEnable.Checked; }
            set
            {
                m_oChkEnable.Checked = value;
                m_oBtnShowTimeSelector.Enabled = value;
                Refresh();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof (Color), "System.Drawing.SystemColors.Control")]
        [AmbientValue(typeof (Color), "System.Drawing.SystemColors.Control")]
        [Description("The color of the button when it is not selected")]
        public Color ButtonColor
        {
            get { return m_oButtonColor; }
            set
            {
                m_oButtonColor = value;
                m_oHourForm.ButtonColor = value;
                m_oMinuteForm.ButtonColor = value;
                m_oHourMinuteForm.ButtonColor = value;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof (Color), "SystemColors.Highlight")]
        [Description("The color of the button when it is selected")]
        public Color SelectedColor
        {
            get { return m_oSelectedColor; }
            set
            {
                m_oSelectedColor = value;
                m_oHourForm.SelectedColor = value;
                m_oMinuteForm.SelectedColor = value;
                m_oHourMinuteForm.SelectedColor = value;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof (Color), "SystemColors.AppWorkspace")]
        [Description("The color of the separator in the time selector")]
        public Color SeparatorColor
        {
            get { return m_oSeparatorColor; }
            set
            {
                m_oSeparatorColor = value;
                m_oHourMinuteForm.SeparatorColor = value;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Display the drop down arrow on the right side of the control")]
        public bool ShowDropDown
        {
            get { return m_oBtnShowTimeSelector.Visible; }
            set
            {
                m_oBtnShowTimeSelector.Visible = value;
                RightToLeft = RightToLeft;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Display a checkbox to the left side of the control")]
        public bool ShowCheckBox
        {
            get { return m_oChkEnable.Visible; }
            set
            {
                m_oChkEnable.Visible = value;
                Checked = Checked;

                if (value == false)
                {
                    m_oBtnShowTimeSelector.Enabled = true;
                }

                RightToLeft = RightToLeft;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Tells the control if the time is on the left or right side of the control")]
        public override RightToLeft RightToLeft
        {
            get { return m_eRightToLeft; }
            set { m_eRightToLeft = value; }
        }

        #endregion

        private int m_nAMPMRange;
        private int m_nEndRange;
        private int m_nHourRange;
        private int m_nMinuteRange;
        private int m_nSeparatorRange;

        // This private data member is used to only select the hours.  It can be
        // used in combination with the value Hour of the TimePickerFormat enum.
        // You can also use it by clicking on the hour in the time picker control.
        private HourSelectorForm m_oHourForm;

        // This private data member is used to select a time where the hours and
        // the minutes are involved.  It is also used by the design-time propertyValue.
        private HourMinuteSelectorForm m_oHourMinuteForm;

        // This private data member is used to select the minutes.  It is used
        // in the time picker when you click on the minutes.  You can also use it
        // when the TimeSlector property is set to Minutes.
        private MinuteSelectorForm m_oMinuteForm;

        private TimeSpan m_sOldTimeSelected;
        private String m_szAMPM;

        // Strings used when drawing the time in the control
        private String m_szHours;
        private String m_szMinutes;

        public TimePicker()
        {
            InitializeComponent();

            m_oHourMinuteForm = new HourMinuteSelectorForm();
            m_oHourMinuteForm.Size = new Size(346 + 2, 135 + 21);
            m_oHourMinuteForm.KeyToHidePicker = Keys.Escape;
            m_oHourMinuteForm.Deactivate += m_oHourMinuteForm_Deactivate;

            m_oHourForm = new HourSelectorForm();
            m_oHourForm.Size = new Size(292, 47);
            m_oHourForm.KeyToHidePicker = Keys.Escape;
            m_oHourForm.Deactivate += m_oHourForm_Deactivate;

            m_oMinuteForm = new MinuteSelectorForm();
            m_oMinuteForm.Size = new Size(242, 146);
            m_oMinuteForm.KeyToHidePicker = Keys.Escape;
            m_oMinuteForm.Deactivate += m_oMinuteForm_Deactivate;

            // If you want these properties to work in the designer,
            // you have to set their default values here.  There is
            // something wrong with the DefaultValue attribute
            // Also put them after the creation of the forms since
            // they set some properties in these forms.
            ButtonColor = SystemColors.Control;
            SelectedColor = SystemColors.Highlight;
            SeparatorColor = SystemColors.AppWorkspace;
            TimeSelector = TimePickerFormat.HoursAndMinutes;
            m_cSeparator = SEPARATOR;
            m_szSeparatorAsString = new String(new[]
                                                   {
                                                       SEPARATOR
                                                   });
            Format = "HH:MM";
            TimeSpan _lSTime = DateTime.Now.TimeOfDay;
            Value = new TimeSpan(_lSTime.Hours, _lSTime.Minutes, 0);
            ShowDropDown = true;
            ShowCheckBox = false;
            RightToLeft = RightToLeft.No;
        }

        #region Helper methods

        // Private method to display the correct time in the time picker control.
        // The following formats are currently supported :
        //  HH:MM = 00:00 to 23:59
        //   H:MM =  0:00 to 23:59
        //  hh:MM = 00:00 AM to 11:59 PM
        //   h:MM =  0:00 AM to 11:59 PM
        //  HH:M  = 00:0 to 23:59
        //   H:M  =  0:0 to 23:59
        //  hh:M  = 00:0 AM to 11:59 PM
        //   h:M  =  0:0 AM to 11:59 PM
        //
        // If the format string is incorrect, a message box will appear to indicate
        // that you must change the format.  Question marks will also appear in the
        // time picker component to give you a more visual hint of the error.
        private void GenerateFormatedText()
        {
            bool l_bAM = true;
            int l_nHour = m_sSelectedTime.Hours;
            int l_nMinute = m_sSelectedTime.Minutes;

            m_szHours = String.Empty;
            m_szMinutes = String.Empty;
            m_szAMPM = String.Empty;

            String[] l_aTimes = m_szTimeFormat.Split(m_cSeparator);

            if (l_aTimes.Length == 0 || l_aTimes.Length > 2)
            {
                MessageBox.Show(ERROR_WRONG_FORMAT_TEXT, ERROR_WRONG_FORMAT_TITLE, MessageBoxButtons.OK,
                                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                m_szHours = UNKNOWN;
                m_szMinutes = UNKNOWN;
                return;
            }

            // This section parse the hours
            if (l_aTimes[0].Equals(HH))
            {
                // The format is from 00 to 24
                if (l_nHour < 10)
                {
                    m_szHours = ZERO;
                }

                m_szHours += Convert.ToString(l_nHour);
            }
            else if (l_aTimes[0].Equals(H))
            {
                // The format is 0 to 24
                m_szHours = Convert.ToString(l_nHour);
            }
            else if (l_aTimes[0].Equals(hh))
            {
                // The format is 00 AM to 12 PM
                if (l_nHour > 12)
                {
                    l_nHour -= 12;
                    l_bAM = false;
                }
                else if (l_nHour == 12)
                {
                    l_bAM = false;
                }

                if (l_nHour < 10)
                {
                    m_szHours = ZERO;
                }

                m_szHours += Convert.ToString(l_nHour);
            }
            else if (l_aTimes[0].Equals(h))
            {
                // The format is 0 AM to 12 PM
                if (l_nHour > 12)
                {
                    l_nHour -= 12;
                    l_bAM = false;
                }
                else if (l_nHour == 12)
                {
                    l_bAM = false;
                }

                m_szHours = Convert.ToString(l_nHour);
            }


            if (l_aTimes[1].Equals(MM))
            {
                // The format is 00 to 59
                if (l_nMinute < 10)
                {
                    m_szMinutes = ZERO;
                }

                m_szMinutes += Convert.ToString(l_nMinute);
            }
            else if (l_aTimes[1].Equals(M))
            {
                // The format is 0 to 59
                m_szMinutes = Convert.ToString(l_nMinute);
            }

            if (l_aTimes[0].Equals(hh) ||
                l_aTimes[0].Equals(h))
            {
                m_szAMPM = (l_bAM ? AM : PM);
            }

            Refresh();
        }

        // Private method to place a form inside the screen.  This will prevent
        // the form of going outside the limits of the screen.
        private Point CalculateLocation(Size sSizeOfForm)
        {
            Point l_sPoint = PointToScreen(Location);

            l_sPoint = new Point(l_sPoint.X - Location.X, l_sPoint.Y + m_oPnlBackground.Height - Location.Y);

            if (l_sPoint.Y + sSizeOfForm.Height > Screen.PrimaryScreen.WorkingArea.Height)
            {
                // We are near the bottom of the screen.  If the form appears, it won't
                // be entirely shown on the screen.  So place the form above the 
                // Time Picker control.
                l_sPoint = new Point(l_sPoint.X, l_sPoint.Y - m_oPnlBackground.Height - sSizeOfForm.Height);
            }

            if (l_sPoint.X < 0)
            {
                // We are to much on the left side of the screen.  This causes the 
                // form to be outside the screen.  We have to push back the form 
                // on the edge of the screen.
                l_sPoint = new Point(0, l_sPoint.Y);
            }
            else if (l_sPoint.X + sSizeOfForm.Width > Screen.PrimaryScreen.WorkingArea.Width)
            {
                // This is the opposite of the previous statement.  We are now 
                // on the right side of the screen.  We have to stick the window to
                // the edge of the right border of the screen.
                l_sPoint = new Point(Screen.PrimaryScreen.WorkingArea.Width - sSizeOfForm.Width, l_sPoint.Y);
            }

            return l_sPoint;
        }

        private bool IsEnabled()
        {
            if (m_oChkEnable.Visible)
            {
                return m_oChkEnable.Checked;
            }
            else
            {
                return Enabled;
            }
        }

        #endregion Helper methods

        #region Events

        [Browsable(true)]
        public event EventHandler ValueChanged;

        private void FireEvents()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, new TimeChangeEventArgs(m_sOldTimeSelected, m_sSelectedTime));
            }
        }

        #endregion

        #region ToString overrides

        // Override of the function ToString inherited from the Object class.
        // It returns a string in the default format HH:MM.  Use the other
        // ToString method if you want to apply another format.
        // [eturns] Returns a formatted string with the default format HH:MM
        public override String ToString()
        {
            return ToString(HHMM);
        }

        // If you want a string representation of your choice, you can enter your
        // format.  The following format are currently supported :
        //  HH:MM = 00:00 to 23:59
        //   H:MM =  0:00 to 23:59
        //  hh:MM = 00:00 AM to 11:59 PM
        //   h:MM =  0:00 AM to 11:59 PM
        //  HH:M  = 00:0 to 23:59
        //   H:M  =  0:0 to 23:59
        //  hh:M  = 00:0 AM to 11:59 PM
        //   h:M  =  0:0 AM to 11:59 PM
        public String ToString(String szTimeFormat)
        {
            bool l_bAM = true;
            int l_nHour = m_sSelectedTime.Hours;
            int l_nMinute = m_sSelectedTime.Minutes;
            String l_szReturnedString = String.Empty;

            String[] l_aTimes = szTimeFormat.Split(m_cSeparator);

            if (l_aTimes.Length == 0 ||
                l_aTimes.Length > 2)
            {
                return l_szReturnedString;
            }

            // This section parse the hours
            if (l_aTimes[0].Equals(HH))
            {
                // The format is from 00 to 24
                if (l_nHour < 10)
                {
                    l_szReturnedString = ZERO;
                }

                l_szReturnedString += Convert.ToString(l_nHour);
            }
            else if (l_aTimes[0].Equals(H))
            {
                // The format is 0 to 24
                l_szReturnedString = Convert.ToString(l_nHour);
            }
            else if (l_aTimes[0].Equals(hh))
            {
                // The format is 00 AM to 12 PM
                if (l_nHour > 12)
                {
                    l_nHour -= 12;
                    l_bAM = false;
                }
                else if (l_nHour == 12)
                {
                    l_bAM = false;
                }

                if (l_nHour < 10)
                {
                    l_szReturnedString = ZERO;
                }

                l_szReturnedString += Convert.ToString(l_nHour);
            }
            else if (l_aTimes[0].Equals(h))
            {
                // The format is 0 AM to 12 PM
                if (l_nHour > 12)
                {
                    l_nHour -= 12;
                    l_bAM = false;
                }
                else if (l_nHour == 12)
                {
                    l_bAM = false;
                }

                l_szReturnedString = Convert.ToString(l_nHour);
            }

            l_szReturnedString += SEPARATOR;

            if (l_aTimes[1].Equals(MM))
            {
                // The format is 00 to 59
                if (l_nMinute < 10)
                {
                    l_szReturnedString += ZERO;
                }

                l_szReturnedString += Convert.ToString(l_nMinute);
            }
            else if (l_aTimes[1].Equals(M))
            {
                // The format is 0 to 59
                l_szReturnedString += Convert.ToString(l_nMinute);
            }

            if (l_aTimes[0].Equals(hh) ||
                l_aTimes[0].Equals(h))
            {
                l_szReturnedString += (l_bAM ? " " + AM : " " + PM);
            }

            return l_szReturnedString;
        }

        #endregion ToString overrides

        // SetBoundsCore is used to set the height of the TimePicker control.
        // Since we don't want the control to be resized in height, we must
        // override the SetBoundsCore method and modify the height of the control.
        // The impact of this override is in design-mode.  The developper won't be
        // able to change the height of the control.
        protected override void SetBoundsCore(int nX, int nY, int nWidth, int nHeight, BoundsSpecified eSpecifiedBound)
        {
            BoundsSpecified _t = eSpecifiedBound & BoundsSpecified.Size;

            if (eSpecifiedBound == BoundsSpecified.Size ||
                eSpecifiedBound == BoundsSpecified.All ||
                eSpecifiedBound == BoundsSpecified.None ||
                eSpecifiedBound == BoundsSpecified.Height)
            {
                nHeight = Font.Height + 7; // MIN_HEIGHT;
            }

            base.SetBoundsCore(nX, nY, nWidth, nHeight, eSpecifiedBound);
        }
    }

    // The TimePickerFormat enum is used with the TimeSelector property of the
    // TimePicker.  TimePickerFormat tells the TimePicker what kind of selection to show.
    public enum TimePickerFormat
    {
        Hours,
        Minutes,
        HoursAndMinutes
    }
}