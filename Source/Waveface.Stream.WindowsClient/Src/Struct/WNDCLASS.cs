using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#region Delegate
delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
#endregion

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
struct WNDCLASS
{
	public uint style;
	public WndProcDelegate lpfnWndProc;
	public int cbClsExtra;
	public int cbWndExtra;
	public IntPtr hInstance;
	public IntPtr hIcon;
	public IntPtr hCursor;
	public IntPtr hbrBackground;
	[MarshalAs(UnmanagedType.LPWStr)]
	public string lpszMenuName;
	[MarshalAs(UnmanagedType.LPWStr)]
	public string lpszClassName;
}