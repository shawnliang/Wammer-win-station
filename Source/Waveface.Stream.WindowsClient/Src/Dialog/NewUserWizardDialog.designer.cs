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
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.tabIntro2 = new System.Windows.Forms.TabPage();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
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
			this.tabChoosePlan.SuspendLayout();
			this.tabSignup.SuspendLayout();
			this.tabConnectCloudServices.SuspendLayout();
			this.tabImportPhoto.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
			this.panel1.Controls.Add(this.buttonNext);
			this.panel1.Controls.Add(this.buttonPrev);
			resources.ApplyResources(this.panel1, "panel1");
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
			this.tabControl1.Controls.Add(this.tabIntro1);
			this.tabControl1.Controls.Add(this.tabIntro2);
			this.tabControl1.Controls.Add(this.tabChoosePlan);
			this.tabControl1.Controls.Add(this.tabSignup);
			this.tabControl1.Controls.Add(this.tabConnectCloudServices);
			this.tabControl1.Controls.Add(this.tabImportPhoto);
			resources.ApplyResources(this.tabControl1, "tabControl1");
			this.tabControl1.HideTabs = true;
			this.tabControl1.Multiline = true;
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.PageIndex = 2;
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabIntro1
			// 
			this.tabIntro1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
			this.tabIntro1.Controls.Add(this.label3);
			this.tabIntro1.Controls.Add(this.label2);
			this.tabIntro1.Controls.Add(this.label1);
			this.tabIntro1.Controls.Add(this.pictureBox1);
			resources.ApplyResources(this.tabIntro1, "tabIntro1");
			this.tabIntro1.Name = "tabIntro1";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(168)))), ((int)(((byte)(169)))));
			this.label3.Name = "label3";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(111)))), ((int)(((byte)(110)))));
			this.label2.Name = "label2";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(111)))), ((int)(((byte)(90)))));
			this.label1.Name = "label1";
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = global::Waveface.Stream.WindowsClient.Properties.Resources.PCIntro1;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// tabIntro2
			// 
			this.tabIntro2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
			this.tabIntro2.Controls.Add(this.label4);
			this.tabIntro2.Controls.Add(this.label5);
			this.tabIntro2.Controls.Add(this.label6);
			this.tabIntro2.Controls.Add(this.pictureBox2);
			resources.ApplyResources(this.tabIntro2, "tabIntro2");
			this.tabIntro2.Name = "tabIntro2";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(168)))), ((int)(((byte)(169)))));
			this.label4.Name = "label4";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(111)))), ((int)(((byte)(110)))));
			this.label5.Name = "label5";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(111)))), ((int)(((byte)(90)))));
			this.label6.Name = "label6";
			// 
			// pictureBox2
			// 
			resources.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Image = global::Waveface.Stream.WindowsClient.Properties.Resources.PCIntro2;
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			// 
			// tabChoosePlan
			// 
			this.tabChoosePlan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
			this.tabChoosePlan.Controls.Add(this.choosePlanControl1);
			resources.ApplyResources(this.tabChoosePlan, "tabChoosePlan");
			this.tabChoosePlan.Name = "tabChoosePlan";
			// 
			// choosePlanControl1
			// 
			resources.ApplyResources(this.choosePlanControl1, "choosePlanControl1");
			this.choosePlanControl1.Name = "choosePlanControl1";
			// 
			// tabSignup
			// 
			this.tabSignup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
			this.tabSignup.Controls.Add(this.nativeSignupControl1);
			resources.ApplyResources(this.tabSignup, "tabSignup");
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
			this.tabConnectCloudServices.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
			this.tabConnectCloudServices.Controls.Add(this.serviceImportControl1);
			resources.ApplyResources(this.tabConnectCloudServices, "tabConnectCloudServices");
			this.tabConnectCloudServices.Name = "tabConnectCloudServices";
			// 
			// serviceImportControl1
			// 
			resources.ApplyResources(this.serviceImportControl1, "serviceImportControl1");
			this.serviceImportControl1.Name = "serviceImportControl1";
			// 
			// tabImportPhoto
			// 
			this.tabImportPhoto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
			this.tabImportPhoto.Controls.Add(this.fileImportControl1);
			resources.ApplyResources(this.tabImportPhoto, "tabImportPhoto");
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
		private SignupControl nativeSignupControl1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
	}
}