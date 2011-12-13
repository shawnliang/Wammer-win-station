using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using Waveface.Component;
using Waveface.Configuration;
using Waveface.Localization;

namespace Waveface
{
    public class LoginForm : Form
    {
        internal CheckBox cbRemember;
        internal Label lblUserName;
        internal TextBox txtPassword;
        internal TextBox txtUserName;
        internal Label lblPassword;
        private IContainer components;
        private Label lText;
        private XPButton btnCancel;
        private XPButton btnOK;
        private GroupBox groupBox1;
        private CultureManager cultureManager;

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
            m_formSettings.UseSize = false;
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
            this.lText = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.btnCancel = new Waveface.Component.XPButton();
            this.btnOK = new Waveface.Component.XPButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbRemember
            // 
            resources.ApplyResources(this.cbRemember, "cbRemember");
            this.cbRemember.Name = "cbRemember";
            // 
            // lText
            // 
            resources.ApplyResources(this.lText, "lText");
            this.lText.Name = "lText";
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
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.lblUserName);
            this.groupBox1.Controls.Add(this.lblPassword);
            this.groupBox1.Controls.Add(this.cbRemember);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            this.cultureManager.UICultureChanged += new Waveface.Localization.CultureManager.CultureChangedHandler(this.cultureManager_UICultureChanged);
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
            this.Controls.Add(this.lText);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.DoubleClick += new System.EventHandler(this.LoginForm_DoubleClick);
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

            Main _main = new Main();
            //_main.Reset();

			Application.DoEvents();

			try
			{
				_main.stationLogin(email, password);
				_doLogin(_main, email, password);
			}
			catch (Waveface.API.V2.ServiceUnavailableException ex)
			{
				MessageBox.Show(ex.Message, "Waveface");
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Waveface");
			}
        }

		private void _doLogin(Main _main, string email, string password)
		{
			if (_main.Login(email, password))
			{
				Application.DoEvents();

				Cursor.Current = Cursors.Default;
				Hide();

				Application.DoEvents();

				_main.ShowDialog();
				_main.Dispose();
				_main = null;
			}
			else
			{
				Cursor.Current = Cursors.Default;

				MessageBox.Show(I18n.L.T("LoginForm.LogInError"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			Show();
		}
		
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult = DialogResult.None;

                if ((txtUserName.Text.Trim() != "") && (txtPassword.Text.Trim() != ""))
                {
                    m_formSettings.Save();

                    doLogin(txtUserName.Text.Trim(), txtPassword.Text.Trim());
                }
                else
                {
                    MessageBox.Show(I18n.L.T("LoginForm.FillAllFields"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception _ex)
            {
                MessageBox.Show(_ex.Message, "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void LoginForm_DoubleClick(object sender, EventArgs e)
        {
            if (CultureManager.ApplicationUICulture.Name == "en-US")
            {
                CultureManager.ApplicationUICulture = new CultureInfo("zh-TW");
                return;
            }

            if (CultureManager.ApplicationUICulture.Name == "zh-TW")
            {
                CultureManager.ApplicationUICulture = new CultureInfo("en-US");
                return;
            }            
        }

        private void cultureManager_UICultureChanged(CultureInfo newCulture)
        {
            I18n.L.CurrentCulture = newCulture;
        }
    }
}
