﻿
namespace Waveface.Stream.WindowsClient
{
	partial class StepByStepWizardDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StepByStepWizardDialog));
			this.nextButton = new System.Windows.Forms.Button();
			this.prevButton = new System.Windows.Forms.Button();
			this.wizardControl = new WizardControl();
			this.SuspendLayout();
			// 
			// nextButton
			// 
			this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.nextButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nextButton.Location = new System.Drawing.Point(432, 409);
			this.nextButton.Name = "nextButton";
			this.nextButton.Size = new System.Drawing.Size(110, 32);
			this.nextButton.TabIndex = 1;
			this.nextButton.Text = "Next";
			this.nextButton.UseVisualStyleBackColor = true;
			this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
			// 
			// prevButton
			// 
			this.prevButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.prevButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.prevButton.Location = new System.Drawing.Point(10, 409);
			this.prevButton.Name = "prevButton";
			this.prevButton.Size = new System.Drawing.Size(110, 32);
			this.prevButton.TabIndex = 2;
			this.prevButton.Text = "Previous";
			this.prevButton.UseVisualStyleBackColor = true;
			this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
			// 
			// wizardControl
			// 
			this.wizardControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.wizardControl.Location = new System.Drawing.Point(0, 0);
			this.wizardControl.Name = "wizardControl";
			this.wizardControl.PageIndex = 0;
			this.wizardControl.Size = new System.Drawing.Size(555, 400);
			this.wizardControl.TabIndex = 3;
			// 
			// StepByStepWizardDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.ClientSize = new System.Drawing.Size(554, 453);
			this.Controls.Add(this.wizardControl);
			this.Controls.Add(this.prevButton);
			this.Controls.Add(this.nextButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StepByStepWizardDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Stream";
			this.Load += new System.EventHandler(this.FirstUseWizardDialog_Load);
			this.ResumeLayout(false);

		}

		#endregion

		protected WizardControl wizardControl;
		protected System.Windows.Forms.Button nextButton;
		protected System.Windows.Forms.Button prevButton;
	}
}