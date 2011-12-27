using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Wammer.Station;
using Waveface.Localization;
using Microsoft.Win32;

namespace StationSetup
{
    static class Program
    {
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
