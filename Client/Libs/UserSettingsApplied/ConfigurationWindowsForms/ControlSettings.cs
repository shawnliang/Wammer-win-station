
#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

#endregion

namespace Waveface.Configuration
{
    public class ControlSettings : ApplicationSettings
    {
        private readonly Control m_control;

        public Control Control
        {
            get { return m_control; }
        }

        public bool SaveOnClose { get; set; }

        public ControlSettings(Control m_control) :
            this(m_control, m_control.GetType().Name)
        {
        }

        public ControlSettings(Control control, string settingsKey) :
            base(settingsKey)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            m_control = control;
            m_control.HandleCreated += ControlHandleCreated;
            m_control.HandleDestroyed += ControlHandleDestroyed;
           
            SaveOnClose = true;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            Load();
        }

        private void OnFormClosing(object sender, CancelEventArgs e)
        {
            if (SaveOnClose == false)
            {
                return;
            }

            Save();
        }

        private void ControlHandleCreated(object sender, EventArgs e)
        {
            Form _form = m_control.FindForm();

            if (_form == null)
            {
                Debug.WriteLine("ControlSettings: missing control form");
                return;
            }

            _form.Load += OnFormLoad;
            _form.Closing += OnFormClosing;
        }

        private void ControlHandleDestroyed(object sender, EventArgs e)
        {
            Form _form = m_control.FindForm();
            
            if (_form == null)
            {
                Debug.WriteLine("ControlSettings: missing control form");
                return;
            }

            _form.Load -= OnFormLoad;
            _form.Closing -= OnFormClosing;
        }
    }
}