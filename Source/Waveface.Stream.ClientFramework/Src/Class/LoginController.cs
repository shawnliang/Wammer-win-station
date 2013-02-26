using MongoDB.Driver.Builders;
using System;
using Waveface.Stream.Core;
using Waveface.Stream.Model;

namespace Waveface.Stream.ClientFramework
{
	public class LoginController
	{
		#region Private Const
		private const string STATION_ID_KEY = "stationId";
		#endregion

		#region Static Var
		private static LoginController _instance;
		#endregion

		#region Public Static Property
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static LoginController Instance
		{
			get
			{
				return _instance ?? (_instance = new LoginController());
			}
		}
		#endregion


		#region Public Property
		/// <summary>
		/// Gets or sets the logined user.
		/// </summary>
		/// <value>
		/// The logined user.
		/// </value>
		public LoginedUser LoginedUser { get; set; }


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
				return LoginedUser != null;
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
		/// Prevents a default instance of the <see cref="LoginController" /> class from being created.
		/// </summary>
		private LoginController()
		{
			this.Logined += LoginController_Logined;
			this.Logouted += LoginController_Logouted;
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
			if (LoginedUser != null && LoginedUser.EMail.Equals(email))
				return null;

			var response = string.Empty;

			try
			{
				OnLogining(EventArgs.Empty);

				response = StationAPI.Login(
					email,
					password,
					(string)StationRegistry.GetValue(STATION_ID_KEY, string.Empty),
					Environment.MachineName);

				return response;
			}
			finally
			{
				OnLogined(new LoginedEventArgs(response));
			}
		}

		/// <summary>
		/// Logins the specified session token.
		/// </summary>
		/// <param name="sessionToken">The session token.</param>
		/// <returns></returns>
		public string Login(string sessionToken)
		{
			if (LoginedUser != null)
				return null;

			var response = string.Empty;

			try
			{
				OnLogining(EventArgs.Empty);

				var loginedUser = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

				if (loginedUser == null)
					return null;

				var userID = loginedUser.user.user_id;

				response = StationAPI.Login(
					userID,
					sessionToken);

				return response;
			}
			finally
			{
				OnLogined(new LoginedEventArgs(response));
			}
		}

		/// <summary>
		/// Logins the SNS.
		/// </summary>
		/// <param name="userID">The user ID.</param>
		/// <param name="sessionToken">The session token.</param>
		/// <returns></returns>
		public string LoginSNS(string userID, string sessionToken)
		{
			if (LoginedUser != null)
				return null;

			var response = string.Empty;

			try
			{
				OnLogining(EventArgs.Empty);

				response = StationAPI.Login(
					userID,
					sessionToken);

				return response;
			}
			finally
			{
				OnLogined(new LoginedEventArgs(response));
			}
		}

		public void Logout()
		{
			if (LoginedUser == null)
				return;

			try
			{
				OnLogouting(EventArgs.Empty);
				StationAPI.Logout(LoginedUser.SessionToken);
			}
			finally
			{
				OnLogouted(EventArgs.Empty);
			}
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the Logined event of the LoginController control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="LoginedEventArgs" /> instance containing the event data.</param>
		void LoginController_Logined(object sender, LoginedEventArgs e)
		{
			var response = e.Response;

			if (response.Length == 0)
				return;

			LoginedUser = new LoginedUser(response);
		}

		/// <summary>
		/// Handles the Logouted event of the LoginController control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		void LoginController_Logouted(object sender, EventArgs e)
		{
			LoginedUser = null;
		}
		#endregion
	}
}
