using System;
using System.Linq;
using Wammer.Station;
using Wammer.Cloud;
using System.Net;
using MongoDB.Driver.Builders;
using Wammer.Model;
using Wammer.Utility;
using Wammer.Station.APIHandler;

namespace Wammer.Station
{
	public class UserLoginHandler : HttpHandler
	{
		public event EventHandler<UserLoginEventArgs> UserLogined;

		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			CheckParameter("email", "password", "apikey");

			string email = Parameters["email"];
						
			Driver existingDriver = Model.DriverCollection.Instance.FindOne(Query.EQ("email", email));
			Boolean isDriverExists = existingDriver != null;

			//Driver not exists => can't user login => exception
			if (!isDriverExists)
				throw new WammerStationException("Cannot find the user with email: " + email, (int)StationApiError.AuthFailed);

			try
			{
				string password = Parameters["password"];
				string apikey = Parameters["apikey"];
				User user = null;
				using (DefaultWebClient client = new DefaultWebClient())
				{
					client.Timeout = 2500;
					client.ReadWriteTimeout = 2000;
					user = User.LogIn(client, email, password, apikey);
				}

				if (user == null)
					throw new WammerStationException("Logined user not found", (int)StationApiError.AuthFailed);

				var loginInfo = user.LoginedInfo;

				LoginedSessionCollection.Instance.Save(loginInfo);

				RespondSuccess(loginInfo);

				OnUserLogined(new UserLoginEventArgs(email, password, apikey, user.Id));
			}
			catch (WammerCloudException e)
			{
				if (e.HttpError != WebExceptionStatus.ProtocolError)
				{
					var sessionData = LoginedSessionCollection.Instance.FindOne(Query.EQ("user.email", email));
					if (sessionData != null)
						RespondSuccess(sessionData);
					else
						throw;
				}
				else
					throw;
			}
		}

		protected void OnUserLogined(UserLoginEventArgs arg)
		{
			EventHandler<UserLoginEventArgs> handler = UserLogined;
			if (handler != null)
			{
				handler(this, arg);
			}
		}
		#endregion

		#region Public Method
		public override object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion
	}

	[Serializable]
	public class UserLoginEventArgs : EventArgs
	{
		public string email { get; private set; }
		public string password { get; private set; }
		public string apikey { get; private set; }
		public string user_id { get; private set; }

		public UserLoginEventArgs(string email, string password, string apikey, string user_id)
		{
			this.email = email;
			this.password = password;
			this.apikey = apikey;
			this.user_id = user_id;
		}
	}
}