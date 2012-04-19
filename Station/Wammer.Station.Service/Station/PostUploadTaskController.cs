using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Wammer
{
	public class PostUploadTaskController : IPostUploadSupportable
	{
		#region Static Var
		private static PostUploadTaskController _instance;
		#endregion

		#region Var
		private PostUploadTaskQueue _queue;
		#endregion

		#region Public Static Property
		public static PostUploadTaskController Instance 
		{
			get
			{
				if (_instance == null)
					_instance = new PostUploadTaskController();
				return _instance;
			}
		}
		#endregion

		#region Private Property
		public PostUploadTaskQueue m_Queue 
		{
			get
			{
				if (_queue == null)
					_queue = new PostUploadTaskQueue();
				return _queue;
			}
		}
		#endregion

		#region Public Method
		public void AddPostUploadAction(PostUploadActionType actionType, NameValueCollection parameters)
		{
		} 
		#endregion
	}
}
