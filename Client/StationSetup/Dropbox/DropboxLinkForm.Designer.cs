namespace Wammer.Station
{
    partial class DropboxLinkForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DropboxLinkForm));
            this.buttonVerift = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.multiPanel = new Waveface.Component.MultiPage.MultiPanel();
            this.Page_Main = new Waveface.Component.MultiPage.MultiPanelPage();
            this.Page_Verify = new Waveface.Component.MultiPage.MultiPanelPage();
            this.Page_successfully = new Waveface.Component.MultiPage.MultiPanelPage();
            this.Page_RetryOrSkip = new Waveface.Component.MultiPage.MultiPanelPage();
            this.buttonRetry = new System.Windows.Forms.Button();
            this.buttonSkip = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.multiPanel.SuspendLayout();
            this.Page_Main.SuspendLayout();
            this.Page_Verify.SuspendLayout();
            this.Page_successfully.SuspendLayout();
            this.Page_RetryOrSkip.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonVerift
            // 
            resources.ApplyResources(this.buttonVerift, "buttonVerift");
            this.buttonVerift.Name = "buttonVerift";
            this.buttonVerift.UseVisualStyleBackColor = true;
            this.buttonVerift.Click += new System.EventHandler(this.buttonVerift_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // multiPanel
            // 
            this.multiPanel.Controls.Add(this.Page_Main);
            this.multiPanel.Controls.Add(this.Page_Verify);
            this.multiPanel.Controls.Add(this.Page_successfully);
            this.multiPanel.Controls.Add(this.Page_RetryOrSkip);
            resources.ApplyResources(this.multiPanel, "multiPanel");
            this.multiPanel.Name = "multiPanel";
            this.multiPanel.SelectedPage = this.Page_successfully;
            // 
            // Page_Main
            // 
            this.Page_Main.Controls.Add(this.button1);
            this.Page_Main.Controls.Add(this.buttonVerift);
            resources.ApplyResources(this.Page_Main, "Page_Main");
            this.Page_Main.Name = "Page_Main";
            // 
            // Page_Verify
            // 
            this.Page_Verify.Controls.Add(this.label1);
            resources.ApplyResources(this.Page_Verify, "Page_Verify");
            this.Page_Verify.Name = "Page_Verify";
            // 
            // Page_successfully
            // 
            this.Page_successfully.Controls.Add(this.buttonOK);
            resources.ApplyResources(this.Page_successfully, "Page_successfully");
            this.Page_successfully.Name = "Page_successfully";
            // 
            // Page_RetryOrSkip
            // 
            this.Page_RetryOrSkip.Controls.Add(this.buttonRetry);
            this.Page_RetryOrSkip.Controls.Add(this.buttonSkip);
            resources.ApplyResources(this.Page_RetryOrSkip, "Page_RetryOrSkip");
            this.Page_RetryOrSkip.Name = "Page_RetryOrSkip";
            // 
            // buttonRetry
            // 
            resources.ApplyResources(this.buttonRetry, "buttonRetry");
            this.buttonRetry.Name = "buttonRetry";
            this.buttonRetry.UseVisualStyleBackColor = true;
            // 
            // buttonSkip
            // 
            resources.ApplyResources(this.buttonSkip, "buttonSkip");
            this.buttonSkip.Name = "buttonSkip";
            this.buttonSkip.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // DropboxLinkForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.multiPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DropboxLinkForm";
            this.ShowInTaskbar = false;
            this.multiPanel.ResumeLayout(false);
            this.Page_Main.ResumeLayout(false);
            this.Page_Verify.ResumeLayout(false);
            this.Page_Verify.PerformLayout();
            this.Page_successfully.ResumeLayout(false);
            this.Page_RetryOrSkip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonVerift;
        private Waveface.Localization.CultureManager cultureManager;
        private System.Windows.Forms.Button button1;
        private Waveface.Component.MultiPage.MultiPanel multiPanel;
        private Waveface.Component.MultiPage.MultiPanelPage Page_Main;
        private Waveface.Component.MultiPage.MultiPanelPage Page_Verify;
        private Waveface.Component.MultiPage.MultiPanelPage Page_successfully;
        private Waveface.Component.MultiPage.MultiPanelPage Page_RetryOrSkip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonRetry;
        private System.Windows.Forms.Button buttonSkip;
        private System.Windows.Forms.Button buttonOK;
    }
}