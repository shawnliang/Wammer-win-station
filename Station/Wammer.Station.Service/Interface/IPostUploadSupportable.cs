using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Wammer
{
	public interface IPostUploadSupportable
	{
		void AddPostUploadAction(string postId, PostUploadActionType actionType, NameValueCollection parameters);
	}
}
