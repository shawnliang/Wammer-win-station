namespace StationSystemTray.Control
{
	partial class FBLoginButton
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
			this.lblLoginMsg = new System.Windows.Forms.Label();
			this.picFBIcon = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.picFBIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// lblLoginMsg
			// 
			this.lblLoginMsg.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblLoginMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblLoginMsg.ForeColor = System.Drawing.Color.White;
			this.lblLoginMsg.Location = new System.Drawing.Point(37, 0);
			this.lblLoginMsg.Name = "lblLoginMsg";
			this.lblLoginMsg.Size = new System.Drawing.Size(146, 38);
			this.lblLoginMsg.TabIndex = 1;
			this.lblLoginMsg.Text = "Login with Facebook";
			this.lblLoginMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblLoginMsg.Click += new System.EventHandler(this.lblLoginMsg_Click);
			// 
			// picFBIcon
			// 
			this.picFBIcon.BackgroundImage = global::StationSystemTray.Properties.Resources.FB_Logo;
			this.picFBIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.picFBIcon.Dock = System.Windows.Forms.DockStyle.Left;
			this.picFBIcon.Location = new System.Drawing.Point(0, 0);
			this.picFBIcon.Margin = new System.Windows.Forms.Padding(0);
			this.picFBIcon.Name = "picFBIcon";
			this.picFBIcon.Size = new System.Drawing.Size(37, 38);
			this.picFBIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.picFBIcon.TabIndex = 0;
			this.picFBIcon.TabStop = false;
			this.picFBIcon.Click += new System.EventHandler(this.picFBIcon_Click);
			// 
			// FBLoginButton
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(120)))), ((int)(((byte)(171)))));
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.lblLoginMsg);
			this.Controls.Add(this.picFBIcon);
			this.Name = "FBLoginButton";
			this.Size = new System.Drawing.Size(183, 38);
			((System.ComponentModel.ISupportInitialize)(this.picFBIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox picFBIcon;
		private System.Windows.Forms.Label lblLoginMsg;
	}
}
