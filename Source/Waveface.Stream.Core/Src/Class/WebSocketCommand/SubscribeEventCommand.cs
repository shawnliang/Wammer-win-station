using System;
using System.Collections.Generic;
using System.Reflection;
using Waveface.Stream.Model;

namespace Waveface.Stream.Core
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
		public override Dictionary<string, Object> Execute(WebSocketCommandData data, Dictionary<string, Object> systemArgs = null)
		{
			var parameters = data.Parameters;

			var channelID = (systemArgs != null && systemArgs.ContainsKey("ChannelID")) ? systemArgs["ChannelID"].ToString() : string.Empty;

			var sessionToken = parameters.ContainsKey("session_token") ? parameters["session_token"].ToString() : string.Empty;

			if (sessionToken.Length == 0)
				return null;

			var loginedUser = LoginedSessionCollection.Instance.FindOneById(sessionToken);

			if (loginedUser == null)
				return null;

			var eventID = parameters.ContainsKey("event_id") ? int.Parse(parameters["event_id"].ToString()) : 0;
			var eventType = (SystemEventType)eventID;

			SystemEventSubscriber.Instance.Subscribe(channelID, eventType, data);

			return null;
		}
		#endregion
	}
}
