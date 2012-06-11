using System;
using System.Linq;

using System;
using System.Linq;
using Wammer.Station;
using System.IO;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace Wammer.Station.APIHandler
{
	public class MoveResourceFolderHandler : HttpHandler
	{
		#region constructor
		public MoveResourceFolderHandler()
		{
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
		public override void HandleRequest()
		{
			CheckParameter("folder");

			string newLocation = Parameters["folder"];

			if (!Directory.Exists(newLocation))
				throw new WammerStationException("folder not exist", (int)StationLocalApiError.NotFound);

			bool isSyncEnabled = Station.Instance.IsSynchronizationStatus;

			try
			{
				if (isSyncEnabled)
					Station.Instance.Stop();

				string oldLocation = FileStorage.ResourceFolder;

				foreach (var user in DriverCollection.Instance.FindAll())
				{
					string oldUserFolder = Path.Combine(oldLocation, user.folder);
					string name = Path.GetFileName(oldUserFolder);
					Directory.Move(oldUserFolder, Path.Combine(newLocation, name));

					DriverCollection.Instance.Update(
						Query.EQ("_id", user.user_id),
						Update.Set("folder", name));
				}

				FileStorage.ResourceFolder = newLocation;
			}
			finally
			{
				if (isSyncEnabled)
					Station.Instance.Start();
			}

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