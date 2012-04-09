using System;
using System.Linq;
using System.Collections.Generic;
using Wammer.Station;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	public class PostGetSingleHandler : HttpHandler
	{
		#region Private Property
		private string m_StationID { get; set; }
		private string m_ResourceBasePath { get; set; }
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="PostGetSingleHandler"/> class.
		/// </summary>
		/// <param name="stationId">The station id.</param>
		/// <param name="resourceBasePath">The resource base path.</param>
		public PostGetSingleHandler(string stationId = null, string resourceBasePath = null)
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
			CheckParameter("group_id", "post_id");

			string groupId = Parameters["group_id"];
			string postId = Parameters["post_id"];

			if (!PermissionHelper.IsGroupPermissionOK(groupId, this.Session))
			{
				throw new WammerStationException(
					PostApiError.PermissionDenied.ToString(),
					(int)PostApiError.PermissionDenied
				);
			}

			PostInfo singlePost = PostCollection.Instance.FindOne(
				Query.And(Query.EQ("group_id", groupId), Query.EQ("_id", postId)));

			if (singlePost == null)
			{
				throw new WammerStationException(
					PostApiError.PostNotExist.ToString(),
					(int)PostApiError.PostNotExist
				);
			}

			List<UserInfo> userList = new List<UserInfo>();
			userList.Add(new UserInfo
			{
				user_id = Session.user.user_id,
				nickname = Session.user.nickname,
				avatar_url = Session.user.avatar_url
			});

			RespondSuccess(
				new PostGetSingleResponse
				{
					post = singlePost,
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