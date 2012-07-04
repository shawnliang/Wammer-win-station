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
#endregion

namespace Waveface
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
		#endregion

		#region Var
		private string _callbackUrl;
		private string _baseUrl;
		private string _fbTurnOnUrl;
		private WService _service;
		#endregion

		#region Private Property
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
			Update();
		}
		#endregion


		#region Public Method
		public void Update()
		{
			var response = m_Service.users_get(m_SessionToken , m_UserID);

			var user = response.user;
			lblSince.Text = DateTimeHelp.ConvertUnixTimestampToDateTime(user.since).ToString();

			var facebook = (from item1 in response.sns
							from item2 in user.sns
							where item1.type == "facebook" && item2.type == "facebook"
							select new 
							{
								Enabled = item1.enabled,
								SnsID = item2.snsid
							}).FirstOrDefault();

			if (facebook != null)
			{
				lblIsFacebookImportEnabled.Text =string.Format("{0} ({1})", (facebook.Enabled) ? "Turned on" : "Turned off", facebook.SnsID);

				label9.Text = (facebook.Enabled) ? "Turn off" : "Turn on";
			}
			else
			{
				lblIsFacebookImportEnabled.Text = "Turned off";

				label9.Text = "Turn on";
			}

			label2.Text = response.storages.waveface.usage.image_objects.ToString();
			label5.Text = user.nickname;
			label7.Text = user.subscribed.ToString();

			label10.Text = user.subscribed ? "UnSubscribed" : "Subscribed";
		}
		#endregion

		private void logoutButton1_Click(object sender, EventArgs e)
		{
			Environment.Exit(-2);
		}

		private void AccountInfoForm_Load(object sender, EventArgs e)
		{

		}

		private void label9_Click(object sender, EventArgs e)
		{
			if (label9.Text == "Turn off")
			{
				m_Service.SNSDisconnect(m_SessionToken, "facebook");
			}
			else 
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
					locale = Thread.CurrentThread.CurrentCulture.ToString()
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

				dialog.ShowDialog();
				//if (dialog.ShowDialog() == DialogResult.OK)
				//{
				//    string url = browser.Url.Query;
				//    NameValueCollection parameters = HttpUtility.ParseQueryString(url);
				//    string apiRetCode = parameters["api_ret_code"];

				//    if (!string.IsNullOrEmpty(apiRetCode) && int.Parse(apiRetCode) != 0)
				//    {

				//    }
				//}
				if (!IsDisposed)
					Show();
			}

			Update();
		}



		public class FBPostData
		{
			public string device_id { get; set; }

			public string device_name { get; set; }

			public string device { get; set; }

			public string api_key { get; set; }

			public string xurl { get; set; }

			public string locale { get; set; }
		}

		private void label10_Click(object sender, EventArgs e)
		{
			m_Service.users_update(m_SessionToken, m_UserID, (label10.Text == "Subscribed"));
			Update();
		}
    }
}