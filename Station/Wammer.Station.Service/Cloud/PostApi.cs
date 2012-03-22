using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using MongoDB.Driver.Builders;
using Wammer.Cloud;
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
		private Driver driver;

		public PostApi(Driver driver)
		{
			this.driver = driver;
		}

		public PostFetchByFilterResponse PostFetchByFilter(WebClient agent, FilterEntity filter)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{CloudServer.PARAM_GROUP_ID, this.driver.groups[0].group_id},
				{CloudServer.PARAM_FILTER_ENTITY, filter.ToFastJSON()},
				{CloudServer.PARAM_SESSION_TOKEN, this.driver.session_token},
				{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			};

			PostFetchByFilterResponse res = 
				CloudServer.requestPath<PostFetchByFilterResponse>(agent, "posts/fetchByFilter", parameters);
			return res;
		}

		public PostFetchByFilterResponse PostFetchByPostId(WebClient agent, List<string> postIds)
		{
			if (postIds == null || postIds.Count == 0)
				throw new ArgumentException("postIds is null or empty");

			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{CloudServer.PARAM_GROUP_ID, this.driver.groups[0].group_id},
				{CloudServer.PARAM_FILTER_ENTITY, GetPostIdList(postIds)},
				{CloudServer.PARAM_SESSION_TOKEN, this.driver.session_token},
				{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			};

			PostFetchByFilterResponse res =
				CloudServer.requestPath<PostFetchByFilterResponse>(agent, "posts/fetchByFilter", parameters);
			return res;
		}

		public PostGetLatestResponse PostGetLatest(WebClient agent, int limit)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{CloudServer.PARAM_LIMIT, limit},
				{CloudServer.PARAM_GROUP_ID, this.driver.groups[0].group_id},
				{CloudServer.PARAM_SESSION_TOKEN, this.driver.session_token},
				{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			};

			PostGetLatestResponse res = 
				CloudServer.requestPath<PostGetLatestResponse>(agent, "posts/getLatest", parameters);
			return res;
		}

		private string GetPostIdList(List<string>postIds)
		{
			StringBuilder buff = new StringBuilder();
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
