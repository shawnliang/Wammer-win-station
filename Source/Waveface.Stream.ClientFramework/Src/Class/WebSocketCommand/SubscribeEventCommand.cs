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

            if (systemEvent == SystemEventType.All)
            {
                StreamClient.Instance.LoginedUser.SubscribedSystemEvent = SystemEventType.All;

                return null;
            }

            if (StreamClient.Instance.LoginedUser.SubscribedSystemEvent == SystemEventType.None)
            {
                StreamClient.Instance.LoginedUser.SubscribedSystemEvent = systemEvent;
            }
            else
            {
                StreamClient.Instance.LoginedUser.SubscribedSystemEvent |= systemEvent; 
            }
            return null;
		}
		#endregion
	}
}
