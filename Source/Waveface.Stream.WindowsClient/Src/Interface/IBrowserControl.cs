using System;

namespace Waveface.Stream.WindowsClient
{
	/// <summary>
	/// 
	/// </summary>
	public interface IBrowserControl
	{
		#region Property
		String Uri { get; set; }
		Boolean IsDebugMode { get; set; }
		#endregion

		#region Method
		void Navigate(string uri);
		#endregion
	}
}
