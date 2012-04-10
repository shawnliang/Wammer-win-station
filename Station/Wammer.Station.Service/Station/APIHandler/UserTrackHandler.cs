using System.Collections.Generic;
using System;
using System.Linq;
using Wammer.Station;
using Wammer.Model;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using Wammer.Cloud;

namespace Wammer.Station.APIHandler
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
		IEnumerable<UserTracks> GetUserTracksSince(string group_id, string since);
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


			IEnumerable<UserTracks> userTracks = db.GetUserTracksSince(group_id, since);

			UserTrackResponse response = new UserTrackResponse();
			response.post_id_list = mergePostIdLists(userTracks);
			response.usertrack_list = mergeDetails(userTracks);
			response.get_count = response.usertrack_list.Count;
			response.group_id = group_id;
			response.latest_timestamp = userTracks.Last().latest_timestamp;
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