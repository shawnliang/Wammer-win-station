using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Core
{
	public class SystemEventEventArgs : EventArgs
	{
		#region Property
		/// <summary>
		/// Gets the data.
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		public object Data { get; private set; }
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="SystemEventEventArgs" /> class.
		/// </summary>
		/// <param name="data">The data.</param>
		public SystemEventEventArgs(object data)
		{
			this.Data = data;
		}
		#endregion
	}
}
