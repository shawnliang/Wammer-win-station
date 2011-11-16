namespace Waveface
{
    partial class SelectKickUser
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
            this.listViewUser = new System.Windows.Forms.ListView();
            this.columnHeaderUName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderUMail = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewUser
            // 
            this.listViewUser.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewUser.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderUName,
            this.columnHeaderUMail});
            this.listViewUser.FullRowSelect = true;
            this.listViewUser.HideSelection = false;
            this.listViewUser.Location = new System.Drawing.Point(12, 12);
            this.listViewUser.Name = "listViewUser";
            this.listViewUser.Size = new System.Drawing.Size(381, 210);
            this.listViewUser.TabIndex = 3;
            this.listViewUser.UseCompatibleStateImageBehavior = false;
            this.listViewUser.View = System.Windows.Forms.View.Details;
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
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(169, 237);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(69, 26);
            this.buttonOK.TabIndex = 9;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // SelectUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 275);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.listViewUser);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectInviteUser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select User";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewUser;
        private System.Windows.Forms.ColumnHeader columnHeaderUName;
        private System.Windows.Forms.ColumnHeader columnHeaderUMail;
        private System.Windows.Forms.Button buttonOK;
    }
}