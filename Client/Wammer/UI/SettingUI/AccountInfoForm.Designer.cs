namespace Waveface
{
    partial class AccountInfoForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountInfoForm));
			this.label1 = new System.Windows.Forms.Label();
			this.lblSince = new System.Windows.Forms.Label();
			this.lblIsFacebookImportEnabled = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.logoutButton1 = new StationSystemTray.LogoutButton();
			this.label10 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// lblSince
			// 
			resources.ApplyResources(this.lblSince, "lblSince");
			this.lblSince.Name = "lblSince";
			// 
			// lblIsFacebookImportEnabled
			// 
			resources.ApplyResources(this.lblIsFacebookImportEnabled, "lblIsFacebookImportEnabled");
			this.lblIsFacebookImportEnabled.Name = "lblIsFacebookImportEnabled";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.Name = "label8";
			// 
			// label9
			// 
			resources.ApplyResources(this.label9, "label9");
			this.label9.Name = "label9";
			this.label9.Click += new System.EventHandler(this.label9_Click);
			// 
			// logoutButton1
			// 
			resources.ApplyResources(this.logoutButton1, "logoutButton1");
			this.logoutButton1.Name = "logoutButton1";
			this.logoutButton1.UseVisualStyleBackColor = true;
			this.logoutButton1.Click += new System.EventHandler(this.logoutButton1_Click);
			// 
			// label10
			// 
			resources.ApplyResources(this.label10, "label10");
			this.label10.Name = "label10";
			this.label10.Click += new System.EventHandler(this.label10_Click);
			// 
			// AccountInfoForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lblIsFacebookImportEnabled);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblSince);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.logoutButton1);
			this.Name = "AccountInfoForm";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.AccountInfoForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private StationSystemTray.LogoutButton logoutButton1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblSince;
		private System.Windows.Forms.Label lblIsFacebookImportEnabled;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
    }
}