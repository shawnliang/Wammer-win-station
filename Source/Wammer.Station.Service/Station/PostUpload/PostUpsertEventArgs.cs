using System;

namespace Wammer.PostUpload
{
	public class PostUpsertEventArgs : EventArgs
	{
		public string PostId { get; private set; }
		public string SessionToken { get; private set; }
		public string UserId { get; private set; }

		public PostUpsertEventArgs(string postId, string sessionToken, string userId)
		{
			this.PostId = postId;
			this.SessionToken = sessionToken;
			this.UserId = userId;
		}
	}
}
