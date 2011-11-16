
#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace Waveface.Configuration
{
    public class ListViewSetting : Setting
    {
        private readonly ListView m_listView;
        private readonly List<GridViewColumn> m_gridViewColumns = new List<GridViewColumn>();

        private readonly string m_name;
        private bool m_useWidth = true;
        private bool m_useDisplayIndex = true;

        #region Properties

        public string Name
        {
            get { return m_name; }
        }

        public ListView ListView
        {
            get { return m_listView; }
        }

        public bool UseWidth
        {
            get { return m_useWidth; }
            set { m_useWidth = value; }
        }

        public bool UseDisplayIndex
        {
            get { return m_useDisplayIndex; }
            set { m_useDisplayIndex = value; }
        }

        public override bool HasChanged
        {
            get
            {
                GridViewColumnSetting[] _originalColumnSettings = OriginalColumnSettings;
                GridViewColumnSetting[] _columnSettings = ColumnSettings;

                if (_originalColumnSettings == null || _columnSettings == null || _originalColumnSettings == _columnSettings)
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

        private GridViewColumnSetting[] OriginalColumnSettings
        {
            get
            {
                return LoadValue(
                    Name,
                    typeof(GridViewColumnSetting[]),
                    SettingsSerializeAs.Binary,
                    null) as GridViewColumnSetting[];
            }
        }

        private GridViewColumnSetting[] ColumnSettings
        {
            get
            {
                GridView _gridView = m_listView.View as GridView;

                if (_gridView == null || _gridView.Columns.Count == 0)
                {
                    return null;
                }

                List<GridViewColumnSetting> _columnSettings = new List<GridViewColumnSetting>(_gridView.Columns.Count);

                for (int displayIndex = 0; displayIndex < _gridView.Columns.Count; displayIndex++)
                {
                    GridViewColumn _gridViewColumn = _gridView.Columns[displayIndex];
                    int _index = m_gridViewColumns.IndexOf(_gridViewColumn);
                    _columnSettings.Add(new GridViewColumnSetting(_gridViewColumn, _index, displayIndex));
                }

                return _columnSettings.ToArray();
            }
        }

        #endregion

        public static readonly DependencyProperty SettingProperty =
            DependencyProperty.RegisterAttached(
                "Setting",
                typeof(string),
                typeof(ListViewSetting),
                new FrameworkPropertyMetadata(OnListViewSettingChanged));


        public ListViewSetting(ListView listView) :
            this(listView.Name, listView)
        {
        }

        public ListViewSetting(string name, ListView listView)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (listView == null)
            {
                throw new ArgumentNullException("listView");
            }

            m_name = name;
            m_listView = listView;
            listView.Initialized += ListViewInitialized;
            SetupGridViewColumns();
        }

        public static string GetSetting(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(SettingProperty) as string;
        }

        public static void SetSetting(DependencyObject dependencyObject, string settingKey)
        {
            dependencyObject.SetValue(SettingProperty, settingKey);
        }

        public override void Load()
        {
            try
            {
                GridView _gridView = m_listView.View as GridView;

                if (_gridView == null || _gridView.Columns.Count == 0)
                {
                    return;
                }

                GridViewColumnSetting[] _columnSettings = OriginalColumnSettings;

                if (_columnSettings == null || _columnSettings.Length == 0)
                {
                    return;
                }

                for (int displayIndex = 0; displayIndex < _columnSettings.Length; displayIndex++)
                {
                    GridViewColumnSetting _columnSetting = _columnSettings[displayIndex];
                   
                    if (_columnSetting.Index < 0 || _columnSetting.Index >= m_gridViewColumns.Count)
                    {
                        continue;
                    }

                    GridViewColumn _gridViewColumn = m_gridViewColumns[_columnSetting.Index];

                    if (m_useWidth)
                    {
                        _gridViewColumn.Width = _columnSetting.Width;
                    }

                    if (!m_useDisplayIndex)
                    {
                        continue;
                    }

                    if (_columnSetting.Index == _columnSetting.DisplayIndex)
                    {
                        continue;
                    }

                    int _oldIndex = _gridView.Columns.IndexOf(_gridViewColumn);
                    _gridView.Columns.Move(_oldIndex, _columnSetting.DisplayIndex);
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
                GridViewColumnSetting[] _columnSettings = ColumnSettings;
               
                if (_columnSettings == null)
                {
                    return;
                }

                SaveValue(
                    Name,
                    typeof(GridViewColumnSetting[]),
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
            return string.Concat(m_name, " (ListView)");
        }

        private void SetupGridViewColumns()
        {
            m_gridViewColumns.Clear();

            GridView _gridView = m_listView.View as GridView;
           
            if (_gridView == null)
            {
                return;
            }

            for (int i = 0; i < _gridView.Columns.Count; i++)
            {
                m_gridViewColumns.Add(_gridView.Columns[i]);
            }
        }

        private void ListViewInitialized(object sender, EventArgs e)
        {
            SetupGridViewColumns();
        }

        private static void OnListViewSettingChanged(DependencyObject dependencyObject,
                                                     DependencyPropertyChangedEventArgs e)
        {
            ListView _listView = dependencyObject as ListView;

            if (_listView == null)
            {
                Debug.WriteLine("ListViewSetting: invalid property attachment");
                return;
            }

            // search on the parent-tree for application settings
            ApplicationSettings _applicationSettings = FindParentSettings(dependencyObject);
            
            if (_applicationSettings == null)
            {
                Debug.WriteLine("ListViewSetting: missing application settings in parent hierarchy");
                return;
            }

            _applicationSettings.Settings.Add(new ListViewSetting(_listView));
        }

        private static ApplicationSettings FindParentSettings(DependencyObject element)
        {
            while (element != null)
            {
                ApplicationSettings _applicationSettings =
                    element.ReadLocalValue(DependencyPropertySetting.ApplicationSettingsProperty) as ApplicationSettings;
               
                if (_applicationSettings != null)
                {
                    return _applicationSettings;
                }

                element = LogicalTreeHelper.GetParent(element);
            }

            return null;
        }
    }
}