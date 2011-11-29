#region

using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Waveface.Localization.Samples;

#endregion

namespace Waveface.Localization.Editor
{
    public partial class FormEditor : Form
    {
        private Localizer m_localizer;
        private DataSet m_ds;

        public FormEditor()
        {
            InitializeComponent();
        }

        private void FormEditor_Load(object sender, EventArgs e)
        {
            m_localizer = new Localizer();

            setDataSourceDictionaries();

            webBrowser.Url = new Uri(m_localizer.WItemsFullPath);

            //Samples routines
            setDataSourceSampleLanguages();
        }

        private void loadAvailableCultures()
        {
            cboAvailablesCultures.ValueMember = "Name";
            cboAvailablesCultures.DisplayMember = "EnglishName";
            cboAvailablesCultures.DataSource = Localizer.AvailableCultures;
            cboAvailablesCultures.SelectedValue = m_localizer.CurrentCulture.Name;

            slblAvailableCulturesCount.Text = String.Concat("Available Cultures: ", Localizer.AvailableCultures.Count.ToString("#,##0"));
        }

        private void loadManagedCultures()
        {
            DataTable _dtManagedCultures = m_localizer.GetDtManagedCultures();
            cboManagedCultures.ValueMember = "ID";
            cboManagedCultures.DisplayMember = "Name";
            cboManagedCultures.DataSource = _dtManagedCultures;

            slblManagedCulturesCount.Text = String.Concat("Managed cultures: ", _dtManagedCultures.Rows.Count.ToString("#,##0"));
        }

        private void addManagedCulture()
        {
            string _cultureName = cboAvailablesCultures.SelectedValue.ToString();

            if (MessageBox.Show(String.Concat("Do you want to add the new Managed Culture (", _cultureName, ") ?"), Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            m_localizer.AddManagedCulture(_cultureName);
            
            if (!String.IsNullOrEmpty(m_localizer.ErrorString))
            {
                MessageBox.Show(m_localizer.ErrorString);
                return;
            }

            loadManagedCultures();
            setDataSourceDictionaries();

            //Samples routines
            setDataSourceSampleLanguages();
        }

        private void removeManagedCulture()
        {
            string _cultureName = cboManagedCultures.SelectedValue.ToString();

            if (MessageBox.Show(String.Concat("Are you sure you want to remove the selected Managed Culture (", _cultureName, ") ?\nYou will lose all your data of this culture."), Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            m_localizer.RemoveManagedCulture(_cultureName);
            
            if (!String.IsNullOrEmpty(m_localizer.ErrorString))
            {
                MessageBox.Show(m_localizer.ErrorString);
                return;
            }
            
            loadManagedCultures();
            setDataSourceDictionaries();

            //Samples routines
            setDataSourceSampleLanguages();
        }

        private void setDataSourceDictionaries()
        {
            m_ds = m_localizer.GetDsItems();
            
            if (!String.IsNullOrEmpty(m_localizer.ErrorString))
            {
                MessageBox.Show(m_localizer.ErrorString);
                return;
            }
            
            grdDictionaries.AutoGenerateColumns = true;
            grdDictionaries.DataMember = m_ds.Tables[0].TableName;
            grdDictionaries.DataSource = m_ds;

            webBrowser.Refresh();
        }

        private void cboManagedCulturesList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            setDataSourceDictionaries();
        }

        private void grdDictionaries_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete current row?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            string _code = e.Row.Cells["ID"].Value.ToString();
            
            if (!m_localizer.DeleteRecordItem(_code))
            {
                MessageBox.Show(m_localizer.ErrorString);
                return;
            }
            
            webBrowser.Refresh();
        }

        private void grdDictionaries_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //DataSet _dataSet = (DataSet) grdDictionaries.DataSource;

            if (!m_localizer.UpdateRecordItem(m_ds))
            {
                MessageBox.Show(m_localizer.ErrorString);
                return;
            }

            webBrowser.Refresh();
        }

        private void btnAddManagedCulture_Click(object sender, EventArgs e)
        {
            addManagedCulture();
        }

        private void btnRemoveManagedCulture_Click(object sender, EventArgs e)
        {
            removeManagedCulture();
        }

        private void tbtnNewDictionariesFile_Click(object sender, EventArgs e)
        {
            createDictionariesFile();
        }

        private void tbtnOpenDictionariesFile_Click(object sender, EventArgs e)
        {
            openDictionariesFile();
        }

        private void tbtnSaveAsDictionariesFile_Click(object sender, EventArgs e)
        {
            saveAsDictionariesFile();
        }

        private void openDictionariesFile()
        {
            OpenFileDialog _openFileDialog = new OpenFileDialog();
            _openFileDialog.Filter = "Xml files|*.xml";

            if (_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                m_localizer.WItemsFullPath = _openFileDialog.FileName;
                m_localizer.Refresh();

                webBrowser.Navigate(new Uri(m_localizer.WItemsFullPath));

                loadManagedCultures();
                setDataSourceDictionaries();

                //Samples routines
                setDataSourceSampleLanguages();
            }
        }

        private void saveAsDictionariesFile()
        {
            SaveFileDialog _saveFileDialog = new SaveFileDialog();
            _saveFileDialog.Filter = "Xml files|*.xml";
            
            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(m_localizer.WItemsFullPath, _saveFileDialog.FileName);
                m_localizer.WItemsFullPath = _saveFileDialog.FileName;
                m_localizer.CreateWItemsFile();
                m_localizer.Refresh();

                webBrowser.Navigate(new Uri(m_localizer.WItemsFullPath));

                loadManagedCultures();
                setDataSourceDictionaries();

                //Samples routines
                setDataSourceSampleLanguages();
            }
        }

        private void createDictionariesFile()
        {
            SaveFileDialog _saveFileDialog = new SaveFileDialog();
            _saveFileDialog.Filter = "Xml files|*.xml";
            
            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                m_localizer.WItemsFullPath = _saveFileDialog.FileName;
                m_localizer.CreateWItemsFile();
                m_localizer.Refresh();

                webBrowser.Navigate(new Uri(m_localizer.WItemsFullPath));

                loadManagedCultures();
                setDataSourceDictionaries();

                //Samples routines
                setDataSourceSampleLanguages();
            }
        }

        #region Samples Forms Routines

        private Localizer m_sampleLocalizer;

        private void setDataSourceSampleLanguages()
        {
            m_sampleLocalizer = m_localizer;

            cboSamplesLanguages.ValueMember = "ID";
            cboSamplesLanguages.DisplayMember = "Name";
            cboSamplesLanguages.DataSource = m_localizer.GetDtManagedCultures();
        }

        private void btnShowSampleFormSingleLanguage_Click(object sender, EventArgs e)
        {
            FormSample _f = new FormSample();
            _f.CurrentCultureName = cboSamplesLanguages.SelectedValue.ToString();
            _f.ShowDialog(this);
        }

        private void btnShowSingleTranslatedItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtItemToTranslate.Text))
                return;

            m_sampleLocalizer = new Localizer();
            m_sampleLocalizer.CurrentCulture = new CultureInfo(cboSamplesLanguages.SelectedValue.ToString());
            MessageBox.Show(m_sampleLocalizer.GetTranslatedText(txtItemToTranslate.Text));
        }

        #endregion

        private void cboSamplesLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            CultureManager.ApplicationUICulture = new CultureInfo(cboSamplesLanguages.SelectedValue.ToString());

            displayStatus();
        }

        private void displayStatus()
        {
            slblCurrentCultureName.Text = String.Concat("Current culture:", m_localizer.CurrentCulture.Name);
            slblDictionaryFileName.Text = String.Concat("File: ", m_localizer.WItemsFileName);                 

            loadAvailableCultures();

            loadManagedCultures();
        }
    }
}