#region

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using UtilityLibrary.WinControls;
using Waveface;
using Waveface.Component;
using Waveface.Windows.Forms;

#endregion

namespace MobileMind.Utility
{
    public class ApplicationInfoDialog : Form
    {
        private Container components;
        private ListViewEx listViewSysInfo;
        private PropertyGrid propertyGrid1;
        private FlatTabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private LinkLabel linkLabel1;
        private XPButton okButton;
        private ApplicationInformation m_applicationInformation;

        public ApplicationInfoDialog()
        {
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int dropShadow = 0x00020000;
                CreateParams param = base.CreateParams;

                if (ApplicationInformation.CheckIfWinXP())
                    param.ClassStyle |= dropShadow;

                return param;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.listViewSysInfo = new UtilityLibrary.WinControls.ListViewEx();
            this.tabControl1 = new FlatTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.okButton = new XPButton();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.CommandsVisibleIfAvailable = true;
            this.propertyGrid1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular,
                                                              System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
            this.propertyGrid1.LargeButtons = false;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid1.Location = new System.Drawing.Point(8, 8);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(483, 278);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.TabStop = false;
            this.propertyGrid1.Text = "propertyGrid1";
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
            this.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
            // 
            // listViewSysInfo
            // 
            this.listViewSysInfo.CheckBookEvenRowBackColor = System.Drawing.Color.Empty;
            this.listViewSysInfo.CheckBookEvenRowForeColor = System.Drawing.Color.Empty;
            this.listViewSysInfo.CheckBookLookEnabled = false;
            this.listViewSysInfo.CheckBookOddRowBackColor = System.Drawing.Color.Empty;
            this.listViewSysInfo.CheckBookOddRowForeColor = System.Drawing.Color.Empty;
            this.listViewSysInfo.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular,
                                                                System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
            this.listViewSysInfo.FullRowSelect = true;
            this.listViewSysInfo.HeaderColor = System.Drawing.Color.Empty;
            this.listViewSysInfo.HeaderImageList = null;
            this.listViewSysInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewSysInfo.Location = new System.Drawing.Point(8, 8);
            this.listViewSysInfo.Name = "listViewSysInfo";
            this.listViewSysInfo.PaintSortedColumnBackground = true;
            this.listViewSysInfo.Size = new System.Drawing.Size(482, 272);
            this.listViewSysInfo.SortingEnabled = true;
            this.listViewSysInfo.SortOrder = System.Windows.Forms.SortOrder.Ascending;
            this.listViewSysInfo.TabIndex = 0;
            this.listViewSysInfo.TabStop = false;
            this.listViewSysInfo.View = System.Windows.Forms.View.Details;
            // 
            // tabControl1
            // 
            this.tabControl1.BorderDarkColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tabControl1.BorderLightColor = System.Drawing.SystemColors.ControlDark;
            this.tabControl1.ControlBackColor = System.Drawing.SystemColors.Control;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(8, 8);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(504, 320);
            this.tabControl1.TabAlignment = FlatTabControl.FlatTabAlignment.Top;
            this.tabControl1.TabIndex = 2;
            this.tabControl1.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.propertyGrid1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(496, 291);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = " Information ";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listViewSysInfo);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(496, 291);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Loaded Assemblies";
            // 
            // okButton
            // 
            this.okButton.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.okButton.BtnShape = emunType.BtnShape.Rectangle;
            this.okButton.BtnStyle = emunType.XPStyle.Silver;
            this.okButton.ImageIndex = 0;
            this.okButton.Location = new System.Drawing.Point(440, 336);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 28);
            this.okButton.TabIndex = 42;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Location = new System.Drawing.Point(8, 344);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(128, 24);
            this.linkLabel1.TabIndex = 43;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "System Information";
            this.linkLabel1.LinkClicked +=
                new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // ApplicationInfoDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(522, 372);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular,
                                                System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApplicationInfoDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Application Information";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            m_applicationInformation = new ApplicationInformation();
            propertyGrid1.SelectedObject = m_applicationInformation;
            InitListViewInfo();
            DoLA();
        }

        private void DoLA()
        {
            ArrayList arrayList = new ArrayList();

            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly _asm in assemblys)
            {
                string _s;

                AssemblyName _assemblyName = _asm.GetName();

                if (arrayList != null)
                    arrayList.Add(_assemblyName);

                Version _version = _assemblyName.Version;

                if (_version == null)
                    _version = new Version(0, 0, 0, 0);

                if (_asm.GlobalAssemblyCache)
                    _s = "Global Assembly Cache";
                else if (_assemblyName != null && _assemblyName.CodeBase.Length != 0)
                    _s = Path.GetDirectoryName(new Uri(_assemblyName.CodeBase, true).LocalPath);
                else
                    _s = String.Empty;

                string[] _strs = new[] {_assemblyName.Name, _version.ToString(), _s};
                ListViewItem _listViewItem = new ListViewItem(_strs);
                listViewSysInfo.Items.Add(_listViewItem);
            }
        }

        #region ListViewSysInfo

        private ColumnHeader _columnHeader1;
        private ColumnHeader _columnHeader2;
        private ColumnHeader _columnHeader3;

        private void InitListViewInfo()
        {
            _columnHeader1 = new ColumnHeader();
            _columnHeader2 = new ColumnHeader();
            _columnHeader3 = new ColumnHeader();

            listViewSysInfo.Columns.AddRange(new[]
                                                 {
                                                     _columnHeader1,
                                                     _columnHeader2,
                                                     _columnHeader3
                                                 });
            _columnHeader1.Text = "Name";
            _columnHeader1.Width = 180;
            _columnHeader2.Text = "Version";
            _columnHeader2.Width = 80;
            _columnHeader3.Text = "Location";
            _columnHeader3.Width = 201;

            // Sorting settings
            listViewSysInfo.SetColumnSortFormat(0, SortedListViewFormatType.String);
            listViewSysInfo.SetColumnSortFormat(1, SortedListViewFormatType.String);
            listViewSysInfo.SetColumnSortFormat(2, SortedListViewFormatType.String);
        }

        #endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("MSInfo32.exe");
            }
            catch
            {
            }

            okButton.Focus();
        }
    }
}