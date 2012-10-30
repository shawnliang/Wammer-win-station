namespace StationSystemTray.Src.Control
{
	partial class SignUpControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignUpControl));
			this.label1 = new System.Windows.Forms.Label();
			this.signupButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Font = new System.Drawing.Font("Arial", 10F);
			this.label1.Location = new System.Drawing.Point(32, 47);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(387, 80);
			this.label1.TabIndex = 0;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// signupButton
			// 
			this.signupButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.signupButton.Font = new System.Drawing.Font("Arial", 10F);
			this.signupButton.Location = new System.Drawing.Point(105, 183);
			this.signupButton.Name = "signupButton";
			this.signupButton.Size = new System.Drawing.Size(252, 43);
			this.signupButton.TabIndex = 1;
			this.signupButton.Text = "Create a Stream account";
			this.signupButton.UseVisualStyleBackColor = true;
			this.signupButton.Click += new System.EventHandler(this.signupButton_Click);
			// 
			// SignUpControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.signupButton);
			this.Controls.Add(this.label1);
			this.Name = "SignUpControl";
			this.Size = new System.Drawing.Size(463, 372);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button signupButton;


	}
}
