using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Waveface.Stream.Model;

namespace Waveface.Stream.Core
{
	public class CreateCollectionCommand : WebSocketCommandBase
	{
		#region Public Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "createCollection"; }
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Executes the specified parameters.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public override Dictionary<string, Object> Execute(WebSocketCommandData data, Dictionary<string, Object> systemArgs = null)
		{
			var parameters = data.Parameters;

			var sessionToken = parameters.ContainsKey("session_token") ? parameters["session_token"].ToString() : string.Empty;

			if (sessionToken.Length == 0)
				return null;

			var loginedUser = LoginedSessionCollection.Instance.FindOneById(sessionToken);

			if (loginedUser == null)
				return null;

			var userID = loginedUser.user.user_id;

			var id = parameters.ContainsKey("id") ? parameters["id"].ToString() : Guid.NewGuid().ToString();
			var name = parameters["name"].ToString();

			var coverAttachID = parameters.ContainsKey("cover_attach") ? parameters["cover_attach"].ToString() : string.Empty;

			var attachmentIDs = from attachmentID in (parameters["attachment_id_array"] as JArray).Values()
								select attachmentID.ToString();

			var visibility = parameters.ContainsKey("visibility") ? int.Parse(parameters["visibility"].ToString()) : 1;
			var timestamp = parameters.ContainsKey("timestamp") ? DateTime.Parse(parameters["timestamp"].ToString()) : default(DateTime?);


			StationAPI.CreateCollection(sessionToken, name, attachmentIDs, id, coverAttachID, true, timestamp);

			if (visibility == 0)
			{
				StationAPI.HideCollection(sessionToken, id, timestamp);
			}

			return null;
		}
		#endregion
	}
}
