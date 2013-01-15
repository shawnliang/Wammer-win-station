using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

public static class ControlExtension
{
	public static Boolean IsDesignMode(this Control control)
	{
		return Process.GetCurrentProcess().ProcessName.Equals("devenv", StringComparison.CurrentCultureIgnoreCase);
	}
}
