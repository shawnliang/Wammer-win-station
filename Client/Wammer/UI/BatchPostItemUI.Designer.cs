using UtilityLibrary.WinControls;

namespace Waveface
{
    partial class BatchPostItemUI
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.progressBar = new UtilityLibrary.WinControls.ProgressBarEx();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonDetail = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.pictureBoxStatu = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatu)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.BackgroundBitmap = null;
            this.progressBar.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.progressBar.Border3D = System.Windows.Forms.Border3DStyle.Sunken;
            this.progressBar.BorderColor = System.Drawing.Color.Black;
            this.progressBar.Enable3DBorder = true;
            this.progressBar.ForegroundBitmap = null;
            this.progressBar.ForegroundColor = System.Drawing.SystemColors.HotTrack;
            this.progressBar.GradientEndColor = System.Drawing.Color.Empty;
            this.progressBar.GradientMiddleColor = System.Drawing.Color.Empty;
            this.progressBar.GradientStartColor = System.Drawing.Color.Empty;
            this.progressBar.Location = new System.Drawing.Point(7, 8);
            this.progressBar.Maximum = 100;
            this.progressBar.Minimum = 0;
            this.progressBar.Name = "progressBar";
            this.progressBar.Orientation = UtilityLibrary.WinControls.Orientation.Horizontal;
            this.progressBar.ProgressText = "";
            this.progressBar.ProgressTextColor = System.Drawing.Color.Empty;
            this.progressBar.ProgressTextHiglightColor = System.Drawing.Color.Empty;
            this.progressBar.ShowProgressText = true;
            this.progressBar.Size = new System.Drawing.Size(156, 28);
            this.progressBar.Smooth = true;
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 0;
            this.progressBar.Value = 0;
            this.progressBar.WaitingGradientSize = 30;
            this.progressBar.WaitingSpeed = 25;
            this.progressBar.WaitingStep = 5;
            // 
            // buttonStart
            // 
            this.buttonStart.BackgroundImage = global::Waveface.Properties.Resources.postItem_play;
            this.buttonStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonStart.Location = new System.Drawing.Point(7, 41);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(26, 26);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonDetail
            // 
            this.buttonDetail.BackgroundImage = global::Waveface.Properties.Resources.postItem_arrow_out;
            this.buttonDetail.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonDetail.Location = new System.Drawing.Point(71, 41);
            this.buttonDetail.Name = "buttonDetail";
            this.buttonDetail.Size = new System.Drawing.Size(26, 26);
            this.buttonDetail.TabIndex = 2;
            this.buttonDetail.UseVisualStyleBackColor = true;
            this.buttonDetail.Click += new System.EventHandler(this.buttonDetail_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.BackgroundImage = global::Waveface.Properties.Resources.postItem_delete;
            this.buttonDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonDelete.Location = new System.Drawing.Point(39, 41);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(26, 26);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // pictureBoxStatu
            // 
            this.pictureBoxStatu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxStatu.BackgroundImage = global::Waveface.Properties.Resources.postItem_red;
            this.pictureBoxStatu.Location = new System.Drawing.Point(145, 46);
            this.pictureBoxStatu.Name = "pictureBoxStatu";
            this.pictureBoxStatu.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxStatu.TabIndex = 4;
            this.pictureBoxStatu.TabStop = false;
            // 
            // BatchPostItemUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonDetail);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.pictureBoxStatu);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Name = "BatchPostItemUI";
            this.Size = new System.Drawing.Size(170, 73);
            this.Load += new System.EventHandler(this.BatchPostItemUI_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ProgressBarEx progressBar;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonDetail;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.PictureBox pictureBoxStatu;
    }
}
