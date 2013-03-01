using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

public static class ControlExtension
{
	public static Boolean IsDesignMode(this Control control)
	{
		return Process.GetCurrentProcess().ProcessName.Equals("devenv", StringComparison.CurrentCultureIgnoreCase);
	}

	public static void SetDoubleBuffered(this Control control)
	{
		//Taxes: Remote Desktop Connection and painting
		//http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
		if (SystemInformation.TerminalServerSession)
			return;

		PropertyInfo prop =
			  typeof(Control).GetProperty(
					"DoubleBuffered",
					BindingFlags.NonPublic |
					BindingFlags.Instance);

		prop.SetValue(control, true, null);
	}
}
