using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station.APIHandler
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI, "/changelogs/get/")]
	class UserTrackHandler : HttpHandler
	{
		private readonly UserTrackHandlerImp impl = new UserTrackHandlerImp(new UserTrackHandlerDB());

		public UserTrackHandler()
		{
		}

		#region Protected Method

		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			if (CloudServer.VersionNotCompatible)
				throw new WammerStationException("Version not supported", (int)GeneralApiError.NotSupportClient);

			CheckParameter("group_id");

			bool include_entities = ("true" == Parameters["include_entities"]);

			ChangeLogResponse result = impl.GetUserTrack(Parameters["group_id"], Convert.ToInt32(Parameters["since"]), include_entities);
			RespondSuccess(result);
		}

		#endregion



		#region Public Method

		public override object Clone()
		{
			return MemberwiseClone();
		}

		#endregion
	}

	public interface IUserTrackHandlerDB
	{
		Driver GetUserByGroupId(string group_id);
		IEnumerable<UserTracks> GetUserTracksSince(string group_id, int since_seq_num);
	}

	internal class UserTrackHandlerDB : IUserTrackHandlerDB
	{
		#region IUserTrackHandlerDB Members

		public Driver GetUserByGroupId(string group_id)
		{
			return DriverCollection.Instance.FindDriverByGroupId(group_id);
		}

		public IEnumerable<UserTracks> GetUserTracksSince(string group_id, int since_seq_num)
		{
			return UserTrackCollection.Instance.Find(
				Query.And(
					Query.EQ("group_id", group_id),
					Query.GT("next_seq_num", since_seq_num))).SetSortOrder(SortBy.Ascending("next_seq_num"));
		}

		#endregion
	}

	public class UserTrackHandlerImp
	{
		private readonly IUserTrackHandlerDB db;

		public UserTrackHandlerImp(IUserTrackHandlerDB db)
		{
			this.db = db;
		}

		public ChangeLogResponse GetUserTrack(string group_id, int since_seq_num, bool include_entities)
		{
			Driver user = db.GetUserByGroupId(group_id);

			if (user == null)
				throw new WammerStationException("user of group " + group_id + " not found",
												 (int)StationLocalApiError.InvalidDriver);

			if (string.IsNullOrEmpty(user.session_token))
				throw new SessionNotExistException("session not exist", (int)GeneralApiError.SessionNotExist);

			if (!user.is_change_history_synced)
			{
				this.LogInfoMsg("changelogs API is not ready because syncing in progress.");
				return new ChangeLogResponse() { next_seq_num = since_seq_num };
			}

			if (user.sync_range.chlog_max_seq < since_seq_num)
				return new ChangeLogResponse() { next_seq_num = user.sync_range.chlog_max_seq + 1 };

			if (since_seq_num < user.sync_range.chlog_min_seq)
				throw new WammerStationException("User's changelogs do not include this seq: " + since_seq_num, (int)UserTrackApiError.SeqNumPurged);


			IEnumerable<UserTracks> userTracks = db.GetUserTracksSince(group_id, since_seq_num);

			Debug.Assert(userTracks != null, "userTracks != null");

			var response = new ChangeLogResponse();
			if (include_entities)
			{
				response.post_list = mergePostIdLists(userTracks);
				response.changelog_list = mergeDetails(userTracks);
				response.get_count = response.changelog_list.Count;
			}
			else
			{
				response.post_list = mergePostIdLists(userTracks);
				response.get_count = response.post_list.Count;
			}

			response.group_id = group_id;
			response.remaining_count = 0;
			response.next_seq_num = userTracks.Count() > 0 ? userTracks.Last().next_seq_num : since_seq_num;

			return response;
		}

		private List<UserTrackDetail> mergeDetails(IEnumerable<UserTracks> tracks)
		{
			var details = new List<UserTrackDetail>();

			foreach (UserTracks ut in tracks)
			{
				if (ut.usertrack_list == null)
					continue;

				details.AddRange(ut.usertrack_list);
			}

			return details;
		}

		private List<PostListItem> mergePostIdLists(IEnumerable<UserTracks> tracks)
		{
			var posts = new HashSet<string>();

			foreach (UserTracks ut in tracks)
			{
				if (ut.post_id_list == null)
					continue;

				foreach (string post_id in ut.post_id_list)
					posts.Add(post_id);
			}

			return posts.Select(postId => new PostListItem { post_id = postId }).ToList();
		}
	}
}