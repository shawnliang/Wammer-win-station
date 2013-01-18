namespace Waveface.Stream.WindowsClient
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContactUsDialog));
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
			resources.ApplyResources(this.emailLink, "emailLink");
			this.emailLink.Name = "emailLink";
			this.emailLink.TabStop = true;
			this.emailLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.emailLink_LinkClicked);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// twitterLink
			// 
			resources.ApplyResources(this.twitterLink, "twitterLink");
			this.twitterLink.Name = "twitterLink";
			this.twitterLink.TabStop = true;
			this.twitterLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.twitterLink_LinkClicked);
			// 
			// fbLink
			// 
			resources.ApplyResources(this.fbLink, "fbLink");
			this.fbLink.Name = "fbLink";
			this.fbLink.TabStop = true;
			this.fbLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.fbLink_LinkClicked);
			// 
			// btnOK
			// 
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// collectLogsLink
			// 
			resources.ApplyResources(this.collectLogsLink, "collectLogsLink");
			this.collectLogsLink.Name = "collectLogsLink";
			this.collectLogsLink.TabStop = true;
			this.collectLogsLink.UseCompatibleTextRendering = true;
			this.collectLogsLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.collectLogsLink_LinkClicked);
			// 
			// ContactUsDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.collectLogsLink);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.fbLink);
			this.Controls.Add(this.twitterLink);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.emailLink);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ContactUsDialog";
			this.ShowIcon = false;
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