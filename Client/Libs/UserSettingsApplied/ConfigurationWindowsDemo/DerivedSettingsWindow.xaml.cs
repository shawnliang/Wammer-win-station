// -- FILE ------------------------------------------------------------------
// name       : DerivedSettingsWindow.cs
// created    : Jani Giannoudis - 2008.04.30
// language   : c#
// environment: .NET 3.0
// --------------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Waveface.Configuration;

namespace Waveface.Solutions.Community.ConfigurationWindowsDemo
{

	// ------------------------------------------------------------------------
	public partial class DerivedSettingsWindow
	{

		// ----------------------------------------------------------------------
		public DerivedSettingsWindow()
		{
			// save all window textbox values to the settings using the class property
			WindowSettings.SettingCollectors.Add( 
				new PropertySettingCollector(
					this,              // owner
					typeof( TextBox ), // element type
					"Text" ) );        // element property

			// save all window cehckbox values to the settings using the dependency property
			WindowSettings.SettingCollectors.Add( 
				new DependencyPropertySettingCollector( 
					this,                           // owner
					ToggleButton.IsCheckedProperty ) ); // dependency property

			InitializeComponent();
		} // DerivedSettingsWindow

		// ----------------------------------------------------------------------
		protected override void OnCollectingSetting( SettingCollectorCancelEventArgs e )
		{
			FrameworkElement frameworkElement = e.Element as FrameworkElement;
			if ( frameworkElement == null )
			{
				return;
			}

			// exclusion rules
			if ( Option3.Name.Equals( frameworkElement.Name ) || Edit3.Name.Equals( frameworkElement.Name ) )
			{
				e.Cancel = true;
			}
		} // OnCollectingSetting

	} // class DerivedSettingsWindow

} // namespace Itenso.Solutions.Community.ConfigurationWindowsDemo
// -- EOF -------------------------------------------------------------------
