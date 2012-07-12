#region

using System;
using System.Security.Permissions;
using System.Web;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Configuration;
using Waveface.Localization;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
#endregion

namespace Waveface
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class AccountInfoForm : Form
	{
		[DllImport("user32.dll")]
		static extern bool ShowCaret(IntPtr hWnd);

		[DllImport("user32.dll")]
		static extern bool HideCaret(IntPtr hWnd);

		#region Const
		private const string CLIENT_API_KEY = @"a23f9491-ba70-5075-b625-b8fb5d9ecd90";

		private const string FB_LOGIN_GUID = @"6CF7FA1E-80F7-48A3-922F-F3B2841C7A0D";
		private const string CALLBACK_MATCH_PATTERN_FORMAT = @"(/" + FB_LOGIN_GUID + "/{0}?.*)";

		private const string CALLBACK_URL_PATH = @"/client/callback";

		private const string WEB_BASE_URL = @"https://waveface.com";
		private const string STAGING_BASE_URL = @"http://staging.waveface.com";
		private const string DEV_WEB_BASE_PAGE_URL = @"https://devweb.waveface.com";
		private const string FB_TURN_ON_URL_PATH = @"/sns/facebook/connect";
		#endregion

		#region Var
		private string _callbackUrl;
		private string _baseUrl;
		private string _fbTurnOnUrl;
		private WService _service;
		#endregion

		#region Private Property
		public Action FBImportOK { get; set; }
		public Action FBImportCancel { get; set; }

		private string m_BaseUrl
		{
			get
			{
				return _baseUrl ??
					   (_baseUrl = WService.CloudBaseURL.Contains("develop.waveface.com") ? DEV_WEB_BASE_PAGE_URL :
					   (WService.CloudBaseURL.Contains("staging.waveface.com") ? STAGING_BASE_URL : WEB_BASE_URL));
			}
		}

		private string m_CallbackUrl
		{
			get { return _callbackUrl ?? (_callbackUrl = Path.Combine(m_BaseUrl, CALLBACK_URL_PATH)); }
		}

		private string m_FBTurnOnUrl
		{
			get
			{
				return _fbTurnOnUrl ??
					   (_fbTurnOnUrl = m_BaseUrl + FB_TURN_ON_URL_PATH + string.Format("?l={0}", Thread.CurrentThread.CurrentCulture));
			}
		}

		private WService m_Service
		{
			get
			{
 				return _service ?? (_service = new WService());
			}
		}

		private string m_SessionToken { get; set; }

		private string m_UserID { get; set; }
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="AccountInfo"/> class.
		/// </summary>
		public AccountInfoForm()
		{
			InitializeComponent();
			m_SessionToken = Main.Current.RT.Login.session_token;
			m_UserID = Main.Current.RT.Login.user.user_id;

			//HideCaret(tbxName.Handle);
			Reset();
		}
		#endregion


		#region Private Method
		private void Reset()
		{
			lblEmail.Text = string.Empty;
			lblIsFacebookImportEnabled.Text = string.Empty;
			lblSince.Text = string.Empty;
			lblUploadedPhotoCount.Text = string.Empty;
			tbxName.Text = string.Empty;
			dataGridView1.Rows.Clear();
		}

		private void ConnectWithFB()
		{
			Hide();
			string fbLoginUrl = string.Format("{0}/{1}/FBConnect", m_CallbackUrl, FB_LOGIN_GUID);
			var postData = new FBPostData
			{
				device_id = StationRegHelper.GetValue("stationId", string.Empty).ToString(),
				device_name = Environment.MachineName,
				device = "windows",
				api_key = CLIENT_API_KEY,
				xurl =
					string.Format(
						"{0}?api_ret_code=%(api_ret_code)d&api_ret_message=%(api_ret_message)s",
						fbLoginUrl),
				locale = Thread.CurrentThread.CurrentCulture.ToString(),
				session_token = m_SessionToken
			};

			var browser = new WebBrowser
			{
				WebBrowserShortcutsEnabled = false,
				IsWebBrowserContextMenuEnabled = false,
				Dock = DockStyle.Fill
			};

			var dialog = new Form
			{
				Width = 750,
				Height = 600,
				Text = Text,
				StartPosition = FormStartPosition.CenterParent,
				Icon = Icon
			};
			dialog.Controls.Add(browser);

			browser.Navigated += (s, ex) =>
			{
				Uri url = browser.Url;
				if (Regex.IsMatch(url.AbsoluteUri, string.Format(CALLBACK_MATCH_PATTERN_FORMAT, "FBConnect"),
								  RegexOptions.IgnoreCase))
				{
					dialog.DialogResult = DialogResult.OK;
				}
			};

			browser.Navigate(m_FBTurnOnUrl,
							 string.Empty,
							 Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(postData, Formatting.Indented)),
							 "Content-Type: application/json");

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				var url = browser.Url;
				var parameters = HttpUtility.ParseQueryString(url.Query);
				var apiRetCode = parameters["api_ret_code"];

				if (!string.IsNullOrEmpty(apiRetCode) && int.Parse(apiRetCode) != 0)
				{
					if (apiRetCode == "4098")
					{
						MessageBox.Show(Properties.Resources.FB_CONNECT_FAILED);
					}

					if (!IsDisposed)
						Show();
					return;
				}
			}

			if (!IsDisposed)
				Show();
		}
		#endregion


		#region Public Method
		public void Update(Boolean checkAccessTokenExpired = false)
		{
			try
			{
				var response = m_Service.users_get(m_SessionToken, m_UserID);

				var user = response.user;

				lblEmail.Text = user.email;

				lblSince.Text = DateTimeHelp.ConvertUnixTimestampToDateTime(user.since).ToString();

				var facebook = (from item1 in response.sns
								from item2 in user.sns
								where item1.type == "facebook" && item2.type == "facebook"
								select new
								{
									Enabled = item1.enabled,
									SnsID = item2.snsid,
									Status = item2.status
								}).FirstOrDefault();

				var accessTokenExpired = facebook == null ? false : facebook.Status.Contains("disconnected");

				if (checkAccessTokenExpired && accessTokenExpired)
				{
					var result = MessageBox.Show(Properties.Resources.RECONNECT_MESSAGE, Properties.Resources.FB_TOKEN_EXPIRED, MessageBoxButtons.YesNo);
					if (result == System.Windows.Forms.DialogResult.Yes)
					{
						ConnectWithFB();
					}
					else
					{
						m_Service.SNSDisconnect(m_SessionToken, "facebook");
					}
				}

				if (facebook != null)
				{
					lblIsFacebookImportEnabled.Text = string.Format("{0} ({1})", (facebook.Enabled) ? Properties.Resources.TURNED_ON : Properties.Resources.TURNED_OFF, facebook.SnsID);

					btnFacebookImport.Text = (facebook.Enabled) ? Properties.Resources.TURN_OFF : Properties.Resources.TURN_ON;
				}
				else
				{
					lblIsFacebookImportEnabled.Text = Properties.Resources.TURNED_OFF;

					btnFacebookImport.Text = Properties.Resources.TURN_ON;
				}

				lblUploadedPhotoCount.Text = response.storages.waveface.usage.image_objects.ToString();
				tbxName.Text = user.nickname;
				//lblNewsletterStatus.Text = user.subscribed.ToString();

				//btnNewsletter.Text = user.subscribed ? "UnSubscribed" : "Subscribed";

				checkBox1.Checked = user.subscribed;

				dataGridView1.Rows.Clear();

				foreach (var device in user.devices)
				{
					dataGridView1.Rows.Add(new object[] { device.device_name, device.device_type, DateTimeHelp.ISO8601ToDateTime(device.last_visit).ToString() });
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(Properties.Resources.UNEXPECTED_EXCEPTION);
				this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			}
		}
		#endregion

		private void btnLogout_Click(object sender, EventArgs e)
		{
			Environment.Exit(-2);
		}

		public class FBPostData
		{
			public string device_id { get; set; }

			public string device_name { get; set; }

			public string device { get; set; }

			public string api_key { get; set; }

			public string xurl { get; set; }

			public string locale { get; set; }

			public string session_token { get; set; }
		}

		//private void btnNewsletter_Click(object sender, EventArgs e)
		//{
		//    m_Service.users_update(m_SessionToken, m_UserID, (btnNewsletter.Text == "Subscribed"));
		//    Update();
		//}

		private void btnFacebookImport_Click(object sender, EventArgs e)
		{
			if (btnFacebookImport.Text == Properties.Resources.TURN_OFF)
			{
				FBImportOK = () => 
				{
					m_Service.SNSDisconnect(m_SessionToken, "facebook");
				};

				FBImportCancel = null;

				lblIsFacebookImportEnabled.Text = Properties.Resources.TURNED_OFF;

				btnFacebookImport.Text = Properties.Resources.TURN_ON;
			}
			else
			{
				ConnectWithFB();

				FBImportOK = null;

				FBImportCancel = () => 
				{
					m_Service.SNSDisconnect(m_SessionToken, "facebook");
				};

				Update();
			}
		}

		private void rbtnSubscribed_CheckedChanged(object sender, EventArgs e)
		{
			//if (!(sender as RadioButton).Checked)
			//    return;

			//m_Service.users_update(m_SessionToken, m_UserID, (sender == rbtnSubscribed));
			//Update();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			using (var dialog = new NameSettingDialog())
			{
				dialog.StartPosition = FormStartPosition.CenterParent;
				dialog.UserName = tbxName.Text;
				if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					tbxName.Text = dialog.UserName;
					m_Service.users_update(m_SessionToken, m_UserID, tbxName.Text, null);
					Update();
				}
			}

			//tbxName.ReadOnly = !tbxName.ReadOnly;
			//if (tbxName.ReadOnly)
			//{
			//    HideCaret(tbxName.Handle);
			//    button2.Focus();
			//}
			//else
			//{
			//    ShowCaret(tbxName.Handle);
			//    tbxName.SelectionStart = tbxName.Text.Length;
			//    tbxName.Focus();
			//}
		}

		private void tbxName_KeyDown(object sender, KeyEventArgs e)
		{
			//if (e.KeyData == Keys.Enter)
			//{
			//    m_Service.users_update(m_SessionToken, m_UserID, tbxName.Text, null);
			//    Update();

			//    button2.PerformClick();
			//}
		}

		private void AccountInfoForm_Load(object sender, EventArgs e)
		{
			Update(true);
		}

		private void AccountInfoForm_Shown(object sender, EventArgs e)
		{
			//Show();
			//Application.DoEvents();
			//Update(true);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (ErrorProviderExtension.HasError(errorProvider1))
			{
				var errorMessage = ErrorProviderExtension.GetErrorMsgs(errorProvider1).FirstOrDefault();

				if (!string.IsNullOrEmpty(errorMessage))
					MessageBox.Show(errorMessage);

				var errorControl = ErrorProviderExtension.GetErrorControls(errorProvider1).FirstOrDefault();

				if (errorControl != null)
					errorControl.Focus();

				return;
			}

			try
			{
				m_Service.users_update(m_SessionToken, m_UserID, checkBox1.Checked);
				m_Service.users_update(m_SessionToken, m_UserID, tbxName.Text, null);

				if (FBImportOK != null)
					FBImportOK();

				this.DialogResult = System.Windows.Forms.DialogResult.OK;
			}
			catch (Exception)
			{
				MessageBox.Show(Properties.Resources.UNEXPECTED_EXCEPTION);
			}
		}

		private void tbxName_Validated(object sender, EventArgs e)
		{
			errorProvider1.SetError(tbxName, (tbxName.Text.Length == 0) ? string.Format(Properties.Resources.FIELD_MUST_HAVE_DATA_PATTERN, Properties.Resources.NAME) : string.Empty);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				if (FBImportCancel != null)
					FBImportCancel();

				this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			}
			catch (Exception)
			{
				MessageBox.Show(Properties.Resources.UNEXPECTED_EXCEPTION);
			}
		}
    }
}