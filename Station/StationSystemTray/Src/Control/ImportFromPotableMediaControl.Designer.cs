namespace StationSystemTray
{
	partial class ImportFromPotableMediaControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportFromPotableMediaControl));
			this.label1 = new System.Windows.Forms.Label();
			this.deviceCombobox = new System.Windows.Forms.ComboBox();
			this.importButton = new System.Windows.Forms.Button();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.progressText = new System.Windows.Forms.Label();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Font = new System.Drawing.Font("Arial", 10F);
			this.label1.Location = new System.Drawing.Point(45, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(426, 79);
			this.label1.TabIndex = 0;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// deviceCombobox
			// 
			this.deviceCombobox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.deviceCombobox.Font = new System.Drawing.Font("Arial", 9F);
			this.deviceCombobox.FormattingEnabled = true;
			this.deviceCombobox.Location = new System.Drawing.Point(48, 126);
			this.deviceCombobox.Name = "deviceCombobox";
			this.deviceCombobox.Size = new System.Drawing.Size(290, 23);
			this.deviceCombobox.TabIndex = 1;
			this.deviceCombobox.DropDown += new System.EventHandler(this.deviceCombobox_DropDown);
			// 
			// importButton
			// 
			this.importButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.importButton.Font = new System.Drawing.Font("Arial", 9F);
			this.importButton.Location = new System.Drawing.Point(344, 126);
			this.importButton.Name = "importButton";
			this.importButton.Size = new System.Drawing.Size(112, 23);
			this.importButton.TabIndex = 2;
			this.importButton.Text = "Import Now";
			this.importButton.UseVisualStyleBackColor = true;
			this.importButton.Click += new System.EventHandler(this.importButton_Click);
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(48, 223);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(408, 23);
			this.progressBar.Step = 1;
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar.TabIndex = 3;
			this.progressBar.Value = 30;
			this.progressBar.Visible = false;
			// 
			// progressText
			// 
			this.progressText.AutoSize = true;
			this.progressText.Location = new System.Drawing.Point(48, 204);
			this.progressText.Name = "progressText";
			this.progressText.Size = new System.Drawing.Size(68, 13);
			this.progressText.TabIndex = 4;
			this.progressText.Text = "Processing...";
			this.progressText.Visible = false;
			// 
			// backgroundWorker
			// 
			this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
			this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
			// 
			// ImportFromPotableMediaControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.progressText);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.importButton);
			this.Controls.Add(this.deviceCombobox);
			this.Controls.Add(this.label1);
			this.Name = "ImportFromPotableMediaControl";
			this.Size = new System.Drawing.Size(507, 289);
			this.Load += new System.EventHandler(this.ImportFromPotableMediaControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox deviceCombobox;
		private System.Windows.Forms.Button importButton;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label progressText;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
	}
}
