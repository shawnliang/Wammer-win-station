using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	public partial class AccountInfoForm : Form
	{
		#region Const
		private const string CLIENT_API_KEY = @"a23f9491-ba70-5075-b625-b8fb5d9ecd90";

		private const string FB_LOGIN_GUID = @"6CF7FA1E-80F7-48A3-922F-F3B2841C7A0D";
		private const string CALLBACK_MATCH_PATTERN_FORMAT = @"(/" + FB_LOGIN_GUID + "/{0}?.*)";

		private const string CALLBACK_URL_PATH = @"/client/callback";

		private const string WEB_BASE_URL = @"https://waveface.com";
		private const string STAGING_BASE_URL = @"http://staging.waveface.com";
		private const string DEV_WEB_BASE_PAGE_URL = @"https://devweb.waveface.com";
		private const string FB_TURN_ON_URL_PATH = @"/sns/facebook/connect";
		public const string DEF_BASE_URL = "https://develop.waveface.com/v2/";
		#endregion


		#region Static Var
		private static AccountInfoForm _instance;
		#endregion


		#region Var
		private string _callbackUrl;
		private string _baseUrl;
		private string _fbTurnOnUrl;
		#endregion


		#region Public Static Property
		public static AccountInfoForm Instance
		{
			get
			{
				return (_instance == null || _instance.IsDisposed) ? (_instance = new AccountInfoForm()) : _instance;
			}
		}
		#endregion


		#region Private Property
		public Action FBImportOK { get; set; }
		public Action FBImportCancel { get; set; }

		private static string m_CloudBaseURL
		{
			get { return (string)StationRegistry.GetValue("cloudBaseURL", DEF_BASE_URL); }
		}

		private string m_BaseUrl
		{
			get
			{
				return _baseUrl ??
					   (_baseUrl = m_CloudBaseURL.Contains("develop.waveface.com") ? DEV_WEB_BASE_PAGE_URL :
					   (m_CloudBaseURL.Contains("staging.waveface.com") ? STAGING_BASE_URL : WEB_BASE_URL));
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

		private string m_SessionToken { get; set; }

		private string m_UserID { get; set; }

		private string m_OriginalName { get; set; }

		private Boolean m_OriginalSubscribed { get; set; }
		#endregion




		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="AccountInfo"/> class.
		/// </summary>
		private AccountInfoForm()
		{
			InitializeComponent();

			if (!StreamClient.Instance.IsLogined)
				return;

			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", StreamClient.Instance.LoginedUser.SessionToken));

			if (loginedSession == null)
				return;

			m_SessionToken = loginedSession.session_token;
			m_UserID = loginedSession.user.user_id;

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
			lblFBImportTip.Text = string.Empty;
			dataGridView1.Rows.Clear();
		}

		private void ConnectWithFB()
		{
			Hide();
			string fbLoginUrl = string.Format("{0}/{1}/FBConnect", m_CallbackUrl, FB_LOGIN_GUID);
			var postData = new FBPostData
			{
				device_id = StationRegistry.GetValue("stationId", string.Empty).ToString(),
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
		public void Update(Boolean isDialogInited = false)
		{
			try
			{
				var userInfo = UserInfo.Instance;

				if (!isDialogInited)
					userInfo.Update();

				lblEmail.Text = userInfo.Email;

				lblSince.Text = userInfo.Since.ToString();

				lblUploadedPhotoCount.Text = userInfo.UploadedPhotoCount.ToString();
				tbxName.Text = userInfo.NickName;

				checkBox1.Checked = userInfo.Subscribed;

				dataGridView1.Rows.Clear();

				foreach (var device in userInfo.Devices)
				{
					dataGridView1.Rows.Add(new object[] { device.device_name, device.device_type, TimeHelper.ISO8601ToDateTime(device.last_visit).ToString() });
				}

				var accessTokenExpired = false;

				if ((userInfo.SNS1 != null && userInfo.SNS2 != null))
				{
					var facebook = (from item1 in userInfo.SNS1
									where item1 != null && item1.type == "facebook"
									from item2 in userInfo.SNS2
									where item2 != null && item2.type == "facebook"
									select new
									{
										Enabled = item1.enabled,
										SnsID = item2.snsid,
										Status = item2.status,
										Status2 = item1.status,
										LastSync = item1.lastSync
									}).FirstOrDefault();

					accessTokenExpired = (facebook == null) ? false : facebook.Status.Contains("disconnected");

					if (facebook != null)
					{
						lblIsFacebookImportEnabled.Text = string.Format("{0} ({1})", (facebook.Enabled) ? Properties.Resources.TURNED_ON : Properties.Resources.TURNED_OFF, facebook.SnsID);

						btnFacebookImport.Text = (facebook.Enabled) ? Properties.Resources.TURN_OFF : Properties.Resources.TURN_ON;

						lblFBImportTip.Text = (facebook.Enabled) ?
							((string.Equals(facebook.Status2, "progress", StringComparison.CurrentCultureIgnoreCase)) ? Properties.Resources.FB_IMPRORT_PROGRESSING : string.Format(Properties.Resources.FB_IMPORT_CLAST_SYNC_PATTERN, TimeHelper.ISO8601ToDateTime(facebook.LastSync).ToString())) :
							string.Empty;

						if (isDialogInited && accessTokenExpired)
						{
							var result = MessageBox.Show(Properties.Resources.RECONNECT_MESSAGE, Properties.Resources.FB_TOKEN_EXPIRED, MessageBoxButtons.YesNo);
							if (result == System.Windows.Forms.DialogResult.Yes)
							{
								ConnectWithFB();

							}
							else
							{
								StationAPI.SNSDisconnect(m_SessionToken, "facebook");
							}
							Update();
							return;
						}
						return;
					}
				}


				lblIsFacebookImportEnabled.Text = Properties.Resources.TURNED_OFF;

				btnFacebookImport.Text = Properties.Resources.TURN_ON;

				lblFBImportTip.Text = string.Empty;

				if (isDialogInited)
					userInfo.Update();
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
			StreamClient.Instance.Logout();
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

		private void btnFacebookImport_Click(object sender, EventArgs e)
		{
			if (!NetworkChecker.Instance.IsNetworkAvailable)
			{
				MessageBox.Show(Properties.Resources.NETWORK_UNAVAILABLE);
				return;
			}

			if (btnFacebookImport.Text == Properties.Resources.TURN_OFF)
			{
				FBImportOK = () =>
				{
					StationAPI.SNSDisconnect(m_SessionToken, "facebook");
				};

				FBImportCancel = null;

				lblIsFacebookImportEnabled.Text = Properties.Resources.TURNED_OFF;

				btnFacebookImport.Text = Properties.Resources.TURN_ON;

				lblFBImportTip.Text = string.Empty;
			}
			else
			{
				ConnectWithFB();

				FBImportOK = null;

				FBImportCancel = () =>
				{
					StationAPI.SNSDisconnect(m_SessionToken, "facebook");
				};

				Update();
			}
		}

		private void AccountInfoForm_Load(object sender, EventArgs e)
		{
			Update(true);

			m_OriginalName = tbxName.Text;
			m_OriginalSubscribed = checkBox1.Checked;
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

			if (!NetworkChecker.Instance.IsNetworkAvailable)
			{
				MessageBox.Show(Properties.Resources.NETWORK_UNAVAILABLE);
				return;
			}

			try
			{
				var needUpdate = false;
				if (m_OriginalSubscribed != checkBox1.Checked)
				{
					needUpdate = true;
					StationAPI.UpdateUser(m_SessionToken, m_UserID, checkBox1.Checked);
				}

				if (m_OriginalName != tbxName.Text)
				{
					needUpdate = true;
					StationAPI.UpdateUser(m_SessionToken, m_UserID, tbxName.Text, null);
				}

				if (FBImportOK != null)
				{
					needUpdate = true;
					FBImportOK();
				}

				if (needUpdate)
					Update();

				this.DialogResult = System.Windows.Forms.DialogResult.OK;
			}
			catch (WebException ex)
			{
				MessageBox.Show(Properties.Resources.NETWORK_EXCEPTION);
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
				{

					if (!NetworkChecker.Instance.IsNetworkAvailable)
					{
						MessageBox.Show(Properties.Resources.NETWORK_UNAVAILABLE);
						return;
					}

					FBImportCancel();
					Update();
				}

				this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			}
			catch (WebException ex)
			{
				MessageBox.Show(Properties.Resources.NETWORK_EXCEPTION);
			}
			catch (Exception)
			{
				MessageBox.Show(Properties.Resources.UNEXPECTED_EXCEPTION);
			}
		}

		private void lblFBImportTip_TextChanged(object sender, EventArgs e)
		{
			if (lblFBImportTip.Text.Length == 0)
			{
				lblFBImportTip.Hide();
				btnFacebookImport.Top = lblFBImportTip.Top;
			}
			else
			{
				lblFBImportTip.Show();
				btnFacebookImport.Top = lblFBImportTip.Top + lblFBImportTip.Height + 5;
			}
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void tabPage2_Click(object sender, EventArgs e)
		{

		}

		private void label8_Click(object sender, EventArgs e)
		{

		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void label4_Click(object sender, EventArgs e)
		{

		}

		private void AccountInfoForm_FormClosed(object sender, FormClosedEventArgs e)
		{

		}

		private void AccountInfoForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.DialogResult != DialogResult.OK)
				button1.PerformClick();
		}
	}
}
