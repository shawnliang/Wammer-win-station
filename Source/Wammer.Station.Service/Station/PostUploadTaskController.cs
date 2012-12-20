using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PostUpload;
using Wammer.Station;
using Waveface.Stream.Model;

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
			get { return _instance ?? (_instance = new PostUploadTaskController()); }
		}

		#endregion

		#region Public Method

		public void AddPostUploadAction(string postId, PostUploadActionType actionType, NameValueCollection parameters, DateTime timestamp, DateTime lastUpdateTime)
		{
			var userId = FindUserId(parameters[CloudServer.PARAM_GROUP_ID]);
			var apiKey = parameters[CloudServer.PARAM_API_KEY];

			switch (actionType)
			{
				case PostUploadActionType.NewPost:
					PostUploadTaskQueue.Instance.Enqueue(new NewPostTask
															{
																PostId = postId,
																UserId = userId,
																Timestamp = timestamp,
																Parameters = ConvertToDictionary(parameters),
																CodeName =
																	CloudServer.CodeName.ContainsKey(apiKey)
																		? CloudServer.CodeName[apiKey]
																		: string.Empty,
																LastUpdateTime = lastUpdateTime
															});
					break;
				case PostUploadActionType.UpdatePost:
					PostUploadTaskQueue.Instance.Enqueue(new UpdatePostTask
															{
																PostId = postId,
																UserId = userId,
																Timestamp = timestamp,
																Parameters = ConvertToDictionary(parameters),
																CodeName =
																	CloudServer.CodeName.ContainsKey(apiKey)
																		? CloudServer.CodeName[apiKey]
																		: string.Empty,
																LastUpdateTime = lastUpdateTime
															});
					break;
				case PostUploadActionType.Comment:
					PostUploadTaskQueue.Instance.Enqueue(new CommentTask
															{
																PostId = postId,
																UserId = userId,
																Timestamp = timestamp,
																Parameters = ConvertToDictionary(parameters),
																CodeName =
																	CloudServer.CodeName.ContainsKey(apiKey)
																		? CloudServer.CodeName[apiKey]
																		: string.Empty,
																LastUpdateTime = lastUpdateTime
															});
					break;
				case PostUploadActionType.Hide:
					PostUploadTaskQueue.Instance.Enqueue(new HidePostTask
															{
																PostId = postId,
																UserId = userId,
																Timestamp = timestamp,
																Parameters = ConvertToDictionary(parameters),
																CodeName =
																	CloudServer.CodeName.ContainsKey(apiKey)
																		? CloudServer.CodeName[apiKey]
																		: string.Empty,
																LastUpdateTime = lastUpdateTime
															});
					break;
				case PostUploadActionType.UnHide:
					PostUploadTaskQueue.Instance.Enqueue(new UnhidePostTask
															{
																PostId = postId,
																UserId = userId,
																Timestamp = timestamp,
																Parameters = ConvertToDictionary(parameters),
																CodeName =
																	CloudServer.CodeName.ContainsKey(apiKey)
																		? CloudServer.CodeName[apiKey]
																		: string.Empty,
																LastUpdateTime = lastUpdateTime
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
				throw new WammerStationException("Unable to find driver of group " + groupId,
												 (int)StationLocalApiError.InvalidGroup);
			}
			return driver.user_id;
		}

		private Dictionary<string, string> ConvertToDictionary(NameValueCollection collection)
		{
			return collection.AllKeys.ToDictionary(key => key, key => collection[key]);
		}

		#endregion
	}
}