using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using Wammer.Cloud;

namespace Wammer.Station.LocalUserTrack
{
	class LocalUserTrackManager
	{
		Dictionary<string, LocalUserTrackCollection> groupUserTracks = new Dictionary<string, LocalUserTrackCollection>();

		public void OnAttachmentReceived(object sender, Wammer.Station.AttachmentUpload.AttachmentEventArgs args)
		{
			try
			{
				if (args.ImgMeta != ImageMeta.Medium)
					return;

				AddMediumImageAddedUserTrack(args.GroupId, args.AttachmentId, args.PostId);
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

				AddMediumImageAddedUserTrack(args.group_id, args.object_id, args.post_id);
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

		private void RemoveUserTrack(string group_id, string object_id)
		{
			var ut = new UserTrackDetail
			{
				target_id = object_id
			};

			lock (groupUserTracks)
			{
				if (groupUserTracks.ContainsKey(group_id))
					groupUserTracks[group_id].Remove(ut);
			}
		}

		private void AddMediumImageAddedUserTrack(string group_id, string object_id, string post_id)
		{
			var ut = new UserTrackDetail
			{
				group_id = group_id,
				target_id = object_id,
				target_type = "attachment",
				timestamp = DateTime.UtcNow,
				actions = new List<UserTrackAction>{
					new UserTrackAction {
						action = "add",
						target_type = "image.medium",
						post_id = post_id
					}
				}
			};

			lock (groupUserTracks)
			{
				if (!groupUserTracks.ContainsKey(group_id))
					groupUserTracks.Add(group_id, new LocalUserTrackCollection());

				groupUserTracks[group_id].Add(ut);
			}
		}
	}
}
