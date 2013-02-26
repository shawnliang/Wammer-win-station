using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Waveface.Stream.Model;

namespace Wammer.Station.AttachmentUpload
{
	public class AttachmentUploadHandlerDB : IAttachmentUploadHandlerDB
	{
		#region IAttachmentUploadHandlerDB Members

		public UpsertResult InsertOrMergeToExistingDoc(Attachment doc)
		{
			var update = new UpdateBuilder();
			BsonDocument bsonDoc = doc.ToBsonDocument();

			update = AppendUpdateStatement(update, string.Empty, bsonDoc);

			SafeModeResult result = AttachmentCollection.Instance.Update(
				Query.EQ("_id", doc.object_id),
				update, UpdateFlags.Upsert);

			return result.UpdatedExisting ? UpsertResult.Update : UpsertResult.Insert;
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

		#endregion

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
	}
}