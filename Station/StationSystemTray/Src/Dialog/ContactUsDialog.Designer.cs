namespace StationSystemTray
{
	partial class ContactUsDialog
	{
		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.emailLink = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.twitterLink = new System.Windows.Forms.LinkLabel();
			this.fbLink = new System.Windows.Forms.LinkLabel();
			this.btnOK = new System.Windows.Forms.Button();
			this.collectLogsLink = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// emailLink
			// 
			this.emailLink.AutoSize = true;
			this.emailLink.Location = new System.Drawing.Point(74, 25);
			this.emailLink.Name = "emailLink";
			this.emailLink.Size = new System.Drawing.Size(134, 15);
			this.emailLink.TabIndex = 0;
			this.emailLink.TabStop = true;
			this.emailLink.Text = "contact@waveface.com";
			this.emailLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.emailLink_LinkClicked);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(29, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(39, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "Email:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(29, 52);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(47, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "Twitter:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(29, 79);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(61, 15);
			this.label3.TabIndex = 3;
			this.label3.Text = "Facebook:";
			// 
			// twitterLink
			// 
			this.twitterLink.AutoSize = true;
			this.twitterLink.Location = new System.Drawing.Point(82, 52);
			this.twitterLink.Name = "twitterLink";
			this.twitterLink.Size = new System.Drawing.Size(67, 15);
			this.twitterLink.TabIndex = 4;
			this.twitterLink.TabStop = true;
			this.twitterLink.Text = "@waveface";
			this.twitterLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.twitterLink_LinkClicked);
			// 
			// fbLink
			// 
			this.fbLink.AutoSize = true;
			this.fbLink.Location = new System.Drawing.Point(96, 79);
			this.fbLink.Name = "fbLink";
			this.fbLink.Size = new System.Drawing.Size(95, 15);
			this.fbLink.TabIndex = 5;
			this.fbLink.TabStop = true;
			this.fbLink.Text = "Waveface\'s Page";
			this.fbLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.fbLink_LinkClicked);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(373, 129);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(79, 30);
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// collectLogsLink
			// 
			this.collectLogsLink.LinkArea = new System.Windows.Forms.LinkArea(82, 11);
			this.collectLogsLink.Location = new System.Drawing.Point(29, 122);
			this.collectLogsLink.Name = "collectLogsLink";
			this.collectLogsLink.Size = new System.Drawing.Size(338, 51);
			this.collectLogsLink.TabIndex = 8;
			this.collectLogsLink.TabStop = true;
			this.collectLogsLink.Text = "If you need technical assistance, please email us your issues and all files under" +
    " this folder.\r\n";
			this.collectLogsLink.UseCompatibleTextRendering = true;
			this.collectLogsLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.collectLogsLink_LinkClicked);
			// 
			// ContactUsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
			this.ClientSize = new System.Drawing.Size(464, 182);
			this.Controls.Add(this.collectLogsLink);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.fbLink);
			this.Controls.Add(this.twitterLink);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.emailLink);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ContactUsDialog";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Contact Us";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.LinkLabel emailLink;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.LinkLabel twitterLink;
		private System.Windows.Forms.LinkLabel fbLink;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.LinkLabel collectLogsLink;
	}
}