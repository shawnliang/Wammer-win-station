namespace Waveface.PostUI
{
    partial class RichText
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
            this.htmlEditorControl = new Waveface.Component.HtmlEditor.HtmlEditorControl();
            this.buttonTest = new System.Windows.Forms.Button();
            this.btnSend = new Waveface.Component.XPButton();
            this.SuspendLayout();
            // 
            // htmlEditorControl
            // 
            this.htmlEditorControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.htmlEditorControl.InnerText = null;
            this.htmlEditorControl.Location = new System.Drawing.Point(6, 6);
            this.htmlEditorControl.Name = "htmlEditorControl";
            this.htmlEditorControl.Size = new System.Drawing.Size(631, 341);
            this.htmlEditorControl.TabIndex = 27;
            this.htmlEditorControl.HtmlException += new Waveface.Component.HtmlEditor.HtmlExceptionEventHandler(this.htmlEditorControl_HtmlException);
            this.htmlEditorControl.HtmlNavigation += new Waveface.Component.HtmlEditor.HtmlNavigationEventHandler(this.htmlEditorControl_HtmlNavigation);
            // 
            // buttonTest
            // 
            this.buttonTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTest.Location = new System.Drawing.Point(655, 69);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(50, 45);
            this.buttonTest.TabIndex = 28;
            this.buttonTest.Text = "Test";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Visible = false;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // btnSend
            // 
            this.btnSend.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSend.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSend.Image = global::Waveface.Properties.Resources.Post;
            this.btnSend.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSend.Location = new System.Drawing.Point(643, 6);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(74, 32);
            this.btnSend.TabIndex = 8;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // RichText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.htmlEditorControl);
            this.Controls.Add(this.btnSend);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.MinimumSize = new System.Drawing.Size(500, 130);
            this.Name = "RichText";
            this.Size = new System.Drawing.Size(722, 360);
            this.ResumeLayout(false);

        }

        #endregion

        private Component.XPButton btnSend;
        private Component.HtmlEditor.HtmlEditorControl htmlEditorControl;
        private System.Windows.Forms.Button buttonTest;
    }
}
