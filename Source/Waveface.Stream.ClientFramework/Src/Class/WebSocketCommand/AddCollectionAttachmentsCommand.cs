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
	public class AddCollectionAttachmentsCommand : WebSocketCommandBase
    {
        #region Public Property
        /// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "addCollectionAttachments"; }
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
			var index = parameters.ContainsKey("index") ? int.Parse(parameters["index"].ToString()) : -1;

			var attachmentIDs = from attachmentID in (parameters["attachment_id_array"] as JArray).Values()
								select attachmentID.ToString();

			if (!attachmentIDs.Any())
				return null;

			var collection = CollectionCollection.Instance.FindOne(Query.EQ("_id", id));

			if (collection == null)
				return null;

			var originalAttachmentIDs = collection.attachment_id_array;
			var insertAttachmentIDs = attachmentIDs.Except(originalAttachmentIDs).Distinct();
			var totalAttachmentCount = originalAttachmentIDs.Count() + insertAttachmentIDs.Count();

			if (index < -1 && index > totalAttachmentCount)
				return null;

			index = originalAttachmentIDs.Count;
			var totalAttachmentIDs = new List<string>(originalAttachmentIDs);
			totalAttachmentIDs.InsertRange(index, insertAttachmentIDs);

			StationAPI.UpdateCollection(sessionToken, id, attachmentIDs: totalAttachmentIDs);
			return null;
		}
		#endregion
	}
}
