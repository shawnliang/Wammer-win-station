using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ServiceProcess;

namespace Wammer.Utility
{
	public static class ServiceHelper
	{
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern Boolean ChangeServiceConfig(
			IntPtr hService,
			UInt32 nServiceType,
			UInt32 nStartType,
			UInt32 nErrorControl,
			String lpBinaryPathName,
			String lpLoadOrderGroup,
			IntPtr lpdwTagId,
			[In] char[] lpDependencies,
			String lpServiceStartName,
			String lpPassword,
			String lpDisplayName);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern Boolean QueryServiceConfig(IntPtr hService, IntPtr intPtrQueryConfig, UInt32 cbBufSize, out UInt32 pcbBytesNeeded);

		[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern IntPtr OpenService(
			IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

		[DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr OpenSCManager(
			string machineName, string databaseName, uint dwAccess);

		private const uint SERVICE_NO_CHANGE = 0xFFFFFFFF;
		private const uint SERVICE_QUERY_CONFIG = 0x00000001;
		private const uint SERVICE_CHANGE_CONFIG = 0x00000002;
		private const uint SERVICE_AUTO_START = 0x00000002;

		private const uint SC_MANAGER_ALL_ACCESS = 0x000F003F;
		private const uint SC_MANAGER_CONNECT = 0x00000001;
		private const uint SC_MANAGER_ENUMERATE_SERVICE = 0x00000004;

		public static void ChangeStartMode(ServiceController svc, ServiceStartMode mode)
		{
			var scManagerHandle = OpenSCManager(null, null, SC_MANAGER_CONNECT | SC_MANAGER_ENUMERATE_SERVICE);
			if (scManagerHandle == IntPtr.Zero)
			{
				throwWin32Exception("OpenSCManager: ");
			}

			var serviceHandle = OpenService(
				scManagerHandle,
				svc.ServiceName,
				SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);

			if (serviceHandle == IntPtr.Zero)
			{
				throwWin32Exception("OpenService: ");
			}

			var result = ChangeServiceConfig(
				serviceHandle,
				SERVICE_NO_CHANGE,
				(uint)mode,
				SERVICE_NO_CHANGE,
				null,
				null,
				IntPtr.Zero,
				null,
				null,
				null,
				null);

			if (result == false)
			{
				throwWin32Exception("ChangeServiceConfig: ");
			}
		}

		private static void throwWin32Exception(string msg)
		{
			int nError = Marshal.GetLastWin32Error();
			var win32Exception = new Win32Exception(nError);
			throw new ExternalException(msg + win32Exception.Message);
		}

		public static bool IsServiceAutoStart(ServiceController svc)
		{
			var scManagerHandle = OpenSCManager(null, null, SC_MANAGER_CONNECT | SC_MANAGER_ENUMERATE_SERVICE);
			if (scManagerHandle == IntPtr.Zero)
			{
				throwWin32Exception("OpenSCManager: ");
			}

			var serviceHandle = OpenService(
				scManagerHandle,
				svc.ServiceName,
				SERVICE_QUERY_CONFIG);

			if (serviceHandle == IntPtr.Zero)
			{
				throwWin32Exception("OpenService: ");
			}

			// Allocate memory for struct.
			uint bytesNeeded = 0;
			QueryServiceConfig(serviceHandle, IntPtr.Zero, 0, out bytesNeeded);

			IntPtr ptr = IntPtr.Zero;
			try
			{
				ptr = Marshal.AllocHGlobal((int)bytesNeeded);
				bool result = QueryServiceConfig(serviceHandle, ptr, bytesNeeded, out bytesNeeded);

				if (result == false)
				{
					throwWin32Exception("QueryServiceConfig: ");
				}

				QUERY_SERVICE_CONFIG qUERY_SERVICE_CONFIG = new QUERY_SERVICE_CONFIG();
				// Copy 
				Marshal.PtrToStructure(ptr, qUERY_SERVICE_CONFIG);
				return qUERY_SERVICE_CONFIG.dwStartType == SERVICE_AUTO_START;
			}
			finally
			{
				// Free memory for struct.
				if (ptr!=IntPtr.Zero)
					Marshal.FreeHGlobal(ptr);
			}

		}

	}


	[StructLayout(LayoutKind.Sequential)]
	public class QUERY_SERVICE_CONFIG
	{
		[MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
		public UInt32 dwServiceType;
		[MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
		public UInt32 dwStartType;
		[MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
		public UInt32 dwErrorControl;
		[MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
		public String lpBinaryPathName;
		[MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
		public String lpLoadOrderGroup;
		[MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
		public UInt32 dwTagID;
		[MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
		public String lpDependencies;
		[MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
		public String lpServiceStartName;
		[MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
		public String lpDisplayName;
	};
}
