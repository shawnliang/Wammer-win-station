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
		string query = "Select * From Win32_Process Where ProcessID = " + process.Id;
		ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
		ManagementObjectCollection processList = searcher.Get();

		foreach (ManagementObject obj in processList)
		{
			string[] argList = new string[] { string.Empty, string.Empty };
			int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
			if (returnVal == 0)
			{
				// return DOMAIN\user
				return argList[1] + "\\" + argList[0];
			}
		}

		return "NO OWNER";
	}

	public static void SafeClose(this Process process, int wait)
	{
		process.CloseMainWindow();
		process.WaitForExit(wait);

		if (process != null && !process.HasExited)
			process.Kill();
	}
}
