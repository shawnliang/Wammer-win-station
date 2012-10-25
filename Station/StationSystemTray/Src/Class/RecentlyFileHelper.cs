using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

public class RecentlyFileHelper
{
	public static string GetShortcutTargetFile(string shortcutFilename)
	{
		var type = Type.GetTypeFromProgID("WScript.Shell");
		object instance = Activator.CreateInstance(type);
		var result = type.InvokeMember("CreateShortCut", BindingFlags.InvokeMethod, null, instance, new object[] { shortcutFilename });
		var targetFile = result.GetType().InvokeMember("TargetPath", BindingFlags.GetProperty, null, result, null) as string;

		return targetFile;
	}

	public static IEnumerable<string> GetRecentlyFiles()
	{
		var recentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
		return from file in (new DirectoryInfo(recentFolder)).EnumerateFiles("*.lnk")
			   let targetFile = GetShortcutTargetFile(file)
			   where targetFile.Length != 0
			   select GetShortcutTargetFile(targetFile);
	}
}