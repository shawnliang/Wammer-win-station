// -- FILE ------------------------------------------------------------------
// name       : SettingListBox.cs
// created    : Jani Giannoudis - 2008.04.28
// language   : c#
// environment: .NET 3.0
// --------------------------------------------------------------------------
using System.Windows.Controls;
using System.ComponentModel;
using Waveface.Configuration;

namespace Waveface.Solutions.Community.ConfigurationWindowsDemo
{

	// ------------------------------------------------------------------------
	public class SettingListBox : ListBox
	{

		// ----------------------------------------------------------------------
		public SettingListBox()
		{
			if ( DesignerProperties.GetIsInDesignMode( this ) )
			{
				return;
			}
			FrameworkElementSettings listBoxSettings = new FrameworkElementSettings( this );
			listBoxSettings.Settings.Add( new DependencyPropertySetting( this, SelectedIndexProperty ) );
		} // SettingListBox

	} // class SettingListBox

} // namespace Itenso.Solutions.Community.ConfigurationWindowsDemo
// -- EOF -------------------------------------------------------------------
