using Wammer.Cloud;
using MongoDB.Driver.Builders;

namespace Wammer.Model
{
	public class PostCollection : Collection<PostInfo>
	{
		#region Var
		private static PostCollection _instance;
		private object cs = new object();
		#endregion

		#region Property
		public static PostCollection Instance
		{
			get { return _instance ?? (_instance = new PostCollection()); }
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
	}
}