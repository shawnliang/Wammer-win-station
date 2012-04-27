using System;
using System.Linq;
using Wammer.Station;
using System.Net;
using Wammer.Cloud;
using MongoDB.Driver.Builders;
using Wammer.Model;
using System.Net.NetworkInformation;
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
			catch (System.Exception)
			{
			}

			LoginedSessionCollection.Instance.Remove(Query.EQ("_id", sessionToken));

			RespondSuccess();
		}
		#endregion
	}
}