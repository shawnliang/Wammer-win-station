using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer
{
	public interface IPostUploadSupportable
	{
		void AddPostUploadAction(PostUploadActionType actionType, string[] parameters);
	}
}
