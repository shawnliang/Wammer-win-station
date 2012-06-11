using Wammer.Cloud;

namespace Wammer.Model
{
	public class PostCollection : Collection<PostInfo>
	{
		#region Var
		private static PostCollection _instance; 
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
	}
}