using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using Wammer.Cloud;

namespace Wammer.Station.LocalUserTrack
{
	public class LocalUserTrackManager
	{
		private Dictionary<string, LocalUserTrackCollection> groupUserTracks = new Dictionary<string, LocalUserTrackCollection>();
		private static DateTime VERY_OLD_TIMESTAMP = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public void OnAttachmentReceived(object sender, Wammer.Station.AttachmentUpload.AttachmentEventArgs args)
		{
			try
			{
				if (args.ImgMeta != ImageMeta.Medium)
					return;

				AddUserTrack(args.GroupId, args.AttachmentId, args.PostId);
			}
			catch (Exception e)
			{
				this.GetLogger().Warn("Unable to generate local user tracks", e);
			}
		}
		
		public void OnThumbnailGenerated(object sender, ThumbnailEventArgs args)
		{
			try
			{
				if (args.meta != ImageMeta.Medium)
					return;

				AddUserTrack(args.group_id, args.object_id, args.post_id);
			}
			catch (Exception e)
			{
				this.GetLogger().Warn("Unable to generate local user tracks", e);
			}
		}

		public void OnAttachmentUpstreamed(object sender, ThumbnailEventArgs args)
		{
			try
			{
				if (args.meta != ImageMeta.Medium)
					return;

				RemoveUserTrack(args.group_id, args.object_id);
			}
			catch (Exception e)
			{
				this.GetLogger().Warn("Unable to remove local user tracks", e);
			}
		}

		public void RemoveUserTrack(string group_id, string object_id)
		{
			var ut = new UserTrackDetail
			{
				target_id = object_id
			};


			LocalUserTrackCollection groupData;

			lock (groupUserTracks)
			{
				if (groupUserTracks.ContainsKey(group_id))
					groupData = groupUserTracks[group_id];
				else
					return;
			}

			lock (groupData)
			{
				groupData.Remove(ut);
			}
		}

		public void AddUserTrack(string group_id, string object_id, string post_id)
		{
			var ut = new UserTrackDetail
			{
				group_id = group_id,
				target_id = object_id,
				target_type = "attachment",
				timestamp = VERY_OLD_TIMESTAMP,
				actions = new List<UserTrackAction>{
					new UserTrackAction {
						action = "add",
						target_type = "image.medium",
						post_id = post_id
					}
				}
			};

			LocalUserTrackCollection groupData;

			lock (groupUserTracks)
			{
				if (!groupUserTracks.ContainsKey(group_id))
				{
					groupData = new LocalUserTrackCollection();
					groupUserTracks.Add(group_id, groupData);
				}
				else
					groupData = groupUserTracks[group_id];
			}


			lock (groupData)
			{
				groupData.Add(ut);
			}
		}

		public IEnumerable<UserTrackDetail> getUserTracksBySession(string group_id, string session_token)
		{
			LocalUserTrackCollection groupData;

			lock (groupUserTracks)
			{
				if (!groupUserTracks.ContainsKey(group_id))
					return new List<UserTrackDetail>();
				else
					groupData = groupUserTracks[group_id];
			}


			lock (groupData)
			{
				return groupData.GetUserTracksBySession(group_id, session_token);
			}
		}

	}
}
