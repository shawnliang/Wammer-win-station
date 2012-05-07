using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using StationSystemTray.Properties;
using System.Threading;

namespace StationSystemTray
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
    public partial class SignUpDialog : Form
    {
        #region Const
        const string WEB_API_VERSION = "{\"version\": \"1.0\"}";
        const string WEB_SIGNUP_PAGE_URL_PATTERN = @"https://waveface.com/signup?device=windows&l={0}";
        const string DEV_WEB_SIGNUP_PAGE_URL_PATTERN = @"http://develop.waveface.com:4343/signup?device=windows&l={0}";
        #endregion


        #region Var
        private string _EMail;
        private string _Password;
        private string _SignUpPage;
        #endregion


        #region Private Property
        /// <summary>
        /// Gets the m_ sign up page.
        /// </summary>
        /// <value>The m_ sign up page.</value>
        private string m_SignUpPage
        {
            get
            {
                if (_SignUpPage == null)
                {
                    string cultureName = Thread.CurrentThread.CurrentCulture.Name;
                    if (Wammer.Cloud.CloudServer.BaseUrl.Contains("develop.waveface.com"))
                    {
                        _SignUpPage = string.Format(DEV_WEB_SIGNUP_PAGE_URL_PATTERN, cultureName);
                    }
                    else
                    {
                        _SignUpPage = string.Format(WEB_SIGNUP_PAGE_URL_PATTERN, cultureName);
                    }
                }
                return _SignUpPage;
            }
        }
        #endregion


        #region Public Property
        /// <summary>
        /// Gets or sets the E mail.
        /// </summary>
        /// <value>The E mail.</value>
        public string EMail
        {
            get { return _EMail; }
            set { _EMail = value; }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }
        #endregion


        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SignUpDialog"/> class.
        /// </summary>
        public SignUpDialog()
        {
            InitializeComponent();
			this.Icon = Resources.Icon;
            webBrowser1.ObjectForScripting = this;
        } 
        #endregion

        #region Private Method
        /// <summary>
        /// Gets the sign up data.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        private string GetSignUpData(string functionName, string attributeName)
        {
            string data = webBrowser1.Document.InvokeScript(functionName, new object[] { attributeName, WEB_API_VERSION }).ToString();

            //not empty => decode
            if (data.Length > 0)
                data = System.Web.HttpUtility.UrlDecode(data);

            return data;
        }
        #endregion


        #region Public Method
        /// <summary>
        /// Signs up completed triggered by webpage.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        public void SignUpCompleted(string functionName)
        {
            string email = GetSignUpData(functionName, "email");
            string password = GetSignUpData(functionName, "passwd");

            Boolean isSignUpCompleted = !(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password));
            if (isSignUpCompleted)
            {
                EMail = email;
                Password = password;
            }
            DialogResult = isSignUpCompleted ? System.Windows.Forms.DialogResult.OK : System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion

        #region Event Process
        /// <summary>
        /// Handles the Load event of the SignUpDialog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SignUpDialog_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate(m_SignUpPage);
        } 
        #endregion
    }
}
