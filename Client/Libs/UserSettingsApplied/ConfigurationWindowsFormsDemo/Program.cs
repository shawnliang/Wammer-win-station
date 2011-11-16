
using System;
using System.Windows.Forms;

namespace Waveface.Solutions.Community.ConfigurationWindowsFormsDemo
{
	public class Program
	{
        private static readonly MyApplicationSettings s_appSettings = new MyApplicationSettings();

		[STAThread]
		static void Main()
		{
			s_appSettings.Load();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );

			SettingsForm _settingsForm = new SettingsForm();
			_settingsForm.FormClosed += SettingsFormClosed;
			_settingsForm.HostName = s_appSettings.HostName;
			Application.Run( _settingsForm );

			s_appSettings.Save();
		} 

		static void SettingsFormClosed( object sender, FormClosedEventArgs e )
		{
			s_appSettings.HostName = ((SettingsForm)sender).HostName;
		} 
	}
}
