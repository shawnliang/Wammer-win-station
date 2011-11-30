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
        private IContainer components;

        private Label lHeader;
        private Label lText;
        private PictureBox pbImage;
        private XPButton btnCancel;
        private XPButton btnOK;
        private GroupBox groupBox1;
        private Localization.CultureManager cultureManager;

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
            InitializeComponent();

            m_formSettings = new FormSettings(this);
            m_formSettings.SaveOnClose = false;
            m_formSettings.Settings.Add(new PropertySetting(this, "UserSetting"));
            m_formSettings.Settings.Add(new PropertySetting(this, "PasswordSetting"));
            m_formSettings.Settings.Add(new PropertySetting(this, "RememberPassword"));

            if ((email != string.Empty) && (password != string.Empty))
            {
                txtUserName.Text = email;
                txtPassword.Text = password;

                doLogin(email, password);
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
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
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.btnCancel = new Waveface.Component.XPButton();
            this.btnOK = new Waveface.Component.XPButton();
            this.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbRemember
            // 
            resources.ApplyResources(this.cbRemember, "cbRemember");
            this.cbRemember.Name = "cbRemember";
            // 
            // Panel1
            // 
            this.Panel1.BackColor = System.Drawing.Color.White;
            this.Panel1.Controls.Add(this.lHeader);
            this.Panel1.Controls.Add(this.lText);
            this.Panel1.Controls.Add(this.pbImage);
            resources.ApplyResources(this.Panel1, "Panel1");
            this.Panel1.Name = "Panel1";
            // 
            // lHeader
            // 
            resources.ApplyResources(this.lHeader, "lHeader");
            this.lHeader.Name = "lHeader";
            // 
            // lText
            // 
            resources.ApplyResources(this.lText, "lText");
            this.lText.Name = "lText";
            // 
            // pbImage
            // 
            resources.ApplyResources(this.pbImage, "pbImage");
            this.pbImage.Name = "pbImage";
            this.pbImage.TabStop = false;
            // 
            // lblUserName
            // 
            resources.ApplyResources(this.lblUserName, "lblUserName");
            this.lblUserName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblUserName.Name = "lblUserName";
            // 
            // txtPassword
            // 
            resources.ApplyResources(this.txtPassword, "txtPassword");
            this.txtPassword.Name = "txtPassword";
            // 
            // txtUserName
            // 
            resources.ApplyResources(this.txtUserName, "txtUserName");
            this.txtUserName.Name = "txtUserName";
            // 
            // lblPassword
            // 
            resources.ApplyResources(this.lblPassword, "lblPassword");
            this.lblPassword.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblPassword.Name = "lblPassword";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblUserName);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.lblPassword);
            this.groupBox1.Controls.Add(this.cbRemember);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // btnCancel
            // 
            this.btnCancel.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnCancel.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnCancel.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnOK.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnOK.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // LoginForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.Panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
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
            Cursor.Current = Cursors.WaitCursor;

            MainForm _mailForm = new MainForm();
            _mailForm.Reset();

            if (_mailForm.Login(email, password))
            {
                Cursor.Current = Cursors.Default;
                Hide();

                _mailForm.ShowDialog();
                _mailForm.Dispose();
                _mailForm = null;
            }
            else
            {
                Cursor.Current = Cursors.Default;

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