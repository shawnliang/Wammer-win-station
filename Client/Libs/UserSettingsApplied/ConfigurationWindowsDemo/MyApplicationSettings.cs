// -- FILE ------------------------------------------------------------------
// name       : MyApplicationSettings.cs
// created    : Jani Giannoudis - 2008.04.28
// language   : c#
// environment: .NET 3.0
// --------------------------------------------------------------------------
using System.Windows;
using Waveface.Configuration;

namespace Waveface.Solutions.Community.ConfigurationWindowsDemo
{

	// ------------------------------------------------------------------------
	public class MyApplicationSettings : WindowsApplicationSettings
	{

		// ----------------------------------------------------------------------
		public MyApplicationSettings( Application application ) :
			base( application, typeof( MyApplicationSettings ) )
		{
			Settings.Add( new FieldSetting( this, "hostName" ) );
		} // MyApplicationSettings

		// ----------------------------------------------------------------------
		public string HostName
		{
			get { return hostName; }
			set { hostName = value; }
		} // HostName

		// ----------------------------------------------------------------------
		// members
		private string hostName;

	} // class MyApplicationSettings

} // namespace Itenso.Solutions.Community.ConfigurationWindowsDemo
// -- EOF -------------------------------------------------------------------
