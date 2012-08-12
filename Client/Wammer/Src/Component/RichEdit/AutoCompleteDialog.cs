
#region

using System.Windows.Forms;

#endregion

namespace Waveface.Component.RichEdit
{
    internal sealed class AutoCompleteDialog : Form
    {
        private ColumnHeader columnHeader1;

        private ListView listView;

        public ListView ListView
        {
            get { return listView; }
        }

        public AutoCompleteDialog()
        {
            InitializeComponent();
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.CausesValidation = false;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(176, 288);
            this.listView.TabIndex = 0;
            this.listView.TabStop = false;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // AutoCompleteDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            this.AutoScroll = true;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(176, 288);
            this.Controls.Add(this.listView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoCompleteDialog";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion InitializeComponent
    }
}