using MongoDB.Bson;

namespace Wammer.Utility
{
	public static class BsonHelper
	{
		public static BsonDocument DeepMerge(this BsonDocument lhs, BsonDocument rhs)
		{
			foreach (BsonElement elem in rhs)
			{
				if (lhs.Contains(elem.Name) &&
				    lhs[elem.Name].IsBsonDocument &&
				    elem.Value.IsBsonDocument)
				{
					DeepMerge(lhs[elem.Name].AsBsonDocument, elem.Value.AsBsonDocument);
				}
				else
				{
					lhs[elem.Name] = elem.Value;
				}
			}

			return lhs;
		}
	}
}