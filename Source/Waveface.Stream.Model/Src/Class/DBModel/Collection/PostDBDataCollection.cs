using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Waveface.Stream.Model
{
	/// <summary>
	/// 
	/// </summary>
	public class PostDBDataCollection : DBCollection<PostDBData>
	{
		#region Var
		private static PostDBDataCollection _instance;
		#endregion


		#region Property       
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static PostDBDataCollection Instance
        { 
            get
            {
                return _instance ?? (_instance = new PostDBDataCollection());
            }
        }
		#endregion


		#region Constructor
		/// <summary>
		/// Prevents a default instance of the <see cref="PostDBDataCollection" /> class from being created.
		/// </summary>
		private PostDBDataCollection()
			: base("posts")
		{
			collection.EnsureIndex(new IndexKeysBuilder().Ascending("group_id"));
			collection.EnsureIndex(new IndexKeysBuilder().Descending("timestamp"));
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Updates the specified post.
		/// </summary>
		/// <param name="post">The post.</param>
		public void Update(PostDBData post)
		{
			lock (this)
			{
				var oldPost = FindOne(Query.EQ("_id", post.ID));
				if (oldPost == null || oldPost.ModifyTime < post.ModifyTime)
				{
					Save(post);
				}
			}
		}
		#endregion
	}
}