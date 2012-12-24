using System;
using System.Collections.Generic;
using System.Text;
using Wammer.Model;
using Wammer.Utility;
using Waveface.Stream.Model;

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

		public NewPostResponse NewPost(string postId, DateTime timestamp, Dictionary<string, string> param)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
			                 		{CloudServer.PARAM_POST_ID, postId},
			                 		{CloudServer.PARAM_TIMESTAMP, timestamp.ToUTCISO8601ShortString()}
			                 	};

			foreach (var key in param.Keys)
			{
				if (key != CloudServer.PARAM_SESSION_TOKEN && key != CloudServer.PARAM_API_KEY)
				{
					if (!parameters.ContainsKey(key))
						parameters.Add(key, param[key]);
				}
			}

			return CloudServer.requestPath<NewPostResponse>("posts/new", parameters);
		}

		public UpdatePostResponse UpdatePost(DateTime updateTime, DateTime lastUpdateTime, Dictionary<string, string> param)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			if (!param.ContainsKey(CloudServer.PARAM_UPDATE_TIME))
			{
				parameters.Add(CloudServer.PARAM_UPDATE_TIME, updateTime.ToUTCISO8601ShortString());
			}

			foreach (var key in param.Keys)
			{
				if (key == CloudServer.PARAM_LAST_UPDATE_TIME)
				{
					parameters.Add(key, lastUpdateTime.ToUTCISO8601ShortString());
				}
				else if (key != CloudServer.PARAM_SESSION_TOKEN && key != CloudServer.PARAM_API_KEY)
				{
					parameters.Add(key, param[key]);
				}
			}

			return CloudServer.requestPath<UpdatePostResponse>("posts/update", parameters);
		}

		public NewPostCommentResponse NewComment(DateTime updateTime, Dictionary<string, string> param)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			if (!param.ContainsKey(CloudServer.PARAM_UPDATE_TIME))
			{
				parameters.Add(CloudServer.PARAM_UPDATE_TIME, updateTime.ToUTCISO8601ShortString());
			}

			foreach (var key in param.Keys)
			{
				if (key != CloudServer.PARAM_SESSION_TOKEN && key != CloudServer.PARAM_API_KEY)
				{
					parameters.Add(key, param[key]);
				}
			}

			return CloudServer.requestPath<NewPostCommentResponse>("posts/newComment", parameters);
		}

		public HidePostResponse HidePost(DateTime updateTime, Dictionary<string, string> param)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			if (!param.ContainsKey(CloudServer.PARAM_UPDATE_TIME))
			{
				parameters.Add(CloudServer.PARAM_UPDATE_TIME, updateTime.ToUTCISO8601ShortString());
			}

			foreach (var key in param.Keys)
			{
				if (key != CloudServer.PARAM_SESSION_TOKEN && key != CloudServer.PARAM_API_KEY)
				{
					parameters.Add(key, param[key]);
				}
			}

			return CloudServer.requestPath<HidePostResponse>("posts/hide", parameters);
		}

		public PostFetchByFilterResponse PostFetchByFilter(FilterEntity filter)
		{
			var parameters = new Dictionary<object, object>
			{
				{CloudServer.PARAM_GROUP_ID, driver.groups[0].group_id},
				{CloudServer.PARAM_FILTER_ENTITY, filter.ToFastJSON()},
				{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
				{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
				{CloudServer.PARAM_COMPONENT_OPTIONS, "[\"comment\",\"preview\",\"soul\",\"content\"]"}
			};

			return CloudServer.requestPath<PostFetchByFilterResponse>("posts/fetchByFilter", parameters, false);
		}

		public PostFetchByFilterResponse PostFetchBySeq(int seq_num, int limit)
		{
			var parameters = new Dictionary<object, object>
			{
				{CloudServer.PARAM_GROUP_ID, driver.groups[0].group_id},
				{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
				{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
				{CloudServer.PARAM_DATUM, seq_num},
				{CloudServer.PARAM_LIMIT, limit},
				{CloudServer.PARAM_COMPONENT_OPTIONS, "[\"comment\",\"preview\",\"soul\",\"content\"]"}
			};

			return CloudServer.requestPath<PostFetchByFilterResponse>("posts/fetchBySeq", parameters, false);
		}

		public PostFetchByFilterResponse PostFetchByPostId(List<string> postIds)
		{
			if (postIds == null || postIds.Count == 0)
				throw new ArgumentException("postIds is null or empty");

			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_GROUP_ID, driver.groups[0].group_id},
			                 		{CloudServer.PARAM_POST_ID_LIST, GetPostIdList(postIds)},
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
									{CloudServer.PARAM_COMPONENT_OPTIONS, "[\"comment\",\"preview\",\"soul\",\"content\"]"}
			                 	};

			return CloudServer.requestPath<PostFetchByFilterResponse>("posts/fetchByFilter", parameters, false);
		}

		public PostGetLatestResponse PostGetLatest(int limit)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_LIMIT, limit},
			                 		{CloudServer.PARAM_GROUP_ID, driver.groups[0].group_id},
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
									{CloudServer.PARAM_COMPONENT_OPTIONS, "[\"comment\",\"preview\",\"soul\",\"content\"]"}
			                 	};

			return CloudServer.requestPath<PostGetLatestResponse>("posts/getLatest", parameters, false);
		}

		public PostGetSingleResponse PostGetSingle(string groupId, string postId)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_POST_ID, postId},
			                 		{CloudServer.PARAM_GROUP_ID, groupId},
			                 		{CloudServer.PARAM_SESSION_TOKEN, driver.session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			var res =
				CloudServer.requestPath<PostGetSingleResponse>("posts/getSingle", parameters);
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