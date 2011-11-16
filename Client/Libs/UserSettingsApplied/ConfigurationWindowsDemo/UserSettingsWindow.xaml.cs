// -- FILE ------------------------------------------------------------------
// name       : UserSettingsWindow.cs
// created    : Jani Giannoudis - 2008.04.28
// language   : c#
// environment: .NET 3.0
// --------------------------------------------------------------------------
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using Waveface.Configuration;

namespace Waveface.Solutions.Community.ConfigurationWindowsDemo
{

	// ------------------------------------------------------------------------
	public enum CustomAlignment
	{
		Left,
		Center,
		Right,
	} // enum CustomAlignment

	// ------------------------------------------------------------------------
	public partial class UserSettingsWindow
	{

		// ----------------------------------------------------------------------
		public static DependencyProperty WindowColorProperty;

		// ----------------------------------------------------------------------
		public UserSettingsWindow()
		{
			// create settings before InitializeComponent()
			windowSettings = new WindowSettings( this );
			windowSettings.SaveOnClose = false; // disable auto-save

			WindowColorProperty =
				DependencyProperty.Register(
				"WindowColor", typeof( Color ), typeof( UserSettingsWindow ),
				new FrameworkPropertyMetadata( Color.FromArgb( 255, 200, 255, 200 ), FrameworkPropertyMetadataOptions.AffectsRender ) );

			InitializeComponent();
			SetupListItems( 50 );

			UserConfigFileLabel.Text = ApplicationSettings.UserConfigurationFilePath;

			// add settings to group
			windowSettings.Settings.Add( 
				new DependencyPropertySetting( "ListColumn.Width", ListColumn, ColumnDefinition.WidthProperty, ListColumn.Width ) );
			windowSettings.Settings.Add( 
				new PropertySetting( "CustomAlignment", this, "Alignment", CustomAlignment.Left ) );
			windowSettings.Settings.Add(
				new DependencyPropertySetting( this, WindowColorProperty, WindowColor ) );
		} // UserSettingsWindow

		// ----------------------------------------------------------------------
		public string HostName
		{
			get { return HostEdit.Text; }
			set { HostEdit.Text = value; }
		} // HostName

		// ----------------------------------------------------------------------
		protected CustomAlignment Alignment
		{
			get
			{
				if ( AlignmentLeft.IsChecked != null && AlignmentLeft.IsChecked.Value )
				{
					return CustomAlignment.Left;
				}
				if ( AlignmentCenter.IsChecked != null && AlignmentCenter.IsChecked.Value )
				{
					return CustomAlignment.Center;
				}
				if ( AlignmentRight.IsChecked != null && AlignmentRight.IsChecked.Value )
				{
					return CustomAlignment.Right;
				}
				return CustomAlignment.Left;
			}
			set
			{
				switch ( value )
				{
					case CustomAlignment.Left:
						AlignmentLeft.IsChecked = true;
						break;
					case CustomAlignment.Center:
						AlignmentCenter.IsChecked = true;
						break;
					case CustomAlignment.Right:
						AlignmentRight.IsChecked = true;
						break;
				}
			}
		} // Alignment

		// ----------------------------------------------------------------------
		public Color WindowColor
		{
			get { return (Color)GetValue( WindowColorProperty ); }
			set { SetValue( WindowColorProperty, value ); }
		} // WindowColor

		// ----------------------------------------------------------------------
		private void SetupListItems( int count )
		{
			for ( int i = 0; i < count; i++ )
			{
				ItemsList.Items.Add( "Item " + ( ( i + 1 ) ) );
			}
			ItemsList.SelectedIndex = 0;
		} // SetupListItems

		// ----------------------------------------------------------------------
		private void ButtonChangeColor( object sender, RoutedEventArgs e )
		{
			ColorDialog colorDialog = new ColorDialog();
			colorDialog.Color = WindowColor;
			colorDialog.FullOpen = true;
			if ( colorDialog.ShowDialog() )
			{
				WindowColor = colorDialog.Color;
			}
		} // ButtonChangeColor

		// ----------------------------------------------------------------------
		private void ButtonResetSettings( object sender, RoutedEventArgs e )
		{
			windowSettings.Reset();
		} // ButtonResetSettings

		// ----------------------------------------------------------------------
		private void ButtonReloadSettings( object sender, RoutedEventArgs e )
		{
			windowSettings.Reload();
		} // ButtonReloadSettings

		// ----------------------------------------------------------------------
		private void ButtonSaveSettings( object sender, RoutedEventArgs e )
		{
			windowSettings.Save();
		} // ButtonSaveSettings

		// ----------------------------------------------------------------------
		private void ButtonOpenUserConfig( object sender, RoutedEventArgs e )
		{
			string fileName = UserConfigFileLabel.Text;
			if ( !File.Exists( fileName ) )
			{
				MessageBox.Show( "File not available:\n\n" + fileName, Title );
				return;
			}
			Process.Start( fileName );
		} // ButtonOpenUserConfig

		// ----------------------------------------------------------------------
		private void ButtonListView( object sender, RoutedEventArgs e )
		{
			ListViewSettingsWindow listViewSettingsWindow = new ListViewSettingsWindow();
			listViewSettingsWindow.ShowDialog();
		} // ButtonListView

		// ----------------------------------------------------------------------
		private void ButtonDerivedWindow( object sender, RoutedEventArgs e )
		{
			DerivedSettingsWindow derivedSettingsWindow = new DerivedSettingsWindow();
			derivedSettingsWindow.ShowDialog();
		} // ButtonDerivedWindow

		// ----------------------------------------------------------------------
		private void ButtonXamlSettings( object sender, RoutedEventArgs e )
		{
			XamlUserSettingsWindow xamlUserSettingsWindow = new XamlUserSettingsWindow();
			xamlUserSettingsWindow.ShowDialog();
		} // ButtonXamlSettings

		// ----------------------------------------------------------------------
		private void ButtonClose( object sender, RoutedEventArgs e )
		{
			if ( SaveAsDefaultOption.IsChecked != null && SaveAsDefaultOption.IsChecked.Value )
			{
				windowSettings.Save();
			}
			Close();
		} // ButtonClose

		// ----------------------------------------------------------------------
		// members
		private readonly WindowSettings windowSettings;

	} // class UserSettingsWindow

} // namespace Itenso.Solutions.Community.ConfigurationWindowsDemo
// -- EOF -------------------------------------------------------------------
