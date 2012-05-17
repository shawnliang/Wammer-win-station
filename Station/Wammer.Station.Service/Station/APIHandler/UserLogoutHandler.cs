using System;
using System.Net;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public class UserLogoutHandler : HttpHandler
	{
		#region Protected Method

		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			CheckParameter("session_token", "apikey");

			string sessionToken = Parameters["session_token"];


			try
			{
				string apiKey = Parameters["apikey"];
				using (WebClient client = new DefaultWebClient())
				{
					User.LogOut(client, sessionToken, apiKey);
				}
			}
			catch (Exception e)
			{
				this.LogDebugMsg("Unable to logout from Stream cloud", e);
			}

			LoginedSession loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

			if (loginedSession != null)
				LoginedSessionCollection.Instance.Remove(Query.EQ("user.email", loginedSession.user.email));

			RespondSuccess();
		}

		#endregion
	}
}