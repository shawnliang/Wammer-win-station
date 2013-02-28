namespace Waveface.Stream.WindowsClient
{
	partial class OldUserWizardDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OldUserWizardDialog));
			this.panel1 = new System.Windows.Forms.Panel();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.tabControl1 = new Waveface.Stream.WindowsClient.TabControlEx();
			this.tabSignIn = new System.Windows.Forms.TabPage();
			this.loginControl1 = new Waveface.Stream.WindowsClient.LoginControl();
			this.tabPlan = new System.Windows.Forms.TabPage();
			this.choosePlanControl1 = new Waveface.Stream.WindowsClient.ChoosePlanControl();
			this.tabImport = new System.Windows.Forms.TabPage();
			this.fileImportControl1 = new Waveface.Stream.WindowsClient.FileImportControl();
			this.panel1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabSignIn.SuspendLayout();
			this.tabPlan.SuspendLayout();
			this.tabImport.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button1);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// button2
			// 
			resources.ApplyResources(this.button2, "button2");
			this.button2.Name = "button2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button1
			// 
			resources.ApplyResources(this.button1, "button1");
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabSignIn);
			this.tabControl1.Controls.Add(this.tabPlan);
			this.tabControl1.Controls.Add(this.tabImport);
			resources.ApplyResources(this.tabControl1, "tabControl1");
			this.tabControl1.HideTabs = true;
			this.tabControl1.Multiline = true;
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.PageIndex = 1;
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabSignIn
			// 
			this.tabSignIn.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabSignIn.Controls.Add(this.loginControl1);
			resources.ApplyResources(this.tabSignIn, "tabSignIn");
			this.tabSignIn.Name = "tabSignIn";
			// 
			// loginControl1
			// 
			this.loginControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
			resources.ApplyResources(this.loginControl1, "loginControl1");
			this.loginControl1.Name = "loginControl1";
			// 
			// tabPlan
			// 
			this.tabPlan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
			this.tabPlan.Controls.Add(this.choosePlanControl1);
			resources.ApplyResources(this.tabPlan, "tabPlan");
			this.tabPlan.Name = "tabPlan";
			// 
			// choosePlanControl1
			// 
			resources.ApplyResources(this.choosePlanControl1, "choosePlanControl1");
			this.choosePlanControl1.Name = "choosePlanControl1";
			// 
			// tabImport
			// 
			this.tabImport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
			this.tabImport.Controls.Add(this.fileImportControl1);
			resources.ApplyResources(this.tabImport, "tabImport");
			this.tabImport.Name = "tabImport";
			// 
			// fileImportControl1
			// 
			resources.ApplyResources(this.fileImportControl1, "fileImportControl1");
			this.fileImportControl1.Name = "fileImportControl1";
			// 
			// OldUserWizardDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OldUserWizardDialog";
			this.Load += new System.EventHandler(this.OldUserWizardDialog_Load);
			this.panel1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabSignIn.ResumeLayout(false);
			this.tabPlan.ResumeLayout(false);
			this.tabImport.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private TabControlEx tabControl1;
		private System.Windows.Forms.TabPage tabSignIn;
		private System.Windows.Forms.TabPage tabPlan;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private LoginControl loginControl1;
		private ChoosePlanControl choosePlanControl1;
		private System.Windows.Forms.TabPage tabImport;
		private FileImportControl fileImportControl1;
	}
}