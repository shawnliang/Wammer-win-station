using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Wammer.Cloud;
using Wammer.Station;
using Wammer.Model;
using MongoDB.Driver.Builders;

using Wammer.PostUpload;

namespace Wammer
{
	public class PostUploadTaskController : IPostUploadSupportable
	{
		#region Static Var
		private static PostUploadTaskController _instance;
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

		#region Public Method
		public void AddPostUploadAction(string postId, PostUploadActionType actionType, NameValueCollection parameters)
		{
			string userId = FindUserId(parameters[CloudServer.PARAM_GROUP_ID]);
			switch (actionType)
			{
				case PostUploadActionType.NewPost:
					PostUploadTaskQueue.Instance.Enqueue(new NewPostTask
					{
						PostId = postId,
						UserId = userId,
						Timestamp = DateTime.Now,
						Parameters = ConvertToDictionary(parameters),
						CodeName = CloudServer.CodeName.ContainsKey(parameters[CloudServer.PARAM_API_KEY]) ?
							CloudServer.CodeName[parameters[CloudServer.PARAM_API_KEY]] : ""
					});
					break;
				case PostUploadActionType.UpdatePost:
					PostUploadTaskQueue.Instance.Enqueue(new UpdatePostTask
					{
						PostId = postId,
						UserId = userId,
						Timestamp = DateTime.Now,
						Parameters = ConvertToDictionary(parameters),
						CodeName = CloudServer.CodeName.ContainsKey(parameters[CloudServer.PARAM_API_KEY]) ?
							CloudServer.CodeName[parameters[CloudServer.PARAM_API_KEY]] : ""
					});
					break;
				case PostUploadActionType.Comment:
					PostUploadTaskQueue.Instance.Enqueue(new CommentTask
					{
						PostId = postId,
						UserId = userId,
						Timestamp = DateTime.Now,
						Parameters = ConvertToDictionary(parameters),
						CodeName = CloudServer.CodeName.ContainsKey(parameters[CloudServer.PARAM_API_KEY]) ?
							CloudServer.CodeName[parameters[CloudServer.PARAM_API_KEY]] : ""
					});
					break;
				case PostUploadActionType.Hide:
					PostUploadTaskQueue.Instance.Enqueue(new HidePostTask
					{
						PostId = postId,
						UserId = userId,
						Timestamp = DateTime.Now,
						Parameters = ConvertToDictionary(parameters),
						CodeName = CloudServer.CodeName.ContainsKey(parameters[CloudServer.PARAM_API_KEY]) ?
							CloudServer.CodeName[parameters[CloudServer.PARAM_API_KEY]] : ""
					});
					break;
				case PostUploadActionType.UnHide:
					PostUploadTaskQueue.Instance.Enqueue(new UnhidePostTask
					{
						PostId = postId,
						UserId = userId,
						Timestamp = DateTime.Now,
						Parameters = ConvertToDictionary(parameters),
						CodeName = CloudServer.CodeName.ContainsKey(parameters[CloudServer.PARAM_API_KEY]) ?
							CloudServer.CodeName[parameters[CloudServer.PARAM_API_KEY]] : ""
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
				throw new WammerStationException("Unable to find driver of group " + groupId, (int)StationLocalApiError.InvalidGroup);
			}
			return driver.user_id;
		}

		private Dictionary<string, string> ConvertToDictionary(NameValueCollection collection)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			foreach (String key in collection.AllKeys)
			{
				dic.Add(key, collection[key]);
			}
			return dic;
		}
		#endregion
	}
}
