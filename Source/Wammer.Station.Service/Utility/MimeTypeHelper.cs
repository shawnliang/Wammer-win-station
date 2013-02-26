using Microsoft.Win32;
using System.IO;

namespace Wammer.Utility
{
	public static class MimeTypeHelper
	{
		/// <summary>
		/// Gets the type of the MIME.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns></returns>
		public static string GetMIMEType(string file)
		{
			var extension = Path.GetExtension(file);
			using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(extension))
			{
				if (registryKey == null)
					return null;
				var value = registryKey.GetValue("Content Type");
				return (value == null) ? "application/unknown" : value.ToString();
			}
		}
	}
}
