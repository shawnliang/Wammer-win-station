using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Core
{
	public class SystemEventSubscribeEventArgs : EventArgs
	{
		#region Property
		/// <summary>
		/// Gets the web socket channel ID.
		/// </summary>
		/// <value>
		/// The web socket channel ID.
		/// </value>
		public String WebSocketChannelID { get; private set; }

		/// <summary>
		/// Gets the type of the event.
		/// </summary>
		/// <value>
		/// The type of the event.
		/// </value>
		public SystemEventType EventType { get; private set; }

		/// <summary>
		/// Gets the data.
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		public WebSocketCommandData Data { get; private set; }
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="SystemEventEventArgs" /> class.
		/// </summary>
		/// <param name="webSocketChannelID">The web socket channel ID.</param>
		/// <param name="eventType">Type of the event.</param>
		/// <param name="data">The data.</param>
		public SystemEventSubscribeEventArgs(string webSocketChannelID, SystemEventType eventType, WebSocketCommandData data)
		{
			this.WebSocketChannelID = webSocketChannelID;
			this.EventType = eventType;
			this.Data = data;
		}
		#endregion
	}
}
