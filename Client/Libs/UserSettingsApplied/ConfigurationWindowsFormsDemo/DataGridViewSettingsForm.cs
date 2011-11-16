
#region

using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Waveface.Configuration;

#endregion

namespace Waveface.Solutions.Community.ConfigurationWindowsFormsDemo
{
    public partial class DataGridViewSettingsForm : Form
    {
        private readonly FormSettings m_formSettings;

        public DataGridViewSettingsForm()
        {
            InitializeComponent();

            m_formSettings = new FormSettings(this);
            m_formSettings.SaveOnClose = false; // disable auto-save
            m_formSettings.Settings.Add(new DataGridViewSetting(customersDataGridView));

            customersDataGridView.DataSource = LoadCustomers();
        }

        private static List<Customer> LoadCustomers()
        {
            List<Customer> _customers = new List<Customer>();

            for (int i = 0; i < 100; i++)
            {
                string _userId = (i + 1).ToString();
                
                Customer _customer = new Customer(
                    "FisrtName" + _userId,
                    "LastName" + _userId,
                    "Street" + _userId,
                    "City" + _userId,
                    "ZipCode" + _userId);

                _customers.Add(_customer);
            }

            return _customers;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (DialogResult == DialogResult.Cancel)
            {
                return; // is canceling
            }

            if (!m_formSettings.Settings.HasChanges)
            {
                return; // nothing to do
            }

            StringBuilder _sb = new StringBuilder();

            foreach (ISetting _setting in m_formSettings.Settings)
            {
                if (!_setting.HasChanged)
                {
                    continue;
                }

                _sb.Append(" - ");
                _sb.Append(_setting.ToString());
                _sb.Append("\n");
            }

            DialogResult _result = MessageBox.Show("Save changes?\n\n" + _sb, Text, MessageBoxButtons.YesNoCancel);
           
            switch (_result)
            {
                case DialogResult.Yes:
                    m_formSettings.Save();
                    break;
                case DialogResult.No:
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }
    }
}