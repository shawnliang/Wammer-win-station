using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

using System.Reflection;

namespace Wammer.Station.AttachmentUpload
{
	public class AttachmentUploadHandlerDB : IAttachmentUploadHandlerDB
	{
		public UpsertResult InsertOrMergeToExistingDoc(Model.Attachment doc)
		{
			var update = new UpdateBuilder();
			BsonDocument bsonDoc = doc.ToBsonDocument();

			update = AppendUpdateStatement(update, "", bsonDoc);

			MongoDB.Driver.SafeModeResult result = AttachmentCollection.Instance.Update(
				Query.EQ("_id", doc.object_id), 
				update, MongoDB.Driver.UpdateFlags.Upsert);

			return result.UpdatedExisting ? UpsertResult.Update : UpsertResult.Insert;
		}

		private static UpdateBuilder AppendUpdateStatement(UpdateBuilder update, string prefix, BsonDocument bsonDoc)
		{
			foreach (BsonElement elem in bsonDoc.Elements)
			{
				if (elem.Name == "_id")
					continue;

				if (elem.Value.IsBsonDocument)
					AppendUpdateStatement(update, prefix + elem.Name + ".", elem.Value.AsBsonDocument);
				else
					update = update.Set(prefix + elem.Name, elem.Value);
			}
			return update;
		}

		public Model.Driver GetUserByGroupId(string groupId)
		{
			return Model.DriverCollection.Instance.FindDriverByGroupId(groupId);
		}


		public LoginedSession FindSession(string sessionToken, string apiKey)
		{
			LoginedSession session = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));
			if (session != null && session.apikey.apikey == apiKey)
			{
				return session;
			}

			return null;
		}
	}
}
