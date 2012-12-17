using System;
using System.Collections.Generic;
using System.Reflection;
using Waveface.Stream.Model;

namespace Waveface.Stream.Core
{
	[Obfuscation]
	public class UnSubscribeEventCommand : WebSocketCommandBase
	{
		#region Public Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "unSubscribeEvent"; }
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
			var parameters = data.Parameters;

			var sessionToken = parameters.ContainsKey("session_token") ? parameters["session_token"].ToString() : string.Empty;

			if (sessionToken.Length == 0)
				return null;

			var loginedUser = LoginedSessionCollection.Instance.FindOneById(sessionToken);

			if (loginedUser == null)
				return null;

			var eventID = parameters.ContainsKey("event_id") ? int.Parse(parameters["event_id"].ToString()) : 0;
			var systemEvent = (SystemEventType)eventID;

			//var subscribedEvents = LoginController.Instance.LoginedUser.SubscribedEvents;

			//if (systemEvent == SystemEventType.All)
			//{
			//	data.Parameters.Clear();
			//	foreach (SystemEventType systemEventType in Enum.GetValues(typeof(SystemEventType)))
			//	{
			//		if (subscribedEvents.ContainsKey(systemEventType))
			//			subscribedEvents.Remove(systemEventType);
			//	}
			//	return null;
			//}

			//if (subscribedEvents.ContainsKey(systemEvent))
			//	subscribedEvents.Remove(systemEvent);

			return null;
		}
		#endregion
	}
}
