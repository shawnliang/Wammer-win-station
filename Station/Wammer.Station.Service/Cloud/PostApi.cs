using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Cloud
{
	public class FilterEntity
	{
		public string timestamp { get; set; }
		public string type { get; set; }
		public int limit { get; set; }
	}

	public class PostApi
	{
		private readonly Driver driver;

		public PostApi(Driver driver)
		{
			this.driver = driver;
		}

		public NewPostResponse NewPost(WebClient agent, string postId, DateTime timestamp, Dictionary<string, string> param)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
			                 		{CloudServer.PARAM_POST_ID, postId},
			                 		{CloudServer.PARAM_TIMESTAMP, timestamp.ToCloudTimeString()}
			                 	};

			foreach (var key in param.Keys)
			{
				if (key != CloudServer.PARAM_SESSION_TOKEN && key != CloudServer.PARAM_API_KEY)
				{
					parameters.Add(key, param[key]);
				}
			}

			return CloudServer.requestPath<NewPostResponse>(agent, "posts/new", parameters);
		}

		public UpdatePostResponse UpdatePost(WebClient agent, DateTime updateTime, Dictionary<string, string> param)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			if (!param.ContainsKey(CloudServer.PARAM_UPDATE_TIME))
			{
				parameters.Add(CloudServer.PARAM_UPDATE_TIME, updateTime.ToCloudTimeString());
			}

			foreach (var key in param.Keys)
			{
				if (key != CloudServer.PARAM_SESSION_TOKEN && key != CloudServer.PARAM_API_KEY)
				{
					parameters.Add(key, param[key]);
				}
			}

			return CloudServer.requestPath<UpdatePostResponse>(agent, "posts/update", parameters);
		}

		public NewPostCommentResponse NewComment(WebClient agent, DateTime updateTime, Dictionary<string, string> param)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			if (!param.ContainsKey(CloudServer.PARAM_UPDATE_TIME))
			{
				parameters.Add(CloudServer.PARAM_UPDATE_TIME, updateTime.ToCloudTimeString());
			}

			foreach (var key in param.Keys)
			{
				if (key != CloudServer.PARAM_SESSION_TOKEN && key != CloudServer.PARAM_API_KEY)
				{
					parameters.Add(key, param[key]);
				}
			}

			return CloudServer.requestPath<NewPostCommentResponse>(agent, "posts/newComment", parameters);
		}

		public HidePostResponse HidePost(WebClient agent, DateTime updateTime, Dictionary<string, string> param)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			if (!param.ContainsKey(CloudServer.PARAM_UPDATE_TIME))
			{
				parameters.Add(CloudServer.PARAM_UPDATE_TIME, updateTime.ToCloudTimeString());
			}

			foreach (var key in param.Keys)
			{
				if (key != CloudServer.PARAM_SESSION_TOKEN && key != CloudServer.PARAM_API_KEY)
				{
					parameters.Add(key, param[key]);
				}
			}

			return CloudServer.requestPath<HidePostResponse>(agent, "posts/hide", parameters);
		}

		public PostFetchByFilterResponse PostFetchByFilter(WebClient agent, FilterEntity filter)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_GROUP_ID, driver.groups[0].group_id},
			                 		{CloudServer.PARAM_FILTER_ENTITY, filter.ToFastJSON()},
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			var res =
				CloudServer.requestPath<PostFetchByFilterResponse>(agent, "posts/fetchByFilter", parameters, false);
			return res;
		}

		public PostFetchByFilterResponse PostFetchByPostId(WebClient agent, List<string> postIds)
		{
			if (postIds == null || postIds.Count == 0)
				throw new ArgumentException("postIds is null or empty");

			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_GROUP_ID, driver.groups[0].group_id},
			                 		{CloudServer.PARAM_POST_ID_LIST, GetPostIdList(postIds)},
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			var res =
				CloudServer.requestPath<PostFetchByFilterResponse>(agent, "posts/fetchByFilter", parameters, false);
			return res;
		}

		public PostGetLatestResponse PostGetLatest(WebClient agent, int limit)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_LIMIT, limit},
			                 		{CloudServer.PARAM_GROUP_ID, driver.groups[0].group_id},
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			var res =
				CloudServer.requestPath<PostGetLatestResponse>(agent, "posts/getLatest", parameters, false);
			return res;
		}

		public PostGetSingleResponse PostGetSingle(WebClient agent, string groupId, string postId)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_POST_ID, postId},
			                 		{CloudServer.PARAM_GROUP_ID, groupId},
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			var res =
				CloudServer.requestPath<PostGetSingleResponse>(agent, "posts/getSingle", parameters);
			return res;
		}

		private string GetPostIdList(List<string> postIds)
		{
			var buff = new StringBuilder();
			buff.Append("[").Append("\"").Append(postIds[0]).Append("\"");

			for (int i = 1; i < postIds.Count; i++)
			{
				buff.Append(",").Append("\"").Append(postIds[i]).Append("\"");
			}

			buff.Append("]");

			return buff.ToString();
		}
	}
}