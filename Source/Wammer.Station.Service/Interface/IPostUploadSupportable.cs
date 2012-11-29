using System;
using System.Collections.Specialized;

namespace Wammer
{
	public interface IPostUploadSupportable
	{
		void AddPostUploadAction(string postId, PostUploadActionType actionType, NameValueCollection parameters, DateTime updateTime, DateTime lastUpdateTime);
	}
}