using System;
using System.Linq;
using Wammer.Station;
using Wammer.Model;
using Wammer.Cloud;
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
			CheckParameter("group_id", "post_id");

			string groupId = Parameters["group_id"];
			string postId = Parameters["post_id"];

			RespondSuccess(
				new FootprintSetLastScanResponse
				{
					last_scan = new LastScanInfo
					{
						timestamp = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ssZ"),
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