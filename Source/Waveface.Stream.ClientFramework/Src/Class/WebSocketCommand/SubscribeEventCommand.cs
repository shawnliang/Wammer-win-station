using System;
using System.Collections.Generic;
using System.Reflection;

namespace Waveface.Stream.ClientFramework
{
	[Obfuscation]
	public class SubscribeEventCommand : WebSocketCommandBase
	{
		#region Public Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "subscribeEvent"; }
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Executes the specified parameters.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public override Dictionary<string, Object> Execute(WebSocketCommandData data)
		{
			if (!StreamClient.Instance.IsLogined)
				return null;

			var parameters = data.Parameters;

			var eventID = parameters.ContainsKey("event_id") ? int.Parse(parameters["event_id"].ToString()) : 0;
			var systemEvent = (SystemEventType)eventID;

			var subscribedEvents = StreamClient.Instance.LoginedUser.SubscribedEvents;

			if (systemEvent == SystemEventType.All)
			{
				data.Parameters.Clear();
				foreach (SystemEventType systemEventType in Enum.GetValues(typeof(SystemEventType)))
				{
					if (subscribedEvents.ContainsKey(systemEventType))
						subscribedEvents.Remove(systemEventType);

					subscribedEvents.Add(systemEventType, data);
				}
				return null;
			}

			if (subscribedEvents.ContainsKey(systemEvent))
				subscribedEvents.Remove(systemEvent);

			subscribedEvents.Add(systemEvent, data);

			return null;
		}
		#endregion
	}
}
