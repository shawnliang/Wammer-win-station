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
			this.tabConnectCloudServices = new System.Windows.Forms.TabPage();
			this.serviceImportControl1 = new Waveface.Stream.WindowsClient.ServiceImportControl();
			this.tabImportPhoto = new System.Windows.Forms.TabPage();
			this.fileImportControl1 = new Waveface.Stream.WindowsClient.FileImportControl();
			this.nativeSignupControl1 = new Waveface.Stream.WindowsClient.NativeSignupControl();
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
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.panel1.Controls.Add(this.buttonNext);
			this.panel1.Controls.Add(this.buttonPrev);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 433);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(706, 48);
			this.panel1.TabIndex = 0;
			// 
			// buttonNext
			// 
			this.buttonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonNext.Location = new System.Drawing.Point(619, 10);
			this.buttonNext.Name = "buttonNext";
			this.buttonNext.Size = new System.Drawing.Size(75, 25);
			this.buttonNext.TabIndex = 1;
			this.buttonNext.Text = "Next";
			this.buttonNext.UseVisualStyleBackColor = true;
			this.buttonNext.Click += new System.EventHandler(this.button2_Click);
			// 
			// buttonPrev
			// 
			this.buttonPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonPrev.Location = new System.Drawing.Point(12, 10);
			this.buttonPrev.Name = "buttonPrev";
			this.buttonPrev.Size = new System.Drawing.Size(75, 25);
			this.buttonPrev.TabIndex = 0;
			this.buttonPrev.Text = "Previous";
			this.buttonPrev.UseVisualStyleBackColor = true;
			this.buttonPrev.Click += new System.EventHandler(this.button1_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabIntro1);
			this.tabControl1.Controls.Add(this.tabIntro2);
			this.tabControl1.Controls.Add(this.tabIntro3);
			this.tabControl1.Controls.Add(this.tabChoosePlan);
			this.tabControl1.Controls.Add(this.tabSignup);
			this.tabControl1.Controls.Add(this.tabConnectCloudServices);
			this.tabControl1.Controls.Add(this.tabImportPhoto);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.HideTabs = true;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Multiline = true;
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.PageIndex = 5;
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(706, 433);
			this.tabControl1.TabIndex = 1;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabIntro1
			// 
			this.tabIntro1.Controls.Add(this.pictureBox1);
			this.tabIntro1.Location = new System.Drawing.Point(0, 0);
			this.tabIntro1.Name = "tabIntro1";
			this.tabIntro1.Padding = new System.Windows.Forms.Padding(3);
			this.tabIntro1.Size = new System.Drawing.Size(706, 433);
			this.tabIntro1.TabIndex = 6;
			this.tabIntro1.Text = "What is AOStream?";
			this.tabIntro1.UseVisualStyleBackColor = true;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Image = global::Waveface.Stream.WindowsClient.Properties.Resources.P1;
			this.pictureBox1.Location = new System.Drawing.Point(3, 3);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(700, 427);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// tabIntro2
			// 
			this.tabIntro2.Controls.Add(this.pictureBox2);
			this.tabIntro2.Location = new System.Drawing.Point(0, 0);
			this.tabIntro2.Name = "tabIntro2";
			this.tabIntro2.Padding = new System.Windows.Forms.Padding(3);
			this.tabIntro2.Size = new System.Drawing.Size(706, 433);
			this.tabIntro2.TabIndex = 7;
			this.tabIntro2.Text = "What is AOStream?";
			this.tabIntro2.UseVisualStyleBackColor = true;
			// 
			// pictureBox2
			// 
			this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox2.Image = global::Waveface.Stream.WindowsClient.Properties.Resources.P2;
			this.pictureBox2.Location = new System.Drawing.Point(3, 3);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(700, 427);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox2.TabIndex = 0;
			this.pictureBox2.TabStop = false;
			// 
			// tabIntro3
			// 
			this.tabIntro3.Controls.Add(this.pictureBox3);
			this.tabIntro3.Location = new System.Drawing.Point(0, 0);
			this.tabIntro3.Name = "tabIntro3";
			this.tabIntro3.Padding = new System.Windows.Forms.Padding(3);
			this.tabIntro3.Size = new System.Drawing.Size(706, 433);
			this.tabIntro3.TabIndex = 8;
			this.tabIntro3.Text = "What is AOStream?";
			this.tabIntro3.UseVisualStyleBackColor = true;
			// 
			// pictureBox3
			// 
			this.pictureBox3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox3.Image = global::Waveface.Stream.WindowsClient.Properties.Resources.P3;
			this.pictureBox3.Location = new System.Drawing.Point(3, 3);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(700, 427);
			this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox3.TabIndex = 0;
			this.pictureBox3.TabStop = false;
			// 
			// tabChoosePlan
			// 
			this.tabChoosePlan.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabChoosePlan.Controls.Add(this.choosePlanControl1);
			this.tabChoosePlan.Location = new System.Drawing.Point(0, 0);
			this.tabChoosePlan.Name = "tabChoosePlan";
			this.tabChoosePlan.Padding = new System.Windows.Forms.Padding(3);
			this.tabChoosePlan.Size = new System.Drawing.Size(706, 433);
			this.tabChoosePlan.TabIndex = 1;
			this.tabChoosePlan.Text = "Upgrade";
			// 
			// choosePlanControl1
			// 
			this.choosePlanControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.choosePlanControl1.Location = new System.Drawing.Point(3, 3);
			this.choosePlanControl1.Name = "choosePlanControl1";
			this.choosePlanControl1.Size = new System.Drawing.Size(700, 427);
			this.choosePlanControl1.TabIndex = 0;
			// 
			// tabSignup
			// 
			this.tabSignup.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabSignup.Controls.Add(this.nativeSignupControl1);
			this.tabSignup.Location = new System.Drawing.Point(0, 0);
			this.tabSignup.Name = "tabSignup";
			this.tabSignup.Padding = new System.Windows.Forms.Padding(3);
			this.tabSignup.Size = new System.Drawing.Size(706, 433);
			this.tabSignup.TabIndex = 4;
			this.tabSignup.Text = "Sign Up";
			// 
			// tabConnectCloudServices
			// 
			this.tabConnectCloudServices.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabConnectCloudServices.Controls.Add(this.serviceImportControl1);
			this.tabConnectCloudServices.Location = new System.Drawing.Point(0, 0);
			this.tabConnectCloudServices.Name = "tabConnectCloudServices";
			this.tabConnectCloudServices.Padding = new System.Windows.Forms.Padding(3);
			this.tabConnectCloudServices.Size = new System.Drawing.Size(706, 433);
			this.tabConnectCloudServices.TabIndex = 0;
			this.tabConnectCloudServices.Text = "Service Connect";
			// 
			// serviceImportControl1
			// 
			this.serviceImportControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.serviceImportControl1.Location = new System.Drawing.Point(3, 3);
			this.serviceImportControl1.Name = "serviceImportControl1";
			this.serviceImportControl1.Size = new System.Drawing.Size(700, 427);
			this.serviceImportControl1.TabIndex = 0;
			// 
			// tabImportPhoto
			// 
			this.tabImportPhoto.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabImportPhoto.Controls.Add(this.fileImportControl1);
			this.tabImportPhoto.Location = new System.Drawing.Point(0, 0);
			this.tabImportPhoto.Name = "tabImportPhoto";
			this.tabImportPhoto.Padding = new System.Windows.Forms.Padding(3);
			this.tabImportPhoto.Size = new System.Drawing.Size(706, 433);
			this.tabImportPhoto.TabIndex = 2;
			this.tabImportPhoto.Text = "Import photos";
			// 
			// fileImportControl1
			// 
			this.fileImportControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fileImportControl1.Location = new System.Drawing.Point(3, 3);
			this.fileImportControl1.Name = "fileImportControl1";
			this.fileImportControl1.Size = new System.Drawing.Size(700, 427);
			this.fileImportControl1.TabIndex = 0;
			// 
			// nativeSignupControl1
			// 
			this.nativeSignupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nativeSignupControl1.Location = new System.Drawing.Point(3, 3);
			this.nativeSignupControl1.Name = "nativeSignupControl1";
			this.nativeSignupControl1.SignupAction = null;
			this.nativeSignupControl1.Size = new System.Drawing.Size(700, 427);
			this.nativeSignupControl1.TabIndex = 1;
			// 
			// NewUserWizardDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(706, 481);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewUserWizardDialog";
			this.Text = "OldUserWizardDialog2";
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
		private NativeSignupControl nativeSignupControl1;
	}
}