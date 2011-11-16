#region

using System;

#endregion

namespace Waveface.Component.DropableNotifyIcon
{
    internal static class OperatingSystem
    {
        public static bool GteWindows7
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT &&
                       Environment.OSVersion.Version >= new Version(6, 0, 7600);
            }
        }
    }
}