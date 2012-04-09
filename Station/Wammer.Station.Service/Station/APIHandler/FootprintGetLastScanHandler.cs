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
		#region Private Property
		private string m_StationID { get; set; }
		private string m_ResourceBasePath { get; set; }
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="FootprintGetLastScanHandler"/> class.
		/// </summary>
		/// <param name="stationId">The station id.</param>
		/// <param name="resourceBasePath">The resource base path.</param>
		public FootprintGetLastScanHandler(string stationId = null, string resourceBasePath = null)
		{
			this.m_StationID = stationId;
			this.m_ResourceBasePath = resourceBasePath;
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