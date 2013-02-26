using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Waveface.Stream.Model;

namespace Waveface.Stream.Core
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
