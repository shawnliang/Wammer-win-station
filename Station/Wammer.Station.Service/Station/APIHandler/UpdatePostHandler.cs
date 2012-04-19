using System;
using System.Linq;

using System;
using System.Linq;
using Wammer.Station;

namespace Wammer.Station.APIHandler
{
	public class UpdatePostHandler : HttpHandler
	{
		#region Protected Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		protected override void HandleRequest()
		{
			//TODO: USe CheckParameter function to check input parameter
			//e.x. CheckParameter("email", "password");

			//TODO: API handel process

			RespondSuccess();
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