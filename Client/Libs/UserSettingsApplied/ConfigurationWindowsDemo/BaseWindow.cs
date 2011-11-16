// -- FILE ------------------------------------------------------------------
// name       : BaseWindow.cs
// created    : Jani Giannoudis - 2008.04.30
// language   : c#
// environment: .NET 3.0
// --------------------------------------------------------------------------
using System.Windows;
using Waveface.Configuration;

namespace Waveface.Solutions.Community.ConfigurationWindowsDemo
{

	// ------------------------------------------------------------------------
	public class BaseWindow : Window
	{

		// ----------------------------------------------------------------------
		public BaseWindow()
		{
			windowSettings = new WindowSettings( this );
			windowSettings.CollectingSetting += WindowCollectingSetting;
		} // BaseWindow

		// ----------------------------------------------------------------------
		public WindowSettings WindowSettings
		{
			get { return windowSettings; }
		} // WindowSettings

		// ----------------------------------------------------------------------
		protected virtual void OnCollectingSetting( SettingCollectorCancelEventArgs e )
		{
		} // OnCollectingSetting

		// ----------------------------------------------------------------------
		private void WindowCollectingSetting( object sender, SettingCollectorCancelEventArgs e )
		{
			OnCollectingSetting( e );
		} // WindowCollectingSetting

		// ----------------------------------------------------------------------
		// members
		private readonly WindowSettings windowSettings;

	} // class BaseWindow

} // namespace Itenso.Solutions.Community.ConfigurationWindowsDemo
// -- EOF -------------------------------------------------------------------
