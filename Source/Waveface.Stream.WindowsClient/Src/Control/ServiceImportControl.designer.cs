namespace Waveface.Stream.WindowsClient
{
	partial class ServiceImportControl
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
			this.svcItem = new ServiceItemControl();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// svcItem
			// 
			this.svcItem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.svcItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.svcItem.Location = new System.Drawing.Point(23, 109);
			this.svcItem.Margin = new System.Windows.Forms.Padding(10);
			this.svcItem.Name = "svcItem";
			this.svcItem.ServiceEnabled = true;
			this.svcItem.ServiceIcon = null;
			this.svcItem.ServiceName = "service name";
			this.svcItem.Size = new System.Drawing.Size(501, 220);
			this.svcItem.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(23, 34);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(493, 65);
			this.label2.TabIndex = 2;
			this.label2.Text = "A better way to fill your journal with memories from the past. Connect o all your" +
    " favorite web services and let Stream generate real life events for you.";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(23, 339);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(228, 45);
			this.label1.TabIndex = 3;
			this.label1.Text = "You can change theses settings whenever you want.";
			// 
			// ServiceImportControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.svcItem);
			this.Name = "ServiceImportControl";
			this.Size = new System.Drawing.Size(555, 400);
			this.Load += new System.EventHandler(this.ServiceImportControl_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private ServiceItemControl svcItem;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}
