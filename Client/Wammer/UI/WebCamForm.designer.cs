namespace Waveface.WebCam
{
    //Design by Pongsakorn Poosankam
    partial class WebCamForm
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
            this.imgVideo = new System.Windows.Forms.PictureBox();
            this.imgCapture = new System.Windows.Forms.PictureBox();
            this.bntCapture = new System.Windows.Forms.Button();
            this.bntSave = new System.Windows.Forms.Button();
            this.bntVideoFormat = new System.Windows.Forms.Button();
            this.bntVideoSource = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgVideo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCapture)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgVideo
            // 
            this.imgVideo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.imgVideo.Location = new System.Drawing.Point(14, 26);
            this.imgVideo.Name = "imgVideo";
            this.imgVideo.Size = new System.Drawing.Size(320, 240);
            this.imgVideo.TabIndex = 0;
            this.imgVideo.TabStop = false;
            // 
            // imgCapture
            // 
            this.imgCapture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.imgCapture.Location = new System.Drawing.Point(14, 26);
            this.imgCapture.Name = "imgCapture";
            this.imgCapture.Size = new System.Drawing.Size(320, 240);
            this.imgCapture.TabIndex = 1;
            this.imgCapture.TabStop = false;
            // 
            // bntCapture
            // 
            this.bntCapture.Location = new System.Drawing.Point(375, 308);
            this.bntCapture.Name = "bntCapture";
            this.bntCapture.Size = new System.Drawing.Size(270, 28);
            this.bntCapture.TabIndex = 5;
            this.bntCapture.Text = "Capture Image";
            this.bntCapture.UseVisualStyleBackColor = true;
            this.bntCapture.Click += new System.EventHandler(this.bntCapture_Click);
            // 
            // bntSave
            // 
            this.bntSave.Location = new System.Drawing.Point(651, 308);
            this.bntSave.Name = "bntSave";
            this.bntSave.Size = new System.Drawing.Size(71, 28);
            this.bntSave.TabIndex = 6;
            this.bntSave.Text = "OK";
            this.bntSave.UseVisualStyleBackColor = true;
            this.bntSave.Click += new System.EventHandler(this.bntSave_Click);
            // 
            // bntVideoFormat
            // 
            this.bntVideoFormat.Location = new System.Drawing.Point(14, 308);
            this.bntVideoFormat.Name = "bntVideoFormat";
            this.bntVideoFormat.Size = new System.Drawing.Size(94, 28);
            this.bntVideoFormat.TabIndex = 7;
            this.bntVideoFormat.Text = "Video Format";
            this.bntVideoFormat.UseVisualStyleBackColor = true;
            this.bntVideoFormat.Click += new System.EventHandler(this.bntVideoFormat_Click);
            // 
            // bntVideoSource
            // 
            this.bntVideoSource.Location = new System.Drawing.Point(114, 308);
            this.bntVideoSource.Name = "bntVideoSource";
            this.bntVideoSource.Size = new System.Drawing.Size(94, 28);
            this.bntVideoSource.TabIndex = 8;
            this.bntVideoSource.Text = "Video Source";
            this.bntVideoSource.UseVisualStyleBackColor = true;
            this.bntVideoSource.Click += new System.EventHandler(this.bntVideoSource_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.imgVideo);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(14, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(348, 279);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.imgCapture);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(375, 14);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(347, 279);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Captured Picture";
            // 
            // WebCamForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 347);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bntVideoSource);
            this.Controls.Add(this.bntVideoFormat);
            this.Controls.Add(this.bntSave);
            this.Controls.Add(this.bntCapture);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WebCamForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Take a photo";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WebCamForm_FormClosing);
            this.Load += new System.EventHandler(this.Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgVideo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCapture)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox imgVideo;
        private System.Windows.Forms.PictureBox imgCapture;
        private System.Windows.Forms.Button bntCapture;
        private System.Windows.Forms.Button bntSave;
        private System.Windows.Forms.Button bntVideoFormat;
        private System.Windows.Forms.Button bntVideoSource;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

