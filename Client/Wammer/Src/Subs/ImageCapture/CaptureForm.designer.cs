namespace Waveface.ImageCapture
{
  partial class CaptureForm
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
      this.SuspendLayout();
      // 
      // ShotForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(264, 266);
      this.Cursor = System.Windows.Forms.Cursors.Cross;
      this.DoubleBuffered = true;
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.ForeColor = System.Drawing.Color.White;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "CaptureForm";
      this.ShowInTaskbar = false;
      this.Text = "ShotForm";
      this.TopMost = true;
      this.Deactivate += new System.EventHandler(this.ShotForm_Deactivate);
      this.Load += new System.EventHandler(this.ShotForm_Load);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ShotForm_KeyDown);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ShotForm_MouseDown);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ShotForm_MouseUp);
      this.ResumeLayout(false);

    }

    #endregion








  }
}