#region

using System;
using System.ComponentModel;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using NLog;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.Configuration;
using Waveface.Localization;

#endregion

namespace Waveface
{
    public class LoginForm : Form
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private CheckBox cbRemember;
        private Label lblUserName;
        private TextBox txtPassword;
        private TextBox txtUserName;
        private Label lblPassword;
        private IContainer components;
        private Label labelTitle;
        private XPButton btnCancel;
        internal XPButton btnOK;
        private GroupBox groupBox1;
        private CultureManager cultureManager;

        private FormSettings m_formSettings;
        private string m_savePassword = "";		

        #region Properties

        public string User
        {
            get { return txtUserName.Text.Trim(); }
        }

        public string Password
        {
            get { return txtPassword.Text; }
        }

        #endregion

        #region Setting

        public bool RememberPassword
        {
            get { return cbRemember.Checked; }
            set
            {
                cbRemember.Checked = value;

                if (cbRemember.Checked)
                    txtPassword.Text = m_savePassword;
                else
                    txtPassword.Text = "";
            }
        }

        #endregion

        public LoginForm()
        {
            InitializeComponent();  

            m_formSettings = new FormSettings(this);
            m_formSettings.UseSize = false;
            m_formSettings.SaveOnClose = true;
        }

        public LoginForm(string email, string password)
        {
            InitializeComponent();

            m_savePassword = password;

            txtUserName.Text = email;
            txtPassword.Text = password;

            m_formSettings = new FormSettings(this);
            m_formSettings.UseSize = false;
            m_formSettings.SaveOnClose = true;
            Opacity = 0;
            Visible = false;
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
            this.labelTitle = new System.Windows.Forms.Label();
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
            // labelTitle
            // 
            resources.ApplyResources(this.labelTitle, "labelTitle");
            this.labelTitle.Name = "labelTitle";
            // 
            // lblUserName
            // 
            resources.ApplyResources(this.lblUserName, "lblUserName");
            this.lblUserName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.DoubleClick += new System.EventHandler(this.lblUserName_DoubleClick);
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
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoginForm_FormClosing);
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.Shown += new System.EventHandler(this.LoginForm_Shown);
            this.DoubleClick += new System.EventHandler(this.LoginForm_DoubleClick);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (txtUserName.Text == string.Empty)
            {
                txtUserName.Focus();
            }
            else if (txtPassword.Text == string.Empty)
            {
                txtPassword.Focus();
            }
            else
            {
                btnOK.Focus();
            }
        }

        private void doLogin(string email, string password)
        {
            Cursor = Cursors.WaitCursor;

            Program.ShowCrashReporter = true;

            try
            {
                QuitOption quit = _doLogin(email, password);
                if (quit == QuitOption.QuitProgram)
                {
                    Close();
                }
                else if (quit == QuitOption.Logout)
                {
                    Environment.Exit(-2);
                }
                else if (quit == QuitOption.Unlink)
                {
                    Environment.Exit(-3);
                }
                else
                {
                    Environment.Exit(-1);
                }
            }
            catch (StationServiceDownException _e)
            {
                NLogUtility.Exception(s_logger, _e, "doLogin");

                MessageBox.Show(I18n.L.T("StationServiceDown"), "Stream");
                Environment.Exit(-1);
            }
            catch (ServiceUnavailableException _e)
            {
                NLogUtility.Exception(s_logger, _e, "doLogin");

                // user should re-register station if receive service unavailable exception
                // so we close the login page here
                MessageBox.Show(I18n.L.T("RegisteredRequired", txtUserName.Text), "Stream");
                Environment.Exit(-1);
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "doLogin");

                MessageBox.Show(I18n.L.T("LoginForm.LogInError") + " : " + _e.Message, "Stream");
                Environment.Exit(-1);
            }
        }

        private QuitOption _doLogin(string email, string password)
        {
            QuitOption _quit;
            string _errorMessage;

            Main _main = new Main();

            if (_main.Login(email, password, out _errorMessage))
            {
                Cursor = Cursors.Default;

                Hide();

                _main.ShowDialog(this);
                _quit = _main.QuitOption;

				if (_quit == QuitOption.Logout)
				{
					_main.RT.REST.Auth_Logout(_main.RT.Login.session_token);
				}
                _main.Dispose();
                _main = null;
            }
            else
            {
                Cursor = Cursors.Default;

                MessageBox.Show((_errorMessage != string.Empty) ? _errorMessage : I18n.L.T("LoginForm.LogInError"), "Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _quit = QuitOption.Logout;
            }

            return _quit;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show(I18n.L.T("NetworkDisconnected"), "Stream", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                return;
            }

            DialogResult = DialogResult.None;

            if ((txtUserName.Text.Trim() != "") && (txtPassword.Text != ""))
            {
                m_formSettings.Save();

                doLogin(txtUserName.Text.Trim(), txtPassword.Text);
            }
            else
            {
                MessageBox.Show(I18n.L.T("LoginForm.FillAllFields"), "Stream", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cultureManager_UICultureChanged(CultureInfo newCulture)
        {
            I18n.L.CurrentCulture = newCulture;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter && !btnOK.Focused)
            {
                btnOK_Click(null, null);
            }

            return base.ProcessDialogKey(keyData);
        }

        #region Debug

        private void lblUserName_DoubleClick(object sender, EventArgs e)
        {
            txtUserName.ReadOnly = !txtUserName.ReadOnly;
        }

        private void LoginForm_DoubleClick(object sender, EventArgs e)
        {
            if (GCONST.DEBUG)
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
        }

        #endregion

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            if ((txtUserName.Text != string.Empty) && (txtPassword.Text != string.Empty))
            {
                Visible = false;
                doLogin(txtUserName.Text, txtPassword.Text);
                return;
            }
        }
    }
}