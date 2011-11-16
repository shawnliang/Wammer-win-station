#region

using System;
using System.IO;
using System.Windows.Forms;

#endregion

[assembly: CLSCompliant(true)]

namespace Waveface
{
    internal static class Program
    {
        // context menu name in the registry
        private const string KeyName = "Wammer Context Menu";

        // context menu text
        private const string MenuText = "Send to Wammer";

        [STAThread]
        private static void Main(string[] args)
        {
            // process register or unregister commands
            if (!ProcessCommand(args))
            {
                // invoked from shell, process the selected file
                WriteFileNameToTempFile(args[0]);
            }
        }

        // Process command line actions (register or unregister).
        // True if processed an action in the command line.</returns>
        private static bool ProcessCommand(string[] args)
        {
            // register
            if ((args.Length == 0) || (string.Compare(args[0], "-register", true) == 0))
            {
                // full path to self, %L is placeholder for selected file
                string _menuCommand = string.Format("\"{0}\" \"%L\"", Application.ExecutablePath);

                // register the context menu
                FileShellExtension.Register("jpegfile", KeyName, MenuText, _menuCommand);
                FileShellExtension.Register("pngfile", KeyName, MenuText, _menuCommand);

                return true;
            }

            // unregister		
            if (string.Compare(args[0], "-unregister", true) == 0)
            {
                // unregister the context menu
                FileShellExtension.Unregister("jpegfile", KeyName);
                FileShellExtension.Unregister("pngfile", KeyName);

                return true;
            }

            // command line did not contain an action
            return false;
        }

        private static void WriteFileNameToTempFile(string filePath)
        {
            using (StreamWriter _outfile = new StreamWriter(Application.StartupPath + @"\ShellContextMenu.dat"))
            {
                _outfile.Write(filePath);
            }
        }
    }
}