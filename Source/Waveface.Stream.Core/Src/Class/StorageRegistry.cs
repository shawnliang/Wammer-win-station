using Microsoft.Win32;
using System;

namespace Waveface.Stream.Core
{
	public class StorageRegistry
	{
		const string KEY_BASE = @"HKEY_CURRENT_USER\Software\AOStream\Users\";
		const string STORAGE_PATH_NAME = "storagePath";

		public static void ClearAll()
		{

			var key = openAostreamKey();


			if (key == null)
				return;

			deleteSubKeyTreeNoThrow(key, "Users");

			key.Close();
		}

		private static void deleteSubKeyTreeNoThrow(RegistryKey key, string subkey)
		{
			try
			{
				key.DeleteSubKeyTree(subkey);
			}
			catch (ArgumentException)
			{
				// subkey does not exist.
			}
		}

		private static RegistryKey openAostreamKey()
		{
			RegistryKey softwareKey = null;
			try
			{

				softwareKey = Registry.CurrentUser.OpenSubKey(@"Software");
				if (softwareKey == null)
					return null;

				return softwareKey.OpenSubKey("AOStream", true);
			}
			finally
			{
				if (softwareKey != null)
					softwareKey.Close();
			}
		}

		public static string QueryStorageLocation(string user_id)
		{
			return (string)Registry.GetValue(KEY_BASE + user_id, STORAGE_PATH_NAME, null);
		}

		public static void Save(string user_id, string storagePath)
		{
			Registry.SetValue(KEY_BASE + user_id, STORAGE_PATH_NAME, storagePath);
		}

		public static void Remove(string userID)
		{
			var aostreamKey = openAostreamKey();
			if (aostreamKey == null)
				return;

			var usersKey = aostreamKey.OpenSubKey("Users", true);
			if (usersKey == null)
				return;

			deleteSubKeyTreeNoThrow(usersKey, userID);
			usersKey.Close();
			aostreamKey.Close();
		}
	}
}