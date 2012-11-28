namespace Waveface.Stream.WindowsClient
{
	partial class ImportFolderAndMediaControl
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
			this.label1 = new System.Windows.Forms.Label();
			this.importFromMediaButton = new System.Windows.Forms.Button();
			this.importFromDiskButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.txtStoreLocation = new System.Windows.Forms.TextBox();
			this.changeButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(36, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(215, 19);
			this.label1.TabIndex = 2;
			this.label1.Text = "Let\'s fill some fun to AOStream!";
			// 
			// importFromMediaButton
			// 
			this.importFromMediaButton.BackColor = System.Drawing.Color.PowderBlue;
			this.importFromMediaButton.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.importFromMediaButton.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.importFromMediaButton.Location = new System.Drawing.Point(55, 185);
			this.importFromMediaButton.Name = "importFromMediaButton";
			this.importFromMediaButton.Padding = new System.Windows.Forms.Padding(0, 15, 0, 10);
			this.importFromMediaButton.Size = new System.Drawing.Size(181, 57);
			this.importFromMediaButton.TabIndex = 5;
			this.importFromMediaButton.Text = "Import Removable Media";
			this.importFromMediaButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.importFromMediaButton.UseVisualStyleBackColor = true;
			this.importFromMediaButton.Click += new System.EventHandler(this.importFromMediaButton_Click);
			// 
			// importFromDiskButton
			// 
			this.importFromDiskButton.BackColor = System.Drawing.Color.PowderBlue;
			this.importFromDiskButton.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.importFromDiskButton.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.importFromDiskButton.Location = new System.Drawing.Point(55, 98);
			this.importFromDiskButton.Name = "importFromDiskButton";
			this.importFromDiskButton.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
			this.importFromDiskButton.Size = new System.Drawing.Size(181, 57);
			this.importFromDiskButton.TabIndex = 4;
			this.importFromDiskButton.Text = "Import Photo Folders";
			this.importFromDiskButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.importFromDiskButton.UseVisualStyleBackColor = true;
			this.importFromDiskButton.Click += new System.EventHandler(this.importFromDiskButton_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(36, 323);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(261, 15);
			this.label2.TabIndex = 6;
			this.label2.Text = "All imported and synced files will be stored at:";
			// 
			// txtStoreLocation
			// 
			this.txtStoreLocation.Font = new System.Drawing.Font("Arial", 9F);
			this.txtStoreLocation.Location = new System.Drawing.Point(39, 345);
			this.txtStoreLocation.Name = "txtStoreLocation";
			this.txtStoreLocation.Size = new System.Drawing.Size(308, 21);
			this.txtStoreLocation.TabIndex = 7;
			// 
			// changeButton
			// 
			this.changeButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.changeButton.Location = new System.Drawing.Point(353, 342);
			this.changeButton.Name = "changeButton";
			this.changeButton.Size = new System.Drawing.Size(137, 26);
			this.changeButton.TabIndex = 8;
			this.changeButton.Text = "Change";
			this.changeButton.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(260, 112);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(230, 39);
			this.label3.TabIndex = 9;
			this.label3.Text = "Choose any folders from this PC and import photo folders into AOStream";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(260, 196);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(230, 39);
			this.label4.TabIndex = 10;
			this.label4.Text = "Insert your USB, SD card, or CD and let old memories live again.";
			// 
			// ImportFolderAndMediaControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.changeButton);
			this.Controls.Add(this.txtStoreLocation);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.importFromMediaButton);
			this.Controls.Add(this.importFromDiskButton);
			this.Controls.Add(this.label1);
			this.Name = "ImportFolderAndMediaControl";
			this.Size = new System.Drawing.Size(555, 400);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button importFromMediaButton;
		private System.Windows.Forms.Button importFromDiskButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtStoreLocation;
		private System.Windows.Forms.Button changeButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
	}
}
