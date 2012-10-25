namespace StationSystemTray.Src.Control
{
	partial class BuildPersonalCloudUserControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuildPersonalCloudUserControl));
			this.googlePlayBtn = new System.Windows.Forms.Button();
			this.appStoreBtn = new System.Windows.Forms.Button();
			this.chromeStoreBtn = new System.Windows.Forms.Button();
			this.firefoxBtn = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tickedLabel = new System.Windows.Forms.Label();
			this.connectedLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// googlePlayBtn
			// 
			this.googlePlayBtn.Image = ((System.Drawing.Image)(resources.GetObject("googlePlayBtn.Image")));
			this.googlePlayBtn.Location = new System.Drawing.Point(35, 99);
			this.googlePlayBtn.Name = "googlePlayBtn";
			this.googlePlayBtn.Size = new System.Drawing.Size(180, 180);
			this.googlePlayBtn.TabIndex = 0;
			this.googlePlayBtn.UseVisualStyleBackColor = true;
			this.googlePlayBtn.Click += new System.EventHandler(this.googlePlayBtn_Click);
			// 
			// appStoreBtn
			// 
			this.appStoreBtn.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.appStoreBtn.Image = ((System.Drawing.Image)(resources.GetObject("appStoreBtn.Image")));
			this.appStoreBtn.Location = new System.Drawing.Point(221, 99);
			this.appStoreBtn.Name = "appStoreBtn";
			this.appStoreBtn.Size = new System.Drawing.Size(180, 180);
			this.appStoreBtn.TabIndex = 1;
			this.appStoreBtn.UseVisualStyleBackColor = true;
			this.appStoreBtn.Click += new System.EventHandler(this.appStoreBtn_Click);
			// 
			// chromeStoreBtn
			// 
			this.chromeStoreBtn.Image = ((System.Drawing.Image)(resources.GetObject("chromeStoreBtn.Image")));
			this.chromeStoreBtn.Location = new System.Drawing.Point(440, 99);
			this.chromeStoreBtn.Name = "chromeStoreBtn";
			this.chromeStoreBtn.Size = new System.Drawing.Size(126, 84);
			this.chromeStoreBtn.TabIndex = 2;
			this.chromeStoreBtn.UseVisualStyleBackColor = true;
			this.chromeStoreBtn.Click += new System.EventHandler(this.chromeStoreBtn_Click);
			// 
			// firefoxBtn
			// 
			this.firefoxBtn.Image = ((System.Drawing.Image)(resources.GetObject("firefoxBtn.Image")));
			this.firefoxBtn.Location = new System.Drawing.Point(440, 195);
			this.firefoxBtn.Name = "firefoxBtn";
			this.firefoxBtn.Size = new System.Drawing.Size(126, 84);
			this.firefoxBtn.TabIndex = 3;
			this.firefoxBtn.UseVisualStyleBackColor = true;
			this.firefoxBtn.Click += new System.EventHandler(this.firefoxBtn_Click);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(41, 54);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(360, 42);
			this.label1.TabIndex = 4;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(437, 54);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(129, 42);
			this.label2.TabIndex = 5;
			this.label2.Text = resources.GetString("label2.Text");
			// 
			// tickedLabel
			// 
			this.tickedLabel.Font = new System.Drawing.Font("Arial", 9F);
			this.tickedLabel.Image = ((System.Drawing.Image)(resources.GetObject("tickedLabel.Image")));
			this.tickedLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tickedLabel.Location = new System.Drawing.Point(32, 319);
			this.tickedLabel.Name = "tickedLabel";
			this.tickedLabel.Size = new System.Drawing.Size(27, 23);
			this.tickedLabel.TabIndex = 6;
			this.tickedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tickedLabel.Visible = false;
			// 
			// connectedLabel
			// 
			this.connectedLabel.AutoSize = true;
			this.connectedLabel.Font = new System.Drawing.Font("Arial", 9F);
			this.connectedLabel.Location = new System.Drawing.Point(56, 322);
			this.connectedLabel.Name = "connectedLabel";
			this.connectedLabel.Size = new System.Drawing.Size(100, 15);
			this.connectedLabel.TabIndex = 7;
			this.connectedLabel.Text = "xxxx is connected";
			this.connectedLabel.Visible = false;
			// 
			// BuildPersonalCloudUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.connectedLabel);
			this.Controls.Add(this.tickedLabel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.firefoxBtn);
			this.Controls.Add(this.chromeStoreBtn);
			this.Controls.Add(this.appStoreBtn);
			this.Controls.Add(this.googlePlayBtn);
			this.Name = "BuildPersonalCloudUserControl";
			this.Size = new System.Drawing.Size(615, 362);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button googlePlayBtn;
		private System.Windows.Forms.Button appStoreBtn;
		private System.Windows.Forms.Button chromeStoreBtn;
		private System.Windows.Forms.Button firefoxBtn;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label tickedLabel;
		private System.Windows.Forms.Label connectedLabel;
	}
}
