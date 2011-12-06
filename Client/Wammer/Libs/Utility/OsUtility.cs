#region

using System;
using System.Runtime.InteropServices;

#endregion

namespace Waveface
{
    internal class OsUtility
    {
        public static bool Is64BitProcess
        {
            get { return IntPtr.Size == 8; }
        }

        public static bool Is64BitOperatingSystem
        {
            get
            {
                // Clearly if this is a 64-bit process we must be on a 64-bit OS.
                if (Is64BitProcess)
                    return true;

                // Ok, so we are a 32-bit process, but is the OS 64-bit?
                // If we are running under Wow64 than the OS is 64-bit.
                bool _isWow64;

                return ModuleContainsFunction("kernel32.dll", "IsWow64Process") &&
                       IsWow64Process(GetCurrentProcess(), out _isWow64) && _isWow64;
            }
        }

        private static bool ModuleContainsFunction(string moduleName, string methodName)
        {
            IntPtr _hModule = GetModuleHandle(moduleName);
            
            if (_hModule != IntPtr.Zero)
                return GetProcAddress(_hModule, methodName) != IntPtr.Zero;

            return false;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] out bool isWow64);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string methodName);
    }
}