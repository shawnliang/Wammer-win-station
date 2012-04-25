using System.Collections.Generic;
using System;
using System.Linq;
using Wammer.Station;
using Wammer.Model;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using Wammer.Cloud;
using System.Globalization;

namespace Wammer.Station.APIHandler
{
	public class UserTrackHandler : HttpHandler
	{
		private UserTrackHandlerImp impl = new UserTrackHandlerImp(new UserTrackHandlerDB());
		
		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			CheckParameter("group_id", "since");

			bool include_entities = false;
			if ("true" == Parameters["include_entities"])
				include_entities = true;

			this.RespondSuccess(
				impl.GetUserTrack(Parameters["group_id"], Parameters["since"], include_entities));
		}
		#endregion

		#region Public Method
		public override object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion
	}

	public interface IUserTrackHandlerDB
	{
		Driver GetUserByGroupId(string group_id);
		IEnumerable<UserTracks> GetUserTracksSince(string group_id, DateTime since);
	}

	class UserTrackHandlerDB : IUserTrackHandlerDB
	{
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
	}

	public class UserTrackHandlerImp
	{
		private IUserTrackHandlerDB db;

		public UserTrackHandlerImp(IUserTrackHandlerDB db)
		{
			this.db = db;
		}

		public UserTrackResponse GetUserTrack(string group_id, string since, bool include_entities)
		{
			Driver user = db.GetUserByGroupId(group_id);

			if (user == null)
				throw new WammerStationException("user of group " + group_id + " not found", (int)StationApiError.InvalidDriver);

			if (!user.is_change_history_synced)
				throw new WammerStationException("usertracks API is not ready. Syncing still in progress.", (int)StationApiError.NotReady);

			DateTime sinceDateTime = Wammer.Utility.TimeHelper.ParseCloudTimeString(since);
			IEnumerable<UserTracks> userTracks = db.GetUserTracksSince(group_id, sinceDateTime);

			UserTrackResponse response = new UserTrackResponse();

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
			response.latest_timestamp = userTracks.Count() > 0 ? userTracks.Last().latest_timestamp : DateTime.UtcNow;
			response.remaining_count = 0;

			return response;
		}

		private List<UserTrackDetail> mergeDetails(IEnumerable<UserTracks> tracks)
		{
			List<UserTrackDetail> details = new List<UserTrackDetail>();

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
			HashSet<string> posts = new HashSet<string>();

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