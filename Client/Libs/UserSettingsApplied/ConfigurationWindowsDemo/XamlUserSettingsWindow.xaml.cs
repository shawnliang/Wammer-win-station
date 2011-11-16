// -- FILE ------------------------------------------------------------------
// name       : XamlUserSettingsWindow.cs
// created    : Jani Giannoudis - 2008.04.30
// language   : c#
// environment: .NET 3.0
// --------------------------------------------------------------------------
using System.Collections.ObjectModel;

namespace Waveface.Solutions.Community.ConfigurationWindowsDemo
{

	// ------------------------------------------------------------------------
	public partial class XamlUserSettingsWindow
	{

		// ----------------------------------------------------------------------
		public XamlUserSettingsWindow()
		{
			InitializeComponent();
		} // XamlUserSettingsWindow

		// ----------------------------------------------------------------------
		public ObservableCollection<Customer> Customers
		{
			get { return customers ?? ( customers = LoadCustomers() ); }
		} // Customers

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
		// members
		private ObservableCollection<Customer> customers;

	} // class XamlUserSettingsWindow

} // namespace Itenso.Solutions.Community.ConfigurationWindowsDemo
// -- EOF -------------------------------------------------------------------
