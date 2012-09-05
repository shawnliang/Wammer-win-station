using System;

namespace Waveface
{
	public class Env
	{
		/// <summary>
		/// Determines whether [is win vista or later].
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if [is win vista or later]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsWinVistaOrLater()
		{
			bool isWinVistaOrLater;

			var os = Environment.OSVersion;
			if (os.Platform == PlatformID.Win32NT && os.Version.Major == 6)
				isWinVistaOrLater = true;
			else
				isWinVistaOrLater = false;
			return isWinVistaOrLater;
		}
	}
}
