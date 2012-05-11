
namespace Wammer.Model
{
	public class PostCollection : Collection<Cloud.PostInfo>
	{
		private static readonly PostCollection instance;

		static PostCollection()
		{
			instance = new PostCollection();
		}

		private PostCollection()
			: base("posts")
		{
		}

		public static PostCollection Instance
		{
			get { return instance; }
		}
	}
}
