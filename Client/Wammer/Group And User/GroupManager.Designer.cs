using Waveface.Component;

namespace Waveface
{
    partial class GroupManager
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
            this.listViewGroup = new System.Windows.Forms.ListView();
            this.columnHeaderGName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderGOwner = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderGDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewUser = new System.Windows.Forms.ListView();
            this.columnHeaderUName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderUMail = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.pictureBoxAvatar = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxMail = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxAvatar = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.signupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonUpdateUser = new Waveface.Component.XPButton();
            this.buttonCreateGroup = new Waveface.Component.XPButton();
            this.buttonDeleteGroup = new Waveface.Component.XPButton();
            this.buttonUpdateGroup = new Waveface.Component.XPButton();
            this.buttonInviteUser = new Waveface.Component.XPButton();
            this.buttonKickUser = new Waveface.Component.XPButton();
            this.buttonChangePW = new Waveface.Component.XPButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).BeginInit();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewGroup
            // 
            this.listViewGroup.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewGroup.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderGName,
            this.columnHeaderGOwner,
            this.columnHeaderGDescription});
            this.listViewGroup.FullRowSelect = true;
            this.listViewGroup.Location = new System.Drawing.Point(21, 176);
            this.listViewGroup.Name = "listViewGroup";
            this.listViewGroup.Size = new System.Drawing.Size(586, 179);
            this.listViewGroup.TabIndex = 1;
            this.listViewGroup.UseCompatibleStateImageBehavior = false;
            this.listViewGroup.View = System.Windows.Forms.View.Details;
            this.listViewGroup.Click += new System.EventHandler(this.listViewGroup_Click);
            // 
            // columnHeaderGName
            // 
            this.columnHeaderGName.Text = "Name";
            this.columnHeaderGName.Width = 115;
            // 
            // columnHeaderGOwner
            // 
            this.columnHeaderGOwner.Text = "Owner";
            this.columnHeaderGOwner.Width = 87;
            // 
            // columnHeaderGDescription
            // 
            this.columnHeaderGDescription.Text = "Description";
            this.columnHeaderGDescription.Width = 211;
            // 
            // listViewUser
            // 
            this.listViewUser.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewUser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewUser.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderUName,
            this.columnHeaderUMail});
            this.listViewUser.FullRowSelect = true;
            this.listViewUser.HideSelection = false;
            this.listViewUser.Location = new System.Drawing.Point(21, 393);
            this.listViewUser.Name = "listViewUser";
            this.listViewUser.Size = new System.Drawing.Size(586, 147);
            this.listViewUser.TabIndex = 2;
            this.listViewUser.UseCompatibleStateImageBehavior = false;
            this.listViewUser.View = System.Windows.Forms.View.Details;
            this.listViewUser.Click += new System.EventHandler(this.listViewUser_Click);
            // 
            // columnHeaderUName
            // 
            this.columnHeaderUName.Text = "Name";
            this.columnHeaderUName.Width = 115;
            // 
            // columnHeaderUMail
            // 
            this.columnHeaderUMail.Text = "Mail";
            this.columnHeaderUMail.Width = 87;
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.Location = new System.Drawing.Point(624, 394);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(80, 80);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox.TabIndex = 3;
            this.pictureBox.TabStop = false;
            // 
            // pictureBoxAvatar
            // 
            this.pictureBoxAvatar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxAvatar.Location = new System.Drawing.Point(12, 9);
            this.pictureBoxAvatar.Name = "pictureBoxAvatar";
            this.pictureBoxAvatar.Size = new System.Drawing.Size(80, 80);
            this.pictureBoxAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxAvatar.TabIndex = 4;
            this.pictureBoxAvatar.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 14);
            this.label1.TabIndex = 5;
            this.label1.Text = "Group:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 376);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 14);
            this.label2.TabIndex = 6;
            this.label2.Text = "User:";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(156, 11);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(211, 22);
            this.textBoxName.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(106, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 14);
            this.label3.TabIndex = 8;
            this.label3.Text = "Name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(106, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 14);
            this.label4.TabIndex = 10;
            this.label4.Text = "email:";
            // 
            // textBoxMail
            // 
            this.textBoxMail.Location = new System.Drawing.Point(156, 39);
            this.textBoxMail.Name = "textBoxMail";
            this.textBoxMail.ReadOnly = true;
            this.textBoxMail.Size = new System.Drawing.Size(211, 22);
            this.textBoxMail.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(106, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 14);
            this.label5.TabIndex = 12;
            this.label5.Text = "Avatar:";
            // 
            // textBoxAvatar
            // 
            this.textBoxAvatar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAvatar.Location = new System.Drawing.Point(156, 67);
            this.textBoxAvatar.Name = "textBoxAvatar";
            this.textBoxAvatar.Size = new System.Drawing.Size(423, 22);
            this.textBoxAvatar.TabIndex = 11;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.textBoxAvatar);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.textBoxMail);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.textBoxName);
            this.panel1.Controls.Add(this.pictureBoxAvatar);
            this.panel1.Location = new System.Drawing.Point(12, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(595, 103);
            this.panel1.TabIndex = 13;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginToolStripMenuItem,
            this.userToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(719, 24);
            this.menuStrip1.TabIndex = 14;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.loginToolStripMenuItem.Text = "Login";
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.loginToolStripMenuItem_Click);
            // 
            // userToolStripMenuItem
            // 
            this.userToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.signupToolStripMenuItem});
            this.userToolStripMenuItem.Name = "userToolStripMenuItem";
            this.userToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.userToolStripMenuItem.Text = "User";
            // 
            // signupToolStripMenuItem
            // 
            this.signupToolStripMenuItem.Name = "signupToolStripMenuItem";
            this.signupToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.signupToolStripMenuItem.Text = "Signup";
            this.signupToolStripMenuItem.Click += new System.EventHandler(this.signupToolStripMenuItem_Click);
            // 
            // buttonUpdateUser
            // 
            this.buttonUpdateUser.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonUpdateUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUpdateUser.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonUpdateUser.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonUpdateUser.Enabled = false;
            this.buttonUpdateUser.Location = new System.Drawing.Point(616, 39);
            this.buttonUpdateUser.Name = "buttonUpdateUser";
            this.buttonUpdateUser.Size = new System.Drawing.Size(95, 31);
            this.buttonUpdateUser.TabIndex = 15;
            this.buttonUpdateUser.Text = "Update";
            this.buttonUpdateUser.UseVisualStyleBackColor = true;
            this.buttonUpdateUser.Click += new System.EventHandler(this.buttonUpdateUser_Click);
            // 
            // buttonCreateGroup
            // 
            this.buttonCreateGroup.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonCreateGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreateGroup.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonCreateGroup.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonCreateGroup.Enabled = false;
            this.buttonCreateGroup.Location = new System.Drawing.Point(616, 176);
            this.buttonCreateGroup.Name = "buttonCreateGroup";
            this.buttonCreateGroup.Size = new System.Drawing.Size(95, 31);
            this.buttonCreateGroup.TabIndex = 16;
            this.buttonCreateGroup.Text = "Create";
            this.buttonCreateGroup.UseVisualStyleBackColor = true;
            this.buttonCreateGroup.Click += new System.EventHandler(this.buttonCreateGroup_Click);
            // 
            // buttonDeleteGroup
            // 
            this.buttonDeleteGroup.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonDeleteGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDeleteGroup.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonDeleteGroup.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonDeleteGroup.Enabled = false;
            this.buttonDeleteGroup.Location = new System.Drawing.Point(616, 213);
            this.buttonDeleteGroup.Name = "buttonDeleteGroup";
            this.buttonDeleteGroup.Size = new System.Drawing.Size(95, 31);
            this.buttonDeleteGroup.TabIndex = 17;
            this.buttonDeleteGroup.Text = "Delete";
            this.buttonDeleteGroup.UseVisualStyleBackColor = true;
            this.buttonDeleteGroup.Click += new System.EventHandler(this.buttonDeleteGroup_Click);
            // 
            // buttonUpdateGroup
            // 
            this.buttonUpdateGroup.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonUpdateGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUpdateGroup.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonUpdateGroup.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonUpdateGroup.Enabled = false;
            this.buttonUpdateGroup.Location = new System.Drawing.Point(616, 250);
            this.buttonUpdateGroup.Name = "buttonUpdateGroup";
            this.buttonUpdateGroup.Size = new System.Drawing.Size(95, 31);
            this.buttonUpdateGroup.TabIndex = 18;
            this.buttonUpdateGroup.Text = "Update";
            this.buttonUpdateGroup.UseVisualStyleBackColor = true;
            this.buttonUpdateGroup.Click += new System.EventHandler(this.buttonUpdateGroup_Click);
            // 
            // buttonInviteUser
            // 
            this.buttonInviteUser.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonInviteUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInviteUser.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonInviteUser.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonInviteUser.Enabled = false;
            this.buttonInviteUser.Location = new System.Drawing.Point(616, 287);
            this.buttonInviteUser.Name = "buttonInviteUser";
            this.buttonInviteUser.Size = new System.Drawing.Size(95, 31);
            this.buttonInviteUser.TabIndex = 19;
            this.buttonInviteUser.Text = "Invite User";
            this.buttonInviteUser.UseVisualStyleBackColor = true;
            this.buttonInviteUser.Click += new System.EventHandler(this.buttonInviteUser_Click);
            // 
            // buttonKickUser
            // 
            this.buttonKickUser.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonKickUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonKickUser.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonKickUser.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonKickUser.Enabled = false;
            this.buttonKickUser.Location = new System.Drawing.Point(616, 324);
            this.buttonKickUser.Name = "buttonKickUser";
            this.buttonKickUser.Size = new System.Drawing.Size(95, 31);
            this.buttonKickUser.TabIndex = 20;
            this.buttonKickUser.Text = "kick User";
            this.buttonKickUser.UseVisualStyleBackColor = true;
            this.buttonKickUser.Click += new System.EventHandler(this.buttonKickUser_Click);
            // 
            // buttonChangePW
            // 
            this.buttonChangePW.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonChangePW.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonChangePW.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonChangePW.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonChangePW.Enabled = false;
            this.buttonChangePW.Location = new System.Drawing.Point(616, 76);
            this.buttonChangePW.Name = "buttonChangePW";
            this.buttonChangePW.Size = new System.Drawing.Size(95, 37);
            this.buttonChangePW.TabIndex = 21;
            this.buttonChangePW.Text = "Change Password";
            this.buttonChangePW.UseVisualStyleBackColor = true;
            this.buttonChangePW.Click += new System.EventHandler(this.buttonChangePW_Click);
            // 
            // GroupManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(719, 552);
            this.Controls.Add(this.buttonChangePW);
            this.Controls.Add(this.buttonKickUser);
            this.Controls.Add(this.buttonInviteUser);
            this.Controls.Add(this.buttonUpdateGroup);
            this.Controls.Add(this.buttonDeleteGroup);
            this.Controls.Add(this.buttonCreateGroup);
            this.Controls.Add(this.buttonUpdateUser);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.listViewUser);
            this.Controls.Add(this.listViewGroup);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimizeBox = false;
            this.Name = "GroupManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "User / Group Manager";
            this.Load += new System.EventHandler(this.GroupManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewGroup;
        private System.Windows.Forms.ColumnHeader columnHeaderGName;
        private System.Windows.Forms.ColumnHeader columnHeaderGOwner;
        private System.Windows.Forms.ColumnHeader columnHeaderGDescription;
        private System.Windows.Forms.ListView listViewUser;
        private System.Windows.Forms.ColumnHeader columnHeaderUName;
        private System.Windows.Forms.ColumnHeader columnHeaderUMail;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.PictureBox pictureBoxAvatar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxMail;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxAvatar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private XPButton buttonUpdateUser;
        private XPButton buttonCreateGroup;
        private XPButton buttonDeleteGroup;
        private System.Windows.Forms.ToolStripMenuItem userToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem signupToolStripMenuItem;
        private XPButton buttonUpdateGroup;
        private XPButton buttonInviteUser;
        private XPButton buttonKickUser;
        private XPButton buttonChangePW;
    }
}