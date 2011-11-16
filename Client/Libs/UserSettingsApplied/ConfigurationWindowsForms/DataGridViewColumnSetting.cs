
#region

using System;
using System.Windows.Forms;

#endregion

namespace Waveface.Configuration
{
    [Serializable]
    internal class DataGridViewColumnSetting
    {
        public int DisplayIndex { get; set; }
        public int Width { get; set; }
        public string Name { get; set; }
        public bool Visible { get; set; }

        public DataGridViewColumnSetting(DataGridViewColumn dataGridViewColumn)
        {
            if (dataGridViewColumn == null)
            {
                throw new ArgumentNullException("dataGridViewColumn");
            }

            DisplayIndex = dataGridViewColumn.DisplayIndex;
            Width = dataGridViewColumn.Width;
            Name = dataGridViewColumn.Name;
            Visible = dataGridViewColumn.Visible;
        }
     
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            DataGridViewColumnSetting _compare = obj as DataGridViewColumnSetting;
            
            if (_compare == null)
            {
                return false;
            }

            return
                string.Equals(Name, _compare.Name) &&
                Equals(DisplayIndex, _compare.DisplayIndex) &&
                Equals(Width, _compare.Width) &&
                Equals(Visible, _compare.Visible);
        }

        public override int GetHashCode()
        {
            int _hash = GetType().GetHashCode();
            _hash = AddHashCode(_hash, Name);
            _hash = AddHashCode(_hash, DisplayIndex);
            _hash = AddHashCode(_hash, Width);
            _hash = AddHashCode(_hash, Visible);
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