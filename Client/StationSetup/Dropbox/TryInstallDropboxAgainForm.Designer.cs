namespace Wammer.Station
{
    partial class TryInstallDropboxAgainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TryInstallDropboxAgainForm));
            this.buttonSkip = new System.Windows.Forms.Button();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.buttonRetry = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSkip
            // 
            resources.ApplyResources(this.buttonSkip, "buttonSkip");
            this.buttonSkip.Name = "buttonSkip";
            this.buttonSkip.UseVisualStyleBackColor = true;
            this.buttonSkip.Click += new System.EventHandler(this.buttonSkip_Click);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // buttonRetry
            // 
            resources.ApplyResources(this.buttonRetry, "buttonRetry");
            this.buttonRetry.Name = "buttonRetry";
            this.buttonRetry.UseVisualStyleBackColor = true;
            this.buttonRetry.Click += new System.EventHandler(this.buttonRetry_Click);
            // 
            // TryInstallDropboxAgainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.buttonRetry);
            this.Controls.Add(this.buttonSkip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TryInstallDropboxAgainForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSkip;
        private Waveface.Localization.CultureManager cultureManager;
        private System.Windows.Forms.Button buttonRetry;
    }
}