using System;

namespace Waveface.Stream.Model
{
	public class CollectionChangedEventArgs : EventArgs
	{
		#region Property
		public string ID { get; private set; }
		#endregion

		#region Constructor
		public CollectionChangedEventArgs(string id)
		{
			this.ID = id;
		}
		#endregion
	}
}
