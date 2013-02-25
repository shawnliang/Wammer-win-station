namespace Waveface.Stream.WindowsClient
{
	partial class NewUserWizardDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewUserWizardDialog));
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonNext = new System.Windows.Forms.Button();
			this.buttonPrev = new System.Windows.Forms.Button();
			this.tabControl1 = new Waveface.Stream.WindowsClient.TabControlEx();
			this.tabIntro1 = new System.Windows.Forms.TabPage();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.tabIntro2 = new System.Windows.Forms.TabPage();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.tabIntro3 = new System.Windows.Forms.TabPage();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.tabChoosePlan = new System.Windows.Forms.TabPage();
			this.choosePlanControl1 = new Waveface.Stream.WindowsClient.ChoosePlanControl();
			this.tabSignup = new System.Windows.Forms.TabPage();
			this.nativeSignupControl1 = new Waveface.Stream.WindowsClient.SignupControl();
			this.tabConnectCloudServices = new System.Windows.Forms.TabPage();
			this.serviceImportControl1 = new Waveface.Stream.WindowsClient.ServiceImportControl();
			this.tabImportPhoto = new System.Windows.Forms.TabPage();
			this.fileImportControl1 = new Waveface.Stream.WindowsClient.FileImportControl();
			this.panel1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabIntro1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.tabIntro2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.tabIntro3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			this.tabChoosePlan.SuspendLayout();
			this.tabSignup.SuspendLayout();
			this.tabConnectCloudServices.SuspendLayout();
			this.tabImportPhoto.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.panel1.Controls.Add(this.buttonNext);
			this.panel1.Controls.Add(this.buttonPrev);
			this.panel1.Name = "panel1";
			// 
			// buttonNext
			// 
			resources.ApplyResources(this.buttonNext, "buttonNext");
			this.buttonNext.Name = "buttonNext";
			this.buttonNext.UseVisualStyleBackColor = true;
			this.buttonNext.Click += new System.EventHandler(this.button2_Click);
			// 
			// buttonPrev
			// 
			resources.ApplyResources(this.buttonPrev, "buttonPrev");
			this.buttonPrev.Name = "buttonPrev";
			this.buttonPrev.UseVisualStyleBackColor = true;
			this.buttonPrev.Click += new System.EventHandler(this.button1_Click);
			// 
			// tabControl1
			// 
			resources.ApplyResources(this.tabControl1, "tabControl1");
			this.tabControl1.Controls.Add(this.tabIntro1);
			this.tabControl1.Controls.Add(this.tabIntro2);
			this.tabControl1.Controls.Add(this.tabIntro3);
			this.tabControl1.Controls.Add(this.tabChoosePlan);
			this.tabControl1.Controls.Add(this.tabSignup);
			this.tabControl1.Controls.Add(this.tabConnectCloudServices);
			this.tabControl1.Controls.Add(this.tabImportPhoto);
			this.tabControl1.HideTabs = true;
			this.tabControl1.Multiline = true;
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.PageIndex = 1;
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabIntro1
			// 
			resources.ApplyResources(this.tabIntro1, "tabIntro1");
			this.tabIntro1.Controls.Add(this.pictureBox1);
			this.tabIntro1.Name = "tabIntro1";
			this.tabIntro1.UseVisualStyleBackColor = true;
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = global::Waveface.Stream.WindowsClient.Properties.Resources.P1;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// tabIntro2
			// 
			resources.ApplyResources(this.tabIntro2, "tabIntro2");
			this.tabIntro2.Controls.Add(this.pictureBox2);
			this.tabIntro2.Name = "tabIntro2";
			this.tabIntro2.UseVisualStyleBackColor = true;
			// 
			// pictureBox2
			// 
			resources.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Image = global::Waveface.Stream.WindowsClient.Properties.Resources.P2;
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			// 
			// tabIntro3
			// 
			resources.ApplyResources(this.tabIntro3, "tabIntro3");
			this.tabIntro3.Controls.Add(this.pictureBox3);
			this.tabIntro3.Name = "tabIntro3";
			this.tabIntro3.UseVisualStyleBackColor = true;
			// 
			// pictureBox3
			// 
			resources.ApplyResources(this.pictureBox3, "pictureBox3");
			this.pictureBox3.Image = global::Waveface.Stream.WindowsClient.Properties.Resources.P3;
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.TabStop = false;
			// 
			// tabChoosePlan
			// 
			resources.ApplyResources(this.tabChoosePlan, "tabChoosePlan");
			this.tabChoosePlan.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabChoosePlan.Controls.Add(this.choosePlanControl1);
			this.tabChoosePlan.Name = "tabChoosePlan";
			// 
			// choosePlanControl1
			// 
			resources.ApplyResources(this.choosePlanControl1, "choosePlanControl1");
			this.choosePlanControl1.Name = "choosePlanControl1";
			// 
			// tabSignup
			// 
			resources.ApplyResources(this.tabSignup, "tabSignup");
			this.tabSignup.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabSignup.Controls.Add(this.nativeSignupControl1);
			this.tabSignup.Name = "tabSignup";
			// 
			// nativeSignupControl1
			// 
			resources.ApplyResources(this.nativeSignupControl1, "nativeSignupControl1");
			this.nativeSignupControl1.Name = "nativeSignupControl1";
			this.nativeSignupControl1.SignupAction = null;
			// 
			// tabConnectCloudServices
			// 
			resources.ApplyResources(this.tabConnectCloudServices, "tabConnectCloudServices");
			this.tabConnectCloudServices.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabConnectCloudServices.Controls.Add(this.serviceImportControl1);
			this.tabConnectCloudServices.Name = "tabConnectCloudServices";
			// 
			// serviceImportControl1
			// 
			resources.ApplyResources(this.serviceImportControl1, "serviceImportControl1");
			this.serviceImportControl1.Name = "serviceImportControl1";
			// 
			// tabImportPhoto
			// 
			resources.ApplyResources(this.tabImportPhoto, "tabImportPhoto");
			this.tabImportPhoto.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabImportPhoto.Controls.Add(this.fileImportControl1);
			this.tabImportPhoto.Name = "tabImportPhoto";
			// 
			// fileImportControl1
			// 
			resources.ApplyResources(this.fileImportControl1, "fileImportControl1");
			this.fileImportControl1.Name = "fileImportControl1";
			// 
			// NewUserWizardDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewUserWizardDialog";
			this.Load += new System.EventHandler(this.NewUserWizardDialog_Load);
			this.panel1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabIntro1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.tabIntro2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.tabIntro3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			this.tabChoosePlan.ResumeLayout(false);
			this.tabSignup.ResumeLayout(false);
			this.tabConnectCloudServices.ResumeLayout(false);
			this.tabImportPhoto.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private TabControlEx tabControl1;
		private System.Windows.Forms.TabPage tabConnectCloudServices;
		private System.Windows.Forms.TabPage tabChoosePlan;
		private System.Windows.Forms.Button buttonNext;
		private System.Windows.Forms.Button buttonPrev;
		private ChoosePlanControl choosePlanControl1;
		private System.Windows.Forms.TabPage tabImportPhoto;
		private FileImportControl fileImportControl1;
		private ServiceImportControl serviceImportControl1;
		private System.Windows.Forms.TabPage tabSignup;
		private System.Windows.Forms.TabPage tabIntro1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TabPage tabIntro2;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.TabPage tabIntro3;
		private System.Windows.Forms.PictureBox pictureBox3;
		private SignupControl nativeSignupControl1;
	}
}