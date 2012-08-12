namespace Waveface.SettingUI
{
	partial class FormOption
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOption));
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.firefoxDialog = new Waveface.Component.FirefoxDialog.FirefoxDialog();
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "General.ico");
            this.imageList.Images.SetKeyName(1, "Email.ico");
            // 
            // firefoxDialog
            // 
            this.firefoxDialog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.firefoxDialog.Font = new System.Drawing.Font("Tahoma", 9F);
            this.firefoxDialog.ImageList = null;
            this.firefoxDialog.Location = new System.Drawing.Point(0, 0);
            this.firefoxDialog.Name = "firefoxDialog";
            this.firefoxDialog.Size = new System.Drawing.Size(395, 289);
            this.firefoxDialog.TabIndex = 0;
            // 
            // FormOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 289);
            this.Controls.Add(this.firefoxDialog);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormOption";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.Form_Load);
            this.ResumeLayout(false);

		}

		#endregion

		private Waveface.Component.FirefoxDialog.FirefoxDialog firefoxDialog;
		private System.Windows.Forms.ImageList imageList;




	}
}

