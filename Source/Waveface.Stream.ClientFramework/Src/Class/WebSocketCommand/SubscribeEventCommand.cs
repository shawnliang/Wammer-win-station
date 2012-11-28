using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using System.Windows.Forms;
using System.Diagnostics;
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
		/// <param name="parameters">The parameters.</param>
        public override Dictionary<string, Object> Execute(Dictionary<string, Object> parameters = null)
		{
            var eventID = parameters.ContainsKey("event_id") ? int.Parse(parameters["event_id"].ToString()) : 0;
            var systemEvent = (SystemEventType)eventID;

            var subscribedEvents = StreamClient.Instance.LoginedUser.SubscribedEvents;

            if (systemEvent == SystemEventType.All)
            {
                foreach (SystemEventType systemEventType in Enum.GetValues(typeof(SystemEventType)))
                {
                    if (subscribedEvents.ContainsKey(systemEventType))
                        subscribedEvents.Remove(systemEventType);

                    subscribedEvents.Add(systemEventType, null);
                }
                return null;
            }

            if (subscribedEvents.ContainsKey(systemEvent))
                subscribedEvents.Remove(systemEvent);

            subscribedEvents.Add(systemEvent, parameters);

            return null;
		}
		#endregion
	}
}
