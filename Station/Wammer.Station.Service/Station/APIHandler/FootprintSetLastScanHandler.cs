using System;
using Wammer.Cloud;
using Wammer.Utility;

namespace Wammer.Station
{
	public class FootprintSetLastScanHandler : HttpHandler
	{
		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
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

			RespondSuccess(
				new FootprintSetLastScanResponse
				{
					last_scan = new LastScanInfo
					{
						timestamp = DateTime.UtcNow,
						user_id = Session.user.user_id,
						group_id = groupId,
						post_id = postId
					}
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