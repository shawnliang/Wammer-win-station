using System;
using System.Linq;

using System;
using System.Linq;
using Wammer.Station;
using Wammer.Cloud;
using System.Net;
using MongoDB.Driver.Builders;
using Wammer.Model;

namespace Wammer.Station
{
	public class UserLoginHandler : HttpHandler
	{		
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
			CheckParameter("email", "password");

			string email = Parameters["email"];
			
			Driver existingDriver = Model.DriverCollection.Instance.FindOne(Query.EQ("email", email));
			Boolean isDriverExists = existingDriver != null;

			//Driver not exists => can't user login => return
			if (!isDriverExists)
				return;

			string password = Parameters["password"];
			using (WebClient client = new WebClient())
			{
				User.LogIn(client, email, password);
			}
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