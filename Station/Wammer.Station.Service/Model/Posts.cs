using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Model
{
	public class PostCollection : Collection<Wammer.Cloud.PostInfo>
	{
		private static PostCollection instance;

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
