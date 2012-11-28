using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using System.Net;

namespace Wammer.Station.APIHandler
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI, "/changelogs/get/")]
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
			if (CloudServer.VersionNotCompatible)
				throw new WammerStationException("Version not supported", (int)GeneralApiError.NotSupportClient);

			CheckParameter("group_id");

			bool include_entities = ("true" == Parameters["include_entities"]);

			ChangeLogResponse result;

			if (Request.RemoteEndPoint.Address.ToString() == "127.0.0.1")
			{
				result = impl.GetUserTrack(Parameters["group_id"], Convert.ToInt32(Parameters["since"]), include_entities);
			}
			else
			{
				var sinceParam = Parameters["since_seq_num"];

				try
				{
					result = ChangeLogsApi.GetChangeHistory(Parameters["session_token"], Parameters["apikey"],
						Parameters["group_id"], sinceParam == null ? -1 : Convert.ToInt32(sinceParam), include_entities);
				}
				catch (WammerCloudException e)
				{
					if (e.HttpError == System.Net.WebExceptionStatus.ProtocolError)
					{
						var webEx = e.InnerException as WebException;
						var status = (webEx.Response as HttpWebResponse).StatusCode;

						RespondError(status, e.response, "application/json");
						return;
					}
					else
						throw;
				}
			}

			var localUserTracks = localUserTrackMgr.getUserTracksBySession(Parameters["group_id"], Parameters["session_token"]);
			
			result.attachment_list = mergeAttachmentIdList(result.attachment_list, localUserTracks);

			if (include_entities)
			{
				result.changelog_list = mergeUserTrackList(result.changelog_list, localUserTracks);
			}
			else
			{
				result.changelog_list = null;
			}

			RespondSuccess(result);
		}

		#endregion


		private static List<AttachmentListItem> mergeAttachmentIdList(List<AttachmentListItem> att_list, IEnumerable<UserTrackDetail> localUserTracks)
		{
			if (att_list == null || att_list.Count == 0)
				return localUserTracks.Select(x =>
					new AttachmentListItem
					{
						attachment_id = x.target_id,
						seq_num = 0,
						update_time = DateTime.UtcNow
					}).ToList();
			else
			{
				var result = localUserTracks.Select(x => new AttachmentListItem
					{
						attachment_id = x.target_id,
						seq_num = 0,
						update_time = DateTime.UtcNow
					}).ToList();
				result.AddRange(att_list);

				return result;
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
				                                 (int) StationLocalApiError.InvalidDriver);

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