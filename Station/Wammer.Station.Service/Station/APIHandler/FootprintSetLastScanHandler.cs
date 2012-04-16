using System;
using System.Linq;
using Wammer.Station;
using Wammer.Model;
using Wammer.Cloud;
using Wammer.Utility;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	public class FootprintSetLastScanHandler : HttpHandler
	{
		#region Private Property
		private string m_StationID { get; set; }
		private string m_ResourceBasePath { get; set; }
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="FootprintSetLastScanHandler"/> class.
		/// </summary>
		/// <param name="stationId">The station id.</param>
		/// <param name="resourceBasePath">The resource base path.</param>
		public FootprintSetLastScanHandler(string stationId = null, string resourceBasePath = null)
		{
			this.m_StationID = stationId;
			this.m_ResourceBasePath = resourceBasePath;
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