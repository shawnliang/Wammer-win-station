using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using System.Windows.Forms;
using Waveface.Stream.Model;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using MongoDB.Bson;
using AutoMapper;
using System.Text.RegularExpressions;

namespace Waveface.Stream.ClientFramework
{
	public class UpdateCollectionCommand : WebSocketCommandBase
    {
        #region Public Property
        /// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "updateCollection"; }
		}
		#endregion


        #region Public Method
        /// <summary>
        /// Executes the specified parameters.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override Dictionary<string, Object> Execute(WebSocketCommandData data)
		{
            var parameters = data.Parameters;

            var sessionToken = StreamClient.Instance.LoginedUser.SessionToken;
            var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

			if (loginedSession == null)
				return null;

			var userID = loginedSession.user.user_id;

			var id = parameters.ContainsKey("id") ? parameters["id"].ToString() : string.Empty;
			var name = parameters["name"].ToString();

			var coverAttachID = parameters.ContainsKey("cover_attach") ? parameters["cover_attach"].ToString() : string.Empty;

			var attachmentIDs = parameters.ContainsKey("attachment_id_array")? (from attachmentID in (parameters["attachment_id_array"] as JArray).Values()
																			   select attachmentID.ToString()): null;

			var visibility = parameters.ContainsKey("visibility") ? int.Parse(parameters["visibility"].ToString()) : 1;
			var timestamp = parameters.ContainsKey("timestamp") ? DateTime.Parse(parameters["timestamp"].ToString()) : default(DateTime?);

			StationAPI.UpdateCollection(sessionToken, id, name, attachmentIDs, coverAttachID, visibility == 0, timestamp);

			return null;
		}
		#endregion
	}
}
