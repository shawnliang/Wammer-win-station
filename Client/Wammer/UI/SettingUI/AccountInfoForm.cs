#region

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Web;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Configuration;

#endregion

namespace Waveface
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class AccountInfoForm : Form
    {
        private FormSettings m_formSettings;

        public AccountInfoForm()
        {
            InitializeComponent();

            m_formSettings = new FormSettings(this);
            m_formSettings.UseLocation = true;
            m_formSettings.UseSize = true;
            m_formSettings.UseWindowState = true;
            m_formSettings.AllowMinimized = true;
            m_formSettings.SaveOnClose = true;
        }

        private void UserAccount_Load(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            string _url = WService.WebURL + "/client/user/profile?";
            _url += "session_token=" + HttpUtility.UrlEncode(Main.Current.RT.Login.session_token) + "&device=windows";

            webBrowser.Navigate(_url);
        }

        private void AccountInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_formSettings.Save();
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Cursor = Cursors.Default;
        }
    }
}