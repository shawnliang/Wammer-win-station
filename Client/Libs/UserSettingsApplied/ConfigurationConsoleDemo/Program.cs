
#region

using System;
using Waveface.Configuration;

#endregion

namespace Waveface.Solutions.Community.ConfigurationConsoleDemo
{
    internal class Program
    {
        [PropertySetting(DefaultValue = -1)]
        public int StatusCode { get; set; }

        public void Execute()
        {
            // setup and load user settings
            ApplicationSettings _settings = new ApplicationSettings(this);
            _settings.Load();

            // show startup values
            Console.WriteLine("Status-Code value: " + StatusCode);
            Console.WriteLine("Status-Code setting: " + _settings["StatusCode"]);
            Console.WriteLine();

            // user input
            Console.Write("Please enter a number (Enter to reset): ");
            
            string _consoleInput = Console.ReadLine();
            
            if (string.IsNullOrEmpty(_consoleInput))
            {
                _settings.Reset();
            }
            else
            {
                try
                {
                    StatusCode = int.Parse(_consoleInput); // modifying the field value
                }
                catch
                {
                    Console.Write("Invalid input. Press any key...");
                    Console.ReadKey();
                    return;
                }
            }

            // save user settings
            _settings.Save();
            Console.WriteLine();
            Console.WriteLine("Settings saved to: " + ApplicationSettings.UserConfigurationFilePath);

            // show working values
            Console.WriteLine();
            Console.WriteLine("Status-Code value: " + StatusCode);
            Console.WriteLine("Status-Code setting: " + _settings["StatusCode"]);

            Console.WriteLine();
            Console.Write("Press any key...");
            Console.ReadKey();
        }

        private static void Main()
        {
            new Program().Execute();
        }
    }
}