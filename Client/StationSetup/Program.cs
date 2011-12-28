using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Wammer.Station;
using Waveface.Localization;
using Microsoft.Win32;

namespace StationSetup
{
    static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [STAThread]
        static void Main(string[] args)
        {
            string culture = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation", "Culture", null);
            if (culture == null)
                CultureManager.ApplicationUICulture = CultureInfo.CurrentCulture;
            else
                CultureManager.ApplicationUICulture = new CultureInfo(culture);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form form = null;

            if (args.Length == 1 && args[0].Equals("--dropbox"))
            {
                form = new DropboxForm();
            }
            else
            {
                if (WavefaceWindowsClientHelper.IsAlreadyResistered())
                {
                    WavefaceWindowsClientHelper.StartWavefaceWindowsClient("", "", "");
                    return;
                }

                form = new SignInForm();

                // force window to have focus
                // please refer http://stackoverflow.com/questions/278237/keep-window-on-top-and-steal-focus-in-winforms
                uint foreThread = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
                uint appThread = GetCurrentThreadId();
                const uint SW_SHOW = 5;
                if (foreThread != appThread)
                {
                    AttachThreadInput(foreThread, appThread, true);
                    BringWindowToTop(form.Handle);
                    ShowWindow(form.Handle, SW_SHOW);
                    AttachThreadInput(foreThread, appThread, false);
                }
                else
                {
                    BringWindowToTop(form.Handle);
                    ShowWindow(form.Handle, SW_SHOW);
                }
                form.Activate();
            }

            Application.Run(form);
        }
    }

    public class WavefaceWindowsClientHelper
    {
        public static bool IsAlreadyResistered()
        {
            return Wammer.Station.Management.StationController.GetOwner() != null;
        }

        public static void StartWavefaceWindowsClient(string email, string password, string token)
        {
            string ProgramDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string WavefaceWindowsClientPath = Path.Combine(ProgramDir, "WavefaceWindowsClient.exe");

            try
            {
                if ((email == string.Empty) || (password == string.Empty))
                {
                    Process p = Process.Start(WavefaceWindowsClientPath, null);
                    p.Close();
                }
                else
                {
                    Process p = Process.Start(WavefaceWindowsClientPath, email + " " + password + " " + token);
                    p.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to start Waveface Windows Client: " + e.Message, "Waveface",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
