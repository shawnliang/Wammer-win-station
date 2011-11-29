namespace Waveface.Localization.Editor
{
    partial class FormEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditor));
            this.cboAvailablesCultures = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddManagedCulture = new System.Windows.Forms.Button();
            this.cboManagedCultures = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grdDictionaries = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.slblCurrentCultureName = new System.Windows.Forms.ToolStripStatusLabel();
            this.slblAvailableCulturesCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.slblManagedCulturesCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.slblDictionaryFileName = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnRemoveManagedCulture = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageEditor = new System.Windows.Forms.TabPage();
            this.tabPageXmlFile = new System.Windows.Forms.TabPage();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.tabPageTest = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnShowSampleFormSingleLanguage = new System.Windows.Forms.Button();
            this.txtItemToTranslate = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cboSamplesLanguages = new System.Windows.Forms.ComboBox();
            this.btnShowSingleTranslatedItem = new System.Windows.Forms.Button();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.tbtnNewDictionariesFile = new System.Windows.Forms.ToolStripButton();
            this.tbtnOpenDictionariesFile = new System.Windows.Forms.ToolStripButton();
            this.tbtnSaveAsDictionariesFile = new System.Windows.Forms.ToolStripButton();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grdDictionaries)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageEditor.SuspendLayout();
            this.tabPageXmlFile.SuspendLayout();
            this.tabPageTest.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboAvailablesCultures
            // 
            resources.ApplyResources(this.cboAvailablesCultures, "cboAvailablesCultures");
            this.cboAvailablesCultures.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAvailablesCultures.FormattingEnabled = true;
            this.cboAvailablesCultures.Name = "cboAvailablesCultures";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btnAddManagedCulture
            // 
            resources.ApplyResources(this.btnAddManagedCulture, "btnAddManagedCulture");
            this.btnAddManagedCulture.Name = "btnAddManagedCulture";
            this.btnAddManagedCulture.UseVisualStyleBackColor = true;
            this.btnAddManagedCulture.Click += new System.EventHandler(this.btnAddManagedCulture_Click);
            // 
            // cboManagedCultures
            // 
            resources.ApplyResources(this.cboManagedCultures, "cboManagedCultures");
            this.cboManagedCultures.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboManagedCultures.FormattingEnabled = true;
            this.cboManagedCultures.Name = "cboManagedCultures";
            this.cboManagedCultures.SelectionChangeCommitted += new System.EventHandler(this.cboManagedCulturesList_SelectionChangeCommitted);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // grdDictionaries
            // 
            resources.ApplyResources(this.grdDictionaries, "grdDictionaries");
            this.grdDictionaries.AllowUserToOrderColumns = true;
            this.grdDictionaries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdDictionaries.Name = "grdDictionaries";
            this.grdDictionaries.RowTemplate.Height = 24;
            this.grdDictionaries.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdDictionaries_CellEndEdit);
            this.grdDictionaries.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.grdDictionaries_UserDeletingRow);
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slblCurrentCultureName,
            this.slblAvailableCulturesCount,
            this.slblManagedCulturesCount,
            this.slblDictionaryFileName});
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            // 
            // slblCurrentCultureName
            // 
            resources.ApplyResources(this.slblCurrentCultureName, "slblCurrentCultureName");
            this.slblCurrentCultureName.Name = "slblCurrentCultureName";
            this.slblCurrentCultureName.Spring = true;
            // 
            // slblAvailableCulturesCount
            // 
            resources.ApplyResources(this.slblAvailableCulturesCount, "slblAvailableCulturesCount");
            this.slblAvailableCulturesCount.Name = "slblAvailableCulturesCount";
            this.slblAvailableCulturesCount.Spring = true;
            // 
            // slblManagedCulturesCount
            // 
            resources.ApplyResources(this.slblManagedCulturesCount, "slblManagedCulturesCount");
            this.slblManagedCulturesCount.Name = "slblManagedCulturesCount";
            this.slblManagedCulturesCount.Spring = true;
            // 
            // slblDictionaryFileName
            // 
            resources.ApplyResources(this.slblDictionaryFileName, "slblDictionaryFileName");
            this.slblDictionaryFileName.Name = "slblDictionaryFileName";
            // 
            // btnRemoveManagedCulture
            // 
            resources.ApplyResources(this.btnRemoveManagedCulture, "btnRemoveManagedCulture");
            this.btnRemoveManagedCulture.Name = "btnRemoveManagedCulture";
            this.btnRemoveManagedCulture.UseVisualStyleBackColor = true;
            this.btnRemoveManagedCulture.Click += new System.EventHandler(this.btnRemoveManagedCulture_Click);
            // 
            // tabControl
            // 
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.Controls.Add(this.tabPageEditor);
            this.tabControl.Controls.Add(this.tabPageXmlFile);
            this.tabControl.Controls.Add(this.tabPageTest);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            // 
            // tabPageEditor
            // 
            resources.ApplyResources(this.tabPageEditor, "tabPageEditor");
            this.tabPageEditor.Controls.Add(this.grdDictionaries);
            this.tabPageEditor.Controls.Add(this.cboAvailablesCultures);
            this.tabPageEditor.Controls.Add(this.btnRemoveManagedCulture);
            this.tabPageEditor.Controls.Add(this.label1);
            this.tabPageEditor.Controls.Add(this.btnAddManagedCulture);
            this.tabPageEditor.Controls.Add(this.cboManagedCultures);
            this.tabPageEditor.Controls.Add(this.label2);
            this.tabPageEditor.Name = "tabPageEditor";
            this.tabPageEditor.UseVisualStyleBackColor = true;
            // 
            // tabPageXmlFile
            // 
            resources.ApplyResources(this.tabPageXmlFile, "tabPageXmlFile");
            this.tabPageXmlFile.Controls.Add(this.webBrowser);
            this.tabPageXmlFile.Name = "tabPageXmlFile";
            this.tabPageXmlFile.UseVisualStyleBackColor = true;
            // 
            // webBrowser
            // 
            resources.ApplyResources(this.webBrowser, "webBrowser");
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.MinimumSize = new System.Drawing.Size(23, 21);
            this.webBrowser.Name = "webBrowser";
            // 
            // tabPageTest
            // 
            resources.ApplyResources(this.tabPageTest, "tabPageTest");
            this.tabPageTest.Controls.Add(this.groupBox1);
            this.tabPageTest.Name = "tabPageTest";
            this.tabPageTest.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.btnShowSampleFormSingleLanguage);
            this.groupBox1.Controls.Add(this.txtItemToTranslate);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cboSamplesLanguages);
            this.groupBox1.Controls.Add(this.btnShowSingleTranslatedItem);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // btnShowSampleFormSingleLanguage
            // 
            resources.ApplyResources(this.btnShowSampleFormSingleLanguage, "btnShowSampleFormSingleLanguage");
            this.btnShowSampleFormSingleLanguage.Name = "btnShowSampleFormSingleLanguage";
            this.btnShowSampleFormSingleLanguage.UseVisualStyleBackColor = true;
            this.btnShowSampleFormSingleLanguage.Click += new System.EventHandler(this.btnShowSampleFormSingleLanguage_Click);
            // 
            // txtItemToTranslate
            // 
            resources.ApplyResources(this.txtItemToTranslate, "txtItemToTranslate");
            this.txtItemToTranslate.Name = "txtItemToTranslate";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // cboSamplesLanguages
            // 
            resources.ApplyResources(this.cboSamplesLanguages, "cboSamplesLanguages");
            this.cboSamplesLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSamplesLanguages.FormattingEnabled = true;
            this.cboSamplesLanguages.Name = "cboSamplesLanguages";
            this.cboSamplesLanguages.SelectedIndexChanged += new System.EventHandler(this.cboSamplesLanguages_SelectedIndexChanged);
            // 
            // btnShowSingleTranslatedItem
            // 
            resources.ApplyResources(this.btnShowSingleTranslatedItem, "btnShowSingleTranslatedItem");
            this.btnShowSingleTranslatedItem.Name = "btnShowSingleTranslatedItem";
            this.btnShowSingleTranslatedItem.UseVisualStyleBackColor = true;
            this.btnShowSingleTranslatedItem.Click += new System.EventHandler(this.btnShowSingleTranslatedItem_Click);
            // 
            // toolStrip
            // 
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnNewDictionariesFile,
            this.tbtnOpenDictionariesFile,
            this.tbtnSaveAsDictionariesFile});
            this.toolStrip.Name = "toolStrip";
            // 
            // tbtnNewDictionariesFile
            // 
            resources.ApplyResources(this.tbtnNewDictionariesFile, "tbtnNewDictionariesFile");
            this.tbtnNewDictionariesFile.Name = "tbtnNewDictionariesFile";
            this.tbtnNewDictionariesFile.Click += new System.EventHandler(this.tbtnNewDictionariesFile_Click);
            // 
            // tbtnOpenDictionariesFile
            // 
            resources.ApplyResources(this.tbtnOpenDictionariesFile, "tbtnOpenDictionariesFile");
            this.tbtnOpenDictionariesFile.Name = "tbtnOpenDictionariesFile";
            this.tbtnOpenDictionariesFile.Click += new System.EventHandler(this.tbtnOpenDictionariesFile_Click);
            // 
            // tbtnSaveAsDictionariesFile
            // 
            resources.ApplyResources(this.tbtnSaveAsDictionariesFile, "tbtnSaveAsDictionariesFile");
            this.tbtnSaveAsDictionariesFile.Name = "tbtnSaveAsDictionariesFile";
            this.tbtnSaveAsDictionariesFile.Click += new System.EventHandler(this.tbtnSaveAsDictionariesFile_Click);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // FormEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip1);
            this.Name = "FormEditor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdDictionaries)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageEditor.ResumeLayout(false);
            this.tabPageEditor.PerformLayout();
            this.tabPageXmlFile.ResumeLayout(false);
            this.tabPageTest.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboAvailablesCultures;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAddManagedCulture;
        private System.Windows.Forms.ComboBox cboManagedCultures;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView grdDictionaries;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel slblCurrentCultureName;
        private System.Windows.Forms.ToolStripStatusLabel slblAvailableCulturesCount;
        private System.Windows.Forms.ToolStripStatusLabel slblManagedCulturesCount;
        private System.Windows.Forms.Button btnRemoveManagedCulture;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageEditor;
        private System.Windows.Forms.TabPage tabPageXmlFile;
        public System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton tbtnOpenDictionariesFile;
        private System.Windows.Forms.ToolStripButton tbtnSaveAsDictionariesFile;
        private System.Windows.Forms.TabPage tabPageTest;
        private System.Windows.Forms.Button btnShowSampleFormSingleLanguage;
        private System.Windows.Forms.ComboBox cboSamplesLanguages;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnShowSingleTranslatedItem;
        private System.Windows.Forms.TextBox txtItemToTranslate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolStripButton tbtnNewDictionariesFile;
        private System.Windows.Forms.ToolStripStatusLabel slblDictionaryFileName;
        private CultureManager cultureManager;
    }
}

