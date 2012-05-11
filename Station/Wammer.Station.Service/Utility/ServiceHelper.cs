using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
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

		private const uint SC_MANAGER_CONNECT = 0x00000001;
		private const uint SC_MANAGER_ENUMERATE_SERVICE = 0x00000004;


		#region Private Method
		private static void throwWin32Exception(string msg)
		{
			int nError = Marshal.GetLastWin32Error();
			var win32Exception = new Win32Exception(nError);
			throw new ExternalException(msg + win32Exception.Message);
		}

		private static IntPtr GetServiceHandle(ServiceController svc, uint dwDesiredAccess)
		{
			var scManagerHandle = OpenSCManager(null, null, SC_MANAGER_CONNECT | SC_MANAGER_ENUMERATE_SERVICE);
			if (scManagerHandle == IntPtr.Zero)
			{
				throwWin32Exception("OpenSCManager: ");
			}

			var serviceHandle = OpenService(
				scManagerHandle,
				svc.ServiceName,
				dwDesiredAccess);

			if (serviceHandle == IntPtr.Zero)
			{
				throwWin32Exception("OpenService: ");
			}

			return serviceHandle;
		}
		#endregion


		public static void ChangeStartMode(ServiceController svc, ServiceStartMode mode)
		{
			var serviceHandle = GetServiceHandle(svc, SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);

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

		public static bool IsServiceAutoStart(ServiceController svc)
		{
			var serviceHandle = GetServiceHandle(svc, SERVICE_QUERY_CONFIG);

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

				var qUERY_SERVICE_CONFIG = new QUERY_SERVICE_CONFIG();
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
