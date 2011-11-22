using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Station.Management
{
	public class StationController
	{
		/// <summary>
		/// Starts station services, including MongoDB service
		/// </summary>
		public static void StartServices();

		/// <summary>
		/// Stops station services, including MongoDB service
		/// </summary>
		public static void StopServices();

		/// <summary>
		/// Gets station service status
		/// </summary>
		public static System.ServiceProcess.ServiceControllerStatus ServiceStatus
		{
			get;
		}

		/// <summary>
		/// Sets whether station service auto starts itself after system boots
		/// </summary>
		/// <param name="autoStart"></param>
		public static void SetServiceAutoStart(bool autoStart);

		/// <summary>
		/// Gets if auto-start of station service is enabled
		/// </summary>
		/// <returns></returns>
		public static bool IsServiceAutoStartEnabled();

		/// <summary>
		/// Gets owner's email
		/// </summary>
		/// <returns>Owner's email. If owner is not set yet, null is returned.</returns>
		public static string GetOwner();

		/// <summary>
		/// Gets default folder to save attachments
		/// </summary>
		/// <remarks>If the owner of station is not set yet, null is returned.</remarks>
		/// <returns></returns>
		public static string GetDefaultFolder();

		/// <summary>
		/// Move default folder to another location
		/// </summary>
		/// <remarks>
		/// Moving the default folder does these internally :
		/// - stop station service
		/// - write new folder location to database
		/// - move folder to the destination
		/// - start station service again
		/// 
		/// This method must be called after station owner is set. Otherwise, InvalidOperation is 
		/// thrown.
		/// </remarks>
		/// <param name="absPath">absolute path to user's folder. The folder must be empty</param>
		/// <exception cref="System.ArgumentException">
		/// absPath is not an absolute path, or it is not an empty folder
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The station's owner is not set yet.
		/// </exception>
		public static void MoveDefaultFolder(string absPath);
	}

}
