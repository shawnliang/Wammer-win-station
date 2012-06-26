namespace Waveface
{
    partial class AccountInfoForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountInfoForm));
			this.webBrowser = new System.Windows.Forms.WebBrowser();
			this.logoutButton1 = new StationSystemTray.LogoutButton();
			this.SuspendLayout();
			// 
			// webBrowser
			// 
			resources.ApplyResources(this.webBrowser, "webBrowser");
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.ScriptErrorsSuppressed = true;
			this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
			// 
			// logoutButton1
			// 
			resources.ApplyResources(this.logoutButton1, "logoutButton1");
			this.logoutButton1.Name = "logoutButton1";
			this.logoutButton1.UseVisualStyleBackColor = true;
			this.logoutButton1.Click += new System.EventHandler(this.logoutButton1_Click);
			// 
			// AccountInfoForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.logoutButton1);
			this.Controls.Add(this.webBrowser);
			this.Name = "AccountInfoForm";
			this.ShowInTaskbar = false;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AccountInfoForm_FormClosing);
			this.Load += new System.EventHandler(this.UserAccount_Load);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser;
		private StationSystemTray.LogoutButton logoutButton1;
    }
}