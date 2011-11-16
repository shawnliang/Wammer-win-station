
#region

using System;
using System.Windows.Controls;

#endregion

namespace Waveface.Configuration
{
    [Serializable]
    internal class GridViewColumnSetting
    {
        private int m_index;
        private int m_displayIndex;
        private double m_width;

        #region Properties

        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        public int DisplayIndex
        {
            get { return m_displayIndex; }
            set { m_displayIndex = value; }
        }

        public double Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        #endregion

        public GridViewColumnSetting(GridViewColumn gridViewColumn, int index, int displayIndex)
        {
            if (gridViewColumn == null)
            {
                throw new ArgumentNullException("gridViewColumn");
            }

            m_index = index;
            m_displayIndex = displayIndex;
            m_width = gridViewColumn.Width;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            GridViewColumnSetting _compare = obj as GridViewColumnSetting;

            if (_compare == null)
            {
                return false;
            }

            return
                Equals(m_index, _compare.m_index) &&
                Equals(m_displayIndex, _compare.m_displayIndex) &&
                Equals(m_width, _compare.m_width);
        }

        public override int GetHashCode()
        {
            int _hash = GetType().GetHashCode();
            _hash = AddHashCode(_hash, m_index);
            _hash = AddHashCode(_hash, m_displayIndex);
            _hash = AddHashCode(_hash, m_width);
            return _hash;
        }

        private static int AddHashCode(int hash, object obj)
        {
            int _combinedHash = obj != null ? obj.GetHashCode() : 0;
          
            if (hash != 0)
            {
                _combinedHash += hash*31;
            }

            return _combinedHash;
        }
    }
}