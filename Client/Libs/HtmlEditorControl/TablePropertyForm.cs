#region

using System;
using System.ComponentModel;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.HtmlEditor
{
    // Form used to enter an Html Table structure
    // Input based on the HtmlTableProperty struct
    internal class TablePropertyForm : Form
    {
        private Container components;
        private Button bCancel;
        private Button bInsert;
        private TextBox textTableCaption;
        private Label labelCaption;
        private Label labelCaptionAlign;
        private Label labelLocation;
        private GroupBox groupLayout;
        private GroupBox groupCaption;
        private Label labelRowColumn;
        private NumericUpDown numericRows;
        private NumericUpDown numericColumns;
        private Label labelPadding;
        private NumericUpDown numericCellPadding;
        private Label labelSpacing;
        private NumericUpDown numericCellSpacing;
        private Label labelWidth;
        private NumericUpDown numericTableWidth;
        private ComboBox listCaptionAlignment;
        private ComboBox listCaptionLocation;
        private GroupBox groupTable;
        private NumericUpDown numericBorderSize;
        private RadioButton radioWidthPercent;
        private Label labelBorderAlign;
        private Label labelBorderSize;
        private Panel groupPercentPixel;
        private ComboBox listTextAlignment;
        private RadioButton radioWidthPixel;

        // private variable for the table properties
        private HtmlTableProperty m_tableProperties;

        // constants for the Maximum values
        private const byte MAXIMUM_CELL_ROW = 250;
        private const byte MAXIMUM_CELL_COL = 50;
        private const byte MAXIMUM_CELL_PAD = 25;
        private const byte MAXIMUM_BORDER = 25;
        private const ushort MAXIMUM_WIDTH_PERCENT = 100;
        private const ushort MAXIMUM_WIDTH_PIXEL = 2500;
        private const ushort WIDTH_INC_DIV = 20;

        public TablePropertyForm()
        {
            InitializeComponent();

            // define the dropdown list value
            listCaptionAlignment.Items.AddRange(Enum.GetNames(typeof (HorizontalAlignOption)));
            listCaptionLocation.Items.AddRange(Enum.GetNames(typeof (VerticalAlignOption)));
            listTextAlignment.Items.AddRange(Enum.GetNames(typeof (HorizontalAlignOption)));

            // ensure default values are listed in the drop down lists
            listCaptionAlignment.SelectedIndex = 0;
            listCaptionLocation.SelectedIndex = 0;
            listTextAlignment.SelectedIndex = 0;

            // define the new maximum values of the dialogs
            numericBorderSize.Maximum = MAXIMUM_BORDER;
            numericCellPadding.Maximum = MAXIMUM_CELL_PAD;
            numericCellSpacing.Maximum = MAXIMUM_CELL_PAD;
            numericRows.Maximum = MAXIMUM_CELL_ROW;
            numericColumns.Maximum = MAXIMUM_CELL_COL;
            numericTableWidth.Maximum = MAXIMUM_WIDTH_PIXEL;

            // define default values based on a new table
            TableProperties = new HtmlTableProperty(true);
        }

        public HtmlTableProperty TableProperties
        {
            get
            {
                // define the appropriate table caption properties
                m_tableProperties.CaptionText = textTableCaption.Text;
                m_tableProperties.CaptionAlignment = (HorizontalAlignOption) listCaptionAlignment.SelectedIndex;
                m_tableProperties.CaptionLocation = (VerticalAlignOption) listCaptionLocation.SelectedIndex;
                // define the appropriate table specific properties
                m_tableProperties.BorderSize = (byte) Math.Min(numericBorderSize.Value, numericBorderSize.Maximum);
                m_tableProperties.TableAlignment = (HorizontalAlignOption) listTextAlignment.SelectedIndex;
                // define the appropriate table layout properties
                m_tableProperties.TableRows = (byte) Math.Min(numericRows.Value, numericRows.Maximum);
                m_tableProperties.TableColumns = (byte) Math.Min(numericColumns.Value, numericColumns.Maximum);
                m_tableProperties.CellPadding = (byte) Math.Min(numericCellPadding.Value, numericCellPadding.Maximum);
                m_tableProperties.CellSpacing = (byte) Math.Min(numericCellSpacing.Value, numericCellSpacing.Maximum);
                m_tableProperties.TableWidth = (ushort) Math.Min(numericTableWidth.Value, numericTableWidth.Maximum);
                m_tableProperties.TableWidthMeasurement = (radioWidthPercent.Checked)
                                                            ? MeasurementOption.Percent
                                                            : MeasurementOption.Pixel;
                // return the properties
                return m_tableProperties;
            }
            set
            {
                // persist the new values
                m_tableProperties = value;
               
                // define the dialog caption properties
                textTableCaption.Text = m_tableProperties.CaptionText;
                listCaptionAlignment.SelectedIndex = (int) m_tableProperties.CaptionAlignment;
                listCaptionLocation.SelectedIndex = (int) m_tableProperties.CaptionLocation;
               
                // define the dialog table specific properties
                numericBorderSize.Value = Math.Min(m_tableProperties.BorderSize, MAXIMUM_BORDER);
                listTextAlignment.SelectedIndex = (int) m_tableProperties.TableAlignment;
                
                // define the dialog table layout properties
                numericRows.Value = Math.Min(m_tableProperties.TableRows, MAXIMUM_CELL_ROW);
                numericColumns.Value = Math.Min(m_tableProperties.TableColumns, MAXIMUM_CELL_COL);
                numericCellPadding.Value = Math.Min(m_tableProperties.CellPadding, MAXIMUM_CELL_PAD);
                numericCellSpacing.Value = Math.Min(m_tableProperties.CellSpacing, MAXIMUM_CELL_PAD);
                radioWidthPercent.Checked = (m_tableProperties.TableWidthMeasurement == MeasurementOption.Percent);
                radioWidthPixel.Checked = (m_tableProperties.TableWidthMeasurement == MeasurementOption.Pixel);
                numericTableWidth.Value = Math.Min(m_tableProperties.TableWidth, numericTableWidth.Maximum);
            }
        }

        // define the measurement values based on the selected mesaurment selected
        private void MeasurementOptionChanged(object sender, EventArgs e)
        {
            // define a dialog for a percentage change
            if (radioWidthPercent.Checked)
            {
                numericTableWidth.Maximum = MAXIMUM_WIDTH_PERCENT;
                numericTableWidth.Increment = MAXIMUM_WIDTH_PERCENT/WIDTH_INC_DIV;
            }

            // define a dialog for a pixel change
            if (radioWidthPixel.Checked)
            {
                numericTableWidth.Maximum = MAXIMUM_WIDTH_PIXEL;
                numericTableWidth.Increment = MAXIMUM_WIDTH_PIXEL/WIDTH_INC_DIV;
            }
        }

        // Clean up any resources being used.
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TablePropertyForm));
            this.bCancel = new System.Windows.Forms.Button();
            this.bInsert = new System.Windows.Forms.Button();
            this.groupCaption = new System.Windows.Forms.GroupBox();
            this.listCaptionLocation = new System.Windows.Forms.ComboBox();
            this.labelLocation = new System.Windows.Forms.Label();
            this.listCaptionAlignment = new System.Windows.Forms.ComboBox();
            this.labelCaptionAlign = new System.Windows.Forms.Label();
            this.labelCaption = new System.Windows.Forms.Label();
            this.textTableCaption = new System.Windows.Forms.TextBox();
            this.groupLayout = new System.Windows.Forms.GroupBox();
            this.numericCellSpacing = new System.Windows.Forms.NumericUpDown();
            this.labelSpacing = new System.Windows.Forms.Label();
            this.numericCellPadding = new System.Windows.Forms.NumericUpDown();
            this.labelPadding = new System.Windows.Forms.Label();
            this.numericColumns = new System.Windows.Forms.NumericUpDown();
            this.numericRows = new System.Windows.Forms.NumericUpDown();
            this.labelRowColumn = new System.Windows.Forms.Label();
            this.groupPercentPixel = new System.Windows.Forms.Panel();
            this.radioWidthPixel = new System.Windows.Forms.RadioButton();
            this.radioWidthPercent = new System.Windows.Forms.RadioButton();
            this.numericTableWidth = new System.Windows.Forms.NumericUpDown();
            this.labelWidth = new System.Windows.Forms.Label();
            this.groupTable = new System.Windows.Forms.GroupBox();
            this.listTextAlignment = new System.Windows.Forms.ComboBox();
            this.labelBorderAlign = new System.Windows.Forms.Label();
            this.labelBorderSize = new System.Windows.Forms.Label();
            this.numericBorderSize = new System.Windows.Forms.NumericUpDown();
            this.groupCaption.SuspendLayout();
            this.groupLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericCellSpacing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericCellPadding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericColumns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericRows)).BeginInit();
            this.groupPercentPixel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericTableWidth)).BeginInit();
            this.groupTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericBorderSize)).BeginInit();
            this.SuspendLayout();
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(328, 353);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 26);
            this.bCancel.TabIndex = 0;
            this.bCancel.Text = "Cancel";
            // 
            // bInsert
            // 
            this.bInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bInsert.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bInsert.Location = new System.Drawing.Point(248, 353);
            this.bInsert.Name = "bInsert";
            this.bInsert.Size = new System.Drawing.Size(75, 26);
            this.bInsert.TabIndex = 1;
            this.bInsert.Text = "Insert";
            // 
            // groupCaption
            // 
            this.groupCaption.Controls.Add(this.listCaptionLocation);
            this.groupCaption.Controls.Add(this.labelLocation);
            this.groupCaption.Controls.Add(this.listCaptionAlignment);
            this.groupCaption.Controls.Add(this.labelCaptionAlign);
            this.groupCaption.Controls.Add(this.labelCaption);
            this.groupCaption.Controls.Add(this.textTableCaption);
            this.groupCaption.Location = new System.Drawing.Point(8, 9);
            this.groupCaption.Name = "groupCaption";
            this.groupCaption.Size = new System.Drawing.Size(395, 102);
            this.groupCaption.TabIndex = 2;
            this.groupCaption.TabStop = false;
            this.groupCaption.Text = "Caption Properties";
            // 
            // listCaptionLocation
            // 
            this.listCaptionLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listCaptionLocation.Location = new System.Drawing.Point(274, 65);
            this.listCaptionLocation.Name = "listCaptionLocation";
            this.listCaptionLocation.Size = new System.Drawing.Size(104, 22);
            this.listCaptionLocation.TabIndex = 8;
            // 
            // labelLocation
            // 
            this.labelLocation.Location = new System.Drawing.Point(200, 65);
            this.labelLocation.Name = "labelLocation";
            this.labelLocation.Size = new System.Drawing.Size(64, 26);
            this.labelLocation.TabIndex = 7;
            this.labelLocation.Text = "Location :";
            // 
            // listCaptionAlignment
            // 
            this.listCaptionAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listCaptionAlignment.Location = new System.Drawing.Point(90, 65);
            this.listCaptionAlignment.Name = "listCaptionAlignment";
            this.listCaptionAlignment.Size = new System.Drawing.Size(104, 22);
            this.listCaptionAlignment.TabIndex = 6;
            // 
            // labelCaptionAlign
            // 
            this.labelCaptionAlign.Location = new System.Drawing.Point(8, 65);
            this.labelCaptionAlign.Name = "labelCaptionAlign";
            this.labelCaptionAlign.Size = new System.Drawing.Size(76, 26);
            this.labelCaptionAlign.TabIndex = 5;
            this.labelCaptionAlign.Text = "Alignment :";
            // 
            // labelCaption
            // 
            this.labelCaption.Location = new System.Drawing.Point(8, 28);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Size = new System.Drawing.Size(64, 26);
            this.labelCaption.TabIndex = 1;
            this.labelCaption.Text = "Caption :";
            // 
            // textTableCaption
            // 
            this.textTableCaption.Location = new System.Drawing.Point(80, 28);
            this.textTableCaption.Name = "textTableCaption";
            this.textTableCaption.Size = new System.Drawing.Size(298, 22);
            this.textTableCaption.TabIndex = 0;
            // 
            // groupLayout
            // 
            this.groupLayout.Controls.Add(this.numericCellSpacing);
            this.groupLayout.Controls.Add(this.labelSpacing);
            this.groupLayout.Controls.Add(this.numericCellPadding);
            this.groupLayout.Controls.Add(this.labelPadding);
            this.groupLayout.Controls.Add(this.numericColumns);
            this.groupLayout.Controls.Add(this.numericRows);
            this.groupLayout.Controls.Add(this.labelRowColumn);
            this.groupLayout.Location = new System.Drawing.Point(8, 231);
            this.groupLayout.Name = "groupLayout";
            this.groupLayout.Size = new System.Drawing.Size(384, 111);
            this.groupLayout.TabIndex = 3;
            this.groupLayout.TabStop = false;
            this.groupLayout.Text = "Cell Properties";
            // 
            // numericCellSpacing
            // 
            this.numericCellSpacing.Location = new System.Drawing.Point(256, 74);
            this.numericCellSpacing.Name = "numericCellSpacing";
            this.numericCellSpacing.Size = new System.Drawing.Size(56, 22);
            this.numericCellSpacing.TabIndex = 6;
            // 
            // labelSpacing
            // 
            this.labelSpacing.Location = new System.Drawing.Point(168, 74);
            this.labelSpacing.Name = "labelSpacing";
            this.labelSpacing.Size = new System.Drawing.Size(80, 26);
            this.labelSpacing.TabIndex = 5;
            this.labelSpacing.Text = "Cell Spacing :";
            // 
            // numericCellPadding
            // 
            this.numericCellPadding.Location = new System.Drawing.Point(96, 74);
            this.numericCellPadding.Name = "numericCellPadding";
            this.numericCellPadding.Size = new System.Drawing.Size(56, 22);
            this.numericCellPadding.TabIndex = 4;
            // 
            // labelPadding
            // 
            this.labelPadding.Location = new System.Drawing.Point(8, 74);
            this.labelPadding.Name = "labelPadding";
            this.labelPadding.Size = new System.Drawing.Size(80, 26);
            this.labelPadding.TabIndex = 3;
            this.labelPadding.Text = "Cell Padding :";
            // 
            // numericColumns
            // 
            this.numericColumns.Location = new System.Drawing.Point(203, 28);
            this.numericColumns.Name = "numericColumns";
            this.numericColumns.Size = new System.Drawing.Size(56, 22);
            this.numericColumns.TabIndex = 2;
            // 
            // numericRows
            // 
            this.numericRows.Location = new System.Drawing.Point(139, 28);
            this.numericRows.Name = "numericRows";
            this.numericRows.Size = new System.Drawing.Size(56, 22);
            this.numericRows.TabIndex = 1;
            // 
            // labelRowColumn
            // 
            this.labelRowColumn.Location = new System.Drawing.Point(8, 28);
            this.labelRowColumn.Name = "labelRowColumn";
            this.labelRowColumn.Size = new System.Drawing.Size(128, 26);
            this.labelRowColumn.TabIndex = 0;
            this.labelRowColumn.Text = "Rows and Columns :";
            // 
            // groupPercentPixel
            // 
            this.groupPercentPixel.Controls.Add(this.radioWidthPixel);
            this.groupPercentPixel.Controls.Add(this.radioWidthPercent);
            this.groupPercentPixel.Location = new System.Drawing.Point(152, 55);
            this.groupPercentPixel.Name = "groupPercentPixel";
            this.groupPercentPixel.Size = new System.Drawing.Size(194, 41);
            this.groupPercentPixel.TabIndex = 9;
            // 
            // radioWidthPixel
            // 
            this.radioWidthPixel.Location = new System.Drawing.Point(89, 10);
            this.radioWidthPixel.Name = "radioWidthPixel";
            this.radioWidthPixel.Size = new System.Drawing.Size(56, 28);
            this.radioWidthPixel.TabIndex = 1;
            this.radioWidthPixel.Text = "Pixels";
            this.radioWidthPixel.CheckedChanged += new System.EventHandler(this.MeasurementOptionChanged);
            // 
            // radioWidthPercent
            // 
            this.radioWidthPercent.Location = new System.Drawing.Point(8, 9);
            this.radioWidthPercent.Name = "radioWidthPercent";
            this.radioWidthPercent.Size = new System.Drawing.Size(75, 28);
            this.radioWidthPercent.TabIndex = 0;
            this.radioWidthPercent.Text = "Percent";
            this.radioWidthPercent.CheckedChanged += new System.EventHandler(this.MeasurementOptionChanged);
            // 
            // numericTableWidth
            // 
            this.numericTableWidth.Location = new System.Drawing.Point(72, 65);
            this.numericTableWidth.Name = "numericTableWidth";
            this.numericTableWidth.Size = new System.Drawing.Size(64, 22);
            this.numericTableWidth.TabIndex = 8;
            // 
            // labelWidth
            // 
            this.labelWidth.Location = new System.Drawing.Point(8, 65);
            this.labelWidth.Name = "labelWidth";
            this.labelWidth.Size = new System.Drawing.Size(56, 26);
            this.labelWidth.TabIndex = 7;
            this.labelWidth.Text = "Width :";
            // 
            // groupTable
            // 
            this.groupTable.Controls.Add(this.listTextAlignment);
            this.groupTable.Controls.Add(this.labelBorderAlign);
            this.groupTable.Controls.Add(this.labelBorderSize);
            this.groupTable.Controls.Add(this.numericBorderSize);
            this.groupTable.Controls.Add(this.labelWidth);
            this.groupTable.Controls.Add(this.numericTableWidth);
            this.groupTable.Controls.Add(this.groupPercentPixel);
            this.groupTable.Location = new System.Drawing.Point(8, 120);
            this.groupTable.Name = "groupTable";
            this.groupTable.Size = new System.Drawing.Size(390, 102);
            this.groupTable.TabIndex = 4;
            this.groupTable.TabStop = false;
            this.groupTable.Text = "Table Properties";
            // 
            // listTextAlignment
            // 
            this.listTextAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listTextAlignment.Location = new System.Drawing.Point(274, 25);
            this.listTextAlignment.Name = "listTextAlignment";
            this.listTextAlignment.Size = new System.Drawing.Size(104, 22);
            this.listTextAlignment.TabIndex = 6;
            // 
            // labelBorderAlign
            // 
            this.labelBorderAlign.Location = new System.Drawing.Point(192, 28);
            this.labelBorderAlign.Name = "labelBorderAlign";
            this.labelBorderAlign.Size = new System.Drawing.Size(76, 26);
            this.labelBorderAlign.TabIndex = 5;
            this.labelBorderAlign.Text = "Alignment :";
            // 
            // labelBorderSize
            // 
            this.labelBorderSize.Location = new System.Drawing.Point(8, 28);
            this.labelBorderSize.Name = "labelBorderSize";
            this.labelBorderSize.Size = new System.Drawing.Size(56, 26);
            this.labelBorderSize.TabIndex = 4;
            this.labelBorderSize.Text = "Border :";
            // 
            // numericBorderSize
            // 
            this.numericBorderSize.Location = new System.Drawing.Point(72, 28);
            this.numericBorderSize.Name = "numericBorderSize";
            this.numericBorderSize.Size = new System.Drawing.Size(104, 22);
            this.numericBorderSize.TabIndex = 3;
            // 
            // TablePropertyForm
            // 
            this.AcceptButton = this.bInsert;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(410, 390);
            this.Controls.Add(this.groupTable);
            this.Controls.Add(this.groupLayout);
            this.Controls.Add(this.groupCaption);
            this.Controls.Add(this.bInsert);
            this.Controls.Add(this.bCancel);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TablePropertyForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Table Properties";
            this.groupCaption.ResumeLayout(false);
            this.groupCaption.PerformLayout();
            this.groupLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericCellSpacing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericCellPadding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericColumns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericRows)).EndInit();
            this.groupPercentPixel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericTableWidth)).EndInit();
            this.groupTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericBorderSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}