// -- FILE ------------------------------------------------------------------
// name       : ListViewSettingsWindow.xaml.cs
// created    : Jani Giannoudis - 2008.05.09
// language   : c#
// environment: .NET 3.0
// --------------------------------------------------------------------------
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Waveface.Configuration;

namespace Waveface.Solutions.Community.ConfigurationWindowsDemo
{

	// ------------------------------------------------------------------------
	public partial class ListViewSettingsWindow
	{

		// ----------------------------------------------------------------------
		public ListViewSettingsWindow()
		{
			InitializeComponent();
			WindowSettings.SaveOnClose = false; // disable auto-save
			WindowSettings.Settings.Add( new ListViewSetting( CustomerListView ) );
		} // ListViewSettingsWindow

		// ----------------------------------------------------------------------
		public ObservableCollection<Customer> Customers
		{
			get { return customers ?? ( customers = LoadCustomers() ); }
		} // Customers

		// ----------------------------------------------------------------------
		protected override void OnClosing( CancelEventArgs e )
		{
			base.OnClosing( e );

			if ( DialogResult == false )
			{
				return; // is canceling
			}

			if ( !WindowSettings.Settings.HasChanges )
			{
				return; // nothing to do
			}

			StringBuilder sb = new StringBuilder();
			foreach ( ISetting setting in WindowSettings.Settings )
			{
				if ( !setting.HasChanged )
				{
					continue;
				}

				sb.Append( " - " );
				sb.Append( setting.ToString() );
				sb.Append( "\n" );
			}

			MessageBoxResult result = MessageBox.Show( "Save changes?\n\n" + sb, Title, MessageBoxButton.YesNoCancel );
			switch ( result )
			{
				case MessageBoxResult.Yes:
					WindowSettings.Save();
					break;
				case MessageBoxResult.No:
					break;
				case MessageBoxResult.Cancel:
					DialogResult = null;
					e.Cancel = true;
					break;
			}
		} // OnClosing

		// ----------------------------------------------------------------------
		private static ObservableCollection<Customer> LoadCustomers()
		{
			ObservableCollection<Customer> customers = new ObservableCollection<Customer>();

			for ( int i = 0; i < 100; i++ )
			{
				string userId = ( i + 1 ).ToString();
				Customer customer = new Customer(
					"FisrtName" + userId,
					"LastName" + userId,
					"Street" + userId,
					"City" + userId,
					"ZipCode" + userId );

				customers.Add( customer );
			}

			return customers;
		} // LoadCustomers

		// ----------------------------------------------------------------------
		private void ButtonCancel( object sender, RoutedEventArgs e )
		{
			DialogResult = false;
		} // ButtonCancel

		// ----------------------------------------------------------------------
		private void ButtonOk( object sender, RoutedEventArgs e )
		{
			DialogResult = true;
		} // ButtonOk

		// ----------------------------------------------------------------------
		// members
		private ObservableCollection<Customer> customers;

	} // class ListViewSettingsWindow

} // namespace Itenso.Solutions.Community.ConfigurationWindowsDemo
// -- EOF -------------------------------------------------------------------
