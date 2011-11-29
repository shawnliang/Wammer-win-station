using System;
using System.ComponentModel;
using System.Windows.Forms;
using Waveface.Component;
using Waveface.Configuration;

namespace Waveface
{
    public class LoginForm : Form
    {
        internal CheckBox cbRemember;
        internal Panel Panel1;
        internal Label lblUserName;
        internal TextBox txtPassword;
        internal TextBox txtUserName;
        internal Label lblPassword;

        private Container components = null;

        private Label lHeader;
        private Label lText;
        private PictureBox pbImage;
        private XPButton btnCancel;
        private XPButton btnOK;
        private GroupBox groupBox1;

        private FormSettings m_formSettings;

        #region Properties

        public string User
        {
            get
            {
                return txtUserName.Text.Trim();
            }
        }

        public string Password
        {
            get
            {
                return txtPassword.Text.Trim();
            }
        }

        #endregion

        #region Setting

        public string UserSetting
        {
            get
            {
                return txtUserName.Text.Trim();
            }
            set
            {
                txtUserName.Text = value;
            }
        }

        public string PasswordSetting
        {
            get
            {
                return RememberPassword ? txtPassword.Text.Trim() : "";
            }
            set
            {
                txtPassword.Text = value;
            }
        }

        public bool RememberPassword
        {
            get
            {
                return cbRemember.Checked;
            }
            set
            {
                cbRemember.Checked = value;
            }
        }

        #endregion

        public LoginForm(string email, string password)
        {
            if ((email != string.Empty) && (password != string.Empty))
                doLogin(email, password);

            InitializeComponent();

            m_formSettings = new FormSettings(this);
            m_formSettings.SaveOnClose = false;
            m_formSettings.Settings.Add(new PropertySetting(this, "UserSetting"));
            m_formSettings.Settings.Add(new PropertySetting(this, "PasswordSetting"));
            m_formSettings.Settings.Add(new PropertySetting(this, "RememberPassword"));
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.cbRemember = new System.Windows.Forms.CheckBox();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.lHeader = new System.Windows.Forms.Label();
            this.lText = new System.Windows.Forms.Label();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new Waveface.Component.XPButton();
            this.btnOK = new Waveface.Component.XPButton();
            this.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbRemember
            // 
            this.cbRemember.AccessibleDescription = "Remember password";
            this.cbRemember.AccessibleName = "Remember Password";
            this.cbRemember.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbRemember.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cbRemember.Location = new System.Drawing.Point(91, 89);
            this.cbRemember.Name = "cbRemember";
            this.cbRemember.Size = new System.Drawing.Size(153, 28);
            this.cbRemember.TabIndex = 3;
            this.cbRemember.Text = "Remember Password";
            // 
            // Panel1
            // 
            this.Panel1.BackColor = System.Drawing.Color.White;
            this.Panel1.Controls.Add(this.lHeader);
            this.Panel1.Controls.Add(this.lText);
            this.Panel1.Controls.Add(this.pbImage);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel1.Location = new System.Drawing.Point(0, 0);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(386, 60);
            this.Panel1.TabIndex = 0;
            // 
            // lHeader
            // 
            this.lHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lHeader.Location = new System.Drawing.Point(16, 8);
            this.lHeader.Name = "lHeader";
            this.lHeader.Size = new System.Drawing.Size(136, 20);
            this.lHeader.TabIndex = 6;
            this.lHeader.Text = "Waveface Login";
            this.lHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lText
            // 
            this.lText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lText.Location = new System.Drawing.Point(32, 30);
            this.lText.Name = "lText";
            this.lText.Size = new System.Drawing.Size(256, 20);
            this.lText.TabIndex = 7;
            this.lText.Text = "Please enter your Username and Password.";
            this.lText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbImage
            // 
            this.pbImage.Image = ((System.Drawing.Image)(resources.GetObject("pbImage.Image")));
            this.pbImage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbImage.Location = new System.Drawing.Point(320, 6);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(48, 48);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbImage.TabIndex = 8;
            this.pbImage.TabStop = false;
            // 
            // lblUserName
            // 
            this.lblUserName.AccessibleDescription = "User Name";
            this.lblUserName.AccessibleName = "User Name";
            this.lblUserName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblUserName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblUserName.Location = new System.Drawing.Point(19, 24);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(63, 15);
            this.lblUserName.TabIndex = 1;
            this.lblUserName.Text = "User name:";
            // 
            // txtPassword
            // 
            this.txtPassword.AccessibleDescription = "Password";
            this.txtPassword.AccessibleName = "Password";
            this.txtPassword.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtPassword.Location = new System.Drawing.Point(91, 61);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(216, 22);
            this.txtPassword.TabIndex = 2;
            // 
            // txtUserName
            // 
            this.txtUserName.AccessibleDescription = "User name";
            this.txtUserName.AccessibleName = "User name";
            this.txtUserName.Location = new System.Drawing.Point(91, 21);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(216, 22);
            this.txtUserName.TabIndex = 1;
            // 
            // lblPassword
            // 
            this.lblPassword.AccessibleDescription = "Password";
            this.lblPassword.AccessibleName = "Password";
            this.lblPassword.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblPassword.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblPassword.Location = new System.Drawing.Point(19, 64);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(64, 18);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Password:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblUserName);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.lblPassword);
            this.groupBox1.Controls.Add(this.cbRemember);
            this.groupBox1.Location = new System.Drawing.Point(32, 72);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(324, 123);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnCancel.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnCancel.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageIndex = 1;
            this.btnCancel.Location = new System.Drawing.Point(281, 210);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnOK.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnOK.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.ImageIndex = 0;
            this.btnOK.Location = new System.Drawing.Point(201, 210);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 28);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // LoginForm
            // 
            this.AccessibleDescription = "Login form";
            this.AccessibleName = "Login Form";
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(386, 249);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.Panel1);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtUserName.Focus();
        }

        private void doLogin(string email, string password)
        {
            Hide();

            MainForm _mailForm = new MainForm();
            _mailForm.Reset();

            if (_mailForm.Login(email, password))
            {
                _mailForm.ShowDialog();
                _mailForm.Dispose();
                _mailForm = null;
            }
            else
            {
                MessageBox.Show("Login Error!");
            }

            Show();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.None;

            if ((txtUserName.Text.Trim() != "") && (txtPassword.Text.Trim() != ""))
            {
                m_formSettings.Save();

                doLogin(txtUserName.Text.Trim(), txtPassword.Text.Trim());
            }
            else
            {
                MessageBox.Show("Please re-type your information.", "System Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}