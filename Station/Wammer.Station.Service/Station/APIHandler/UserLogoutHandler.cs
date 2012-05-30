using System;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI, "/auth/logout/")]
	public class UserLogoutHandler : HttpHandler
	{
		#region Protected Method

		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			CheckParameter("session_token", "apikey");

			var sessionToken = Parameters["session_token"];


			try
			{
				var apiKey = Parameters["apikey"];
				User.LogOut(sessionToken, apiKey);
			}
			catch (Exception e)
			{
				this.LogDebugMsg("Unable to logout from Stream cloud", e);
			}

			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

			if (loginedSession != null)
				LoginedSessionCollection.Instance.Remove(Query.EQ("user.email", loginedSession.user.email));

			RespondSuccess();
		}

		#endregion
	}
}