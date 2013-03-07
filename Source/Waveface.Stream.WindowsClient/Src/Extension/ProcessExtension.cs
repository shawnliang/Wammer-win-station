using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public static class ProcessExtension
{
	public static void WaitForFormLoad(this Process process)
	{
		process.WaitForInputIdle();
		while (!process.HasExited && process.MainWindowHandle == IntPtr.Zero)
		{
			Application.DoEvents();
			Thread.Sleep(100);
		}
	}

	public static string GetProcessOwner(this Process process)
	{
		var query = "Select * From Win32_Process Where ProcessID = " + process.Id;
		var searcher = new ManagementObjectSearcher(query);
		var processList = searcher.Get().OfType<ManagementObject>();

		foreach (var obj in processList)
		{
			var argList = new string[2];
			int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
			if (returnVal == 0)
			{
				return string.Join(@"\", argList.Reverse().ToArray());
			}
		}

		return null;
	}

	public static void SafeClose(this Process process, int wait)
	{
		process.CloseMainWindow();
		process.WaitForExit(wait);

		if (process != null && !process.HasExited)
			process.Kill();
	}
}
