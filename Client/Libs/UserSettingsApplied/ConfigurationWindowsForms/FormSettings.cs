
#region

using System;
using System.ComponentModel;
using System.Windows.Forms;

#endregion

namespace Waveface.Configuration
{
    public class FormSettings : ApplicationSettings
    {
        private readonly Form m_form;
        private readonly PropertySetting m_topSetting;
        private readonly PropertySetting m_leftSetting;
        private readonly PropertySetting m_widthSetting;
        private readonly PropertySetting m_heightSetting;
        private readonly PropertySetting m_stateSetting;
        private DialogResult? m_saveCondition;

        #region Properties

        public Form Form
        {
            get { return m_form; }
        }

        public ISetting TopSetting
        {
            get { return m_topSetting; }
        }

        public ISetting LeftSetting
        {
            get { return m_leftSetting; }
        }

        public ISetting WidthSetting
        {
            get { return m_widthSetting; }
        }

        public ISetting HeightSetting
        {
            get { return m_heightSetting; }
        }

        public ISetting StateSetting
        {
            get { return m_stateSetting; }
        }

        public DialogResult? SaveCondition
        {
            get { return m_saveCondition; }
            set { m_saveCondition = value; }
        }

        public bool UseLocation { get; set; }

        public bool UseSize { get; set; }

        public bool UseWindowState { get; set; }

        public bool AllowMinimized { get; set; }

        public bool SaveOnClose { get; set; }

        #endregion

        public FormSettings(Form m_form) :
            this(m_form, m_form.GetType().Name)
        {
        }

        public FormSettings(Form form, string settingsKey) :
            base(settingsKey)
        {
            if (form == null)
            {
                throw new ArgumentNullException("form");
            }

            m_form = form;
            UseLocation = true;
            UseSize = true;
            UseWindowState = true;
            SaveOnClose = true;

            // settings 
            m_topSetting = CreateSetting("Window.Top", "Top");
            m_leftSetting = CreateSetting("Window.Left", "Left");
            m_widthSetting = CreateSetting("Window.Width", "Width");
            m_heightSetting = CreateSetting("Window.Height", "Height");
            m_stateSetting = CreateSetting("Window.WindowState", "WindowState");

            // subscribe to parent form's events
            m_form.Load += FormLoad;
            m_form.Closing += FormClosing;
        }

        public override void Save()
        {
            if (m_saveCondition.HasValue && m_saveCondition.Value != m_form.DialogResult)
            {
                return;
            }

            base.Save();
        }

        private void FormLoad(object sender, EventArgs e)
        {
            if (UseLocation)
            {
                Settings.Add(m_topSetting);
                Settings.Add(m_leftSetting);
            }

            if (UseSize)
            {
                Settings.Add(m_widthSetting);
                Settings.Add(m_heightSetting);
            }

            if (UseWindowState)
            {
                Settings.Add(m_stateSetting);
            }

            Load();
        }

        private void FormClosing(object sender, CancelEventArgs e)
        {
            if (!SaveOnClose)
            {
                return;
            }

            Save();
        }

        private PropertySetting CreateSetting(string name, string propertyName)
        {
            PropertySetting _propertySetting = new PropertySetting(name, m_form, propertyName);
            _propertySetting.ValueSaving += ValueSaving;
            return _propertySetting;
        }

        private void ValueSaving(object sender, SettingValueCancelEventArgs e)
        {
            if (AllowMinimized == false && m_form.WindowState == FormWindowState.Minimized)
            {
                e.Cancel = true;
            }
        }
    }
}