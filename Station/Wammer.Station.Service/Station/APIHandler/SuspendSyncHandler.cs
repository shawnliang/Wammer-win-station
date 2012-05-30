using System;

namespace Wammer.Station
{
	internal class SuspendSyncHandler : HttpHandler
	{
		#region Event
		public event EventHandler SyncSuspended; 
		#endregion


		#region Protected Method
		/// <summary>
		/// Called when [sync suspended].
		/// </summary>
		protected void OnSyncSuspended()
		{
			var handler = SyncSuspended;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion


		#region Public Method
		public override void HandleRequest()
		{
			Station.Instance.SuspendSync();
			RespondSuccess();

			OnSyncSuspended();
		} 
		#endregion
	}
}