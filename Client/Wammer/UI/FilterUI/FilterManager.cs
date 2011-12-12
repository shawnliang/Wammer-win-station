#region

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using Waveface.API.V2;

#endregion

namespace Waveface.FilterUI
{
    public partial class FilterManager : Form
    {
        private FilterItem m_currentFilterItem;
        private string m_filterString;
        private TimeRangeFilter m_timeRangeFilter;
        private TimeStampFilter m_timeStampFilter;
        private bool m_isAddMode;

        public LeftArea MyParent { get; set; }

        public FilterManager()
        {
            InitializeComponent();

            (new TabOrderManager(this)).SetTabOrder(TabOrderManager.TabScheme.AcrossFirst);

            ResetAll();

            FillListview();
        }

        private void FillListview()
        {
            listViewFiles.Items.Clear();

            List<Fetch_Filter> _filters = FilterHelper.GetList();

            if (_filters != null)
            {
                foreach (Fetch_Filter _f in _filters)
                {
                    FilterItem _item = new FilterItem();
                    _item.Name = _f.filter_name;
                    _item.Filter = _f.filter_entity.ToString();
                    _item.IsAllPost = false;
                    _item.searchfilter_id = _f.filter_id;

                    ListViewItem _lvi = new ListViewItem(_f.filter_name, 0);
                    _lvi.Tag = _item;

                    listViewFiles.Items.Add(_lvi);
                }
            }
        }

        #region Events

        private void rbTime_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTimeRange.Checked)
            {
                panelTimeRange.Visible = true;
                panelTimeStamp.Visible = false;
            }
            else
            {
                panelTimeRange.Visible = false;
                panelTimeStamp.Visible = true;
            }
        }

        private void btnSaveUpdate_Click(object sender, EventArgs e)
        {
            if(m_isAddMode)
                DoSave();
            else
                DoUpdate();
        }

        private void addFileButton_Click(object sender, EventArgs e)
        {
            ResetAll();
        }

        private void listViewFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                m_isAddMode = false;

                btnSaveUpdate.Text = "Update";
                Text = "Filter Manager [Edit]";

                m_currentFilterItem = (FilterItem)listViewFiles.SelectedItems[0].Tag;
                SetUiUsingFilterString(m_currentFilterItem);
                m_filterString = m_currentFilterItem.Filter;
            }
        }

        #endregion

        private void DoSave()
        {
            if (textBoxName.Text == string.Empty)
            {
                MessageBox.Show("Please enter a value for the \"Name\" field.", "Waveface", MessageBoxButtons.OK);
                return;
            }

            MR_fetchfilters_item _item = Main.Current.RT.REST.FetchFilters_New(textBoxName.Text, GetFilterString(), "");

            if (_item == null)
            {
                MessageBox.Show("Add New Filter error!");
            }
            else
            {
                MessageBox.Show("Add New Filter success!");

                ResetAll();
                FillListview();
            }
        }

        private void DoUpdate()
        {
            if (textBoxName.Text == string.Empty)
            {
                MessageBox.Show("Please enter a value for the \"Name\" field.", "Waveface", MessageBoxButtons.OK);
                return;
            }

            MR_fetchfilters_item _item = Main.Current.RT.REST.FetchFilters_Update(m_currentFilterItem.searchfilter_id, textBoxName.Text, GetFilterString(), "");

            if (_item == null)
            {
                MessageBox.Show("Update Filter error!");
            }
            else
            {
                MessageBox.Show("Update Filter success!");

                ResetAll();
                FillListview();
            }
        }

        #region FilterString

        private string GetFilterString()
        {
            if (rbTimeRange.Checked)
            {
                return CreateTimeRangeFilterString();
            }
            else
            {
                return CreateTimeStampFilterString();
            }
        }

        private TimeStampFilter CreateTimeStampFilter()
        {
            int _value = (int)numericUpDown_TS_Limit.Value;
            int _limit = comboBox_TS_PN.Text == "<" ? (_value * -1) : _value;

            return FilterHelper.GetTimeStampFilter(datePicker_TS_Time.Value, _limit, getTypeString(), "[offset]");
        }

        private string CreateTimeStampFilterString()
        {
            return FilterHelper.GetTimeStampFilterJson(CreateTimeStampFilter());
        }

        private TimeRangeFilter CreateTimeRangeFilter()
        {
            DateTime _from;
            DateTime _to;

            DateTime _dt1 = datePicker_TR_From.Value;
            DateTime _dt2 = datePicker_TR_To.Value;

            if (_dt1 > _dt2)
            {
                _from = _dt2;
                _to = _dt1;
            }
            else
            {
                _from = _dt1;
                _to = _dt2;
            }

            _to = _to.AddHours(23);
            _to = _to.AddMinutes(59);
            _to = _to.AddSeconds(59);

            return FilterHelper.GetTimeRangeFilter(_from, _to, 10, getTypeString(), "[offset]");
        }

        private string CreateTimeRangeFilterString()
        {
            return FilterHelper.GetTimeRangeFilterJson(CreateTimeRangeFilter());
        }

        #endregion

        #region getType

        private string getTypeString()
        {
            string _type = cmbType.Text;

            switch (_type)
            {
                case "All":
                    return "all";

                case "Text":
                    return "text";

                case "Image":
                    return "image";

                case "Web Link":
                    return "link";

                case "Rich Text":
                    return "rtf";

                case "Document":
                    return "doc";
            }

            return "";
        }

        private int getIndexFromTypeString(string type)
        {
            switch (type)
            {
                case "all":
                    return 0;

                case "text":
                    return 1;

                case "image":
                    return 2;

                case "link":
                    return 3;

                case "rtf":
                    return 4;

                case "doc":
                    return 5;
            }

            return -1;
        }

        #endregion

        private void ResetAll()
        {
            m_isAddMode = true;

            m_filterString = string.Empty;
            m_currentFilterItem = null;

            m_timeRangeFilter = null;
            m_timeStampFilter = null;

            btnSaveUpdate.Text = "Save";
            Text = "Filter Manager [New]";

            rbTimeRange.Checked = true;
            textBoxName.Text = "";
            cmbType.SelectedIndex = 0;
            datePicker_TR_From.Value = DateTime.Today;
            datePicker_TR_To.Value = DateTime.Today;
            comboBox_TS_PN.SelectedIndex = 0;
            datePicker_TS_Time.Value = DateTime.Today;
            numericUpDown_TS_Limit.Value = 20;

            textBoxName.Focus();
        }

        private void SetUiUsingFilterString(FilterItem filterItem)
        {
            textBoxName.Text = filterItem.Name;

            string _filterString = filterItem.Filter;

            m_timeRangeFilter = null;
            m_timeStampFilter = null;

            //Hack
            try
            {
                JsonSerializerSettings _settings = new JsonSerializerSettings();
                _settings.MissingMemberHandling = MissingMemberHandling.Error;
                m_timeRangeFilter = JsonConvert.DeserializeObject<TimeRangeFilter>(_filterString, _settings);

                rbTimeRange.Checked = true;

                datePicker_TR_From.Value = DateTimeHelp.ISO8601ToDateTime(m_timeRangeFilter.time[0]);
                datePicker_TR_To.Value = DateTimeHelp.ISO8601ToDateTime(m_timeRangeFilter.time[1]);
                cmbType.SelectedIndex = getIndexFromTypeString(m_timeRangeFilter.type);
            }
            catch
            {
                try
                {
                    m_timeStampFilter = JsonConvert.DeserializeObject<TimeStampFilter>(_filterString);

                    rbTimeStamp.Checked = true;

                    comboBox_TS_PN.SelectedIndex = (m_timeStampFilter.limit < 0) ? 0 : 1;
                    datePicker_TS_Time.Value = DateTimeHelp.ISO8601ToDateTime(m_timeStampFilter.timestamp);
                    numericUpDown_TS_Limit.Value = Math.Abs(m_timeStampFilter.limit);
                    cmbType.SelectedIndex = getIndexFromTypeString(m_timeStampFilter.type);
                }
                catch
                {
                }
            }
        }
    }
}