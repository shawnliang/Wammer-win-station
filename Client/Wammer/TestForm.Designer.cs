namespace Waveface
{
    partial class TestForm
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
            this.PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.buttonGetLatest = new System.Windows.Forms.Button();
            this.textBoxPostID = new System.Windows.Forms.TextBox();
            this.buttonGetSingle = new System.Windows.Forms.Button();
            this.textBoxGroupID = new System.Windows.Forms.TextBox();
            this.textBoxGetLatestLimit = new System.Windows.Forms.TextBox();
            this.buttonPreview = new System.Windows.Forms.Button();
            this.buttonPreviewAdv = new System.Windows.Forms.Button();
            this.textBoxPreviewURL = new System.Windows.Forms.TextBox();
            this.buttonPostGet = new System.Windows.Forms.Button();
            this.buttonNewPost = new System.Windows.Forms.Button();
            this.textBoxNewPost = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxGetLimit = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxDatum = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonNewComment = new System.Windows.Forms.Button();
            this.buttonGetComments = new System.Windows.Forms.Button();
            this.buttonUpload = new System.Windows.Forms.Button();
            this.textBoxImageURL = new System.Windows.Forms.TextBox();
            this.buttonFileGet = new System.Windows.Forms.Button();
            this.textBoxFileGet = new System.Windows.Forms.TextBox();
            this.buttonFileDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PropertyGrid
            // 
            this.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Right;
            this.PropertyGrid.HelpVisible = false;
            this.PropertyGrid.Location = new System.Drawing.Point(718, 0);
            this.PropertyGrid.Name = "PropertyGrid";
            this.PropertyGrid.Size = new System.Drawing.Size(517, 688);
            this.PropertyGrid.TabIndex = 0;
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(29, 14);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(89, 40);
            this.buttonLogin.TabIndex = 1;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // buttonGetLatest
            // 
            this.buttonGetLatest.Location = new System.Drawing.Point(29, 78);
            this.buttonGetLatest.Name = "buttonGetLatest";
            this.buttonGetLatest.Size = new System.Drawing.Size(89, 41);
            this.buttonGetLatest.TabIndex = 2;
            this.buttonGetLatest.Text = "Get Latest";
            this.buttonGetLatest.UseVisualStyleBackColor = true;
            this.buttonGetLatest.Click += new System.EventHandler(this.buttonGetLatest_Click);
            // 
            // textBoxPostID
            // 
            this.textBoxPostID.Location = new System.Drawing.Point(416, 109);
            this.textBoxPostID.Name = "textBoxPostID";
            this.textBoxPostID.Size = new System.Drawing.Size(254, 22);
            this.textBoxPostID.TabIndex = 3;
            this.textBoxPostID.Text = "6b47bb35-386e-45f5-af86-47d7453ce9f3";
            // 
            // buttonGetSingle
            // 
            this.buttonGetSingle.Location = new System.Drawing.Point(29, 125);
            this.buttonGetSingle.Name = "buttonGetSingle";
            this.buttonGetSingle.Size = new System.Drawing.Size(89, 41);
            this.buttonGetSingle.TabIndex = 4;
            this.buttonGetSingle.Text = "Get Single";
            this.buttonGetSingle.UseVisualStyleBackColor = true;
            this.buttonGetSingle.Click += new System.EventHandler(this.buttonGetSingle_Click);
            // 
            // textBoxGroupID
            // 
            this.textBoxGroupID.Location = new System.Drawing.Point(416, 76);
            this.textBoxGroupID.Name = "textBoxGroupID";
            this.textBoxGroupID.Size = new System.Drawing.Size(254, 22);
            this.textBoxGroupID.TabIndex = 5;
            this.textBoxGroupID.Text = "7ede8167-0fbf-496a-a2b6-95b476f7391d";
            // 
            // textBoxGetLatestLimit
            // 
            this.textBoxGetLatestLimit.Location = new System.Drawing.Point(178, 88);
            this.textBoxGetLatestLimit.Name = "textBoxGetLatestLimit";
            this.textBoxGetLatestLimit.Size = new System.Drawing.Size(29, 22);
            this.textBoxGetLatestLimit.TabIndex = 6;
            this.textBoxGetLatestLimit.Text = "50";
            // 
            // buttonPreview
            // 
            this.buttonPreview.Location = new System.Drawing.Point(29, 432);
            this.buttonPreview.Name = "buttonPreview";
            this.buttonPreview.Size = new System.Drawing.Size(114, 41);
            this.buttonPreview.TabIndex = 7;
            this.buttonPreview.Text = "Preview";
            this.buttonPreview.UseVisualStyleBackColor = true;
            this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
            // 
            // buttonPreviewAdv
            // 
            this.buttonPreviewAdv.Location = new System.Drawing.Point(29, 480);
            this.buttonPreviewAdv.Name = "buttonPreviewAdv";
            this.buttonPreviewAdv.Size = new System.Drawing.Size(114, 41);
            this.buttonPreviewAdv.TabIndex = 8;
            this.buttonPreviewAdv.Text = "Preview Adv";
            this.buttonPreviewAdv.UseVisualStyleBackColor = true;
            this.buttonPreviewAdv.Click += new System.EventHandler(this.buttonPreviewAdv_Click);
            // 
            // textBoxPreviewURL
            // 
            this.textBoxPreviewURL.Location = new System.Drawing.Point(167, 463);
            this.textBoxPreviewURL.Name = "textBoxPreviewURL";
            this.textBoxPreviewURL.Size = new System.Drawing.Size(503, 22);
            this.textBoxPreviewURL.TabIndex = 9;
            this.textBoxPreviewURL.Text = "http://www.microsoft.com";
            // 
            // buttonPostGet
            // 
            this.buttonPostGet.Location = new System.Drawing.Point(29, 195);
            this.buttonPostGet.Name = "buttonPostGet";
            this.buttonPostGet.Size = new System.Drawing.Size(89, 41);
            this.buttonPostGet.TabIndex = 10;
            this.buttonPostGet.Text = "Get";
            this.buttonPostGet.UseVisualStyleBackColor = true;
            this.buttonPostGet.Click += new System.EventHandler(this.buttonPostGet_Click);
            // 
            // buttonNewPost
            // 
            this.buttonNewPost.Location = new System.Drawing.Point(29, 275);
            this.buttonNewPost.Name = "buttonNewPost";
            this.buttonNewPost.Size = new System.Drawing.Size(89, 41);
            this.buttonNewPost.TabIndex = 11;
            this.buttonNewPost.Text = "New Post";
            this.buttonNewPost.UseVisualStyleBackColor = true;
            this.buttonNewPost.Click += new System.EventHandler(this.buttonNewPost_Click);
            // 
            // textBoxNewPost
            // 
            this.textBoxNewPost.Location = new System.Drawing.Point(270, 285);
            this.textBoxNewPost.Name = "textBoxNewPost";
            this.textBoxNewPost.Size = new System.Drawing.Size(364, 22);
            this.textBoxNewPost.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(349, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 22);
            this.label1.TabIndex = 13;
            this.label1.Text = "Group ID";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(349, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Post ID";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(139, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Limit";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(145, 182);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Limit";
            // 
            // textBoxGetLimit
            // 
            this.textBoxGetLimit.Location = new System.Drawing.Point(194, 179);
            this.textBoxGetLimit.Name = "textBoxGetLimit";
            this.textBoxGetLimit.Size = new System.Drawing.Size(132, 22);
            this.textBoxGetLimit.TabIndex = 16;
            this.textBoxGetLimit.Text = "+3";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(142, 210);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "datum";
            // 
            // textBoxDatum
            // 
            this.textBoxDatum.Location = new System.Drawing.Point(194, 207);
            this.textBoxDatum.Name = "textBoxDatum";
            this.textBoxDatum.Size = new System.Drawing.Size(369, 22);
            this.textBoxDatum.TabIndex = 18;
            this.textBoxDatum.Text = "6b47bb35-386e-45f5-af86-47d7453ce9f3";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(145, 238);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "filter";
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Location = new System.Drawing.Point(194, 235);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(132, 22);
            this.textBoxFilter.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(203, 285);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 22);
            this.label7.TabIndex = 22;
            this.label7.Text = "Content";
            // 
            // buttonNewComment
            // 
            this.buttonNewComment.Location = new System.Drawing.Point(29, 322);
            this.buttonNewComment.Name = "buttonNewComment";
            this.buttonNewComment.Size = new System.Drawing.Size(114, 41);
            this.buttonNewComment.TabIndex = 23;
            this.buttonNewComment.Text = "New Comment";
            this.buttonNewComment.UseVisualStyleBackColor = true;
            this.buttonNewComment.Click += new System.EventHandler(this.buttonNewComment_Click);
            // 
            // buttonGetComments
            // 
            this.buttonGetComments.Location = new System.Drawing.Point(29, 369);
            this.buttonGetComments.Name = "buttonGetComments";
            this.buttonGetComments.Size = new System.Drawing.Size(114, 41);
            this.buttonGetComments.TabIndex = 24;
            this.buttonGetComments.Text = "*Get Comments";
            this.buttonGetComments.UseVisualStyleBackColor = true;
            this.buttonGetComments.Click += new System.EventHandler(this.buttonGetComments_Click);
            // 
            // buttonUpload
            // 
            this.buttonUpload.Location = new System.Drawing.Point(29, 550);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(114, 41);
            this.buttonUpload.TabIndex = 25;
            this.buttonUpload.Text = "Upload";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);
            // 
            // textBoxImageURL
            // 
            this.textBoxImageURL.Location = new System.Drawing.Point(167, 560);
            this.textBoxImageURL.Name = "textBoxImageURL";
            this.textBoxImageURL.Size = new System.Drawing.Size(503, 22);
            this.textBoxImageURL.TabIndex = 26;
            this.textBoxImageURL.Text = "C:\\Users\\WinDev\\Desktop\\Chrysanthemum.jpg";
            // 
            // buttonFileGet
            // 
            this.buttonFileGet.Location = new System.Drawing.Point(29, 597);
            this.buttonFileGet.Name = "buttonFileGet";
            this.buttonFileGet.Size = new System.Drawing.Size(114, 41);
            this.buttonFileGet.TabIndex = 25;
            this.buttonFileGet.Text = "File Get";
            this.buttonFileGet.UseVisualStyleBackColor = true;
            this.buttonFileGet.Click += new System.EventHandler(this.buttonFileGet_Click);
            // 
            // textBoxFileGet
            // 
            this.textBoxFileGet.Location = new System.Drawing.Point(167, 619);
            this.textBoxFileGet.Name = "textBoxFileGet";
            this.textBoxFileGet.Size = new System.Drawing.Size(503, 22);
            this.textBoxFileGet.TabIndex = 26;
            // 
            // buttonFileDelete
            // 
            this.buttonFileDelete.Location = new System.Drawing.Point(29, 644);
            this.buttonFileDelete.Name = "buttonFileDelete";
            this.buttonFileDelete.Size = new System.Drawing.Size(114, 41);
            this.buttonFileDelete.TabIndex = 27;
            this.buttonFileDelete.Text = "File Delete";
            this.buttonFileDelete.UseVisualStyleBackColor = true;
            this.buttonFileDelete.Click += new System.EventHandler(this.buttonFileDelete_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1235, 688);
            this.Controls.Add(this.buttonFileDelete);
            this.Controls.Add(this.textBoxFileGet);
            this.Controls.Add(this.textBoxImageURL);
            this.Controls.Add(this.buttonFileGet);
            this.Controls.Add(this.buttonUpload);
            this.Controls.Add(this.buttonGetComments);
            this.Controls.Add(this.buttonNewComment);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxFilter);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxDatum);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxGetLimit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxNewPost);
            this.Controls.Add(this.buttonNewPost);
            this.Controls.Add(this.buttonPostGet);
            this.Controls.Add(this.textBoxPreviewURL);
            this.Controls.Add(this.buttonPreviewAdv);
            this.Controls.Add(this.buttonPreview);
            this.Controls.Add(this.textBoxGetLatestLimit);
            this.Controls.Add(this.textBoxGroupID);
            this.Controls.Add(this.buttonGetSingle);
            this.Controls.Add(this.textBoxPostID);
            this.Controls.Add(this.buttonGetLatest);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.PropertyGrid);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Name = "TestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TestForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid PropertyGrid;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.Button buttonGetLatest;
        private System.Windows.Forms.TextBox textBoxPostID;
        private System.Windows.Forms.Button buttonGetSingle;
        private System.Windows.Forms.TextBox textBoxGroupID;
        private System.Windows.Forms.TextBox textBoxGetLatestLimit;
        private System.Windows.Forms.Button buttonPreview;
        private System.Windows.Forms.Button buttonPreviewAdv;
        private System.Windows.Forms.TextBox textBoxPreviewURL;
        private System.Windows.Forms.Button buttonPostGet;
        private System.Windows.Forms.Button buttonNewPost;
        private System.Windows.Forms.TextBox textBoxNewPost;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxGetLimit;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxDatum;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonNewComment;
        private System.Windows.Forms.Button buttonGetComments;
        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.TextBox textBoxImageURL;
        private System.Windows.Forms.Button buttonFileGet;
        private System.Windows.Forms.TextBox textBoxFileGet;
        private System.Windows.Forms.Button buttonFileDelete;
    }
}