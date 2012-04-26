using System;
using System.Linq;
using Wammer.Station;
using Wammer.Model;
using Wammer.Cloud;
using Wammer.Utility;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	public class FootprintGetLastScanHandler : HttpHandler
	{	

		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
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

			MongoCursor<PostInfo> posts = PostCollection.Instance
				.Find(Query.And(Query.EQ("group_id", groupId), Query.EQ("hidden", "false")))
				.SetSortOrder(SortBy.Descending("timestamp"));

			LastScanInfo lastScan;
			if (posts.Count() > 0)
			{
				lastScan = new LastScanInfo
					{
						timestamp = posts.First().timestamp,
						user_id = Session.user.user_id,
						group_id = groupId,
						post_id = posts.First().post_id
					};
			}
			else
			{
				lastScan = new LastScanInfo();
			}

			RespondSuccess(new FootprintSetLastScanResponse { last_scan = lastScan });
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