#region

using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using Waveface.UI.FilterUI;

#endregion

namespace Waveface.FilterUI
{
    public partial class FilterManager : Form
    {
        public FilterManager()
        {
            InitializeComponent();

            (new TabOrderManager(this)).SetTabOrder(TabOrderManager.TabScheme.AcrossFirst);

            ResetAllUI();
        }

        private void ResetAllUI()
        {
            tabControl.SelectedIndex = 0;

            textBoxName.Text = "";
            cmbType.SelectedIndex = 0;
            datePicker_TR_From.Value = DateTime.Today;
            datePicker_TR_To.Value = DateTime.Today;
            comboBox_TS_PN.SelectedIndex = 0;
            datePicker_TS_Time.Value = DateTime.Today;
            numericUpDown_TS_Limit.Value = 20;

            textBoxName.Focus();
        }

        private void addFileButton_Click(object sender, EventArgs e)
        {
            if (!PromptSave())
                return;

            ResetAllUI();
        }

        private bool PromptSave()
        {
            if (tabControl.SelectedIndex == 0)
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

                _from = _from.ToUniversalTime();
                _to = _to.ToUniversalTime();

                _to = _to.AddHours(23);
                _to = _to.AddMinutes(59);
                _to = _to.AddSeconds(59);

                FilterTime _ft = new FilterTime();
                _ft.time = new string[2];
                _ft.time[0] = DateTimeHelp.ToISO8601(_from);
                _ft.time[1] = DateTimeHelp.ToISO8601(_to);
                _ft.type = getTypeString();

                MessageBox.Show(JsonConvert.SerializeObject(_ft));
            }
            else
            {
                DateTime _dt = datePicker_TS_Time.Value.ToUniversalTime();
                string _limit = comboBox_TS_PN.Text == "<" ? "-" : "+";
                _limit += numericUpDown_TS_Limit.Value;

                FilterTimeStamp _fts = new FilterTimeStamp();
                _fts.timestamp = DateTimeHelp.ToISO8601(_dt);
                _fts.limit = _limit;
                _fts.type = getTypeString();

                MessageBox.Show(JsonConvert.SerializeObject(_fts));
            }

            return true;
        }

        private string getTypeString()
        {
            string _ret = string.Empty;
            string _type = cmbType.Text;

            switch (_type)
            {
                case "All":
                    return "";

                case "Text":
                    return "text";

                case "Image":
                    return "image";

                case "Web Link":
                    return "link";

                case "Document":
                    return "doc";

                case "Rich Text":
                    return "rtf";
            }

            return "";
        }
    }
}