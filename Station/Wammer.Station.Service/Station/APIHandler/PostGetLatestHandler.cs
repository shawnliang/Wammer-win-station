using System;
using System.Linq;
using System.Collections.Generic;
using Wammer.Station;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using MongoDB.Driver.Builders;
using MongoDB.Driver;

namespace Wammer.Station
{
	public class PostGetLatestHandler : HttpHandler
	{
		#region Private Property
		private string m_StationID { get; set; }
		private string m_ResourceBasePath { get; set; }
		#endregion

		private const int DEFAULT_LIMIT = 50;
		private const int MAX_LIMIT = 200;

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="PostGetLatestHandler"/> class.
		/// </summary>
		/// <param name="stationId">The station id.</param>
		/// <param name="resourceBasePath">The resource base path.</param>
		public PostGetLatestHandler(string stationId = null, string resourceBasePath = null)
		{
			this.m_StationID = stationId;
			this.m_ResourceBasePath = resourceBasePath;
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Checks the parameter.
		/// </summary>
		/// <param name="arguementNames">The arguement names.</param>
		private void CheckParameter(params string[] arguementNames)
		{
			if (arguementNames == null)
				throw new ArgumentNullException("arguementNames");

			var nullArgumentNames = from arguementName in arguementNames
									where Parameters[arguementName] == null
									select arguementName;

			var IsAllParameterReady = !nullArgumentNames.Any();
			if (!IsAllParameterReady)
			{
				throw new FormatException(string.Format("Parameter {0} is null.", string.Join("、", nullArgumentNames.ToArray())));
			}
		}
		#endregion


		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		protected override void HandleRequest()
		{
			CheckParameter("group_id");

			string groupId = Parameters["group_id"];

			if (!PermissionHelper.IsGroupPermissionOK(groupId, this.Session))
			{
				throw new WammerStationException(
					PostApiError.PermissionDenied.ToString(),
					(int)PostApiError.PermissionDenied
				);
			}

			int limit = (Parameters["limit"] == null ? DEFAULT_LIMIT : int.Parse(Parameters["limit"]));
			if (limit > MAX_LIMIT)
			{
				limit = MAX_LIMIT;
			}
			else if (limit < 1)
			{
				throw new WammerStationException(
					PostApiError.InvalidParameterLimit.ToString(), 
					(int)PostApiError.InvalidParameterLimit
				);
			}

			MongoCursor<PostInfo> posts = PostCollection.Instance
				.Find(Query.And(Query.EQ("group_id", groupId), Query.EQ("hidden", "false")))
				.SetLimit(limit)
				.SetSortOrder(SortBy.Descending("timestamp"));

			List<PostInfo> postList = new List<PostInfo>();
			foreach (PostInfo post in posts)
			{
				postList.Add(post);
			}

			List<UserInfo> userList = new List<UserInfo>();
			userList.Add(new UserInfo{
				user_id=Session.user.user_id, nickname=Session.user.nickname, avatar_url=Session.user.avatar_url});

			long totalCount = PostCollection.Instance
				.Find(Query.And(Query.EQ("group_id", groupId), Query.EQ("hidden", "false"))).Count();

			RespondSuccess(
				new PostGetLatestResponse { 
					total_count = totalCount, 
					get_count = postList.Count, 
					group_id = groupId, 
					posts = postList, 
					users = userList
				}
			);
		}		
		#endregion

		#region Public Method
		public override object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion
	}
}