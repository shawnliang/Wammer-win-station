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
			
			var apiKey = Parameters["apikey"];
			var sessionToken = Parameters["session_token"];

			Station.Instance.Logout(apiKey, sessionToken);

			RespondSuccess();
		}

		#endregion
	}
}