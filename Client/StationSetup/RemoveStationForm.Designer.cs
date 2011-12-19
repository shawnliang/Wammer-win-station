namespace Wammer.Station
{
    partial class RemoveStationForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoveStationForm));
			this.label3 = new System.Windows.Forms.Label();
			this.labelName = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.buttonYes = new System.Windows.Forms.Button();
			this.buttonNo = new System.Windows.Forms.Button();
			this.cultureManager = new Waveface.Localization.CultureManager(this.components);
			this.SuspendLayout();
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.BackColor = System.Drawing.SystemColors.Window;
			this.label3.Name = "label3";
			// 
			// labelName
			// 
			resources.ApplyResources(this.labelName, "labelName");
			this.labelName.Name = "labelName";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.BackColor = System.Drawing.SystemColors.Window;
			this.label4.Name = "label4";
			// 
			// buttonYes
			// 
			resources.ApplyResources(this.buttonYes, "buttonYes");
			this.buttonYes.Name = "buttonYes";
			this.buttonYes.UseVisualStyleBackColor = true;
			this.buttonYes.Click += new System.EventHandler(this.buttonYes_Click);
			// 
			// buttonNo
			// 
			resources.ApplyResources(this.buttonNo, "buttonNo");
			this.buttonNo.Name = "buttonNo";
			this.buttonNo.UseVisualStyleBackColor = true;
			this.buttonNo.Click += new System.EventHandler(this.buttonNo_Click);
			// 
			// cultureManager
			// 
			this.cultureManager.ManagedControl = this;
			// 
			// RemoveStationForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.buttonNo);
			this.Controls.Add(this.buttonYes);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.label3);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RemoveStationForm";
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.DoubleClick += new System.EventHandler(this.RemoveStationForm_DoubleClick);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonYes;
        private System.Windows.Forms.Button buttonNo;
        private Waveface.Localization.CultureManager cultureManager;
    }
}