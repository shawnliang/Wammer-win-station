#region

using System;
using System.Security.Permissions;
using System.Web;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Configuration;
using Waveface.Localization;

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

            string _device_id = HttpUtility.UrlEncode(StationRegHelper.GetValue("stationId", string.Empty).ToString());
            string _device_name = HttpUtility.UrlEncode(Environment.MachineName);

            string _url = WService.WebURL + "/client/user/profile?";

            _url += "session_token=" + HttpUtility.UrlEncode(Main.Current.RT.Login.session_token) + "&" +
                    "device_id=" + HttpUtility.UrlEncode(_device_id) + "&" +
                    "device_name=" + HttpUtility.UrlEncode(_device_name) + "&" +
                    "device=windows" + "&" +
                    "l=" + CultureManager.ApplicationUICulture.Name;

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

		private void logoutButton1_Click(object sender, EventArgs e)
		{
			Environment.Exit(-2);
		}
    }
}