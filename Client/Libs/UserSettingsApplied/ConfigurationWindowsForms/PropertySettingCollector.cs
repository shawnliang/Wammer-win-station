#region

using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

#endregion

namespace Waveface.Configuration
{
    public class PropertySettingCollector : SettingCollector
    {
        private readonly Control m_owner;
        private readonly Type m_elementType;
        private readonly string m_propertyName;

        public Control Owner
        {
            get { return m_owner; }
        }

        public Type ElementType
        {
            get { return m_elementType; }
        }

        public string PropertyName
        {
            get { return m_propertyName; }
        }

        public PropertySettingCollector(Control owner, Type elementType, string propertyName)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            m_owner = owner;
            m_elementType = elementType;
            m_propertyName = propertyName;
        }

        public override void Collect()
        {
            Collect(m_owner.Controls, true);
        }

        private void Collect(Control.ControlCollection children, bool recursive)
        {
            foreach (Control _control in children)
            {
                bool _add = _control.GetType().IsAssignableFrom(m_elementType);

                string _controlId = null;
               
                if (_add)
                {
                    _controlId = GetControlId(_control);
                   
                    if (string.IsNullOrEmpty(_controlId))
                    {
                        _add = false;
                        Debug.WriteLine("PropertySettingCollector: missing id for control " + _control);
                    }
                }

                if (_add && !OnCollectingSetting(_control))
                {
                    _add = false;
                }

                if (_add)
                {
                    string _settingName = string.Concat(_controlId, ".", m_propertyName);
                    PropertySetting _propertySetting = new PropertySetting(_settingName, _control, m_propertyName);
                    _propertySetting.DefaultValue = _propertySetting.Value;
                    ApplicationSettings.Settings.Add(_propertySetting);
                }

                if (recursive && _control.Controls.Count > 0)
                {
                    Collect(_control.Controls, true);
                }
            }
        }

        private string GetControlId(Control control)
        {
            if (control.Parent == m_owner)
            {
                return control.Name;
            }

            StringBuilder _sb = new StringBuilder();

            while (control != null && control != m_owner)
            {
                if (_sb.Length > 0)
                {
                    _sb.Insert(0, ".");
                }

                _sb.Insert(0, control.Name);
                control = control.Parent;
            }

            return _sb.ToString();
        }
    }
}