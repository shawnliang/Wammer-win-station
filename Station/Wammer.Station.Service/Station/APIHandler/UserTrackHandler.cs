using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station.APIHandler
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI, "/usertracks/get/")]
	class UserTrackHandler : HttpHandler
	{
		private readonly UserTrackHandlerImp impl = new UserTrackHandlerImp(new UserTrackHandlerDB());
		private readonly LocalUserTrack.LocalUserTrackManager localUserTrackMgr;

		public UserTrackHandler(LocalUserTrack.LocalUserTrackManager localUserTrackMgr)
		{
			this.localUserTrackMgr = localUserTrackMgr;
		}

		#region Protected Method

		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			CheckParameter("group_id", "since");

			bool include_entities = "true" == Parameters["include_entities"];

			UserTrackResponse result;

			if (Request.RemoteEndPoint.Address.ToString() == "127.0.0.1")
			{
				result = impl.GetUserTrack(Parameters["group_id"], Parameters["since"], include_entities);
			}
			else
			{
				result = UserTracksApi.GetChangeHistory(Parameters["session_token"], Parameters["apikey"],
					Parameters["group_id"], Parameters["since"]);
			}

			var localUserTracks = localUserTrackMgr.getUserTracksBySession(Parameters["group_id"], Parameters["session_token"]);
			
			result.attachment_id_list = mergeAttachmentIdList(result.attachment_id_list, localUserTracks);

			if (include_entities)
			{
				result.usertrack_list = mergeUserTrackList(result.usertrack_list, localUserTracks);
			}
			else
			{
				result.usertrack_list = null;
			}

			RespondSuccess(result);
		}

		#endregion


		private static List<string> mergeAttachmentIdList(List<string> id_list, IEnumerable<UserTrackDetail> localUserTracks)
		{
			if (id_list == null || id_list.Count == 0)
				return localUserTracks.Select(x => x.target_id).ToList();
			else
			{
				HashSet<string> output = new HashSet<string>(id_list);
				foreach (var id in localUserTracks.Select(x => x.target_id))
					output.Add(id);

				return output.ToList();
			}
		}

		private static List<UserTrackDetail> mergeUserTrackList(List<UserTrackDetail> track1, IEnumerable<UserTrackDetail> track2)
		{
			if (track1 == null || track1.Count == 0)
				return track2.ToList();
			else
			{
				var output = track2.ToList();
				output.AddRange(track1);

				return output;
			}
		}

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
		IEnumerable<UserTracks> GetUserTracksSince(string group_id, DateTime since);
	}

	internal class UserTrackHandlerDB : IUserTrackHandlerDB
	{
		#region IUserTrackHandlerDB Members

		public Driver GetUserByGroupId(string group_id)
		{
			return DriverCollection.Instance.FindDriverByGroupId(group_id);
		}

		public IEnumerable<UserTracks> GetUserTracksSince(string group_id, DateTime since)
		{
			return UserTrackCollection.Instance.Find(
				Query.And(
					Query.EQ("group_id", group_id),
					Query.GTE("latest_timestamp", since))).SetSortOrder(SortBy.Ascending("latest_timestamp"));
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

		public UserTrackResponse GetUserTrack(string group_id, string since, bool include_entities)
		{
			Driver user = db.GetUserByGroupId(group_id);

			if (user == null)
				throw new WammerStationException("user of group " + group_id + " not found",
				                                 (int) StationLocalApiError.InvalidDriver);

			if (!user.is_change_history_synced)
				throw new WammerStationException("usertracks API is not ready. Syncing still in progress.",
				                                 (int) StationLocalApiError.NotReady);

			DateTime sinceDateTime = TimeHelper.ParseCloudTimeString(since);
			IEnumerable<UserTracks> userTracks = db.GetUserTracksSince(group_id, sinceDateTime);

			Debug.Assert(userTracks != null, "userTracks != null");

			var response = new UserTrackResponse();
			if (include_entities)
			{
				response.post_id_list = mergePostIdLists(userTracks);
				response.usertrack_list = mergeDetails(userTracks);
				response.get_count = response.usertrack_list.Count;
			}
			else
			{
				response.post_id_list = mergePostIdLists(userTracks);
				response.get_count = response.post_id_list.Count;
			}

			response.group_id = group_id;
			response.latest_timestamp = userTracks.Any() ? userTracks.Last().latest_timestamp : DateTime.UtcNow;
			response.remaining_count = 0;

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

		private List<string> mergePostIdLists(IEnumerable<UserTracks> tracks)
		{
			var posts = new HashSet<string>();

			foreach (UserTracks ut in tracks)
			{
				if (ut.post_id_list == null)
					continue;

				foreach (string post_id in ut.post_id_list)
					posts.Add(post_id);
			}
			return posts.ToList();
		}
	}
}