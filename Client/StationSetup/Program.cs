using System;
using System.Diagnostics;
using System.Windows.Forms;
using Wammer.Station;

namespace StationSetup
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (System.IO.File.Exists(@"Registered.dat"))
            {
                ProcessStartInfo _startInfo = new ProcessStartInfo();
                _startInfo.FileName = "WammerZ.exe";
                Process.Start(_startInfo);
                return;
            }

            Application.Run(new SignInForm());
        }
    }
}
