using System.Collections.Generic;
using System;
using System.Linq;
using Wammer.Station;
using Wammer.Model;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using Wammer.Cloud;

namespace Wammer.Station
{
	public class UserTrackHandler : HttpHandler
	{
		const int LIMIT = 200;

		#region Private Property
		private string m_StationID { get; set; }
		private string m_ResourceBasePath { get; set; }
		#endregion


		#region Constructor
		public UserTrackHandler()
		{
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
			CheckParameter("group_id", "since");

			if ("true" == Parameters["include_entities"])
				throw new WammerStationException("Station does not support include_entities=true", (int)StationApiError.Error);


			string since = Parameters["since"];
			string group_id = Parameters["group_id"];

			IMongoQuery queryCriteria;

			if (string.IsNullOrEmpty(since))
				queryCriteria = Query.And(
					Query.EQ("group_id", group_id),
					Query.EQ("hidden", "false"));
			else
				queryCriteria = Query.And(
					Query.EQ("group_id", group_id),
					Query.EQ("hidden", "false"),
					Query.GTE("update_time", since));

			MongoCursor<PostInfo> posts = PostCollection.Instance.Find(
				queryCriteria).SetSortOrder(SortBy.Ascending("update_time"));

			long total_count = posts.Count();

			posts = posts.SetLimit(LIMIT);

			List<string> idList = getIdList(posts);

			RespondSuccess(new UserTrackResponse
			{
				api_ret_code = 0,
				api_ret_message = "success",
				get_count = (int)total_count,
				group_id = group_id,
				latest_timestamp = posts.Last().update_time,
				status = 200,
				timestamp = DateTime.UtcNow,
				remaining_count = (int)(total_count - idList.Count),
				post_id_list = idList
			});
		}

		protected List<string> getIdList(MongoCursor<PostInfo> posts)
		{
			List<string> idList = new List<string>();

			foreach (PostInfo post in posts)
			{
				idList.Add(post.post_id);
				if (idList.Count == LIMIT)
					return idList;
			}

			return idList;
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