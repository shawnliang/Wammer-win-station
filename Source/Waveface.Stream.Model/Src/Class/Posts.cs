using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System.Linq;

namespace Waveface.Stream.Model
{
	public class PostCollection : Collection<PostInfo>
	{
		#region Var
		private static PostCollection _instance;
		private object cs = new object();
		#endregion


		static PostCollection()
		{
			_instance = new PostCollection();
			_instance.collection.EnsureIndex(new IndexKeysBuilder().Ascending("group_id"));
			_instance.collection.EnsureIndex(new IndexKeysBuilder().Descending("timestamp"));
		}


		#region Property
		public static PostCollection Instance
		{
			get { return _instance; }
		}
		#endregion
		
		private PostCollection()
			: base("posts")
		{
		}

		public void Update(PostInfo post)
		{
			lock (cs)
			{
				var oldPost = FindOne(Query.EQ("_id", post.post_id));
				if (oldPost == null || oldPost.update_time < post.update_time)
				{
					Save(post);
				}
			}
		}

        //public void UpdateAttachments(PostInfo post)
        //{
        //    Update(
        //        Query.EQ("_id", post.post_id),
        //        MongoDB.Driver.Builders.Update.Set("attachments", new BsonArray(post.attachments.Select(x => x.ToBsonDocument())))
        //    );
        //}
	}
}