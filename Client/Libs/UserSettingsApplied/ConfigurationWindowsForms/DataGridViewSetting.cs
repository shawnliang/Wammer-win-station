
#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;

#endregion

namespace Waveface.Configuration
{
    public class DataGridViewSetting : Setting
    {
        private readonly DataGridView m_dataGridView;
        private readonly string m_name;

        public DataGridViewSetting(DataGridView dataGridView) :
            this(dataGridView.Name, dataGridView)
        {
        }

        public DataGridViewSetting(string name, DataGridView dataGridView)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (dataGridView == null)
            {
                throw new ArgumentNullException("dataGridView");
            }

            m_name = name;
            m_dataGridView = dataGridView;
            UseVisibility = true;
            UseWidth = true;
            UseDisplayIndex = true;
        }

        #region Properties

        public string Name
        {
            get { return m_name; }
        }

        public DataGridView DataGridView
        {
            get { return m_dataGridView; }
        }

        public bool UseVisibility { get; set; }

        public bool UseWidth { get; set; }

        public bool UseDisplayIndex { get; set; }

        public override bool HasChanged
        {
            get
            {
                DataGridViewColumnSetting[] _originalColumnSettings = OriginalColumnSettings;
                DataGridViewColumnSetting[] _columnSettings = ColumnSettings;
                
                if (_originalColumnSettings == null || _columnSettings == null ||
                    _originalColumnSettings == _columnSettings)
                {
                    return false;
                }

                if (_originalColumnSettings.Length != _columnSettings.Length)
                {
                    return true;
                }

                for (int i = 0; i < _originalColumnSettings.Length; i++)
                {
                    if (!_originalColumnSettings[i].Equals(_columnSettings[i]))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private DataGridViewColumnSetting[] OriginalColumnSettings
        {
            get
            {
                return LoadValue(
                    Name,
                    typeof (DataGridViewColumnSetting[]),
                    SettingsSerializeAs.Binary,
                    null) as DataGridViewColumnSetting[];
            }
        }

        private DataGridViewColumnSetting[] ColumnSettings
        {
            get
            {
                if (m_dataGridView.Columns.Count == 0)
                {
                    return null;
                }

                List<DataGridViewColumnSetting> _columns =  new List<DataGridViewColumnSetting>(m_dataGridView.Columns.Count);

                foreach (DataGridViewColumn _dataGridViewColumn in m_dataGridView.Columns)
                {
                    _columns.Add(new DataGridViewColumnSetting(_dataGridViewColumn));
                }

                return _columns.ToArray();
            }
        }

        #endregion

        public override void Load()
        {
            try
            {
                DataGridViewColumnSetting[] _columnSettings = OriginalColumnSettings;
               
                if (_columnSettings == null || _columnSettings.Length == 0)
                {
                    return;
                }

                foreach (DataGridViewColumnSetting _columnSetting in _columnSettings)
                {
                    DataGridViewColumn _dataGridViewColumn = m_dataGridView.Columns[_columnSetting.Name];
                   
                    if (_dataGridViewColumn == null)
                    {
                        continue;
                    }

                    if (UseVisibility)
                    {
                        _dataGridViewColumn.Visible = _columnSetting.Visible;
                    }

                    if (UseWidth)
                    {
                        _dataGridViewColumn.Width = _columnSetting.Width;
                    }

                    if (UseDisplayIndex)
                    {
                        _dataGridViewColumn.DisplayIndex = _columnSetting.DisplayIndex;
                    }
                }
            }
            catch
            {
                if (ThrowOnErrorLoading)
                {
                    throw;
                }
            }
        }

        public override void Save()
        {
            try
            {
                DataGridViewColumnSetting[] _columnSettings = ColumnSettings;
                
                if (_columnSettings == null)
                {
                    return;
                }

                SaveValue(
                    Name,
                    typeof (DataGridViewColumnSetting[]),
                    SettingsSerializeAs.Binary,
                    _columnSettings,
                    null);
            }
            catch
            {
                if (ThrowOnErrorSaving)
                {
                    throw;
                }
            }
        }

        public override string ToString()
        {
            return string.Concat(m_name, " (DataGridView)");
        }
    }
}