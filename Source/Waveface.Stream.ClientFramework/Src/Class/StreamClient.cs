using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Waveface.Stream.Model;
using System.Net;
using Waveface.Stream.Core;

namespace Waveface.Stream.ClientFramework
{
	/// <summary>
	/// 
	/// </summary>
	public class StreamClient
	{
		#region Private Const
		private const string APP_NAME = "Stream";

		private const string STREAM_RELATIVED_FOLDER = @"Waveface\Stream\";
		private const string DATA_RELATIVED_FOLDER = STREAM_RELATIVED_FOLDER + @"Data\";
		private const string STREAM_DATX_FILE_NAME = @"Stream.datx";

		private const string RELATIVED_LOGINED_SESSION_XML_FILE = @"LoginedSession.xml";
		#endregion


		#region Static Var
		private static StreamClient _instance;
		#endregion


		#region Var
		private WebClientControlServer _server;
		private String _dataPath;
		private String _streamDatxFile;
		#endregion


		#region Static Public Property
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static StreamClient Instance
		{
			get
			{
				return _instance ?? (_instance = new StreamClient());
			}
		}
		#endregion


		#region Private Property
		/// <summary>
		/// Gets the m_ server.
		/// </summary>
		/// <value>The m_ server.</value>
		private WebClientControlServer m_Server
		{
			get
			{
				return _server ?? (_server = WebClientControlServer.Instance);
			}
		}

		private string m_DataPath
		{
			get
			{
				return _dataPath ?? (_dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DATA_RELATIVED_FOLDER));
			}
		}

		private string m_StreamDatxFile
		{
			get
			{
				return _streamDatxFile ?? (_streamDatxFile = Path.Combine(m_DataPath, STREAM_DATX_FILE_NAME));
			}
		}
		#endregion


		#region Public Property
		/// <summary>
		/// Gets the logined user.
		/// </summary>
		/// <value>
		/// The logined user.
		/// </value>
		public LoginedUser LoginedUser
		{
			get
			{
				return LoginController.Instance.LoginedUser;
			}
		}

		/// <summary>
		/// Gets or sets the is logined.
		/// </summary>
		/// <value>
		/// The is logined.
		/// </value>
		public Boolean IsLogined
		{
			get
			{
				return LoginController.Instance.IsLogined;
			}
		}
		#endregion


		#region Event
		/// <summary>
		/// Occurs when [logining].
		/// </summary>
		public event EventHandler Logining;

		/// <summary>
		/// Occurs when [logined].
		/// </summary>
		public event EventHandler<LoginedEventArgs> Logined;

		/// <summary>
		/// Occurs when [logouting].
		/// </summary>
		public event EventHandler Logouting;

		/// <summary>
		/// Occurs when [logouted].
		/// </summary>
		public event EventHandler Logouted;
		#endregion


		#region Constructor
		/// <summary>
		/// Prevents a default instance of the <see cref="StreamClient" /> class from being created.
		/// </summary>
		private StreamClient()
		{
			Init();
		}
		#endregion



		#region Private Method
		/// <summary>
		/// Inits this instance.
		/// </summary>
		private void Init()
		{
			SynchronizationContextHelper.SetMainSyncContext();

			Waveface.Stream.Core.AutoMapperSetting.IniteMap();

			LoginController.Instance.Logined += StreamClient_Logined;
			LoginController.Instance.Logouted += StreamClient_Logouted;


			if (File.Exists(m_StreamDatxFile) && Datx.IsFileExist(m_StreamDatxFile, RELATIVED_LOGINED_SESSION_XML_FILE))
			{
				var sessionToken = Datx.Read<String>(m_StreamDatxFile, RELATIVED_LOGINED_SESSION_XML_FILE, GetStreamDatxPassword());
				RetryLogin(sessionToken, 5000);
			}

			m_Server.Start();
		}


		private string RetryLogin(string sessionToken, int timeout)
		{
			DateTime until = DateTime.Now.AddMilliseconds(timeout);

			do
			{
				try
				{
					return Login(sessionToken);
				}
				catch (WebException e)
				{
					bool tryAgain = (e.Status == WebExceptionStatus.ConnectFailure && DateTime.Now < until);
					if (!tryAgain)
						throw;
				}

			} while (true);
		}

		private SecureString GetStreamDatxPassword()
		{
			//Waveface Stream 98D3B9C7-A57B-4A01-ADB8-329CD0F7E669

			byte[] buffer = new byte[52];
			buffer[0] = 0x57;
			buffer[1] = 0x61;
			buffer[2] = 0x76;
			buffer[3] = 0x65;
			buffer[4] = 0x66;
			buffer[5] = 0x61;
			buffer[6] = 0x63;
			buffer[7] = 0x65;
			buffer[8] = 0x20;
			buffer[9] = 0x53;
			buffer[10] = 0x74;
			buffer[11] = 0x72;
			buffer[12] = 0x65;
			buffer[13] = 0x61;
			buffer[14] = 0x6d;
			buffer[15] = 0x20;
			buffer[16] = 0x39;
			buffer[17] = 0x38;
			buffer[18] = 0x44;
			buffer[19] = 0x33;
			buffer[20] = 0x42;
			buffer[21] = 0x39;
			buffer[22] = 0x43;
			buffer[23] = 0x37;
			buffer[24] = 0x2d;
			buffer[25] = 0x41;
			buffer[26] = 0x35;
			buffer[27] = 0x37;
			buffer[28] = 0x42;
			buffer[29] = 0x2d;
			buffer[30] = 0x34;
			buffer[31] = 0x41;
			buffer[32] = 0x30;
			buffer[33] = 0x31;
			buffer[34] = 0x2d;
			buffer[35] = 0x41;
			buffer[36] = 0x44;
			buffer[37] = 0x42;
			buffer[38] = 0x38;
			buffer[39] = 0x2d;
			buffer[40] = 0x33;
			buffer[41] = 0x32;
			buffer[42] = 0x39;
			buffer[43] = 0x43;
			buffer[44] = 0x44;
			buffer[45] = 0x30;
			buffer[46] = 0x46;
			buffer[47] = 0x37;
			buffer[48] = 0x45;
			buffer[49] = 0x36;
			buffer[50] = 0x36;
			buffer[51] = 0x39;

			SecureString ret = new SecureString();
			foreach (byte b in buffer)
				ret.AppendChar(Convert.ToChar(b));
			ret.MakeReadOnly();

			return ret;
		}
		#endregion


		#region Protected Method
		/// <summary>
		/// Raises the <see cref="E:Logining" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void OnLogining(EventArgs e)
		{
			this.RaiseEvent(Logining, e);
		}

		/// <summary>
		/// Raises the <see cref="E:Logined" /> event.
		/// </summary>
		/// <param name="e">The <see cref="LoginedEventArgs" /> instance containing the event data.</param>
		protected void OnLogined(LoginedEventArgs e)
		{
			this.RaiseEvent<LoginedEventArgs>(Logined, e);
		}

		/// <summary>
		/// Raises the <see cref="E:Logouting" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void OnLogouting(EventArgs e)
		{
			this.RaiseEvent(Logouting, e);
		}

		/// <summary>
		/// Raises the <see cref="E:Logouted" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void OnLogouted(EventArgs e)
		{
			this.RaiseEvent(Logouted, e);
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Logins the specified email.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="password">The password.</param>
		/// <returns></returns>
		public string Login(string email, string password)
		{
			return LoginController.Instance.Login(email, password);
		}

		/// <summary>
		/// Logins the specified session token.
		/// </summary>
		/// <param name="sessionToken">The session token.</param>
		/// <returns></returns>
		public string Login(string sessionToken)
		{
			return LoginController.Instance.Login(sessionToken);
		}

		/// <summary>
		/// Logins the SNS.
		/// </summary>
		/// <param name="userID">The user ID.</param>
		/// <param name="sessionToken">The session token.</param>
		/// <returns></returns>
		public string LoginSNS(string userID, string sessionToken)
		{
			return LoginController.Instance.LoginSNS(userID, sessionToken);
		}

		public void Logout()
		{
			LoginController.Instance.Logout();
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the Logined event of the StreamClient control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		void StreamClient_Logined(object sender, LoginedEventArgs e)
		{
			OnLogined(e);

			var response = e.Response;

			if (response.Length == 0)
				return;

			Datx.Insert<String>(LoginedUser.SessionToken, m_StreamDatxFile, RELATIVED_LOGINED_SESSION_XML_FILE, GetStreamDatxPassword());
		}

		void StreamClient_Logouted(object sender, EventArgs e)
		{
			Waveface.Stream.ClientFramework.UserInfo.Instance.Clear();

			Datx.RemoveFile(m_StreamDatxFile, RELATIVED_LOGINED_SESSION_XML_FILE);
			OnLogouted(e);
		}

		#endregion
	}
}
