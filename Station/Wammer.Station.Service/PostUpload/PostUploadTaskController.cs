using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Wammer.Station;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace Wammer.PostUpload
{
	public class PostUploadTaskController : IPostUploadSupportable
	{
		#region Var
		private IUndoablePostUploadTaskQueue taskQueue;
		#endregion

		#region Public Method
		public PostUploadTaskController(IUndoablePostUploadTaskQueue queue)
		{
			this.taskQueue = queue;
		}

		public void AddPostUploadAction(string postId, PostUploadActionType actionType, NameValueCollection parameters)
		{
			string userId = FindUserId(parameters["group_id"]);
			switch (actionType)
			{
				case PostUploadActionType.NewPost:
					taskQueue.Enqueue(new NewPostTask
					{
						PostId = postId,
						UserId = userId,
						Timestamp = DateTime.UtcNow,
						Parameters = parameters
					});
					break;
				case PostUploadActionType.UpdatePost:
					taskQueue.Enqueue(new UpdatePostTask
					{
						PostId = postId,
						UserId = userId,
						Timestamp = DateTime.UtcNow,
						Parameters = parameters
					});
					break;
				case PostUploadActionType.Comment:
					taskQueue.Enqueue(new CommentTask
					{
						PostId = postId,
						UserId = userId,
						Timestamp = DateTime.UtcNow,
						Parameters = parameters
					});
					break;
				case PostUploadActionType.Hide:
					taskQueue.Enqueue(new HidePostTask
					{
						PostId = postId,
						UserId = userId,
						Timestamp = DateTime.UtcNow,
						Parameters = parameters
					});
					break;
				case PostUploadActionType.UnHide:
					taskQueue.Enqueue(new UnhidePostTask
					{
						PostId = postId,
						UserId = userId,
						Timestamp = DateTime.UtcNow,
						Parameters = parameters
					});
					break;
				default:
					this.LogWarnMsg("Post upload action type " + actionType.ToString() + " is not supported.");
					break;
			}
		} 
		#endregion

		#region Private Method
		private string FindUserId(string groupId)
		{
			Driver driver = DriverCollection.Instance.FindOne(Query.ElemMatch("groups", Query.EQ("group_id", groupId)));
			if (driver == null)
			{
				throw new WammerStationException("Unable to find driver of group " + groupId, (int)StationApiError.InvalidGroup);
			}
			return driver.user_id;
		}
		#endregion
	}
}
