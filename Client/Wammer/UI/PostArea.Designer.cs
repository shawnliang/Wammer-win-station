using Waveface.Component;

namespace Waveface
{
    partial class PostArea
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PostArea));
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnCreatePost = new Waveface.Component.XPButton();
            this.labelDisplay = new System.Windows.Forms.Label();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.panelList = new System.Windows.Forms.Panel();
            this.postList = new Waveface.PostsList();
            this.panelButtom = new System.Windows.Forms.Panel();
            this.linkLabelReadMore = new System.Windows.Forms.LinkLabel();
            this.labelPostInfo = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.btnRefresh = new Waveface.Component.XPButton();
            this.panelTop.SuspendLayout();
            this.panelList.SuspendLayout();
            this.panelButtom.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(95)))), ((int)(((byte)(98)))));
            this.panelTop.Controls.Add(this.btnCreatePost);
            this.panelTop.Controls.Add(this.btnRefresh);
            this.panelTop.Controls.Add(this.labelDisplay);
            this.panelTop.Controls.Add(this.comboBoxType);
            this.panelTop.Name = "panelTop";
            // 
            // btnCreatePost
            // 
            resources.ApplyResources(this.btnCreatePost, "btnCreatePost");
            this.btnCreatePost.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnCreatePost.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnCreatePost.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnCreatePost.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(95)))), ((int)(((byte)(98)))));
            this.btnCreatePost.Name = "btnCreatePost";
            this.btnCreatePost.UseVisualStyleBackColor = true;
            this.btnCreatePost.Click += new System.EventHandler(this.btnCreatePost_Click);
            // 
            // labelDisplay
            // 
            resources.ApplyResources(this.labelDisplay, "labelDisplay");
            this.labelDisplay.ForeColor = System.Drawing.Color.White;
            this.labelDisplay.Name = "labelDisplay";
            // 
            // comboBoxType
            // 
            resources.ApplyResources(this.comboBoxType, "comboBoxType");
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Items.AddRange(new object[] {
            resources.GetString("comboBoxType.Items"),
            resources.GetString("comboBoxType.Items1"),
            resources.GetString("comboBoxType.Items2"),
            resources.GetString("comboBoxType.Items3"),
            resources.GetString("comboBoxType.Items4"),
            resources.GetString("comboBoxType.Items5")});
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.SelectedIndexChanged += new System.EventHandler(this.comboBoxType_SelectedIndexChanged);
            // 
            // panelList
            // 
            resources.ApplyResources(this.panelList, "panelList");
            this.panelList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelList.Controls.Add(this.postList);
            this.panelList.Name = "panelList";
            // 
            // postList
            // 
            resources.ApplyResources(this.postList, "postList");
            this.postList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.postList.DetailView = null;
            this.postList.Name = "postList";
            // 
            // panelButtom
            // 
            resources.ApplyResources(this.panelButtom, "panelButtom");
            this.panelButtom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(96)))), ((int)(((byte)(0)))));
            this.panelButtom.Controls.Add(this.linkLabelReadMore);
            this.panelButtom.Controls.Add(this.labelPostInfo);
            this.panelButtom.Name = "panelButtom";
            // 
            // linkLabelReadMore
            // 
            resources.ApplyResources(this.linkLabelReadMore, "linkLabelReadMore");
            this.linkLabelReadMore.LinkColor = System.Drawing.Color.White;
            this.linkLabelReadMore.Name = "linkLabelReadMore";
            this.linkLabelReadMore.TabStop = true;
            this.linkLabelReadMore.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelReadMore_LinkClicked);
            // 
            // labelPostInfo
            // 
            resources.ApplyResources(this.labelPostInfo, "labelPostInfo");
            this.labelPostInfo.ForeColor = System.Drawing.Color.White;
            this.labelPostInfo.Name = "labelPostInfo";
            // 
            // panelMain
            // 
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Controls.Add(this.panelList);
            this.panelMain.Controls.Add(this.panelButtom);
            this.panelMain.Name = "panelMain";
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // btnRefresh
            // 
            resources.ApplyResources(this.btnRefresh, "btnRefresh");
            this.btnRefresh.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnRefresh.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnRefresh.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnRefresh.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(95)))), ((int)(((byte)(98)))));
            this.btnRefresh.Image = global::Waveface.Properties.Resources.refresh;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // PostArea
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.Name = "PostArea";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelList.ResumeLayout(false);
            this.panelButtom.ResumeLayout(false);
            this.panelButtom.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private PostsList postList;
        private System.Windows.Forms.Panel panelList;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Panel panelButtom;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label labelPostInfo;
        private System.Windows.Forms.LinkLabel linkLabelReadMore;
        private System.Windows.Forms.Label labelDisplay;
        private XPButton btnRefresh;
        private XPButton btnCreatePost;
        private Localization.CultureManager cultureManager;
    }
}
