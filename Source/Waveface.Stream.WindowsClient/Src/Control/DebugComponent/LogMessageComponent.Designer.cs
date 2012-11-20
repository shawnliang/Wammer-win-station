namespace Waveface.Stream.WindowsClient
{
    partial class LogMessageComponent
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.logMessageListBox1 = new Waveface.Stream.WindowsClient.LogMessageListBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1154, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // logMessageListBox1
            // 
            this.logMessageListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logMessageListBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.logMessageListBox1.FormattingEnabled = true;
            this.logMessageListBox1.Location = new System.Drawing.Point(0, 25);
            this.logMessageListBox1.Name = "logMessageListBox1";
            this.logMessageListBox1.Size = new System.Drawing.Size(1154, 357);
            this.logMessageListBox1.TabIndex = 1;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(36, 22);
            this.toolStripLabel1.Text = "Clear";
            this.toolStripLabel1.Click += new System.EventHandler(this.toolStripLabel1_Click);
            // 
            // LogMessageComponent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logMessageListBox1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "LogMessageComponent";
            this.Size = new System.Drawing.Size(1154, 382);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private LogMessageListBox logMessageListBox1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
    }
}
