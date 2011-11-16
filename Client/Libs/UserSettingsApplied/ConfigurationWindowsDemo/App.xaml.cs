// -- FILE ------------------------------------------------------------------
// name       : App.cs
// created    : Jani Giannoudis - 2008.04.28
// language   : c#
// environment: .NET 3.0
// --------------------------------------------------------------------------
using System;
using System.ComponentModel;

namespace Waveface.Solutions.Community.ConfigurationWindowsDemo
{

	// ------------------------------------------------------------------------
	public partial class App 
	{

		// ----------------------------------------------------------------------
		public App()
		{
			appSettings = new MyApplicationSettings( this );
		} // OnStartup

		// ----------------------------------------------------------------------
		protected override void OnActivated( EventArgs e )
		{
			base.OnActivated( e );
			ApplySettings();
		} // OnActivated

		// ----------------------------------------------------------------------
		private void ApplySettings()
		{
			if ( settingsApplied )
			{
				return;
			}

			// window settings
			UserSettingsWindow userSettingsWindow = Current.MainWindow as UserSettingsWindow;
			if ( userSettingsWindow != null )
			{
				userSettingsWindow.HostName = appSettings.HostName;
				userSettingsWindow.Closing += SettingsWindowClosing;
			}

			settingsApplied = true;
		} // ApplySettings

		// ----------------------------------------------------------------------
		private void SettingsWindowClosing( object sender, CancelEventArgs e )
		{
			UserSettingsWindow userSettingsWindow = Current.MainWindow as UserSettingsWindow;
			if ( userSettingsWindow != null )
			{
				appSettings.HostName = userSettingsWindow.HostName;
			}
		} // SettingsWindowClosing

		// ----------------------------------------------------------------------
		// members
		private readonly MyApplicationSettings appSettings;
		private bool settingsApplied;

	} // class App

} // namespace Itenso.Solutions.Community.ConfigurationWindowsDemo
// -- EOF -------------------------------------------------------------------
