
namespace Waveface.Stream.WindowsClient
{
	partial class FBLoginButton : System.Windows.Forms.UserControl
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
			this.SuspendLayout();
			// 
			// FBLoginButton
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Name = "FBLoginButton";
			this.Size = new System.Drawing.Size(194, 60);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FBLoginButton_MouseDown);
			this.MouseLeave += new System.EventHandler(this.FBLoginButton_MouseLeave);
			this.MouseHover += new System.EventHandler(this.FBLoginButton_MouseHover);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FBLoginButton_MouseUp);
			this.ResumeLayout(false);

		}

		#endregion

	}
}
