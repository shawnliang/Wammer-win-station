using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.WindowsClient
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBrowserControl
	{
		#region Property
		Boolean IsDebugMode { get; set; }
		#endregion

		#region Method
		void Navigate(string uri);
        #endregion
    }
}
