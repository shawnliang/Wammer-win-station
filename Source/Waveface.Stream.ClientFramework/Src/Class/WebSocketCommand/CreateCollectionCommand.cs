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
        public override Dictionary<string, Object> Execute(WebSocketCommandData data)
		{
            var parameters = data.Parameters;

            var sessionToken = StreamClient.Instance.LoginedUser.SessionToken;
            var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

			if (loginedSession == null)
				return null;

			var userID = loginedSession.user.user_id;

			var id = parameters.ContainsKey("id") ? parameters["id"].ToString() : Guid.NewGuid().ToString();
			var name = parameters["name"].ToString();

			var attachmentIDs = from attachmentID in (parameters["attachment_id_array"] as JArray).Values()
								select attachmentID.ToString();

			var visibility = parameters.ContainsKey("visibility") ? int.Parse(parameters["visibility"].ToString()) : 1;
			var timestamp = parameters.ContainsKey("timestamp") ? DateTime.Parse(parameters["timestamp"].ToString()) : default(DateTime?);

			StationAPI.CreateCollection(sessionToken, name, attachmentIDs, id, timestamp);

			if (visibility == 0)
			{
				StationAPI.HideCollection(sessionToken, id, timestamp);
			}

			return null;
		}
		#endregion
	}
}
