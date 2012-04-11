using System;
using System.Linq;
using Wammer.Station;
using Wammer.Cloud;
using System.Net;
using MongoDB.Driver.Builders;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public class UserLoginHandler : HttpHandler
	{		
		#region Private Method
		/// <summary>
		/// Checks the parameter.
		/// </summary>
		/// <param name="arguementNames">The arguement names.</param>
		private void CheckParameter(params string[] arguementNames)
		{
			if (arguementNames == null)
				throw new ArgumentNullException("arguementNames");

			var nullArgumentNames = from arguementName in arguementNames
									where Parameters[arguementName] == null
									select arguementName;

			var IsAllParameterReady = !nullArgumentNames.Any();
			if (!IsAllParameterReady)
			{
				throw new FormatException(string.Format("Parameter {0} is null.", string.Join("、", nullArgumentNames.ToArray())));
			}
		}
		#endregion


		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		protected override void HandleRequest()
		{
			CheckParameter("email", "password", "apikey");

			string email = Parameters["email"];
						
			Driver existingDriver = Model.DriverCollection.Instance.FindOne(Query.EQ("email", email));
			Boolean isDriverExists = existingDriver != null;

			//Driver not exists => can't user login => exception
			if (!isDriverExists)
				throw new WammerStationException("Cannot find the user with email: " + email, (int)StationApiError.AuthFailed);

			var loginInfo = LoginedSessionCollection.Instance.FindOne(Query.EQ("user.email", email));

			if (loginInfo != null)
			{
				RespondSuccess(loginInfo);
				return;
			}


			string password = Parameters["password"];
			string apikey = Parameters["apikey"];
			User user = null;
			using (WebClient client = new DefaultWebClient())
			{			
				user = User.LogIn(client, email, password, apikey);
			}

			if(user == null)
				throw new WammerStationException("Logined user not found", (int)StationApiError.AuthFailed);

			loginInfo = user.LoginedInfo;

			LoginedSessionCollection.Instance.Save(loginInfo);

			RespondSuccess(loginInfo);
		}
		#endregion

		#region Public Method
		public override object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion
	}
}