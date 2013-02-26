using System;
using System.Diagnostics;
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
		if (System.Windows.Forms.SystemInformation.TerminalServerSession)
			return;

		System.Reflection.PropertyInfo aProp =
			  typeof(System.Windows.Forms.Control).GetProperty(
					"DoubleBuffered",
					System.Reflection.BindingFlags.NonPublic |
					System.Reflection.BindingFlags.Instance);

		aProp.SetValue(control, true, null);
	}
}
