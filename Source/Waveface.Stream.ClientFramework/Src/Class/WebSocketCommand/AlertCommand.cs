using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using System.Windows.Forms;
using System.Reflection;

namespace Waveface.Stream.ClientFramework
{
    [Obfuscation]
	public class AlertCommand : WebSocketCommandBase
	{
		#region Public Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "alert"; }
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Executes the specified parameters.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
        public override Dictionary<string, Object> Execute(WebSocketCommandData data)
		{
            var parameters = data.Parameters;

            SynchronizationContextHelper.SendMainSyncContext(() =>
                {
                    MessageBox.Show(parameters["message"].ToString());
                }
             );
            return null;
		}
		#endregion
	}
}
